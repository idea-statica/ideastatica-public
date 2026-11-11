# C++ FEA link example

This example links an FEA application written in native C++ to IDEA StatiCa Checkbot. The link is written in C++/CLI
and built on the [IdeaStatiCa.BimApiLink](https://www.nuget.org/packages/IdeaStatiCa.BimApiLink) package. A small fake
FEA application, an MFC window, stands in for yours, so the example runs without any FEA software.

It has the same model and the same link as the [FEA link example](../FeaExample/README.md), which is written in C#.
The model is one corner of a steel frame: a 3 m column and a 5 m beam, both IPE200 in S 355, meeting at node 2. It has
three load cases (self-weight, dead load and snow), four combinations of them, and the internal forces of both members
for each load case.

## What it shows

- Calling a .NET link from a native C++ application, through one exported C++ function.
- Importing a connection: the user selects a node and the members that meet there.
- Converting nodes, members, cross-sections and materials to IDEA StatiCa objects.
- Importing load groups, load cases and combinations.
- Importing the internal forces of the members, which Checkbot turns into the loads of the connection.

## Projects

| Project | What it is |
|---|---|
| `CppFeaExampleApp` | The fake FEA application: a window that starts Checkbot. Native C++ with MFC. |
| `CppFeaExampleApi` | The API of the fake FEA application, in native C++. Your link uses the API of your application instead. |
| `BimApiLinkCppFeaExample` | The link, in C++/CLI. Start your own link from a copy of it. |
| `BimApiLinkCppFeaExampleBases` | A base class for each importer of the link, in C#. See [The importer base classes](#the-importer-base-classes). |

Start reading at `CheckbotLink.h` and `CheckbotLink.cpp`, then `Model.h`, then the `Importers` folder.

## How the link works

1. When the user clicks **Start Checkbot**, the application calls `StartCheckbot` with its `IFeaApi`. The function is
   plain C++ to the application, so the application needs no .NET code of its own. It returns at once, and the
   application keeps running while Checkbot is open.
2. `CheckbotLink::Run` starts Checkbot. It registers one importer for each kind of object, and `ResultsImporter` for
   the internal forces.
3. When the user imports in Checkbot, Checkbot asks `Model` what is selected. `Model` answers with identifiers only.
   Each selected node becomes a connection of the selected members that meet there.
4. Checkbot then asks for each object by its identifier. BimApiLink passes the request to the importer of that kind of
   object. The importer reads the object from the FEA application and returns an IDEA StatiCa object.
5. `Model` also gives every combination of the model. Their load cases and load groups are imported with them.
6. Last, Checkbot asks `ResultsImporter` for the internal forces of the imported members, for every load case.

Every object is identified by its FEA id. Checkbot looks up cross-sections and materials by their names in the IDEA
StatiCa library.

Some objects refer to others: a member to its cross-section, a cross-section to its material. The classes in the
`BimApi` folder keep only the id of the referenced object, and BimApiLink imports it when it reads the reference. An
importer can also get a referenced object itself with `Get<T>`, as `LoadCaseImporter` does for the load group.

Lengths are in metres. The fake application gives forces in kN and moments in kNm, and `ResultsImporter` converts them
to N and Nm. The internal forces are in each member's own axes, which `MemberImporter` passes on with the member.

### .NET in a native application

The link is a .NET 10 DLL that the application loads like any other DLL. The first call to `StartCheckbot` starts .NET
in the application. For that, the link needs these files next to it:

- `Ijwhost.dll`, `BimApiLinkCppFeaExample.runtimeconfig.json` and `BimApiLinkCppFeaExample.deps.json`, which the
  build of the link creates.
- `BimApiLinkCppFeaExampleBases.dll`, with the base classes of the importers.
- The .NET assemblies of the NuGet packages, with the `runtimes` folder and any language folders, such as `de-DE`.
  A .NET application would find them by itself; a native one does not, so the link's project copies them next to
  the link (`CopyLocalLockFileAssemblies`).

The three C++ projects build into one folder, `bin\Release` or `bin\Debug`, and the build of the link copies all of
these files there. The application runs from that folder.

### The importer base classes

The C++/CLI compiler cannot derive a class from BimApiLink's `IntIdentifierImporter<T>`, nor from an abstract C#
class derived from it: it reports error C3766 for the generic methods of `IImporter`. It can derive from a class that
is not abstract. So `BimApiLinkCppFeaExampleBases` has one such class for each kind of object, and each importer of
the link derives from one and overrides `Create`. `ResultsImporter` needs none: it implements
`IInternalForcesImporter<T>` directly.

## Requirements

- Windows with IDEA StatiCa 26.1 or later installed.
- Visual Studio 2026 with the **Desktop development with C++** workload, including its **C++/CLI support** and
  **C++ MFC** components.
- The .NET 10 SDK.

The application and its API are 64-bit native C++. The link targets .NET 10. All packages come from nuget.org.

## Run it

1. Open `CppFeaExample.slnx` in Visual Studio and build it.
2. Start `CppFeaExampleApp`. Set it as the startup project, or run `bin\Release\CppFeaExampleApp.exe`.
3. The window shows the path to Checkbot of IDEA StatiCa 26.1. Change it if your Checkbot is elsewhere, and click
   **Start Checkbot**.
4. Checkbot opens and asks for a new project. Keep the EN design code and create the project.
5. Import the connections. Checkbot imports the column, the beam and one connection where they meet. Open the
   connection in IDEA StatiCa Connection to see the loads that come from the internal forces.

The Checkbot project is kept in the folder `CppFeaExampleCheckbotProject` next to the application, and the next run
opens it again. Delete the folder to start over. The link writes its log to
`%TEMP%\IdeaStatiCa\Logs\BimApiLinkCppFeaExample.log`.

## Make it your own

1. Copy `BimApiLinkCppFeaExample` and `BimApiLinkCppFeaExampleBases` into your own solution and rename them. Their
   package references take their versions from `Directory.Packages.props` at the root of this repository, so add a
   `Version` to each of them: the package version on nuget.org that matches your IDEA StatiCa, 26.1 or later.
2. Replace `CppFeaExampleApi` with the API of your application. `IFeaGeometryApi`, `IFeaLoadsApi` and `IFeaResultsApi`
   list what the link needs from it.
3. Call `StartCheckbot` from a command in your application. Give it a writable folder for the Checkbot project, one
   per model, for example next to the model file. Keep the object you pass to it until the application ends, because
   Checkbot can ask for the model at any time.
4. Install the link with your application, with all the files listed in [.NET in a native application](#net-in-a-native-application)
   next to it. The computer needs the .NET 10 runtime.
