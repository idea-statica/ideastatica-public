"""
NORSOK N-004 Rev. 3 (Feb 2013) — Chapter 6.4 Tubular joints (simple joints).

Library module: pure functions + dataclasses, returns structured results with all
intermediate values. This is the reference implementation of "layer 2" (the NORSOK
joint-resistance engine) for the IdeaCon add-on, and the Python twin of
`norsok-6.4-calculator.html` — formulas are kept bit-for-bit identical so the two
implementations cross-check.

Scope: simple joints (6.4.3). NO joint cans (assumes T_c = T_n), NO overlap/ring/cast.
Classification K/Y/X is given as fractions (manual); all three classes are always
computed; the weighted result uses the fractions.

Units: PURE SI — forces in N, moments in N·m, lengths in m, stresses in Pa. (Qu/Qf/β/γ/τ are
dimensionless, so the resistance formulas themselves are unit-agnostic given consistent inputs; the
project convention is strict SI.) Helper builders accept kN/kNm + mm/MPa for convenience
(JointInput.from_kN) or pure SI (JointInput.from_SI).

PRIORITY: this is NORSOK (clauses 6.x, Tables 6-3/6-4, LRFD with gamma_M = 1.15).
Do NOT mix in API RP-2A-WSD allowable-stress apparatus.
"""
from __future__ import annotations
from dataclasses import dataclass, field, asdict, replace
from math import sin, radians, sqrt
from typing import Literal

JointClass = Literal["K", "Y", "X"]
CLASSES: tuple[JointClass, ...] = ("K", "Y", "X")


# --------------------------------------------------------------------------- #
# Inputs
# --------------------------------------------------------------------------- #
@dataclass
class JointInput:
    """Canonical fields are PURE SI: lengths m, stress Pa, force N, moment N·m.
    (Qu/Qf/β/γ/τ are dimensionless ratios, so the resistance formulas are unit-agnostic as long as the
    inputs are mutually consistent — but the project convention is now strict SI throughout; use the
    builders for kN/kNm or mm/MPa convenience.)"""
    # chord (CHS)
    D: float          # chord outer diameter [m]
    T: float          # chord wall thickness [m]
    fy_chord: float   # chord yield strength [Pa]
    # brace (CHS)
    d: float          # brace outer diameter [m]
    t: float          # brace wall thickness [m]
    fy_brace: float   # brace yield strength [Pa]
    theta_deg: float  # brace-chord angle [deg]
    g: float          # gap (K joints) [m] — the single-gap shortcut (used when K_components is empty)
    # classification fractions (0..1), should sum to 1.0
    frK: float = 1.0
    frY: float = 0.0
    frX: float = 0.0
    # K balancing components: each is (fraction_of_axial_balanced_via_this_gap, gap_m). A brace that
    # balances against several braces (KT / N joints) has ONE component PER gap, each with its OWN gap ->
    # its OWN Q_g -> its OWN K resistance (Table 6-3 row K). Their fractions sum to frK. When empty, the
    # single-gap shortcut (frK, g) is used as one component -> backward compatible. (Y and X have no gap.)
    K_components: list = None   # list[tuple[float, float]]  (frK_i, g_i_m)
    # brace design forces
    N_Sd: float = 0.0    # axial [N]  (+ tension / - compression)
    M_ip_Sd: float = 0.0  # in-plane bending [N·m]
    M_op_Sd: float = 0.0  # out-of-plane bending [N·m]
    # chord design stresses for Qf (6.54/6.55) [Pa]
    sigma_a_Sd: float = 0.0
    sigma_my_Sd: float = 0.0
    sigma_mz_Sd: float = 0.0
    # material factor
    gamma_M: float = 1.15

    @classmethod
    def from_SI(cls, **kw) -> "JointInput":
        """Explicit pure-SI builder (alias of the constructor) — lengths m, stress Pa, force N, moment N·m."""
        return cls(**kw)

    @classmethod
    def from_kN(cls, *, D, T, fy_chord, d, t, fy_brace, theta_deg, g,
                frK=1.0, frY=0.0, frX=0.0, K_components=None,
                N_Sd_kN=0.0, M_ip_Sd_kNm=0.0, M_op_Sd_kNm=0.0,
                sigma_a_Sd=0.0, sigma_my_Sd=0.0, sigma_mz_Sd=0.0,
                gamma_M=1.15) -> "JointInput":
        """Convenience builder taking the ENGINEERING units engineers type: lengths mm, stress MPa,
        forces kN, moments kNm. Converts everything to the canonical SI fields. (sigma_* in MPa, gap in
        mm; K_components gaps in mm.)"""
        MM, MPA = 1e-3, 1e6
        Kc = ([(fr, gi * MM) for (fr, gi) in K_components] if K_components else None)
        return cls(
            D=D * MM, T=T * MM, fy_chord=fy_chord * MPA, d=d * MM, t=t * MM, fy_brace=fy_brace * MPA,
            theta_deg=theta_deg, g=g * MM, frK=frK, frY=frY, frX=frX, K_components=Kc,
            N_Sd=N_Sd_kN * 1e3, M_ip_Sd=M_ip_Sd_kNm * 1e3, M_op_Sd=M_op_Sd_kNm * 1e3,
            sigma_a_Sd=sigma_a_Sd * MPA, sigma_my_Sd=sigma_my_Sd * MPA, sigma_mz_Sd=sigma_mz_Sd * MPA,
            gamma_M=gamma_M,
        )


