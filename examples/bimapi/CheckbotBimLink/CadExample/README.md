# CAD link example

This example links a CAD application to IDEA StatiCa Checkbot. It is built on the
[IdeaStatiCa.BimApiLink](https://www.nuget.org/packages/IdeaStatica.BimApiLink) package. A small fake CAD application
stands in for yours, so the example runs without any CAD software.

The fake model is one steel connection: an HE200B beam bolted to an HE240B column through an end plate, with a square
opening in the beam web.

## What it shows

- Importing one connection: the user picks a point and selects the parts of the connection.
- Importing many connections at once: the link finds the joints among the selected objects with
  [IdeaStatiCa.ItemsSorter](https://www.nuget.org/packages/IdeaStatiCa.ItemsSorter).
- Converting members, plates, bolts, cuts and openings to IDEA StatiCa objects.

## Projects

| Project | What it is |
|---|---|
| `BimApiLinkCadExample` | The link. Start your own link from a copy of it. |
| `CadExampleApi` | The fake CAD application. Your link uses the API of your application instead. |

Start reading at `CheckbotLink.cs`, then `Model.cs`, then the `Importers` folder.

## How the link works

1. `CheckbotLink.Run` starts Checkbot and registers one importer for each kind of object.
2. When the user imports in Checkbot, Checkbot asks `Model` what is selected. `Model` answers with identifiers only.
3. Checkbot then asks for each object by its identifier. BimApiLink passes the request to the importer of that kind of
   object. The importer reads the object from the CAD application and returns an IDEA StatiCa object.

How objects are identified:

| Object | Identifier |
|---|---|
| Member, plate, bolt grid, cut | Its CAD id. |
| Connected member | The CAD id of the member, as text. |
| Material, cross-section | Its name, which Checkbot looks up in the IDEA StatiCa library. |
| Node | Its coordinates, `x;y;z` (see `Conversion/NodeId.cs`). |
| Connection | The coordinates of its node. |

Some objects refer to others: a plate to its material, a member to its cross-section. The classes in the `BimApi`
folder keep only the identifier of the referenced object, and BimApiLink imports it when it reads the reference.
Where the kind of the referenced object depends on the CAD data, the importer gets the object itself with `Get<T>`.
For example, a cut can be made by a plate, a negative plate or a member.

Lengths are in metres and areas in square metres.

## Requirements

- Windows with IDEA StatiCa 26.1 installed.
- The .NET 10 SDK.

The link targets .NET Framework 4.8 and `net10.0-windows10.0.22621`, the Windows version `IdeaStatiCa.ItemsSorter`
requires. All packages come from nuget.org.

## Run it

The link is a library. `BimLinkExampleRunner`, in the folder next to this one, is a small window that starts it.

1. Build `examples/IdeaStatiCa-SDK-Examples.sln`.
2. Start `BimLinkExampleRunner`.
3. Select **CAD**, enter the path to Checkbot, for example
   `C:\Program Files\IDEA StatiCa\StatiCa 26.1\IdeaCheckbot.exe`, and click **Run IDEA StatiCa Checkbot**.
4. Checkbot opens and asks for a new project. Keep the EN design code and create the project.
5. Import the connection with **Bulk** or **One**. Both give the same connection of the column and the beam. Open it in IDEA StatiCa Connection to see the end plate, its four M16 bolts and the
   opening in the beam web.

The Checkbot project is kept in the folder `CadExampleCheckbotProject` next to the runner, and the next run opens it
again. Delete the folder to start over.

## Make it your own

1. Copy `BimApiLinkCadExample` and rename it, for example to `MyCadLink`. Its package references take their versions
   from `Directory.Packages.props` at the root of this repository, so add `Version="26.1.0.5973"` to each of them.
2. Replace `CadExampleApi` with the API of your application. `ICadGeometryApi` lists what the link needs from it.
3. Call `CheckbotLink.Run` from a command in your application. Give it a writable folder for the Checkbot project,
   one per model, for example next to the model file.
