# NORSOK Checker — Python → C# unification (context for Lukáš & Ondřej)

*July 2026, branch `feature/norsok-64-csharp-port` (based on `feature/norsok-checker` @ `da19427f`).*

## Why

We had two tools: the **C# WPF app** (this folder) and the **python pipeline**
(`app.py` + `norsok/extract.py` + `norsok/n64.py`, plus Lukáš's standalone per-joint
verification scripts). The goal of this branch: fold all verified python §6.4 logic into
the C# app so there is **one shipping tool**, with the python kept only as the
verification oracle.

## What we found before porting

The python §6.4 was trustworthy: `n64.py` and Lukáš's scripts were written independently
and **agree on every Table 6-3 / 6-4 formula** — that cross-validation made them ground
truth. The C# §6.4 (`TubularJointCheck.cs`), however, had **~8 formula transcription
defects**, e.g.:

| old C# | verified reference |
|---|---|
| Qu K-axial `min(16+1.2γ, 40β)·Qg` — drops β^1.2 | `min((16+1.2γ)·β^1.2, 40·β^1.2)·Qg` |
| Qu IPB `5 + 0.7γ·β^1.2` — precedence | `(5 + 0.7γ)·β^1.2` |
| Qu OPB `2.5 + 4.5·β^0.2·γ^0.6` | `2.5 + (4.5 + 0.2γ)·β^2.6` |
| Qu T/Y tension used a compression-style form | `30·β` |
| Qu X tension wrong | `6.4·γ^(0.6β²)` |
| Qg φ = `(fy_b·T)/(fy_c·T)` — T cancels | φ = `(t·fy_b)/(T·fy_c)` |
| Qf `1 − C1·(σa/fy) …` — sign | `1 + C1·(σa/fy) − C2·… − C3·A²` |

Also structurally: joint type was a **manual dropdown** (no classification), the chord's
own forces were fed through the brace formula, chord stresses were a worst-envelope hack,
and there was no §6.4.3.1 lesser-of-clamped-capacity rule and no per-gap K weighting.

§6.3 needed **no** porting — the C# member checks were already ahead of `n63.py`
(which was never wired into the python app).

## What was done (commit map)

| Commit | Content |
|---|---|
| `4d7b931b` | Synced `ConMember`/`ConLoadEffect` API models from main (`Origin`, `AxisY/Z`, `ConnectedBy` — needed by the topology port) |
| `2ac9165e` | **Engine port**: `Services/Norsok64/Norsok64Engine.cs` = bit-for-bit port of `n64.py` (Qu/Qf/Qg/Qβ, Eq 6.52–6.57, per-K-gap weighted resistance, chord-overstressed guard, §6.4.3.1 clamp rule) + `UT_NorsokChecker` with 11 tests pinned to numbers from *running* the python |
| `cd0db9a2` | **Topology port**: `extract.py` → `JointForceResolver` (verified force-reading recipe, chord Begin/End stress averaging), `KyxClassifier` (automatic K/Y/X from the transverse-force balance, per-gap K components), `JointTopologyBuilder` (chord identification, joint-plane RANSAC fit, θ/side/toe-to-toe gaps, assumption gate). Wired into the app; the dropdown is now only a fallback |
| `acf20a5e` | Report derivation blocks (per-class Qu/Qf table, K-per-gap, chord-stress trail, validity) + auto/manual UI toggle |
| `1a74217c` | Nested-monorepo build fix (`Directory.Build.props` + CS0618 suppression in the generated client) |
| `a586b5fc` | **Live validation** against the four benchmark `.ideaCon` + python moved to `reference/` |

## How it is verified (three layers of oracles)

1. **Formula engine** — 11 tests pinned to numbers produced by *running* the python:
   the `n64.py` self-test and Lukáš's K / X / six-T-Y scripts (e.g. K util 0.3011,
   X 1.3130 FAIL, T/Y pure tension 0.5459), plus out-of-range-clamp and
   chord-overstress oracles.
