# Connection API — LLM Reference

A single-file HTTP reference for AI coding assistants (and people) that drive the IDEA StatiCa Connection REST API from any language. It states what the service's own OpenAPI document does not say, the canonical call sequence, request bodies for the common tasks, the known issues of the release it was checked against, and a map of the routes.

> [!NOTE]
> Written for IDEA StatiCa **26.1** and checked against the **26.1.1** release: **API version 5.0**, all routes under **`/api/5`**, default port **5000**. Read the version of the service you talk to with `GET /api/5/clients/idea-service-version` (a JSON string, `"<major>.<minor>.<patch>.<build>"`). Older services expose older route sets; the deprecated prefixes `/api/1` to `/api/4` are still served but have fewer routes. Use `/api/5`. If `/api/5` routes do not answer, the service predates API 5.0 and this page does not apply to it.
>
> Section 9 lists the known defects of that release, each with a workaround that still works once the defect is fixed. Where a later service differs from this page, its own OpenAPI document is right.

Related pages: [Overview](connection_api_overview.md) | [Getting started](connection_api_getting_started.md) | [Concepts](connection_api_concepts.md) | [Changelog](connection_api_changelog.md) | [Parameters getting started](../api_parameters_getting_started.md) | [Expression parameter reference](../api_parameter_reference_guide.md)

## 1. Find or start the service

The service usually runs already. **Check before starting anything:**

```
GET http://localhost:5000/heartbeat                              -> 200
GET http://localhost:5000/api/5/clients/idea-service-version     -> "<major>.<minor>.<patch>.<build>"
```

Only if nothing answers, start `IdeaStatiCa.ConnectionRestApi.exe` from the installation folder (e.g. `C:\Program Files\IDEA StatiCa\StatiCa 26.1`) and poll `/heartbeat` until it answers 200. It listens on port 5000; for another port pass `-port:5193` (colon form: `IdeaStatiCa.ConnectionRestApi.exe -port:5193`). Running the service needs a licensed IDEA StatiCa installation on the same machine.

**The running service's OpenAPI document is the contract** for route names, methods, schemas and field names:

- Swagger UI: `http://localhost:5000/index.html` (the root `/` answers 301 and redirects there).
- OpenAPI document: `http://localhost:5000/swagger/5.0/swagger.json`. It is large — search it for the path or schema name you need (e.g. `"ConLoadEffect"`) instead of loading it whole. Section 12 maps the routes of API 5.0 and gives a one-line command that prints the routes of the running service.
- `/swagger/index.html`, `/swagger/v1/swagger.json` and `/openapi.json` do not exist (404).

## 2. Core rules

1. **`ClientId` header on every request.** `GET /api/5/clients/connect-client` returns the id as a raw GUID in `text/plain` (unquoted; do not JSON-parse it). Send it as the header `ClientId` on every other request; only `connect-client` and `idea-service-version` work without it. The swagger may not declare the header (section 9). A request without it is refused with `401` *"The 'ClientId' header is required. Call ConnectClient to obtain one."* (`/api/4` answers `401` too; only `/api/1` to `/api/3` answer `400` *"Request failed Missing ClientId in headers"*).
2. **Sessions are per client.** A project lives in service memory until you close it. One ClientId can hold several open projects (`GET /api/5/projects` lists them); another ClientId sees none of them and gets `404` on their ids. Use one ClientId per process and close only the projects you opened (section 9); there is no route to end a client session.
3. **Use ids from live responses.** `projectId` is the GUID that `open` or `create` returns. Connection, member, operation and load-effect ids are opaque integers that may not start at 1 and may have gaps. Material, cross-section and bolt-assembly ids come from the project's `GET .../materials/...` lists; an id the project does not hold is `422` naming it.
4. **SI units on input**: metres, radians, newtons, newton-metres, pascals. A 5 mm weld is `"size": 0.005`; a 300 kN axial force is `"n": 300000`.
5. **Bare JSON arrays of connection ids** for `calculate`, `results`, `rawresults-text`, `calculate-async` and the multi-connection reports: `[1]`, `[1,2,3]` — never a single number or an object.
6. **HTTP 200 is not success.** Operation POST/PUT answer `{operation, issues[], addedWithoutIssues}` and store an operation even when it fails; `calculate` answers 200 with `passed: false` and a diagnostic row. Read the body every time.
7. **Operation order matters.** An operation can only reference operations before it. There is no reorder and **no per-operation DELETE**; `DELETE .../operations` removes **all** operations of the connection together with its applied templates and its parameters (and their links).
8. **Send only the fields the schema lists, spelled as the schema spells them; read back what you wrote.** The service does not reject unknown or misplaced JSON fields today: they are ignored without an error, property names bind case-insensitively (`"Thickness"`, `"N"`), numeric strings are coerced, and a missing field gets its default. Do not rely on that; a later version may reject such requests. A misspelled key answers 200 and is lost — inside `sectionLoad` it stores a zero. On full-replace routes (typed-operation PUT, `PUT .../connections/{connectionId}`, `set-load-settings`) an omitted field is reset, not kept: a connection PUT with only `description` sets `name` to null.
9. **Send `"active": true` on every operation POST.** An omitted `active` defaults to true, but an operation stored inactive looks exactly like a successful add (no geometry, no message).
10. **JSON is camelCase; most enums are camelCase strings** (`ecen`, `doubleFillet`, `stress_Strain`). Exceptions to copy as they are: load-effect `position` (`Begin` / `End`) and `steelEdition` (`AISC_360_16_LRFD`, …). Take enum values from the swagger.
11. **One calculation at a time per project.** Only a second *async* job answers `409`. A synchronous `calculate` or `rawresults-text` sent while a calculation runs is not refused: it waits until that calculation finishes, then answers 200; writes to the project wait for it too. Use async jobs for long runs, or a generous client timeout. A synchronous calculation the client abandons (timeout, dropped connection) is cancelled, stores no results and leaves the project unlocked.
12. **A write empties stored results.** A write inside a connection empties that connection's results; a write to shared project data (materials, cross-sections, project data, settings, a connection rename) empties every connection's. This holds even for a write that changes nothing (a PUT of unchanged values, `PUT .../settings` with the current value). Finish editing, then calculate.

## 3. Canonical workflow

```
GET  /api/5/clients/connect-client                         -> ClientId (text/plain GUID)
POST /api/5/projects/open      multipart/form-data, file field "ideaConFile"
                                                           -> ConProject {projectId, projectInfo, connections[]}
 or POST /api/5/projects       JSON ConProjectData         -> ConProject (holds one empty connection CON1, id 1)
GET  /api/5/projects/{projectId}/connections               -> [ConConnection {id, name, ...}]
     ... edit: materials, members, operations, load effects, parameters
POST /api/5/projects/{projectId}/connections/calculate     body [1,2]  -> [ConResultSummary]
POST /api/5/projects/{projectId}/connections/results       body [1,2]  -> [ConnectionCheckRes]
GET  /api/5/projects/{projectId}/connections/{connectionId}/reports/pdf  -> PDF bytes
GET  /api/5/projects/{projectId}/download                  -> .ideaCon bytes (all API changes included)
GET  /api/5/projects/{projectId}/close                     -> 204
```

