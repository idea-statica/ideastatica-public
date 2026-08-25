# CalculationBulkTool

## What it does

A WPF desktop tool for batch CBFEM calculation of IDEA StatiCa Connection projects. You select an IDEA StatiCa installation folder and a folder containing `.ideaCon` files (searched recursively, including subfolders). The tool first loads the list of connections from every project, then calculates each connection one by one and exports the results to a CSV file (`Export-<timestamp>.csv`) in the selected folder. For each passed connection it records the result summary values (Analysis, Bolts, Welds, Plates, Preloaded bolts) and detailed bolt results parsed from the raw JSON results (utilization in tension, shear, and interaction). Connections that fail to calculate are listed in `FailedProjects-<timestamp>.txt`. Progress and errors are logged via Serilog.

The Connection API service is started automatically from the selected installation folder using `ConnectionApiServiceRunner` — no manually started service is needed.

## Prerequisites

- Windows with IDEA StatiCa installed (the SDK version must match the installed product version; current product version is IDEA StatiCa 26.0). The default installation path offered by the app may point to an older version — use **Set Idea StatiCa API Path** to select your installed version folder, e.g. `C:\Program Files\IDEA StatiCa\StatiCa 26.0`.
- .NET 10 SDK (the project targets `net10.0-windows`).

## Build & run

Build with the `Debug` or `Release` configuration — these reference the `IdeaStatiCa.ConnectionApi` project from this repository:

```console
dotnet build CalculationBulkTool.csproj -c Debug
dotnet run --project CalculationBulkTool.csproj -c Debug
```

In the app: set the IDEA StatiCa path, select the folder with `.ideaCon` files, load the project items, then start the calculation. The CSV export is written to the same folder.

## Key API calls used

- `ConnectionApiServiceRunner(ideaPath)` + `runner.CreateApiClient()` — start the service and create the client
- `conClient.Project.OpenProjectAsync(filePath)` / `conClient.Project.CloseProjectAsync(projectId)`
- `conClient.Calculation.CalculateAsync(projectId, connectionIds)` — CBFEM calculation, returns result summaries
- `conClient.Calculation.GetRawJsonResultsAsync(projectId, conCalculationParameter)` — raw JSON results used to extract per-bolt checks
