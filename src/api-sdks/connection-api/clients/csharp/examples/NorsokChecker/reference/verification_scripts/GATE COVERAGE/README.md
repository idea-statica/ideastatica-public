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
| **CON8** | an **oblique joint plane** with no member's local CSYS aligned to it — see below | **OK**, and the M_ip / M_op split is genuinely resolved rather than read off my / mz |
| **CON9** | the chord is an **I-section** (`IPE100`) | ERROR naming `RolledI` — §6.4 is for tubular sections |
| **CON10** | the **chord alone**, every brace deleted | ERROR: "No brace (chord only)" |
| **CON11** | a brace at **θ = 20°** — above the 5° error floor, below the 30° warning floor | OK with a warning: θ outside 30–90° |
| **CON12** | a brace **60 mm eccentric along the chord** (D/4 = 35 mm on `PIPE127STD`) | OK with a warning: eccentricity along the chord |
| **CON13** | one brace **8° out of plane**, the other four coplanar | OK with a warning: borderline off plane |
| **CON14** | brace tilts for which the plane cannot be found, only **fitted** | OK with the plane-fit warning; basis "closest pair" |

CON9–CON14 are copies of CON1, so they keep its five CUT operations, with one thing changed each.
Their load effects were re-solved for the new geometry (residual 0.000 kN/kNm) and then **scaled by
1/100** to undo the unit mistake described under CON1 — equilibrium is linear, so a uniform factor
keeps the residual at zero while the utilisation scales with it. CON9 needed 1/280 rather than
1/100: its chord is an IPE100 (the not-tubular gate), far weaker than the `PIPE127STD` the forces
were solved against, so the same loads utilise it several times as hard. CON10 is the exception:
deleting the braces takes their loadings with them, so its inherited load effects reference members
that no longer exist and the service answers 404 — the connection has no usable load effect, which
is fine for a gate that fires before any load is read.

Three notes from building them, each of which cost an attempt:

- **A tilted brace moves the plane, it does not leave it.** The plane is fitted from the members, so
  tilting one brace by 8° put THAT brace at 0° and the other four at 6–9°. CON13 tilts M6 and leaves
  the other four in XY, so the fit lands where it is supposed to.
- **The plane-fit warning (CON14) cannot be derived by hand.** "Are any two braces within 2° of a
  common plane" is not the builder's test: it tries each brace's own perpendicular as a candidate and
  counts inliers against that, so the answer depends on the seed. Two hand-derived spreads were
  wrong. The tilts used come from `PlaneFitSearchTests` (marked `Explicit`), which asks the builder.
- **A synthetic fixture is not CON1.** `GateCoverageTests` uses a 273 mm chord with three 48.3 mm
  braces rather than CON1's angles, because CON1 relies on its CUT operations to resolve overlapping
  feet and a fixture has none — copying its bearings produced "feet overlap (gap −70 mm)" and the
  joint was rejected before the gate under test could speak.

