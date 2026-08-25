# Gate coverage — `test_cs.ideaCon`

The other files under `verification_scripts/` are numerical benchmarks: they pin the §6.4
formulas to known utilisations (`UT_NorsokChecker/TestData/live_oracle.json`). This one does the
opposite — it checks that a joint **outside** the scope of §6.4 is correctly refused, and that
the reason given is the true one.

It exists because two real defects were invisible on the numerical benchmarks. All four of those
have `isBearing == isContinuous` and sections named `CHS<D>/<T>`, and under those two conditions
both defects are silent: the python and the C# agreed to 1e-6 while both being wrong the same
way. Keep this file for exactly that reason.

## One connection per condition

| | covers | expected |
|---|---|---|
| **CON1** | five section-naming conventions: `76.0x3.5`, `PIPE127STD`, `PIPE(Imp)3-1/2XS`, `GB-SSP42X2.5`, `CHS30,3` — only the last one is parseable as `CHS<D>/<T>` | **OK**, 5 checks, every D/T taken from the model |
| **CON2** | a member that is not a circular hollow section (`IPE100`) | ERROR naming the real section type |
| **CON3** | out-of-plane eccentricity, 10 mm | ERROR |
| **CON4** | eccentricity **and** overlapping brace feet | ERROR, two gates at once |
| **CON5** | six gates at once, including a brace at θ = 0° | ERROR listing all six |
| **CON6** | **no continuous member** | ERROR: §6.4 needs a through chord |
| **CON7** | **two continuous members** | ERROR: chord taken as the larger diameter |

The bearing flag sits on a brace throughout — that is what surfaced the chord defect.

## The two defects it catches

**Sections recognised by name.** The gate used to be "the name parses as `CHS<D>/<T>`". Measured
against the Eurocode cross-section library that rejects 2641 of 2760 circular profiles (96 %).
Worse, a name can be actively wrong: `PIPE127STD` is D = 141.3 mm, because 127 is the nominal
size — so a parsed name is not merely missing, it can be confidently incorrect. CON1 is built
from names that fail in each of those ways.

**Chord picked by the bearing flag.** `isBearing` is a modelling choice the user may put on any
member; it says which member carries the others in the FE model, not which one is the chord. When
the bearing member is a brace, every θ / β / γ / gap is referenced to the wrong member. On CON1
that made a 76 mm brace the chord: β came out 1.855 and 1.342 (outside 0.2–1.0), two gaps went
negative, one brace read θ = 0°, and the joint was rejected as out of scope — a joint that is in
fact perfectly valid.

## Why nothing is reported for a rejected joint

CON2–CON7 produce **no** checks, no chord stresses and no classification — not merely a failed
check. Every error gate means the quantities the check rests on are properties of the whole
joint: the joint plane, the chord stresses averaged across it, the K/Y/X balance over all braces.
The chord stresses make it concrete — they are the *resultant* chord force over the chord
section, i.e. a sum over every brace, so on CON2 the resultant includes an I-section with no D/T
at all. Publishing per-brace utilisations next to that would read as "failed, but here are the
numbers".

Only `brace_forces` survives, being each brace's own section forces resolved into the plane.

Warnings do not block: the 6.4.3.1 validity ranges (β, γ, θ) are warnings deliberately, because
the norm's rule there is to compute with the parameters clamped and keep the lesser capacity.

## Running it

Open the file in the app (`../../python_prototype`) and step through the connections, or drive it
directly:

```python
from norsok import extract
extract.set_base("http://127.0.0.1:5000/api/4")
pid = extract.open_project(session, "test_cs.ideaCon")
for conn in extract.list_connections(session, pid):
    d = extract.build_for(session, pid, conn["id"])
    print(conn["name"], d["verdict"]["status"],
          sum(len(le["braces"]) for le in d["joint_checks"]))
```

Verified on IDEA StatiCa 26.0 and 26.1, over both `/api/3` and `/api/4`.

Background: [`../../../PYTHON_STOPGAP.md`](../../../PYTHON_STOPGAP.md).
