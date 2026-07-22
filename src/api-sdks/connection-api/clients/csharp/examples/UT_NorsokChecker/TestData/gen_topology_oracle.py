# -*- coding: utf-8 -*-
"""Generate topology_oracle.json from topology_fixtures.json by running the REFERENCE
python implementation (python_prototype/norsok/extract.py build_connection) offline.

The C# topology tests compare the ported pipeline against these numbers. Re-run this
script whenever the fixtures change:  python gen_topology_oracle.py
"""
import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
PROTO = os.path.abspath(os.path.join(HERE, "..", "..", "NorsokChecker", "python_prototype"))
sys.path.insert(0, PROTO)

from norsok import extract  # noqa: E402

with open(os.path.join(HERE, "topology_fixtures.json"), encoding="utf-8") as f:
    fixtures = json.load(f)

xm = extract.xs_map(fixtures["crossSections"])

oracle = {}
for fx in fixtures["fixtures"]:
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
with open(out_path, "w", encoding="utf-8") as f:
    json.dump(oracle, f, indent=1)
print(f"wrote {out_path}")
for name, o in oracle.items():
    checks = next(iter(o["joint_checks"].values()), {})
    summary = ", ".join(
        f"{b}: {('SKIP' if c.get('skipped') else f'util={c['util']:.4f} {'PASS' if c['passed'] else 'FAIL'}')}"
        for b, c in checks.items())
    print(f"  {name}: verdict={o['verdict_status']}  {summary}")