- **Open:** `multipart/form-data` with the file in a part named exactly **`ideaConFile`**. Another name gives `400` *"The ideaConFile field is required."*; a JSON body gives `415`; a file that is not a valid `.ideaCon` is refused (status code: section 9). Each entry of `connections[]` carries an integer `id` and a `name`.
- **Create:** a JSON body is required (at least `{}`), e.g. `POST /api/5/projects` `{"name": "demo", "countryCode": "american", "steelEdition": "AISC_360_16_ASD"}`. `countryCode` picks the design code: `ecen` (default), `american`, `canada`, `australia`, `rus`, `chn`, `india`, `hkg` (`sia` is in the enum but refused with `422`). `steelEdition` picks the edition within the code and changes the results (AISC LRFD vs ASD):

  | `countryCode` | default `steelEdition` | accepted |
  |---|---|---|
  | `ecen` | `EN_1993_1_8_2005` | `EN_1993_1_8_2005`, `EN_1993_1_8_2024` |
  | `american` | `AISC_360_16_LRFD` | `AISC_360_10_LRFD`, `AISC_360_10_ASD`, `AISC_360_16_LRFD`, `AISC_360_16_ASD`, `AISC_360_22_LRFD`, `AISC_360_22_ASD` |
  | `canada` | `CSA_S16_14` | `CSA_S16_14`, `CSA_S16_19` |
  | other codes | `NotSpecified` | `NotSpecified` only |

  Another combination is `422`. Read the edition from `GET /api/5/projects/{projectId}` (the list `GET /api/5/projects` reports `NotSpecified`). `PUT /api/5/projects/{projectId}` changes it; it cannot change `countryCode` (`422`; convert it as described in section 7). The design code also decides which library names resolve (section 7).
- **Report:** a report takes seconds to tens of seconds; use a generous timeout. It shows the connection's active load effects and calculates first when no matching calculation is stored. An unknown connection id is `422`. For several connections send one `POST /api/5/projects/{projectId}/reports/pdf` (or `/reports/word`) with an id array instead of looping. `.../reports/word` and `.../reports/htmlZip` exist per connection too.
- **Close:** always close the projects you opened; `close` is a GET with a side effect, and it answers `204` also for a project that is already closed. Never close a project id another ClientId opened (section 9).

## 4. Reading results

**`calculate`** returns one `ConResultSummary` per requested connection: `id`, `passed` (bool), `resultSummary[]` of `CheckResSummary` rows (`name`, `checkValue`, `checkStatus` — true = satisfied, `loadCaseId`, `unityCheckMessage`, `skipped`). A trimmed response (values illustrative):

```json
POST /api/5/projects/{projectId}/connections/calculate        body [1]
[ { "id": 1, "passed": true,
    "resultSummary": [
      { "name": "Analysis", "checkValue": 100.0,  "checkStatus": true, "loadCaseId": 0, "unityCheckMessage": "100.0%",         "skipped": false },
      { "name": "Plates",   "checkValue": 0.34,   "checkStatus": true, "loadCaseId": 2, "unityCheckMessage": "0.3 < 5.0%",     "skipped": false },
      { "name": "Bolts",    "checkValue": 90.13,  "checkStatus": true, "loadCaseId": 6, "unityCheckMessage": " 90.1 < 100%",   "skipped": false },
      { "name": "Welds",    "checkValue": 38.49,  "checkStatus": true, "loadCaseId": 6, "unityCheckMessage": " 38.5 < 100%",   "skipped": false },
      { "name": "Buckling", "checkValue": -100.0, "checkStatus": true, "loadCaseId": 0, "unityCheckMessage": "Not calculated", "skipped": true } ] } ]
```

- `checkValue` is **not one unit**: a percentage for Analysis, Welds, Bolts and Anchors (`85.35` = 85.35 %, already scaled — do not multiply by 100; for Analysis it is the share of the load applied, 100 % = the full load), a small number for Plates: the plastic strain in %, which the message compares with the strain limit (`0.3 < 5.0%`); it equals the largest `checkResPlate[].maxStrain` / 100. For display use `unityCheckMessage`, which matches the desktop application; for pass/fail use `checkStatus`, do not parse the message.
- `skipped: true` means the check was not run; ignore its `checkValue`. A row reading `Not calculated` (`checkValue` -100, e.g. the buckling row) was not evaluated either. When it names a component the model contains, either the part is misplaced or an adjacent check is far above 100 %; lower the load to tell them apart.
- `passed: false` alone does not mean the joint failed — read the Analysis row. A message **ending** in `There are no loads on members` (other warnings may be concatenated in front) means nothing to solve; `CBFEM analysis failed - the solver did not produce any results.` means the connection has no members. Read the Analysis row also when `passed` is true.
- An unknown connection id (`[999]`, or `[2,999]`) fails the whole request with `404`, in `calculate` and in `results`; nothing is calculated.

**`results`** returns one `ConnectionCheckRes` per connection with per-element rows. Trimmed (values illustrative, matching the summary above):

```json
POST /api/5/projects/{projectId}/connections/results          body [1]
[ { "id": 1, "name": "CON1", "connectionID": "00000000-0000-0000-0000-000000000000",
    "checkResSummary": [ "...the same rows as calculate..." ],
    "checkResPlate": [ { "name": "EP", "checkStatus": true, "loadCaseId": 2, "maxStress": 355.08, "maxStrain": 34.18, "items": [] } ],
    "checkResWeld":  [ { "name": "BEAM-bfl 1", "id": 3, "unityCheck": 38.49, "checkStatus": true, "isFullStrength": false, "loadCaseId": 6, "items": [5] },
                       { "name": "",           "id": 3, "unityCheck": 35.20, "checkStatus": true, "isFullStrength": false, "loadCaseId": 6, "items": [6] },
                       { "name": "BEAM-tfl 1", "id": 4, "unityCheck": "NaN", "checkStatus": true, "isFullStrength": true,  "loadCaseId": 0, "items": [7] } ],
    "checkResBolt":  [ { "name": "B15", "unityCheck": 90.13, "checkStatus": true, "loadCaseId": 6 } ],
    "checkResAnchor": [], "checkResConcreteBlock": [], "bucklingResults": [],
    "messages": { "messages": [] } } ]
```

| list | fields | units |
|---|---|---|
| `checkResPlate` | `name`, `checkStatus`, `loadCaseId`, `maxStress`, `maxStrain`, `items` | `maxStress` in MPa; `maxStrain` on a ratio×10000 scale (plastic strain in % = `maxStrain` / 100) |
| `checkResWeld` | `name`, `id`, `unityCheck`, `checkStatus`, `loadCaseId`, `isFullStrength`, `items` | `unityCheck` in % |
| `checkResBolt`, `checkResAnchor`, `checkResConcreteBlock` | `name`, `unityCheck`, `checkStatus`, `loadCaseId` | `unityCheck` in % |
| `bucklingResults` | `loadCaseId`, `shape`, `factor` | — |

- `results` **reads** the last calculation, it does not run one. Without a `calculate` after the last edit it answers 200 with empty lists and no error. A list is also empty when the connection has no such item.
- A weld with `isFullStrength: true` (a `cjp` weld, or any `edgeToEdge` weld) has `unityCheck` as the **string** `"NaN"`; guard numeric parsing.
- A `doubleFillet` weld gives two rows with the same `id`, one per leg (one of them with an empty `name`). Take the maximum per `id`.
- A bolt `name` such as `B15` carries an opaque solver number; it need not start at 1 or be sequential.
- `loadCaseId` on an element row is the load effect governing that element, so one response mixes load effects. For one load effect, calculate it alone (section 5), then read `results`.
- `connectionID` (a GUID) reads all-zero for a connection created through the API; key on `id`.
- `messages.messages[]` carries errors and warnings (`description`, `number`); it may also be `null`.

