# ConversionBulkTool

## What it does

A WPF desktop tool ("IDEA StatiCa Connection Design Code Converter") for bulk conversion of `.ideaCon` projects to a different design code. You select an IDEA StatiCa installation folder, a folder containing `.ideaCon` files (searched recursively), and a target design code (the UI offers AISC (America), IS800 (India), CSA (Canada), AS (Australia), GB (China), HKG (Hong Kong), SP (Russia)). The tool works in two passes:

1. It opens every project and collects the default conversion mappings (steel grades, welds, bolt grades, bolt assemblies/fasteners, concrete, cross-sections) returned by the API, merged across all projects.
2. It shows the merged mappings in a **Custom Conversion** dialog where you can review and edit the target values. After confirmation it reopens each project, applies your mapping on top of the default one, changes the design code, and saves the converted file into a `Converted` subfolder as `<name>-<designCode>.ideaCon`.

Each file in the list is marked with a success or failure indicator; the error message is shown as a tooltip on failed items. The Connection API service is started automatically via `ConnectionApiServiceRunner`.

## Prerequisites

- Windows with IDEA StatiCa installed (the SDK version must match the installed product version; current product version is IDEA StatiCa 26.0). The default installation path offered by the app may point to an older version — use **Set Idea StatiCa API Path** to select your installed version folder, e.g. `C:\Program Files\IDEA StatiCa\StatiCa 26.0`.
- .NET 10 SDK (the project targets `net10.0-windows`).

## Build & run

The project has four configurations (see `ConversionBulkTool.csproj`):

- `Debug`, `Release` — project reference to `IdeaStatiCa.ConnectionApi` from this repository
- `Debug_NuGet`, `Release_NuGet` — `IdeaStatiCa.ConnectionApi` NuGet package

```console
dotnet run --project ConversionBulkTool.csproj -c Debug_NuGet
```

In the app: set the IDEA StatiCa path, select the folder with `.ideaCon` files, pick the target design code, and press **Conversion**.

## Key API calls used

- `ConnectionApiServiceRunner(ideaPath)` + `service.CreateApiClient()` — start the service and create the client
- `conClient.Project.OpenProjectAsync(filePath)` / `conClient.Project.CloseProjectAsync(projectId)`
- `conClient.Conversion.GetConversionMappingAsync(projectId, countryCode)` — get the default material/cross-section mapping for the target code
- `conClient.Conversion.ChangeCodeAsync(projectId, mapping)` — convert the project to the target design code
- `conClient.Project.SaveProjectAsync(projectId, outputPath)` — save the converted project
