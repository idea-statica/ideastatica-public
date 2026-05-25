# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this project does

This is a BIM integration plugin that connects **YJK** (a Chinese structural analysis platform) to **IDEA StatiCa** (structural connection design). It uses an **in-process thin wrapper + AssemblyResolve** architecture:

- `IDEAStatiCa.yjk.dll` — thin wrapper loaded by YJK as a plugin via `[CommandMethod]` entry point in `Main.cs`. Has zero NuGet dependencies. Installs an `AssemblyResolve` hook, then reflection-loads `YjkPlugin.dll` from IDEA's `net48\` folder.
- `YjkPlugin.dll` — class library containing all plugin logic: reads the YJK structural model in-process (no IPC), runs the importers, and launches IDEA StatiCa Checkbot for connection design.

Both DLLs output to `C:\Program Files\IDEA StatiCa\StatiCa 26.0\net48\`. All IDEA SDK and NuGet dependencies are loaded at runtime from that same folder via the `AssemblyResolve` hook — nothing is copied to the YJK install folder.

## Build

Target: **.NET Framework 4.8**.

- `yjk.csproj` — OutputType: Class Library. Debug output: `C:\Program Files\IDEA StatiCa\StatiCa 26.0\net48\`. Sources: `Main.cs`, `AssemblyResolver.cs`. Zero NuGet and zero IDEA SDK references.
- `YjkPlugin\YjkPlugin.csproj` — OutputType: Class Library. Debug output: `C:\Program Files\IDEA StatiCa\StatiCa 26.0\net48\`. Contains all plugin logic. All IDEA SDK and NuGet references have `<Private>False</Private>` so MSBuild does not copy them to the output folder.

Build with Visual Studio. Both projects must be built — `yjk.csproj` has a `ProjectReference` to `YjkPlugin.csproj` with `ReferenceOutputAssembly=false` to enforce build order.

The projects reference unmanaged YJK DLLs (`ClrYJKAPI.dll`, `YAPIData.dll`, `YJKAPI.dll`) from `YJKS\YJKS_8_1_0\` with `<Private>False</Private><CopyLocal>False</CopyLocal>` — they are never copied out of the YJK folder.

NuGet packages resolve from the global cache (`%USERPROFILE%\.nuget\packages\`). HintPaths in `YjkPlugin.csproj` use `$(USERPROFILE)\.nuget\packages\packagename\version\` format.

The path to IDEA StatiCa Checkbot is configured in `YjkPlugin\app.config` under `CheckbotLocation`.

## Process architecture

```
yjks.exe  (single process — everything runs here)
  └─ IDEAStatiCa.yjk.dll   (thin wrapper, loaded by YJK from net48\)
       ├─ [CommandMethod("idea_statica")] — Main.Run()
       ├─ AssemblyResolver.Install() — hooks AppDomain.AssemblyResolve
       ├─ Captures Dispatcher.CurrentDispatcher (YJK UI thread)
       └─ Assembly.LoadFrom(net48\YjkPlugin.dll)
            └─ PluginEntry.Run(workingDirectory)
                 ├─ FeaApis  — calls Hi_CToSDesign etc. natively (in-process)
                 ├─ Importers / BimApis
                 ├─ YjkBimLink / YjkApplication / Model
                 └─ Launches IDEA StatiCa Checkbot
