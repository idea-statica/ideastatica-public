# Connection API changelog

Changes in the Connection REST API between releases. Each section compares the OpenAPI description of a release with the previous one.

<!-- Release sections start here; the newest is first. -->

## 26.1.1

Compared with 26.1.0. API version 5.0. Paths are shown without the `/api/5` prefix.

### Breaking changes

#### `DELETE /projects/{projectId}/connections/{connectionId}/templates/{templateId}`

- for the `path` request parameter `templateId`, the `type/format` was changed from `string/uuid` to `integer/int32`

#### `GET /projects/{projectId}/connections/{connectionId}/templates/{templateId}/common-properties`

- for the `path` request parameter `templateId`, the `type/format` was changed from `string/uuid` to `integer/int32`

#### `PUT /projects/{projectId}/connections/{connectionId}/templates/{templateId}/common-properties`

- for the `path` request parameter `templateId`, the `type/format` was changed from `string/uuid` to `integer/int32`

#### `POST /projects/{projectId}/connections/{connectionId}/templates/{templateId}/explode`

- for the `path` request parameter `templateId`, the `type/format` was changed from `string/uuid` to `integer/int32`

#### `POST /projects/{projectId}/connections/{connectionId}/templates/{templateId}/load-defaults`

- for the `path` request parameter `templateId`, the `type/format` was changed from `string/uuid` to `integer/int32`

### Other changes

#### `POST /projects/{projectId}/connections/{connectionId}/operations/anchor-grid`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/anchor-grid`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/anchor-grid/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/bolt-grid`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/bolt-grid`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/bolt-grid/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/contact`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/contact`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/contact-grid`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/contact-grid`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/contact-grid/{operationId}`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/contact/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/cut`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/cut`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/cut/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/negative-member`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/negative-member`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/negative-member/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/negative-plate`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/negative-plate`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/negative-plate/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/pin-grid`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/pin-grid`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/pin-grid/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/plate-cut`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/plate-cut`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/plate-cut/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/stiffening-member`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/stiffening-member`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/stiffening-member/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/stiffening-plate`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/stiffening-plate`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/stiffening-plate/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/weld`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/weld`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/weld/{operationId}`

- added the non-success response with the status `409`

#### `POST /projects/{projectId}/connections/{connectionId}/operations/work-plane`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/work-plane`

- added the non-success response with the status `409`

#### `GET /projects/{projectId}/connections/{connectionId}/operations/work-plane/{operationId}`

- added the non-success response with the status `409`

#### `PUT /projects/{projectId}/materials/cross-sections/library/{cssId}`

- endpoint added

## 26.1.0

Compared with 26.0.5. API version 4.0 to 5.0. Paths are shown without the `/api/5` prefix.

### Breaking changes

#### `POST /projects/{projectId}/connections/{connectionId}/apply-template`

