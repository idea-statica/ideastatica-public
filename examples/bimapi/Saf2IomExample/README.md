# SAF to IOM example

This example converts a SAF file to IOM, the IDEA Open Model, with the
[IdeaStatiCa.SAF2IOM](https://www.nuget.org/packages/IdeaStatiCa.SAF2IOM) package. It is a small window: open a SAF
file, see what the IOM holds, and save it. Use it to see what SAF2IOM makes of the SAF files of your application.

[SAF](https://www.saf.guide) is an open format for structural analysis models: an Excel workbook with one sheet for
each kind of object. To link an FEA application to IDEA StatiCa Checkbot through SAF files, see the
[SAF FEA link example](../CheckbotBimLink/SafFeaExample/README.md).

The sample, `SafFiles/saf-example1.xlsx`, is a steel structure saved by SCIA Engineer, 12 m by 8 m and 4 m high:
31 members, three load cases with the internal forces of the members, and two load combinations.

## What it shows

- Converting a SAF file with `SAF2IOMConverter`, and the input it needs, `ISafConverterInput`.
- What the IOM holds: the members, a connection at every node where two or more members meet and at every supported
  node, the load cases, and the internal forces of the members.
- Saving the model and its internal forces in one file that the Connection API can create a project from.

Start reading at `MainWindow.xaml.cs`, at `Convert`, then `SafConverterInput.cs`.

## Requirements

- Windows and the .NET 10 SDK.
- To create a Connection project from the IOM: IDEA StatiCa 26.1 or later.

All packages come from nuget.org.

## Run it

1. Build `Saf2IomExample` and start it.
2. Click **Open SAF...**. The dialog opens in the `SafFiles` folder next to the application. Open `saf-example1.xlsx`.
3. After a few seconds the window lists what the IOM holds: 31 members, 24 connections, 3 load cases, 0 load
   combinations, and internal forces on 31 members.
4. Click **Save IOM...** and save the file.

The load combinations are missing because the sample's two combinations follow a national standard, and SAF does not
hold the load factors of such combinations. SAF2IOM skips them. The log, `%TEMP%\IdeaStatiCa\Logs\Saf2IomExample.log`,
says which objects SAF2IOM skipped.

## Use the IOM

The saved file is an `OpenModelContainer` in XML: the model and its internal forces together. The
[IdeaStatiCa.ConnectionApi](https://www.nuget.org/packages/IdeaStatiCa.ConnectionApi) package creates a Connection
project from it with `CreateProjectFromIomFileAsync`. Give it the ids of the connections to create, the `Id` of each
`ConnectionPoint` in the model. Without them, it creates only the first connection. For the sample, the project gets
all 24 connections, with internal forces on their members.

## Make it your own

1. Add the IdeaStatiCa.SAF2IOM package to your project. The package references here have no versions, because they
   come from `Directory.Packages.props`. Give yours a `Version`: the package version on nuget.org that matches your
   IDEA StatiCa, 26.1 or later. Give all four parts of it, as nuget.org lists it.
2. Copy `SafConverterInput.cs`.
3. Call `SAF2IOMConverter.Convert` as `Convert` in `MainWindow.xaml.cs` does. Pick the design code the IOM is made
   for: here it is EN, `DesignCode`.
