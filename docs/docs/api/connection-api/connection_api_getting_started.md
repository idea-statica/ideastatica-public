# Get Started with the Connection API

This guide takes you from a clean machine to a first working script: install the client SDK, start the Connection REST API service, open a project, run the CBFEM calculation, read the check results, and save a report.

The Connection API is a REST API (OpenAPI 3) exposed by a locally hosted service that ships with the IDEA StatiCa desktop installation. This guide was written and verified against **IDEA StatiCa 26.0** (API version 3.0). The Connection API requires **IDEA StatiCa 24.1 or later**.

## Prerequisites

- **IDEA StatiCa desktop installation.** The API service executable `IdeaStatiCa.ConnectionRestApi.exe` is part of the standard installation, in the setup directory — for example `C:\Program Files\IDEA StatiCa\StatiCa 26.0`.
- **A valid IDEA StatiCa license.** Starting the API service reserves a seat on your license while the service runs, in the same way as launching the desktop application. The **Basic license type is not supported** — the service reports "This API is disabled for Basic license type" and exits. Trial and Educational licenses are supported.
  <!-- TODO (product team): confirm and complete the exact edition/seat behavior before publication. The statements above are sourced from the service startup code (seat reservation + Basic-license rejection); per-edition availability wording still needs product sign-off. -->
- **For the C# client:** a .NET project targeting .NET Standard 2.0 or later (e.g. any current .NET SDK).
- **For the Python client:** Python 3.8 or later.
  <!-- TODO (SDK team): confirm the supported Python floor. setup.py declares PYTHON_REQUIRES = ">= 3.8" but does not pass it to setup(), so the published package carries no requires-python constraint, and the generated client README says 3.7+. -->

> [!IMPORTANT]
> The client SDK version must match the version of the installed IDEA StatiCa service. Always install the SDK package whose version corresponds to your IDEA StatiCa version — for IDEA StatiCa 26.0, use a 26.0.x package. Mixing versions leads to missing endpoints or deserialization errors.

## Install the client SDK

You can call the REST API directly from any language, but we recommend one of the provided clients.

# [.Net](#tab/dotnet)

