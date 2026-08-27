# Chapter 6.3 — which checks need no member length, and which do

*2026-08-27. Companion to `CHAPTER_63_FINDINGS.md`, which disconnected §6.3. That document's
reasoning is about Eq 6.27 specifically; this one asks which checks it applies to.*

Every quote below was read from `N-004u3-16016541.pdf` (Rev. 3, February 2013) with
`pdftotext -layout`. Where the extraction is unreliable, this document says so instead of
resolving it — see the Eq 6.23 caveat at the end.

## The finding: Eq 6.28 was disconnected for Eq 6.27's reasons

§6.3.8.2 contains **two** checks, and the norm separates them in one sentence. Eq 6.27 is the
beam-column check, and then, verbatim:

> **and at all cross sections along their length:**
>
> `N_Sd/N_cl,Rd + √(M²_y,Sd + M²_z,Sd)/M_Rd ≤ 1.0`   (6.28)
>
> `N_cl,Rd = f_cl·A/γ_M`   design axial **local** buckling resistance

Eq 6.28 contains no `k`, no `l`, no `C_m` and no `N_E`. Its axial resistance is *local* buckling:
`f_cl` comes from Eq 6.6–6.8, which take only `f_y` and `f_cle`, and `f_cle = 2·C_e·E·t/D` — section
and material, nothing else. **Eq 6.28 is evaluable from a connection model alone.**

`CHAPTER_63_FINDINGS.md` treats §6.3.8.2 as one item ("two inputs that **Eq 6.27** needs cannot be
expressed by the app at all") and its three findings are all about Eq 6.27 — `N_Ez = N_Ey`, `C_m`
case (b), and the (b)/(c) exclusivity. None of them touch Eq 6.28, and the document never names it
as separable. It went dark with its neighbour.

The same holds for Eq 6.44 and Eq 6.51 in §6.3.9 — the section-level twins of 6.28 — but those need
an external pressure `p_Sd`, which a connection model does not carry, so they stay out regardless.

## Evaluable from a connection model alone

Cross-section, material, joint forces, γ_M. No length, no end moments.

| check | equation | why it is length-free |
|---|---|---|
| §6.3.1 applicability | — | `t ≥ 6 mm`, `D/t < 120` — geometry |
| Axial tension | 6.1 | `N_t,Rd = A·f_y/γ_M`, with "γ_M = 1.15" stated inline |
| Bending | 6.9–6.12 | `M_Rd = f_m·W/γ_M`; `f_m` branches only on `f_y·D/(E·t)` |
| Beam shear | 6.13 | `V_Rd = A·f_y/(2√3·γ_M)` |
| Torsional shear | 6.14 | `M_T,Rd = 2·I_p·f_y/(D·√3·γ_M)` |
| Tension + bending | 6.26 | the norm's own words: "at all cross sections along their length" |
| **Compression + bending, cross-section** | **6.28** | **the finding above** |
| Shear + bending | 6.31–6.32 | ratios of 6.9 and 6.13 — but see the ±20° proviso below |
| Shear + bending + torsion | 6.33 | `f_m,Red = f_m·√(1 − 3(τ_T,Sd/f_d)²)`, all section-level |

For a member in **tension** this set is complete — nothing in §6.3 is left out except the pressure
clauses. For a member in **compression** it leaves out only the two genuinely length-dependent
checks, and keeps the section-level 6.28.

### The ±20° proviso on 6.31/6.33, which the app does not check

The norm gates both:

> provided that the direction of the shear force and the moment vectors are orthogonal within ± 20°

`NorsokCheckRunner.cs` builds a resultant `V` from `(Vy, Vz)` and a resultant `M` from `(My, Mz)`
independently, so the condition is neither checked nor guaranteed. It IS checkable from the joint
forces — the angle between the two vectors — and doing so turns an unstated assumption into a
stated one.

## Correctly excluded, and what each would need

| check | equation | what it would take |
|---|---|---|
| Axial compression | 6.2–6.8 | one unbraced length `l` per member (and per side of a through member); `k` can default to 1.0 |
| Compression + bending, stability | 6.27, 6.29, 6.30 | the same `l`, plus `k_y` and `k_z` **separately** |
| Hoop buckling | 6.15–6.20 | an external pressure `p_Sd`, plus a DIFFERENT length: "length of tubular between stiffening rings, diaphragms, or end connections" |
| Ring stiffener | 6.21 | out of scope — it sizes a detail, it is not a member utilisation |
| Tension/compression + bending + pressure | 6.34–6.51 | `p_Sd` and a Method A/B declaration; gated on Eq 6.15 first |

## C_m has a conservative default in the norm; kl does not

This matters because it shrinks the re-enabling checklist, even though it does not by itself
re-enable anything.

**`C_m = 1.0` is a valid conservative bound for every row of Table 6-2.** The norm says what the
factor is for:

> The use of the moment reduction factor (Cm) in the combined interaction equations, such as
> Equation (6.27), is to obtain an equivalent moment that is **less conservative**.

So omitting the reduction is safe. And the norm prescribes 1.0 itself for the one case Table 6-2
declines to cover:

> For a cantilever tubular member, Cm=1.0.

Every tabulated value is ≤ 1.0: case (a) is 0.85; case (c) is "1.0 − 0.4·N_Sd/N_E, **or 0.85,
whichever is less**"; case (b) is `0.6 − 0.4·M1,Sd/M2,Sd` where the ratio is "smaller to larger
moments" and "negative when bent in single curvature", so it cannot fall below −1 and `C_m` cannot
exceed 1.0.

**Cost**: `C_m` multiplies the moment term of Eq 6.27 linearly, so against case (a) the moment term
grows by `1/0.85 = 1.176`. On the BENCHMARK.md DIAGONAL 1 example that is 76.6 % → 86.6 %, about ten
utilisation points — bounded, and in the safe direction.

**`kl` has no such default.** `N_E = π²EA/(kl/i)²`, so as `kl` grows `N_E → 0`, `1 − N_Sd/N_Ey` goes
negative and the amplifier changes sign. A conservative default would have to be infinite. The norm
offers only Table 6-2's tabulated `k` (0.7 / 0.8 / 1.0, keyed to structural element types a
connection model does not know) and "in lieu of such a rational analysis, values of effective length
factors, k, and moment reduction factors, Cm, may be taken from Table 6-2" — which relieves the
designer of computing `k`, never of knowing `l`.

