# PublishBulkTool

## What it does

A WPF desktop tool for bulk publishing of connection designs to the IDEA StatiCa Connection Library. You select an IDEA StatiCa installation folder and a folder containing `.ideaCon` files (searched recursively). The tool first loads the list of connections from every project, then publishes each connection to the Connection Library under the connection's name, into either a **Private set** or a **Company** design set (selected in the UI via `ConTemplatePublishParam.DesignSetType`). Each project in the list is marked with a success or failure indicator. The Connection API service is started automatically via `ConnectionApiServiceRunner`.

## Prerequisites

- Windows with IDEA StatiCa installed (the SDK version must match the installed product version; current product version is IDEA StatiCa 26.0). The default installation path offered by the app may point to an older version — use **Set Idea StatiCa API Path** to select your installed version folder, e.g. `C:\Program Files\IDEA StatiCa\StatiCa 26.0`.
- .NET 10 SDK (the project targets `net10.0-windows`).

## Build & run

The project has four configurations (see `PublishBulkTool.csproj`):

- `Debug`, `Release` — project reference to `IdeaStatiCa.ConnectionApi` from this repository
- `Debug_NuGet`, `Release_NuGet` — `IdeaStatiCa.ConnectionApi` NuGet package

```console
dotnet run --project PublishBulkTool.csproj -c Debug_NuGet
```

In the app: set the IDEA StatiCa path, select the folder with `.ideaCon` files, load the project items, choose the design set type, and press the publish button.

## Key API calls used

- `ConnectionApiServiceRunner(ideaPath)` + `service.CreateApiClient()` — start the service and create the client
- `conClient.Project.OpenProjectAsync(filePath)` / `conClient.Project.CloseProjectAsync(projectId)`
- `conClient.ConnectionLibrary.PublishConnectionAsync(projectId, connectionId, publishParams)` — publish a connection to the Connection Library
