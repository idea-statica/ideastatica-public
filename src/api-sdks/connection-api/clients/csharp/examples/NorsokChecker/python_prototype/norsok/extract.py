# -*- coding: utf-8 -*-
"""Geometry extraction for the joint viewer.

Reads an .ideaCon via the Connection REST API and returns, per connection:
  - a NORSOK-style 2D projection (plane = chord axis x first-brace axis),
  - 3D member data (axis, origin, diameter) for a Three.js tube view to compare 1:1 with IDEA,
  - per-member meta (role, section, theta, beta),
  - non-blocking assumption diagnostics (warnings, not hard rejects yet).

Service lifecycle is managed by app.py; this module talks to a running service at BASE.
"""
import requests, math, os
import numpy as np
from .n64 import JointInput, check_joint, C_coeffs   # NORSOK 6.4 resistance engine (same package)

BASE = "http://localhost:5000/api/4"

# ---- assumption-check tolerances (NORSOK + our layer-1 policy) ----
COPLANAR_WARN_DEG = 5.0     # 0-5 OK silently; 5-15 warning
COPLANAR_MAX_DEG = 15.0     # NORSOK 6.4.2: ±15° = common plane; >15 = different plane (out of simple scope)
PARALLEL_MIN_THETA_DEG = 5.0  # brace nearly parallel to chord -> degenerate
OUT_OF_PLANE_OFFSET_MM = 5.0  # default: origin offset perpendicular to plane > this (mm) -> error. NORSOK has no value; UI-tunable.
ECC_ALONG_CHORD_FRAC = 0.25      # e > D/4 along chord -> warning (NORSOK/API D/4 gate)
# TWO independent, UI-tunable plane tolerances (decoupled 2026-06-28):
PLANE_FIT_TOL_DEG = 2.0      # RANSAC FIT: only braces within this angle BUILD the plane (strict; default 2°)
COPLANAR_EVAL_DEG = 15.0     # EVALUATION: after the plane is built, any member beyond this from it -> joint
                             # is multiplanar -> hide 2D / exclude from the simple-joint check (NORSOK ±15°)
# node-equilibrium residual classification (input forces are stored rounded -> a tiny residual is normal).
# INTERNAL CALC IS PURE SI: tolerances in N / N·m (UI shows kN/kNm). 1 kN = 1000 N, 1 kNm = 1000 N·m.
EQUILIBRIUM_TOL_FORCE_N = 1000.0   # |residual force|  <= this (N)   -> OK (balanced); above -> imbalance
EQUILIBRIUM_TOL_MOM_NM = 1000.0    # |residual moment| <= this (N·m) -> OK

# K/Y/X classification — "balanced to within 10%" gate (NORSOK Fig 6-2 text). NORSOK says a brace "should"
# be balanced to within 10% to be a pure K-joint -> "should", not "shall", so the gate is OPTIONAL and
# UI-tunable. DEFAULT 0.0 = no tolerance = always do the honest, continuous K/X/Y breakdown (conservative;
# the user chose 0% default). Set >0 (e.g. 0.10) to treat a nearly-balanced brace as 100% K and ignore the
# small unbalanced remainder, exactly per the NORSOK "within 10%" wording.
K_BALANCE_GATE = 0.0               # residual transverse force / total transverse force <= this -> 100% K

# ---------- vector helpers ----------
def dot(a, b): return a[0]*b[0]+a[1]*b[1]+a[2]*b[2]
def nrm(a): return math.sqrt(dot(a, a))
def cross(a, b): return [a[1]*b[2]-a[2]*b[1], a[2]*b[0]-a[0]*b[2], a[0]*b[1]-a[1]*b[0]]
def unit(a):
    n = nrm(a); return [a[0]/n, a[1]/n, a[2]/n] if n > 1e-12 else [0, 0, 0]
def vv(d): return [d['x'], d['y'], d['z']]
def scal(a, k): return [a[0]*k, a[1]*k, a[2]*k]
def add(a, b): return [a[0]+b[0], a[1]+b[1], a[2]+b[2]]
def sub(a, b): return [a[0]-b[0], a[1]-b[1], a[2]-b[2]]

def eff_dir_sign(m):
    """Sign to apply to a member's axisX to get its EFFECTIVE direction (the way the body extends
    FROM the joint node). Current API schema: isContinuous (bool) + connectedBy ("begin"/"end")
    replaced the old positionOnRefLine double (0.0/0.5/1.0) — isContinuous ~ old 0.5, connectedBy
    "end" ~ old 1.0, "begin" ~ old 0.0. A continuous member's own axisX orientation is arbitrary
    for this purpose (it's drawn/resolved both ways elsewhere), so it maps to the same -1 branch
    as "end"."""
    return -1.0 if (m.get("isContinuous") or m.get("connectedBy") == "end") else 1.0
def proj2d(p3, o3, ex, ey):
    r = sub(p3, o3); return [dot(r, ex), dot(r, ey)]


def parse_chs(name):
    """'CHS168.3/8.0' -> (168.3, 8.0) in mm; (None,None) if not a CHS name."""
    try:
        if "CHS" not in (name or "").upper():
            return None, None
        core = name.upper().replace("CHS", "").strip()
        d, t = core.split("/"); return float(d), float(t)
    except Exception:
        return None, None

# ---------- REST session / project ----------
def connect(session):
    cid = session.get(f"{BASE}/clients/connect-client", timeout=15).text.strip().strip('"')
    session.headers.update({"ClientId": cid})
    return cid

def open_project(session, ideacon):
    with open(ideacon, "rb") as f:
        r = session.post(f"{BASE}/projects/open",
                         files={"ideaConFile": (os.path.basename(ideacon), f, "application/octet-stream")},
                         timeout=120)
    r.raise_for_status()
    return r.json()["projectId"]

def list_connections(session, pid):
    return session.get(f"{BASE}/projects/{pid}/connections", timeout=30).json()

def get_members(session, pid, conn_id):
    return session.get(f"{BASE}/projects/{pid}/connections/{conn_id}/members", timeout=30).json()

def get_cross_sections(session, pid):
    return session.get(f"{BASE}/projects/{pid}/materials/cross-sections", timeout=30).json()

def get_load_effects(session, pid, conn_id):
    """ConLoadEffect[] = [{id,name,active,isPercentage, memberLoadings:[{memberId,position,sectionLoad:{n,vy,vz,mx,my,mz}}]}].
    Forces in N, moments in N·m, in each member's LOCAL CSYS at its inserted end (Begin/End).

    ALWAYS request ?isPercentage=false: a user may have entered the load effects as PERCENTAGE
    (each component = σ/f_y ratio, 0..1) rather than absolute forces. Without this flag the service
    returns them in whatever form they were saved, and our whole pipeline (equilibrium, plane
    transform, 6.4 checks) assumes N / N·m — percentage values (~0.003) would silently read as
    ~0.003 N and every check would collapse to util≈0. The service converts to absolute forces
    regardless of how the file stored them, so we cannot know what the user did — we force N here."""
    r = session.get(f"{BASE}/projects/{pid}/connections/{conn_id}/load-effects",
                    params={"isPercentage": "false"}, timeout=30)
    r.raise_for_status()
    return r.json()

def close_project(session, pid):
    try: session.get(f"{BASE}/projects/{pid}/close", timeout=30)
    except Exception: pass

def xs_map(xs):
    out = {}
    for c in xs:
        d, t = parse_chs(c.get("name", ""))
        # Material is inline on the cross-section: c["material"]["element"] is a MatSteel IOM object
        # with fy/fu ALREADY IN Pa (verified: "S 355" -> fy=355000000.0). fy40/fu40 apply for
        # 40mm < thickness <= 100mm. We pick fy by the wall thickness t (mm). 6.4 is materially
        # fy_CHORD only, but we store fy for every section (brace fy only matters for overlap Q_g / 6.3).
        fy = fu = mat_name = None
        el = ((c.get("material") or {}).get("element")) or {}
        if el:
            fy_thin, fu_thin = el.get("fy"), el.get("fu")
            fy_thick, fu_thick = el.get("fy40"), el.get("fu40")
            mat_name = el.get("name")
            # t is in mm here (from parse_chs); >40 mm -> use the fy40/fu40 band when present
            use_thick = (t is not None and t > 40.0 and fy_thick is not None)
            fy = (fy_thick if use_thick else fy_thin)
            fu = (fu_thick if use_thick else fu_thin)
        out[c["id"]] = {"name": c.get("name"), "D": d, "T": t,
                        "type": c.get("crossSectionType"), "isCHS": (d is not None),
                        "fy": fy, "fu": fu, "material": mat_name}   # fy/fu in Pa (SI), or None
    return out

# ---------- node-equilibrium self-check (verified force-reading recipe) ----------
def member_loading_global(m, section_load, node):
    """One member loading -> (F_glob[N], M_glob_about_node[N·m]) in GLOBAL coords.
    Recipe (verified vs IDEA unbalanced forces to 0.000, see ideastatica-member-forces skill):
      F_glob = n*axisX + vy*axisY + vz*axisZ
      M_sec  = mx*axisX + my*axisY + mz*axisZ            (API section moments are N·m)
      application point depends on model.forcesIn ("node"/None -> node-mode; "position" -> position-mode):
        node-mode: ended/brace -> origin (origin already embeds ecc);
                   continuous   -> projection of node onto the member axis line.
        position-mode (model.x = distance along the axis, default 0; x=0 == node): ecc is added on
                   BOTH sides here (unlike node-mode, where origin already carries it); ended
                   begin -> node + x*axisXn + ecc; ended end -> node - x*axisXn + ecc; continuous ->
                   ALWAYS node - x*axisXn + ecc (not mirrored — the one exception in this recipe).
      M_glob = M_sec + cross(r, F_glob),  r = application_point - node."""
    p = m["position"]
    ax, ay, az = vv(p["axisX"]), vv(p["axisY"]), vv(p["axisZ"])
    sl = section_load
    F = add(add(scal(ax, sl.get("n", 0.0)), scal(ay, sl.get("vy", 0.0))), scal(az, sl.get("vz", 0.0)))
    Msec = add(add(scal(ax, sl.get("mx", 0.0)), scal(ay, sl.get("my", 0.0))), scal(az, sl.get("mz", 0.0)))
    forces_in = (m.get("model") or {}).get("forcesIn")
    if forces_in == "position":
        axn = unit(ax)
        ecc = add(add(scal(ax, p.get("offsetEx", 0.0)), scal(ay, p.get("offsetEy", 0.0))), scal(az, p.get("offsetEz", 0.0)))
        x = (m.get("model") or {}).get("x", 0.0) or 0.0
        if m.get("isContinuous"):
            ap = add(add(node, scal(axn, -x)), ecc)                                    # always -axisX
        elif m.get("connectedBy") == "end":
            ap = add(add(node, scal(axn, -x)), ecc)
        else:
            ap = add(add(node, scal(axn, x)), ecc)
    elif m.get("isContinuous"):
        o = vv(p["origin"]); t = dot(sub(node, o), ax); ap = add(o, scal(ax, t))  # node projected on axis
    else:
        ap = vv(p["origin"])                                                       # origin already has ecc
    r = sub(ap, node)
    return F, add(Msec, cross(r, F))

