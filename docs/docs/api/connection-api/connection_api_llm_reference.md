# Connection API — LLM Reference

A single-file reference for the IDEA StatiCa Connection API, written to be pasted into (or fetched by) an AI coding assistant. It covers how to connect, the full client method catalog, key data models, canonical workflows, and the mistakes assistants most often make with this SDK. It is also usable as a human quick reference.

> [!NOTE]
> Verified against SDK version 26.0.2.0407 (Python package `ideastatica-connection-api`, NuGet package `IdeaStatiCa.ConnectionApi`), REST API version 3.0, June 2026. This page is re-verified with each SDK release. If your installed IDEA StatiCa version differs, install the matching SDK version — method surfaces change between versions.

## Scope

- **Product:** IDEA StatiCa Connection (structural steel connection design, CBFEM analysis). The Connection API requires IDEA StatiCa 24.1 or later; this page describes the surface shipped with version 26.0.
- **What the API is:** a REST API (OpenAPI 3, base path `/api/3/`) exposed by a locally hosted service, `IdeaStatiCa.ConnectionRestApi.exe`, which ships with the desktop installation (worked example path: `C:\Program Files\IDEA StatiCa\StatiCa 26.0`).
- **Primary client covered here:** the Python SDK `ideastatica_connection_api` (Python 3.8+, `pip install ideastatica-connection-api`). The C# client (`IdeaStatiCa.ConnectionApi` on NuGet) exposes the same surface; its accessors and extension methods are listed in [C# client surface](#c-client-surface).
- **Escalation:** SDK bugs or missing functionality go to [GitHub Discussions](https://github.com/idea-statica/ideastatica-public/discussions). The OpenAPI specification lives in the [public repository](https://github.com/idea-statica/ideastatica-public/blob/main/src/api-sdks/connection-api/clients/csharp/api/openapi.yaml).
- **Related pages:** [Overview](connection_api_overview.md) | [Getting started](connection_api_getting_started.md) | [Concepts](connection_api_concepts.md) | [Parameters getting started](../api_parameters_getting_started.md) | [Expression parameter reference](../api_parameter_reference_guide.md)

## Core rules

1. Use only the official SDK (`ideastatica-connection-api` on PyPI, `IdeaStatiCa.ConnectionApi` on NuGet). Never invent endpoints, method names, parameters, or response fields. If something is not on this page or in the generated client, it does not exist.
2. Use the client accessors, not standalone API classes. In Python: `api_client.project`, `api_client.report`, `api_client.export`, and `api_client.connection_library` are extension wrappers with file-handling helpers; always prefer them over the raw generated equivalents.
3. After opening a project, get its id from `api_client.project.active_project_id` (Python) or `client.Project.ProjectId` (C#). Almost every other call requires this id.
4. Parameter expressions are always strings: `IdeaParameterUpdate(key="...", expression="15")`, never `expression=15`.
5. Templates returned by `connection_library.get_template(...)` are BASE64-encoded. Decode to XML (`base64.b64decode(b64).decode("utf-8")`) before passing them to `get_default_template_mapping` or `apply_template`.
6. The mapping returned by `get_default_template_mapping` is a `TemplateConversions` object whose `.conversions` member is a flat list. Material-type items carry a category label in `.description` (values include `"Steel"`, `"Concrete"`, `"Weld"`, `"Bolt Assembly"` — note the capital A — and `"Bolt grade"`); cross-section/member items instead carry the name of the item from the template (e.g. the operation or member name), not a fixed label. These strings are localized — the values listed apply to a service running in English. Filter on `.description` (or, more robustly, on `.original_value`), then set `.new_value` on the items you want to remap.
7. The public API has no endpoints for adding individual plates, bolts, welds, or cuts. Geometry and manufacturing operations are authored by applying templates, by the Connection Library propose/apply pipeline, or by importing an IOM model. Do not generate code that calls `add_plate`, `add_bolt`, `add_weld`, `add_cut`, or similar — they do not exist.
8. The SDK tracks one ACTIVE project id per client — the last one opened via `open_project_from_filepath` / `OpenProjectAsync`. The service itself can hold multiple open projects per client (`get_active_projects` returns all of them); each stays in server memory until `close_project` is called for it. On exit/disposal the SDK clients automatically close only the active project — close any others yourself.

## Connecting to the service

The service is local. Two patterns exist in both SDKs:

- **Attacher** — connect to a service you started yourself. Run `IdeaStatiCa.ConnectionRestApi.exe` from the installation directory; by default it listens on port 5000 and serves a Swagger UI at `http://localhost:5000`. A specific port can be set with the `-port=NUMBER` command-line argument.
- **Runner** — let the SDK start the service from the installation directory on a free port and wait for it to become ready (readiness is checked on `GET {base_url}/heartbeat`).

### Python boilerplate (always start from this)

```python
import logging
from ideastatica_connection_api.connection_api_service_attacher import ConnectionApiServiceAttacher

logging.basicConfig(level=logging.INFO)

BASE_URL = "http://localhost:5000"          # service started manually from the install dir
PROJECT_FILE = r"C:\projects\my-connection.ideaCon"

attacher = ConnectionApiServiceAttacher(BASE_URL)

with attacher.create_api_client() as api_client:
    try:
        project = api_client.project.open_project_from_filepath(PROJECT_FILE)
        project_id = api_client.project.active_project_id

        # YOUR CODE HERE

    except Exception as e:
        print(f"Error: {e}")
        raise
# the active project is closed automatically when the with-block exits
```

`open_project_from_filepath` accepts `.ideacon` files (opened directly) and `.xml`/`.iom` files (routed through the IOM import endpoint).

Letting the SDK start the service instead (async runner):

```python
import asyncio
from ideastatica_connection_api.connection_api_service_runner import ConnectionApiServiceRunner

SETUP_DIR = r"C:\Program Files\IDEA StatiCa\StatiCa 26.0"

async def main():
    async with ConnectionApiServiceRunner(SETUP_DIR) as runner:
        with runner.create_api_client() as api_client:
            project = api_client.project.open_project_from_filepath(r"C:\projects\my-connection.ideaCon")
            project_id = api_client.project.active_project_id
            # YOUR CODE HERE

asyncio.run(main())
```

### C# boilerplate

```csharp
using System;
using System.Threading.Tasks;
using IdeaStatiCa.ConnectionApi;

internal class Program
{
    private static async Task Main()
    {
        // Option A: attach to a running service (start IdeaStatiCa.ConnectionRestApi.exe first)
        var factory = new ConnectionApiServiceAttacher("http://localhost:5000");

        // Option B: start the service from the installation directory on a free port
        // using var runner = new ConnectionApiServiceRunner(@"C:\Program Files\IDEA StatiCa\StatiCa 26.0");
        // IConnectionApiClient client = await runner.CreateApiClient();

        await using IConnectionApiClient client = await factory.CreateApiClient();

        var project = await client.Project.OpenProjectAsync(@"C:\projects\my-connection.ideaCon");
        Guid projectId = client.Project.ProjectId;

        // YOUR CODE HERE

        await client.Project.CloseProjectAsync(projectId);
    } // DisposeAsync also closes the active project if it is still open
}
```

### Calling REST directly

From any other language: call `GET /api/3/clients/connect-client` once, send the returned id in a `ClientId` header on every subsequent request, and work against the paths in the OpenAPI spec. The SDK clients do exactly this for you.

## API surface — Python client

Accessors on `api_client` (a `ConnectionApiClient` inside a `with` block). "ext" marks hand-written extension classes that add the file-handling methods listed in their tables; everything else is the generated client.

| Accessor | Class | Notes |
|---|---|---|
| `api_client.project` | `ProjectExtApi` (ext) | also holds `active_project_id` |
| `api_client.connection` | `ConnectionApi` | |
| `api_client.member` | `MemberApi` | |
| `api_client.operation` | `OperationApi` | read/delete/common-properties/weld-sizing only |
| `api_client.template` | `TemplateApi` | |
| `api_client.connection_library` | `ConnectionLibraryExtApi` (ext) | |
| `api_client.parameter` | `ParameterApi` | |
| `api_client.load_effect` | `LoadEffectApi` | |
| `api_client.material` | `MaterialApi` | |
| `api_client.calculation` | `CalculationApi` | |
| `api_client.report` | `ReportExtApi` (ext) | |
| `api_client.export` | `ExportExtApi` (ext) | |
| `api_client.conversion` | `ConversionApi` | |
| `api_client.settings` | `SettingsApi` | |
| `api_client.presentation` | `PresentationApi` | |
| `api_client.client_id` | str | set automatically on connect |

### Project — `api_client.project`

| Method | Returns / notes |
|---|---|
| `open_project_from_filepath(file_path)` (ext, preferred) | `ConProject`; opens `.ideacon`, or imports `.xml`/`.iom`; sets `active_project_id` |
| `download_project(projectId, fileName)` (ext) | saves the current project (including API-made changes) to an `.ideaCon` file |
| `close_project(project_id)` | releases the project on the service |
| `create_empty_project(con_project_data=None)` | `ConProject`; `ConProjectData.design_code` selects the code ("ECEN", "American", "AUS"; default ECEN). Does NOT set `active_project_id` — store the returned `project_id` yourself |
| `get_active_projects()` | `List[ConProject]` opened by this client |
| `get_project_data(project_id)` | `ConProject` |
| `update_project_data(project_id, con_project_data)` | `ConProject` |
| `import_iom(container_xml_file=..., connections_to_create=None)` | `ConProject` from an `OpenModelContainer` XML (bytes); optional list of IOM connection point ids to create. Does NOT set `active_project_id` |
| `update_from_iom(project_id, container_xml_file)` | `ConProject`; updates an open project from IOM (model and results) |
| `open_project(idea_con_file=...)` | raw generated upload; prefer `open_project_from_filepath` |

### Connection — `api_client.connection`

| Method | Returns / notes |
|---|---|
| `get_connections(project_id)` | `List[ConConnection]` |
| `get_connection(project_id, connection_id)` | `ConConnection` |
| `create_empty_connection(project_id, name=None)` | `ConConnection` |
| `update_connection(project_id, connection_id, con_connection)` | `ConConnection` |
| `delete_connection(project_id, connection_id)` | `List[ConConnection]` (remaining connections) |
| `copy_connection(project_id, connection_id, name=None)` | `ConConnection` |
| `get_production_cost(project_id, connection_id)` | `ConProductionCost` |
| `get_connection_topology(project_id, connection_id)` | `str` (JSON) |

### Member — `api_client.member`

| Method | Returns / notes |
|---|---|
| `get_members(project_id, connection_id)` | `List[ConMember]` |
| `get_member(project_id, connection_id, member_id)` | `ConMember` |
| `add_member(project_id, connection_id, con_member)` | `ConMember` |
| `update_member(project_id, connection_id, con_member)` | `ConMember` |
| `set_bearing_member(project_id, connection_id, member_id)` | `ConMember` |

### Operation — `api_client.operation`

| Method | Returns / notes |
|---|---|
| `get_operations(project_id, connection_id)` | `List[ConOperation]` (read-only view: `operation_type`, `id`, `name`, `active`, `is_imported`) |
| `delete_operations(project_id, connection_id)` | deletes ALL operations in the connection |
| `get_common_operation_properties(project_id, connection_id)` | `ConOperationCommonProperties` |
| `update_common_operation_properties(project_id, connection_id, con_operation_common_properties)` | sets common weld/plate/bolt-assembly material ids for all operations |
| `pre_design_welds(project_id, connection_id, design_type=None)` | `str`; weld sizing pre-design; `design_type` is a `ConWeldSizingMethodEnum` (`fullStrength` default, `minimumDuctility`, `overStrengthFactor`, `capacityEstimation`) |

There are no methods to create or edit individual operations (see Core rule 7).

### Template — `api_client.template`

| Method | Returns / notes |
|---|---|
| `create_con_template(project_id, connection_id)` | `str` — template (.contemp content) created from the connection |
| `create_template_from_connection(project_id, connection_id)` | `ConTemplateCreateResult` — template plus structured metadata |
| `get_default_template_mapping(project_id, connection_id, con_template_mapping_get_param)` | `TemplateConversions`; pass `ConTemplateMappingGetParam(template=xml, member_ids=[...])` |
| `apply_template(project_id, connection_id, con_template_apply_param)` | `ConTemplateApplyResult`; pass `ConTemplateApplyParam(connection_template=xml, mapping=mapping)` |
| `get_templates_in_connection(project_id, connection_id)` | `List[ConConnectionTemplate]` |
| `get_template_in_connection(project_id, connection_id, template_instance_id)` | `ConConnectionTemplate` |
| `delete(project_id, connection_id, template_id)` | removes the template including its operations |
| `delete_all(project_id, connection_id)` | removes all templates including operations |
| `explode(project_id, connection_id, template_id)` | removes template parameters, keeps operations |
| `explode_all(project_id, connection_id)` | explodes all templates |
| `get_template_common_operation_properties(project_id, connection_id, template_id)` | `ConOperationCommonProperties` |
| `update_template_common_operation_properties(project_id, connection_id, template_id, con_operation_common_properties)` | per-template common properties |
| `load_defaults(project_id, connection_id, template_id)` | `ParameterUpdateResponse`; loads parameter defaults |

C#-only helper: `client.Template.ImportTemplateFromFile(fileName)` reads a `.contemp` file from disk into a `ConTemplateMappingGetParam`. Python has no equivalent — read the file yourself.

### Connection Library — `api_client.connection_library`

| Method | Returns / notes |
|---|---|
| `get_design_sets()` | `List[ConDesignSet]` available to the signed-in user |
| `propose(project_id, connection_id, con_connection_library_search_parameters=None)` | `List[ConDesignItem]` matching the connection topology |
| `get_template(design_set_id=..., design_item_id=...)` | `str` — BASE64-encoded template; decode before use (Core rule 5) |
| `get_design_item_picture(design_set_id=..., design_item_id=...)` | returns `None` in the generated client — use `save_design_item_picture` instead |
| `save_design_item_picture(design_set_id, design_item_id, file_name)` (ext) | downloads the design item PNG and writes it to `file_name` |
| `publish_connection(project_id, connection_id, con_template_publish_param=None)` | `bool`; publishes the connection as a template to the Private or Company set (`ConTemplatePublishParam(name=..., author=..., company_name=..., design_set_type=...)`, `ConDesignSetType` is `private` or `company`) |

### Parameter — `api_client.parameter`

| Method | Returns / notes |
|---|---|
| `get_parameters(project_id, connection_id, include_hidden=None)` | `List[IdeaParameter]` |
| `update(project_id, connection_id, idea_parameter_update)` | `ParameterUpdateResponse`; takes a `List[IdeaParameterUpdate]` |
| `delete_parameters(project_id, connection_id)` | deletes all parameters and parameter-model links |
| `evaluate_expression(project_id, connection_id, body=None)` | `str`; evaluates an expression string in the connection's parameter context |

See the [parameters getting started guide](../api_parameters_getting_started.md) and the [expression reference](../api_parameter_reference_guide.md) for expression syntax.

### Load effects — `api_client.load_effect`

| Method | Returns / notes |
|---|---|
| `get_load_effects(project_id, connection_id, is_percentage=None)` | `List[ConLoadEffect]` |
| `get_load_effect(project_id, connection_id, load_effect_id, is_percentage=None)` | `ConLoadEffect` |
| `add_load_effect(project_id, connection_id, con_load_effect)` | `ConLoadEffect` |
| `update_load_effect(project_id, connection_id, con_load_effect)` | `ConLoadEffect` |
| `delete_load_effect(project_id, connection_id, load_effect_id)` | `int` |
| `get_load_settings(project_id, connection_id)` | `ConLoadSettings` (`loads_in_equilibrium`, `loads_in_percentage`) |
| `set_load_settings(project_id, connection_id, con_load_settings)` | `ConLoadSettings` |

### Material — `api_client.material`

GET methods list items **used in the project**; ADD methods add an item **from the MPRL library** (the IDEA StatiCa material and product range library) to the project.

| Method | Returns / notes |
|---|---|
| `get_all_materials(project_id)` | all materials in the project |
| `get_steel_materials` / `get_concrete_materials` / `get_bolt_grade_materials` / `get_welding_materials` / `get_headed_stud_grade_materials` / `get_bolt_assemblies` / `get_cross_sections` `(project_id)` | per-type lists of items used in the project |
| `add_material_steel` / `add_material_concrete` / `add_material_bolt_grade` / `add_material_weld` / `add_material_headed_stud_grade` / `add_bolt_assembly` `(project_id, ConMprlElement(mprl_name="S 355"))` | adds the named MPRL item to the project |
| `add_cross_section(project_id, ConMprlCrossSection(material_name="S 355", mprl_name="HEB200"))` | adds an MPRL cross-section |

### Calculation and results — `api_client.calculation`

| Method | Returns / notes |
|---|---|
| `calculate(project_id, request_body)` | `List[ConResultSummary]`; `request_body` is the list of connection ids; runs the CBFEM analysis |
| `get_results(project_id, request_body)` | `List[ConnectionCheckRes]` — detailed check results |
| `get_raw_json_results(project_id, request_body)` | `List[str]` — raw CBFEM results, one JSON string per connection (serialized `CheckResultsData`); parse with `json.loads` |

There is no endpoint returning FEM mesh stress/strain fields; `get_raw_json_results` is the deepest result level in the public API.

### Report — `api_client.report`

| Method | Returns / notes |
|---|---|
| `save_report_pdf(project_id, connection_id, file_name)` (ext, preferred) | writes a single-connection PDF report |
| `save_report_word(project_id, connection_id, file_name)` (ext, preferred) | writes a single-connection Word report |
| `save_multiple_report_pdf(project_id, connection_ids, file_name)` (ext) | one PDF with a section per connection — use for batches, do not loop single reports |
| `save_multiple_report_word(project_id, connection_ids, file_name)` (ext) | Word equivalent |
| `generate_pdf` / `generate_word` / `generate_html_zip` / `generate_pdf_for_mutliple` / `generate_word_for_multiple` | raw generated methods; note `generate_pdf_for_mutliple` is spelled exactly like that (a shipped operationId typo) |

### Export — `api_client.export`

| Method | Returns / notes |
|---|---|
| `export_ifc_file(project_id, connection_id, file_name)` (ext, preferred) | writes the connection to an IFC file |
| `export_iom(project_id, connection_id, version=None)` | `str` — IOM XML including `OpenModelContainer` |
| `export_iom_connection_data(project_id, connection_id)` | `ConnectionData` (IdeaRS.OpenModel.Connection.ConnectionData) |
| `export_ifc(project_id, connection_id)` | `str` — raw IFC content |

### Design-code conversion — `api_client.conversion`

Converts a whole project to a different design code. Per the spec, only ECEN (Eurocode) source projects are supported.

| Method | Returns / notes |
|---|---|
| `get_conversion_mapping(project_id, country_code=None)` | `ConConversionSettings` — default mappings to the target `CountryCode` (`ecen`, `american`, `canada`, `australia`, `india`, `sia`, `rus`, `chn`, `hkg`) |
| `change_code(project_id, con_conversion_settings)` | `str`; applies the conversion. Edit the `ConversionMapping` lists (`steel`, `concrete`, `cross_sections`, `fasteners`, `welds`, `bolt_grade` — each item has `source_value`/`target_value`) before calling |

### Settings — `api_client.settings`

| Method | Returns / notes |
|---|---|
| `get_settings(project_id, search=None)` | `Dict[str, object]` of project setting values; `search` filters by key |
| `update_settings(project_id, request_body)` | `Dict[str, object]`; pass a dict of key-value pairs to change |

Settings live here and only here — there is no `get_setup`/`update_setup` on the project API.

### Presentation — `api_client.presentation`

| Method | Returns / notes |
|---|---|
| `get_data_scene3_d(project_id, connection_id)` | `DrawData` for 3D scene visualization |
| `get_data_scene3_d_text(project_id, connection_id)` | `str` — the same data serialized as JSON |

### Client/service info

`connect_client` is called automatically; the id is at `api_client.client_id`. The service assembly version is available via the generated `ClientApi`:

```python
from ideastatica_connection_api import ClientApi
service_version = ClientApi(api_client.client).get_version()
```

## C# client surface

`IConnectionApiClient` (created by `ConnectionApiServiceAttacher` or `ConnectionApiServiceRunner`, both returning a connected client from `CreateApiClient()`). Accessors: `ClientApi`, `Calculation`, `Connection`, `ConnectionLibrary`, `Conversion`, `Export`, `LoadEffect`, `Material`, `Member`, `Operation`, `Parameter`, `Presentation`, `Project`, `Report`, `Settings`, `Template`, plus `ActiveProjectId` and `ClientId` properties.

Generated method names are the Python names in PascalCase with an `Async` suffix and `Guid projectId` instead of a string: `calculate` → `CalculateAsync(Guid projectId, List<int> requestBody)`, `get_connections` → `GetConnectionsAsync(Guid projectId)`, `update` → `UpdateAsync`, `get_settings` → `GetSettingsAsync`, and so on. The extension methods differ from Python in these places:

| C# extension member | Python equivalent / notes |
|---|---|
| `Project.ProjectId` (Guid) | `project.active_project_id` |
| `Project.OpenProjectAsync(filePath)` | `open_project_from_filepath` (.ideaCon only in C#) |
| `Project.SaveProjectAsync(projectId, fileName)` | `download_project` |
| `Project.CreateProjectFromIomFileAsync(iomFilePath, connectionsToCreate = null)` | IOM file import; sets `ProjectId` |
| `Project.UpdateProjectFromIomFileAsync(projectId, iomFilePath)` | `update_from_iom` from a file path |
| `Project.CreateProjectAsync(designCode)` / `CreateProjectAsync(designCode, name)` | empty project; design codes "ECEN", "American", "AUS"; sets `ProjectId` |
| `Report.SaveReportPdfAsync(projectId, connectionId, filePath)` | `save_report_pdf` |
| `Report.SaveMultipleReportsPdfAsync(projectId, connectionIds, filePath)` | `save_multiple_report_pdf` — note the C# name is plural "Reports", the Python name is singular "report" |
| `Report.SaveReportWordAsync` / `SaveMultipleReportsWordAsync` | Word equivalents |
| `Export.ExportIfcFileAsync(projectId, connectionId, filePath)` | `export_ifc_file` |
| `Template.ImportTemplateFromFile(fileName)` | no Python equivalent; reads a `.contemp` file into `ConTemplateMappingGetParam` |
| `ConnectionLibrary.GetDesignItemPictureDataAsync(designSetId, designItemId)` | returns `byte[]` |
| `ConnectionLibrary.SaveDesignItemPictureAsync(designSetId, designItemId, filePath)` | `save_design_item_picture` |

Most C# `Con*` model types live in the `IdeaStatiCa.Api.Connection.Model` namespace (e.g. `ConResultSummary.Passed`); the detailed check-result types (`ConnectionCheckRes`, `CheckResSummary`, ...) live in `IdeaRS.OpenModel.Connection` (e.g. `CheckResSummary.CheckValue`), and a few presentation/operation types (`DrawData`, `ConOperationCommonProperties`) in `IdeaStatiCa.ConnectionApi.Model`. All use PascalCase property names.

## Key data models (Python field names, JSON aliases in parentheses)

```text
ConResultSummary                       # returned by calculation.calculate
  .id              int                 # connection id
  .passed          bool                # overall pass/fail for the connection
  .result_summary  List[CheckResSummary]   (resultSummary)

CheckResSummary                        # one check group (e.g. plates, bolts, welds)
  .name            str                 # check description - USE THIS for the check name
  .check_value     float               (checkValue)  utilization expressed as a percentage (85.0 = 85 %);
                                       # already scaled by 100 by the service - do not multiply again
                                       # use .check_status for pass/fail
                                       # value is meaningless when .skipped is True - always gate on .skipped
                                       # (declared optional in the generated model, so guard for None too)
  .check_status    bool                (checkStatus) True = check satisfied
  .unity_check_message str             (unityCheckMessage)
  .skipped         bool                # True = check not performed; skip these when reporting
  .load_case_id    int                 (loadCaseId)
  # NO .check_section and NO .check_type - these fields do not exist

IdeaParameterUpdate(key="gap", expression="15")   # expression is ALWAYS a string

IdeaParameter                          # returned by parameter.get_parameters
  .key .expression .default .value .unit .parameter_type (parameterType)
  .description .validation_status (validationStatus) .is_visible (isVisible)
  .lower_bound (lowerBound) .upper_bound (upperBound) .parameter_validation

ConDesignSet                           # connection_library.get_design_sets
  .id  .name  .description  .owner_id (ownerId)  .type

ConDesignItem                          # connection_library.propose
  .name  .design_code (designCode)  .version
  .con_design_set_id  (conDesignSetId)   # UUID string
  .con_design_item_id (conDesignItemId)  # UUID string
  # NO .con_design_set_name - build a lookup: {ds.id: ds.name for ds in get_design_sets()}

ConConnectionLibrarySearchParameters   # propose() filter
  members: List[int], in_predefined_set/in_company_set/in_personal_set: bool,
  has_bolts/has_welds/has_anchor/has_clip_angles/is_moment/is_shear/is_truss/is_parametric:
  SearchOption ('ignore' | 'must' | 'mustNot')

ConTemplateMappingGetParam(template=<xml str>, member_ids=[...])
ConTemplateApplyParam(connection_template=<xml str>, mapping=<TemplateConversions>)

TemplateConversions                    # returned by get_default_template_mapping
  .conversions     List[BaseTemplateConversion]   # FLAT list - filter by .description
  .country_code    str (countryCode)
BaseTemplateConversion
  .description     str                 # "Steel", "Concrete", "Weld", "Bolt Assembly", "Bolt grade",
                                       # or the cross-section/member item name from the template
                                       # (localized; English values shown)
  .original_value  str (originalValue)
  .new_value       str (newValue)      # set this to remap

ConTemplateApplyResult
  .applied_without_issues bool (appliedWithoutIssues)
  .issues          List[ConNonConformityIssue]

ConLoadEffect
  .id int  .name str  .active bool
  .is_percentage   bool (isPercentage)
  .member_loadings List[ConLoadEffectMemberLoad] (memberLoadings)
ConLoadEffectMemberLoad
  .member_id       int (memberId)
  .position        ConLoadEffectPositionEnum.END ('End') | .BEGIN ('Begin')
  .section_load    ConLoadEffectSectionLoad (sectionLoad)
ConLoadEffectSectionLoad(n=, vy=, vz=, mx=, my=, mz=)
  # forces n, vy, vz in kN; moments mx, my, mz in kNm
  # n: positive = tension at the End position; sign inverted at Begin
  # my: sign inverted at Begin relative to the standard convention

ConConnection
  .id int  .identifier  .name  .description
  .analysis_type (analysisType)  .is_calculated (isCalculated)  .include_buckling (includeBuckling)

ConMember
  .id int  .name  .active  .is_continuous (isContinuous)  .cross_section_id (crossSectionId)
  .mirror_y .mirror_z  .is_bearing (isBearing)  .position  .model  .stiffness_analysis

ConProject
  .project_id (projectId)  .project_info (projectInfo, ConProjectData)  .connections
ConProjectData
  .name .description .project_number (projectNumber) .author .design_code (designCode) .var_date (date)

ConMprlElement(mprl_name="S 355")
ConMprlCrossSection(material_name="S 355", mprl_name="HEB200")
ConLoadSettings(loads_in_equilibrium=..., loads_in_percentage=...)
ConOperationCommonProperties(weld_material_id=..., plate_material_id=..., bolt_assembly_id=...)
```

## Workflows

All Python snippets are complete programs. Replace the file paths and start the service (or switch to the runner pattern) before running.

### 1. Calculate all connections and report results

```python
import logging
from ideastatica_connection_api.connection_api_service_attacher import ConnectionApiServiceAttacher

logging.basicConfig(level=logging.INFO)

with ConnectionApiServiceAttacher("http://localhost:5000").create_api_client() as api_client:
    api_client.project.open_project_from_filepath(r"C:\projects\my-connection.ideaCon")
    project_id = api_client.project.active_project_id

    connections = api_client.connection.get_connections(project_id)
    conn_ids = [c.id for c in connections]

    results = api_client.calculation.calculate(project_id, conn_ids)

    for conn_result in results:
        print(f"Connection {conn_result.id}: {'PASS' if conn_result.passed else 'FAIL'}")
        for summary in conn_result.result_summary:
            if summary.skipped:
                continue
            status = "OK" if summary.check_status else "NOT OK"
            print(f"  {summary.name}: {summary.check_value:.1f} % ({status})")

    # one PDF containing a section per connection - preferred for batches
    api_client.report.save_multiple_report_pdf(project_id, conn_ids, r"C:\reports\all-connections.pdf")

    # save the project including any changes made through the API
    api_client.project.download_project(project_id, r"C:\projects\my-connection-calculated.ideaCon")
```

### 2. Apply a Connection Library template

```python
import base64
import logging
from ideastatica_connection_api.connection_api_service_attacher import ConnectionApiServiceAttacher
from ideastatica_connection_api import (
    ConConnectionLibrarySearchParameters,
    ConTemplateMappingGetParam,
    ConTemplateApplyParam,
)

logging.basicConfig(level=logging.INFO)

with ConnectionApiServiceAttacher("http://localhost:5000").create_api_client() as api_client:
    api_client.project.open_project_from_filepath(r"C:\projects\my-connection.ideaCon")
    project_id = api_client.project.active_project_id

    conn = api_client.connection.get_connections(project_id)[0]
    member_ids = [m.id for m in api_client.member.get_members(project_id, conn.id)]

    # 1) propose design items matching the connection topology
    candidates = api_client.connection_library.propose(
        project_id, conn.id,
        ConConnectionLibrarySearchParameters(in_predefined_set=True))
    if not candidates:
        raise RuntimeError("No library design items match this connection.")
    item = candidates[0]
    print(f"Applying '{item.name}' ({item.design_code})")

    # 2) download the template - it is BASE64, decode to XML before use
    template_b64 = api_client.connection_library.get_template(
        design_set_id=item.con_design_set_id,
        design_item_id=item.con_design_item_id)
    template_xml = base64.b64decode(template_b64).decode("utf-8")

    # 3) get the default mapping of template entities onto this connection
    mapping = api_client.template.get_default_template_mapping(
        project_id, conn.id,
        ConTemplateMappingGetParam(template=template_xml, member_ids=member_ids))

    # optional: remap materials - .conversions is a FLAT list, filter by .description
    for c in mapping.conversions:
        if c.description == "Steel":
            c.new_value = "S 450"

    # 4) apply
    result = api_client.template.apply_template(
        project_id, conn.id,
        ConTemplateApplyParam(connection_template=template_xml, mapping=mapping))
    print("Applied without issues:", result.applied_without_issues)
```

### 3. Parameter sweep

```python
import json
import logging
from ideastatica_connection_api.connection_api_service_attacher import ConnectionApiServiceAttacher
from ideastatica_connection_api import IdeaParameterUpdate

logging.basicConfig(level=logging.INFO)

with ConnectionApiServiceAttacher("http://localhost:5000").create_api_client() as api_client:
    api_client.project.open_project_from_filepath(r"C:\projects\parametric-connection.ideaCon")
    project_id = api_client.project.active_project_id
    conn_id = api_client.connection.get_connections(project_id)[0].id

    # discover available parameter keys first
    for p in api_client.parameter.get_parameters(project_id, conn_id):
        print(p.key, "=", p.expression)

    for thickness_mm in [10, 12, 15, 20]:
        api_client.parameter.update(
            project_id, conn_id,
            [IdeaParameterUpdate(key="plateThickness", expression=str(thickness_mm))])

        summary = api_client.calculation.calculate(project_id, [conn_id])[0]

        raw = api_client.calculation.get_raw_json_results(project_id, [conn_id])
        detailed = json.loads(raw[0])   # CheckResultsData as a dict

        print(f"t = {thickness_mm} mm -> passed: {summary.passed}")
```

`"plateThickness"` is an example — parameter keys are project-specific; always read them from `get_parameters` first.

### 4. Convert a project to a different design code

```python
import logging
from ideastatica_connection_api.connection_api_service_attacher import ConnectionApiServiceAttacher
from ideastatica_connection_api import CountryCode

logging.basicConfig(level=logging.INFO)

with ConnectionApiServiceAttacher("http://localhost:5000").create_api_client() as api_client:
    # source project must use the ECEN design code
    api_client.project.open_project_from_filepath(r"C:\projects\ecen-connection.ideaCon")
    project_id = api_client.project.active_project_id

    settings = api_client.conversion.get_conversion_mapping(project_id, CountryCode.AMERICAN)
    for m in settings.steel or []:
        print(f"steel: {m.source_value} -> {m.target_value}")
    # edit m.target_value entries here if the defaults are not what you want

    api_client.conversion.change_code(project_id, settings)
    api_client.project.download_project(project_id, r"C:\projects\aisc-connection.ideaCon")
```

### 5. C# end-to-end example

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using IdeaStatiCa.ConnectionApi;

internal class Program
{
    private static async Task Main()
    {
        var factory = new ConnectionApiServiceAttacher("http://localhost:5000");
        await using IConnectionApiClient client = await factory.CreateApiClient();

        await client.Project.OpenProjectAsync(@"C:\projects\my-connection.ideaCon");
        Guid projectId = client.Project.ProjectId;

        var connections = await client.Connection.GetConnectionsAsync(projectId);
        var connectionIds = connections.Select(c => c.Id).ToList();

        var results = await client.Calculation.CalculateAsync(projectId, connectionIds);
        foreach (var res in results)
        {
            Console.WriteLine($"Connection {res.Id}: {(res.Passed ? "PASS" : "FAIL")}");
            foreach (var check in res.ResultSummary)
            {
                if (check.Skipped) continue;
                Console.WriteLine($"  {check.Name}: {check.CheckValue:F1} %");
            }
        }

        await client.Report.SaveMultipleReportsPdfAsync(projectId, connectionIds, @"C:\reports\all-connections.pdf");
        await client.Project.SaveProjectAsync(projectId, @"C:\projects\my-connection-calculated.ideaCon");

        await client.Project.CloseProjectAsync(projectId);
    }
}
```

More worked examples (both languages) are in the repository: <https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api>.

## Mistakes to avoid

| Wrong | Correct |
|---|---|
| `IdeaParameterUpdate(key="gap", expression=15)` | `expression="15"` — expressions are always strings |
| Passing the `connection_library.get_template` result straight to `apply_template` | Decode it first: `base64.b64decode(b64).decode("utf-8")` |
| `summary.check_section`, `summary.check_type` | These fields do not exist; use `summary.name` |
| `item.con_design_set_name` | Does not exist; build a lookup from `get_design_sets()`: `{ds.id: ds.name for ds in ...}` |
| Looping `save_report_pdf` per connection for a batch | `save_multiple_report_pdf(project_id, conn_ids, file)` (C#: `SaveMultipleReportsPdfAsync`) |
| `api_client.project.get_setup(...)` / `update_setup(...)` | `api_client.settings.get_settings(...)` / `update_settings(...)` |
| `api_client.project.import_iom_file(...)` | `open_project_from_filepath(path)` for `.xml`/`.iom` files, or `api_client.project.import_iom(...)` |
| Instantiating `ProjectApi()` / `ReportApi()` directly | Use the client accessors: `api_client.project`, `api_client.report`, `api_client.export`, `api_client.connection_library` |
| Forgetting the project id after opening | `project_id = api_client.project.active_project_id` (C#: `client.Project.ProjectId`) |
| `api_client.operation.add_plate(...)`, `add_bolt`, `add_weld`, `add_cut` | No such endpoints; author geometry via `apply_template`, the Connection Library pipeline, or IOM import |

## Common error causes

- **Connection refused / timeout on the base URL** — the service is not running, or you attached to the wrong port. Start `IdeaStatiCa.ConnectionRestApi.exe` from the installation directory, or use the runner pattern. Readiness can be checked at `GET {base_url}/heartbeat`.
- **`API executable not found at path: ...`** from the runner (Python: `FileNotFoundError`; C#: `FileNotFoundException`) — the `setup_dir` does not point at the directory containing `IdeaStatiCa.ConnectionRestApi.exe` (worked example: `C:\Program Files\IDEA StatiCa\StatiCa 26.0`).
- **Method not found / unexpected 404** — SDK package version does not match the installed service version, or the method was hallucinated. Verify against this page and reinstall the SDK version matching your IDEA StatiCa installation.
- **404 or empty results referencing a project id** — the project was closed (clients close the active project on exit) or you used a stale id. Re-open and re-read `active_project_id`.
- **Pydantic validation error on a parameter update** — a non-string was passed to `expression`.
- **Template apply produces no change or reports issues** — check `ConTemplateApplyResult.applied_without_issues` and `.issues`; verify the template XML was base64-decoded and that `member_ids` in the mapping request match members from `get_members`.
- **Design-code conversion fails** — the source project is not ECEN; only ECEN projects can be converted.

## How to use this page with an AI assistant

Paste this entire page into the assistant's context (or point the tool at this URL) before asking it to generate Connection API code. Instruct it to: use only methods listed here, start from the standard boilerplate, and state explicitly when something you ask for is not available in the public API rather than inventing a call. When the assistant's SDK version differs from yours, regenerate against the matching package — method surfaces are version-specific. Report SDK bugs and missing functionality at [GitHub Discussions](https://github.com/idea-statica/ideastatica-public/discussions).
