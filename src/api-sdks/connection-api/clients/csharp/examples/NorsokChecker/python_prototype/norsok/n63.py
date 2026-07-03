"""
NORSOK N-004 Rev. 3 (Feb 2013) — Chapter 6.3 Tubular members (CHS).

Library module: pure functions + dataclasses, returns structured results with all
intermediate values. Python twin of `norsok-6.3-calculator.html` — formulas kept
identical so the two implementations cross-check.

Checks implemented (all verified verbatim from the standard):
  6.3.2 axial tension (6.1)
  6.3.3 axial compression + buckling (6.2)-(6.8)
  6.3.4 bending (6.9)-(6.12)
  6.3.5 shear (6.13) and torsion (6.14)
  6.3.8.1 tension + bending (6.26)
  6.3.8.2 compression + bending (6.27)-(6.30)
  6.3.8.3 shear + bending (6.31)-(6.32)
Out of scope: 6.3.6/6.3.9 hydrostatic pressure (IDEA Connection has no surface loads).

Units: PURE SI — forces N, moments N·m, lengths m, stresses Pa. (Section props/f_cl/f_m and the
interaction checks are ratios × f_y, so the formulas are unit-agnostic given consistent inputs; the
project convention is strict SI.) Builders: from_kN (mm/MPa/kN/kNm convenience) or from_SI.
gamma_M = 1.15 base (6.3.7); variable gamma_M per (6.22) only applies to hydrostatic
combos (out of scope), so a single gamma_M is used here (default 1.15, overridable).

PRIORITY: NORSOK (clauses 6.x, LRFD gamma_M). Not API RP-2A-WSD.
"""
from __future__ import annotations
from dataclasses import dataclass, asdict, field
from math import pi, sqrt, sin, radians


# --------------------------------------------------------------------------- #
# Section properties (CHS)
# --------------------------------------------------------------------------- #
@dataclass
class Section:
    D: float
    t: float
    di: float = field(init=False)
    A: float = field(init=False)
    I: float = field(init=False)
    W: float = field(init=False)   # elastic section modulus
    Z: float = field(init=False)   # plastic section modulus
    i: float = field(init=False)   # radius of gyration
    Ip: float = field(init=False)  # polar moment

    def __post_init__(self):
        D, t = self.D, self.t
        self.di = D - 2 * t
        self.A = pi / 4 * (D * D - self.di ** 2)
        self.I = pi / 64 * (D ** 4 - self.di ** 4)
        self.W = pi / 32 * (D ** 4 - self.di ** 4) / D
        self.Z = (1 / 6) * (D ** 3 - self.di ** 3)
        self.i = sqrt(self.I / self.A)
        self.Ip = pi / 32 * (D ** 4 - self.di ** 4)


# --------------------------------------------------------------------------- #
# Material strengths
# --------------------------------------------------------------------------- #
def f_cl(fy: float, E: float, D: float, t: float) -> tuple[float, float, float, str]:
    """Characteristic local buckling strength f_cl (6.6)-(6.8) + f_cle, ratio, eq used."""
    Cx = 0.3
    fcle = 2 * Cx * E * t / D
    r = fy / fcle
    if r <= 0.170:
        return fy, fcle, r, "6.6"
    if r <= 1.911:
        return (1.047 - 0.274 * r) * fy, fcle, r, "6.7"
    return fcle, fcle, r, "6.8"


def f_m(fy: float, E: float, D: float, t: float, Z: float, W: float) -> tuple[float, float, str, str | None]:
    """Characteristic bending strength f_m (6.10)-(6.12) + parameter p, eq used, optional warning.
    NB eq(6.12) is only valid up to f_y*D/(E*t) <= 120*f_y/E  (i.e. D/t <= 120)."""
    p = fy * D / (E * t)
    if p <= 0.0517:
        return (Z / W) * fy, p, "6.10", None
    if p <= 0.1034:
        return (1.13 - 2.58 * p) * (Z / W) * fy, p, "6.11", None
    warn = None
    if (D / t) > 120:
        warn = (f"D/t = {D / t:.1f} > 120 — outside the stated validity of eq(6.12) "
                f"(valid for D/t <= 120, i.e. f_y*D/(E*t) <= 120*f_y/E); f_m extrapolated.")
    return (0.94 - 0.76 * p) * (Z / W) * fy, p, "6.12", warn