# --------------------------------------------------------------------------- #
# Core formulas (mirror of the HTML calculator)
# --------------------------------------------------------------------------- #
def Qu_axial(cls: JointClass, beta: float, gamma: float, Qg: float, Qb: float,
             load_axial: Literal["tension", "compression"]) -> float:
    """Table 6-3 axial Q_u by joint class and load sense."""
    if cls == "K":
        # tension and compression share the same expression
        return min((16 + 1.2 * gamma) * beta ** 1.2 * Qg,
                   40 * beta ** 1.2 * Qg)
    if cls == "Y":
        if load_axial == "tension":
            return 30 * beta
        return min(2.8 + (20 + 0.8 * gamma) * beta ** 1.6,
                   2.8 + 36 * beta ** 1.6)
    # X
    if load_axial == "tension":
        return 6.4 * gamma ** (0.6 * beta * beta)
    return (2.8 + (12 + 0.1 * gamma) * beta) * Qb


def Qu_ipb(beta: float, gamma: float) -> float:
    """Table 6-3 in-plane bending Q_u (same for all classes)."""
    return (5 + 0.7 * gamma) * beta ** 1.2


def Qu_opb(beta: float, gamma: float) -> float:
    """Table 6-3 out-of-plane bending Q_u (same for all classes)."""
    return 2.5 + (4.5 + 0.2 * gamma) * beta ** 2.6


def Q_beta(beta: float) -> float:
    """Geometric factor Q_beta (note (a) under Table 6-3)."""
    return 0.3 / (beta * (1 - 0.833 * beta)) if beta > 0.6 else 1.0


def Q_g(g: float, D: float, t: float, T: float, fy_b: float, fy_c: float,
        gamma: float) -> float:
    """Gap factor Q_g (note (b) under Table 6-3). Linear interp in the transition."""
    gD = g / D
    if gD >= 0.05:
        return max(1 + 0.2 * (1 - 2.8 * gD) ** 3, 1.0)
    phi = (t * fy_b) / (T * fy_c)
    if gD <= -0.05:
        return 0.13 + 0.65 * phi * gamma ** 0.5
    # -0.05 < gD < 0.05 : linear interpolation between the two limiting values
    Qg_pos = max(1 + 0.2 * (1 - 2.8 * 0.05) ** 3, 1.0)
    Qg_neg = 0.13 + 0.65 * phi * gamma ** 0.5
    w = (gD - (-0.05)) / 0.10
    return Qg_neg + (Qg_pos - Qg_neg) * w


def C_coeffs(cls: JointClass, load_kind: Literal["axial-tension", "axial-compression", "moment"],
             beta: float) -> tuple[float, float, float, str]:
    """Table 6-4 coefficients C1, C2, C3 (+ note). X interpolates between beta 0.9..1.0."""
    if load_kind == "moment":
        return 0.2, 0.0, 0.4, "All joints, brace moment"
    if cls == "K":
        return 0.2, 0.2, 0.3, "K, balanced axial"
    if cls == "Y":
        return 0.3, 0.0, 0.8, "T/Y, brace axial"
    # X — linear interpolation between beta=0.9 and beta=1.0

    def lerp(a: float, b: float) -> float:
        if beta <= 0.9:
            return a
        if beta >= 1.0:
            return b
        return a + (b - a) * (beta - 0.9) / 0.1
    if load_kind == "axial-tension":
        return lerp(0.0, 0.2), 0.0, lerp(0.4, 0.2), "X, brace axial tension (β-interp)"
    return lerp(0.2, -0.2), 0.0, lerp(0.5, 0.2), "X, brace axial compression (β-interp)"