def node_equilibrium(members, load_effects, node):
    """For each load effect, sum F and M over all member loadings -> residual (unbalanced) force/moment
    at the node. A balanced joint -> ~0. PLAIN SUM (no Begin/End sign flip — API forces are already
    'member action on node'). PURE SI output (N, N·m); the UI converts to kN/kNm.
    Returns [{id, name, sumF_N:[x,y,z], sumM_Nm:[x,y,z], resF_N, resM_Nm, ok}]."""
    mbyid = {m["id"]: m for m in members}
    out = []
    for le in (load_effects or []):
        SF, SM = [0.0, 0.0, 0.0], [0.0, 0.0, 0.0]
        for ml in le.get("memberLoadings", []):
            m = mbyid.get(ml.get("memberId"))
            if m is None:
                continue
            F, M = member_loading_global(m, ml.get("sectionLoad", {}), node)
            SF, SM = add(SF, F), add(SM, M)
        resF, resM = nrm(SF), nrm(SM)   # SI: N, N·m
        out.append({"id": le.get("id"), "name": le.get("name"),
                    "sumF_N": SF, "sumM_Nm": SM,
                    "resF_N": resF, "resM_Nm": resM,
                    # a small residual is expected (rounded input). ok = within SI tolerance.
                    "ok": (resF <= EQUILIBRIUM_TOL_FORCE_N and resM <= EQUILIBRIUM_TOL_MOM_NM)})
    return out

# ---------- STEP 2: brace forces resolved into the joint plane (for the 6.4 check) ----------
def brace_subplane_normal(m, ex, n_plane):
    """The brace's OWN sub-plane normal n_b = unit(ex × brace_dir), FLIPPED so dot(n_b, n_plane) >= 0
    (every brace's sub-normal points to the SAME side of the joint-check plane, so M_ip carries a
    CONSISTENT sign across braces). Degenerate (brace parallel to chord) falls back to n_plane.

    Used by BOTH brace_force_in_plane (resolving brace section forces) and chord_stresses_all_les
    (resolving the chord's own stress at that brace's footprint) — they must agree on this frame, or
    a brace's M_ip and the chord bending used for its own Qf would silently disagree. Factored out
    2026-07-01 after an audit flagged the two call sites as verbatim-duplicated (a future tweak to the
    degenerate-fallback threshold or the flip rule could otherwise drift between them unnoticed)."""
    ax = unit(vv(m["position"]["axisX"]))
    bx = unit(scal(ax, eff_dir_sign(m)))
    n_b = cross(ex, bx)
    if nrm(n_b) < 1e-9:
        # brace parallel to chord (degenerate) — sub-plane undefined; fall back to the joint normal
        n_b = list(n_plane)
    n_b = unit(n_b)
    if dot(n_b, n_plane) < 0:          # unify orientation across all braces
        n_b = scal(n_b, -1.0)
    return bx, n_b


def brace_force_in_plane(m, section_load, ex, n_plane):
    """Resolve ONE brace loading into NORSOK joint-check components, in the brace's OWN sub-plane
    (chord axis × this brace) — see the decisions below. Returns a dict with N_Sd, M_ip, M_op (the
    three quantities eq 6.57 needs) plus the diagnostic split (torsion, shears, sub-normal).

    Frame for THIS brace:
      bx  = brace axis (effective direction from the node into the body; +N_Sd = tension, pulls out)
      n_b = unit(ex × brace_dir) = normal of the brace's OWN sub-plane (chord axis, this brace).
            FLIPPED so dot(n_b, n_plane) >= 0 -> every brace's sub-normal points to the SAME side of
            the joint-check plane, so M_ip carries a CONSISTENT sign across braces (needed for the
            in-plane force/moment balance in classification; eq 6.57 itself uses |·|).
      ip  = in-plane transverse axis = unit(n_b × bx)  (lies in the sub-plane, perpendicular to bx).

    Forces (section forces M_sec, per the user's choice — NO r×F transfer to the node; for a brace
    without eccentricity this equals the at-node value anyway):
      F = n*axisX + vy*axisY + vz*axisZ ; M = mx*axisX + my*axisY + mz*axisZ   (N, N·m)
      N_Sd  = dot(F, bx)              axial (+tension)
      V_ip  = dot(F, ip)             in-plane shear
      V_op  = dot(F, n_b)            out-of-plane shear
      M_tor = dot(M, bx)             torsion about the brace axis (NOT used by 6.57)
      M_ip  = dot(M, n_b)            in-plane bending (vector along the sub-plane normal)
      M_op  = dot(M, ip)             out-of-plane bending (vector along the in-plane transverse axis)
    The bending vector (M minus its torsion part) splits exactly into M_ip*n_b + M_op*ip."""
    p = m["position"]
    ax, ay, az = vv(p["axisX"]), vv(p["axisY"]), vv(p["axisZ"])
    sl = section_load
    F = add(add(scal(ax, sl.get("n", 0.0)), scal(ay, sl.get("vy", 0.0))), scal(az, sl.get("vz", 0.0)))
    M = add(add(scal(ax, sl.get("mx", 0.0)), scal(ay, sl.get("my", 0.0))), scal(az, sl.get("mz", 0.0)))

    bx, n_b = brace_subplane_normal(m, ex, n_plane)
    ip = unit(cross(n_b, bx))          # in-plane transverse axis (in the sub-plane, perp to bx)

    return {
        "N_Sd": dot(F, bx),            # N, +tension
        "V_ip": dot(F, ip),            # N
        "V_op": dot(F, n_b),           # N
        "M_ip": dot(M, n_b),           # N·m, in-plane bending
        "M_op": dot(M, ip),            # N·m, out-of-plane bending
        "M_tor": dot(M, bx),           # N·m, torsion (excluded from 6.57)
        "sub_normal_dot": dot(n_b, n_plane),   # >=0 by construction (sanity: ~1 if brace in-plane)
    }

def brace_forces_in_plane(braces, load_effects, ex, n_plane):
    """For every load effect × every brace, resolve the brace section forces into joint-plane
    components. Returns, in PURE SI (forces N, moments N·m — the internal-calc convention; the UI
    converts to kN/kNm at render time):
        [{id, name, active, braces:[{name, N_Sd, M_ip, M_op, V_ip, V_op, M_tor, sub_normal_dot}]}]
    A brace missing from a LE -> zeros."""
    bbyid = {b["id"]: b for b in braces}
    out = []
    for le in (load_effects or []):
        loads_by_member = {ml.get("memberId"): ml.get("sectionLoad", {})
                           for ml in le.get("memberLoadings", [])}
        rows = []
        for b in braces:
            sl = loads_by_member.get(b["id"], {})
            c = brace_force_in_plane(b, sl, ex, n_plane)   # already pure SI
            c["name"] = b.get("name")
            rows.append(c)
        out.append({"id": le.get("id"), "name": le.get("name"),
                    "active": le.get("active", True), "braces": rows})
    return out

# ---------- STEP 4 prep: chord stresses at each brace footprint, for the Qf chord-action factor ----------
def _chord_avg_load(chord, load_effect):
    """Average chord internal force/moment vector at the joint, in GLOBAL coords, for ONE load effect.
    NORSOK p.31 (verified): 'The average of the chord loads and bending moments on either side of the
    brace intersection should be used in Equations (6.54) and (6.55).' A continuous chord carries TWO
    loadings in the LE (Begin AND End) = the two sides of the intersection -> average them. (If only one
    side is present, use it as-is.) Returns (F_avg[N], M_avg[N·m], sides) about the chord member's OWN
    section axes mapped to global via axisX/Y/Z. Moments are SECTION moments (no r×F transfer — Qf wants
    the chord section forces at the joint, not a moment about the node).
    `sides` = [{F[N], M[N·m]}, ...] one entry per raw sectionLoad (Begin/End) BEFORE averaging — kept for
    transparency (so the UI can show each side individually, then the average, then the derived stress)."""
    p = chord["position"]
    ax, ay, az = vv(p["axisX"]), vv(p["axisY"]), vv(p["axisZ"])
    sls = [ml.get("sectionLoad", {}) for ml in (load_effect.get("memberLoadings", []) or [])
           if ml.get("memberId") == chord["id"]]
    if not sls:
        return [0.0, 0.0, 0.0], [0.0, 0.0, 0.0], []
    sides = []
    F = [0.0, 0.0, 0.0]; M = [0.0, 0.0, 0.0]
    for sl in sls:
        Fi = add(add(scal(ax, sl.get("n", 0.0)), scal(ay, sl.get("vy", 0.0))), scal(az, sl.get("vz", 0.0)))
        Mi = add(add(scal(ax, sl.get("mx", 0.0)), scal(ay, sl.get("my", 0.0))), scal(az, sl.get("mz", 0.0)))
        sides.append({"F": Fi, "M": Mi})
        F = add(F, Fi); M = add(M, Mi)
    n = len(sls)
    return scal(F, 1.0 / n), scal(M, 1.0 / n), sides


