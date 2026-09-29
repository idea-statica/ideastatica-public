# PublishBulkTool

## What it does

A WPF desktop tool for bulk publishing of connection designs to the IDEA StatiCa Connection Library. You select an IDEA StatiCa installation folder and a folder containing `.ideaCon` files (searched recursively). The tool first loads the list of connections from every project, then publishes each connection to the Connection Library under the connection's name, into either a **Private set** or a **Company** design set (selected in the UI via `ConTemplatePublishParam.DesignSetType`). Each project in the list is marked with a success or failure indicator. The Connection API service is started automatically via `ConnectionApiServiceRunner`.

## Prerequisites

- Windows with IDEA StatiCa installed (the SDK version must match the installed product version). On start the app looks for an installation itself: first its own folder, then the newest `C:\Program Files\IDEA StatiCa\StatiCa *` that contains `IdeaStatiCa.ConnectionRestApi.exe`. Use **Set Idea StatiCa API Path** to override it; a folder without that executable is rejected there and then.
- .NET 10 SDK (the project targets `net10.0-windows`).

## When something goes wrong

Failures are reported in the window, not thrown at the process: the status line carries the summary,
a dialog carries the full exception, and a project that could not be read or published gets its
reason printed under its name while the rest of the batch continues.

If the service itself will not start, the message quotes its exit code and the last lines it wrote —
the service is launched as a child process and produces no Windows Event Log entry of its own, so
this is the only place that information exists. A cold start can take a while; the runner waits
`ConnectionApiServiceRunner.DefaultStartupTimeout` (120 s) and its constructor takes a longer value
for machines that need one.

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