**What this changes:** `l` is the single irreducible input. With `k = 1.0` and `C_m = 1.0` both taken
from the norm, re-enabling Eq 6.27 needs one number per member (two for a through member) rather
than the seven-per-member grid `CHAPTER_63_FINDINGS.md` costs it at.

## Where CHAPTER_63_FINDINGS.md is overstated

- **It treats §6.3.8.2 as one check.** The substantive item — see the top of this document.
- **"`C_m ≤ 1.0` is a real bound, though the norm never says so."** The norm does say so, for
  cantilevers, and the same paragraph of that document then quotes it. Comm. 6.3.8.2's "less
  conservative" sentence settles the factor's character.
- **The re-enabling cost is over-stated.** Its "175 values per connection" table assumes `M1` per
  plane per load effect; `C_m = 1.0` removes that column entirely.

## Open: the exponent in Eq 6.23 — NOT resolved

`MaterialFactorCalc.cs:54` implements the axial term of Eq 6.23 as `(σ_c,Sd/f_cl)² · ξ_c` — squared.
The PDF's text layer shows a clear `2` on the hoop term and none on the axial term, which would make
the code's squaring wrong and its `λ_s` too small — hence γ_M too low, which is unconservative.

**This could not be settled here.** The equation is typeset in a Symbol font that `pdftotext` does
not reproduce, and no PDF rasteriser is available in this sandbox (`pdftoppm`, `mutool` and
ImageMagick are all absent, and installing poppler needs admin rights). So the exponent has to be
read off the rendered page by a human, or by a machine that can render it.

**Until then, do not re-enable the variable-γ_M branch of §6.3.7.** It only runs for class 4
sections (`f_y/f_cle > 0.170`); below that γ_M = 1.15 and the branch never executes, so it does not
block any of the checks listed above. The BENCHMARK.md case (CHS 500×20, `f_y/f_cle = 0.070`) is in
that safe range.

## The state of the code, if this subset is ever wanted

Nothing here has been implemented — this document is a survey. What a decision would be starting
from, measured 2026-08-27:

- **§6.3 is disconnected, not removed.** `NorsokCheckRunner.cs:235-240` is a comment where the call
  to `EvaluateTubularMemberFormulas` used to be; that method and everything under
  `Services/Formulas/` still compiles.
- **Eq 6.28 already has its own entry point.** `CompressionBendingCheck.EvaluateCrossSection` takes
  `(N_Sd, N_cl_Rd, M_y_Sd, M_z_Sd, M_Rd)` — no length, no `C_m`, no `N_E` — and returns its own
  result row tagged `6.3.8.2` / `6.28`.
- So the length-free subset is a question of which calls to make and how to report them, not of
  writing formulas. The two things that would still need work: separating the length-free checks
  from the length-dependent ones inside `EvaluateTubularMemberFormulas` (today it is one block), and
  the ±20° proviso on 6.31/6.33, which nothing checks.
- **Hydrostatic pressure is out of scope** by the same reasoning that keeps the length out: `p_Sd`
  would have to be typed, and §6.3.6.1 also needs a second length (between stiffening rings), which
  is a different quantity from the buckling `kl`.

## What was verified, and how

- **Read in the norm** (`pdftotext -layout`): every equation number and threshold in the tables
  above; the "and at all cross sections along their length" sentence separating 6.28 from 6.27;
  `N_cl,Rd = f_cl·A/γ_M` and its "local buckling" description; the `f_cl` chain 6.6–6.8 and
  `f_cle = 2·C_e·E·t/D`; Eq 6.29/6.30 as `π²EA/(kl/i)²` and "k … relate to buckling in the y and z
  directions"; the ±20° proviso; Table 6-2's four notes and its three C_m cases; Comm. 6.3.8.2's
  cantilever and "less conservative" sentences.
- **Not re-verified**: BENCHMARK.md's six hand calculations. They were used only to see which
  equations are already trusted — and to note that Eq 6.14 and the torsion path are NOT among them
  (BENCHMARK.md says so itself).
- **Unresolved**: the Eq 6.23 exponent, above.
- The python reference (`n63.py`) is **not** a usable oracle for §6.3.8.3: it implements Eq 6.31 as
  `(M/M_Rd)/√(1.4 − V/V_Rd)`, with a square root the norm does not have. BENCHMARK.md records the
  same error in the source worksheet. The C# `ShearBendingCheck` is correct. Where the python IS
  useful: it takes `k_y` and `k_z` separately and uses `max(k_y, k_z)` for Eq 6.5, which matches the
  norm's "l = longer unbraced length in y or z direction" — independent confirmation of
  `CHAPTER_63_FINDINGS.md`'s first finding.