def chord_stress_at_brace(F_avg, M_avg, ex, sec_c, n_b, side):
    """Chord nominal stresses at ONE brace footprint, in the NORSOK Qf sign convention (PURE SI, Pa).

    Inputs:
      F_avg, M_avg = averaged chord force[N] / section-moment[N·m] (both global), from _chord_avg_load.
      ex   = chord axis (unit, global).
      sec_c= chord section dict (D, T in mm) -> we compute A, I in SI.
      n_b  = the brace's in-plane sub-plane normal (unit, global) — SAME vector used for the brace's M_ip,
             so 'in-plane bending of the chord' is measured in the same plane as the brace sits.
      side = +1 / -1 : which chord face the brace foot is on (in the +n_b / -n_b direction).

    Stresses (chord thickness at the joint is used, per p.31):
      A   = pi/4 (D^2 - di^2),  I = pi/64 (D^4 - di^4),  di = D - 2T   (chord section, SI)
      sigma_a  = N_chord / A     with N_chord = dot(F_avg, ex)        [NORSOK: + in TENSION]
      sigma_my = M_ip * z_ip / I  evaluated at the FOOTPRINT fibre:
                 M_ip = dot(M_avg, n_b)  (in-plane chord bending, signed)
                 z_ip = side * R         (R = D/2; oriented distance of the foot fibre from the centroid)
                 -> raw fibre stress is +tension. NORSOK wants sigma_my POSITIVE IN COMPRESSION at the
                    footprint, so we FLIP the sign:  sigma_my = -(M_ip * z_ip / I).
      sigma_mz = M_op * z / I   out-of-plane chord bending; only enters Qf via A^2 (squared) so its sign
                 is irrelevant -> report the magnitude-correct value at the same foot fibre.
                 M_op = dot(M_avg, ip),  ip = unit(n_b x ex)  (out-of-plane transverse axis).

    Returns {sigma_a, sigma_my, sigma_mz, A, I, R, N_chord, M_ip, M_op, side} (SI/Pa) — the intermediate
    A/I/R/N_chord/M_ip/M_op/side are echoed for the detailed-check modal's transparent derivation, not
    used further in this function. All zero if the section has no D/T."""
    D_mm, T_mm = sec_c.get("D"), sec_c.get("T")
    if not (D_mm and T_mm):
        return {"sigma_a": 0.0, "sigma_my": 0.0, "sigma_mz": 0.0,
                "A": 0.0, "I": 0.0, "R": 0.0, "N_chord": 0.0, "M_ip": 0.0, "M_op": 0.0, "side": side}
    D = D_mm * 1e-3; T = T_mm * 1e-3; di = D - 2 * T; R = D / 2.0
    from math import pi
    A = pi / 4.0 * (D * D - di * di)
    I = pi / 64.0 * (D ** 4 - di ** 4)
    N_chord = dot(F_avg, ex)
    sigma_a = N_chord / A if A > 0 else 0.0           # + tension (NORSOK convention for sigma_a)
    # in-plane chord bending at the foot fibre
    M_ip = dot(M_avg, n_b)
    z_ip = side * R
    sigma_my_fibre = (M_ip * z_ip / I) if I > 0 else 0.0   # +tension by mechanics
    sigma_my = -sigma_my_fibre                         # NORSOK: + for COMPRESSION in the footprint
    # out-of-plane chord bending (sign irrelevant for Qf; squared in A^2)
    ip = unit(cross(n_b, ex)) if nrm(cross(n_b, ex)) > 1e-9 else [0.0, 0.0, 0.0]
    M_op = dot(M_avg, ip)
    sigma_mz = (M_op * R / I) if I > 0 else 0.0
    return {"sigma_a": sigma_a, "sigma_my": sigma_my, "sigma_mz": sigma_mz,
            "A": A, "I": I, "R": R, "N_chord": N_chord, "M_ip": M_ip, "M_op": M_op, "side": side}


def chord_stresses_all_les(chord, braces, load_effects, ex, n_plane, sec_c, side_by_name):
    """Per LE × per brace: chord stresses (sigma_a/my/mz, Pa) at that brace's footprint for Qf.
    Uses brace_subplane_normal — the SAME frame brace_force_in_plane uses — so chord 'in-plane' bending
    matches the brace's M_ip plane. side_by_name = {brace name -> +1/-1 chord face} (from brace_side(),
    passed in because brace_side is a nested helper of build_connection).
    Returns [{id, name, braces:[{name, sigma_a, sigma_my, sigma_mz, A, I, R, N_chord, M_ip, M_op, side,
             chord_side}]}] — chord_side = [{N_chord, M_ip, M_op}, ...] one entry PER RAW chord sectionLoad
    (Begin/End) projected onto THIS brace's frame, for the "both sides -> average" transparency table."""
    out = []
    for le in (load_effects or []):
        F_avg, M_avg, sides = _chord_avg_load(chord, le)
        rows = []
        for b in braces:
            # rebuild this brace's sub-plane normal n_b (unified to n_plane), exactly as STEP 2 does —
            # SAME helper as brace_force_in_plane, so the two can never drift apart (see brace_subplane_normal).
            bx, n_b = brace_subplane_normal(b, ex, n_plane)
            side = side_by_name.get(b.get("name"), 1)   # +1 / -1 chord face
            st = chord_stress_at_brace(F_avg, M_avg, ex, sec_c, n_b, side)
            st["name"] = b.get("name")
            # per-side (Begin/End) raw N/M projected onto this brace's frame, BEFORE averaging
            ip_axis = unit(cross(n_b, ex)) if nrm(cross(n_b, ex)) > 1e-9 else [0.0, 0.0, 0.0]
            st["chord_side"] = [{"N_chord": dot(s["F"], ex), "M_ip": dot(s["M"], n_b),
                                  "M_op": dot(s["M"], ip_axis)} for s in sides]
            rows.append(st)
        out.append({"id": le.get("id"), "name": le.get("name"), "braces": rows})
    return out

# ---------- STEP 4: NORSOK 6.4 resistance check, per brace per LE ----------
def _to_py(o):
    """Recursively convert numpy scalars (np.bool_/np.floating/np.integer) to plain Python types so the
    result is JSON-serialisable (app.py serialises build_for output straight to the UI). Some upstream
    geometry comes from numpy, so a numpy bool can sneak into JointResult.validity etc."""
    if isinstance(o, dict):
        return {k: _to_py(v) for k, v in o.items()}
    if isinstance(o, (list, tuple)):
        return [_to_py(v) for v in o]
    if isinstance(o, np.generic):
        return o.item()
    return o


