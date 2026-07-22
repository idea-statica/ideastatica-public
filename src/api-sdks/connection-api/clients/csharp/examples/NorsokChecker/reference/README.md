# Reference implementations (verification oracle)

The **C# WPF app (`NorsokChecker`) is the product**. Everything in this folder is the
verified reference material it was ported from — kept for verification, not for use.

| Folder | What it is |
|---|---|
| `python_prototype/` | The python NORSOK §6.4 pipeline (pywebview app): `norsok/n64.py` = the §6.4 resistance engine, `norsok/extract.py` = topology + K/Y/X force-balance classification. The C# `Services/Norsok64/` is a faithful port of both; `UT_NorsokChecker` pins the port to numbers produced by RUNNING this code (see `TestData/gen_topology_oracle.py` and `gen_live_oracle.py`). |
| `verification_scripts/` | Lukáš J.'s standalone per-joint verification scripts (K, X, six T/Y load cases) — the hand-checked ground truth for Table 6-3/6-4 formulas — plus the benchmark `.ideaCon` projects (`K_CONNECTION`, `X_CONNECTION`, `TY_CONNECTION`, `TY_CONNECTION_UNIT_TEST`) used by the live validation tests. |

Rules of engagement:

- **Do not extend the python** — new functionality goes into the C# app. If a §6.4 formula
  is in doubt, the order of truth is: NORSOK N-004 Rev.3 PDF → `verification_scripts/` →
  `python_prototype/norsok/n64.py` → the C# port (which must match all three).
- The benchmark `.ideaCon` files are load-bearing: `UT_NorsokChecker`'s live validation
  (`LiveValidationTests`, `[Explicit]`) opens them through a local Connection RestAPI and
  compares the C# pipeline against `TestData/live_oracle.json` captured from the python.
