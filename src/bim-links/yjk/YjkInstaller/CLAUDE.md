# YjkInstaller — CLAUDE.md

## What this project does

`YjkInstaller.exe` is a console application that installs or uninstalls the IDEA StatiCa YJK plugin on a user's machine. It requires administrator privileges (enforced by `app.manifest`).

The plugin connects YJK (盈建科, a Chinese structural analysis platform) to IDEA StatiCa Checkbot.

---

## How to distribute

The installer is self-contained — it embeds the three YJK replacement DLLs as resources. `IDEAStatiCa.yjk.dll` and `YjkPlugin.dll` are **not** bundled with the installer; they are deployed directly to IDEA StatiCa's `net48\` folder by the build output path (`YjkPlugin.csproj` and `yjk.csproj` both output to `C:\Program Files\IDEA StatiCa\StatiCa 26.0\net48\`). The user must have IDEA StatiCa 26.0 installed before running the installer.

The three YJK replacement DLLs (`ClrYJKAPI.dll`, `YAPIData.dll`, `YJKAPI.dll`) are **embedded inside `YjkInstaller.exe`** as resources (see `Resources/` folder and `YjkInstaller.csproj`). They never need to be distributed separately — the installer extracts them at install time. If you need to update these DLLs for a new YJK version, replace the files in `Resources/` and rebuild.

---

## What install does, step by step

Running `YjkInstaller.exe /i` performs these steps in order:

### Step 1 — Find YJK installation directory
`Yjk.GetInstallation()` tries three sources in order:
1. The `/i:path` command-line override, if provided
2. Registry: `HKEY_LOCAL_MACHINE\SOFTWARE\YJKSOFT\YJKS8.1.0` → value `INSTALLFOLDER`
3. Hard-coded default: `C:\YJKS\YJKS_8_1_0\`

If none resolves to an existing directory, the installer exits with an error.

### Step 2 — Find IDEA StatiCa net48 path
`Yjk.GetIdeaNet48Path()` reads `HKLM\SOFTWARE\IDEAStatiCa\*`, picks the highest version subkey, reads `InstallDir64`, and appends `\net48`. This is where `IDEAStatiCa.yjk.dll` lives.

### Step 3 — Extract YJK replacement DLLs
`YjkPlugin.ExtractReplacementDlls()` extracts three DLLs from embedded resources into the YJK install directory, **overwriting** the stock versions:
- `ClrYJKAPI.dll`
- `YAPIData.dll`
- `YJKAPI.dll`

These are **not** the standard YJK DLLs. They are modified versions that add the ability to load .NET plugin assemblies at startup. Without them, YJK ignores `apiPlugList.txt` entirely.

### Step 4 — Register plugin in `apiPlugList.txt`
`YjkConfig.AddToApiPlugList()` adds a **relative path** entry to `[YJK install dir]\apiPlugList.txt` pointing from the YJK folder to `IDEAStatiCa.yjk.dll` in IDEA's `net48\` folder.

Example entry (for default install paths):
```
..\..\Program Files\IDEA StatiCa\StatiCa 26.0\net48\IDEAStatiCa.yjk.dll
```

The relative path is computed at install time via `Uri.MakeRelativeUri` so it works for any install location. **YJK requires a relative path** — absolute paths cause a `NotSupportedException` because YJK prepends its own install dir.

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

---

## What uninstall does (`/u`)

Reverses steps 4–5 only. The replacement DLLs are **not** deleted — removing them risks breaking the YJK installation. Only the configuration changes are reverted:
1. Remove the `IDEAStatiCa.yjk.dll` relative-path line from `apiPlugList.txt`
2. Remove `<MenuMacro UID="ID_idea_statica">` from `YJK.CUI`
3. Remove `<SubPanel Title="IDEA StatiCa">` from `YJK.CUI`

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
├── Yjk.cs               — YJK + IDEA install directory discovery (registry + fallback)
├── YjkConfig.cs         — edits apiPlugList.txt and YJK.CUI; computes relative path
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
5. Rebuild both `yjk` and `YjkInstaller` projects.

## If you need to support a new IDEA StatiCa version

The IDEA install root is discovered from registry automatically (highest version wins). The only required change is rebuilding `YjkPlugin.csproj` and `yjk.csproj` against the new IDEA version's `net48\` DLLs. No installer changes needed unless the registry key structure changes.
