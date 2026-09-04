# Norsok Checker — Analysis & Implementation Plan

> **Status, August 2026 — this document describes §6.3 as a live feature; it no longer is.**
> §6.3 (tubular members) has been disconnected from the UI and the check pipeline, and chapter 5
> (design classification) has been removed. The §6.3 formula analysis below stays valid and is the
> starting point for whoever re-enables it — see `CHAPTER_63_FINDINGS.md` for why it was
> mothballed and what re-enabling requires. What the app checks today: the CBFEM plate/weld/bolt
> group (optional) and §6.4 tubular joints.

## Overview

A WPF desktop application that evaluates steel connection designs against **NORSOK N-004 Rev. 3** (Design of Steel Structures) requirements using the IDEA StatiCa Connection API. The tool:

1. **Adjusts project settings** (code factors γM) to match Norsok Table 6-1
2. **Runs CBFEM calculation** via Connection API
3. **Retrieves raw results** (`GetRawJsonResultsAsync`) — plates, welds, bolts
4. **Evaluates Norsok formulas** from Chapter 6.3 on top of CBFEM output
5. **Generates a compliance report** showing each formula, populated variable values, and PASS/FAIL

Source norm: `N-004u3-16016541.pdf` (NORSOK N-004, Rev. 3, February 2013)
Test project: `norsok.ideaCon`

---

## Step 0: Project Settings — Code Factors (Table 6-1)

Before any calculation, the app must set the correct material factors via `Settings.UpdateSettingsAsync()`.

**NORSOK N-004 material factor (§6.3.7):**

| Factor | Norsok Value | EC3 Default | Purpose |
|--------|-------------|-------------|---------|
| γM0 | **1.15** | 1.0 | Class 1,2,3 cross-sections |
| γM1 | **1.15** | 1.0 | Class 4 cross-sections, buckling |
| γM2 | **1.30** | 1.25 | Net section at bolt holes, fillet and partial penetration welds, bolted connections |
| γBC | **1.05** | N/A | Additional building code factor (§6.1) |

> **There is no γM3 row, and there never should have been one.** Verified against N-004 Rev. 3
> page 14: Table 6-1 lists γM0, γM1 and γM2 only, and neither "M3" nor "slip" appears anywhere in
> the standard. This table used to carry `γM3 | 1.30 | 1.25 | Slip-resistant connections`, the
> report printed that row under the standard's own caption, and `ProjectSettingsService` wrote the
> value into the engineer's project — a factor with no basis, in a tool that discloses it modifies
> the input model. γM3 is a real EN 1993-1-8 factor; where N-004 is silent the project's own value
> stands, so the app now leaves it untouched and logs that it did.

> **Note**: γM is not constant for class 4 cross-sections — see §6.3.7 Equation (6.22).
> For λs < 0.5: γM = 1.15
> For 0.5 ≤ λs ≤ 1.0: γM = 0.85 + 0.60·λs
> For λs > 1.0: γM = 1.45

The app reads current settings, adjusts γM values, recalculates, and logs the change.

---

## Step 1: Calculation & Result Caching

### Workflow
1. Open project → get connections
2. **Check** if `{projectName}_rawresults.json` exists on disk
3. If exists → prompt user: *"Stored results found. Use cached results or trigger new API calculation?"*
4. If "Use cached" → load JSON, skip calculation
5. If "Calculate" → call `CalculateAsync` + `GetRawJsonResultsAsync`, save to JSON

### API Calls
```csharp
// Calculate
var calcResults = await client.Calculation.CalculateAsync(projectId, connectionIds);

// Get raw CBFEM results (JSON string representing CheckResultsData)
var rawResults = await client.Calculation.GetRawJsonResultsAsync(projectId, connectionIds);

// Save to disk
File.WriteAllText(cachePath, rawResults[0]);
```

---

## Step 2: Norsok N-004 Chapter 6.3 — Tubular Members

### Applicability
- Unstiffened and ring-stiffened tubulars
- Thickness t ≥ 6 mm
- D/t < 120
- **Hydrostatic pressure: NOT applicable** (IDEA StatiCa limitation)

