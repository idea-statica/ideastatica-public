"""
`norsok` package — ALL the app's data + calculation layer in one place, so the whole app
(apps/norsok_joint_calculator/) is a single shippable folder. app.py (the pywebview entry point)
stays in the app root and does `from norsok import extract`.

- `extract` — data/API layer: reads geometry, forces, materials (fy) over the IDEA Connection
  REST API, builds the joint (plane fit, gaps), STEP 2 brace forces, STEP 3 K/Y/X classification,
  STEP 4 prep (chord stresses for Qf). `build_for(...)` returns the JSON the UI consumes.
- `n64` — NORSOK 6.4 tubular joints (simple joints): check_joint(JointInput) -> JointResult.
- `n63` — NORSOK 6.3 tubular members (CHS): check_member(MemberInput) -> MemberResult.
  NOTE: 6.3 (n63) is NOT wired into the app yet (user deferred member checks); it ships here
  ready for later, but extract.py does not import it.

n64/n63 are pure-SI (forces N, moments N·m, lengths m, stress Pa) with kN/MPa convenience builders.
They are the moved twins of python/norsok_64.py and python/norsok_63.py (kept there as a standalone
reference); formulas are identical so the two cross-check.

PRIORITY: NORSOK (clauses 6.x, LRFD gamma_M = 1.15). Do NOT mix in API RP-2A-WSD allowable-stress.
"""
from .n64 import (
    JointInput, JointResult, ClassResult, check_joint,
    Qu_axial, Qu_ipb, Qu_opb, Q_beta, Q_g, C_coeffs, Qf, CLASSES,
)
from .n63 import (
    MemberInput, MemberResult, Check, Section, check_member, f_cl, f_m,
)

__all__ = [
    "JointInput", "JointResult", "ClassResult", "check_joint",
    "Qu_axial", "Qu_ipb", "Qu_opb", "Q_beta", "Q_g", "C_coeffs", "Qf", "CLASSES",
    "MemberInput", "MemberResult", "Check", "Section", "check_member", "f_cl", "f_m",
]