# --------------------------------------------------------------------------- #
# Inputs
# --------------------------------------------------------------------------- #
@dataclass
class MemberInput:
    """Canonical fields are PURE SI: lengths m, stress Pa, force N, moment N·m."""
    D: float           # outer diameter [m]
    t: float           # wall thickness [m]
    fy: float          # yield strength [Pa]
    E: float = 210e9   # Young's modulus [Pa]  (210000 MPa)
    L: float = 6.0     # system length [m]
    ky: float = 1.0
    kz: float = 1.0
    Cmy: float = 0.85
    Cmz: float = 0.85
    N_Sd: float = 0.0    # [N]  (+ tension / - compression)
    My_Sd: float = 0.0   # in-plane bending [N·m]
    Mz_Sd: float = 0.0   # out-of-plane bending [N·m]
    Vy_Sd: float = 0.0   # shear y [N]
    Vz_Sd: float = 0.0   # shear z [N]
    MT_Sd: float = 0.0   # torsion [N·m]
    gamma_M: float = 1.15

    @classmethod
    def from_SI(cls, **kw) -> "MemberInput":
        """Explicit pure-SI builder (alias of the constructor) — lengths m, stress Pa, force N, moment N·m."""
        return cls(**kw)

    @classmethod
    def from_kN(cls, *, D, t, fy, E=210000.0, L=6000.0, ky=1.0, kz=1.0, Cmy=0.85, Cmz=0.85,
                N_Sd_kN=0.0, My_Sd_kNm=0.0, Mz_Sd_kNm=0.0,
                Vy_Sd_kN=0.0, Vz_Sd_kN=0.0, MT_Sd_kNm=0.0, gamma_M=1.15) -> "MemberInput":
        """Convenience builder in engineering units: D/t/L in mm, fy/E in MPa, forces kN, moments kNm.
        Converts everything to the canonical SI fields."""
        MM, MPA = 1e-3, 1e6
        return cls(D=D * MM, t=t * MM, fy=fy * MPA, E=E * MPA, L=L * MM, ky=ky, kz=kz, Cmy=Cmy, Cmz=Cmz,
                   N_Sd=N_Sd_kN * 1e3, My_Sd=My_Sd_kNm * 1e3, Mz_Sd=Mz_Sd_kNm * 1e3,
                   Vy_Sd=Vy_Sd_kN * 1e3, Vz_Sd=Vz_Sd_kN * 1e3, MT_Sd=MT_Sd_kNm * 1e3,
                   gamma_M=gamma_M)


@dataclass
class Check:
    name: str
    clause: str
    util: float          # utilisation (<=1 pass); -1 = N/A
    passed: bool
    detail: dict         # intermediate values


@dataclass
class MemberResult:
    section: dict
    f_cle: float
    f_cl: float
    f_cl_eq: str
    f_m: float
    f_m_eq: str
    M_Rd: float          # [N·m]
    checks: list[Check]
    governing: str
    max_util: float
    warnings: list[str]

    def as_dict(self) -> dict:
        d = asdict(self)
        return d