2. **Topology pipeline** — shared-fixture oracle: synthetic K/X/TY/KT joints in REST
   JSON (`UT_NorsokChecker/TestData/topology_fixtures.json`) are consumed by **both**
   `gen_topology_oracle.py` (runs the real `extract.py` offline) and the C# tests —
   every intermediate value (forces, σ, frK/frX/frY, per-gap K, util) compared at 1e-6.
3. **Live end-to-end** — `LiveValidationTests` (`[Explicit]`) opens the four benchmark
   projects through a local Connection RestAPI and compares the C# pipeline per-LE /
   per-brace against `live_oracle.json` captured from the python on the same files:

   | file | governing results |
   |---|---|
   | `K_CONNECTION` | CON1 0.453 (WARNING gate), CON2 0.718 |
   | `X_CONNECTION` | CON1 0.620, CON3 **1.3130 FAIL** (= Lukáš X script), CON2 1.938 FAIL |
   | `TY_CONNECTION` | CON1 **0.8747** (= INTERACTION_COMPRESSION), CON2 0.718 |
   | `TY_CONNECTION_UNIT_TEST` | CON1 **0.9904** (= IN_PLANE_BENDING) |

   Everything matches to 1e-6. Re-run: `dotnet test --filter FullyQualifiedName~LiveValidationTests`
   (service dir via `IDEASTATICA_SETUP_DIR`, default `StatiCa 26.1`); re-capture the
   oracle with `python UT_NorsokChecker/TestData/gen_live_oracle.py`.

## Defects the validation caught (this includes “the issue for Ondřej”)

1. **CHS names with comma separators** — the benchmark files name sections
   `CHS457,16 - CHORD(CHS457,16)`; the old `parse_chs` only understood `CHS457/16`,
   so every member read as “not CHS” and the whole joint failed the gate. This is the
   *“ISSUE TO FIX FOR ONDREJ”* from commit `da19427f` — fixed on **both** sides
   (tolerant name parser), and the C# additionally reads D/T from the cross-section
   **parameters** (naming-independent).
2. **`GetLoadEffectsAsync(isPercentage: false)`** was missing in the C# app —
   percentage-stored load effects would silently collapse every check to util ≈ 0.
3. **numpy SVD sign indeterminacy** — the joint-frame `ey` from `np.linalg.svd` can come
   out flipped, flipping side/M_ip/σ_mz signs run-to-run. Both implementations now
   canonicalise the sign (align with the mean of the aligned perp components).
   Physically immaterial; determinism matters for comparison.

## Where things live now

- **Product**: this C# WPF app. §6.4 runs on **auto topology** by default
  (chord/braces, θ, gaps and K/Y/X derived from geometry + force balance); the manual
  joint-type/gap/θ inputs are a greyed fallback used only when the topology gate
  rejects the joint.
- **Reference (do not extend)**: `reference/python_prototype/` and
  `reference/verification_scripts/` (Lukáš's scripts + benchmark `.ideaCon`).
  Order of truth when a formula is in doubt: N-004 Rev.3 PDF → verification scripts →
  `n64.py` → the C# port (must match all three).
- **Tests**: `UT_NorsokChecker` — 26 offline (always run) + 4 live (`[Explicit]`).

## Open items

- **Lukáš**: engineering review of the §6.4 report derivation blocks (per-class Qu/Qf
  table, K-per-gap, chord-stress trail — wording/units/conventions). Also your
  `TY_JOINT_NORSOK_ISSUES.docx` σ_mz sign question: numerically immaterial (σ_mz enters
  Qf only squared in A², and Eq 6.57 uses |M_op|), but worth closing formally.
- **Ondřej**: the comma-name CHS issue is fixed as described above; please sanity-check
  the tolerant parser against any other section-naming conventions you know of.
- Chord-overstressed forced-FAIL is an **app-level safety rule**, not a norm clause
  (Qf has no floor in N-004) — flagged in the report; future idea: fall back to a §6.3
  member check of the chord wall in that case.
