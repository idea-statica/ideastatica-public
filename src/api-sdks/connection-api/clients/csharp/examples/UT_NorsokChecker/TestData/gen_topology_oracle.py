# -*- coding: utf-8 -*-
"""Generate topology_oracle.json from topology_fixtures.json by running the REFERENCE
python implementation (python_prototype/norsok/extract.py build_connection) offline.

The C# topology tests compare the ported pipeline against these numbers.

    python gen_topology_oracle.py            regenerate the oracle
    python gen_topology_oracle.py --check    diff against the stored oracle, exit 1 on any
                                             difference and write nothing

USE --check FIRST. An oracle its generator cannot reproduce is a hand-edited file, and the tests
then pin the edits rather than the reference. That is what this file had become: running it
overwrote every fixture's entry with an ERROR verdict, because the fixtures said
crossSectionType "CHS" while extract.TUBULAR_TYPES wants "rolledchs" -- so the reference answered
"CHS457/16.0 is CHS -- NORSOK 6.4 applies to tubular sections only", which is self-contradictory
prose and was the tell. The fixture data is corrected; --check is here so the next divergence is
found by running the script instead of by trusting it.

TWO KNOWN, DELIBERATE DIVERGENCES remain, and they are skipped rather than papered over:

  * CON17_RIGID_SHIFT / CON18_BRACES_ONLY -- the C# measures a brace's out-of-plane offset from the
    joint plane THROUGH THE CHORD; this reference measures it from the work point. That was the
    round-3 fix (a joint displaced as a rigid body must be assessed), so the reference rejects
    CON17 where the app accepts it. Deliberate: the app is right and the python is the outlier.

  * M_op sign on K_TEST/BA and KT_TEST/KV -- the C# uses a right-handed brace frame, so two of the
    stored M_op values are negated relative to this reference. See BraceFrameTests.
"""
import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
PROTO = os.path.abspath(os.path.join(HERE, "..", "..", "NorsokChecker", "reference", "python_prototype"))
sys.path.insert(0, PROTO)

from norsok import extract  # noqa: E402

with open(os.path.join(HERE, "topology_fixtures.json"), encoding="utf-8") as f:
    fixtures = json.load(f)

xm = extract.xs_map(fixtures["crossSections"])

CHECK = "--check" in sys.argv[1:]

# Fixtures the reference cannot reproduce by design (see the module docstring). Skipped rather
# than regenerated, so --check does not report a difference that is a decision.
SKIP = ("CON17", "CON18")

# Individual leaves where the C# is deliberately the opposite sign, so --check can be a gate
# instead of a wall of known noise. The app resolves each brace in a RIGHT-HANDED frame
# (n_b = ex x brace_dir), this reference does not, and the norm signs neither -- eq (6.57) takes
# |M_op|, so no resistance moves. BraceFrameTests pins the app's convention.
#
# Listed as exact paths, not as a blanket "ignore M_op": a sign flip anywhere ELSE is a finding,
# and these two are the only ones measured.
SIGN_FLIPPED = {
    "K_TEST/brace_forces/LE1/BA/M_op",
    "KT_TEST/brace_forces/LE1/KV/M_op",
}

oracle = {}
skipped = []
for fx in fixtures["fixtures"]:
    if fx["name"].startswith(SKIP):
        skipped.append(fx["name"])
        continue
    conn = {"id": fx["connectionId"], "name": fx["name"]}
    data = extract.build_connection(None, None, conn, fx["members"], xm,
                                    load_effects=fx["loadEffects"])
    # compact, stable subset for the C# assertions
    o = {
        "chord": data["chord"]["name"],
        "verdict_status": data["verdict"]["status"],
        "verdict_errors": data["verdict"]["errors"],
        "braces": {bm["name"]: {
            "theta_deg": bm["theta_deg"],
            "beta": bm["beta"],
            "coplanar_dev_deg": bm["coplanar_dev_deg"],
        } for bm in data["braces"]},
        "gaps": [{"a": g["between"][0], "b": g["between"][1], "gap_m": g["gap_m"],
                  "side": g["side"], "adjacent": g["adjacent"]} for g in data["gaps"]],
        "equilibrium": [{"id": e["id"], "resF_N": e["resF_N"], "resM_Nm": e["resM_Nm"]}
                        for e in data["equilibrium"]],
        "brace_forces": {le["name"]: {r["name"]: {
            "N_Sd": r["N_Sd"], "M_ip": r["M_ip"], "M_op": r["M_op"],
            "V_ip": r["V_ip"], "V_op": r["V_op"], "side": r["side"],
        } for r in le["braces"]} for le in data["brace_forces"]},
        "chord_stresses": {le["name"]: {r["name"]: {
            "sigma_a": r["sigma_a"], "sigma_my": r["sigma_my"], "sigma_mz": r["sigma_mz"],
        } for r in le["braces"]} for le in data["chord_stresses"]},
        "classification": {le["name"]: {c["name"]: {
            "frK": c["frK"], "frX": c["frX"], "frY": c["frY"], "q_trans": c["q_trans"],
            "K_components": [{"partner": k["partner"], "gap_m": k["gap_m"], "frac": k["frac"]}
                             for k in c["K_components"]],
        } for c in le["classes"]} for le in data["classification"]},
        "joint_checks": {le["name"]: {r["name"]: (
            {"skipped": True, "reason": r.get("reason")} if r.get("skipped") else {
                "skipped": False,
                "util": r["util"], "passed": r["passed"],
                "N_Rd_weighted": r["N_Rd_weighted"],
                "M_Rd_ip": r["M_Rd_ip"], "M_Rd_op": r["M_Rd_op"],
                "within_range": r["within_range"],
                "chord_overstressed": r["chord_overstressed"],
                "dom_class": r["dom_class"],
            }) for r in le["braces"]} for le in data["joint_checks"]},
    }
    oracle[fx["name"]] = o

