# Connection API concepts

This page explains the core concepts behind the IDEA StatiCa Connection API: how the service and client lifecycle works, how a Connection project maps to REST resources, and how the main workflows (templates, parameters, load effects, calculation, results, and reports) fit together.

The Connection API requires IDEA StatiCa 24.1 or later. The SDK clients ship per product version — always install the `IdeaStatiCa.ConnectionApi` (NuGet) or `ideastatica_connection_api` (pip) package version that matches your installed IDEA StatiCa version. The examples on this page were verified against IDEA StatiCa 26.0.

If you have not run the service and made your first call yet, start with [Getting started](connection_api_getting_started.md). For what the API is and what it can do at a glance, see the [overview](connection_api_overview.md).

> [!NOTE]
> All REST paths on this page are relative to the API base path `/api/3`. The tables list the Python SDK accessor (`api_client.<group>.<method>`) and the C# SDK accessor (`conClient.<Group>.<Method>`) for each endpoint; both call the same REST operation.

## Service and client lifecycle

The Connection API is exposed by a locally running service, `IdeaStatiCa.ConnectionRestApi.exe`, which ships with the desktop installation (for example `C:\Program Files\IDEA StatiCa\StatiCa 26.0`). Your application talks to it over HTTP. There are two ways to get a connected client:

- **Attacher** — you start (and own) the service yourself, then attach to its URL. `ConnectionApiServiceAttacher("http://localhost:5000").CreateApiClient()` in C#, `ConnectionApiServiceAttacher(base_url).create_api_client()` in Python. Use this when one long-running service is shared by several scripts, or when you want to watch the service console while developing.
- **Runner** — the SDK starts the service for you from the installation directory, on a free port, and waits until it responds on its `/heartbeat` endpoint. `new ConnectionApiServiceRunner(setupDir)` in C#; in Python, `ConnectionApiServiceRunner(setup_dir)` is an async context manager. The runner stops the service process again when it is disposed.

When a client is created, the SDK calls `GET /clients/connect-client` to register with the service and receive a unique client id. From then on the SDK sends this id automatically in the `ClientId` HTTP header of every request — you never handle it yourself. `GET /clients/idea-service-version` returns the version of the running service; in C# it is available as `conClient.ClientApi.GetVersionAsync()` (in Python, `ClientApi` is used internally during connect; instantiate `ClientApi` directly if you need the service version).