def Qf(C1: float, C2: float, C3: float, sa: float, smy: float, smz: float,
       fy: float) -> tuple[float, float]:
    """Chord action factor Q_f (6.54) and chord utilisation A^2 (6.55).
    NB: (6.55) denominator of the 2nd term is 1.62*fy^2  (NOT (1.62*fy)^2)."""
    A2 = (sa / fy) ** 2 + (smy * smy + smz * smz) / (1.62 * fy * fy)
    qf = 1.0 + C1 * (sa / fy) - C2 * (smy / (1.62 * fy)) - C3 * A2
    return qf, A2


# --------------------------------------------------------------------------- #
# Results
# --------------------------------------------------------------------------- #
@dataclass
class ClassResult:
    cls: JointClass
    Qu_axial: float
    Qf_axial: float
    Qf_axial_A2: float
    C_axial: tuple[float, float, float, str]
    N_Rd: float        # [N]
    util: float        # eq (6.57) for this class (abs-value convention)
    util_axial_term: float
    util_ip_term: float
    util_op_term: float


@dataclass
class JointResult:
    # geometry
    beta: float
    gamma: float
    tau: float
    theta_deg: float
    sin_theta: float
    gD: float
    # validity
    validity: dict[str, bool]
    within_range: bool
    # helper factors
    Q_beta: float
    Q_g: float
    # bending resistances (shared across classes)
    Qu_ipb: float
    Qu_opb: float
    Qf_moment: float
    Qf_moment_A2: float
    M_Rd_ip: float     # [N·m]
    M_Rd_op: float     # [N·m]
    # per-class axial
    per_class: dict[str, ClassResult]
    # weighted
    N_Rd_weighted: float        # [N]
    util_weighted: float
    passed: bool
    load_axial: str             # "tension" | "compression"
    # K balancing breakdown: one entry per gap (frK_i, g_i_m, Qg_i, N_Rd_i[N]). Sum of frK_i = frK.
    K_terms: list = None
    # True when Q_f (eq 6.54, no floor in the norm) drove an ACTIVE class's N_Rd, or either shared M_Rd,
    # to <=0 — the chord wall has no meaningful remaining resistance. Forces `passed=False` regardless
    # of util_weighted (an app-level safety decision, not a norm requirement — see memory
    # norsok-audit-2026-07-01 for the future idea of falling back to a 6.3 member check in this case).
    chord_overstressed: bool = False

    def as_dict(self) -> dict:
        d = asdict(self)
        d["per_class"] = {k: asdict(v) for k, v in self.per_class.items()}
        return d