out_path = os.path.join(HERE, "topology_oracle.json")

if skipped:
    print("skipped (deliberate divergence, see the docstring): " + ", ".join(skipped))

if CHECK:
    # Compare, report, write NOTHING. Every leaf is compared with a tolerance, because the point is
    # whether the reference still produces these numbers and not whether it prints them to the same
    # width -- an exact string diff on floats would fail on a harmless repr change.
    with open(out_path, encoding="utf-8") as f:
        stored = json.load(f)

    diffs = []

    def walk(path, a, b):
        if isinstance(a, dict) and isinstance(b, dict):
            for k in sorted(set(a) | set(b)):
                if k not in a:
                    diffs.append(f"{path}/{k}: only in the stored oracle")
                elif k not in b:
                    diffs.append(f"{path}/{k}: only in the regenerated oracle")
                else:
                    walk(f"{path}/{k}", a[k], b[k])
        elif isinstance(a, list) and isinstance(b, list):
            if len(a) != len(b):
                diffs.append(f"{path}: {len(a)} entries stored, {len(b)} regenerated")
            else:
                for i, (x, y) in enumerate(zip(a, b)):
                    walk(f"{path}[{i}]", x, y)
        elif isinstance(a, (int, float)) and isinstance(b, (int, float)) \
                and not isinstance(a, bool) and not isinstance(b, bool):
            # A recorded sign flip is compared on magnitude; anything else on value.
            if path in SIGN_FLIPPED:
                if abs(abs(a) - abs(b)) > max(1e-6, abs(a) * 1e-6):
                    diffs.append(f"{path}: |stored| {abs(a)}, |regenerated| {abs(b)} "
                                 "(recorded sign flip, but the MAGNITUDE moved)")
            elif abs(a - b) > max(1e-6, abs(a) * 1e-6):
                diffs.append(f"{path}: stored {a}, regenerated {b}")
        elif a != b:
            diffs.append(f"{path}: stored {a!r}, regenerated {b!r}")

    for name in sorted(set(stored) | set(oracle)):
        if name.startswith(SKIP):
            continue
        if name not in stored:
            diffs.append(f"{name}: only in the regenerated oracle")
        elif name not in oracle:
            diffs.append(f"{name}: only in the stored oracle")
        else:
            walk(name, stored[name], oracle[name])

    if diffs:
        print(f"\n{len(diffs)} difference(s) against {out_path}:")
        for d in diffs[:40]:
            print("  " + d)
        if len(diffs) > 40:
            print(f"  ... and {len(diffs) - 40} more")
        print("\nThe stored oracle is NOT what the reference produces. Decide which is right "
              "before regenerating: a deliberate divergence belongs in SKIP with its reason.")
        sys.exit(1)
    print(f"OK - the reference reproduces {out_path} "
          f"({len(oracle)} fixture(s) compared)")
    sys.exit(0)

with open(out_path, "w", encoding="utf-8") as f:
    json.dump(oracle, f, indent=1)
print(f"wrote {out_path}")
for name, o in oracle.items():
    checks = next(iter(o["joint_checks"].values()), {})
    summary = ", ".join(
        f"{b}: {('SKIP' if c.get('skipped') else f'util={c['util']:.4f} {'PASS' if c['passed'] else 'FAIL'}')}"
        for b, c in checks.items())
    print(f"  {name}: verdict={o['verdict_status']}  {summary}")