The most important lifecycle rule: **a project opened through the API is server-side state**. The service holds it in memory and identifies it by a project id (a GUID) that almost every endpoint takes as its first argument. The state lives until you close the project (`GET /projects/{projectId}/close`) or the client is disposed — both SDK clients close the active project automatically (the Python client in the `with` block's `__exit__`, the C# client in `Dispose`/`DisposeAsync`). In the OpenAPI specification, endpoints that need an open project are marked with the `x-requiresProject` extension flag.

A minimal, complete lifecycle in both languages — attach, connect, open, list connections, clean up:

# [Python](#tab/python)

```python
import logging
import ideastatica_connection_api
import ideastatica_connection_api.connection_api_service_attacher as connection_api_service_attacher

logging.basicConfig(level=logging.INFO)

base_url = "http://localhost:5000"
project_file = r"C:\projects\my-connection.ideaCon"

attacher = connection_api_service_attacher.ConnectionApiServiceAttacher(base_url)

with attacher.create_api_client() as api_client:
    project = api_client.project.open_project_from_filepath(project_file)
    project_id = api_client.project.active_project_id

    connections = api_client.connection.get_connections(project_id)
    for con in connections:
        print(f"{con.id}: {con.name}")

# the active project is closed automatically when the with block exits
```

# [C#](#tab/csharp)

```csharp
using System;
using System.Threading.Tasks;
using IdeaStatiCa.ConnectionApi;

namespace ConnectionApiConcepts
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			string baseUrl = "http://localhost:5000";
			string projectFile = @"C:\projects\my-connection.ideaCon";

			var serviceFactory = new ConnectionApiServiceAttacher(baseUrl);

			using (var conClient = await serviceFactory.CreateApiClient())
			{
				var project = await conClient.Project.OpenProjectAsync(projectFile);
				Guid projectId = conClient.Project.ProjectId;

				var connections = await conClient.Connection.GetConnectionsAsync(projectId);
				foreach (var con in connections)
				{
					Console.WriteLine($"{con.Id}: {con.Name}");
				}

				// the active project is closed when the client is disposed
			}
		}
	}
}
```

---

## Projects

A project is the same thing you save as an `.ideaCon` file in IDEA StatiCa Connection: one or more connection design items plus shared materials, settings, and parameters. There are three ways to get a project into the service:

1. **Open an existing `.ideaCon` file** — the most common route.
2. **Create an empty project** — the C# extension `CreateProjectAsync(designCode)` takes a design-code string (for example `"ECEN"`, `"American"`, `"AUS"`); Python uses the generated `create_empty_project`.
3. **Import an IDEA Open Model (IOM) file** — see [Working with IOM](#working-with-iom) below.

After you open or create a project, keep its id: in Python it is stored for you in `api_client.project.active_project_id`; in C# in `conClient.Project.ProjectId`. To get your changes back out of the service, download the project — the file includes everything the API modified.

| Endpoint | Python | C# |
|---|---|---|
| `POST /projects/open` | `project.open_project_from_filepath(path)` (for `.ideaCon`) | `Project.OpenProjectAsync(filePath)` |
| `POST /projects` (create empty) | `project.create_empty_project(...)` | `Project.CreateProjectAsync("ECEN")` |
| `POST /projects/import-iom-file` | `project.open_project_from_filepath(path)` (for `.xml`/`.iom`) or `project.import_iom(...)` | `Project.CreateProjectFromIomFileAsync(iomFilePath, connectionsToCreate)` |
| `POST /projects/{projectId}/update-iom-file` | `project.update_from_iom(...)` | `Project.UpdateProjectFromIomFileAsync(projectId, iomFilePath)` |
| `GET /projects/{projectId}/download` | `project.download_project(projectId, fileName)` (note: camelCase parameter names, unlike the rest of the SDK) | `Project.SaveProjectAsync(projectId, fileName)` |
| `GET /projects` | `project.get_active_projects()` | `Project.GetActiveProjectsAsync()` |
| `GET /projects/{projectId}` | `project.get_project_data(...)` | `Project.GetProjectDataAsync(...)` |
| `PUT /projects/{projectId}` | `project.update_project_data(...)` | `Project.UpdateProjectDataAsync(...)` |
| `GET /projects/{projectId}/close` | `project.close_project(project_id)` | `Project.CloseProjectAsync(projectId)` |

## Connections and members

A project contains one or more **connections** (design items). Each connection has an integer id, a name, an analysis type, and a calculated flag, and is the target of nearly all other resources — operations, templates, parameters, load effects, results, and reports are all addressed as `/projects/{projectId}/connections/{connectionId}/...`.

A connection is built from **members** — the beams and columns that meet in the joint. One member is the *bearing member*, the one the others connect to; you can change which member that is. Members carry the cross-section and position data that manufacturing operations attach to.

| Endpoint | Python | C# |
|---|---|---|
| `GET /projects/{projectId}/connections` | `connection.get_connections(project_id)` | `Connection.GetConnectionsAsync(projectId)` |
| `POST /projects/{projectId}/connections` | `connection.create_empty_connection(...)` | `Connection.CreateEmptyConnectionAsync(...)` |
| `GET .../connections/{connectionId}` | `connection.get_connection(...)` | `Connection.GetConnectionAsync(...)` |
| `PUT .../connections/{connectionId}` | `connection.update_connection(...)` | `Connection.UpdateConnectionAsync(...)` |
| `DELETE .../connections/{connectionId}` | `connection.delete_connection(...)` | `Connection.DeleteConnectionAsync(...)` |
| `POST .../connections/{connectionId}/copy` | `connection.copy_connection(...)` | `Connection.CopyConnectionAsync(...)` |
| `GET .../connections/{connectionId}/production-cost` | `connection.get_production_cost(...)` | `Connection.GetProductionCostAsync(...)` |
| `GET .../connections/{connectionId}/get-topology` | `connection.get_connection_topology(...)` | `Connection.GetConnectionTopologyAsync(...)` |
| `GET .../connections/{connectionId}/members` | `member.get_members(...)` | `Member.GetMembersAsync(...)` |
| `GET .../members/{memberId}` | `member.get_member(...)` | `Member.GetMemberAsync(...)` |
| `POST .../connections/{connectionId}/members` | `member.add_member(...)` | `Member.AddMemberAsync(...)` |
| `PUT .../connections/{connectionId}/members` | `member.update_member(...)` | `Member.UpdateMemberAsync(...)` |
| `PUT .../members/{memberId}/set-bearing-member` | `member.set_bearing_member(...)` | `Member.SetBearingMemberAsync(...)` |

## Manufacturing operations — read this first

Manufacturing operations (plates, bolt grids, welds, cuts, stiffeners, and so on) are what turn bare members into a designed connection. Before you plan an automation workflow, understand what the public API does and does not let you do with them:

- You can **list** the operations in a connection, **delete all** of them, read and update their **common properties**, and run **weld sizing** (`pre_design_welds` / `PreDesignWeldsAsync`, with a `ConWeldSizingMethodEnum` method: `fullStrength`, `minimumDuctility`, `overStrengthFactor`, or `capacityEstimation`).
- The public API does **not** expose endpoints for adding or editing individual plates, bolts, welds, or cuts. You author connection geometry by **applying templates** (see [Templates](#templates)) or by letting the **Connection Library** propose a design (see [Connection Library](#connection-library)) — and you parametrize the result with [Parameters](#parameters).

| Endpoint | Python | C# |
|---|---|---|
| `GET .../connections/{connectionId}/operations` | `operation.get_operations(...)` | `Operation.GetOperationsAsync(...)` |
| `DELETE .../connections/{connectionId}/operations` | `operation.delete_operations(...)` | `Operation.DeleteOperationsAsync(...)` |
| `GET .../operations/common-properties` | `operation.get_common_operation_properties(...)` | `Operation.GetCommonOperationPropertiesAsync(...)` |
| `PUT .../operations/common-properties` | `operation.update_common_operation_properties(...)` | `Operation.UpdateCommonOperationPropertiesAsync(...)` |
| `POST .../operations/weld-sizing` | `operation.pre_design_welds(...)` | `Operation.PreDesignWeldsAsync(...)` |

## Templates

A connection template captures the full design of a connection — its operations, parameters, and their links — so it can be reapplied to a different connection with compatible topology. Templates are the primary way to author geometry through the API.

There are two ways to **create** a template from a connection:

- `GET .../connections/{connectionId}/get-template` (`template.create_con_template` / `Template.CreateConTemplateAsync`) returns the raw template content — the same format as a `.contemp` file.
- `POST .../connections/{connectionId}/templates/create-from-connection` (`template.create_template_from_connection` / `Template.CreateTemplateFromConnectionAsync`) creates a reusable template with structured metadata: design code, manufacturing type, member typology, and operation and parameter counts.

**Applying** a template is always a two-step flow:

1. `get_default_template_mapping` — pass the template content (in Python wrapped in `ConTemplateMappingGetParam(template=...)`, optionally with `member_ids`); the service returns a `TemplateConversions` object describing how template entities map to your connection.
2. `apply_template` — pass `ConTemplateApplyParam(connection_template=..., mapping=...)` with the (optionally edited) mapping.

The `TemplateConversions.conversions` field is a **flat list** of conversion items; there is no nested grouping. Material-type items carry a category label in `description` — `"Steel"` (plate material), `"Concrete"`, `"Weld"`, `"Bolt Assembly"` (note the capitalization) — while cross-section and member items carry the member or operation name from the template as their `description`, with the cross-section name in `original_value`. These labels are localized strings (the values shown apply to a service running in English), so prefer matching on `original_value`/`original_template_id` over exact-matching `description`. Customize the mapping by setting `new_value` on the items you want to change.

In C#, the extension `Template.ImportTemplateFromFile(fileName)` reads a `.contemp` file from disk straight into a ready-to-use `ConTemplateMappingGetParam`.

Templates applied to a connection remain associated with it and can be listed and managed. Two cleanup operations are easy to confuse:

- **Explode** keeps the operations the template created but removes the template's parameters and parametric links — the geometry stays, the parametrization is gone.
- **Delete** removes everything the template brought in.

| Endpoint | Python | C# |
|---|---|---|
| `GET .../connections/{connectionId}/get-template` | `template.create_con_template(...)` | `Template.CreateConTemplateAsync(...)` |
| `POST .../templates/create-from-connection` | `template.create_template_from_connection(...)` | `Template.CreateTemplateFromConnectionAsync(...)` |
| `POST .../connections/{connectionId}/get-default-mapping` | `template.get_default_template_mapping(...)` | `Template.GetDefaultTemplateMappingAsync(...)` |
| `POST .../connections/{connectionId}/apply-template` | `template.apply_template(...)` | `Template.ApplyTemplateAsync(...)` |
| `GET .../connections/{connectionId}/templates` | `template.get_templates_in_connection(...)` | `Template.GetTemplatesInConnectionAsync(...)` |
| `GET .../templates/{templateInstanceId}` | `template.get_template_in_connection(...)` | `Template.GetTemplateInConnectionAsync(...)` |
| `DELETE .../connections/{connectionId}/templates` | `template.delete_all(...)` | `Template.DeleteAllAsync(...)` |
| `DELETE .../templates/{templateId}` | `template.delete(...)` | `Template.DeleteAsync(...)` |
| `POST .../templates/explode` | `template.explode_all(...)` | `Template.ExplodeAllAsync(...)` |
| `POST .../templates/{templateId}/explode` | `template.explode(...)` | `Template.ExplodeAsync(...)` |
| `GET/PUT .../templates/{templateId}/common-properties` | `template.get_template_common_operation_properties(...)` / `template.update_template_common_operation_properties(...)` | `Template.GetTemplateCommonOperationPropertiesAsync(...)` / `Template.UpdateTemplateCommonOperationPropertiesAsync(...)` |
| `POST .../templates/{templateId}/load-defaults` | `template.load_defaults(...)` | `Template.LoadDefaultsAsync(...)` |

## Connection Library

The Connection Library is the set of predefined, company, and personal connection designs known from the desktop application. Through the API you can browse it, ask it to **propose** suitable design items for a given connection (based on its topology and design code), download a design item's template and picture, and **publish** a finished connection back to your Private or Company set as a new template (`ConTemplatePublishParam`).

> [!IMPORTANT]
> `connection_library.get_template` / `ConnectionLibrary.GetTemplateAsync` returns the template **BASE64-encoded**. Decode it to XML before passing it to `get_default_template_mapping` or `apply_template`.

| Endpoint | Python | C# |
|---|---|---|
| `GET /connection-library/get-design-sets` | `connection_library.get_design_sets()` | `ConnectionLibrary.GetDesignSetsAsync()` |
| `GET /connection-library/get-template` | `connection_library.get_template(design_set_id, design_item_id)` | `ConnectionLibrary.GetTemplateAsync(...)` |
| `GET /connection-library/get-picture` | `connection_library.get_design_item_picture(...)` or `connection_library.save_design_item_picture(design_set_id, design_item_id, file_name)` | `ConnectionLibrary.GetDesignItemPictureDataAsync(...)` or `ConnectionLibrary.SaveDesignItemPictureAsync(...)` |
| `POST .../connections/{connectionId}/propose` | `connection_library.propose(project_id, connection_id, search_params)` | `ConnectionLibrary.ProposeAsync(...)` |
| `POST .../connections/{connectionId}/publish` | `connection_library.publish_connection(...)` | `ConnectionLibrary.PublishConnectionAsync(...)` |

The following complete script shows the whole propose-and-apply pipeline end to end:

```python
import base64
import logging
import ideastatica_connection_api
import ideastatica_connection_api.connection_api_service_attacher as connection_api_service_attacher
from ideastatica_connection_api import (
    ConConnectionLibrarySearchParameters,
    ConTemplateMappingGetParam,
    ConTemplateApplyParam,
)

logging.basicConfig(level=logging.INFO)

base_url = "http://localhost:5000"
project_file = r"C:\projects\my-connection.ideaCon"

attacher = connection_api_service_attacher.ConnectionApiServiceAttacher(base_url)

with attacher.create_api_client() as api_client:
    api_client.project.open_project_from_filepath(project_file)
    project_id = api_client.project.active_project_id

    connection = api_client.connection.get_connections(project_id)[0]

    # 1. Ask the Connection Library to propose design items for this topology
    search = ConConnectionLibrarySearchParameters(in_predefined_set=True)
    proposals = api_client.connection_library.propose(project_id, connection.id, search)
    if not proposals:
        raise RuntimeError("No design items proposed for this connection topology.")
    item = proposals[0]

    # 2. Download the template - the response is BASE64-encoded, decode it to XML
    template_b64 = api_client.connection_library.get_template(
        design_set_id=item.con_design_set_id,
        design_item_id=item.con_design_item_id)
    template_xml = base64.b64decode(template_b64).decode("utf-8")

    # 3. Get the default mapping of the template onto this connection
    mapping = api_client.template.get_default_template_mapping(
        project_id, connection.id,
        ConTemplateMappingGetParam(template=template_xml))

    # 4. Optionally edit the mapping, then apply the template
    apply_param = ConTemplateApplyParam(connection_template=template_xml, mapping=mapping)
    api_client.template.apply_template(project_id, connection.id, apply_param)

    # 5. Calculate and print the outcome
    results = api_client.calculation.calculate(project_id, [connection.id])
    for res in results:
        print(f"Connection {res.id} passed: {res.passed}")
```

## Design code conversion

A whole project can be converted from one design code to another. The flow mirrors the template mapping flow: ask for the default mapping, edit it, then change the code. `get_conversion_mapping(project_id, country_code)` returns a `ConConversionSettings` object with the target design code and per-category mapping lists — `concrete`, `cross_sections`, `fasteners`, `steel`, `welds`, `bolt_grade` — where each `ConversionMapping` pairs a `source_value` with a `target_value`. Adjust the target values you care about and pass the settings to `change_code`.

> [!NOTE]
> Per the API reference, only projects in the ECEN (Eurocode) design code are supported as the conversion source.

| Endpoint | Python | C# |
|---|---|---|
| `GET /projects/{projectId}/get-default-mapping` | `conversion.get_conversion_mapping(project_id, country_code)` | `Conversion.GetConversionMappingAsync(...)` |
| `POST /projects/{projectId}/change-code` | `conversion.change_code(project_id, settings)` | `Conversion.ChangeCodeAsync(...)` |

## Parameters

Parameters make a connection design parametric: named values (plate thicknesses, bolt counts, offsets...) that operations reference through expressions. The API reads and writes them per connection. Updates are a list of `IdeaParameterUpdate(key=..., expression=...)` items — and the **expression is always a string**, even for numbers: write `expression="15"`, not `expression=15`. The evaluate-expression endpoint lets you test an expression against a connection without changing anything.

For the expression syntax and how to set up parametric designs, see [Getting started with Parameters](../api_parameters_getting_started.md) and the [Expression Parameter Reference Guide](../api_parameter_reference_guide.md).

| Endpoint | Python | C# |
|---|---|---|
| `GET .../connections/{connectionId}/parameters` | `parameter.get_parameters(...)` | `Parameter.GetParametersAsync(...)` |
| `PUT .../connections/{connectionId}/parameters` | `parameter.update(project_id, connection_id, [IdeaParameterUpdate(...)])` | `Parameter.UpdateAsync(...)` |
| `DELETE .../connections/{connectionId}/parameters` | `parameter.delete_parameters(...)` | `Parameter.DeleteParametersAsync(...)` |
| `POST .../connections/{connectionId}/evaluate-expression` | `parameter.evaluate_expression(project_id, connection_id, body)` | `Parameter.EvaluateExpressionAsync(...)` |

## Load effects

Load effects are the internal forces (N, Vy, Vz, Mx, My, Mz) applied to the members of a connection — what the CBFEM analysis actually loads the model with. The API offers full CRUD on a connection's load effects plus project-wide load settings: `ConLoadSettings` carries `loads_in_equilibrium` and `loads_in_percentage` flags, matching the corresponding options in the desktop application.

| Endpoint | Python | C# |
|---|---|---|
| `GET .../connections/{connectionId}/load-effects` | `load_effect.get_load_effects(...)` | `LoadEffect.GetLoadEffectsAsync(...)` |
| `GET .../load-effects/{loadEffectId}` | `load_effect.get_load_effect(...)` | `LoadEffect.GetLoadEffectAsync(...)` |
| `POST .../connections/{connectionId}/load-effects` | `load_effect.add_load_effect(...)` | `LoadEffect.AddLoadEffectAsync(...)` |
| `PUT .../connections/{connectionId}/load-effects` | `load_effect.update_load_effect(...)` | `LoadEffect.UpdateLoadEffectAsync(...)` |
| `DELETE .../load-effects/{loadEffectId}` | `load_effect.delete_load_effect(...)` | `LoadEffect.DeleteLoadEffectAsync(...)` |
| `GET .../load-effects/get-load-settings` | `load_effect.get_load_settings(...)` | `LoadEffect.GetLoadSettingsAsync(...)` |
| `POST .../load-effects/set-load-settings` | `load_effect.set_load_settings(...)` | `LoadEffect.SetLoadSettingsAsync(...)` |

## Materials and cross-sections

Material endpoints follow one simple convention: **GET lists the items already used in the project; POST adds a new item from the MPRL** (the material and product range library that ships with IDEA StatiCa). Eight groups are available: all materials (GET only), steel, concrete, bolt grades, welding materials, headed stud grades, cross-sections, and bolt assemblies.

| Endpoint (under `/projects/{projectId}/materials`) | Python | C# |
|---|---|---|
| `GET /materials` | `material.get_all_materials(...)` | `Material.GetAllMaterialsAsync(...)` |
| `GET` / `POST /materials/steel` | `material.get_steel_materials` / `material.add_material_steel` | `Material.GetSteelMaterialsAsync` / `Material.AddMaterialSteelAsync` |
| `GET` / `POST /materials/concrete` | `material.get_concrete_materials` / `material.add_material_concrete` | `Material.GetConcreteMaterialsAsync` / `Material.AddMaterialConcreteAsync` |
| `GET` / `POST /materials/bolt-grade` | `material.get_bolt_grade_materials` / `material.add_material_bolt_grade` | `Material.GetBoltGradeMaterialsAsync` / `Material.AddMaterialBoltGradeAsync` |
| `GET` / `POST /materials/welding` | `material.get_welding_materials` / `material.add_material_weld` | `Material.GetWeldingMaterialsAsync` / `Material.AddMaterialWeldAsync` |
| `GET` / `POST /materials/headed-stud-grade` | `material.get_headed_stud_grade_materials` / `material.add_material_headed_stud_grade` | `Material.GetHeadedStudGradeMaterialsAsync` / `Material.AddMaterialHeadedStudGradeAsync` |
| `GET` / `POST /materials/cross-sections` | `material.get_cross_sections` / `material.add_cross_section` | `Material.GetCrossSectionsAsync` / `Material.AddCrossSectionAsync` |
| `GET` / `POST /materials/bolt-assemblies` | `material.get_bolt_assemblies` / `material.add_bolt_assembly` | `Material.GetBoltAssembliesAsync` / `Material.AddBoltAssemblyAsync` |

## Calculation and results

The calculate endpoint runs the CBFEM (Component-Based Finite Element Method) analysis — the same solver as the desktop application — for the connection ids you pass, and returns one `ConResultSummary` per connection. Results come at three levels of detail:

1. **Summary** (`calculate`): each `ConResultSummary` has an `id`, an overall `passed` flag, and a `result_summary` list. Each item in that list has `name` (the check description, for example plates, bolts, welds), `check_value` (the utilization as a percentage — the utilization ratio multiplied by 100, so a bolt at 85 % utilization returns `check_value = 85.0`), `check_status` (bool), `unity_check_message`, `load_case_id`, and `skipped` (skip these entries when reporting). Use the boolean `check_status` (and the overall `passed` flag) for pass/fail decisions rather than comparing `check_value` to a threshold: the value is meaningless when `skipped` is true, so always gate on `skipped` first (the field is also declared optional in the generated models, so defensive code should tolerate `None`). There is **no** `check_section` or `check_type` field — use `name`.
2. **Detailed** (`get_results`): the detailed check results of the CBFEM analysis.
3. **Raw JSON** (`get_raw_json_results`): the serialized `CheckResultsData` structure as a JSON string — parse it with a JSON library when you need every number the solver produced.

| Endpoint | Python | C# |
|---|---|---|
| `POST /projects/{projectId}/connections/calculate` | `calculation.calculate(project_id, connection_ids)` | `Calculation.CalculateAsync(projectId, requestBody)` |
| `POST /projects/{projectId}/connections/results` | `calculation.get_results(...)` | `Calculation.GetResultsAsync(...)` |
| `POST /projects/{projectId}/connections/rawresults-text` | `calculation.get_raw_json_results(...)` | `Calculation.GetRawJsonResultsAsync(...)` |

Project settings are read and written as a dictionary of setting keys and values via the Settings endpoints: `GET`/`PUT /projects/{projectId}/settings` — `settings.get_settings(project_id, search=None)` / `settings.update_settings` in Python, `Settings.GetSettingsAsync` / `Settings.UpdateSettingsAsync` in C#. The optional `search` argument filters the returned settings by keyword.

## Reports and exports

Reports come in three formats — PDF, Word, and a zipped HTML package — per connection, plus multi-connection PDF and Word reports for a whole list of connection ids in one document. The SDK extension wrappers save them straight to a file, which is almost always what you want.

> [!NOTE]
> The multi-connection PDF operation id contains a typo, `GeneratePdfForMutliple`, which is propagated as-is into the generated SDK method names (`generate_pdf_for_mutliple`, `GeneratePdfForMutlipleAsync`). It is shipped that way; the extension wrappers (`save_multiple_report_pdf` / `SaveMultipleReportsPdfAsync`) spell it out for you.

| Endpoint | Python | C# |
|---|---|---|
| `GET .../connections/{connectionId}/reports/pdf` | `report.save_report_pdf(project_id, connection_id, file_name)` | `Report.SaveReportPdfAsync(projectId, connectionId, filePath)` |
| `GET .../connections/{connectionId}/reports/word` | `report.save_report_word(...)` | `Report.SaveReportWordAsync(...)` |
| `GET .../connections/{connectionId}/reports/htmlZip` | `report.generate_html_zip(...)` | `Report.GenerateHtmlZipAsync(...)` |
| `POST /projects/{projectId}/reports/pdf` (multi) | `report.save_multiple_report_pdf(project_id, connection_ids, file_name)` | `Report.SaveMultipleReportsPdfAsync(projectId, connectionIds, filePath)` |
| `POST /projects/{projectId}/reports/word` (multi) | `report.save_multiple_report_word(...)` | `Report.SaveMultipleReportsWordAsync(...)` |

Exports take a single connection out of the project in a machine-readable form:

| Endpoint | Python | C# |
|---|---|---|
| `GET .../connections/{connectionId}/export-iom` | `export.export_iom(...)` | `Export.ExportIomAsync(...)` |
| `GET .../connections/{connectionId}/export-iom-connection-data` | `export.export_iom_connection_data(...)` | `Export.ExportIomConnectionDataAsync(...)` |
| `GET .../connections/{connectionId}/export-ifc` | `export.export_ifc_file(project_id, connection_id, file_name)` | `Export.ExportIfcFileAsync(projectId, connectionId, filePath)` |

For custom visualization, the Presentation endpoints return the data behind the 3D scene: `GET .../connections/{connectionId}/presentations` (`presentation.get_data_scene3_d` / `Presentation.GetDataScene3DAsync`) and `GET .../presentations/text` for the same data serialized as JSON (`presentation.get_data_scene3_d_text` / `Presentation.GetDataScene3DTextAsync`).

## Working with IOM

IDEA Open Model (IOM) is the open data format for describing a structural model — geometry, cross-sections, materials, loads, and results — independent of any source application. The Connection API accepts IOM as a second way in, next to `.ideaCon` files:

- **Open a `.ideaCon` file** when the connection already exists as an IDEA StatiCa project — typically designed in the desktop application or produced by an earlier API run.
- **Import an IOM file** when the model comes from somewhere else — your own application, an FEA or CAD tool, or a generator script. The service creates a new Connection project from the `OpenModelContainer` XML (`project.open_project_from_filepath` routes `.xml`/`.iom` files automatically in Python; in C#, use `Project.CreateProjectFromIomFileAsync`, optionally restricting which connection points become connections via `connectionsToCreate`). An already-open project can also be updated in place from a newer IOM file (`update_from_iom` / `UpdateProjectFromIomFileAsync`), for example after the model or its load results changed upstream.
- **Export IOM** when downstream tools need the connection model back in an open format (see [Reports and exports](#reports-and-exports)).

To learn the format itself, start with the [IOM documentation](../../iom/iom_getting_started.md). For a worked example that builds an IOM model in code and sends it to IDEA StatiCa, see [Creating a simple app](../api_create_a_simple_app.md).

## See also

- [Connection API overview](connection_api_overview.md) — what the API is, architecture, and version history
- [Getting started](connection_api_getting_started.md) — run the service and make your first call
- [LLM reference](connection_api_llm_reference.md) — single-file SDK reference for AI assistants
- [Getting started with Parameters](../api_parameters_getting_started.md) and the [Expression Parameter Reference Guide](../api_parameter_reference_guide.md)
- SDK reference: C# and Python client documentation under this section
