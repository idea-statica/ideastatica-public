# Connection API examples

Runnable example applications and scripts live in the GitHub repository next to the SDK clients:

* C#: [src/api-sdks/connection-api/clients/csharp/examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples)
* Python: [src/api-sdks/connection-api/clients/python/examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples)

All examples need a running Connection REST service from an installed IDEA StatiCa (see [Getting started](connection_api_getting_started.md)). Where an example hardcodes an installation path (for example `C:\Program Files\IDEA StatiCa\StatiCa 25.1`), change it to the version installed on your machine — the SDK package version must match the installed product version.

## Where to start

* **Python** — [calculate_project_using_runner](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/calculate_project_using_runner): the smallest end-to-end script. Starts the service automatically (runner pattern), opens a project, calculates, and reads results.
* **C#** — [CodeSamples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples/CodeSamples): a console app with one focused sample per API area; the reference documentation embeds its snippets. For a complete application, see **ConApiWpfClient**.

## C# examples

| Example | What it shows |
| --- | --- |
| [CodeSamples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples/CodeSamples) | Console app with focused samples per API area — open and calculate a project, read results, work with templates, parameters, and reports. |
| [ConApiWpfClient](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples/ConApiWpfClient) | A complete WPF desktop client: open projects, run calculations, browse results. Ships two solutions — one against the published NuGet packages, one against SDK project references for debugging. |
| [CalculationBulkTool](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples/CalculationBulkTool) | WPF tool that batch-calculates a folder of `.ideaCon` projects. |
| [ConversionBulkTool](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples/ConversionBulkTool) | WPF tool that converts projects to a different design code in bulk. |
| [PublishBulkTool](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples/PublishBulkTool) | WPF tool that publishes connection designs to the Connection Library in bulk. |

## Python examples

| Example | What it shows |
| --- | --- |
| [calculate_project_using_runner](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/calculate_project_using_runner) | Start the service automatically with `ConnectionApiServiceRunner`, open a project, calculate, and read check results. |
| [Calculate_HSS_norm_cond](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/Calculate_HSS_norm_cond) | Open and calculate a hollow-section (HSS) connection project and evaluate the check results. |
| [apply-template](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/apply-template) | Apply a `.conTemp` connection design template to a connection, including the conversion mapping. |
| [calc-design-resistance-1](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/calc-design-resistance-1) | Calculate a sample project set up for design-resistance analysis. |
| [calc-fatigue-1](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/calc-fatigue-1) | Calculate a sample project set up for fatigue analysis. |
| [calc-fire-1](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/calc-fire-1) | Calculate a sample project set up for fire-resistance analysis. |
| [calc-stiffness-1](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/calc-stiffness-1) | Calculate a sample project set up for stiffness analysis. |
| [conversion](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/conversion) | Convert a project to a different design code: read the default conversion mapping, adjust it, and run the conversion. |
| [generate-report](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/generate-report) | Generate connection check reports and save them to disk. |
| [import-export](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/import-export) | Open a project from a file and download the modified project back to disk. |

The [projects](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples/projects) folder holds the sample `.ideaCon` projects shared by the Python examples.

## Contributing

Found a problem in an example, or built something worth sharing? Open an [issue](https://github.com/idea-statica/ideastatica-public/issues) or start a thread in [GitHub Discussions](https://github.com/idea-statica/ideastatica-public/discussions).