### Scope (what we implement)

| Section | Title | Status |
|---------|-------|--------|
| 6.3.2 | Axial Tension | **Step 1** |
| 6.3.3 | Axial Compression | **Step 2** |
| 6.3.4 | Bending | **Step 3** |
| 6.3.5 | Shear | **Step 4** |
| 6.3.6 | Hydrostatic Pressure | SKIP (not applicable) |
| 6.3.7 | Material Factor | **Step 5** (variable γM) |
| 6.3.8.1 | Axial Tension + Bending | **Step 6** |
| 6.3.8.2 | Axial Compression + Bending | **Step 7** |
| 6.3.8.3 | Shear + Bending Interaction | **Step 8** |
| 6.3.8.4 | Shear + Bending + Torsion | **Step 9** |
| 6.3.9 | Combined with Hydrostatic | SKIP |

---

## Formula 6.3.2 — Axial Tension (Eq. 6.1)

**Check condition:**

```
N_Sd ≤ N_t,Rd
```

**Design tensile resistance:**

```
N_t,Rd = A · f_y / γ_M
```

**Variables to populate from raw results:**

| Variable | Description | Source |
|----------|-------------|--------|
| N_Sd | Design axial force (tension, positive) | Raw results → member internal forces |
| A | Cross-sectional area | Member geometry (π/4 · (D² - (D-2t)²)) |
| f_y | Characteristic yield strength | Material properties from project |
| γ_M | Material factor = 1.15 | Table 6-1 (or Eq. 6.22 for class 4) |

**Report output example:**
```
§6.3.2 Axial Tension — Equation (6.1)
N_Sd ≤ N_t,Rd = A · f_y / γ_M

  N_Sd  = 1250.0 kN     (design axial tensile force)
  A     = 7854 mm²       (cross-sectional area, D=200mm, t=10mm)
  f_y   = 355 MPa        (S355 yield strength)
  γ_M   = 1.15           (Table 6-1)

  N_t,Rd = 7854 × 355 / 1.15 = 2424.4 kN

  Utilization: N_Sd / N_t,Rd = 1250.0 / 2424.4 = 0.515
  Result: PASS ✓
```

---

## Formula 6.3.3 — Axial Compression (Eq. 6.2–6.8)

**Check condition:**

```
N_Sd ≤ N_c,Rd = A · f_c / γ_M
```

**Characteristic compressive strength f_c:**

```
f_c = [1.0 - 0.28·λ²] · f_cl        for λ ≤ 1.34    (Eq. 6.3)
f_c = 0.9/λ² · f_cl                  for λ > 1.34    (Eq. 6.4)
```

**Column slenderness λ:**

```
λ = √(f_cl / f_E)                                     (Eq. 6.5)

f_E = π²·E / (k·l/i)²               (Euler buckling)
```

**Local buckling strength f_cl:**

```
f_cl = f_y                            for f_y/f_cle ≤ 0.170    (Eq. 6.6)
f_cl = (1.047 - 0.274·f_y/f_cle)·f_y  for 0.170 < f_y/f_cle ≤ 1.911   (Eq. 6.7)
f_cl = f_cle                           for f_y/f_cle > 1.911    (Eq. 6.8)
```

**Elastic local buckling:**

```
f_cle = 2·C_e·E·t/D                  C_e = 0.3
```

**Variables to populate:**

| Variable | Description | Source |
|----------|-------------|--------|
| N_Sd | Design axial force (compression) | Raw results |
| A | Cross-sectional area | Geometry |
| f_y | Yield strength | Material |
| E | Young's modulus = 2.1×10⁵ MPa | Constant |
| D | Outside diameter | Geometry |
| t | Wall thickness | Geometry |
| k | Effective length factor | Table 6-2 or user input |
| l | Unbraced length | Member length |
| i | Radius of gyration | Geometry |
| C_e | Buckling coefficient = 0.3 | Constant |

---

## Formula 6.3.4 — Bending (Eq. 6.9–6.12)

**Check condition:**