Still unreached, and why: a connection with **no members** (probably not constructible),
**`forcesIn` other than node/position** (a model-level setting these scripts do not touch), and the
**missing-data skip row** (in C# the not-tubular gate fires first, so it cannot be isolated).

The bearing flag sits on a brace throughout — that is what surfaced the chord defect.

## CON8 — an oblique joint plane, so the in-plane / out-of-plane split is actually exercised

CON1–CON7 all lie in the global XY plane with every member's local CSYS aligned to it
(`axisZ == global Z` on all of them). In that arrangement `JointForceResolver` has nothing to do:
local `mz` **is** the in-plane moment and local `my` **is** the out-of-plane one. A bug that swapped
them, dropped the projection, or fitted the wrong plane normal would produce identical numbers — so
none of those seven joints can catch it.

CON8 is a copy of CON1 — so its five CUT operations survive, and with them CON1's resolution of the
brace feet — with the **whole layout rotated into an oblique plane**. Nothing else changes:

| | CON1 | CON8 |
|---|---|---|
| plane normal | `(0, 0, 1)` | `(0.660, −0.660, −0.358)` — **49°** from every global axis |
| in-plane angles (M1 / M4 / M3 / M5 / M6) | +60 / +125 / +140 / −120 / −90 | **identical** |
| θ per brace | 60 / 55 / 40 / 60 / 90 | **identical** |
| members off the plane | 0.00° | 0.00° |
| each member's local z vs the plane normal | **0°** (aligned) | **50.9° … 69.0°** |

The in-plane projection is CON1's, number for number, which is the point: the joint is no more of an
overlap joint than CON1 is, the same cuts apply, and it stays assessable. What differs is only that
the plane is oblique — and because the service derives `axisY = normalize(globalZ × axisX)`, that
alone makes every member's local frame non-parallel to the joint plane. The member **axes stay in the
plane**; nothing is rotated out of it, and `alphaRotation` plays no part.

That tilt is what the joint exists for. In CON1 every local z sits on the plane normal, so local `mz`
**is** the in-plane moment and local `my` **is** the out-of-plane one — a resolver that swapped them,
dropped the projection or fitted the wrong normal would give identical numbers. In CON8 the closest
any frame comes is 50.9°, so a unit `mz` on any brace splits across **both** `M_ip` and `M_op`.

A first attempt got this wrong in a way worth recording: searching for directions with
well-tilted frames changed the in-plane angles, which pushed θ to 28° on two braces — below the 30°
§6.4 requires, making the joint unassessable, and losing the projection that was the whole
requirement. The layout is fixed; only the plane's orientation is free.

Its 15 load effects are the inherited ones **re-solved for the new directions** — rotating a member
invalidates a balance computed for the old one. Brace magnitudes are kept (they were sized against
each member's own resistance, which has not changed) and the chord M2 re-absorbs the remainder,
then the whole set is scaled by 1/100 as above. Verified on the saved file: residual
**0.000000 kN/kNm** on all 15, worst **0.398 axial+bending and 0.196 shear** as ratios — i.e.
39.8 % and 19.6 % of characteristic capacity.

## CON1 also carries 15 load effects

The other connections keep their single load effect. CON1 was given 14 more (`LE2`..`LE15`) so
that the envelope has something to choose between — with one state there is no governing state to
report, and the per-brace envelope cannot be exercised at all.

How they were built, and what holds for every one of them:

- **every brace is loaded in every state** (M1, M3, M4, M5, M6), and the chord M2 carries the
  imbalance across its two sides;
- **node equilibrium is exact**: residual 0.00000 kN / 0.00000 kNm on all 14, verified on the
  *saved* file rather than only on the generated numbers. (`LE1`, which predates this, carries
  0.0006.)
- **per member `|n| + |my| + |mz| ≤ 0.40`** and **`|vy| + |vz| ≤ 0.20`**, in the RATIOS the
  percentage view returns — see the unit warning below. Worst 0.400 / 0.200.
- **the chord's own utilisation is checked too**, which is the constraint that binds: M2 absorbs
  all five braces, so at their full shear budget it reached 0.25–0.34 against its own 0.20 limit.
  Five states are therefore scaled down (0.85, 0.70) until M2 fits.
- **`LE9` deliberately overloads ONE brace** — M4 at 1.20 — so the "member overloaded" and NORSOK
  chord-overstressed paths stay exercised. Only that member, only that state; CON1 as a whole
  stays calculable.
- no torsion is applied (`mx = 0` on the braces); the percentage view reports `mx` as 0 regardless
  (verified on 40 loadings), so it cannot be budgeted this way at all.

> ⚠️ **`?isPercentage=true` returns a RATIO 0..1, not a percentage.** `0.4` means 40 %. The first
> version of these states was budgeted against those values read as percentages, so every one came
> out **100× too large — 1300 % to 2500 % of capacity**. CBFEM then refused the whole connection
> with `Connected member is overloaded [LE2] - M1`, and §6.4 reported `A = 3.3 … 8.0` on the chord
> (A = 1 is the chord at yield). Nothing caught it, because the generator's own checks compared the
> same misread numbers against the same limit — and this README claimed "worst actual 39.81 %",
> which was really 3981 %. Proof of the scale, in one line: M1 is `CHS 76.0x3.5`, so
> `N_pl = 283 kN`; the absolute read was 2 424 594 N and the percentage read 8.578, and
> `2 424 594 / 8.578 = 282 653 N` — that divisor **is** `N_pl`. It is stated in the API skill
> (`io-shapes.md:383`) and was simply not read.

The N / My / Mz and Vy / Vz split is randomised per state and per member from a fixed seed, so the
set exercises different force combinations rather than one pattern scaled 14 times. Utilisations
are per member against that member's own characteristic resistance, which matters here: `CHS30,3`
(M5, M6) has a bending divisor of 778 against `PIPE127STD`'s 41 696, i.e. **53× weaker**, so equal
forces on every brace would overload the small ones while leaving the chord idle.

**CON1 calculates.** With these states CBFEM returns results (analysis 95.7 %, plates 18.8 %)
rather than refusing the connection. `LE1` carries M4 at 1.086 — that predates this work and is
the model's own state, left untouched deliberately as the control every measurement is proved
against.

Two things measured while generating these, worth knowing before editing the file:

- **the loading `position` is not derivable from `ConMember.connectedBy`.** M1 has
  `connectedBy: "end"`, yet its loading is stored as `Position: "Begin"`, and posting `"End"` for it
  is rejected: *422 "Defined member loading Member:1, Position:End is not defined in connection 1"*.
  Read the slots from an existing load effect instead of deriving them.
- **`GET /download` returns a much smaller file** (25 kB against the original 557 kB) because the
  cached results and previews are not written. Nothing is lost: all 7 connections, their members and
  all 6 cross-sections — including the five deliberately unparseable names — were compared before
  and after and are identical.

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