def joint_checks_all_les(chord_sec, brace_secs, params_by_name, brace_forces, classification,
                         chord_stresses, gamma_M=1.15):
    """Run the NORSOK 6.4 simple-joint check (n64.check_joint) for EVERY brace in EVERY load effect.

    6.4 checks the CHORD WALL at each brace footprint -> one check per brace (NOT per chord). It is
    materially fy_CHORD only; the brace fy enters only the overlap Q_g branch. Each brace's classification
    (frK/frX/frY + per-gap K_components from STEP 3) drives the weighted resistance; the chord stresses
    from STEP 4 prep drive Qf.

    Inputs (all SI from the upstream steps):
      chord_sec       = data['chord']['section']  {D,T(mm), fy(Pa)}
      brace_secs      = {brace name -> section {D,T(mm), fy(Pa)}}
      params_by_name  = {brace name -> {beta, gamma, tau, theta_deg}}
      brace_forces    = STEP 2 output [{id,name,braces:[{name,N_Sd,M_ip,M_op,...}]}]
      classification  = STEP 3 output [{id,name,classes:[{name,frK,frX,frY,K_components:[{gap_m,frac}],...}]}]
      chord_stresses  = STEP 4-prep output [{id,name,braces:[{name,sigma_a,sigma_my,sigma_mz}]}]

    Returns [{id, name, braces:[{name, util, passed, N_Rd_weighted, util_axial, util_ipb, util_opb,
              within_range, Qf, governing_note, K_terms:[{frK,g_m,Q_g,N_Rd}], skipped, reason}]}].
    A brace is skipped (no check) when section D/T or fy is missing, or geometry is out of nothing-to-do."""
    D_mm, T_mm = chord_sec.get("D"), chord_sec.get("T")
    fy_chord = chord_sec.get("fy")
    # index classification + stresses by LE id then brace name
    cls_by_le = {le.get("id"): {c["name"]: c for c in le.get("classes", [])} for le in (classification or [])}
    st_by_le = {le.get("id"): {b["name"]: b for b in le.get("braces", [])} for le in (chord_stresses or [])}
    out = []
    for le in (brace_forces or []):
        leid = le.get("id")
        cls_map = cls_by_le.get(leid, {})
        st_map = st_by_le.get(leid, {})
        rows = []
        for bf in le.get("braces", []):
            nm = bf.get("name")
            bsec = brace_secs.get(nm, {})
            par = params_by_name.get(nm, {})
            cl = cls_map.get(nm)
            st = st_map.get(nm, {})
            d_mm, t_mm = bsec.get("D"), bsec.get("T")
            fy_brace = bsec.get("fy")
            theta_deg = par.get("theta_deg")
            # guard: need full chord+brace geometry, chord material, and a classification
            if not (D_mm and T_mm and fy_chord and d_mm and t_mm and theta_deg and cl):
                rows.append({"name": nm, "skipped": True,
                             "reason": "missing section/material/classification data"})
                continue
            # A brace with no axial classification (frK=frY=frX=0, e.g. N_Sd≈0 -> "no transverse
            # force") has no chord-wall axial resistance to divide by -> the axial term of eq. (6.57)
            # is simply absent (n64.check_joint already gives it 0, see wN==0 guard there). That does
            # NOT make the whole 6.4 check inapplicable: Table 6-3's bending Qu and Table 6-4's moment
            # row are the SAME for every joint class (see M_Rd_ip/M_Rd_op in n64.JointResult, "shared
            # across classes") — a pure-moment brace still has a real bending check to run. Only skip
            # here when there is truly nothing to check at all (no axial classification AND no bending
            # load either).
            fr_sum = (cl.get("frK", 0.0) or 0.0) + (cl.get("frY", 0.0) or 0.0) + (cl.get("frX", 0.0) or 0.0)
            has_moment = abs(bf.get("M_ip", 0.0) or 0.0) > 1e-9 or abs(bf.get("M_op", 0.0) or 0.0) > 1e-9
            if fr_sum <= 1e-9 and not has_moment:
                rows.append({"name": nm, "skipped": True,
                             "reason": cl.get("note") or "no axial force to classify (K/Y/X = 0) and no bending load"})
                continue
            # K balancing components -> [(frac_of_axial, gap_m)]; gaps that came back None -> 0 (touching)
            K_components = [(kc.get("frac", 0.0), (kc.get("gap_m") or 0.0))
                           for kc in (cl.get("K_components") or [])]
            # representative single gap for reporting (Q_g) = the first K gap, else 0
            g_rep = K_components[0][1] if K_components else 0.0
            inp = JointInput.from_SI(
                D=D_mm * 1e-3, T=T_mm * 1e-3, fy_chord=fy_chord,
                d=d_mm * 1e-3, t=t_mm * 1e-3, fy_brace=(fy_brace or fy_chord),
                theta_deg=theta_deg, g=g_rep,
                frK=cl.get("frK", 0.0), frY=cl.get("frY", 0.0), frX=cl.get("frX", 0.0),
                K_components=(K_components or None),
                N_Sd=bf.get("N_Sd", 0.0), M_ip_Sd=bf.get("M_ip", 0.0), M_op_Sd=bf.get("M_op", 0.0),
                sigma_a_Sd=st.get("sigma_a", 0.0), sigma_my_Sd=st.get("sigma_my", 0.0),
                sigma_mz_Sd=st.get("sigma_mz", 0.0),
                gamma_M=gamma_M,
            )
            r = check_joint(inp)
            # representative Qf = the dominant class's axial Qf (for display); the weighted util already
            # blends per-class. Pick the class with the largest fraction.
            frac = {"K": cl.get("frK", 0.0), "Y": cl.get("frY", 0.0), "X": cl.get("frX", 0.0)}
            dom = max(frac, key=frac.get) if max(frac.values()) > 1e-9 else "K"
            domR = r.per_class.get(dom)
            C_mo = C_coeffs("K", "moment", r.beta)
            base_ax = fy_chord * (T_mm * 1e-3) ** 2 / (gamma_M * r.sin_theta)     # f_y*T^2/(gamma_M*sinTheta)
            # FULL per-class breakdown (K/Y/X), each with its OWN Table 6-4 coeffs -> OWN Qf,axial ->
            # OWN Qu,axial -> OWN N_Rd — Table 6-4 gives K/T-Y/X each a different (C1,C2,C3) row, so their
            # Qf,axial genuinely differ (only the MOMENT coeffs/Qf are shared across classes, see C_mo
            # above). A class with fr==0 is still computed here (cheap) — the UI decides whether to show
            # its block (user: "pokud některý z modů do posudku nevstupuje, blok se vynechá").
            per_class_out = {}
            for cls in ("K", "Y", "X"):
                cR = r.per_class[cls]
                C_ax_c = C_coeffs(cls, "axial-tension" if r.load_axial == "tension" else "axial-compression",
                                   r.beta)
                per_class_out[cls] = {
                    "fr": frac[cls], "Qu_axial": cR.Qu_axial, "Qf_axial": cR.Qf_axial,
                    "Qf_axial_A2": cR.Qf_axial_A2, "N_Rd": cR.N_Rd,
                    "C_axial": {"C1": C_ax_c[0], "C2": C_ax_c[1], "C3": C_ax_c[2], "note": C_ax_c[3]},
                }
            # K per-gap: each component shares the K class's Qf,axial (Table 6-4 depends on CLASS, not
            # gap) but has its OWN Q_g(g_i) -> OWN Qu,axial(g_i) -> OWN N_Rd(g_i) (r.K_terms already has
            # frK_i/g_i/Q_g_i/N_Rd_i from n64.check_joint; just attach the shared K coeffs/Qf here).
            K_per_gap = [{**t, "Qf_axial": per_class_out["K"]["Qf_axial"],
                          "C_axial": per_class_out["K"]["C_axial"]} for t in r.K_terms]
            rows.append({
                "name": nm, "skipped": False,
                "util": r.util_weighted, "passed": r.passed,
                "chord_overstressed": r.chord_overstressed,   # forced FAIL: Qf (eq 6.54) collapsed to <=0
                # True when this brace has no K/Y/X classification (frK=frY=frX=0, e.g. N_Sd≈0) — the
                # axial term of eq. (6.57) is then structurally absent (not just numerically 0), while
                # the bending check (M_Rd_ip/M_Rd_op, shared across classes) still ran normally above.
                "no_axial_classification": fr_sum <= 1e-9,
                "N_Rd_weighted": r.N_Rd_weighted,     # N
                "util_axial": domR.util_axial_term if domR else None,
                "util_ipb": domR.util_ip_term if domR else None,
                "util_opb": domR.util_op_term if domR else None,
                "M_Rd_ip": r.M_Rd_ip, "M_Rd_op": r.M_Rd_op,   # N·m
                "within_range": r.within_range, "validity": r.validity,
                "Qf": (domR.Qf_axial if domR else None),
                "Qf_axial_A2": (domR.Qf_axial_A2 if domR else None),
                "Qu_axial_dom": (domR.Qu_axial if domR else None),
                "N_Rd_dom": (domR.N_Rd if domR else None),
                "dom_class": dom,
                "K_terms": r.K_terms,                 # [{frK,g_m,Q_g,N_Rd}] per balancing gap (unchanged)
                "K_per_gap": K_per_gap,                # same, + Qf_axial/C_axial (K class, shared)
                "per_class": per_class_out,            # {"K":{...},"Y":{...},"X":{...}} — ALL classes
                "load_axial": r.load_axial,
                "Q_beta": r.Q_beta, "Q_g": r.Q_g, "gD": r.gD,
                "base_ax": base_ax,                   # f_y*T^2/(gamma_M*sinTheta)  [N]
                "C_moment": {"C1": C_mo[0], "C2": C_mo[1], "C3": C_mo[2], "note": C_mo[3]},
                # inputs echoed for the detailed-check modal (SI: forces N, moments N·m, angles deg)
                "inputs": {
                    "D": D_mm * 1e-3, "T": T_mm * 1e-3, "d": d_mm * 1e-3, "t": t_mm * 1e-3,
                    "fy_chord": fy_chord, "fy_brace": (fy_brace or fy_chord),
                    "theta_deg": theta_deg,
                    "beta": r.beta, "gamma": r.gamma, "tau": r.tau, "sin_theta": r.sin_theta,
                    "N_Sd": bf.get("N_Sd", 0.0), "M_ip_Sd": bf.get("M_ip", 0.0),
                    "M_op_Sd": bf.get("M_op", 0.0),
                    "sigma_a": st.get("sigma_a", 0.0), "sigma_my": st.get("sigma_my", 0.0),
                    "sigma_mz": st.get("sigma_mz", 0.0),
                    "frK": cl.get("frK", 0.0), "frY": cl.get("frY", 0.0), "frX": cl.get("frX", 0.0),
                    "gamma_M": gamma_M,
                },
                # chord-stress derivation trail (raw sides -> average -> section props -> sigma), echoed
                # for the modal's "derivation of stresses" block. See chord_stress_at_brace/chord_side.
                "chord_deriv": {
                    "A": st.get("A"), "I": st.get("I"), "R": st.get("R"), "side": st.get("side"),
                    "N_chord": st.get("N_chord"), "M_ip_chord": st.get("M_ip"), "M_op_chord": st.get("M_op"),
                    "chord_side": st.get("chord_side", []),  # per raw sectionLoad, BEFORE averaging
                },
                "Qu_ipb": r.Qu_ipb, "Qu_opb": r.Qu_opb, "Qf_moment": r.Qf_moment,
                "Qf_moment_A2": r.Qf_moment_A2,
                "M_ip_Sd": bf.get("M_ip", 0.0), "M_op_Sd": bf.get("M_op", 0.0),  # N·m (for the modal)
            })
        out.append({"id": leid, "name": le.get("name"), "braces": rows})
    return _to_py(out)   # strip numpy scalars so the dict is JSON-serialisable for the UI