```
M_Sd ≤ M_Rd = f_m · W / γ_M
```

**Characteristic bending strength f_m:**

```
f_m = (Z/W)·f_y                                       for f_y·D/(E·t) ≤ 0.0517    (Eq. 6.10)
f_m = (1.13 - 2.58·f_y·D/(E·t))·(Z/W)·f_y            for 0.0517 < ... ≤ 0.1034   (Eq. 6.11)
f_m = (0.94 - 0.76·f_y·D/(E·t))·(Z/W)·f_y            for 0.1034 < ... ≤ 120·f_y/E (Eq. 6.12)
```

**Section moduli (tubular):**

```
W = π/32 · [D⁴ - (D-2t)⁴] / D          (elastic)
Z = 1/6  · [D³ - (D-2t)³]              (plastic)
```

**Variables to populate:**

| Variable | Description | Source |
|----------|-------------|--------|
| M_Sd | Design bending moment | Raw results |
| W | Elastic section modulus | Geometry |
| Z | Plastic section modulus | Geometry |
| f_y, E, D, t | As above | Material / Geometry |
| γ_M | Material factor | §6.3.7 |

---

## Formula 6.3.5 — Shear (Eq. 6.13–6.14)

**Beam shear check:**

```
V_Sd ≤ V_Rd = A · f_y / (2·√3·γ_M)       (Eq. 6.13)
```

**Torsional shear check:**

```
M_T,Sd ≤ M_T,Rd = 2·I_p·f_y / (D·√3·γ_M)  (Eq. 6.14)

I_p = π/32 · [D⁴ - (D-2t)⁴]              (polar moment of inertia)
```

---

## Formula 6.3.7 — Variable Material Factor (Eq. 6.22–6.25)

For class 4 cross-sections (f_y/f_cle > 0.170), γ_M is not constant:

```
γ_M = 1.15                               for λ_s < 0.5      (Eq. 6.22a)
γ_M = 0.85 + 0.60·λ_s                    for 0.5 ≤ λ_s ≤ 1.0 (Eq. 6.22b)
γ_M = 1.45                               for λ_s > 1.0      (Eq. 6.22c)
```

Where:

```
λ̄_s = |σ_c,Sd|/f_cl · λ_c + (σ_p,Sd/f_h)² · λ_h       (Eq. 6.23)
```

`λ_c = f_y/f_cle` and `λ_h = f_y/f_he` (Eq. 6.24) — the code calls the same two `ξ_c`/`ξ_h`.

> **The implementation does not match this.** `MaterialFactorCalc.cs:54-56` computes
> `√(a² + b²)` where the norm asks for `a + b²`: it squares the axial term, which is first power in
> the norm, and takes a square root the norm does not have. The result is unconservative (too small
> a `λ̄_s`, hence too low a γ_M). Recorded in `CHAPTER_63_REVISIT.md`; not fixed, because §6.3 is
> disconnected and this branch only runs for class 4 sections.

---

## Formula 6.3.8.1 — Axial Tension + Bending (Eq. 6.26)

**Interaction check:**

```
(N_Sd / N_t,Rd)^1.75 + √(M_y,Sd² + M_z,Sd²) / M_Rd ≤ 1.0
```

> Note: The exponent 1.75 on normal force is Norsok-specific (not standard EC3 linear interaction).

---

## Formula 6.3.8.2 — Axial Compression + Bending (Eq. 6.27–6.28)

**Stability check (Eq. 6.27):**

```
N_Sd/N_c,Rd + 1/M_Rd · √[(C_my·M_y,Sd/(1 - N_Sd/N_Ey))² + (C_mz·M_z,Sd/(1 - N_Sd/N_Ez))²]^0.5 ≤ 1.0
```

**Cross-section check (Eq. 6.28):**

```
N_Sd/N_cl,Rd + √(M_y,Sd² + M_z,Sd²) / M_Rd ≤ 1.0
```

Where:
```
N_Ey = π²·E·A / (k·l/i_y)²    (Eq. 6.29)
N_Ez = π²·E·A / (k·l/i_z)²    (Eq. 6.30)
N_cl,Rd = f_cl · A / γ_M
```

