# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this project does

This is a BIM integration plugin that connects **YJK** (a Chinese structural analysis platform) to **IDEA StatiCa** (structural connection design). It uses a **thin launcher + driver EXE** architecture:

- `IDEAStatiCa.yjk.dll` — thin launcher loaded by YJK as a plugin via `[CommandMethod]` entry point in `Main.cs`. Has zero NuGet dependencies. Spawns `YjkDriver.exe` as a separate process.
- `YjkDriver.exe` — driver process that runs outside YJK's process space with its own `YjkDriver.exe.config`. Contains all plugin logic: reads the YJK structural model, runs the importers, and launches IDEA StatiCa Checkbot for connection design.

## Build

Target: **.NET Framework 4.8**.

- `yjk.csproj` — OutputType: Class Library. Debug output: `YJKS\YJKS_8_1_0\`
- `YjkDriver.csproj` — OutputType: WinExe (no console window). Debug output: `YJKS\YJKS_8_1_0\IdeaStatiCa\`. `Main.cs` launches it directly from there — no AfterBuild copy step.

Build with Visual Studio. Both projects must be built — building `yjk` triggers a build-order dependency on `YjkDriver` (via `ReferenceOutputAssembly=false` ProjectReference).

The projects depend on unmanaged YJK DLLs (`ClrYJKAPI.dll`, `YAPIData.dll`, `YJKAPI.dll`) in `YJKS\YJKS_8_1_0\`. There are no test projects.

NuGet packages resolve from the global cache (`%USERPROFILE%\.nuget\packages\`) — there is no local `packages\` folder. Hint paths in `YjkDriver.csproj` use `$(USERPROFILE)\.nuget\packages\packagename\version\` format.

The path to IDEA StatiCa Checkbot is configured in `YjkDriver/app.config` under `CheckbotLocation` (defaults to `C:\Program Files\IDEA StatiCa\StatiCa 25.1\IdeaCheckbot.exe`).

## Process architecture

```
YJK process
  └─ IDEAStatiCa.yjk.dll  (thin launcher)
       └─ Process.Start("YjkDriver.exe", "<pid> <workingDir>")
                │
                ▼