# ---------- STEP 3: K/Y/X joint classification (per LE, per brace) ----------
def classify_kyx(braces_geom, gaps, gate=K_BALANCE_GATE):
    """Decompose each brace's axial force into K / X / Y components, per NORSOK Comm. 6.4.2.

    INPUT (all already computed upstream, in SI):
      braces_geom = [{name, side(+1/-1), theta_deg, N_Sd}] for ONE load effect.
        side  = which chord face the brace sits on (extract.brace_side; +1=+ey face, -1=-ey).
        theta = angle to the chord axis (deg); the TRANSVERSE (perpendicular-to-chord) force is N_Sd*sinθ.
        N_Sd  = axial force (+tension = pulls away from the node).
      gaps = extract.compute_gaps output [{between:[a,b], gap_m, side, adjacent}] — used to attach the
             right toe-to-toe gap to each K pair (so Layer 2 can degrade K->Y via Q_g for a large gap).
      gate = "balanced to within X" tolerance (fraction, 0..1). 0 = honest continuous breakdown
             (default, conservative); 0.10 = the NORSOK "within 10%" pure-K shortcut.

    METHOD (per brace b, the one being classified):
      1. TRANSVERSE force with sign: q_i = N_Sd_i * sin(theta_i) * side_i, where the sign convention is
         "+ = the transverse component points AWAY from the chord on that brace's own face". Two braces on
         the SAME face balance each other only when their q have OPPOSITE signs (one pushes the chord wall,
         the other pulls it) -> that mutual cancellation IS K. Same sign -> they do NOT balance (both load
         the chord the same way) -> that shared part goes into the chord (Y) or through it (X).
      2. K = the part of |q_b| cancelled by SAME-SIDE partners with OPPOSITE-sign q. Greedy: pair against
         the nearest such partner first (smallest toe-to-toe gap), then the next, until either q_b is fully
         cancelled or no opposite-sign same-side capacity is left. Each pairing yields a frK component with
         its own gap (-> own Q_g in Layer 2). This realises the case (e) "gap1 vs gap2 weighted average".
      3. X = of the REMAINING |q_b| (not cancelled same-side), the part carried THROUGH the chord, i.e.
         balanced by ANY brace on the OPPOSITE side (NORSOK: opposite-side balance = X; NO coaxiality
         needed — Fig 6-2 case (h)). Capacity = total opposite-side transverse force available.
      4. Y = whatever is still left (reacted as beam shear in the chord).
      5. GATE: if the leftover (X+Y, i.e. everything not cancelled same-side) <= gate*|q_b|, treat the brace
         as 100% K (the small remainder is ignored, per the NORSOK "within 10%" wording).

    The K/X/Y fractions are of the AXIAL force N_Sd (so Layer 2 can scale each part of N_Sd into its own
    6.4 check); they are derived from the transverse balance but reported as fractions of N_Sd, since K, X
    and Y of the same axial force partition it (frK+frX+frY = 1 for a loaded brace).

    Returns [{name, N_Sd, M_ip_Sd, M_op_Sd, q_trans, frK, frX, frY,
              K_components:[{partner, gap_m, frac}], note}].
    """
    EPS = 1e-9
    # transverse (perpendicular-to-chord) force WITH sign, per brace
    info = {}
    for b in braces_geom:
        th = math.radians(b.get("theta_deg") or 0.0)
        q = (b.get("N_Sd") or 0.0) * math.sin(th) * (1.0 if b.get("side", 1) >= 0 else -1.0)
        info[b["name"]] = {"q": q, "side": b.get("side", 1), "N_Sd": b.get("N_Sd") or 0.0,
                           "theta_deg": b.get("theta_deg") or 0.0,
                           "M_ip_Sd": b.get("M_ip", 0.0), "M_op_Sd": b.get("M_op", 0.0)}

    # gap lookup: toe-to-toe gap between two same-face braces (by unordered name pair)
    gap_of = {}
    for g in (gaps or []):
        a, c = g["between"]
        gap_of[frozenset((a, c))] = g.get("gap_m")

    out = []
    for b in braces_geom:
        nm = b["name"]
        me = info[nm]
        q_b = me["q"]
        absq = abs(q_b)
        if absq < EPS:
            # unloaded (or purely axial-along-chord) brace -> no transverse force to classify
            out.append({"name": nm, "N_Sd": me["N_Sd"], "M_ip_Sd": me["M_ip_Sd"], "M_op_Sd": me["M_op_Sd"],
                        "q_trans": q_b,
                        "frK": 0.0, "frX": 0.0, "frY": 0.0, "K_components": [], "note": "no transverse force"})
            continue

        # --- 1+2: K = same-side, opposite-sign partners, nearest gap first ---
        same_side_opp = []   # (gap_m_or_inf, partner_name, available_abs_q)
        for other in braces_geom:
            onm = other["name"]
            if onm == nm:
                continue
            o = info[onm]
            if o["side"] != me["side"]:
                continue                     # K requires SAME side
            if o["q"] * q_b >= 0:
                continue                     # K requires OPPOSITE sign (mutual cancellation)
            gm = gap_of.get(frozenset((nm, onm)))
            same_side_opp.append((gm if gm is not None else float("inf"), onm, abs(o["q"])))
        same_side_opp.sort(key=lambda t: t[0])   # nearest (smallest gap) first

        remaining = absq
        K_components = []
        for gm, onm, avail in same_side_opp:
            if remaining <= EPS:
                break
            take = min(remaining, avail)
            if take <= EPS:
                continue
            K_components.append({"partner": onm, "gap_m": (None if gm == float("inf") else gm),
                                 "q": take, "frac": take / absq})
            remaining -= take
        balanced_K = absq - remaining       # total transverse force cancelled same-side
        leftover = remaining                # not cancelled same-side -> goes to X then Y

        # --- 5: 10% (or gate) shortcut -> treat as pure K ---
        if leftover <= gate * absq + EPS and balanced_K > EPS:
            frac_scale = absq / balanced_K if balanced_K > EPS else 0.0
            for kc in K_components:
                kc["frac"] *= frac_scale    # rescale so frK sums to 1 (remainder folded into K)
            out.append({"name": nm, "N_Sd": me["N_Sd"], "M_ip_Sd": me["M_ip_Sd"], "M_op_Sd": me["M_op_Sd"],
                        "q_trans": q_b,
                        "frK": 1.0, "frX": 0.0, "frY": 0.0, "K_components": K_components,
                        "note": ("balanced to %.1f%% <= gate %.0f%% -> 100%% K"
                                 % (100.0*leftover/absq, 100.0*gate))})
            continue

        # --- 3: X = remaining balanced by OPPOSITE-side braces (through the chord, no coaxiality) ---
        opp_capacity = sum(abs(info[o["name"]]["q"]) for o in braces_geom
                           if o["name"] != nm and info[o["name"]]["side"] != me["side"])
        X = min(leftover, opp_capacity)
        Y = leftover - X                    # 4: whatever is still left -> beam shear in the chord

        frK = balanced_K / absq
        frX = X / absq
        frY = Y / absq
        note = []
        if frK > EPS and not K_components:
            note.append("K with no gap data")
        out.append({"name": nm, "N_Sd": me["N_Sd"], "M_ip_Sd": me["M_ip_Sd"], "M_op_Sd": me["M_op_Sd"],
                    "q_trans": q_b,
                    "frK": frK, "frX": frX, "frY": frY,
                    "K_components": K_components, "note": "; ".join(note)})
    return out

def classify_kyx_all_les(braces_geom_by_le, gaps, gate=K_BALANCE_GATE):
    """Run classify_kyx for every load effect. braces_geom_by_le = [{id, name, braces_geom:[...]}].
    Returns [{id, name, classes:[...per brace...]}]."""
    out = []
    for le in braces_geom_by_le:
        out.append({"id": le["id"], "name": le["name"],
                    "classes": classify_kyx(le["braces_geom"], gaps, gate)})
    return out

# ---------- chord identification (robust, non-blocking) ----------
def identify_chord(members, xm):
    """Return (chord_member, warnings[]). Strategy mirrors the planned layer-1 rule
    but never raises — it flags problems so the viewer can show them."""
    warns = []
    if not members:
        return None, ["No members in this connection — nothing to identify as a chord."]
    bearings = [m for m in members if m.get("isBearing")]
    continuous = [m for m in members if m.get("isContinuous")]

    chord = None
    if len(bearings) == 1:
        chord = bearings[0]
    elif len(bearings) > 1:
        warns.append(f"{len(bearings)} bearing members — chord ambiguous.")
        chord = bearings[0]
    else:
        # no bearing flag — our rule says chord must be continuous; fall back by diameter
        warns.append("No bearing member — chord picked by heuristic (continuous / largest Ø).")
        cand = continuous if continuous else members
        # pick largest diameter
        def diam(m): return (xm.get(m["crossSectionId"], {}).get("D") or 0)
        chord = max(cand, key=diam)

    if len(continuous) == 0:
        warns.append("No continuous member (chord must be continuous).")
    elif len(continuous) > 1:
        warns.append(f"{len(continuous)} continuous members — chord ambiguous.")
    return chord, warns