- the request property `connectionTemplate` became not nullable
- the request property `mapping/conversions` became not nullable
- the request property `connectionTemplate` became required
- the request property `mapping` became required
- the request property `mapping/conversions` became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/BoltAssemblyTemplateConversion]/allOf[subschema #2]/$type` with a default value became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/BoltGradeTemplateConversion]/allOf[subschema #2]/$type` with a default value became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/CleatTemplateConversion]/allOf[subschema #2]/$type` with a default value became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/ConcreteTemplateConversion]/allOf[subschema #2]/$type` with a default value became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/CssTemplateConversion]/allOf[subschema #2]/$type` with a default value became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/ElectrodeTemplateConversion]/allOf[subschema #2]/$type` with a default value became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/MemberTemplateConversion]/allOf[subschema #2]/$type` with a default value became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/PinTemplateConversion]/allOf[subschema #2]/$type` with a default value became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/PlateMaterialTemplateConversion]/allOf[subschema #2]/$type` with a default value became required
- the request property `mapping/conversions/items/oneOf[#/components/schemas/TimberTemplateConversion]/allOf[subschema #2]/$type` with a default value became required

#### `GET /projects/{projectId}/connections/{connectionId}/export-iom-connection-data`

- the `beams/items/plates/items/material` response's property `type` changed from `string` to `object` for status `200` (shared schema: `PlateData`, also at `foldedPlates/items/plates/items/material`, `plates/items/material`)

### Other changes

#### Schemas

- removed the schema `ConcreteBlock`

#### `GET /projects`

- added the optional property `items/connections/items/steelEdition` to the response with the `200` status
- added the optional property `items/projectInfo/steelEdition` to the response with the `200` status
- removed the optional property `items/connections/items/isCalculated` from the response with the `200` status

#### `POST /projects`

- added the new optional request property `steelEdition`
- added the optional property `connections/items/steelEdition` to the response with the `200` status
- added the optional property `projectInfo/steelEdition` to the response with the `200` status
- removed the optional property `connections/items/isCalculated` from the response with the `200` status

#### `POST /projects/import-iom-file`

- added the optional property `connections/items/steelEdition` to the response with the `200` status
- added the optional property `projectInfo/steelEdition` to the response with the `200` status
- removed the optional property `connections/items/isCalculated` from the response with the `200` status

#### `POST /projects/open`

- added the optional property `connections/items/steelEdition` to the response with the `200` status
- added the optional property `projectInfo/steelEdition` to the response with the `200` status
- removed the optional property `connections/items/isCalculated` from the response with the `200` status

#### `GET /projects/{projectId}`

- added the optional property `connections/items/steelEdition` to the response with the `200` status
- added the optional property `projectInfo/steelEdition` to the response with the `200` status
- removed the optional property `connections/items/isCalculated` from the response with the `200` status

#### `PUT /projects/{projectId}`

- added the new optional request property `steelEdition`
- added the optional property `connections/items/steelEdition` to the response with the `200` status
- added the optional property `projectInfo/steelEdition` to the response with the `200` status
- removed the optional property `connections/items/isCalculated` from the response with the `200` status

#### `GET /projects/{projectId}/connections`

- added the optional property `items/steelEdition` to the response with the `200` status
- removed the optional property `items/isCalculated` from the response with the `200` status

#### `POST /projects/{projectId}/connections`

- added the optional property `steelEdition` to the response with the `200` status
- removed the optional property `isCalculated` from the response with the `200` status

#### `POST /projects/{projectId}/connections/results`

- added the optional property `items/checkResAnchor/items/loadCaseId` to the response with the `200` status
- added the optional property `items/checkResBolt/items/loadCaseId` to the response with the `200` status

#### `DELETE /projects/{projectId}/connections/{connectionId}`

- added the optional property `items/steelEdition` to the response with the `200` status
- removed the optional property `items/isCalculated` from the response with the `200` status

#### `GET /projects/{projectId}/connections/{connectionId}`

- added the optional property `steelEdition` to the response with the `200` status
- removed the optional property `isCalculated` from the response with the `200` status

#### `PUT /projects/{projectId}/connections/{connectionId}`

- added the new optional request property `steelEdition`
- added the optional property `steelEdition` to the response with the `200` status
- removed the optional property `isCalculated` from the response with the `200` status

#### `POST /projects/{projectId}/connections/{connectionId}/copy`

- added the optional property `steelEdition` to the response with the `200` status
- removed the optional property `isCalculated` from the response with the `200` status

#### `GET /projects/{projectId}/connections/{connectionId}/export-iom-connection-data`

- added the optional property `anchorGrids/items/concreteBlock/element` to the response with the `200` status
- added the optional property `anchorGrids/items/concreteBlock/id` to the response with the `200` status
- added the optional property `anchorGrids/items/concreteBlock/typeName` to the response with the `200` status
- added the optional property `anchorGrids/items/originalModelId` to the response with the `200` status
- added the optional property `beams/items/plates/items/material/element` to the response with the `200` status (shared schema: `PlateData`, also at `foldedPlates/items/plates/items/material/element`, `plates/items/material/element`)
- added the optional property `beams/items/plates/items/material/id` to the response with the `200` status (shared schema: `PlateData`, also at `foldedPlates/items/plates/items/material/id`, `plates/items/material/id`)
- added the optional property `beams/items/plates/items/material/typeName` to the response with the `200` status (shared schema: `PlateData`, also at `foldedPlates/items/plates/items/material/typeName`, `plates/items/material/typeName`)
- added the optional property `boltGrids/items/originalModelId` to the response with the `200` status
- added the optional property `pinGrids/items/originalModelId` to the response with the `200` status
- removed the optional property `anchorGrids/items/concreteBlock/height` from the response with the `200` status
- removed the optional property `anchorGrids/items/concreteBlock/lenght` from the response with the `200` status
- removed the optional property `anchorGrids/items/concreteBlock/material` from the response with the `200` status
- removed the optional property `anchorGrids/items/concreteBlock/width` from the response with the `200` status
- the response property `beams/items/plates/items/material` became not nullable for the status `200` (shared schema: `PlateData`, also at `foldedPlates/items/plates/items/material`, `plates/items/material`)
- the `beams/items/plates/items/material` response's property `type` changed from `string` to `object` for status `200` (backward compatible) (shared schema: `PlateData`, also at `foldedPlates/items/plates/items/material`, `plates/items/material`)

#### `POST /projects/{projectId}/connections/{connectionId}/get-default-mapping`

- the response property `conversions` became not nullable for the status `200`
- the response property `conversions` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/BoltAssemblyTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/BoltGradeTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/CleatTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/ConcreteTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/CssTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/ElectrodeTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/MemberTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/PinTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/PlateMaterialTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`
- the response property `conversions/items/oneOf[#/components/schemas/TimberTemplateConversion]/allOf[subschema #2]/$type` became required for the status `200`

#### `DELETE /projects/{projectId}/connections/{connectionId}/load-effects`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/members`

- added the optional property `items/position/alignedPlate/partTypeSequence` to the response with the `200` status (shared schema: `ConAlignedPlate`, also at `items/position/relatedPlate/partTypeSequence`)

#### `POST /projects/{projectId}/connections/{connectionId}/members`

- added the new optional request property `position/alignedPlate/partTypeSequence` (shared schema: `ConAlignedPlate`, also at `position/relatedPlate/partTypeSequence`)
- added the optional property `position/alignedPlate/partTypeSequence` to the response with the `200` status (shared schema: `ConAlignedPlate`, also at `position/relatedPlate/partTypeSequence`)

#### `PUT /projects/{projectId}/connections/{connectionId}/members`

- added the new optional request property `position/alignedPlate/partTypeSequence` (shared schema: `ConAlignedPlate`, also at `position/relatedPlate/partTypeSequence`)
- added the optional property `position/alignedPlate/partTypeSequence` to the response with the `200` status (shared schema: `ConAlignedPlate`, also at `position/relatedPlate/partTypeSequence`)

#### `GET /projects/{projectId}/connections/{connectionId}/members/{memberId}`

- added the optional property `position/alignedPlate/partTypeSequence` to the response with the `200` status (shared schema: `ConAlignedPlate`, also at `position/relatedPlate/partTypeSequence`)

#### `GET /projects/{projectId}/connections/{connectionId}/members/{memberId}/linkable-properties`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/members/{memberId}/set-bearing-member`

- added the optional property `position/alignedPlate/partTypeSequence` to the response with the `200` status (shared schema: `ConAlignedPlate`, also at `position/relatedPlate/partTypeSequence`)

#### `GET /projects/{projectId}/connections/{connectionId}/operations/anchor-grid`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/anchor-grid`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/anchor-grid`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/anchor-grid/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/bolt-grid`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/bolt-grid`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/bolt-grid`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/bolt-grid/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/contact`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/contact`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/contact`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/contact-grid`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/contact-grid`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/contact-grid`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/contact-grid/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/contact/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/cut`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/cut`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/cut`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/cut/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/negative-member`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/negative-member`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/negative-member`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/negative-member/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/negative-plate`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/negative-plate`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/negative-plate`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/negative-plate/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/pin-grid`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/pin-grid`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/pin-grid`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/pin-grid/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/plate-cut`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/plate-cut`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/plate-cut`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/plate-cut/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/stiffening-member`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/stiffening-member`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/stiffening-member`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/stiffening-member/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/stiffening-plate`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/stiffening-plate`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/stiffening-plate`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/stiffening-plate/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/weld`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/weld`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/weld`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/weld/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/work-plane`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/operations/work-plane`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/operations/work-plane`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/work-plane/{operationId}`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/operations/{operationId}/linkable-properties`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/parameters`

- added the optional property `items/valueType` to the response with the `200` status

#### `POST /projects/{projectId}/connections/{connectionId}/parameters`

- endpoint added

#### `PUT /projects/{projectId}/connections/{connectionId}/parameters`

- added the new optional request property `items/description`
- added the new optional request property `items/isVisible`
- added the new optional request property `items/valueType`
- added the optional property `parameters/items/valueType` to the response with the `200` status

#### `GET /projects/{projectId}/connections/{connectionId}/parameters/linkable-properties`

- endpoint added

#### `GET /projects/{projectId}/connections/{connectionId}/parameters/links`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/parameters/links`

- endpoint added

#### `DELETE /projects/{projectId}/connections/{connectionId}/parameters/links/{linkId}`

- endpoint added

#### `DELETE /projects/{projectId}/connections/{connectionId}/parameters/{key}`

- endpoint added

#### `POST /projects/{projectId}/connections/{connectionId}/templates/{templateId}/load-defaults`

- added the optional property `parameters/items/valueType` to the response with the `200` status

#### `GET /projects/{projectId}/get-default-mapping`

- added the non-success response with the status `501`

#### `GET /projects/{projectId}/materials/cross-sections`

- added the optional property `items/definition` to the response with the `200` status
- added the optional property `items/id` to the response with the `200` status
- added the optional property `items/name` to the response with the `200` status
- the `items/` response's property `type` was narrowed from `any` to `object` for status `200`

#### `POST /projects/{projectId}/materials/cross-sections`

- the response's body `type` was narrowed from `any` to `object` for status `200`
- added the optional property `definition` to the response with the `200` status
- added the optional property `id` to the response with the `200` status
- added the optional property `name` to the response with the `200` status

#### `POST /projects/{projectId}/materials/cross-sections/parametric`

- endpoint added

#### `GET /projects/{projectId}/materials/cross-sections/parametric/shapes`

- endpoint added

#### `GET /projects/{projectId}/materials/cross-sections/parametric/shapes/{shapeType}`

- endpoint added

#### `PUT /projects/{projectId}/materials/cross-sections/parametric/{cssId}`

- endpoint added

#### `GET /projects/{projectId}/materials/cross-sections/{cssId}`

- endpoint added

#### `GET /projects/{projectId}/materials/cross-sections/{cssId}/geometry`

- endpoint added

#### `POST /projects/{projectId}/update-iom-file`

- added the optional property `connections/items/steelEdition` to the response with the `200` status
- added the optional property `projectInfo/steelEdition` to the response with the `200` status
- removed the optional property `connections/items/isCalculated` from the response with the `200` status

### Known issues

- The OpenAPI description of 26.1.0 lists the IOM types `CutData`, `CutBeamByBeamData`, `ConcreteBlockData` and `WeldData` with `id` only. The service still returns all their properties; only their description is missing. Clients generated from this description may drop these properties.
