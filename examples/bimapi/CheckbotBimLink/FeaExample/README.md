# FEA link example

This example links an FEA application to IDEA StatiCa Checkbot. It is built on the
[IdeaStatiCa.BimApiLink](https://www.nuget.org/packages/IdeaStatiCa.BimApiLink) package. A small fake FEA application
stands in for yours, so the example runs without any FEA software. If your application is written in native C++, see
the [C++ FEA link example](../CppFeaExample/README.md): the same model and the same link, in C++/CLI.

The fake model is one corner of a steel frame: a 3 m column and a 5 m beam, both IPE200 in S 355, meeting at node 2.
It has three load cases (self-weight, dead load and snow), four combinations of them, and the internal forces of both
members for each load case.

## What it shows

- Importing a connection: the user selects a node and the members that meet there.
- Converting nodes, members, cross-sections and materials to IDEA StatiCa objects.
- Importing load groups, load cases and combinations.
- Importing the internal forces of the members, which Checkbot turns into the loads of the connection.

## Projects

| Project | What it is |
|---|---|
| `BimApiLinkFeaExample` | The link. Start your own link from a copy of it. |
| `FeaExampleApi` | The fake FEA application. Your link uses the API of your application instead. |

Start reading at `CheckbotLink.cs`, then `Model.cs`, then the `Importers` folder.

## How the link works

1. `CheckbotLink.Run` starts Checkbot. It registers one importer for each kind of object, and `ResultsImporter` for the
   internal forces.
2. When the user imports in Checkbot, Checkbot asks `Model` what is selected. `Model` answers with identifiers only.
   Each selected node becomes a connection of the selected members that meet there.
3. Checkbot then asks for each object by its identifier. BimApiLink passes the request to the importer of that kind of
   object. The importer reads the object from the FEA application and returns an IDEA StatiCa object.
4. `Model` also gives every combination of the model. Their load cases and load groups are imported with them.
5. Last, Checkbot asks `ResultsImporter` for the internal forces of the imported members, for every load case.

Every object is identified by its FEA id. Checkbot looks up cross-sections and materials by their names in the IDEA
StatiCa library.

Some objects refer to others: a member to its cross-section, a cross-section to its material. The classes in the
`BimApi` folder keep only the id of the referenced object, and BimApiLink imports it when it reads the reference. An
importer can also get a referenced object itself with `Get<T>`, as `LoadCaseImporter` does for the load group.

Lengths are in metres. The fake application gives forces in kN and moments in kNm, and `ResultsImporter` converts them
to N and Nm. The internal forces are in each member's own axes, which `MemberImporter` passes on with the member.

## Requirements

- Windows with IDEA StatiCa 26.1 installed.
- The .NET 10 SDK.

The link targets .NET Framework 4.8 and .NET 10. All packages come from nuget.org.

## Run it

The link is a library. `BimLinkExampleRunner`, in the folder next to this one, is a small window that starts it.

1. Build `examples/IdeaStatiCa-SDK-Examples.sln`.
2. Start `BimLinkExampleRunner`.
3. Select **FEA**, enter the path to Checkbot, for example
   `C:\Program Files\IDEA StatiCa\StatiCa 26.1\IdeaCheckbot.exe`, and click **Run IDEA StatiCa Checkbot**.
4. Checkbot opens and asks for a new project. Keep the EN design code and create the project.
5. Import the connections. Checkbot imports the column, the beam and one connection where they meet. Open the
   connection in IDEA StatiCa Connection to see the loads that come from the internal forces.

The Checkbot project is kept in the folder `FeaExampleCheckbotProject` next to the runner, and the next run opens it
again. Delete the folder to start over.

## Make it your own

1. Copy `BimApiLinkFeaExample` and rename it, for example to `MyFeaLink`. Its package references take their versions
   from `Directory.Packages.props` at the root of this repository, so add `Version="26.1.0.5973"` to each of them.
2. Replace `FeaExampleApi` with the API of your application. `IFeaGeometryApi`, `IFeaLoadsApi` and `IFeaResultsApi`
   list what the link needs from it.
3. Call `CheckbotLink.Run` from a command in your application. Give it a writable folder for the Checkbot project,
   one per model, for example next to the model file.