**`rawresults-text`** is the full CBFEM result set, for computing rather than display. `POST /api/5/projects/{projectId}/connections/rawresults-text` with an id array returns an array of **strings**, each a JSON document — parse twice (`[json.loads(s) for s in response.json()]`). The single-connection form `POST .../connections/{connectionId}/rawresults-text` returns one string. For a connection without results the element is a sentence (`Results for connection {id} failed: …`), not JSON. Raw values are **SI**: Pa, m, N, and ratios 0..1 (`maxStress` Pa / 1e6 = the MPa of `results`; raw `maxStrain` × 10000 = `results` `maxStrain`; raw ratios × 100 = the percentages). Non-applicable fields are the string `"NaN"`. Most groups (`plates`, `bolts`, `welds`, `boltsAnchor`, `concreteBlocks`, `summary`, …) are objects keyed by element id as a string, one entry per element, for the governing load effect; per-load-effect forces of bolts, anchors and welds are in `forcesAllLoadCases`, keyed by load-effect id as a string. `rawresults-text` calculates again itself, so pass `?loadEffectIds=` to it as well when you need a subset.

## 5. Asynchronous calculation and load-effect subsets

```
POST   /api/5/projects/{projectId}/connections/calculate-async                body [1,2]  -> 202 ConCalculationJob
POST   /api/5/projects/{projectId}/connections/{connectionId}/calculate-async            -> 202 ConCalculationJob
GET    /api/5/projects/{projectId}/calculation-jobs/{jobId}                               -> ConCalculationJob
DELETE /api/5/projects/{projectId}/calculation-jobs/{jobId}                               -> 202, cancels
```

Start, then poll about once a second until the status is terminal:

```json
POST /api/5/projects/{projectId}/connections/1/calculate-async?loadEffectIds=1&loadEffectIds=3      (no body)
-> 202 { "jobId": "<jobId>", "status": "queued", "progress": 0.0, "connectionIds": [1], ... }

GET  /api/5/projects/{projectId}/calculation-jobs/<jobId>
-> 200 { "jobId": "<jobId>", "projectId": "<projectId>", "status": "running", "progress": 0.4,
         "connectionIds": [1], "connectionsCompleted": 0, "currentConnectionId": 1,
         "message": "<load effect and solver iteration being computed>", ... }
-> 200 { ..., "status": "finished", "progress": 1.0, "connectionsCompleted": 1, "finishedAt": "...",
         "results": [ { "id": 1, "passed": true, "resultSummary": [ ... ] } ] }
```

- `ConCalculationJob`: `jobId`, `projectId`, `status`, `progress`, `connectionIds`, `connectionsCompleted`, `currentConnectionId`, `message`, `createdAt`, `finishedAt`, `error`, `results` (`ConResultSummary[]` once finished).
- `status` ∈ `queued`, `running`, `finished`, `failed`, `cancelled`; poll until it is one of the last three. The 202 start response shows `queued` or already `running`. `progress` is a fraction 0..1 (not a percentage) of the **current connection**: it restarts at 0 when the job moves to the next connection, so it is not monotonic across a multi-connection job. On `failed`, read `error`.
- Async is not faster. It avoids holding one HTTP request open for minutes, shows progress, and can be cancelled.
- Starting a job while one is active for the project is `409`; an unknown connection id is `404` before any job is registered.
- Cancel kills the running solver and skips the remaining connections. The cancelled job's `results` list is empty, even for connections that finished; a connection the cancel interrupted keeps the results it had stored before. Read `results` to see what is stored. Cancelling a finished job answers `202` and changes nothing.
- **`?loadEffectIds=`** exists only on the single-connection routes (`.../connections/{connectionId}/calculate`, `.../rawresults-text`, `.../calculate-async`). Repeat it for several ids: `?loadEffectIds=1&loadEffectIds=2`. It solves only the listed load effects, overriding each load effect's `active` flag; an unknown id is `422`. In a parametric loop, restricting each run to the governing load effects is usually the biggest saving. The single-connection `calculate` returns one `ConResultSummary` object, not an array.

## 6. Typed operations

The model is built from **operations** of these kinds: `weld`, `cut`, `plate-cut`, `bolt-grid`, `anchor-grid`, `pin-grid`, `contact`, `contact-grid`, `negative-member`, `negative-plate`, `stiffening-member`, `stiffening-plate`, `work-plane`. Each kind has the same four routes:

```
GET  /api/5/projects/{projectId}/connections/{connectionId}/operations/{kind}                 list (full typed objects)
POST /api/5/projects/{projectId}/connections/{connectionId}/operations/{kind}                 add     -> ConAddOperationResult
PUT  /api/5/projects/{projectId}/connections/{connectionId}/operations/{kind}                 update  -> ConAddOperationResult
GET  /api/5/projects/{projectId}/connections/{connectionId}/operations/{kind}/{operationId}   one
```

- **PUT targets the `id` in the body.** There is no PUT (405) and no DELETE on `.../{kind}/{operationId}`. An unknown id is `404`.
- **An id that belongs to an operation of another kind is `409`** on the GET by id, the PUT and a POST; the message names both types. Retrying does not help — use the right `{kind}`. A POST carrying the id of an operation of the same kind replaces it; leave `id` out to append.
- **The wrapper:** `{"operation": {"id": 5, "name": "CUT1", "operationType": "CutBeamData", "active": true, ...}, "issues": [{"operationId", "description", "details", "severity"}], "addedWithoutIssues": true}` with `severity` ∈ `info`, `warning`, `error`, `terminatedError`. The new operation's id is `operation.id`. A failed operation is still stored. A missing catalog item (material, bolt assembly) is refused with `422`; a reference to an object that does not exist (e.g. a `plateId`) answers 200 with `addedWithoutIssues: false`.
- **`addedWithoutIssues: true` does not prove geometry** (section 9). After each add, count what you expected in `GET /api/5/projects/{projectId}/connections/{connectionId}/export-iom-connection-data` (JSON by default: `plates`, `welds`, `beams`, `boltGrids`, `anchorGrids`, `concreteBlocks`, …; negative volumes are listed with `isNegativeObject: true`). A weld appears there only as `{"id": N}`; read its type, size and parts from `GET .../export-iom` (XML).
- When several operations fail, fix the **first** one; later ones often fail only because they depend on it.
- **Edit = GET → change → PUT.** An unmodified typed GET body PUT back re-creates the operation. A PUT replaces the whole operation, so never PUT a hand-trimmed body: a grid sent without its `geometry.userPositions` is emptied and still reports success.
- `GET .../connections/{connectionId}/operations` lists every operation as a thin `ConOperation` (`id`, `name`, `active`, `isImported`, `operationType`). `operationType` is a label (e.g. `CutBeamData` for a cut) that does not follow `{kind}`; do not match on it. Composite operations authored in the desktop application (e.g. a base-plate or end-plate assembly) appear only there and cannot be edited through typed routes.
- Author bolt, anchor and pin grids with `"definedBy": "plate"`; `coordinateSystem` describes imported grids and is refused (`422`) for new ones.
- **Reference parts with `ConConnectedItem`:** `{"operationId", "plateSubIndex": 1}` for the plate an operation created; `{"memberId", "plateSubIndex"}` for a wall of a member — in a weld, the wall's 0-based index in that member's `plates[]` of `export-iom-connection-data`; in a grid, see the example below. The index base differs by context (section 8).

```json
POST /api/5/projects/{projectId}/connections/{connectionId}/operations/cut
{ "name": "CUT1", "active": true,
  "member": { "kind": "member", "memberId": 2 },
  "cutBy":  { "kind": "member", "memberId": 1 },
  "cuttingMethod": "surface", "cuttingPlane": "closer", "direction": "perpendicular",
  "websWeld":    { "kind": "continuous", "type": "doubleFillet", "size": 0.004 },
  "flangesWeld": { "kind": "continuous", "type": "doubleFillet", "size": 0.006 } }
```

