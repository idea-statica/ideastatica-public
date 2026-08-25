# Chapter 6.3 (tubular members) — why it is being disconnected

*August 2026. Read this before re-enabling §6.3 or touching `Services/Formulas/*`.*

§6.3 is being taken out of the app's UI and check pipeline. The code stays — nothing is deleted —
but it stops producing numbers.

**This is not a verdict on the formulas.** `BENCHMARK.md` verifies them: six hand calculations on
CHS 500×20 / S355 covering section properties, Eq 6.1 (tension), 6.2–6.8 (compression), 6.9–6.12
(bending), 6.26 (tension+bending) and 6.13 (shear), plus a summary table against a source
worksheet. Where the app and the worksheet differ, the document argues the app follows the
standard. That work also found and fixed a real bug — before `19aebe16` all demands were enveloped
across members and checked against the *first* CHS member's capacities.

The reason for disconnecting is narrower: **two inputs that Eq 6.27 needs cannot be expressed by
the app at all, and cannot be derived from a connection model** — they are properties of the
member's unbraced span, which lies outside the joint. Neither is covered by the benchmark, and
neither could be: the benchmark runs a single `k = 0.7` for both planes, because that is all the
grid offers.

## What triggered it

The intent was to extend the L/k entry from one connection to all of them. Working out how led to
three findings.

### 1. `N_Ez = N_Ey` is wrong

`Services/NorsokCheckRunner.cs`:

```csharp
double N_Ey = CompressionBendingCheck.EulerBucklingLoad(geo.A, mK, mL, geo.i);
double N_Ez = N_Ey; // Same for tubular (symmetric)
```

The comment is true of the **section** — a round tube has one `i`, one `M_Rd`, no strong axis. But
Eq 6.29/6.30 do not take `kl/i` from the section, they take it from the **restraint**, and that is
not symmetric.

Concrete case: three joints in a row along a chord. In Z the chord is braced at every joint; in Y
only at the ends. Same unbraced length `L`, different `k` per plane. Worked through with
`E = 210 GPa`, `A = 22167 mm²`, `i = 156 mm`, `L = 6000 mm`:

| k_y | k_z | N_Ey | N_Ez | ratio |
|---|---|---|---|---|
| 0.7 | 0.7 | 63 384 kN | 63 384 kN | 1.00 |
| 1.0 | 0.5 | 31 058 | 124 232 | **0.25** |
| 2.0 | 0.7 | 7 764 | 63 384 | **0.12** |

Up to **eightfold**, on a round tube. And the app cannot express it: the member grid has a single
`k` column, so the two planes can never differ.

### 2. C_m case (b) — verified, but the entry does not scale

**Case (b) itself is verified.** `BENCHMARK.md` demonstrates it directly: with `M1y = +25 kNm`
against a joint-end `My = −25` (so `M1/M2 = −1`, single curvature, `C_m,b = 1.0`), Eq 6.27 moves
from 76.6 % (case (a), C_m = 0.85) to 86.6 %, hand-checked as
`0.2011 + 1.0×25 / (0.87691×42.887) = 0.866`. The document notes that the pre-`063c8c2b` fixed
`C_m = 0.85` *"understated this member by 10 points on identical loads"*. So the sign convention
works and the case earns its place — leaving it out is the non-conservative choice.

What does not work is the entry. Three things:

- **The volume.** `M1` is per load effect, not per member: for 1 chord + 3 braces and 15 load
  effects that is 150 values per connection, 750 across five (see the table below). The engineer
  would have to pull each one from the global model, which the app cannot see.
- **The ratio is not built as smaller-over-larger.** `CmEndMoments` divides whatever arrives in
  whatever order and `Math.Clamp(-1, 1)` truncates the result, so the bound is a property of the
  code rather than of the calculation. It happens to give the right answer on the benchmark case,
  where `|M1| = |M2|`; an `M1` larger than the joint moment would be silently cut instead of
  inverted.
- **The sign is prescribed, not computed.** The norm gives the sign from the curvature — positive
  in reverse, negative in single — so it is not the result of dividing two numbers. Nothing in a
  column headed `M1y [kNm]` conveys that, and it decides between C_m = 0.20 and 1.00.

```csharp
double CmEndMoments(double m1, double m2)
{
    if (Math.Abs(m2) < 1e-9) return 0.6;
    double ratio = Math.Clamp(m1 / m2, -1.0, 1.0);
    return 0.6 - 0.4 * ratio;
}
```
- **The sign convention is inconsistent with the rest of the check.** Traced through
  `NorsokCheckRunner` and `CompressionBendingCheck`:

  | quantity | how it enters Eq 6.27 | sign used? |
  |---|---|---|
  | `N_Sd` | `Math.Abs(N)` | **no** |
  | `M_y,Sd`, `M_z,Sd` | passed signed, then squared under the root | **no** — cancels |
  | `M2` into `CmEndMoments` | passed **signed** | **yes** |

  Everywhere else the moments are effectively absolute, which is what the square root in 6.27
  implies. The one place a sign decides anything is this ratio.