# ---------- build payload for one connection ----------
def build_connection(session, pid, conn, members, xm, len_factor=6.0, min_len=0.3,
                     oop_tol_mm=OUT_OF_PLANE_OFFSET_MM, plane_tol_deg=PLANE_FIT_TOL_DEG,
                     coplanar_tol_deg=COPLANAR_EVAL_DEG, load_effects=None,
                     kyx_gate=K_BALANCE_GATE):
    """len_factor = drawn member length as a multiple of its diameter (each member length = len_factor*D).
    min_len = floor in metres so a tiny-Ø member is still visible.
    oop_tol_mm = out-of-plane origin offset tolerance in mm (UI-tunable).
    plane_tol_deg = RANSAC plane-FIT inlier tolerance (deg, UI-tunable, strict ~2°): which braces BUILD the plane.
    coplanar_tol_deg = EVALUATION tolerance (deg, UI-tunable ~15°): a member beyond this from the built
                       plane makes the joint multiplanar (2D hidden / out of simple-joint scope)."""
    chord, warns = identify_chord(members, xm)
    if chord is None:
        return {
            "connection_id": conn["id"], "connection": conn.get("name"),
            "chord": None, "braces": [], "params": None, "params_all": [], "gaps": [],
            "verdict": {"status": "ERROR", "errors": warns, "warnings": []},
            "coplanar": False, "plane_outliers": [], "eval_outliers": [],
            "plane_tol_deg": float(plane_tol_deg), "coplanar_tol_deg": float(coplanar_tol_deg),
            "plane_spread": None, "plane_fit_basis": None, "plane_warn": None,
            "node": [0.0, 0.0, 0.0], "equilibrium": [], "brace_forces": [], "chord_stresses": [],
            "classification": [], "joint_checks": [], "kyx_gate": float(kyx_gate),
            "view_center": [0.0, 0.0, 0.0], "units": "world coords in metres; D_mm in mm",
            "members2d": [], "members3d": [],
        }
    braces = [m for m in members if m["id"] != chord["id"]]

    cx = unit(vv(chord["position"]["axisX"]))
    sec_c = xm.get(chord["crossSectionId"], {})

    def eff_dir(m):
        """Effective direction the member body extends FROM the joint node.
        isContinuous / connectedBy tell which END sits at the node (see eff_dir_sign):
          connectedBy="begin" -> node at member start -> body runs along +axisX
          connectedBy="end"   -> node at member end   -> body runs along -axisX
        (isContinuous = node in the middle; treated as continuous, drawn both ways.)"""
        ax = unit(vv(m["position"]["axisX"]))
        sign = eff_dir_sign(m)
        return [ax[0]*sign, ax[1]*sign, ax[2]*sign]

    def ecc_vec(m):
        """The member's eccentricity as a GLOBAL vector = offsetEx*axisX + offsetEy*axisY + offsetEz*axisZ.
        Verified law (2026-06-28, all 40 joints, 0.00 mm): the offsets are applied in the member's LOCAL
        CSYS, and `origin - ecc_vec` lies on the member's CANONICAL axis — the axis that passes through
        the joint node. So this is the clean per-member eccentricity, straight from the API."""
        p = m["position"]
        ax = vv(p["axisX"]); ay = vv(p["axisY"]); az = vv(p["axisZ"])
        ex_, ey_, ez_ = p.get("offsetEx", 0.0), p.get("offsetEy", 0.0), p.get("offsetEz", 0.0)
        return add(add(scal(ax, ex_), scal(ay, ey_)), scal(az, ez_))

    # WORK POINT (node) = the GLOBAL ORIGIN (0,0,0), ALWAYS. Verified law (2026-06-28, all 40 joints):
    # every member's canonical axis (origin minus its local-CSYS eccentricity) passes through (0,0,0).
    # No best-fit, no mean-of-origins needed — the node is simply (0,0,0).
    node = [0.0, 0.0, 0.0]
    chord_o = vv(chord["position"]["origin"])
    # VIEW CENTER (display + rotation pivot) = the node (0,0,0). Everything drawn is centred on the joint.
    view_center = list(node)

    # ---- joint plane: X = chord axis (fixed); in-plane Y = ROBUST fit through brace dirs ----
    # The plane MUST contain the chord axis. Instead of a plain SVD over ALL braces (which lets one
    # multiplanar/outlier brace tilt the plane for everybody), we do a RANSAC-like fit:
    #   - each candidate plane = chord axis + one brace's perpendicular component (uniquely fixes it)
    #   - score = how many braces lie within plane_tol_deg of that plane (INLIERS)
    #   - pick the candidate with the most inliers (tie -> smallest total deviation)
    #   - final in-plane Y = SVD over the INLIER perp-components only (cleaner than a single brace)
    # Outliers (braces beyond the tolerance) do NOT influence the plane; they are still classified later.
    ex = unit(cx)
    fallback_ey = unit(cross([0, 0, 1], ex)) if abs(dot(ex, [0, 0, 1])) < 0.99 else [0, 1, 0]

    def perp_of(b):
        bd = unit(eff_dir(b))
        perp = sub(bd, scal(ex, dot(bd, ex)))   # component across the chord
        return unit(perp) if nrm(perp) > 1e-6 else None

    perps = [(b, perp_of(b)) for b in braces]
    perps = [(b, p) for (b, p) in perps if p is not None]

    plane_outliers = []
    plane_warn = None          # set when the FIT tolerance couldn't cluster >=2 braces
    plane_fit_basis = None     # human-readable note of what built the plane
    if perps:
        tol = max(0.0, float(plane_tol_deg))
        def dev_deg(n_cand, b):
            bd = unit(eff_dir(b))
            return math.degrees(math.asin(max(-1.0, min(1.0, abs(dot(bd, n_cand))))))
        # try each brace's perp as the seed plane; count inliers
        best = None  # (score, inlier_perps, n_cand)
        for (bseed, pseed) in perps:
            n_cand = unit(cross(ex, pseed))
            if nrm(n_cand) < 1e-9:
                continue
            inl = [(b, p) for (b, p) in perps if dev_deg(n_cand, b) <= tol]
            tot = sum(dev_deg(n_cand, b) for (b, p) in inl)
            score = (len(inl), -tot)
            if best is None or score > best[0]:
                best = (score, [p for (b, p) in inl], n_cand)
        n_inliers = best[0][0] if best else 0

        if len(perps) == 1:
            # only ONE brace defines the plane -> chord + that brace; the plane is always unique, no pair
            # tolerance applies. Don't crash, don't warn about a pair.
            inlier_perps = [perps[0][1]]
            plane_fit_basis = f"single brace {perps[0][0].get('name')}"
        elif n_inliers >= 2:
            # normal case: a cluster of >=2 coplanar braces built the plane
            inlier_perps = best[1]
            plane_fit_basis = f"{n_inliers} coplanar braces within {tol:g}deg"
        else:
            # NO pair fell within the FIT tolerance. Build from the most-nearly-coplanar PAIR (smallest
            # mutual angular deviation) and warn with that deviation. Edge case: if several pairs share
            # the same minimal deviation (no unique closest pair), the plane is ambiguous -> fit across
            # ALL braces instead and say so.
            pairs = []   # (dev_deg, bi, bj, pi, pj)
            for i in range(len(perps)):
                for j in range(i + 1, len(perps)):
                    bi, pi = perps[i]; bj, pj = perps[j]
                    n_ij = cross(ex, pi)
                    if nrm(n_ij) < 1e-9:
                        continue
                    n_ij = unit(n_ij)
                    d = dev_deg(n_ij, bj)   # how far brace j's axis is off the plane (chord, brace i)
                    pairs.append((d, bi, bj, pi, pj))
            if pairs:
                pairs.sort(key=lambda t: t[0])
                d0 = pairs[0][0]
                tied = [p for p in pairs if abs(p[0] - d0) <= 1e-6]   # pairs sharing the minimal deviation
                if len(tied) > 1:
                    # ambiguous: no single closest pair -> average the plane over ALL braces
                    inlier_perps = [p for (b, p) in perps]
                    plane_fit_basis = "all braces (tie on closest pair)"
                    plane_warn = (f"No two braces are coplanar within the {tol:g}deg fit tolerance and the "
                                  f"closest-pair deviation ({d0:.1f}deg) is shared by {len(tied)} pairs, so the "
                                  f"plane is averaged across all braces. The 2D plane is only indicative — "
                                  f"check the 3D view.")
                else:
                    d, bi, bj, pi, pj = pairs[0]
                    inlier_perps = [pi, pj]
                    plane_fit_basis = f"closest pair {bi.get('name')}-{bj.get('name')}"
                    plane_warn = (f"No two braces are coplanar within the {tol:g}deg fit tolerance; "
                                  f"the joint plane was built from the closest pair "
                                  f"{bi.get('name')}-{bj.get('name')} (mutual deviation {d:.1f}deg > {tol:g}deg). "
                                  f"The 2D plane is only indicative — check the 3D view.")
            else:
                inlier_perps = [p for (b, p) in perps]
                plane_fit_basis = "all braces (no valid pair)"
                plane_warn = ("Could not form a brace pair for the joint plane; "
                              "fitted across all braces — the 2D plane is only indicative.")

        # final in-plane Y: SVD over the chosen perp components (sign-consistent), perpendicular to chord
        M = np.array(inlier_perps)
        ref = M[0]
        M = np.array([d if np.dot(d, ref) >= 0 else -d for d in M])
        U, S, Vt = np.linalg.svd(M)
        ey = unit(list(Vt[0]))
        ey = unit(sub(ey, scal(ex, dot(ey, ex))))
        n_tmp = unit(cross(ex, ey))
        plane_spread = abs(float(S[1])) if len(S) > 1 else 0.0
        # record which braces ended up as outliers (axis beyond tol from the chosen plane)
        plane_outliers = [b.get("name") for b in braces
                          if math.degrees(math.asin(max(-1.0, min(1.0, abs(dot(unit(eff_dir(b)), n_tmp)))))) > tol]
    else:
        ey = fallback_ey
        plane_spread = 0.0
        plane_fit_basis = "no brace (fallback orientation)"
    n_plane = unit(cross(ex, ey))   # normal of the SVD-fitted joint plane

    def theta(m):
        # angle between chord axis and the brace's effective direction (acute, 0..90)
        bd = unit(eff_dir(m))
        ang = math.degrees(math.acos(max(-1, min(1, dot(cx, bd)))))
        return ang if ang <= 90 else 180 - ang

    def coplanar_dev(m):
        """angle (deg) of brace effective DIRECTION out of the joint plane (0 = axis in plane)."""
        bd = unit(eff_dir(m))
        comp = abs(dot(bd, n_plane))
        return math.degrees(math.asin(max(-1, min(1, comp))))

    # Eccentricity of each member is taken DIRECTLY from the API (ecc_vec = the offsets applied in the
    # local CSYS, verified to be the exact shift of the canonical axis off the node). We decompose this
    # clean eccentricity vector against the joint frame: component on the plane normal = out-of-plane
    # offset; component along the chord axis = e for the e>D/4 gate.
    def out_of_plane_offset(m):
        """perpendicular distance (m) of the member from the joint PLANE = eccentricity component
        along the plane normal."""
        return abs(dot(ecc_vec(m), n_plane))

    def ecc_along_chord(m):
        """signed eccentricity (m) ALONG the chord axis = e in the e>D/4 gate."""
        return dot(ecc_vec(m), ex)

    R_chord = (sec_c.get("D") or 0) / 2.0 / 1000.0   # chord radius (m) — feet land on the chord SURFACE

    def brace_side(m):
        """Which face of the chord the brace attaches to, in the joint plane: +1 = +ey face, -1 = -ey face.
        = sign of the brace's in-plane direction component along ey. Braces on opposite faces never form a
        gap with each other (gaps are per-face)."""
        d3 = unit(eff_dir(m))
        dp = sub(d3, scal(n_plane, dot(d3, n_plane)))
        if nrm(dp) < 1e-9:
            return 1
        return 1 if dot(unit(dp), ey) >= 0 else -1

    def brace_landing_and_foot(m):
        """In the joint PLANE (NORSOK Fig 6-6): the brace foot lands on the chord SURFACE of ITS OWN face
        (z = side*R_chord), NOT on the axis. Returns (landing_m, foot_half_m), both along-chord.
          landing = along-chord coord where the (in-plane) brace axis meets the surface line z=side*R.
          foot    = (d/2)/sin(theta_in-plane) = half the brace footprint along the chord.
        Surface (not axis) landing is essential: a big chord with thin steep braces gives a POSITIVE gap
        with NO eccentricity (feet land ~R/tan(theta) apart); eccentricity shifts the landing further.
        We keep the brace's real in-plane direction (do NOT flip it) so braces on the -ey face land on the
        -R face correctly. Project into the plane first (generally-oriented local axes aren't in-plane)."""
        side = brace_side(m)
        d3 = unit(eff_dir(m))
        dp = sub(d3, scal(n_plane, dot(d3, n_plane)))   # project dir into plane
        if nrm(dp) < 1e-9:
            return 0.0, 0.0
        dp = unit(dp)
        o = vv(m["position"]["origin"])
        ox, oy = dot(sub(o, node), ex), dot(sub(o, node), ey)
        dx, dy = dot(dp, ex), dot(dp, ey)
        nn = math.sqrt(dx*dx + dy*dy); dx, dy = dx/nn, dy/nn
        z_surf = side * R_chord                                 # land on this brace's own chord face
        landing = ox if abs(dy) < 1e-9 else ox + ((z_surf - oy)/dy)*dx
        sin_th = abs(dy)                                        # in-plane angle to chord
        D = (xm.get(m["crossSectionId"], {}).get("D") or 0) / 1000.0
        foot = (D/2)/sin_th if sin_th > 1e-3 else D/2
        return landing, foot

    def compute_gaps(brace_members):
        """Gaps between braces ON THE SAME CHORD FACE (NORSOK Fig 6-6), signed.
        Returns EVERY pair on a face, not just adjacent ones: force balancing in N-brace joints can pair
        NON-adjacent braces (Fig 6-2 (d)/(e): an outer brace balances against a far brace across an
        intermediate one — that gap spans the intermediates and is often large, K may revert to Y). So for
        N braces on a face there are C(N,2) pair gaps; `adjacent` flags the N-1 neighbour pairs. Layer 2
        (the actual 6.4 check) picks which pairs/weights to use from the force balance.
        Both chord faces handled (grouped by side; never a gap across faces). gap>0 = clear; gap<=0 =
        overlap. Returns [{between:[a,b], gap_m, side:'+'|'-', adjacent:bool}]."""
        groups = {1: [], -1: []}
        for b in brace_members:
            lnd, ft = brace_landing_and_foot(b)
            groups[brace_side(b)].append((b.get("name"), lnd, ft))
        out = []
        for sd, items in groups.items():
            if len(items) < 2:
                continue
            items.sort(key=lambda r: r[1])         # order along the chord within this face
            for i in range(len(items)):
                for j in range(i + 1, len(items)):
                    (na, la, fa), (nb, lb, fb) = items[i], items[j]
                    gap = (lb - fb) - (la + fa)    # toe-to-toe between the two feet (spans any in between)
                    out.append({"between": [na, nb], "gap_m": gap,
                                "side": "+" if sd > 0 else "-", "adjacent": (j == i + 1)})
        return out

    # continuous members are drawn to ONE common length driven by the largest continuous-member Ø, so the
    # through-members look like long through-members rather than each shrinking to its own diameter.
    # NOTE: the real member length is NOT available from the API — `origin` is just the anchor of the
    # reference line (its distance from the node = however the member was defined), and it does NOT match
    # the length IDEA draws (verified on TEST/CON2: a continuous member's GUI diagram length ≠ 2*t-from-
    # origin). So we use a purely visual length, not a claimed-real one.
    cont_D = max([(xm.get(m["crossSectionId"], {}).get("D") or 0) for m in members
                  if m.get("isContinuous")] + [0]) / 1000.0
    cont_half = max(min_len, len_factor * cont_D) if cont_D > 0 else min_len

    def mem_len(m):
        """drawn HALF/length (VISUAL only — real length unknown from API). Continuous → one common length
        (cont_half, from the largest through Ø). Brace → len_factor * own diameter."""
        if m.get("isContinuous"):
            return cont_half
        D = (xm.get(m["crossSectionId"], {}).get("D") or 60) / 1000.0
        return max(min_len, len_factor * D)

    def seg3d(m):
        org = vv(m["position"]["origin"])
        # We draw each member on its REAL axis (= through origin, i.e. WITH its eccentricity) so the view
        # matches IDEA's graphics 1:1. ANY continuous member is drawn on that real axis line centred at
        # the point CLOSEST to the node (0,0,0) — its origin is a distant line ref (~1.4 m away), so the
        # brace branch below would draw it far off and as a half-stub (CON3 had its 2nd continuous member
        # M2 mis-drawn this way). The real axis is the canonical axis shifted by the eccentricity, so it
        # passes ~eccentricity away from (0,0,0) — exactly the offset IDEA shows.
        if m.get("isContinuous"):
            ax = unit(vv(m["position"]["axisX"]))
            half = mem_len(m)
            t = dot(sub(node, org), ax)              # project node onto the member's real axis line
            center = add(org, scal(ax, t))           # point on that axis nearest the node
            return sub(center, scal(ax, half)), add(center, scal(ax, half))
        # brace / non-continuous member: from its own origin (the insertion point) along its direction.
        bd = eff_dir(m)
        return org, add(org, scal(bd, mem_len(m)))

    members2d, members3d, braces_meta = [], [], []

    def push(m, role):
        sec = xm.get(m["crossSectionId"], {})
        p1, p2 = seg3d(m)
        members2d.append({
            "name": m.get("name"), "role": role,
            "p1": proj2d(p1, node, ex, ey), "p2": proj2d(p2, node, ex, ey),
            "D_mm": sec.get("D"), "T_mm": sec.get("T"), "section": sec.get("name"),
        })
        members3d.append({
            "name": m.get("name"), "role": role,
            "p1": p1, "p2": p2,
            "D_mm": sec.get("D"), "T_mm": sec.get("T"), "isCHS": sec.get("isCHS", False),
        })

    push(chord, "chord")
    D_chord_m = (sec_c.get("D") or 0) / 1000.0
    for b in braces:
        # a 2nd (or further) continuous member is a through-member too — draw it chord-like (grey, long),
        # not as a short brace. Only genuinely ended members are drawn as braces.
        push(b, "chord" if b.get("isContinuous") else "brace")
        secb = xm.get(b["crossSectionId"], {})
        D = sec_c.get("D"); d = secb.get("D")
        braces_meta.append({
            "name": b.get("name"), "section": secb, "theta_deg": theta(b),
            "beta": (d/D if (d and D) else None),
            "coplanar_dev_deg": coplanar_dev(b),
            "oop_offset_m": out_of_plane_offset(b),
            "ecc_along_m": ecc_along_chord(b),
            "isCHS": secb.get("isCHS", False),
            "origin": vv(b["position"]["origin"]),
            "forces_in": (b.get("model") or {}).get("forcesIn"),
        })

    # NORSOK params per brace (β, γ shared from chord, τ, θ). γ is a chord property (same for all braces).
    gamma_chord = (sec_c["D"]/(2*sec_c["T"])) if (sec_c.get("D") and sec_c.get("T")) else None
    params_all = []
    for bm in braces_meta:
        sb = bm["section"]
        params_all.append({
            "name": bm["name"],
            "beta": (sb.get("D")/sec_c["D"]) if (sb.get("D") and sec_c.get("D")) else None,
            "gamma": gamma_chord,
            "tau": (sb.get("T")/sec_c["T"]) if (sb.get("T") and sec_c.get("T")) else None,
            "theta_deg": bm["theta_deg"],
        })
    params = params_all[0] if params_all else None   # kept for backward compat (1st brace)

    # gaps between adjacent braces along the chord (NORSOK Fig 6-6). Only true braces (ended members);
    # a second continuous member isn't a gap brace.
    gap_braces = [b for b in braces if not b.get("isContinuous")]
    gaps = compute_gaps(gap_braces) if len(gap_braces) >= 2 else []

    chord_forces_in = (chord.get("model") or {}).get("forcesIn")
    verdict = classify_assumptions(chord, sec_c, braces, braces_meta, D_chord_m, warns, oop_tol_mm,
                                   chord_forces_in, gaps, plane_warn)

    # Per-member status for colouring (OK green / problem amber|red). A member is flagged if its name
    # appears in any verdict error (red) or warning (amber). Chord can also be flagged.
    def member_status(nm):
        for e in verdict["errors"]:
            if e.startswith(nm + ":") or (nm + "-") in e or ("-" + nm) in e:
                return "ERROR"
        for w in verdict["warnings"]:
            if w.startswith(nm + ":") or (nm + "-") in w or ("-" + nm) in w:
                return "WARNING"
        return "OK"
    for coll in (members2d, members3d):
        for m in coll:
            m["status"] = member_status(m["name"])

    # coplanar = does the WHOLE joint lie within the EVALUATION tolerance of the built plane?
    # TWO decoupled tolerances: the plane is BUILT from strict inliers (plane_tol_deg ~2°, RANSAC), then
    # EVERY brace is evaluated against that finished plane at coplanar_tol_deg (~15°, NORSOK). A brace may
    # be excluded from the FIT (not strictly coplanar) yet still pass evaluation (<15°) -> 2D stays. Only
    # when some member is beyond coplanar_tol_deg is the joint multiplanar -> 2D hidden / out of scope.
    coplanar = all(bm["coplanar_dev_deg"] <= coplanar_tol_deg for bm in braces_meta)
    eval_outliers = [bm["name"] for bm in braces_meta if bm["coplanar_dev_deg"] > coplanar_tol_deg]

    # node-equilibrium self-check (confirms forces/axes/levers are read right; a balanced joint -> ~0).
    # NOTE: a tiny residual is EXPECTED — input forces are stored rounded (e.g. 300007 N = "300 kN"),
    # so "balanced" lands near, not exactly, zero. EQUILIBRIUM_TOL_* classify OK vs noticeable.
    equilibrium = node_equilibrium(members, load_effects, node)

    # STEP 2: resolve each brace's section forces into joint-plane components (N_Sd, M_ip, M_op) per LE.
    # Uses the SAME geometric frame as the plane fit (ex = chord axis, n_plane = joint normal). Only true
    # braces (ended members) — a 2nd continuous member is not a brace to be checked.
    brace_forces = brace_forces_in_plane(gap_braces, load_effects, ex, n_plane)

    # per-brace chord face (+1/-1) and angle — needed by BOTH the chord-stress prep and the classification.
    side_by_name = {b.get("name"): brace_side(b) for b in gap_braces}
    theta_by_name = {b.get("name"): theta(b) for b in gap_braces}
    # stamp the chord-face side onto each brace-forces row too (results-sheet table + 2D marker; same
    # value classify_kyx uses, kept in ONE place: brace_side()).
    for le in brace_forces:
        for row in le["braces"]:
            row["side"] = side_by_name.get(row.get("name"), 1)

    # STEP 4 prep: chord nominal stresses at each brace footprint (per LE) for the Qf chord-action factor.
    # Average of both sides of the intersection (NORSOK p.31); sigma in the NORSOK sign convention.
    chord_stresses = chord_stresses_all_les(chord, gap_braces, load_effects, ex, n_plane, sec_c, side_by_name)

    # STEP 3: K/Y/X classification per LE. Assemble per-brace geometry (side, theta) + N_Sd (from STEP 2),
    # then decompose the transverse force into K (same-side opposite-sign cancellation, nearest gap first),
    # X (through-chord to the opposite side, no coaxiality), Y (leftover beam shear). See classify_kyx.
    braces_geom_by_le = [{
        "id": le["id"], "name": le["name"],
        "braces_geom": [{"name": r["name"], "side": side_by_name.get(r["name"], 1),
                         "theta_deg": theta_by_name.get(r["name"], 90.0), "N_Sd": r["N_Sd"]}
                        for r in le["braces"]],
    } for le in brace_forces]
    classification = classify_kyx_all_les(braces_geom_by_le, gaps, kyx_gate)

    # STEP 4: NORSOK 6.4 resistance check per brace per LE (chord-wall check; materially fy_chord).
    brace_secs = {bm["name"]: bm["section"] for bm in braces_meta}
    params_by_name = {p["name"]: p for p in params_all}
    joint_checks = joint_checks_all_les(sec_c, brace_secs, params_by_name,
                                        brace_forces, classification, chord_stresses)

    return {
        "connection_id": conn["id"],
        "connection": conn.get("name"),
        "chord": {"name": chord.get("name"), "section": sec_c, "isCHS": sec_c.get("isCHS", False),
                  "status": member_status(chord.get("name"))},
        "braces": braces_meta,
        "params": params,
        "params_all": params_all,      # NORSOK params for EVERY brace (not just the 1st)
        "gaps": gaps,                  # [{between:[a,b], gap_m}] adjacent-brace gaps along chord (Fig 6-6)
        "verdict": verdict,            # {status, errors[], warnings[]}
        "coplanar": coplanar,          # False -> some member beyond coplanar_tol_deg -> 2D hidden / out of scope
        "plane_outliers": plane_outliers,  # braces NOT used to BUILD the plane (beyond plane_tol_deg FIT tol)
        "eval_outliers": eval_outliers,    # braces beyond coplanar_tol_deg from the built plane (multiplanar)
        "plane_tol_deg": float(plane_tol_deg),
        "coplanar_tol_deg": float(coplanar_tol_deg),
        "plane_spread": plane_spread,  # SVD 2nd singular value = out-of-plane scatter of inlier brace dirs
        "plane_fit_basis": plane_fit_basis,  # human note: what built the plane (cluster / closest pair / single)
        "plane_warn": plane_warn,      # set when no pair met the FIT tol -> closest-pair fallback used
        "node": node,                  # work point = chord insertion point (NORSOK reference)
        "equilibrium": equilibrium,    # per-LE residual force/moment at the node (self-check; ~0 = balanced)
        "brace_forces": brace_forces,  # STEP 2: per-LE brace forces resolved into the joint plane (N_Sd/M_ip/M_op)
        "chord_stresses": chord_stresses,  # STEP 4 prep: per-LE chord sigma_a/my/mz [Pa] at each brace footprint (Qf)
        "classification": classification,  # STEP 3: per-LE K/Y/X decomposition (frK/frX/frY + K_components per brace)
        "joint_checks": joint_checks,      # STEP 4: per-LE NORSOK 6.4 resistance check per brace (util, N_Rd, Qf, K_terms)
        "kyx_gate": float(kyx_gate),       # the "balanced within X" gate used (0 = honest breakdown)
        "view_center": view_center,    # mean of all member origins (display center + rotation pivot only)
        "units": "world coords in metres; D_mm in mm",
        "members2d": members2d,
        "members3d": members3d,
    }