# --------------------------------------------------------------------------- #
# Main entry point
# --------------------------------------------------------------------------- #
def _check_joint_once(inp: JointInput, clamp_beta=None, clamp_gamma=None, clamp_theta_deg=None) -> JointResult:
    """Run the full NORSOK 6.4 simple-joint check ONCE with the given inputs, returning all
    intermediate values. If any clamp_* is given, it OVERRIDES the corresponding geometric ratio
    (beta/gamma/theta_deg) right after it's derived from D/d/T/t/theta_deg — everything downstream
    (Qu, Qf, N_Rd, M_Rd) then uses the override. Used by check_joint() to implement the norm's
    6.4.3.1 "outside the validity range, take the lesser of actual-vs-limiting-parameter capacity"
    rule without duplicating the whole formula set for a clamped pass. g/D has NO clamp parameter
    here (see check_joint(): it only matters for K, so it's clamped by overriding g itself upstream)."""
    D, T, d, t = inp.D, inp.T, inp.d, inp.t
    fy = inp.fy_chord            # f_y in (6.52)/(6.53) keys on chord yield
    gM = inp.gamma_M

    beta = d / D if clamp_beta is None else clamp_beta
    gamma = D / (2 * T) if clamp_gamma is None else clamp_gamma
    theta_deg = inp.theta_deg if clamp_theta_deg is None else clamp_theta_deg
    theta = radians(theta_deg)
    sinT = sin(theta)
    tau = t / T
    gD = inp.g / D

    # validity range (6.4.3.1). g/D only matters for K (it feeds Q_g, which only the K row of
    # Table 6-3 uses) — a brace with frK=0 never touches Q_g, so an odd gap on it isn't a real
    # validity breach for THIS brace and must not be reported as one.
    validity = {
        "0.2<=beta<=1.0": 0.2 <= beta <= 1.0,
        "10<=gamma<=50": 10 <= gamma <= 50,
        "30<=theta<=90": 30 <= inp.theta_deg <= 90,
        "g/D>-0.6 (K)": (gD > -0.6) or inp.frK <= 0.0,
    }
    within = all(validity.values())

    Qb = Q_beta(beta)

    # K balancing components: each gap g_i -> its own Q_g(g_i). Default (no list): one component (frK, g).
    K_comps = inp.K_components if inp.K_components else ([(inp.frK, inp.g)] if inp.frK > 0 else [])
    # representative Q_g for reporting = the one from the single/first gap (the detail is per-component).
    Qg = Q_g(inp.g, D, t, T, inp.fy_brace, inp.fy_chord, gamma)

    load_axial = "tension" if inp.N_Sd >= 0 else "compression"
    load_kind_ax = "axial-tension" if inp.N_Sd >= 0 else "axial-compression"

    # shared bending resistances (independent of classification)
    QuI = Qu_ipb(beta, gamma)
    QuO = Qu_opb(beta, gamma)
    Cm = C_coeffs("K", "moment", beta)           # moment coeffs same for all classes
    QfM, QfM_A2 = Qf(Cm[0], Cm[1], Cm[2],
                     inp.sigma_a_Sd, inp.sigma_my_Sd, inp.sigma_mz_Sd, fy)
    baseM = fy * T * T * d / (gM * sinT)
    MRd_ip = baseM * QuI * QfM
    MRd_op = baseM * QuO * QfM

    base = fy * T * T / (gM * sinT)

    per_class: dict[str, ClassResult] = {}
    for cls in CLASSES:
        # K's representative N_Rd uses the representative Qg (single/first gap); the weighted resistance
        # below uses the per-component gaps. Y/X have no gap, so Qg is irrelevant for them.
        QuA = Qu_axial(cls, beta, gamma, Qg, Qb, load_axial)
        Cax = C_coeffs(cls, load_kind_ax, beta)
        QfA, QfA_A2 = Qf(Cax[0], Cax[1], Cax[2],
                         inp.sigma_a_Sd, inp.sigma_my_Sd, inp.sigma_mz_Sd, fy)
        NRd = base * QuA * QfA
        a = abs(inp.N_Sd) / NRd
        b = (abs(inp.M_ip_Sd) / MRd_ip) ** 2
        c = abs(inp.M_op_Sd) / MRd_op
        per_class[cls] = ClassResult(
            cls=cls, Qu_axial=QuA, Qf_axial=QfA, Qf_axial_A2=QfA_A2,
            C_axial=Cax, N_Rd=NRd, util=a + b + c,
            util_axial_term=a, util_ip_term=b, util_op_term=c,
        )

    # --- weighted axial resistance (NORSOK/API weighted average of axial capacity) ---
    # K splits PER BALANCING GAP: each component i has its own Q_g(g_i) -> own K N_Rd(K,i) (Table 6-3
    # row K). The K share of the weighted resistance is Σ_i frK_i · N_Rd(K, Q_g(g_i)). Y and X carry no
    # gap, so they use the plain per-class N_Rd. (Comm. 6.4.2: a brace balanced partly via gap1 and partly
    # via gap2 needs the weighted average of the two K resistances.)
    CaxK = C_coeffs("K", load_kind_ax, beta)
    QfK, _ = Qf(CaxK[0], CaxK[1], CaxK[2], inp.sigma_a_Sd, inp.sigma_my_Sd, inp.sigma_mz_Sd, fy)
    K_terms = []   # (frK_i, g_i, Qg_i, QuA_i, NRd_i)
    wN_K = 0.0
    for frK_i, g_i in K_comps:
        Qg_i = Q_g(g_i, D, t, T, inp.fy_brace, inp.fy_chord, gamma)
        QuA_i = Qu_axial("K", beta, gamma, Qg_i, Qb, load_axial)
        NRd_i = base * QuA_i * QfK
        wN_K += frK_i * NRd_i
        K_terms.append((frK_i, g_i, Qg_i, QuA_i, NRd_i))

    wN = wN_K + inp.frY * per_class["Y"].N_Rd + inp.frX * per_class["X"].N_Rd
    # wN == 0 means no axial classification (frK=frY=frX=0, e.g. a pure-moment brace with N_Sd≈0)
    # — there is no chord-wall axial resistance to divide by, so the axial share of util is simply
    # ABSENT (0), not undefined. This is a legitimate, common input (moment-only brace), not an
    # error: the shared M_Rd check above is unaffected (Table 6-3's bending columns and Table 6-4's
    # moment row are the same for every class), so callers must NOT skip the whole brace on this —
    # only the axial term drops out of eq. (6.57)'s sum.
    axial_term = abs(inp.N_Sd) / wN if wN > 0.0 else 0.0
    wU = axial_term + (abs(inp.M_ip_Sd) / MRd_ip) ** 2 + abs(inp.M_op_Sd) / MRd_op

    # CHORD-OVERSTRESSED GUARD: Q_f (eq 6.54) has no floor in the norm, so under a heavily-stressed
    # chord it can go <=0, driving N_Rd/M_Rd <=0. NORSOK doesn't define what that means, so this is an
    # app-level safety decision (user-chosen 2026-07-01): treat it as an explicit FAIL, not a silent
    # 0/negative utilisation term that could read as PASS. Only ACTIVE axial classes (frK/frY/frX>0) are
    # checked — an unused class going negative (e.g. Y's N_Rd<0 on a pure-K joint, frY=0) is a harmless
    # hypothetical and must NOT fail a joint it plays no part in. The two bending resistances are shared
    # by every class, so they are always checked regardless of classification.
    chord_overstressed = (MRd_ip <= 0.0) or (MRd_op <= 0.0)
    if inp.frK > 0.0 and any(nrd_i <= 0.0 for (_, _, _, _, nrd_i) in K_terms):
        chord_overstressed = True
    if inp.frY > 0.0 and per_class["Y"].N_Rd <= 0.0:
        chord_overstressed = True
    if inp.frX > 0.0 and per_class["X"].N_Rd <= 0.0:
        chord_overstressed = True

    return JointResult(
        beta=beta, gamma=gamma, tau=tau, theta_deg=inp.theta_deg, sin_theta=sinT, gD=gD,
        validity=validity, within_range=within,
        Q_beta=Qb, Q_g=Qg,
        Qu_ipb=QuI, Qu_opb=QuO, Qf_moment=QfM, Qf_moment_A2=QfM_A2,
        M_Rd_ip=MRd_ip, M_Rd_op=MRd_op,
        per_class=per_class,
        N_Rd_weighted=wN, util_weighted=wU, passed=(wU <= 1.0) and not chord_overstressed,
        chord_overstressed=chord_overstressed,
        load_axial=load_axial,
        K_terms=[{"frK": fr, "g_m": gi, "Q_g": qg, "Qu_axial": qu, "N_Rd": nr}
                 for (fr, gi, qg, qu, nr) in K_terms],
    )