### 3. Cases (b) and (c) are treated as alternatives

The runner evaluates all three and keeps the worst:

```csharp
var cmCases = new (string Label, double Cmy, double Cmz)[]
{
    ("(a)", 0.85, 0.85),
    ("(b)", CmEndMoments(m1y, My), CmEndMoments(m1z, Mz)),
    ("(c)", cmc, cmc),
};
```

But note 1 makes (b) and (c) **exclusive**: (b) is *"for members with no transverse loading"*, (c)
*"for members with transverse loading"*. Table 6-2 also assigns them per element type — jacket
braces get "(b) or (c)", K-braces and X-braces only (c).

Taking the maximum is conservative, so no result is unsafe because of this. But on a transversely
loaded member (b) can govern, and (b) is not a case the norm offers for it. And **transverse
loading cannot be detected from a connection model**: it acts on the span, outside the joint.
Measured on `TY_CONNECTION_UNIT_TEST`, the chord reports `vz = 65 kN` at *both* ends with the same
sign — shear passing through the node from the brace. A distributed load between supports would
not appear at all.

## Why this is not simply fixed

Doing §6.3 correctly needs, per member:

| input | why it cannot be derived |
|---|---|
| `L` | the unbraced span is not in the joint model |
| `k_y`, `k_z` | restraint conditions differ per plane (finding 1) |
| transverse loading, per plane | acts on the span, invisible at the node (finding 3) |
| `M1`, per plane | moments at the far ends of the span |

**Seven inputs per member**, of which five are per member and two (`M1y`, `M1z`) are per load
effect. The chord needs two rows, not one — as a through member it has an unbraced span on each
side of the node, and the API returns two loadings for it against a brace's one.

For 1 chord + 3 braces (so 5 rows) and 15 load effects:

| | per connection | × 5 connections |
|---|---|---|
| per-member (`L`, `k_y`, `k_z`, transverse ×2) | 5 × 5 = **25** | 125 |
| per-LC (`M1y`, `M1z`) | 5 × 15 × 2 = **150** | 750 |
| **total** | **175** | **875** |

For 1 chord + 6 braces and 30 LC it is **520 per connection, 2600 across five**.

Nobody fills that in correctly. And a half-filled grid is worse than no grid: the app cannot tell
"not entered" from "entered as zero", and the numbers it produces look as authoritative either way.
`L = 5000 mm` and `k = 0.7` are the current defaults — neither is conservative, and both are
invisible assumptions.

## What is verified, and what the verification cannot reach

**Verified — `BENCHMARK.md`.** Six hand calculations on CHS 500×20 / S355, plus a summary table
against a source worksheet and a documented demonstration of C_m case (b). It caught a real bug
(`19aebe16`, demands enveloped across members) and it argues each deviation from the source
worksheet in the app's favour. This is a genuine oracle for the formulas.

**Not reachable by it — the two findings above.** Both are about inputs the app cannot express:

- the benchmark runs one `k = 0.7` for both planes, because the grid has one `k` column. A
  benchmark cannot exercise a distinction the data model does not have.
- transverse loading is never varied, so (b)-vs-(c) exclusivity is never tested. And it could not
  be from a connection model — the load acts on the span.

**Also worth knowing:** the automated tests do not cover §6.3 at all.
`UT_NorsokChecker/TestData/live_oracle.json` pins §6.4 only, and `n63.py` in
`reference/python_prototype/norsok/` was never wired into the python app — it does not even compute
C_m, taking it as an input defaulting to `0.85` (`n63.py:100-101`), and `0.6 - 0.4` appears nowhere
in the python. So the §6.3 verification is a document, not a test: it holds for the case it was
computed on, and nothing re-runs it when the code changes.

## Two things worth writing down while they are fresh