# forcesIn modes we can interpret (we know where the force acts). bolts/selectedMemberFace are not
# supported: the application point is a bolt group / a chosen member face, which we don't resolve.
SUPPORTED_FORCES_IN = {"node", "position", None}  # None = legacy/unspecified, treated as node

# ---------- assumption classification (hard ERROR vs soft WARNING) ----------
def classify_assumptions(chord, sec_c, braces, braces_meta, D_chord_m, chord_warns,
                         oop_tol_mm=OUT_OF_PLANE_OFFSET_MM, chord_forces_in=None, gaps=None,
                         plane_warn=None):
    """Return {status: 'OK'|'WARNING'|'ERROR', errors:[...], warnings:[...]}.
    Implements the agreed layer-1 gate. Non-blocking: always returns, never raises."""
    errors, warnings = [], []

    # --- joint-plane FIT warning (RANSAC couldn't cluster >=2 braces within the fit tol) ---
    if plane_warn:
        warnings.append(plane_warn)

    # --- gaps (NORSOK Fig 6-6): a negative gap between ADJACENT feet = they OVERLAP = overlap joint, out
    #     of the 6.4 gap-joint rules -> hard ERROR (layer-1 decision: negative gap is a hard reject).
    #     Only ADJACENT pairs matter for overlap; a non-adjacent pair's "gap" spans an intermediate brace
    #     and being negative there just means the intermediate sits between them, not an overlap. ---
    for g in (gaps or []):
        if g.get("adjacent") and g["gap_m"] < 0:
            a, b = g["between"]
            errors.append(f"{a}-{b}: feet overlap (gap {g['gap_m']*1000:.0f} mm < 0) — overlap joint, out of 6.4 gap rules.")

    # --- chord identification problems become ERRORs where they break the rule ---
    for w in chord_warns:
        if "continuous member" in w:        # "No continuous member" / "N continuous members"
            errors.append(w)                # chord must be exactly one continuous member
        else:
            warnings.append(w)

    # 0. forces input mode — we only support node / position (we know where the force acts).
    if chord_forces_in not in SUPPORTED_FORCES_IN:
        errors.append(f"{chord.get('name')}: unsupported forces input '{chord_forces_in}' (only node/position).")
    for bm in braces_meta:
        fi = bm.get("forces_in")
        if fi not in SUPPORTED_FORCES_IN:
            errors.append(f"{bm['name']}: unsupported forces input '{fi}' (only node/position).")

    # 1. all members CHS
    if not sec_c.get("isCHS"):
        errors.append(f"Chord not CHS ({sec_c.get('name')}).")
    for bm in braces_meta:
        if not bm.get("isCHS"):
            errors.append(f"{bm['name']}: not CHS ({bm['section'].get('name')}).")

    # 4. at least one brace
    if not braces:
        errors.append("No brace (chord only).")

    for bm in braces_meta:
        nm = bm["name"]
        # 5. brace nearly parallel to chord (degenerate)
        if bm["theta_deg"] < PARALLEL_MIN_THETA_DEG:
            errors.append(f"{nm}: θ={bm['theta_deg']:.1f}° — parallel to chord (degenerate).")
        # 6 / 8. coplanarity of the brace AXIS
        dev = bm["coplanar_dev_deg"]
        if dev > COPLANAR_MAX_DEG:
            errors.append(f"{nm}: {dev:.1f}° off plane (>{COPLANAR_MAX_DEG:.0f}°) — different plane / multiplanar.")
        elif dev > COPLANAR_WARN_DEG:
            warnings.append(f"{nm}: {dev:.1f}° off plane (borderline).")
        # 7. out-of-plane offset of the ORIGIN -> not allowed
        oop_mm = bm["oop_offset_m"] * 1000.0
        if oop_mm > oop_tol_mm:
            errors.append(f"{nm}: out-of-plane ecc. {oop_mm:.0f} mm (>{oop_tol_mm:.0f} mm).")
        # 9. eccentricity along the chord (e > D/4) -> warning
        if D_chord_m > 0:
            e = abs(bm["ecc_along_m"])
            if e > ECC_ALONG_CHORD_FRAC * D_chord_m:
                warnings.append(f"{nm}: ecc. along chord e={e*1000:.0f} mm (>D/4={ECC_ALONG_CHORD_FRAC*D_chord_m*1000:.0f} mm).")
        # 10. geometry ranges 6.4.3.1 (per brace)
        beta = bm.get("beta")
        if beta is not None and not (0.2 <= beta <= 1.0):
            warnings.append(f"{nm}: β={beta:.3f} outside 0.2–1.0.")
        th = bm["theta_deg"]
        if not (30.0 <= th <= 90.0) and th >= PARALLEL_MIN_THETA_DEG:
            warnings.append(f"{nm}: θ={th:.1f}° outside 30–90°.")

    # gamma range (chord)
    if sec_c.get("D") and sec_c.get("T"):
        g = sec_c["D"] / (2 * sec_c["T"])
        if not (10.0 <= g <= 50.0):
            warnings.append(f"γ={g:.2f} outside 10–50.")

    status = "ERROR" if errors else ("WARNING" if warnings else "OK")
    return {"status": status, "errors": errors, "warnings": warnings}

