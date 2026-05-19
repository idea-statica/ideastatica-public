# YjkInstaller — CLAUDE.md

## What this project does

`YjkInstaller.exe` is a console application that installs or uninstalls the IDEA StatiCa YJK plugin on a user's machine. It requires administrator privileges (enforced by `app.manifest`).

The plugin connects YJK (盈建科, a Chinese structural analysis platform) to IDEA StatiCa Checkbot. Installing the plugin involves four distinct steps across three config files and a set of DLL copies. This installer automates all of them.

---

## How to distribute

The installer is **not** self-contained for the plugin DLLs. When releasing a new version:

1. Build the `yjk` plugin project in Release (`IDEAStatiCa.yjk.dll` and all its dependencies land in `yjk\bin\Release\`).
2. Build this project in Release (`YjkInstaller.exe` lands in `YjkInstaller\bin\Release\net48\`).
3. Package them together — copy `YjkInstaller.exe` into the same folder as the plugin output, or copy all plugin DLLs into the installer's output folder. The user receives a single folder containing both.

The three YJK replacement DLLs (`ClrYJKAPI.dll`, `YAPIData.dll`, `YJKAPI.dll`) are **embedded inside `YjkInstaller.exe`** as resources (see `Resources/` folder and `YjkInstaller.csproj`). They never need to be distributed separately — the installer extracts them at install time. If you need to update these DLLs for a new YJK version, replace the files in `Resources/` and rebuild.

---

## What install does, step by step

Running `YjkInstaller.exe /i` performs these steps in order:

### Step 1 — Find YJK installation directory
`Yjk.GetInstallation()` tries three sources in order:
1. The `/i:path` command-line override, if provided
2. Registry: `HKEY_LOCAL_MACHINE\SOFTWARE\YJKSOFT\YJKS8.0.0` → value `INSTALLFOLDER`
3. Hard-coded default: `C:\YJKS\YJKS_8_0_0\`

If none resolves to an existing directory, the installer exits with an error.

### Step 2 — Extract YJK replacement DLLs
`YjkPlugin.ExtractReplacementDlls()` extracts three DLLs from embedded resources into the YJK install directory, **overwriting** the stock versions:
- `ClrYJKAPI.dll`
- `YAPIData.dll`
- `YJKAPI.dll`

These are **not** the standard YJK DLLs. They are modified versions that add the ability to load .NET plugin assemblies at startup. Without them, YJK ignores `apiPlugList.txt` entirely. The source files are stored in `Resources/` and baked into the exe as `EmbeddedResource` items.

### Step 3 — Copy plugin DLLs
`YjkPlugin.CopyPluginFiles()` copies all files from the installer's own directory to the YJK install directory, skipping:
- The three replacement DLLs (already handled in step 2)
- `YjkInstaller.exe`, `YjkInstaller.exe.config`, `YjkInstaller.pdb`

This copies `IDEAStatiCa.yjk.dll` and all its runtime dependencies (Serilog, MathNet, Autofac, IdeaStatiCa libs, etc.).

### Step 4 — Register plugin in `apiPlugList.txt`
`YjkConfig.AddToApiPlugList()` adds the line `IDEAStatiCa.yjk.dll` to `[YJK install dir]\apiPlugList.txt`.

Format: one DLL name (no path) per line. YJK reads this file at startup and loads each listed assembly from its own install directory.

### Step 5 — Add ribbon button to `YJK.CUI`
`YjkConfig.AddToCui()` edits `[YJK install dir]\YJK.CUI` (a large XML file defining the entire YJK UI) in two places:

**Insertion point 1 — command definition** in `<MacroGroup Name="YjkMacros">`:
```xml
<MenuMacro UID="ID_idea_statica">
    <Command>idea_statica</Command>
    <HelpString UID="XLS_Column">IDEA StatiCa</HelpString>
    <HelpRef>IDEA StatiCa</HelpRef>
    <Description>IDEA StatiCa</Description>
    <RcImage Name="RCDATA_2525" />
</MenuMacro>
```

**Insertion point 2 — ribbon button** in `<SubRibbon Id="IDModule_Layer">`, inserted after `<SubPanel Title="导出IFC">`:
```xml
<SubPanel Title="IDEA StatiCa">
    <RibbonRow Title="">
        <UIElement ElementType="CommandButton" Label="IDEA StatiCa"
            Orientation="Vertical" Id="ID_idea_statica" ShowLabel="True"
            ResizeStyle="Auto" Size="Large" MenuMacroId="ID_idea_statica" />
    </RibbonRow>
</SubPanel>
```

The original `YJK.CUI` is backed up to `YJK.CUI.bak` before modification.

The command name `idea_statica` maps to the `[CommandMethod("idea_statica")]` entry point in `Main.cs` of the plugin project.

### Step 6 — Merge assembly binding redirects into `yjks.exe.config`
`YjkConfig.MergeBindingRedirects()` adds missing `<dependentAssembly>` entries to `[YJK install dir]\yjks.exe.config`. The stock YJK config is missing redirects for assemblies the plugin depends on. The installer adds them only if not already present:

| Assembly | New version |
|---|---|
| `Microsoft.Bcl.AsyncInterfaces` | 8.0.0.0 |
| `System.Runtime.CompilerServices.Unsafe` | 6.0.3.0 |
| `System.Memory` | 4.0.5.0 |
| `System.Threading.Tasks.Extensions` | 4.2.4.0 |
| `System.Collections.Immutable` | 5.0.0.0 |
| `System.Buffers` | 4.0.3.0 |
| `System.IO.Pipelines` | 6.0.0.1 |
| `Serilog` | 4.2.0.0 |

The original `yjks.exe.config` is backed up to `yjks.exe.config.bak` before modification.

---

## What uninstall does (`/u`)

Reverses steps 4–6 only. The replacement DLLs and plugin DLLs are **not** deleted — removing them risks breaking the YJK installation if it has come to depend on them. Only the configuration changes are reverted:
1. Remove `IDEAStatiCa.yjk.dll` line from `apiPlugList.txt`
2. Remove `<MenuMacro UID="ID_idea_statica">` from `YJK.CUI`
3. Remove `<SubPanel Title="IDEA StatiCa">` from `YJK.CUI`
4. Remove the plugin-specific `<dependentAssembly>` entries from `yjks.exe.config`

---

## Command-line reference

| Argument | Description |
|---|---|
| `/i` | Install using auto-detected YJK path |
| `/i:C:\path\to\yjk` | Install using explicit YJK install directory |
| `/u` | Uninstall (reverts config changes only) |
| `ls` | Print JSON `[{"Key":"LS","Value":"1"}]` — 1 if plugin is registered |
| `aps` | Print JSON `[{"Key":"APS","Value":"1"}]` — 1 if YJK install dir found |
| `/?` | Show help |

---

## File structure

```
YjkInstaller/
├── Program.cs           — entry point, argument parsing, JSON status output
├── Yjk.cs               — YJK install directory discovery (registry + fallback)
├── YjkConfig.cs         — edits apiPlugList.txt, YJK.CUI, yjks.exe.config
├── YjkPlugin.cs         — orchestrates install/uninstall; extracts embedded DLLs
├── YjkInstaller.csproj  — SDK-style net48 console exe; embeds Resources/*.dll
├── app.manifest         — requireAdministrator UAC elevation
└── Resources/
    ├── ClrYJKAPI.dll    — replacement DLL (embedded resource)
    ├── YAPIData.dll     — replacement DLL (embedded resource)
    └── YJKAPI.dll       — replacement DLL (embedded resource)
```

---

## If you need to support a new YJK version

1. Obtain the new replacement DLLs for the new YJK version from the YJK SDK / contacts.
2. Replace `Resources/ClrYJKAPI.dll`, `Resources/YAPIData.dll`, `Resources/YJKAPI.dll`.
3. If the registry key changes (e.g. `YJKS9.0.0`), update `YjkRegistryKey` in `Yjk.cs` and `YjkDefaultInstallPath`.
4. If the CUI structure changes, update the insertion logic in `YjkConfig.AddToCui()` — check the new `YJK.CUI` to find the correct parent elements.
5. If the plugin gains new assembly dependencies, add their binding redirects to `PluginBindingRedirects` in `YjkConfig.cs`.
6. Rebuild both `yjk` and `YjkInstaller` projects and repackage.