**`C_m ≤ 1.0` is a real bound, though the norm never says so.** The formula is unbounded on its
own — `M1/M2 = −2` would give 1.40 — and Table 6-2 states no cap for case (b) (the only C_m limits
in the text are case (c)'s "or 0.85, whichever is less" and `C_m = 1.0` for cantilevers, p. 60).
The bound comes from two directions instead:

- reading "ratio of **smaller** to larger" as absolute values gives `|ratio| ≤ 1`, hence
  `C_m ∈ [0.2, 1.0]`;
- and physically C_m is a *reduction* factor — how much of the end moment adds to the second-order
  effect. Exceeding 1.0 would mean amplifying a moment beyond its own value. The ceiling is "no
  reduction at all", which is 1.0, and it occurs at constant moment along the member: the peak is
  everywhere, including midspan where the P-δ deflection is largest, so there is nothing to reduce.

Any implementation that can produce `C_m > 1` has built the ratio wrongly. Worth a guard rather
than a comment.

**The planes have to stay separate even for a round tube.** Eq 6.27 adds the components
geometrically — `√((C_my·M_y)² + (C_mz·M_z)²)` — which reads as though the planes were
interchangeable, and for a tube the *section* genuinely has no strong axis: one `i`, one `M_Rd`.
Measured: with `C_my = C_mz` the component form and a single-resultant form give bit-identical
results; with different values they diverge by up to 12 %.

But the section is not what Eq 6.29/6.30 ask about — the **restraint** is, and that is not
symmetric (finding 1). So a single per-member C_m is only self-consistent when the restraint is
symmetric too, which is exactly the case that cannot be assumed.

## What re-enabling would require

1. `k_y` / `k_z` as separate inputs, and `N_Ez` computed from `k_z`.
2. A decision on transverse loading — either a per-plane input, or a documented statement that the
   check assumes (c) always, or that it assumes the worst of (b)/(c) as it does today.
3. `M1` either dropped (with a documented conservative C_m) or entered per load effect. If it is
   kept, the ratio must be built from magnitudes with the curvature supplied separately, and a
   blank cell must be distinguishable from a zero.
4. **`BENCHMARK.md` extended, or its numbers pinned as tests.** The document already verifies the
   formulas by hand; what it cannot do is re-run itself. Pinning a few of its results as unit tests
   in `UT_NorsokChecker` would make the §6.3 side as regression-proof as §6.4 is, and would cover
   the new inputs (different `k` per plane, the (b)/(c) choice) as they are added.

Until then the formulas stay in `Services/Formulas/` and are not called — the same status `n63.py`
has on the python side.

## What the change actually did

Three things landed together, because they touch the same two methods and only make sense as one
step:

**§5 removed outright.** `Services/DesignClassification.cs` and
`Models/DesignClassificationInput.cs` deleted, along with the §5 panel (design class, fatigue,
through-thickness) and the chapter toggle. Design class, steel quality level and NDT inspection
category enter no resistance equation — those results carried `Utilization = 0` and could not fail
a check. Removing the chapter also retired three known defects without writing a fix: the wrong
stress quantity for Table 5-3, a missing shear branch, and a hardcoded stress direction for
Table 5-4.

**§6.3 disconnected, not deleted.** `EvaluateTubularMemberFormulas` and everything under
`Services/Formulas/` still compiles; nothing calls it. The L / k / M1y / M1z grid columns are gone
— they were what made the single-`k` defect and the mis-built case (b) ratio reachable.
`MemberDisplayInfo.L/K/M1y/M1z` stay as unused properties, cheaper than a model change that would
have to be undone.

**The CBFEM calculation became optional.** With §5 gone and §6.3 mothballed, the engine run serves
one optional group, so `CalculateAsync` + `GetRawJsonResultsAsync` now sit inside
`if (includeCbfem)`. §6.4 needs load effects and geometry only — verified: nothing in the §6.4
branch reads `parsed`. Two consequences worth knowing:

- `EvaluateNorsokFormulas` takes `rawJsonResults` as nullable and skips `RawResultsParser.Parse`
  when it is null.
- Shape detection no longer reads plate names (`hasArc` → CHS). It was never needed:
  `CrossSectionDetector` already derives the shape from `CrossSectionType` when the project is
  loaded, with no calculation. Only wall thickness and `f_y` are still refined from the plates,
  and only when a calculation ran.

**Careful — `Services/Formulas/TubularJointCheck.cs` is §6.4, not §6.3**, and it is live at
`NorsokCheckRunner.cs:91` and `:757`. It is also the file the python → C# port found ~8 formula
transcription defects in. Do not fold it into the mothballed set.

Verified with the 26 offline tests in `UT_NorsokChecker`, which pin §6.4 against
`TestData/live_oracle.json` at 1e-6: 26/26 before the change, 26/26 after. **Not one §6.4 number
moved**, which was the whole condition on this change.

---

*All figures measured against the code as of `dc11192d` and against NORSOK N-004 Rev. 3, read from
rendered page images (p. 20–21 for Table 6-2 and its notes, p. 60 for the cantilever note).*