# ---------- high-level: open + list, and build a chosen connection ----------
def open_and_list(session, ideacon):
    """Open project, return (pid, connections_list)."""
    pid = open_project(session, ideacon)
    conns = list_connections(session, pid)
    return pid, conns

def build_for(session, pid, conn_id, oop_tol_mm=OUT_OF_PLANE_OFFSET_MM,
              plane_tol_deg=PLANE_FIT_TOL_DEG, coplanar_tol_deg=COPLANAR_EVAL_DEG,
              kyx_gate=K_BALANCE_GATE):
    conns = list_connections(session, pid)
    conn = next(c for c in conns if c["id"] == conn_id)
    members = get_members(session, pid, conn_id)
    xm = xs_map(get_cross_sections(session, pid))
    try:
        load_effects = get_load_effects(session, pid, conn_id)
    except Exception:
        load_effects = None
    return build_connection(session, pid, conn, members, xm, oop_tol_mm=oop_tol_mm,
                            plane_tol_deg=plane_tol_deg, coplanar_tol_deg=coplanar_tol_deg,
                            load_effects=load_effects, kyx_gate=kyx_gate)

if __name__ == "__main__":
    import json, sys
    ic = sys.argv[1] if len(sys.argv) > 1 else os.path.join(os.path.dirname(__file__), "..", "norsok.ideaCon")
    s = requests.Session(); connect(s)
    pid, conns = open_and_list(s, os.path.abspath(ic))
    print("connections:", [(c["id"], c["name"]) for c in conns])
    for c in conns:
        d = build_for(s, pid, c["id"])
        v = d["verdict"]
        print(f"\n== {d['connection']} == {v['status']}  chord={d['chord']['name']} braces={len(d['braces'])}")
        for e in v["errors"]:   print("   [E] ", e)
        for w in v["warnings"]: print("   [W] ", w)
    close_project(s, pid)