**Members** (`.../connections/{connectionId}/members`): POST adds a `ConMember` (the first member added becomes the bearing member; the project must already hold a cross-section), PUT updates the member named by the body `id`, `DELETE .../members/{memberId}` removes one, `PUT .../members/{memberId}/set-bearing-member` moves the bearing role. Angles in `position` are radians, offsets metres. `POST .../connections/{connectionId}/copy` is a deep copy with all members and operations; for a separate joint use `POST /api/5/projects/{projectId}/connections`.

### Example: beam bolted to a column through an end plate

Built in the empty `CON1` (id 1) of a project created with `{"countryCode": "ecen"}`; `.../` stands for `/api/5/projects/{projectId}/`. `<NAME>` is the id an earlier response returned: `id` of a cross-section or member, `operation.id` of an operation. Check `addedWithoutIssues` after every operation POST.

```json
POST .../materials/cross-sections     {"mprlName": "HEB300", "materialName": "S 355"}   -> {"id": <HEB300>, "name": "HEB300", ...}
POST .../materials/cross-sections     {"mprlName": "IPE300", "materialName": "S 355"}   -> {"id": <IPE300>, ...}
POST .../materials/bolt-assemblies    {"mprlName": "M20 8.8"}      then <M20> = id of "M20 8.8" in GET .../materials/bolt-assemblies

POST .../connections/1/members        column: vertical (gamaPitch = pi/2), starts at the node; bearing because it is first
{ "name": "COL", "crossSectionId": <HEB300>, "isContinuous": false, "connectedBy": "begin",
  "position": { "definedBy": "rotationsOfX", "betaDirection": 0, "gamaPitch": 1.5708, "alphaRotation": 0,
                "offsetEx": 0, "offsetEy": 0, "offsetEz": 0 } }
POST .../connections/1/members        beam: along +X, raised 0.6 m
{ "name": "BEAM", "crossSectionId": <IPE300>, "isContinuous": false, "connectedBy": "begin",
  "position": { "definedBy": "rotationsOfX", "betaDirection": 0, "gamaPitch": 0, "alphaRotation": 0,
                "offsetEx": 0, "offsetEy": 0, "offsetEz": 0.6 } }

POST .../connections/1/operations/stiffening-plate        end plate across the beam, mid-plane 0.15 + 0.02/2 from the node
{ "name": "EP", "active": true, "plateType": "structural", "shape": "rectangular", "thickness": 0.02,
  "width": 0.22, "width2": 0.22, "height": 0.10, "height2": 0.10,
  "positioning": { "conPlatePositioningType": "memberLcs", "onMemberId": <BEAM>,
                   "offset": {"x": 0.16, "y": 0, "z": 0}, "rotation": {"x": 0, "y": 0, "z": 0} } }

POST .../connections/1/operations/work-plane              plane on the outer face of the end plate
{ "name": "WP-EP", "active": true, "method": "byNormalVector", "relatedTo": "member", "memberId": <BEAM>,
  "origin": {"x": 0.17, "y": 0, "z": 0}, "normalVector": {"x": 1, "y": 0, "z": 0} }
POST .../connections/1/operations/cut                     trim the beam there
{ "name": "CUT-BEAM", "active": true, "member": {"kind": "member", "memberId": <BEAM>},
  "cutBy": {"kind": "workPlane", "operationId": <WP-EP>}, "cuttingMethod": "surface", "direction": "perpendicular" }

POST .../connections/1/operations/weld                    beam web onto the end plate face; repeat for each flange
{ "name": "W-WEB", "active": true, "placement": "edgeToSurface",
  "firstMemberOrPlate":  {"memberId": <BEAM>, "plateSubIndex": <web>}, "firstPlateEdgeIndex": [1],
  "secondMemberOrPlate": {"operationId": <EP>, "plateSubIndex": 1},   "secondPlateEdgeIndex": 1,
  "weld": {"kind": "continuous", "type": "doubleFillet", "size": 0.005} }

POST .../connections/1/operations/bolt-grid               8 x M20 through the end plate and the column flange
{ "name": "BOLTS", "active": true, "boltAssemblyId": <M20>, "boltShearForceTransfer": "tensionShearInteraction",
  "connectedItems": [ {"operationId": <EP>, "plateSubIndex": 1}, {"memberId": <COL>, "plateSubIndex": <flange>} ],
  "definedBy": "plate", "isExploded": true,
  "geometry": { "type": "orthogonal", "userPositions": [
      {"x": -0.18, "y": -0.05}, {"x": -0.18, "y": 0.05}, {"x": -0.09, "y": -0.05}, {"x": -0.09, "y": 0.05},
      {"x":  0.09, "y": -0.05}, {"x":  0.09, "y": 0.05}, {"x":  0.18, "y": -0.05}, {"x":  0.18, "y": 0.05} ] } }

GET .../connections/1/export-iom-connection-data         expect 1 entry in boltGrids with 8 positions, the weld entries in welds
```

- `<web>`: 0-based index of the web in the beam's `plates[]` (`beams[]` entry named `BEAM` in `export-iom-connection-data`); for this beam the web is the wall whose `axisZ` runs along global Y, the flanges along Z. Take wall indexes from the export, never assume them.
- `<flange>`: in a grid, a member's `plateSubIndex` counts only the member's walls **parallel to the first item**, so the column's two flanges are 0 and 1. Use the one facing the beam and check in the export that the bolts sit on it — a wrong but valid index still reports success.
- The **first** `connectedItems` entry gives the grid its frame: `userPositions` are in the end plate's local x, y (here x along the beam depth). Positions are not clipped to the parts.
- A plate in `memberLcs` is transverse to the member by default, and `offset.x` runs along the member axis. Its `width`/`width2` run along the section **depth** and `height`/`height2` across its width; each is an extent from the plate origin, so `width` = `width2` centres the plate.
- Then add a load effect (section 7) and calculate.

## 7. Templates, Connection Library, parameters, load effects, materials

### Load effects

```json
POST /api/5/projects/{projectId}/connections/{connectionId}/load-effects/set-load-settings
{ "loadsInEquilibrium": false, "loadsInPercentage": false }

POST /api/5/projects/{projectId}/connections/{connectionId}/load-effects
{ "name": "LE1", "active": true, "isPercentage": false,
  "memberLoadings": [ { "memberId": 2, "position": "End",
      "sectionLoad": { "n": 0, "vy": 0, "vz": -40000, "mx": 0, "my": 25000, "mz": 0 } } ] }
-> 200 the stored ConLoadEffect, with its new "id"

PUT  /api/5/projects/{projectId}/connections/{connectionId}/load-effects
{ "id": 1, "name": "LE1 heavier", "isPercentage": false,
  "memberLoadings": [ { "memberId": 2, "position": "End",
      "sectionLoad": { "n": 0, "vy": 0, "vz": -60000, "mx": 0, "my": 35000, "mz": 0 } } ] }

GET  /api/5/projects/{projectId}/connections/{connectionId}/load-effects?isPercentage=false
```

