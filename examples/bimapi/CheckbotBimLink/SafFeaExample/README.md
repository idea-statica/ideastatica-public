# SAF FEA link example

This example links an FEA application to IDEA StatiCa Checkbot through SAF files. The FEA application saves the
selected part of its model as a SAF file, and the [IdeaStatiCa.SAF2IOM](https://www.nuget.org/packages/IdeaStatiCa.SAF2IOM)
package converts the file for Checkbot. A small fake FEA application stands in for yours, so the example runs without
any FEA software.

To link through the API of your application instead, see the [FEA link example](../FeaExample/README.md).
[Why use SAF for a Checkbot link?](what_is_SAF.md) helps you choose.

The fake model is a steel truss: 25 members, one load case, and the internal forces of the members. It comes as two
SAF files in the `SafFiles` folder: the truss as designed, and the same truss with smaller cross-sections.

## What it shows

- Importing connections and members from a SAF file.
- Synchronizing: Checkbot asks for the objects it imported before, and updates them from a new SAF file.
- Keeping track of which IDEA StatiCa object came from which SAF object, so a sync updates the objects instead of
  adding new ones, even after Checkbot has been closed and opened again.

## Projects

| Project | What it is |
|---|---|
| `SafFeaExampleLink` | The link. Start your own link from a copy of it. |
| `SafFeaExampleApp` | The fake FEA application: a window that starts Checkbot. Your application saves its own SAF files instead. |

Start reading at `CheckbotLink.cs`, then `ISafExport.cs`, then `SafApplication.cs`.

## How the link works

1. `CheckbotLink.Run` starts Checkbot, and answers its requests until Checkbot is closed.
2. When the user imports connections or members in Checkbot, `SafApplication` asks the FEA application to save the
   selected objects to a SAF file: `ISafExport.ExportSelection`.
3. SAF2IOM reads the file, and BimImporter builds the IDEA StatiCa model from it. For connections, BimImporter finds
   the nodes where members meet.
4. When the user synchronizes, `SafApplication` asks the FEA application for the objects Checkbot imported before,
   by their SAF ids: `ISafExport.ExportObjects`. Then it imports them again.

Every object keeps its SAF id from one export to the next. The link remembers which IDEA StatiCa id belongs to which
SAF id, in `bimapi-data.json` in the Checkbot project folder. The last SAF file stays in the same folder, as
`export.xlsx`, so you can open it to see what was imported. Close it before the next import or sync: while Excel has
it open, the FEA application cannot overwrite it.

## Requirements

- Windows with IDEA StatiCa 26.1 or later installed.
- The .NET 10 SDK.

The link targets .NET Framework 4.8 and .NET 10. The fake application runs on .NET 10. All packages come from
nuget.org.

## Run it

1. Build `SafFeaExampleApp`. It references the link, so the link is built with it.
2. Start `SafFeaExampleApp`.
3. The window shows the path to Checkbot of IDEA StatiCa 26.1. Change it if your Checkbot is elsewhere, and click
   **Start Checkbot**.
4. Checkbot opens and asks for a new project. Keep the EN design code and create the project.
5. Import the connections. Checkbot imports the 25 members of the truss and a connection at each of its 14 nodes.
6. In the example window, tick **Smaller cross-sections**. Then click **Sync** in Checkbot. All the members change to
   SHS100/100/6.0.

If Checkbot does not know a cross-section or material name from the SAF file, it asks you to pick the matching one
from its library.

The Checkbot project is kept in the folder `SafFeaExampleCheckbotProject` next to the application, and the next run
opens it again. Delete the folder to start over. The application writes its log to
`%TEMP%\IdeaStatiCa\Logs\SafFeaExampleApp.log`.

## Make it your own

1. Copy `SafFeaExampleLink` and rename it, for example to `MySafLink`. Its package references take their versions
   from `Directory.Packages.props` at the root of this repository, so add `Version="26.1.0.5973"` to each of them.
2. Implement `ISafExport` with the SAF export of your application. Give every object the same SAF id in every export.
3. Call `CheckbotLink.Run` from a command in your application. Give it a writable folder for the Checkbot project,
   one per model, for example next to the model file.
4. Change `ApplicationName` in `SafApplication.cs`: Checkbot shows it as the name of your application.