def check_joint(inp: JointInput) -> JointResult:
    """Run the NORSOK 6.4 simple-joint check, applying 6.4.3.1's out-of-range rule VERBATIM:
    "The equations can be used for joints with geometries which lie outside the validity ranges, by
    taking the usable strength as the LESSER of the capacities calculated on the basis of: (a) actual
    geometric parameters, (b) imposed limiting parameters for the validity range, where these limits
    are infringed." (p.28) — this is NOT the same as the old behaviour (compute with actual params,
    just show a warning): the norm requires an actual second calculation at the clamped limit and
    taking whichever capacity is smaller (more conservative).

    beta/gamma/theta are ALWAYS relevant (they feed every class's Qu and the shared M_Rd), so an
    out-of-range value on any of them always triggers the clamped comparison pass. g/D only feeds
    Q_g, which ONLY matters for the K class — a brace with frK=0 never uses Q_g at all, so an
    out-of-range gap on such a brace is clamped but produces no user-visible effect (as intended:
    don't warn about a parameter that plays no part in this brace's actual check)."""
    beta = inp.d / inp.D
    gamma = inp.D / (2 * inp.T)
    out_of_range = not (0.2 <= beta <= 1.0 and 10 <= gamma <= 50 and 30 <= inp.theta_deg <= 90)
    K_comps = inp.K_components if inp.K_components else ([(inp.frK, inp.g)] if inp.frK > 0 else [])
    gD_out_of_range = inp.frK > 0.0 and any((gi / inp.D) <= -0.6 for (_, gi) in K_comps)

    actual = _check_joint_once(inp)
    if not (out_of_range or gD_out_of_range):
        return actual

    clamp_beta = min(max(beta, 0.2), 1.0)
    clamp_gamma = min(max(gamma, 10.0), 50.0)
    clamp_theta = min(max(inp.theta_deg, 30.0), 90.0)
    inp_clamped = inp
    if gD_out_of_range:
        # clamp EVERY K component's gap to the g/D=-0.6 limit (only K uses g/D; Y/X are untouched)
        clamped_comps = [(fr, max(gi, -0.6 * inp.D)) for (fr, gi) in K_comps]
        inp_clamped = replace(inp, K_components=clamped_comps)
    limiting = _check_joint_once(inp_clamped, clamp_beta=clamp_beta, clamp_gamma=clamp_gamma,
                                  clamp_theta_deg=clamp_theta)

    # take the LESSER capacity (6.4.3.1) — compare on N_Rd_weighted/M_Rd, the quantities eq (6.57)
    # actually divides by; keep the WHOLE result (per_class/K_terms breakdown included) from whichever
    # pass gave the smaller N_Rd_weighted, so the detailed-check modal shows one internally-consistent
    # derivation rather than a mix of two passes.
    lesser = limiting if limiting.N_Rd_weighted < actual.N_Rd_weighted else actual
    MRd_ip = min(actual.M_Rd_ip, limiting.M_Rd_ip)
    MRd_op = min(actual.M_Rd_op, limiting.M_Rd_op)
    wN = lesser.N_Rd_weighted
    axial_term = abs(inp.N_Sd) / wN if wN > 0.0 else 0.0
    wU = axial_term + (abs(inp.M_ip_Sd) / MRd_ip) ** 2 + abs(inp.M_op_Sd) / MRd_op if (MRd_ip > 0 and MRd_op > 0) else float('inf')
    chord_overstressed = lesser.chord_overstressed or (MRd_ip <= 0.0) or (MRd_op <= 0.0)
    # beta/gamma/theta_deg/gD/validity/within_range must ALWAYS reflect the brace's ACTUAL geometry
    # (never the clamped comparison pass) — those fields describe the joint, not which of the two
    # calculations happened to win. Only the RESISTANCE side (N_Rd/M_Rd/util/passed) may come from
    # the clamped pass, per 6.4.3.1's "take the lesser capacity" rule.
    return replace(lesser, beta=actual.beta, gamma=actual.gamma, tau=actual.tau,
                   theta_deg=actual.theta_deg, sin_theta=actual.sin_theta, gD=actual.gD,
                   validity=actual.validity, within_range=actual.within_range,
                   M_Rd_ip=MRd_ip, M_Rd_op=MRd_op, N_Rd_weighted=wN, util_weighted=wU,
                   passed=(wU <= 1.0) and not chord_overstressed, chord_overstressed=chord_overstressed)


