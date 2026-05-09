# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this project does

This is a BIM integration plugin (`IDEAStatiCa.yjk.dll`) that connects **YJK** (a Chinese structural analysis platform) to **IDEA StatiCa** (structural connection design). It is loaded by YJK as a plugin via two AutoCAD-style `[CommandMethod]` entry points in `Main.cs`. The plugin reads the YJK structural model and analysis results, then launches IDEA StatiCa Checkbot for connection design.

## Build

Target: **.NET Framework 4.8**, output type: Class Library.

Build with Visual Studio or MSBuild. Debug output is written directly to the YJK installation folder:
```
..\..\..\..\..\..\..\..\YJKS\YJKS_8_0_0\
```

The project depends on unmanaged YJK DLLs (`ClrYJKAPI.dll`, `YAPIData.dll`, `YJKAPI.dll`) that must be present in `YJKS\YJKS_8_0_0\` relative to the repo root. There are no test projects.

The path to IDEA StatiCa Checkbot is configured in `app.config` under `CheckbotLocation` (defaults to `C:\Program Files\IDEA StatiCa\StatiCa 25.1\IdeaCheckbot.exe`).

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

**Entry & DI** — `Main.cs` wires Autofac, builds the container, and calls `YjkBimLink.Create(...).Run(model)`. `YjkBimLink` extends `FeaBimLink` and creates a `YjkApplication` instance. `YjkApplication` extends `BimApiApplication` and handles `ImportSelection` / `Synchronize` callbacks from IDEA StatiCa.

## Key domain knowledge

### Thread marshaling
All YJK API calls after initial selection must happen on the YJK UI thread. `Model.GetUserSelection()` uses `YjkDispatcher.Invoke()` to marshal back. Do not call YJK APIs (`Hi_CToSDesign`, `Hi_AddToAndReadYjk`, `_Hi_DesignData`, etc.) outside of a dispatcher invoke.

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
Rolled angle sections (`CrossSectionType.RolledAngle`) must use `ResultLocalSystemType.Principle` instead of `Local`, and their `Mz` and `Vz` results must be negated. This is handled in `ResultsImporter`. The cross-section type check currently uses `GetType().Name == "CrossSectionByParameters"` + `cs.Type == CrossSectionType.RolledAngle` — prefer `is CrossSectionByParameters cs` pattern matching if modifying this code.

### Cross-section parsing
YJK sections are defined by a `Kind` integer and a comma-separated `ShapeVal` string. `FeaCrossSectionApi.CreateCrossSection()` maps `Kind` values to IDEA StatiCa factories:
- 1 = Rectangle, 2 = I-section (symmetric→RolledI, asymmetric→WeldedAsymI), 3 = Circle, 5 = Channel, 7 = Box, 8 = CHS, 9 = Double channel, 26 = named steel profile, 28 = L-angle, 29 = T-section
- Unrecognised kinds fall back to `CrossSectionBy.ByName`

### Material mapping
Steel: `matType == 5`, grade encoded in `matGrade2` as an integer (80201=Q235, 80202=Q345, …, 80209=Q690).  
Concrete: `matType == 6`, `matGrade` holds Fck directly.

### Result deduplication
`ResultsImporter` shifts result section positions by `5e-6` when two results land at the same normalised location (precision `1e-6`), because IDEA StatiCa cannot handle duplicate positions. The first result is always at 0 and the last at 1.

## ID strategy
Cross-section and material IDs are **strings** (changed from int in recent commits). The ID used is the `ShapeVal` string for parametric sections, or `section.Name` for named profiles. There is intentionally no deduplication lookup for cross-sections currently — each member call adds a new entry.