# --------------------------------------------------------------------------- #
# Main entry point
# --------------------------------------------------------------------------- #
def check_member(inp: MemberInput) -> MemberResult:
    """Run all NORSOK 6.3 member checks, returning intermediate values per check."""
    S = Section(inp.D, inp.t)
    fy, E, gM = inp.fy, inp.E, inp.gamma_M
    fcl, fcle, ratio, fcl_eq = f_cl(fy, E, inp.D, inp.t)
    fm, p, fm_eq, fm_warn = f_m(fy, E, inp.D, inp.t, S.Z, S.W)
    MRd = fm * S.W / gM
    Mmag = sqrt(inp.My_Sd ** 2 + inp.Mz_Sd ** 2)
    Vmag = sqrt(inp.Vy_Sd ** 2 + inp.Vz_Sd ** 2)

    checks: list[Check] = []

    # 6.3.2 axial tension
    if inp.N_Sd > 0:
        NtRd = S.A * fy / gM
        u = inp.N_Sd / NtRd
        checks.append(Check("Axial tension", "6.3.2 (6.1)", u, u <= 1.0,
                            {"N_t_Rd": NtRd, "N_Sd": inp.N_Sd}))

    # 6.3.3 axial compression
    NcRd = None
    if inp.N_Sd < 0:
        kgov = max(inp.ky, inp.kz)
        lam = (kgov * inp.L / (pi * S.i)) * sqrt(fcl / E)
        fc = (1.0 - 0.28 * lam * lam) * fcl if lam <= 1.34 else (0.9 / (lam * lam)) * fcl
        NcRd = S.A * fc / gM
        u = abs(inp.N_Sd) / NcRd
        checks.append(Check("Axial compression", "6.3.3 (6.2)-(6.8)", u, u <= 1.0,
                            {"f_cle": fcle, "f_cl": fcl, "f_cl_eq": fcl_eq, "k_gov": kgov,
                             "lambda_bar": lam, "f_c": fc, "N_c_Rd": NcRd, "N_Sd": inp.N_Sd}))

    # 6.3.4 bending
    if Mmag > 0:
        u = Mmag / MRd
        checks.append(Check("Bending", "6.3.4 (6.9)-(6.12)", u, u <= 1.0,
                            {"f_m": fm, "f_m_eq": fm_eq, "p": p, "M_Rd": MRd, "M_Sd": Mmag,
                             "shape_factor_ZW": S.Z / S.W}))

    # 6.3.5 shear
    VRd = S.A * fy / (2 * sqrt(3) * gM)
    if Vmag > 0:
        u = Vmag / VRd
        checks.append(Check("Shear", "6.3.5 (6.13)", u, u <= 1.0,
                            {"V_Rd": VRd, "V_Sd": Vmag, "Vy_Sd": inp.Vy_Sd, "Vz_Sd": inp.Vz_Sd}))

    # 6.3.5 torsion
    MtRd = None
    if abs(inp.MT_Sd) > 0:
        MtRd = 2 * S.Ip * fy / (inp.D * sqrt(3) * gM)
        u = abs(inp.MT_Sd) / MtRd
        checks.append(Check("Torsion", "6.3.5 (6.14)", u, u <= 1.0,
                            {"M_T_Rd": MtRd, "M_T_Sd": inp.MT_Sd}))

    # 6.3.8.1 tension + bending
    if inp.N_Sd > 0 and Mmag > 0:
        NtRd = S.A * fy / gM
        t1 = (inp.N_Sd / NtRd) ** 1.75
        t2 = Mmag / MRd
        u = t1 + t2
        checks.append(Check("Combined: tension + bending", "6.3.8.1 (6.26)", u, u <= 1.0,
                            {"term_axial^1.75": t1, "term_bending": t2}))

    # 6.3.8.2 compression + bending
    if inp.N_Sd < 0 and NcRd is not None:
        NEy = pi * pi * E * S.A / (inp.ky * inp.L / S.i) ** 2
        NEz = pi * pi * E * S.A / (inp.kz * inp.L / S.i) ** 2
        NclRd = fcl * S.A / gM
        Nabs = abs(inp.N_Sd)
        ay = inp.Cmy * inp.My_Sd / (1 - Nabs / NEy)
        az = inp.Cmz * inp.Mz_Sd / (1 - Nabs / NEz)
        amp = sqrt(ay * ay + az * az)
        u1 = Nabs / NcRd + amp / MRd                 # (6.27) stability
        u2 = Nabs / NclRd + Mmag / MRd               # (6.28) section
        u = max(u1, u2)
        checks.append(Check("Combined: compression + bending", "6.3.8.2 (6.27)-(6.30)", u, u <= 1.0,
                            {"N_Ey": NEy, "N_Ez": NEz, "N_cl_Rd": NclRd,
                             "util_stability_6.27": u1, "util_section_6.28": u2}))

    # 6.3.8.3 shear + bending
    if Vmag > 0 and Mmag > 0:
        rv = Vmag / VRd
        if rv >= 0.4:
            u = (Mmag / MRd) / sqrt(1.4 - rv)        # (6.31)
            note = "V/V_Rd>=0.4 (6.31); valid only if shear & moment vectors orthogonal within +/-20deg"
        else:
            u = Mmag / MRd                            # (6.32)
            note = "V/V_Rd<0.4 (6.32)"
        checks.append(Check("Combined: shear + bending", "6.3.8.3 (6.31)-(6.32)", u, u <= 1.0,
                            {"V/V_Rd": rv, "note": note}))

    real = [c for c in checks if c.util >= 0]
    if real:
        gov = max(real, key=lambda c: c.util)
        governing, max_util = gov.name, gov.util
    else:
        governing, max_util = "—", -1.0

    return MemberResult(
        section=asdict(S), f_cle=fcle, f_cl=fcl, f_cl_eq=fcl_eq, f_m=fm, f_m_eq=fm_eq,
        M_Rd=MRd, checks=checks, governing=governing, max_util=max_util,
        warnings=[w for w in [fm_warn] if w],
    )


# --------------------------------------------------------------------------- #
# Self-test (run: python norsok_63.py)
# --------------------------------------------------------------------------- #
if __name__ == "__main__":
    inp = MemberInput.from_kN(
        D=508, t=16, fy=355, E=210000, L=6000, ky=1.0, kz=1.0, Cmy=0.85, Cmz=0.85,
        N_Sd_kN=1200, My_Sd_kNm=80, Mz_Sd_kNm=20, Vy_Sd_kN=100, Vz_Sd_kN=0, MT_Sd_kNm=0,
    )
    r = check_member(inp)
    # display: section A in mm² (m²·1e6), f_cl/f_m in MPa (Pa/1e6), M_Rd in kNm (N·m/1e3)
    print(f"A={r.section['A']*1e6:.0f} mm2  Z/W={r.section['Z']/r.section['W']:.3f}")
    print(f"f_cl={r.f_cl/1e6:.1f} MPa ({r.f_cl_eq})  f_m={r.f_m/1e6:.1f} MPa ({r.f_m_eq})  M_Rd={r.M_Rd/1e3:.1f} kNm")
    for c in r.checks:
        print(f"  [{c.clause}] {c.name}: util={c.util:.3f} {'PASS' if c.passed else 'FAIL'}")
    print(f"governing: {r.governing}  max_util={r.max_util:.3f}")
