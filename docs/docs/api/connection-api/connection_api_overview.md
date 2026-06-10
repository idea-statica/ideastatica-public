# Connection API

The IDEA StatiCa Connection API is a REST API (OpenAPI 3) for automating the design and code-checking of steel connections in IDEA StatiCa Connection. It is exposed by a service that runs locally on your machine and ships with the IDEA StatiCa desktop installation.

The Connection API has been available since **IDEA StatiCa 24.1**. In the current release the API version is **3.0** and all endpoints live under the `/api/3/` base path; older product versions serve older API versions (24.1 shipped API 1.0 under `/api/1/`), which is another reason the SDK package version must match the installed product version.

## API architecture

The API is served by a locally hosted REST service, `IdeaStatiCa.ConnectionRestApi.exe`, located in the IDEA StatiCa installation folder (for example `C:\Program Files\IDEA StatiCa\StatiCa 26.0`). The service communicates over HTTP, by default at `http://localhost:5000`, and is described by an OpenAPI 3 specification. A Swagger UI runs on the service itself, so you can explore and try every endpoint interactively in a browser.

Your application connects as a client, opens a project in the service, works with it through REST resources (connections, members, operations, templates, parameters, load effects, results, reports), and closes it when done. See [Concepts](connection_api_concepts.md) for the full service and project lifecycle.

## Clients

You can call the REST API directly from any programming language, but we recommend one of the official SDK clients:

| Client | Package | Requirements |
| --- | --- | --- |
| C# / .NET | NuGet: [IdeaStatiCa.ConnectionApi](https://www.nuget.org/packages/IdeaStatiCa.ConnectionApi) | .NET Standard 2.0 or 2.1 compatible runtime |
| Python | pip: [ideastatica-connection-api](https://pypi.org/project/ideastatica-connection-api/) | Python 3.8 or later |

Both clients handle connecting to the service, client identification, project lifecycle, and file upload/download for you, and add convenience wrappers on top of the generated API (for example, opening a project directly from a file path or saving a report straight to disk).

> [!IMPORTANT]
> The SDK clients are published per IDEA StatiCa version. Always install the SDK package version that matches the IDEA StatiCa version installed on the machine running the service — for example, with IDEA StatiCa 26.0 installed, use a 26.0.x package. Mixing SDK and service versions is the most common cause of unexpected errors.
<!-- TODO(editor): before publication, re-verify the latest published package versions directly on NuGet and PyPI (registries have lagged behind the repository version in the past). SDK source version at draft time: 26.0.2.0407. -->

## What you can do

The API is organized into 16 resource groups:

* **Projects** — open existing `.ideaCon` files, create empty projects, import a project from an IDEA Open Model (IOM) file, update a project from IOM, and download the modified project.
* **Connections and members** — list, create, update, copy, and delete connections; query and update members; set the bearing member; get connection topology and production cost.
* **Calculation and results** — run the CBFEM analysis and read a pass/fail summary, detailed results, or the raw check results as JSON.
* **Manufacturing operations** — list the operations in a connection, read and update their common properties, delete all operations, and pre-design welds (weld sizing).
* **Templates** — create a template from an existing connection, apply a template to another connection with parameter mapping, explode or delete applied templates, and load template defaults.
* **Connection Library** — browse design sets, let the service propose suitable design items for a connection, retrieve templates and preview pictures, and publish your own designs to a private or company set.
* **Design-code conversion** — get the default conversion mapping and convert a whole project to a different design code.
* **Materials and cross-sections** — list the steel, concrete, bolt, and weld materials, bolt assemblies, and cross-sections used in a project, and add new ones from the material and product range library (MPRL).
* **Parameters** — read and update design parameters, evaluate expressions, and delete parameters. See the [parameters getting started guide](../api_parameters_getting_started.md) and the [expression reference](../api_parameter_reference_guide.md).
* **Load effects** — create, read, update, and delete load effects, and get or set load settings such as loads in equilibrium.
* **Reports** — generate PDF or Word reports for a single connection or for multiple connections at once, and an HTML (zip) report for a single connection.
* **Exports** — export a connection to IOM, IOM connection data, or IFC.
* **3D presentation** — retrieve scene data for rendering the connection in your own viewer.
* **Settings** — read and update project-level settings.
* **Client and service info** — connect a client and query the service version.

> [!NOTE]
> The public API does not expose endpoints for adding individual plates, bolts, welds, or cuts. Connection geometry is authored by applying templates, by using the Connection Library propose workflow, or by importing a project from IOM with connection data; members can also be added and updated directly. See [Concepts](connection_api_concepts.md) for details.

## API history

* **24.1** — The REST Connection API was introduced. It replaces the older ConnHiddenCalculator API from `IdeaStatiCa.Plugin`, which is obsolete; its client code was removed from the repository (the client was deleted in February 2025, the last obsolete interface in January 2026). If you are migrating from it, start with [Getting started](connection_api_getting_started.md).
* **After 24.1** — Later releases extended the API with the Connection Library (propose and publish), design-code conversion, project settings endpoints, template sub-resources (create from connection, explode, load defaults, per-template common properties), and weld sizing pre-design.
<!-- TODO(API team): confirm the exact product version in which each post-24.1 capability shipped and expand this into per-version rows before publication. Do not guess version numbers. -->

If you are on an older version of IDEA StatiCa, we recommend updating to the latest version and the matching SDK package.

## Resources

* [Getting started](connection_api_getting_started.md) — install, run the service, and write your first script in C# or Python.
* [Concepts](connection_api_concepts.md) — service lifecycle, projects, templates, parameters, results, and how the domain model maps to REST resources.
* [C# client reference](../../../../src/api-sdks/connection-api/clients/csharp/README.md) and [C# examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples)
* [Python client reference](../../../../src/api-sdks/connection-api/clients/python/README.md) and [Python examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples)
* [OpenAPI specification](https://developer.ideastatica.com/specs/connection-api/openapi.yaml) — the machine-readable contract for the whole API ([source on GitHub](https://github.com/idea-statica/ideastatica-public/blob/main/src/api-sdks/connection-api/clients/csharp/api/openapi.yaml)). The running service also serves the spec itself, through the Swagger UI at its root URL.
* [GitHub Discussions](https://github.com/idea-statica/ideastatica-public/discussions) — questions, bug reports, and feature requests.

## AI assistants

* [LLM reference](connection_api_llm_reference.md) — a single-page, verified reference of the Python SDK surface, designed to be pasted into an AI assistant's context.
* Using ChatGPT? Try the official [IDEA StatiCa Connection API Assistant for Python](https://chatgpt.com/g/g-69ba8672efa081918789abcd0acb8d6b-idea-statica-connection-api-assistant-for-python), a custom GPT that helps you write Python scripts for the Connection API.