# --------------------------------------------------------------------------- #
# Tiny self-test (run: python norsok_64.py)
# --------------------------------------------------------------------------- #
if __name__ == "__main__":
    # default K-joint from the HTML calculator
    inp = JointInput.from_kN(
        D=508, T=16, fy_chord=355, d=300, t=12, fy_brace=355, theta_deg=45, g=60,
        frK=1.0, frY=0.0, frX=0.0,
        N_Sd_kN=-800, M_ip_Sd_kNm=40, M_op_Sd_kNm=10,
        sigma_a_Sd=0, sigma_my_Sd=0, sigma_mz_Sd=0,
    )
    r = check_joint(inp)
    print(f"beta={r.beta:.3f} gamma={r.gamma:.2f} tau={r.tau:.3f} sinT={r.sin_theta:.4f}")
    print(f"Q_beta={r.Q_beta:.3f} Q_g={r.Q_g:.3f}  load_axial={r.load_axial}")
    print(f"within_range={r.within_range}  {r.validity}")
    print(f"M_Rd,ip={r.M_Rd_ip/1e3:.1f} kNm  M_Rd,op={r.M_Rd_op/1e3:.1f} kNm  (shared)")   # N·m -> kNm
    for cls in CLASSES:
        c = r.per_class[cls]
        print(f"  {cls}: Qu_ax={c.Qu_axial:.3f}  Qf_ax={c.Qf_axial:.3f}  "
              f"N_Rd={c.N_Rd/1e3:.1f} kN  util={c.util:.3f}")
    print(f"weighted N_Rd={r.N_Rd_weighted/1e3:.1f} kN  util={r.util_weighted:.3f}  "
          f"{'PASS' if r.passed else 'FAIL'}")