Install the [IdeaStatiCa.ConnectionApi](https://www.nuget.org/packages/IdeaStatiCa.ConnectionApi) NuGet package:

```console
dotnet add package IdeaStatiCa.ConnectionApi
```

Pick the package version that matches your installed IDEA StatiCa version (for IDEA StatiCa 26.0, a 26.0.x package).

# [Python](#tab/python)

Install the [ideastatica-connection-api](https://pypi.org/project/ideastatica-connection-api/) package from PyPI:

```console
pip install ideastatica_connection_api
```

To pin the package to your installed IDEA StatiCa version (recommended), use for example:

```console
pip install "ideastatica_connection_api==26.0.*"
```

---

## Run the API service

The Connection API service runs locally on your computer. There are two ways to get it running.

### Option A — let the SDK start the service

Both clients provide a *service runner* that starts `IdeaStatiCa.ConnectionRestApi.exe` from the IDEA StatiCa setup directory on a free port, waits until the service is ready, and stops it again when the runner is disposed.

# [.Net](#tab/dotnet)

```csharp
using IdeaStatiCa.ConnectionApi;

using (var serviceRunner = new ConnectionApiServiceRunner(@"C:\Program Files\IDEA StatiCa\StatiCa 26.0"))
using (var client = await serviceRunner.CreateApiClient())
{
    // use the client here
}
```

# [Python](#tab/python)

The Python runner is an asynchronous context manager, so it is used inside an `asyncio` coroutine:

```python
import asyncio
from ideastatica_connection_api.connection_api_service_runner import ConnectionApiServiceRunner

SETUP_DIR = r"C:\Program Files\IDEA StatiCa\StatiCa 26.0"

async def main():
    async with ConnectionApiServiceRunner(SETUP_DIR) as service_runner:
        with service_runner.create_api_client() as api_client:
            pass  # use the client here

asyncio.run(main())
```

> [!NOTE]
> The Python service runner additionally requires the `aiohttp` package, which is not installed automatically with the SDK package — install it separately with `pip install aiohttp`.

---

### Option B — attach to a manually started service

Start the service yourself from a console:

```console
cd "C:\Program Files\IDEA StatiCa\StatiCa 26.0"
IdeaStatiCa.ConnectionRestApi.exe
```

By default the service listens on port 5000. To use a different port, pass the optional `-port` argument with an equals sign:

```console
IdeaStatiCa.ConnectionRestApi.exe -port=5193
```

With the service running you can open its Swagger UI in a browser at the root URL (for example http://localhost:5000) to explore and try every endpoint interactively. Then attach a client:

# [.Net](#tab/dotnet)

```csharp
using IdeaStatiCa.ConnectionApi;

var attacher = new ConnectionApiServiceAttacher("http://localhost:5000");
using (var client = await attacher.CreateApiClient())
{
    // use the client here
}
```

# [Python](#tab/python)

```python
from ideastatica_connection_api.connection_api_service_attacher import ConnectionApiServiceAttacher

attacher = ConnectionApiServiceAttacher("http://localhost:5000")
with attacher.create_api_client() as api_client:
    pass  # use the client here
```

---

Attaching is convenient during development: you start the service once and run your script against it repeatedly without paying the service startup time.

## Your first script

The script below opens an existing `.ideaCon` project, lists its connections, runs the CBFEM calculation for all of them, prints the check results, saves a PDF report of the first connection, and saves the modified project back to disk. It assumes a service is already running on port 5000 (Option B above). Replace the project file path with one of your own projects — any `.ideaCon` file saved by IDEA StatiCa Connection works.

# [.Net](#tab/dotnet)

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using IdeaStatiCa.ConnectionApi;

namespace ConnectionApiQuickstart
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string baseUrl = "http://localhost:5000";
            const string projectFile = @"C:\Projects\my-connection-project.ideaCon";

            var attacher = new ConnectionApiServiceAttacher(baseUrl);

            // Disposing the client closes the open project on the service.
            using (var client = await attacher.CreateApiClient())
            {
                // 1. Open an existing .ideaCon project.
                var project = await client.Project.OpenProjectAsync(projectFile);
                Guid projectId = client.Project.ProjectId;
                Console.WriteLine($"Opened project {projectId}");

                // 2. List all connections in the project.
                var connections = await client.Connection.GetConnectionsAsync(projectId);
                foreach (var con in connections)
                {
                    Console.WriteLine($"Connection {con.Id}: {con.Name}");
                }

                // 3. Run the CBFEM calculation for all connections.
                var connectionIds = connections.Select(c => c.Id).ToList();
                var results = await client.Calculation.CalculateAsync(projectId, connectionIds);

                // 4. Print the check results.
                foreach (var result in results)
                {
                    Console.WriteLine($"Connection {result.Id} passed: {result.Passed}");
                    foreach (var check in result.ResultSummary)
                    {
                        if (check.Skipped)
                        {
                            continue; // this check was not calculated
                        }
                        string status = check.CheckStatus ? "OK" : "FAILED";
                        Console.WriteLine($"  {check.Name}: {check.CheckValue:F1} % [{status}]");
                    }
                }

                // 5. Save a PDF report for the first connection.
                await client.Report.SaveReportPdfAsync(projectId, connections.First().Id,
                    @"C:\Projects\connection-report.pdf");

                // 6. Save the project, including all changes made through the API.
                await client.Project.SaveProjectAsync(projectId,
                    @"C:\Projects\my-connection-project-calculated.ideaCon");
            }
        }
    }
}
```

# [Python](#tab/python)

```python
import logging

from ideastatica_connection_api.connection_api_service_attacher import ConnectionApiServiceAttacher

logging.basicConfig(level=logging.INFO)

BASE_URL = "http://localhost:5000"
PROJECT_FILE = r"C:\Projects\my-connection-project.ideaCon"

attacher = ConnectionApiServiceAttacher(BASE_URL)

