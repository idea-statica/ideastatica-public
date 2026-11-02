# CodeSamples

## What it does

An interactive console application that demonstrates individual calls of the IDEA StatiCa Connection API C# SDK (`IdeaStatiCa.ConnectionApi`). On start it asks how to connect to the Connection API service — either **run and attach** (option `0`, `ConnectionApiServiceRunner` starts the service from an IDEA StatiCa installation directory) or **attach to an already running service** (option `1`, `ConnectionApiServiceAttacher`, default base URL `http://localhost:5000`). It then lists all available samples in a numbered menu and runs the one you pick.

The samples themselves live in the [`Samples/`](Samples) folder — one static method per file (opening and calculating projects, reading and updating connections, members, materials, load effects, parameters and settings, applying templates, importing/exporting IOM and IFC, generating PDF/Word/HTML reports, 3D scene data, production cost, etc.). The menu is built by reflection: `ClientExamples.GetExampleMethods()` collects every public static method that takes a single `IConnectionApiClient` parameter and returns a `Task`, so adding a new sample file automatically adds a menu entry.

Note: the `Samples/*.cs` files are embedded in the published Connection API reference documentation as code examples — keep them small and self-contained.

## Prerequisites

- Windows with IDEA StatiCa installed (the SDK package version must match the installed product version; current product version is IDEA StatiCa 26.0).
- .NET 10 SDK (the project targets `net10.0`).
- Sample input files are in [`Inputs/`](Inputs) and are copied to the output directory on build.

## Build & run

The project has four configurations (see `CodeSamples.csproj`):

| Configuration | Reference to the SDK |
| --- | --- |
| `Debug`, `Release` | Project reference to `..\..\src\IdeaStatiCa.ConnectionApi\IdeaStatiCa.ConnectionApi.csproj` (SDK built from this repository) |
| `Debug_NuGet`, `Release_NuGet` | `IdeaStatiCa.ConnectionApi` NuGet package |

```console
dotnet run --project CodeSamples.csproj -c Debug_NuGet
```

When asked for the IDEA StatiCa directory (option `0`), enter the path of your installed version, e.g. `C:\Program Files\IDEA StatiCa\StatiCa 26.0` (the default offered by the app may point to an older version).

## Key API calls used

Each sample uses different endpoints; the common pattern (e.g. in `Samples/OpenProject.cs`, `Samples/Calculate.cs`, `Samples/GeneratePdf.cs`) is:

- `conClient.Project.OpenProjectAsync(filePath)` / `conClient.Project.CloseProjectAsync(projectId)`
- `conClient.Connection.GetConnectionsAsync(projectId)`
- `conClient.Calculation.CalculateAsync(projectId, connectionIds)`
- `conClient.Report.SaveReportPdfAsync(projectId, connectionId, pdfFilePath)`

See the individual files in [`Samples/`](Samples) for the full list of demonstrated endpoints.