YjkDriver.exe process  (all plugin logic, isolated from YJK's CLR)
  ├─ own YjkDriver.exe.config  (binding redirects + codeBase + probing)
  ├─ GrpcBimHostingFactory
  ├─ YjkBimLink / YjkApplication / Model
  ├─ FeaApis / Importers / BimApis
  └─ YjkBimLink.Run(model)
```

**Working directory**: `Main.cs` derives the working directory from `Directory.GetCurrentDirectory()` (YJK sets the CWD to the open project folder), creates `IdeaStatiCa-<projectname>\` subfolder, and passes it as `args[1]` to the driver.

**Launcher dir**: `Main.cs` uses `new Uri(typeof(Main).Assembly.CodeBase).LocalPath` — not `Assembly.Location` — because YJK loads DLLs via URI paths that `Path.GetDirectoryName` alone can't handle.

## Assembly isolation

YJK ships older versions of several assemblies (`System.Memory`, `Microsoft.Bcl.AsyncInterfaces`, `CommunityToolkit.Mvvm`, etc.) in `YJKS_8_1_0\`. The driver uses newer versions. To prevent the CLR from picking up YJK's copies:

- All plugin DLLs (NuGet outputs) land in `YJKS_8_1_0\IdeaStatiCa\` (the driver's `OutputPath`)
- `YjkDriver.exe` also lives in `IdeaStatiCa\`, so the CLR finds all DLLs by default probing in the EXE's own directory — no `<probing>` element needed
- For each assembly where YJK ships a conflicting version in the root folder, `YjkDriver.exe.config` has a `<bindingRedirect>` to redirect to the version in `IdeaStatiCa\` — since the EXE is inside that folder the CLR picks up the correct copy automatically

**Do not** patch `yjks.exe.config` — the driver's isolation means it is never needed.

## Source file layout

`YjkDriver.csproj` does not copy source files — it links them from the `yjk/` folder using `<Compile Include="..\FeaApis\*.cs"><Link>...</Link></Compile>`. Source files live in `yjk/` and are shared by both projects. `YjkDriver/` contains only: `Program.cs`, `app.config`, `Properties/`.

## Architecture

The code has three distinct layers that data flows through in sequence:

```
YJK native APIs
      ↓
FeaApis/        — reads and abstracts YJK data into plain C# objects (FeaMember, FeaNode, etc.)
      ↓
Importers/      — transforms FeaApis objects into IDEA StatiCa's object model
      ↓
BimApis/        — IDEA StatiCa model objects (Member1D, CrossSectionByParameters, etc.)
```

**Orchestration** — `Model.cs` implements `IFeaModel` and is the bridge. `Model.GetUserSelection()` is the main entry point called by the framework: it reads from YJK, populates the FeaApis layer, then returns selected node/member identifiers. The framework then calls the importers to convert each identifier on demand.

**Entry & DI** — `YjkDriver/Program.cs` wires Autofac, builds the container, and calls `YjkBimLink.Create(...).Run(model)`. `YjkBimLink` extends `FeaBimLink` and creates a `YjkApplication` instance. `YjkApplication` extends `BimApiApplication` and handles `ImportSelection` / `Synchronize` callbacks from IDEA StatiCa.

**Synchronize flow** — `YjkApplication.Synchronize()` calls `Model.Refresh()` then `_bimImporter.ImportSelected(items, countryCode)`. `Model.Refresh()` calls `ReadModel()` (DB read + YJK UI thread switch + load cases + clear results) followed by `geometry.GetAll()` which reads every member on every floor unconditionally. This fills `_members`/`_nodes` so the importers have fresh data without requiring a user selection. `Model.ReadModel()` is also called by `GetUserSelection()` before `geometry.GetSelected()`.

## Key domain knowledge

### Thread marshaling
Only the YJK UI ribbon switch (`CsQSetCurrentRibbonLabel("IDDSN_DSP")`) must run on the YJK UI thread via `YjkDispatcher.Invoke()` inside `Model.ReadModel()`. Geometry calls (`Hi_CToSDesign`, `Hi_AddToAndReadYjk`, `_Hi_DesignData`, etc.) do not need to be marshalled — they can be called from the calling thread directly.

### Units
- YJK provides coordinates and section dimensions in **millimetres**
- IDEA StatiCa expects **metres**
- All conversions go through `UnitConverter.MmToM()` — apply it whenever reading YJK geometry or cross-section dimensions
- Internal forces from YJK are in kN/kNm; `ResultsImporter` multiplies by `unitConversionFactor = 1000` to convert to N/Nm

### Sign conventions
YJK and IDEA StatiCa use different sign conventions. Corrections applied in `FeaResultsApi.GetFeaMemberResult()`:
- `Mz *= -1`, `Vy *= -1` for all member types
- `My *= -1` additionally for **columns only**

### Rotation angles
YJK rotation angles are stored with opposite sign to IDEA StatiCa. All rotation angles are multiplied by `-1` in `FeaGeometryApi.GetRotationAngle()`.

### Local coordinate system (LCS)
`FeaGeometryApi.CalculateMemberLcs()` distinguishes two cases:
- **Columns** (member axis parallel to global Z): local Y = global −X, local Z = X × Y
- **Beams/Braces**: local Y = −(X × globalZ), local Z = X × Y

### L-angle (rolled angle) sections — special case
Rolled angle sections (`CrossSectionType.RolledAngle`) are handled by `LAnglePrincipalAxesConverter` in `Helpers/`. YJK returns internal forces for L-angles in principal axes (u, v) rather than local axes. `FeaResultsApi.GetFeaMemberResult()` detects the RolledAngle cross-section type, computes the principal axis angle α, and rotates the forces to local axes before storing them. The standard YJK→IDEA sign corrections (`Mz *= -1`, `Vy *= -1`, `My *= -1` for columns) are then applied as normal.

**Principal axis angle (`ComputePrincipalAngle`):** Uses a two-rectangle thin-walled approximation. The product of inertia `Iyz` is negated before passing to `Atan2` because YJK's local Y points left — mirroring Y flips the sign of the product of inertia.

**Force rotation (`ToLocalAxes`):** The v-axis moments are negated before rotation because the u and v axes point outward from the centroid toward the material. By the right-hand rule, positive Mv produces tension on the **+z** side (opposite to the standard convention assumed by the rotation formula), so Mv must be negated:
```
My = Mu*cos + Mv*sin    (i.e. − (−Mv)*sin)
Mz = Mu*sin − Mv*cos    (i.e. + (−Mv)*cos)
```
The shear forces Vu, Vv use the standard rotation without negation.

The cross-section type check currently uses `GetType().Name == "CrossSectionByParameters"` + `cs.Type == CrossSectionType.RolledAngle` — prefer `is CrossSectionByParameters cs` pattern matching if modifying this code.

**Known limitation:** If an L-angle is defined by name (Kind==26 named steel profile, or Kind==28 with unequal flanges falling back to `ByName`), the `CrossSectionBy.ByParameters` check in `FeaResultsApi.GetFeaMemberResult()` will be `false` and the principal-axis force conversion will be skipped. YJK still outputs forces in u/v principal axes for these sections, so the imported results will be incorrect. There is currently no workaround.

### Cross-section parsing
YJK sections are defined by a `Kind` integer and a comma-separated `ShapeVal` string. `FeaCrossSectionApi.CreateCrossSection()` maps `Kind` values to IDEA StatiCa factories:
- 1 = Rectangle, 2 = I-section (symmetric→RolledI, asymmetric→WeldedAsymI), 3 = Circle, 5 = Channel, 7 = Box, 8 = CHS, 9 = Double channel, 26 = named steel profile, 28 = L-angle, 29 = T-section
- Unrecognised kinds fall back to `CrossSectionBy.ByName`

### Material mapping
Steel: `matType == 5`, grade encoded in `matGrade2` as an integer (80201=Q235, 80202=Q345, …, 80209=Q690).  
Concrete: `matType == 6`, `matGrade` holds Fck directly.

### Load case and load group mapping
YJK's `dsnGetLDCaseBySortNew` fills three parallel arrays: `LDCase` (analysis ID), `LDCaseOld` (modelling ID), `LDKind` (load kind integer). **The switch in `FeaLoadsApi.GetLoadCasesAndCombos()` keys on `LDKind[i]`**, not `LDCase[i]`:
- 1=Dead, 2=Live, 3=Wind, 4=HorizontalSeismic, 5=VerticalSeismic, 6=CivilDefence, 7=Crane, 8=Temperature

Load groups are created dynamically via `EnsureLoadGroup()` — at most two groups are produced (Permanent id=1, Variable id=2), only for categories that actually appear. Variable types: Live, Wind, Seismic, Crane, Temperature. Permanent types: Dead, CivilDefence (and the default).

`LoadCaseImporter.GetLoadType()` maps each `TypeOfLoadCase` to an IDEA StatiCa `(LoadCaseType, LoadCaseSubType)` pair:
- Dead → (Permanent, PermanentStandard)
- Live → (Variable, VariableStatic), Wind → (Variable, VariableDynamic)
- HorizontalSeismic / VerticalSeismic → (Accidental, VariableDynamic)
- CivilDefence → (Accidental, VariableStatic)
- Crane / Temperature → (Variable, VariableStatic)

### Result deduplication
`ResultsImporter` shifts result section positions by `5e-6` when two results land at the same normalised location (precision `1e-6`), because IDEA StatiCa cannot handle duplicate positions. The first result is always at 0 and the last at 1.

## ID strategy
Cross-section and material IDs are **strings** (changed from int in recent commits). The ID used is the `ShapeVal` string for parametric sections, or `section.Name` for named profiles. There is intentionally no deduplication lookup for cross-sections currently — each member call adds a new entry.

## Logging
All classes use `private IPluginLogger _logger = AppLogger.Instance` (singleton, Serilog-backed). Every public/protected method logs its entry with `LogInformation` including key input parameters. This is intentional — all logs use `LogInformation` (not `LogDebug`) so the output lands in `IdeaYJKPlugin.log` for user bug reporting. Do not downgrade entry logs to `LogDebug`. Warnings (`LogWarning`) are used for unexpected inputs (unrecognised enum values, missing data) that fall back to a default.

Key private methods that also log (because they contain meaningful YJK I/O or sign-sensitive logic):
- `FeaGeometryApi.GetConnectedMembers` — entry (memberType, nodeCount) and exit (members added count)
- `FeaGeometryApi.GetSelectedMembers` — entry (memberType)
- `FeaGeometryApi.GetMembers` — entry (memberType)
- `FeaGeometryApi.GetSelected` / `GetAll` — exit summary (total members and nodes)
- `FeaGeometryApi.GetRotationAngle` — logs raw YJK value and corrected value (sign flip is a known bug source)
- `FeaResultsApi.GetFeaMemberResult` — entry (memberId, nSect, loadCaseId, type)
- `ResultsImporter.GetResultsForMember` — entry via `AppLogger.Instance` (static method, no instance logger); logs memberId, loadCaseId, resultCount

Do NOT add logging to: data classes, simple one-liner `Get*` retrievals, pure math helpers (`UnitConverter`, `CalculateMemberLcs`, `LAnglePrincipalAxesConverter`), or `AddNode` when a node already exists (duplicate-skip is intentional and silent).