```

**Working directory**: `Main.cs` derives the working directory from `Directory.GetCurrentDirectory()` (YJK sets the CWD to the open project folder), creates `IdeaStatiCa-<projectname>\` subfolder, and passes it to `PluginEntry.Run()`.

**Why in-process**: All YJK API calls (`Hi_CToSDesign`, `Hi_AddToAndReadYjk`, etc.) require YJK's in-process global state. Running them from a separate process causes access violation 0xC0000005. The in-process architecture eliminates this — no IPC, no named pipe, no separate exe.

## Assembly isolation

IDEA StatiCa installs two builds of its SDK under `C:\Program Files\IDEA StatiCa\StatiCa 26.0\`:
- **Root folder** — .NET 8 build (references `System.Runtime 8.0.0.0`, will not load under net48)
- **`net48\` subfolder** — .NET Framework 4.8 build (references `mscorlib 4.0.0.0`) — **always use this**

`AssemblyResolver` hooks `AppDomain.AssemblyResolve` and probes `net48\` first, then the root folder as fallback (for assemblies only present at root, e.g. `CommunityToolkit.Mvvm`). This fires for every assembly that the CLR cannot find by normal probing — i.e. assemblies not already loaded by YJK and not in YJK's install folder.

YJK ships its own older versions of some assemblies (`Microsoft.Bcl.AsyncInterfaces`, `Microsoft.Extensions.DependencyInjection.Abstractions`, etc.) in `YJKS_8_1_0\`. When our plugin needs a different version, the CLR version mismatch triggers `AssemblyResolve` and we serve IDEA's copy. Both coexist in-process as separate `Assembly` instances.

**Do not** patch `yjks.exe.config` — the resolver handles all binding entirely from managed code.

**`GRPC_CSHARP_EXT_OVERRIDE_LOCATION`**: Set by `AssemblyResolver.Install()` to point at `net48\grpc_csharp_ext.x64.dll`. Required because YJK ships its own `Grpc.Core.dll` without the native extension DLL, and Grpc.Core computes the native DLL path from its own `CodeBase` (pointing at the YJK folder) before the OS loader searches PATH.

## NuGet version alignment with IDEA 26.0

All NuGet references in `YjkPlugin.csproj` are pinned to the versions IDEA 26.0 ships in `net48\`, so the resolver never needs to do version translation. `CommunityToolkit.Mvvm` is not referenced — its only usage (`AppLogger.RelayCommand`) was replaced with an inline `DelegateCommand`.

## Source file layout

All plugin source files live in `YjkPlugin/` and are compiled by `YjkPlugin.csproj`. `yjk.csproj` compiles only `Main.cs`, `AssemblyResolver.cs`, and `Properties\AssemblyInfo.cs`.

`YjkPlugin/` contains: `PluginEntry.cs`, `app.config`, `Properties/`, and the plugin source folders:
- `BimApis/` — IDEA StatiCa model objects
- `FeaApis/` — YJK data abstraction layer
- `Helpers/` — utilities (UnitConverter, LAnglePrincipalAxesConverter, WindowHelper)
- `Importers/` — transformers from FeaApis to BimApis
- `ViewModels/` — AppLogger, MessageViewModel, MessageSeverity
- `Model.cs`, `YjkApplication.cs`, `YjkBimLink.cs` — orchestration layer

## Source file layout

All plugin source files live in `YjkDriver/` and are compiled directly by `YjkDriver.csproj` (no `<Link>` indirection). `yjk.csproj` compiles only `Main.cs` and `Properties/AssemblyInfo.cs` — it does not reference any of the subfolders.

`YjkDriver/` contains: `Program.cs`, `app.config`, `Properties/`, and the plugin source folders:
- `BimApis/` — IDEA StatiCa model objects
- `FeaApis/` — YJK data abstraction layer
- `Helpers/` — utilities (UnitConverter, YjkDispatcher, LAnglePrincipalAxesConverter, WindowHelper)
- `Importers/` — transformers from FeaApis to BimApis
- `ViewModels/` — AppLogger, MessageViewModel, MessageSeverity
- `Model.cs`, `YjkApplication.cs`, `YjkBimLink.cs` — orchestration layer

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

**Entry & DI** — `YjkPlugin/PluginEntry.cs` wires Autofac, builds the container, and calls `YjkBimLink.Create(...).Run(model)`. `YjkBimLink` extends `FeaBimLink` and creates a `YjkApplication` instance. `YjkApplication` extends `BimApiApplication` and handles `ImportSelection` / `Synchronize` callbacks from IDEA StatiCa.

**Synchronize flow** — `YjkApplication.Synchronize()` calls `Model.Refresh()` then `_bimImporter.ImportSelected(items, countryCode)`. `Model.Refresh()` calls `ReadModel()` (DB read + YJK UI thread switch + load cases + clear results) followed by `geometry.GetAll()` which reads every member on every floor unconditionally. This fills `_members`/`_nodes` so the importers have fresh data without requiring a user selection. `Model.ReadModel()` is also called by `GetUserSelection()` before `geometry.GetSelected()`.

## YJK API internals

### DLL inventory and nature
Three YJK API DLLs are referenced by this project, all in `YJKS\YJKS_8_1_0\`:

| DLL | Type | Description |
|---|---|---|
| `YJKAPI.dll` | .NET 4.0 managed assembly | Contains `CsToYjk`, `YJKIPC`, `DataFuncApplication`, `MiddleTrans`, `Register` namespaces. The main API surface for reading YJK model data. |
| `YAPIData.dll` | .NET 4.0 managed assembly | Plain data classes: `APIData.Hi_DbModelData`, `Mdl_ColSeg`, `Mdl_BeamSeg`, `Mdl_BraceSeg`, `Mdl_Joint`, etc. These are the model data containers. |
| `ClrYJKAPI.dll` | Native/mixed-mode x64 DLL | Unmanaged wrapper. Cannot be reflection-loaded. Provides `Hi_CToSDesign` (floor/member geometry queries). Depends on `YJKAPI.dll` and `YAPIData.dll`. |

`NSYJKDriver.dll` (also in `YJKS_8_1_0\`) is a **fully native C++ DLL** that launches a headless YJK instance for batch processing. It is NOT an external .NET client — it uses `LoadYJKDlls`/`StartupYJKS`/`OpenYJKProject` to start its own YJK. It does not demonstrate how to connect to a running interactive YJK session.

### In-process requirement
**All YJK API calls (`Hi_AddToAndReadYjk`, `Hi_CToSDesign`, etc.) only work inside YJK's process.** They access YJK's internal global state (shared memory, COM handles, native pointers) that exists only in `yjks.exe`. Loading these DLLs into a separate process gives you the DLL binary but none of the live state — constructors crash with access violation (0xC0000005) immediately.

### Assembly conflicts
YJK ships its own versions of several .NET assemblies in `YJKS_8_1_0\`. Confirmed conflicts with our plugin's NuGet dependencies:

| Assembly | YJK version | Our version |
|---|---|---|
| `CommunityToolkit.Mvvm` | 8.0.0 | 8.4.2 |
| `Microsoft.Bcl.AsyncInterfaces` | 6.0.x | 10.0.x |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 6.0.x | 10.0.x |

`CommunityToolkit.Mvvm` is only used in `AppLogger.cs` for `RelayCommand` (dead UI code). `Microsoft.Bcl.AsyncInterfaces` and `Microsoft.Extensions.DependencyInjection.Abstractions` are transitive dependencies of Autofac/gRPC — YJK's own code does not call into our Autofac container so redirecting these to our versions is safe.

### YJKIPC — cross-process mechanism
`YJKAPI.dll` contains a cross-process IPC mechanism in the `YJKIPC` and `DataFuncApplication` namespaces, using a **Windows Memory-Mapped File named `YJKJMDATA`**.

**How it works (discovered by IL decompilation):**

- **YJK creates `YJKJMDATA`** at startup (`MemoryMappedFile.CreateOrOpen`) and keeps it populated with model data.
- **`Link2YJK.LinkPrepare<T>(model)`** — opens the *existing* `YJKJMDATA` MMF, JSON-serializes `model` (with `TypeNameHandling`, compressed via `DeflateStream`), and writes it to the MMF. Called from within YJK's process to push data into the shared segment.
- **`Link2YJK.GetIPCdata<T>()`** — opens the existing `YJKJMDATA` MMF, reads all bytes, decompresses, and JSON-deserializes to `T`. Can be called from an external process to pull data out.
- **`Link2YJK.GetIPCUnionID()`** — reads an `int` from the MMF. Identifies the current YJK session/project.
- **`YJKIPC.YJKSControl.SelectYjkProcess()`** — shows a WinForms `ListBox` dialog listing running processes for the user to pick the YJK instance. Stores the selected process's handle in a static field. **Cannot be called headlessly** — it creates a UI window.
- **`YJKIPC.YJKSControl.IPC_ShareMem()`** — calls `MemoryMappedFile.CreateOrOpen("YJKJMDATA")` and reads four fixed-size chunks (16, 48, 64, 976 bytes) from it.
- **`YJKIPC.YJKSControl.IPC_Pipe()`** — connects to YJK's named pipe using the stored process handle. Uses `EventWaitHandle` for synchronization.
- **`YJKIPC.YJKSControl.RunCmdWithMemoryMappedFile()`** — sends a command to YJK via the MMF + two `EventWaitHandle` objects for request/response synchronization.

**Data types available via `GetIPCdata<T>()`** (from string table scan of `YJKAPI.dll`):
- Structural: `SpBeam`, `SpBeamEx`, `SpColm`, `SpBrac`, `SpWall`, `SpSlab`, `SpNode`
- Model data: `Mdl_ColSeg`, `Mdl_BeamSeg`, `Mdl_BraceSeg`, `Mdl_Joint`, `Mdl_Grid`, `Mdl_BeamSect`, `Mdl_ColSect`, `Mdl_WallSeg`, `Mdl_WallSect`, `Mdl_LoadSeg`, `Mdl_LoadSect`, `Mdl_Floor`, `Mdl_StdFlr`, `Mdl_Axis`, `Mdl_Group`, `Mdl_Slab`, `Mdl_SlabHole`, `Mdl_MidSlab`, `Mdl_StairDef`, `Mdl_StairSeg`, `Mdl_Property`, `Mdl_FillWall`, `Mdl_XNQSeg`, `Mdl_XNQSect`, `Mdl_BeamJY`, `Mdl_JGSeg`, `Mdl_WallHole`, `Mdl_WallHoleDef`, `Mdl_SlabHoleDef`, `Mdl_SkinSeg`

**`Sp*` types location:** Not found in `YJKAPI.dll` or `YAPIData.dll` — likely in one of the larger YJK assemblies (`NSStructureModelPlus.dll`, `DsnToolBase.dll`, etc.) that are not referenced by our project.

**`DataFuncApplication.Link2YJK`** is a singleton (`Instance` property, thread-safe double-checked locking pattern visible in IL). Fields: `instance`, `padlock`, `DeleteIdList`, `AddIdSet`, `ModifyIdSet`, `memoryMappedFile_`.

### Ribbon switch thread requirement
`CsQSetCurrentRibbonLabel("IDDSN_DSP")` modifies YJK's UI and **must run on YJK's UI thread**. All other YJK API calls (geometry reads, data reads) can run from any thread within YJK's process — they access shared in-process state without thread affinity requirements.

## Key domain knowledge

### Thread marshaling
The ribbon switch (`CsQSetCurrentRibbonLabel("IDDSN_DSP")`) must run on the YJK UI thread. `Main.Run()` captures `Dispatcher.CurrentDispatcher` and assigns it to `PluginEntry.YjkDispatcher` before invoking plugin code. The plugin switches the ribbon via `PluginEntry.YjkDispatcher.Invoke(() => new ClrYJKSUI().CsQSetCurrentRibbonLabel("IDDSN_DSP"))`. All other YJK geometry/data calls can run from any thread within YJK's process.

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