- Forces in N, moments in N·m. The six components go **inside `sectionLoad`**; flat on the loading they are not read and the POST fails (section 9). A misspelled key inside `sectionLoad` is ignored and stores a zero.
- A loading is a force vector in the member's local axes at the free end of the member's stub. A positive `n` is tension in a stub on the +x side of the node (a member connected by `begin`), compression on the -x side.
- **Author and read in forces.** Send `"isPercentage": false` on POST and PUT, and read with `?isPercentage=false`. The percentage form has known issues (section 9).
- POST stores the loadings you send and, in addition, a zero row for every `(memberId, position)` pair that already exists in the connection from other load effects. The POST response does not show these rows; GET the load effect back. A member can get its first loading in a later load effect.
- `PUT .../load-effects` updates the single load effect named by the body `id` (one object, not an array). It changes only `(memberId, position)` pairs that load effect already has (another is `422`); a member left out keeps its loading. The PUT response echoes what you sent; read the stored state with `GET .../load-effects/{loadEffectId}`. A load effect's own path takes no PUT.
- The first load effect that loads a member fixes the end (`Begin`/`End`) it may be loaded at; reuse the `(memberId, position)` pairs of an existing load effect. A wrong pair is `422` *"… is not defined in connection …"*; an unknown `memberId` is `404` (section 9).
- `DELETE .../load-effects` deletes all of them (new ones are numbered from 1 again); `DELETE .../load-effects/{loadEffectId}` deletes one. `active: false` excludes a load effect from calculations and reports.
- `set-load-settings` belongs to the connection, not to a load effect; set it before authoring load effects. It replaces both flags: send `loadsInEquilibrium` and `loadsInPercentage` every time — `{}` or a misspelled key resets both to `false`. `loadsInEquilibrium` (default `true`) requires the forces to balance; set it to `false` to load only the members that carry the applied forces (the bearing member takes the reactions).

### Parameters

```json
POST .../connections/{connectionId}/parameters
{ "key": "t", "parameterType": "Expression", "expression": "0.02", "valueType": "plateThickness" }
-> { "key": "t", "expression": "0.02", "value": 0.02, "validationStatus": "Ok", ... }

GET  .../connections/{connectionId}/operations/{operationId}/linkable-properties
-> [ { "propertyId": "151", "name": "Thickness", "group": "Stiffening plate", "valueType": "Float", "currentValue": 0.01, ... }, ... ]

POST .../connections/{connectionId}/parameters/links
{ "parameter": "t", "kind": "operation", "operationId": 4, "propertyId": "151" }
-> { "id": 1, "parameter": "t", "owner": { ... }, "propertyId": "151" }

PUT  .../connections/{connectionId}/parameters
[ { "key": "t", "expression": "0.025" } ]
-> { "setToModel": true, "parameters": [ ... ], "failedValidations": [] }
```

