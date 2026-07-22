# -*- coding: utf-8 -*-
"""Generate live_oracle.json: run the REFERENCE python pipeline (reference/python_prototype)
over the four benchmark .ideaCon projects through a LOCAL Connection RestAPI, and capture
per-connection / per-LE / per-brace NORSOK 6.4 results.

The C# live validation test (LiveValidationTests, [Explicit]) runs the ported pipeline over
the same files against the same service and compares. Re-run when the benchmarks change:

  python gen_live_oracle.py [service_exe_dir] [port]

Defaults: "C:/Program Files/IDEA StatiCa/StatiCa 26.1", port 5100.
"""
import json
import os
import subprocess
import sys
import time

import requests

HERE = os.path.dirname(os.path.abspath(__file__))
NORSOK_DIR = os.path.abspath(os.path.join(HERE, "..", "..", "NorsokChecker"))
PROTO = os.path.join(NORSOK_DIR, "reference", "python_prototype")
SCRIPTS = os.path.join(NORSOK_DIR, "reference", "verification_scripts")
sys.path.insert(0, PROTO)

from norsok import extract  # noqa: E402

SETUP_DIR = sys.argv[1] if len(sys.argv) > 1 else r"C:\Program Files\IDEA StatiCa\StatiCa 26.1"
PORT = int(sys.argv[2]) if len(sys.argv) > 2 else 5100

BENCHMARKS = [
    ("K_CONNECTION", os.path.join(SCRIPTS, "NORSOK CHAPTER 6.4 K AND KT-JOINTS", "K_CONNECTION.ideaCon")),
    ("X_CONNECTION", os.path.join(SCRIPTS, "NORSOK CHAPTER 6.4 X CONNECTION", "X_CONNECTION.ideaCon")),
    ("TY_CONNECTION", os.path.join(SCRIPTS, "NORSOK CHAPTER 6.4 T_Y CONNECTION", "TY_CONNECTION.ideaCon")),
    ("TY_CONNECTION_UNIT_TEST", os.path.join(SCRIPTS, "NORSOK CHAPTER 6.4 T_Y CONNECTION",
                                             "NORSOK_TY_CONNECTIONS_UNIT_TESTS", "TY_CONNECTION_UNIT_TEST.ideaCon")),
]


def wait_ready(base, timeout_s=60):
    t0 = time.time()
    while time.time() - t0 < timeout_s:
        try:
            if requests.get(f"{base}/heartbeat", timeout=3).status_code == 200:
                return True
        except Exception:
            pass
        time.sleep(1)
    return False


def main():
    exe = os.path.join(SETUP_DIR, "IdeaStatiCa.ConnectionRestApi.exe")
    base = f"http://localhost:{PORT}"
    extract.BASE = f"{base}/api/4"
    proc = subprocess.Popen([exe, f"-port={PORT}"], cwd=SETUP_DIR,
                            stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    try:
        if not wait_ready(base):
            raise RuntimeError("RestAPI did not become ready")
        print(f"service ready on {base} ({SETUP_DIR})")

        oracle = {"service_setup_dir": SETUP_DIR, "benchmarks": {}}
        for name, path in BENCHMARKS:
            s = requests.Session()
            extract.connect(s)
            pid, conns = extract.open_and_list(s, path)
            entry = {"connections": {}}
            for c in conns:
                d = extract.build_for(s, pid, c["id"])
                per_le = {}
                for le in d["joint_checks"]:
                    per_le[str(le["id"])] = {r["name"]: (
                        {"skipped": True, "reason": r.get("reason")} if r.get("skipped") else {
                            "skipped": False,
                            "util": r["util"], "passed": r["passed"],
                            "N_Rd_weighted": r["N_Rd_weighted"],
                            "M_Rd_ip": r["M_Rd_ip"], "M_Rd_op": r["M_Rd_op"],
                            "within_range": r["within_range"],
                            "chord_overstressed": r["chord_overstressed"],
                            "frK": r["inputs"]["frK"], "frY": r["inputs"]["frY"], "frX": r["inputs"]["frX"],
                            "N_Sd": r["inputs"]["N_Sd"], "M_ip_Sd": r["inputs"]["M_ip_Sd"],
                            "M_op_Sd": r["inputs"]["M_op_Sd"],
                            "sigma_a": r["inputs"]["sigma_a"], "sigma_my": r["inputs"]["sigma_my"],
                            "sigma_mz": r["inputs"]["sigma_mz"],
                            "theta_deg": r["inputs"]["theta_deg"], "beta": r["inputs"]["beta"],
                        }) for r in le["braces"]}
                entry["connections"][c.get("name") or str(c["id"])] = {
                    "id": c["id"],
                    "chord": d["chord"]["name"] if d["chord"] else None,
                    "verdict": d["verdict"]["status"],
                    "gaps": [{"a": g["between"][0], "b": g["between"][1], "gap_m": g["gap_m"],
                              "adjacent": g["adjacent"]} for g in d["gaps"]],
                    "joint_checks": per_le,
                }
                worst = max((r["util"] for le in d["joint_checks"] for r in le["braces"]
                             if not r.get("skipped")), default=None)
                print(f"  {name}/{c.get('name')}: verdict={d['verdict']['status']} "
                      f"braces={len(d['braces'])} worst_util={worst if worst is None else round(worst, 4)}")
            extract.close_project(s, pid)
            oracle["benchmarks"][name] = entry

        out = os.path.join(HERE, "live_oracle.json")
        with open(out, "w", encoding="utf-8") as f:
            json.dump(oracle, f, indent=1)
        print(f"wrote {out}")
    finally:
        proc.kill()


if __name__ == "__main__":
    main()
