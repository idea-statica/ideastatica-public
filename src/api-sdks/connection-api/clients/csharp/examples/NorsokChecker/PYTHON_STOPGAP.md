# Why the python prototype was reopened (Aug 2026)

*Read `UNIFICATION.md` first — it records the July 2026 python → C# port and states that
`reference/` is not to be extended. This file records a deliberate, temporary exception to
that rule, and what has to happen next.*

## The decision

`UNIFICATION.md` is right: the C# WPF app is the product, and the python belongs in
`reference/` as the verification oracle. We went against it anyway, knowingly.

The reason is delivery risk, not a change of mind. A customer needs a working NORSOK 6.4
tool sooner than the C# app can be made ready, and the python prototype already runs
end-to-end. Fixing the python is the fallback that guarantees the customer gets *something*
if the C# app does not get its remaining defects cleared in time.

**This does not reverse the unification.** The C# app stays the product. Every fix made here
is to be carried into it — see *Carry-over* below — and the python returns to
reference-only status once that is done.

## Ground rules we held to

- **Nothing in the calculation was touched.** `n64.py`, `n63.py` and the formula/force paths
  in `extract.py` are untouched, so the three oracle layers described in `UNIFICATION.md`
  stay valid. Every change was re-checked against `UT_NorsokChecker/TestData/live_oracle.json`:
  390 values, 0 mismatches, worst relative difference 9e-08 (tolerance 1e-6).
- **Only endpoints that exist in v3.** A customer may be on 26.0, whose service predates some
  v4 routes, so nothing was built on a v4-only endpoint. Verified by running the whole path
  against a 26.0 service via `/api/3`, not just by reading the route list. (The one v4-only
  call used during investigation — `/materials/cross-sections/library` — is not in the app.)
- **Behaviour and service access only** — messages, service lifecycle, section reading,
  display. No new features.

## What was changed

| Commit | Change |
|---|---|
| `c2428a45` | User-facing messages translated to English (9 in `app.py`, 1 comment in `extract.py`) |
| `f156e3d9` | Service is launched on an OS-assigned free port, passed with the service's own `-port=` switch, instead of assuming 5000 is available. Mirrors `ConnectionApiServiceRunner` |
| `5978e416` | **Tubular sections are recognised from the model, not the section name** (see below) |
| `8bc5d716` | D/T in member labels rounded to the chosen section decimals |
| `c3b3e815` | D/T columns in the Members table; `theta=0` no longer reported as "missing data" |
| `25dbb877` | **Chord picked by continuity + largest diameter, not by the bearing flag** (see below) |
| `27840512` | Dropped the noise warning about a bearing member that is not the chord |

### The two that matter

**Section recognition (`5978e416`).** The gate was "the name parses as `CHS<D>/<T>`". Measured
against the Eurocode cross-section library, that rejects **2641 of 2760 circular profiles
(96 %)** — `RO323.9X12.5`, `MSRR101.6x10.0`, bare `76.0x3.5`, `GB-SSP42X2.5`, and every ASME
`PIPE...SCH40`. Worse, a name can be confidently wrong: `PIPE127STD` is really D = 141.3 mm,
because 127 is the nominal size.

Now `crossSectionType` decides whether a member is tubular, and D/T come from the connection's
own IOM model (`export-iom-connection-data`, present in v3): T is the facet thickness, D is
recovered from the facet ring as `maxdist / cos(pi/n) + T`. Measured worst error 0.4 %, and
independent of the project's facet-division setting (identical D at 24 / 64 / 96 divisions) and
of cuts (verified on a model with 5 cuts and 124 welds). The name is kept only as a cross-check.

Only tubular members are measured — an I-section gives 3 facets and two thicknesses, which the
formula would happily turn into a plausible number. Rejection messages now name the real section
type instead of claiming a tube is "not CHS".

**Chord identification (`25dbb877`).** `identify_chord` preferred `isBearing` over
`isContinuous`. But the bearing flag is a modelling choice the user may put on any member — it
decides which member carries the others in the FE model, not which one is the chord. On a joint
whose bearing member is a brace, every theta / beta / gamma / gap was referenced to the wrong
member: in `test_cs.ideaCon` a 76 mm brace became the chord, beta came out 1.855 and 1.342
(outside 0.2–1.0), two gaps went negative, one brace read theta = 0 deg, and the joint was
rejected as out of scope. With the 141 mm continuous member as the chord, all five braces are
valid and the joint reports OK.

The chord is now **the continuous member with the largest diameter**, which is what 6.4 means by
a chord.

### Why the oracle did not catch either

All four benchmark files have `isBearing == isContinuous`, and their sections are named
`CHS<D>/<T>`. Both defects are invisible under those conditions — the python and the C# agree
to 1e-6 while both being wrong the same way. `test_cs.ideaCon` (five naming conventions, bearing
flag on a brace) is the file that exposes them; keep it.

## Carry-over to C#

Each of these is a real defect of the C# app too, not a python-only artefact. They are listed
smallest-blast-radius first.

| Python change | C# counterpart |
|---|---|
| Section recognition | `JointSectionMap.FromCrossSections` gates on `ChsTypes` **and** then requires `ParseChs` for the dimensions. Its `Parameters`-first path reads `ParameterDouble` "D"/"T", which **catalogue sections do not carry** — measured: a `rolledCHS` has exactly one parameter, `UniqueName`. So in practice the C# also falls back to name parsing and inherits the same 96 %. It needs the IOM facet path |
| Chord identification | **Confirmed present in C#**: `JointTopologyBuilder.IdentifyChord` is a literal port — `bearings[0]` first, continuity only as a fallback, and its own summary says so ("Chord = the bearing member; fallback continuous / largest Ø"). Same defect, same consequences |
| `theta = 0` guard | C# is **logically correct** (`JointCheckOrchestrator` tests `thetaDeg <= 0` explicitly — it never had python's falsy-zero problem) but reports the same misleading text, `"missing section/material/classification data"`, for a parallel brace. Message only |
| Free port | `ConnectionApiServiceRunner` already does this correctly; nothing to carry |
| English messages, label rounding, table columns | UI-level, re-decide in the C# UI rather than port literally |

`UNIFICATION.md`'s open item for Ondřej — "sanity-check the tolerant CHS name parser against
other naming conventions" — is **answered by this work, differently than it was framed**: the
parser should not be made more tolerant, it should stop being the source of truth. A tolerant
parser cannot solve `PIPE127STD` (nominal size) or `PIPE...SCH40` (no dimensions in the name)
at all.

## Sequence

1. Close out the python app (packaging as a one-dir build with an exe is the remaining item).
2. Hand it to the customer as the stopgap.
3. Then return to the C# app and carry the two substantive fixes over, with `test_cs.ideaCon`
   added to its test data.
4. The python goes back to reference-only, as `UNIFICATION.md` intends.

## Verification used throughout

- `live_oracle.json` re-checked after every change: 390 values, 0 mismatches. The comparison
  also asserts the chord identity, which it previously did not.
- `test_cs.ideaCon`: went from `ERROR — outside simple-joint scope` to a clean `OK`.
- Measured on **26.0** (`26.0.5.1259`) and **26.1** (`26.1.0.2007`), via `/api/3` and `/api/4` —
  identical values in all four combinations, including facet counts.
- Windows note: this branch needs `core.longpaths=true` to check out. The benchmark
  `.ideaCon` paths exceed MAX_PATH and git otherwise fails with
  `fatal: Could not reset index file`.