- `parameterType` ∈ `Float`, `Bool`, `Int`, `String`, `Expression`; `Expression` reaches every drivable target. `expression` is always a **string**, evaluated before it is stored (a bad one is `422` explaining why). A text value needs inner quotes: `"'S 355'"`. Another parameter is referenced in square brackets: `"[t] * 2"`; units can be stated: `"Length(20,'mm')"`. Create a parameter before the ones that read it.
- `PUT .../parameters` takes an **array** and answers `{setToModel, parameters, failedValidations}`. An unknown key is `422`. There is no PUT per key. Read `failedValidations` — a value the linked property cannot take is reported there.
- A parameter drives nothing until it is linked. `propertyId` is an opaque string, per owner (`"151"` is a stiffening plate's thickness, not a global id) — always read it from `linkable-properties` of that owner (`.../operations/{operationId}/...`, `.../members/{memberId}/...`, or `.../parameters/linkable-properties` for all). `kind` ∈ `operation`, `member`, `crossSection`, `material`, `boltAssembly`, each with its own owner id field. Linking applies the value at once. `DELETE .../parameters/links/{linkId}` unlinks and keeps the last value.
- A linked property belongs to its parameter: writing that property through its own route answers 200 and is overwritten. Change the parameter instead.
- A 200 on a link or PUT does not prove the model changed; read the property back (e.g. plate `thickness` in `export-iom-connection-data`).
- `POST .../connections/{connectionId}/evaluate-expression` takes a JSON string and returns a JSON string (`"1+2*3"` → `"7"`). An unknown property returns 200 with `Property '<name>' not found`; an expression that cannot be evaluated fails (section 9) — create it as a parameter to get a `422` that says why. Syntax: [Expression parameter reference](../api_parameter_reference_guide.md).
- Template parameters that are not visible are listed only with `GET .../parameters?includeHidden=true`.

### Templates and the Connection Library

Apply a library design to a connection whose bearing member has id 1 and attached member id 2:

```json
GET  /api/5/connection-library/get-design-sets
-> [ { "id": "<designSetId>", "name": "...", "description": "...", "ownerId": "...", "type": "Idea" }, ... ]

POST /api/5/projects/{projectId}/connections/{connectionId}/propose
{ "hasBolts": "must", "isMoment": "must" }
-> [ { "name": "...", "designCode": "...", "conDesignSetId": "<guid>", "conDesignItemId": "<guid>", "version": "..." }, ... ]

GET  /api/5/connection-library/get-template?designSetId=<guid>&designItemId=<guid>
-> text/plain, Base64. Decode it: the result is the template XML string used below.

POST /api/5/projects/{projectId}/connections/{connectionId}/get-default-mapping
{ "template": "<template XML>", "memberIds": [1, 2] }
-> { "countryCode": "...", "conversions": [
       { "$type": "IdeaStatiCa.Api.Connection.Model.MemberTemplateConversion, IdeaStatiCa.Api",
         "originalValue": "...", "newValue": "...", "isBearing": true, ... },
       { "$type": "IdeaStatiCa.Api.Connection.Model.CssTemplateConversion, IdeaStatiCa.Api",
         "originalValue": "...", "newValue": "...", ... }, ... ] }

POST /api/5/projects/{projectId}/connections/{connectionId}/apply-template
{ "connectionTemplate": "<template XML>", "mapping": <the get-default-mapping response> }
-> { "appliedWithoutIssues": true, "templateModel": { ... }, "issues": [] }
```

- Note the two field names: `template` in `get-default-mapping`, **`connectionTemplate`** in `apply-template`. Send the mapping back as returned — the entries are matched by position — changing only `newValue` where you want another material or section; every entry keeps its `$type`.
- `memberIds` is positional: index 0 is mapped onto the template's bearing member, 1+ onto its attached members. `[]` maps by member name. Sending only the attached member's id puts it in the bearing slot. To template two attachments of one bearing member, call once per attachment with `[bearingId, attachedId]`.
- `propose` search flags (`hasBolts`, `hasWelds`, `hasAnchor`, `hasClipAngles`, `isMoment`, `isShear`, `isTruss`, `isParametric`) take `ignore` (default), `must`, `mustNot`. `GET /api/5/connection-library/get-picture` (same query) returns the PNG preview.
- **Template of your own connection:** `GET .../connections/{connectionId}/get-template` (text/plain, used as is) or `POST .../connections/{connectionId}/templates/create-from-connection` (metadata plus the same string in `template`). `POST .../connections/{connectionId}/publish` stores a connection in your Private or Company design set.
- A template parameter whose key already exists is added under a suffixed key (e.g. `D_2`).
- `GET .../connections/{connectionId}/templates` lists the applied templates, each with an integer `templateId` that the per-template routes take. `templates/explode` drops the template's parameters and keeps its operations; `DELETE .../templates` removes the operations too.

### Materials and cross-sections

```json
GET  /api/5/projects/{projectId}/materials/steel/library            -> [ "...", "S 355", "..." ]   names this design code accepts
POST /api/5/projects/{projectId}/materials/steel                    { "mprlName": "S 355" }
POST /api/5/projects/{projectId}/materials/cross-sections           { "mprlName": "HEB200", "materialName": "S 355" }
-> { "id": 3, "name": "HEB200", "definition": { ... } }
POST /api/5/projects/{projectId}/materials/bolt-assemblies          { "mprlName": "M16 8.8" }
GET  /api/5/projects/{projectId}/materials/bolt-assemblies          -> [ { "id": 1, "name": "M16 8.8", "diameter": 0.016, ... } ]
```

- Routes are project-scoped: `GET .../materials/steel` (and likewise per type) lists what the project holds; `GET /api/5/projects/{projectId}/materials/{materialType}/library` lists the names the project's design code accepts. Types, spelled exactly: `steel`, `concrete`, `bolt-grade`, `welding`, `headed-stud-grade`, `reinforcement`, `pin`, `cross-sections`, `bolt-assemblies`.
- Add from the library with **`mprlName`**, not `name`. Names match byte for byte (`S 355` resolves, `S355` is `422`; `M16 8.8` needs size and grade); copy them from the library list. The lists differ per design code: `M16 8.8` is `422` on an `american` project.
- Every cross-section POST adds a new entry, even an identical one, and there is no DELETE; keep the `id` it returns. The other material POSTs answer the id of the **oldest** entry with that name, so take ids from the list.
- Material objects come back in SI: `fy` of `S 355` is `355000000.0` (Pa), a bolt `diameter` `0.016` (m).
- `GET .../materials/cross-sections/{cssId}/geometry` returns the outline. Sections from dimensions: `GET .../materials/cross-sections/parametric/shapes/{shapeType}` returns a template that is itself the body for `POST .../materials/cross-sections/parametric` once `materialName` is set. `PUT .../materials/cross-sections/library/{cssId}` swaps a library section in place; members using it follow.
- Pins exist for the `ecen` design code only.

### Design-code conversion, settings, exports

- **Convert a project** to another code: `GET /api/5/projects/{projectId}/get-default-mapping?countryCode=american` returns `ConConversionSettings` (`targetDesignCode`, and lists `steel`, `crossSections`, `fasteners`, `welds`, `concrete`, `boltGrade` of `{sourceValue, targetValue}`). Review every `targetValue` — a name the suggestion table does not know gets one generic value of the target code — then send the object to `POST /api/5/projects/{projectId}/change-code` (`204`). Targets: `american`, `canada`, `australia`, `rus`, `chn`, `india`, `hkg`. `ecen` is never a target (`501`); the project's own code, a pin used by an operation, or (for `change-code`) a reinforcement or headed-stud material is `422`. `steelEdition` resets to the target's default; set it afterwards with `PUT /api/5/projects/{projectId}`.
- **Settings:** `GET /api/5/projects/{projectId}/settings` without `search` returns the whole key → value dictionary (`search` matches a whole key part only). `PUT` takes an object of existing keys, all or nothing, e.g. `{"calculation/Analysis/CodeFactors/EN/GammaM0": 1.0}`.
- **Exports:** `export-iom` (XML; decode it as section 9 says; the export is the input of `POST /api/5/projects/import-iom-file`, field `containerXmlFile`), `export-iom-connection-data` (JSON, or XML with `Accept: application/xml`), `export-ifc`, `export-dwg` (can take up to about 10 minutes).

## 8. Where the swagger is not enough

The swagger of the running service names every route and field. The rules below are what it does not tell you; each one fails quietly when ignored. Where the service and its swagger disagree, section 9 says which one to follow.

### Behaviour the swagger does not declare

- **Send a `ClientId` header on every request** except `GET /api/5/clients/connect-client` (which returns the id) and `GET /api/5/clients/idea-service-version`. The header is part of the contract even where the swagger does not declare it (section 9).
- **Send only what the schemas list.** Most schemas are marked `additionalProperties: false` and almost no field `required`. The service does not enforce either today: unknown fields are ignored and missing ones get defaults (section 2, rule 8). Do not depend on that, and read back what you wrote.
- **Read the status code first, then the body.** An error body is ProblemDetails-shaped JSON (`title`, `status`, `detail`; a `400` from the binder adds an `errors` map). Parse it whatever its media type says, and allow an empty body (section 9).
- **Count what an operation created** in `GET .../connections/{connectionId}/export-iom-connection-data` after every add; a clean `addedWithoutIssues: true` is not proof (section 9).
- **`POST /api/5/projects` creates a project with one empty connection** (`CON1`, id 1). Use it, or read the connection list; do not add a first connection on the assumption that there is none.
- **Close only the projects your ClientId opened.**

### Names and index bases that differ between schemas

- **Library items are created by `mprlName`** (`ConMprlCrossSection`, `ConMprlElement`). The objects a GET returns carry `name` instead; do not post a GET result back to a create route.
- **Axes are spelled two ways:** a grid's `coordinateSystem` (`ConLocalCoordinateSystem`) takes `xAxis` / `yAxis` / `zAxis`; a member's `position` (`ConMemberPosition`) and an added member's `positioning` (`ConAddedMemberPositioning`) take `axisX` / `axisY` / `axisZ`.
- **Plate indexes on a member start at different numbers:** `onMemberPlateIndex` (added member) counts from 0, `plateOnMemberIndex` (plate positioned on a member) counts from 1.
- **`plateSubIndex` depends on the operand** (`ConConnectedItem`): 0-based in a weld's member operand, 1-based in a weld's operation operand and in a cut operand.

## 9. Known issues

Defects of the 26.1.1 release. Each entry gives what happens and what to do instead. Code that follows the workarounds also works against a service where the defect is fixed.

1. **The swagger does not declare the `ClientId` header.** There is neither a header parameter nor a security scheme, so a client generated from the 26.1.1 swagger, or code written from it alone, sends no `ClientId`, and every call answers `401`. **Do:** add the header to every request after `connect-client` (section 2, rule 1). The official C# and Python clients add it themselves.
2. **Error media types and bodies differ from the swagger.** The swagger declares most error responses as `application/json` (some as `text/plain`, `text/json` or `application/xml`). The service sends `404`, `409`, `422` and `500` as `application/problem+json`, and `400`, `401` and `415` as `application/json`. A `405`, and the `415` of `projects/open` when the body is not `multipart/form-data`, come with no body at all. **Do:** branch on the status code, parse the body as JSON whatever the media type, and allow an empty body.
3. **`close` closes a project that another ClientId opened.** It answers `204` and the project is gone for its owner too. After that, the owner's `GET /api/5/projects` answers `404` for as long as the service runs. The `404` that the swagger declares for `close` does not occur. **Do:** close only the projects your ClientId opened, and use one ClientId per process.
4. **The summary of `POST /api/5/projects` says "with no connections".** The new project holds one empty connection, `CON1` with id 1. **Do:** build in `CON1`, or read `GET /api/5/projects/{projectId}/connections` before adding connections.
5. **Load effects in percentage are not reliable.**
   - `GET .../load-effects?isPercentage=true` returns the forces in N and N·m, flagged `"isPercentage": true`. Without the query parameter, the flag follows the connection's `loadsInPercentage` setting; the values are forces either way.
   - A POST with `"isPercentage": true` stores wrong forces and drops `mx`.

   **Do:** author with `"isPercentage": false` in N and N·m on POST and PUT, and read with `?isPercentage=false`.
6. **`addedWithoutIssues: true` does not prove geometry.** A bolt, anchor or pin grid with an empty `geometry.userPositions` list is a clean success that creates no fasteners. Values are not range-checked: a plate `thickness` of 0 or below is accepted with `addedWithoutIssues: true`. **Do:** count what you expected in `GET .../connections/{connectionId}/export-iom-connection-data` after every add.
7. **Some client errors answer `500` with an internal message:**
   - `projects/open` with a file that is not a valid `.ideaCon`;
   - a load-effect POST with the forces flat on the loading instead of inside `sectionLoad`;
   - `evaluate-expression` with an expression that cannot be evaluated.

   **Do:** treat these `500`s as a fault in your request, and do not quote or parse their `detail`. To find out why an expression fails, create it as a parameter; that answers `422` with the reason.
8. **A `404` for an unknown `memberId` in a load-effect body does not name the member.** Its `detail` is an internal message. **Do:** take member ids from `GET .../connections/{connectionId}/members`.
9. **`export-iom` declares the wrong encoding.** The XML declaration says `utf-16`, but the bytes are UTF-8. **Do:** decode by the bytes, not by the declaration: UTF-16 when they start with a byte-order mark (`FF FE` or `FE FF`), UTF-8 otherwise. Before sending the export to `POST /api/5/projects/import-iom-file`, make the bytes match the encoding the declaration names (for 26.1.1: re-encode to UTF-16).

## 10. Minimal example (Python, `requests` only)

```python
import requests

BASE = "http://localhost:5000/api/5"
s = requests.Session()

requests.get("http://localhost:5000/heartbeat", timeout=5).raise_for_status()   # service must already run

client_id = s.get(f"{BASE}/clients/connect-client").text.strip().strip('"')     # text/plain, not JSON
s.headers["ClientId"] = client_id

with open(r"C:\projects\my-connection.ideaCon", "rb") as f:
    r = s.post(f"{BASE}/projects/open", files={"ideaConFile": ("model.ideaCon", f, "application/octet-stream")})
r.raise_for_status()
project = r.json()
pid = project["projectId"]
ids = [c["id"] for c in project["connections"]]

try:
    r = s.post(f"{BASE}/projects/{pid}/connections/calculate", json=ids, timeout=3600)
    r.raise_for_status()
    for con in r.json():
        print(con["id"], "PASS" if con["passed"] else "FAIL")
        for row in con["resultSummary"]:
            if not row.get("skipped"):
                print("   ", row["name"], row["unityCheckMessage"])

    r = s.post(f"{BASE}/projects/{pid}/connections/results", json=ids)
    r.raise_for_status()
    for res in r.json():
        for bolt in res.get("checkResBolt") or []:
            print("   ", bolt["name"], bolt["unityCheck"], "%")

    r = s.get(f"{BASE}/projects/{pid}/download")
    r.raise_for_status()
    with open(r"C:\projects\my-connection-calculated.ideaCon", "wb") as f:
        f.write(r.content)
finally:
    s.get(f"{BASE}/projects/{pid}/close")
```

## 11. Mistakes to avoid

| Wrong | Right |
|---|---|
| Starting the service without checking | `GET /heartbeat` first; start the exe only if nothing answers |
| `-port=5193` | `-port:5193` |
| `/swagger/index.html`, `/openapi.json` | `/index.html`, `/swagger/5.0/swagger.json` |
| `resp.json()` on `connect-client` | Read the body as text and strip it |
| No `ClientId` header (generated client, plain HTTP library) | Add `ClientId` to every request after `connect-client` |
| Multipart field `idea_con_file` or `file` | `ideaConFile` |
| `{"connectionIds": [1]}` or `1` as calculate body | `[1]` |
| Millimetres or kN in a body | Metres and newtons (`0.005`, `300000`) |
| Trusting `200` from an operation POST | Check `addedWithoutIssues` and `issues[]`, then count in `export-iom-connection-data` |
| PUT with the operation id in the URL | `PUT .../operations/weld` with `"id": 7` in the body |
| PUT of a hand-written or trimmed operation body | GET the operation, change one field, PUT the whole object back |
| Deleting one operation | Not possible; `DELETE .../operations` removes all of them, templates and parameters included |
| Assuming a member's wall order (`plateSubIndex`) | Read the walls from `export-iom-connection-data` |
| `"name": "HEB200"` on a cross-section POST | `"mprlName": "HEB200"`, `"materialName": "S 355"` |
| `"S355"`, `"ipe300"`, `"M16"` | Exact library spelling: `"S 355"`, `"IPE300"`, `"M16 8.8"` — copy from `.../library` |
| `n`, `vz`, `my` directly on a member loading | Inside `"sectionLoad": {...}` |
| `"isPercentage": true`, or `GET .../load-effects` without `?isPercentage=false` | Author and read with `isPercentage` false (section 9) |
| Closing a project id you did not open | Close only your own projects; it can close the project for its owner too (section 9) |
| `set-load-settings` with one flag or `{}` | Send both `loadsInEquilibrium` and `loadsInPercentage` |
| `PUT .../connections/{connectionId}` with only the changed field | GET, change, PUT the whole object; omitted fields are reset |
| Calling `results` without `calculate` | `calculate` first; empty lists mean "not calculated" |
| Multiplying `unityCheck` or `checkValue` by 100 | They are already in % (plates: % plastic strain, see section 4) |
| `float(row["unityCheck"])` on every weld | Guard the string `"NaN"` (full-strength welds) |
| Passing the library `get-template` body straight to `apply-template` | Base64-decode it first |
| `template` in the `apply-template` body | `connectionTemplate` |
| `"expression": 15`, `"expression": "t * 2"` | `"expression": "15"`, `"expression": "[t] * 2"` |
| Guessing a `propertyId` | Read it from `.../linkable-properties` of that owner |
| Converting only `ecen` projects | Any code converts to any listed target except `ecen` |

**Common errors**

| Symptom | Cause |
|---|---|
| Connection refused | Service not running, or another port |
| `401` "The 'ClientId' header is required" on every call | `ClientId` header missing or misspelled |
| `400` with an `errors` map | Body does not bind (wrong enum value, wrong nesting). Ignore the entry `"request": ["The request field is required."]`; the other key names the cause |
| `404` on a project id | Project closed, or opened by another ClientId |
| `404` on `calculate` / `results` | An unknown connection id in the array; nothing was calculated |
| `404` on a load-effect POST | Unknown `memberId` in the body (section 9) |
| `409` | Operation id of another kind, or a second async job on the project (a synchronous calculate waits instead) |
| `415` on `open` (the body can be empty, section 9) | JSON body instead of `multipart/form-data` |
| `405` (the body can be empty, section 9) | Wrong method for the route, e.g. PUT with the operation id in the path |
| `422` | Validation; the `detail` names the field, id or name that failed (a report for an unknown connection id is `422` too) |
| `500` on `open` | The file is not a valid `.ideaCon` (section 9) |
| `500` on a load-effect POST | Forces placed flat on the loading instead of inside `sectionLoad` (section 9) |
| `500` on `evaluate-expression` | The expression cannot be evaluated; create it as a parameter to get a `422` that says why (section 9) |
| `passed: false`, Analysis row "There are no loads on members" | No active load effect with forces |

## 12. Route map

The routes of API 5.0 as released in 26.1.1, relative to `/api/5`. A line with several methods lists one route per method. A later service can add routes; its OpenAPI document is the complete list (command at the end of this section).

**Client**

```
GET  /clients/connect-client
GET  /clients/idea-service-version
```

**Projects**

```
POST      /projects/open
GET       /projects/{projectId}/close
GET POST  /projects
GET       /projects/{projectId}/download
GET PUT   /projects/{projectId}
POST      /projects/import-iom-file
POST      /projects/{projectId}/update-iom-file
```

**Connections**

```
GET POST        /projects/{projectId}/connections
GET PUT DELETE  /projects/{projectId}/connections/{connectionId}
POST            /projects/{projectId}/connections/{connectionId}/copy
GET             /projects/{projectId}/connections/{connectionId}/production-cost
GET             /projects/{projectId}/connections/{connectionId}/get-topology
```

**Members**

```
GET POST PUT  /projects/{projectId}/connections/{connectionId}/members
GET DELETE    /projects/{projectId}/connections/{connectionId}/members/{memberId}
PUT           /projects/{projectId}/connections/{connectionId}/members/{memberId}/set-bearing-member
```

**Materials and cross-sections**

```
GET       /projects/{projectId}/materials
GET POST  /projects/{projectId}/materials/steel
GET POST  /projects/{projectId}/materials/concrete
GET POST  /projects/{projectId}/materials/bolt-grade
GET POST  /projects/{projectId}/materials/welding
GET POST  /projects/{projectId}/materials/headed-stud-grade
GET POST  /projects/{projectId}/materials/reinforcement
GET POST  /projects/{projectId}/materials/cross-sections
GET       /projects/{projectId}/materials/cross-sections/{cssId}
GET       /projects/{projectId}/materials/cross-sections/{cssId}/geometry
GET POST  /projects/{projectId}/materials/bolt-assemblies
GET       /projects/{projectId}/materials/{materialType}/library
POST      /projects/{projectId}/materials/cross-sections/parametric
PUT       /projects/{projectId}/materials/cross-sections/parametric/{cssId}
PUT       /projects/{projectId}/materials/cross-sections/library/{cssId}
GET       /projects/{projectId}/materials/cross-sections/parametric/shapes
GET       /projects/{projectId}/materials/cross-sections/parametric/shapes/{shapeType}
GET POST  /projects/{projectId}/materials/pin
```

**Operations**

```
GET DELETE    /projects/{projectId}/connections/{connectionId}/operations
GET PUT       /projects/{projectId}/connections/{connectionId}/operations/common-properties
POST          /projects/{projectId}/connections/{connectionId}/operations/weld-sizing
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/weld
GET           /projects/{projectId}/connections/{connectionId}/operations/weld/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/cut
GET           /projects/{projectId}/connections/{connectionId}/operations/cut/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/plate-cut
GET           /projects/{projectId}/connections/{connectionId}/operations/plate-cut/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/bolt-grid
GET           /projects/{projectId}/connections/{connectionId}/operations/bolt-grid/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/anchor-grid
GET           /projects/{projectId}/connections/{connectionId}/operations/anchor-grid/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/pin-grid
GET           /projects/{projectId}/connections/{connectionId}/operations/pin-grid/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/contact
GET           /projects/{projectId}/connections/{connectionId}/operations/contact/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/contact-grid
GET           /projects/{projectId}/connections/{connectionId}/operations/contact-grid/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/negative-member
GET           /projects/{projectId}/connections/{connectionId}/operations/negative-member/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/negative-plate
GET           /projects/{projectId}/connections/{connectionId}/operations/negative-plate/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/stiffening-member
GET           /projects/{projectId}/connections/{connectionId}/operations/stiffening-member/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/stiffening-plate
GET           /projects/{projectId}/connections/{connectionId}/operations/stiffening-plate/{operationId}
GET POST PUT  /projects/{projectId}/connections/{connectionId}/operations/work-plane
GET           /projects/{projectId}/connections/{connectionId}/operations/work-plane/{operationId}
```

**Load effects**

```
GET POST PUT DELETE  /projects/{projectId}/connections/{connectionId}/load-effects
GET DELETE           /projects/{projectId}/connections/{connectionId}/load-effects/{loadEffectId}
GET                  /projects/{projectId}/connections/{connectionId}/load-effects/get-load-settings
POST                 /projects/{projectId}/connections/{connectionId}/load-effects/set-load-settings
POST                 /projects/{projectId}/connections/{connectionId}/load-effects/calculate-load-extremes
```

**Calculation and results**

```
POST  /projects/{projectId}/connections/calculate
POST  /projects/{projectId}/connections/{connectionId}/calculate
POST  /projects/{projectId}/connections/results
POST  /projects/{projectId}/connections/rawresults-text
POST  /projects/{projectId}/connections/{connectionId}/rawresults-text
```

**Asynchronous calculation**

```
POST        /projects/{projectId}/connections/calculate-async
POST        /projects/{projectId}/connections/{connectionId}/calculate-async
GET DELETE  /projects/{projectId}/calculation-jobs/{jobId}
```

**Reports**

```
POST  /projects/{projectId}/reports/pdf
POST  /projects/{projectId}/reports/word
GET   /projects/{projectId}/connections/{connectionId}/reports/pdf
GET   /projects/{projectId}/connections/{connectionId}/reports/word
GET   /projects/{projectId}/connections/{connectionId}/reports/htmlZip
```

**Exports**

```
GET  /projects/{projectId}/connections/{connectionId}/export-iom
GET  /projects/{projectId}/connections/{connectionId}/export-iom-connection-data
GET  /projects/{projectId}/connections/{connectionId}/export-ifc
GET  /projects/{projectId}/connections/{connectionId}/export-dwg
```

**3D scene data**

```
GET  /projects/{projectId}/connections/{connectionId}/presentations
GET  /projects/{projectId}/connections/{connectionId}/presentations/text
```

**Parameters**

```
GET POST PUT DELETE  /projects/{projectId}/connections/{connectionId}/parameters
DELETE               /projects/{projectId}/connections/{connectionId}/parameters/{key}
GET POST             /projects/{projectId}/connections/{connectionId}/parameters/links
DELETE               /projects/{projectId}/connections/{connectionId}/parameters/links/{linkId}
GET                  /projects/{projectId}/connections/{connectionId}/members/{memberId}/linkable-properties
GET                  /projects/{projectId}/connections/{connectionId}/operations/{operationId}/linkable-properties
GET                  /projects/{projectId}/connections/{connectionId}/parameters/linkable-properties
POST                 /projects/{projectId}/connections/{connectionId}/evaluate-expression
```

**Templates**

```
GET         /projects/{projectId}/connections/{connectionId}/templates/{templateInstanceId}
GET DELETE  /projects/{projectId}/connections/{connectionId}/templates
GET         /projects/{projectId}/connections/{connectionId}/get-template
POST        /projects/{projectId}/connections/{connectionId}/templates/create-from-connection
POST        /projects/{projectId}/connections/{connectionId}/get-default-mapping
POST        /projects/{projectId}/connections/{connectionId}/apply-template
POST        /projects/{projectId}/connections/{connectionId}/templates/explode
POST        /projects/{projectId}/connections/{connectionId}/templates/{templateId}/explode
DELETE      /projects/{projectId}/connections/{connectionId}/templates/{templateId}
GET PUT     /projects/{projectId}/connections/{connectionId}/templates/{templateId}/common-properties
POST        /projects/{projectId}/connections/{connectionId}/templates/{templateId}/load-defaults
```

**Connection Library**

```
GET   /connection-library/get-design-sets
POST  /projects/{projectId}/connections/{connectionId}/propose
GET   /connection-library/get-template
GET   /connection-library/get-picture
POST  /projects/{projectId}/connections/{connectionId}/publish
```

**Design-code conversion**

```
GET   /projects/{projectId}/get-default-mapping
POST  /projects/{projectId}/change-code
```

**Settings**

```
GET PUT  /projects/{projectId}/settings
```

The running service's OpenAPI document is the complete list for the service you talk to, with the request and response schema of each route. Print its routes with one command:

```console
curl -s http://localhost:5000/swagger/5.0/swagger.json | python -c "import json,sys; [print(m.upper(), p) for p, ops in json.load(sys.stdin)['paths'].items() for m in ops]"
```

Then search the same document for the path you picked to get its request and response schemas.

## 13. Clients and how to use this page with an AI assistant

The generated C# and Python clients mirror this OpenAPI one to one: [Python client](clients/python/README.md), [C# client](clients/csharp/README.md).

Give this page to the assistant (paste it, or point the tool at its URL) before asking for Connection API code. Ask it to check that the service runs before starting one, to take route and field names from the running service's swagger, to use this page for the behaviour the swagger does not state, and to say so when something is not available rather than inventing a route or field. Report problems and missing functionality in [GitHub Discussions](https://github.com/idea-statica/ideastatica-public/discussions).