---

## Formula 6.3.8.3 — Shear + Bending Interaction (Eq. 6.31–6.32)

```
M_Sd/M_Rd ≤ 1.4 - V_Sd/V_Rd       for V_Sd ≥ 0.4·V_Rd    (Eq. 6.31)
M_Sd/M_Rd ≤ 1.0                    for V_Sd < 0.4·V_Rd    (Eq. 6.32)
```

---

## Formula 6.3.8.4 — Shear + Bending + Torsion (Eq. 6.33)

```
M_Sd/M_Red,Rd ≤ 1.4 - V_Sd/V_Rd   for V_Sd ≥ 0.4·V_Rd    (Eq. 6.33)

M_Red,Rd = W · f_m,Red / γ_M
f_m,Red = f_m · √(1 - 3·(τ_T,Sd/f_d)²)
τ_T,Sd = M_T,Sd / (2·π·R²·t)
f_d = f_y / γ_M
```

---

## Architecture

*Rewritten 2026-08-27. The previous version of this section described the app as it was before §6.4
existed: it listed a `Services/ResultCache.cs` that **does not exist**, named all nine §6.3 formula
files, and did not mention `Services/Norsok64/` — the engine that actually runs. A reader following
it was pointed at the mothballed half.*

```
NorsokChecker/
├── App.xaml(.cs)
├── MainWindow.xaml(.cs)                 # UI: config, check, results, log  (large — see below)
├── MainWindow.Joint64.cs                # the §6.4 results sheet (partial)
├── Controls/
│   ├── Joint3DView.xaml(.cs)            # the joint, from presentations/text
│   └── Joint64DerivationWindow.xaml(.cs)# WebView2 host for one brace's derivation
├── Converters/
│   └── FractionConverters.cs            # proportional-bar widths (namespace stays .Controls)
├── Models/
│   ├── ConnectionCheckResult.cs         # connections-grid row
│   ├── MemberDisplayInfo.cs             # members-grid row
│   ├── Joint64RowView.cs                # §6.4 table row + LC selector option
│   └── NorsokFormulaResult.cs           # THE check-result DTO: Passed / NotAssessed / IsNote
├── Resources/                           # embedded KaTeX (offline formulas) + its licence
├── Services/
│   ├── NorsokCheckRunner.cs             # orchestrator: CBFEM plates/welds/bolts + §6.4
│   ├── ProjectSettingsService.cs        # NORSOK Table 6-1 γM into the project
│   ├── ServiceLocator.cs                # find/version-check the REST service (26.0.x)
│   ├── ServiceReaper.cs                 # Job Object: our service dies with us
│   ├── CrossSectionDetector.cs          # section types from the Material API
│   ├── MemberGeometryReader.cs          # member geometry from the Member API
│   ├── JointPresentationReader.cs       # presentations/text → member meshes
│   ├── RawResultsParser.cs              # rawresults-text → plates/welds/bolts
│   ├── NorsokHtmlReportGenerator.cs     # the report AND the §6.4 derivation sheet
│   ├── Telemetry.cs, AppLog.cs
│   │
│   ├── Norsok64/                        # ★ THE LIVE ENGINE — §6.4 simple tubular joints
│   │   ├── Norsok64Engine.cs            #   the formulas: Qu, Qf, A², N_Rd, M_Rd, eq 6.57
│   │   ├── Joint64Input.cs / Joint64Result.cs   # SI-strict DTOs at the engine boundary
│   │   ├── JointTopologyBuilder.cs      #   chord, joint plane, gaps, the assumption gates
│   │   ├── JointTopologyModels.cs       #   the vocabulary
│   │   ├── KyxClassifier.cs             #   K/Y/X from the transverse-force balance
│   │   ├── JointForceResolver.cs        #   forces into the joint plane; chord stresses
│   │   ├── JointCheckOrchestrator.cs    #   per load effect, per brace
│   │   ├── JointEnvelope.cs             #   the governing state per brace
│   │   ├── JointSectionMap.cs           #   cross-section id → D/T/fy
│   │   ├── TubeFromIom.cs               #   D/T from the IOM facet ring, not the name
│   │   ├── Joint64ReportAdapter.cs      #   a check row → a report card
│   │   └── Vec3.cs
│   │
│   └── Formulas63_Mothballed/           # §6.3 tubular MEMBERS — NOT CALLED
│       ├── (nine §6.3 formula classes, MaterialFactorCalc among them)
│       ├── TubularGeometry.cs / TubularGeometryCalc.cs
│       └── see CHAPTER_63_FINDINGS.md and CHAPTER_63_REVISIT.md
├── NuGet.config / NorsokChecker.csproj / NorsokChecker.sln
└── *.md                                 # ANALYSIS, BENCHMARK, CHAPTER_63_*, PYTHON_STOPGAP, UNIFICATION
```