# Leaving the 'with' block closes the open project on the service.
with attacher.create_api_client() as api_client:
    # 1. Open an existing .ideaCon project.
    project = api_client.project.open_project_from_filepath(PROJECT_FILE)
    project_id = api_client.project.active_project_id
    print(f"Opened project {project_id}")

    # 2. List all connections in the project.
    connections = api_client.connection.get_connections(project_id)
    for con in connections:
        print(f"Connection {con.id}: {con.name}")

    # 3. Run the CBFEM calculation for all connections.
    connection_ids = [con.id for con in connections]
    results = api_client.calculation.calculate(project_id, connection_ids)

    # 4. Print the check results.
    for result in results:
        print(f"Connection {result.id} passed: {result.passed}")
        for check in result.result_summary:
            if check.skipped or check.check_value is None:
                continue  # this check was not calculated
            status = "OK" if check.check_status else "FAILED"
            print(f"  {check.name}: {check.check_value:.1f} % [{status}]")

    # 5. Save a PDF report for the first connection.
    api_client.report.save_report_pdf(project_id, connections[0].id,
                                      r"C:\Projects\connection-report.pdf")

    # 6. Save the project, including all changes made through the API.
    api_client.project.download_project(project_id,
                                        r"C:\Projects\my-connection-project-calculated.ideaCon")
```

---

## Understanding the check results

The calculation returns one result summary per calculated connection (`ConResultSummary`):

- `passed` (`Passed` in C#) — overall pass/fail for the connection.
- `result_summary` (`ResultSummary`) — a list of individual check summaries (`CheckResSummary`), one per check type (analysis, plates, bolts, welds, ...). Each item has:
  - `name` (`Name`) — the name of the check; use this to describe the check.
  - `check_value` (`CheckValue`) — the utilization as a percentage; a value at or below 100 passes. Do not multiply by 100.
  - `check_status` (`CheckStatus`) — pass/fail of this check.
  - `unity_check_message` (`UnityCheckMessage`) — a detail message about the check.
  - `skipped` (`Skipped`) — `true` when the check was not calculated; ignore `check_value` in that case.
  - `load_case_id` (`LoadCaseId`) — the governing load case.

There is no `check_section` or `check_type` field on the check summary — use `name`.

For detailed results beyond the summary, the calculation group also provides `get_results` / `GetResultsAsync` and `get_raw_json_results` / `GetRawJsonResultsAsync` (the full raw CBFEM results as a list of JSON strings, one per connection). See [Concepts](connection_api_concepts.md) for an overview of all API groups.

## Troubleshooting

**The SDK and service versions do not match.** Symptoms include HTTP 404 on endpoints that should exist, or errors while deserializing responses. Check the installed IDEA StatiCa version against your package version (`pip show ideastatica_connection_api`, or the NuGet package version in your project file) and align them.

**The service does not start.** Run `IdeaStatiCa.ConnectionRestApi.exe` directly from a console in the setup directory and read its output. Common causes: a required .NET runtime component is missing on the machine (the console output names the missing framework), or the license check fails (for example, the Basic license type is not supported, or no seat is available). You can verify a running service by opening `http://localhost:<port>/heartbeat` in a browser.

**Port already in use.** Start the service on another port with `-port=<number>` and pass that URL to `ConnectionApiServiceAttacher`, or use `ConnectionApiServiceRunner`, which always picks a free port automatically.

**A project stays open on the service.** Projects are server-side state. The clients close the active project automatically — in Python when the `with` block exits, in C# when the client is disposed. If your script crashed without cleanup, restart the service or call `close_project` / `CloseProjectAsync` explicitly.

## Next steps

- [Connection API overview](connection_api_overview.md) — architecture, capability map, and API history.
- [Concepts](connection_api_concepts.md) — projects, connections, operations, templates, parameters, load effects, results, and reports.
- [Getting started with Parameters](../api_parameters_getting_started.md) and the [Expression Parameter Reference Guide](../api_parameter_reference_guide.md) — drive parametric connection design through the API.
- [AI / LLM reference](connection_api_llm_reference.md) — a single-page SDK reference designed to be pasted into an AI assistant's context.
- [OpenAPI specification](https://developer.ideastatica.com/specs/connection-api/openapi.yaml) — the machine-readable contract for all REST endpoints, for calling the API without an SDK.
- Client reference and examples on GitHub: [C# client](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp) ([examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples), including the **ConApiWpfClient** desktop app and **CodeSamples**) and [Python client](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python) ([examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples)).
- Questions, bug reports, or missing functionality: [GitHub Discussions](https://github.com/idea-statica/ideastatica-public/discussions).