**Why `Formulas63_Mothballed` is named that.** §6.3 is disconnected: two inputs eq 6.27 needs — the
effective length `kl` per plane and the far-end moments for `C_m` — are properties of the member's
unbraced span, which lies outside the joint this app can see. The code compiles and is unchanged,
but nothing calls it. The folder name says so, because the previous name (`Formulas/`) made it look
like a peer of the live engine. `CHAPTER_63_REVISIT.md` records which §6.3 checks would need no
length at all, and would therefore be re-enableable.

**The boundary that holds.** `Services/*.cs` at the root touch the API or the OS; nothing in
`Services/Norsok64/` holds an `IConnectionApiClient` — those files take API DTOs as data and
compute. That is why the §6.4 engine can be unit-tested against JSON fixtures with no service
running.

**Known structural debt.** `MainWindow.xaml.cs` still carries four responsibilities that are not UI
(service lifecycle, the check workflow, data loading, report export) alongside the event handlers.
`NorsokHtmlReportGenerator.cs` is the report, the derivation sheet, the KaTeX plumbing and a 330-line
CSS constant in one file. Both are deliberate deferrals, not oversights: the split of MainWindow is
real design work (eight shared private fields and three test seams), and the app is under daily
change.

## Connection API Endpoints Used

| Endpoint | Purpose |
|---|---|
| `Project.OpenProjectAsync(path)` | Load .ideaCon file, returns ConProject with connections |
| `Project.CloseProjectAsync(id)` | Clean shutdown |
| `Settings.GetSettingsAsync(projectId)` | Read current code factors |
| `Settings.UpdateSettingsAsync(projectId, dict)` | Set γM factors to Norsok values |
| `Calculation.CalculateAsync(projectId, ids)` | Run CBFEM analysis |
| `Calculation.GetRawJsonResultsAsync(projectId, ids)` | Get raw CBFEM results (CheckResultsData JSON) |
| `Calculation.GetResultsAsync(projectId, ids)` | Get structured plate/weld/bolt results |
| `Parameter.GetParametersAsync(projectId, id)` | Read connection parameters (geometry) |
| `Member.GetMembersAsync(projectId, id)` | Get member cross-section data |

## Implementation Order

We go **formula by formula**, verifying each one is correctly populated before moving to the next:

1. **Infrastructure**: Result caching + settings update + raw result parsing
2. **§6.3.2**: Axial Tension (simplest formula — proves the pipeline works)
3. **§6.3.3**: Axial Compression (introduces local buckling, Euler buckling)
4. **§6.3.4**: Bending (introduces section moduli W, Z)
5. **§6.3.5**: Shear (beam + torsional)
6. **§6.3.7**: Variable material factor (depends on 6.3.3 results)
7. **§6.3.8.1–6.3.8.4**: Combined load interactions

## References

- NORSOK N-004, Rev. 3, February 2013 — Design of Steel Structures
- NORSOK M-001 Ed. 6, 2020 — Structural Steel Design
- NORSOK M-120 — Material Data Sheets for Structural Steel
- EN 1993-1-8 — Design of steel structures: Design of joints
- ISO 5817 — Welding: Fusion-welded joints in steel, quality levels
