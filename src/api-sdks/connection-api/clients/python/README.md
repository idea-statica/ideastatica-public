# ideastatica-connection-api

The Python package for the Connection Rest API 1.0

- API version: 1.0
- Package version: 24.1.2.1474

IDEA StatiCa Connection API, used for the automated design and calculation of steel connections.

## Requirements.

Python 3.7+

## Installation

### pip install 

We reccomend using pip to install the package into your environment.

```sh
pip install ideastatica_connection_api
```

Then import the package in your project:
```python
import ideastatica_connection_api
```

If the python package is hosted on a repository, you can install directly using:

```sh
pip install git+https://github.com/GIT_USER_ID/GIT_REPO_ID.git
```
(you may need to run `pip` with root permission: `sudo pip install git+https://github.com/GIT_USER_ID/GIT_REPO_ID.git`)

### Setuptools

Install via [Setuptools](http://pypi.python.org/pypi/setuptools).

```sh
python setup.py install --user
```
(or `sudo python setup.py install` to install the package for all users)

<a id="usage"></a>
## Usage

`ClientApiClientFactory` manages creation of clients on the running service. 
We currently only support connecting to a service running on a localhost (eg. 'http://localhost:5000/').

To start the service, manually navigate to the "C:\Program Files\IDEA StatiCa\StatiCa 24.0\net6.0-windows" folder. Using CLI:

```console
IdeaStatiCa.ConnectionRestApi.exe -port:5193
```

```python
// Connect any new service to latest version of IDEA StatiCa.
client_factory = ConnectionApiClientFactory('http://localhost:5000/')
```

```python
conClient = client_factory.create_connection_api_client();
```

## Getting Started

Please follow the [installation procedure](#installation--usage) and then run the following:


<a id="documentation-for-api-endpoints"></a>
## Documentation for API Endpoints

The `ConnectionApiClient` wraps all API endpoing controllers into object based or action baseds API endpoints.

Methods marked with an **^** denote that they have an additional extension in the Client.

  ### CalculationApi

  
  
  Method | Description
  ------------- | -------------
[**calculate**](docs/CalculationApi.md#calculate) | Run CBFEM caluclation and return the summary of the results
[**get_raw_json_results**](docs/CalculationApi.md#get_raw_json_results) | Get json string which represents raw CBFEM results (an instance of CheckResultsData)
[**get_results**](docs/CalculationApi.md#get_results) | Get detailed results of the CBFEM analysis
  ### ClientApi

  
  
  Method | Description
  ------------- | -------------
[**connect_client**](docs/ClientApi.md#connect_client) | Connect a client to the ConnectionRestApi service. Method returns a unique identifier of the client.
[**get_version**](docs/ClientApi.md#get_version) | Get the IdeaStatica version
  ### ConnectionApi

  
  
  Method | Description
  ------------- | -------------
[**get_connection**](docs/ConnectionApi.md#get_connection) | Get data about a specific connection in the project
[**get_connections**](docs/ConnectionApi.md#get_connections) | Get data about all connections in the project
[**get_production_cost**](docs/ConnectionApi.md#get_production_cost) | Get production cost of the connection
[**update_connection**](docs/ConnectionApi.md#update_connection) | Update data of a specific connection in the project
  ### ExportApi

  
  
  Method | Description
  ------------- | -------------
[**export_ifc^**](docs/ExportApi.md#export_ifc) | Export connection to IFC format
[**export_iom**](docs/ExportApi.md#export_iom) | Export connection to XML which includes https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs
[**export_iom_connection_data**](docs/ExportApi.md#export_iom_connection_data) | Get https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs for required connection
  ### LoadEffectApi

  
  
  Method | Description
  ------------- | -------------
[**add_load_effect**](docs/LoadEffectApi.md#add_load_effect) | Add new load effect to the connection
[**delete_load_effect**](docs/LoadEffectApi.md#delete_load_effect) | Delete load effect loadEffectId
[**get_load_effect**](docs/LoadEffectApi.md#get_load_effect) | Get load impulses from loadEffectId
[**get_load_effects**](docs/LoadEffectApi.md#get_load_effects) | Get all load effects which are defined in connectionId
[**get_load_settings**](docs/LoadEffectApi.md#get_load_settings) | Get Load settings for connection in project
[**set_load_settings**](docs/LoadEffectApi.md#set_load_settings) | Set Load settings for connection in project
[**update_load_effect**](docs/LoadEffectApi.md#update_load_effect) | Update load impulses in conLoading
  ### MaterialApi

  
  
  Method | Description
  ------------- | -------------
[**add_bolt_assembly**](docs/MaterialApi.md#add_bolt_assembly) | Add bolt assembly to the project
[**add_cross_section**](docs/MaterialApi.md#add_cross_section) | Add cross section to the project
[**add_material_bolt_grade**](docs/MaterialApi.md#add_material_bolt_grade) | Add material to the project
[**add_material_concrete**](docs/MaterialApi.md#add_material_concrete) | Add material to the project
[**add_material_steel**](docs/MaterialApi.md#add_material_steel) | Add material to the project
[**add_material_weld**](docs/MaterialApi.md#add_material_weld) | Add material to the project
[**get_all_materials**](docs/MaterialApi.md#get_all_materials) | Get materials which are used in the project projectId
[**get_bolt_assemblies**](docs/MaterialApi.md#get_bolt_assemblies) | Get bolt assemblies which are used in the project projectId
[**get_bolt_grade_materials**](docs/MaterialApi.md#get_bolt_grade_materials) | Get materials which are used in the project projectId
[**get_concrete_materials**](docs/MaterialApi.md#get_concrete_materials) | Get materials which are used in the project projectId
[**get_cross_sections**](docs/MaterialApi.md#get_cross_sections) | Get cross sections which are used in the project projectId
[**get_steel_materials**](docs/MaterialApi.md#get_steel_materials) | Get materials which are used in the project projectId
[**get_welding_materials**](docs/MaterialApi.md#get_welding_materials) | Get materials which are used in the project projectId
  ### MemberApi

  
  
  Method | Description
  ------------- | -------------
[**get_member**](docs/MemberApi.md#get_member) | Get information about the requires member in the connection
[**get_members**](docs/MemberApi.md#get_members) | Get information about all members in the connection
[**set_bearing_member**](docs/MemberApi.md#set_bearing_member) | Set bearing member for memberIt
[**update_member**](docs/MemberApi.md#update_member) | Update the member in the connection by newMemberData
  ### OperationApi

  
  
  Method | Description
  ------------- | -------------
[**delete_operations**](docs/OperationApi.md#delete_operations) | Delete all operations for the connection
[**get_common_operation_properties**](docs/OperationApi.md#get_common_operation_properties) | Get common operation properties
[**get_operations**](docs/OperationApi.md#get_operations) | Get the list of operations for the connection
[**update_common_operation_properties**](docs/OperationApi.md#update_common_operation_properties) | Update common properties for all operations
  ### ParameterApi

  
  
  Method | Description
  ------------- | -------------
[**evaluate_expression**](docs/ParameterApi.md#evaluate_expression) | Evaluate the expression and return the result
[**get_parameters**](docs/ParameterApi.md#get_parameters) | Get all parameters which are defined for projectId and connectionId
[**update_parameters**](docs/ParameterApi.md#update_parameters) | Update parameters for the connection connectionId in the project projectId by values passed in parameters
  ### PresentationApi

  
  
  Method | Description
  ------------- | -------------
[**get_data_scene3_d**](docs/PresentationApi.md#get_data_scene3_d) | Returns data for scene3D
[**get_data_scene3_d_text**](docs/PresentationApi.md#get_data_scene3_d_text) | Return serialized data for scene3D in json format
  ### ProjectApi

  
  
  Method | Description
  ------------- | -------------
[**close_project**](docs/ProjectApi.md#close_project) | Close the project. Needed for releasing resources in the service.
[**download_project^**](docs/ProjectApi.md#download_project) | Download the actual ideacon project from the service. It includes alle changes which were made by previous API calls.
[**get_active_projects**](docs/ProjectApi.md#get_active_projects) | Get the list of projects in the service which were opened by the client which was connected by M:IdeaStatiCa.ConnectionRestApi.Controllers.ClientController.ConnectClient
[**get_project_data**](docs/ProjectApi.md#get_project_data) | Get data of the project.
[**get_setup**](docs/ProjectApi.md#get_setup) | Get setup from project
[**import_iom^**](docs/ProjectApi.md#import_iom) | Create the IDEA Connection project from IOM provided in xml format.  The parameter 'containerXmlFile' passed in HTTP body represents :  <see href=\"https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs\">IdeaRS.OpenModel.OpenModelContainer</see>  which is serialized to XML string by  <see href=\"https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs\">IdeaRS.OpenModel.Tools.OpenModelContainerToXml</see>
[**open_project^**](docs/ProjectApi.md#open_project) | Open ideacon project from ideaConFile
[**update_from_iom^**](docs/ProjectApi.md#update_from_iom) | Update the IDEA Connection project by <see href=\"https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs\">IdeaRS.OpenModel.OpenModelContainer</see>  (model and results).  IOM is passed in the body of the request as the xml string.  <see href=\"https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs\">IdeaRS.OpenModel.Tools.OpenModelContainerToXml</see> should be used to generate the valid xml string
[**update_project_data**](docs/ProjectApi.md#update_project_data) | Updates ConProjectData of project
[**update_setup**](docs/ProjectApi.md#update_setup) | Update setup of the project
  ### ReportApi

  
  
  Method | Description
  ------------- | -------------
[**generate_pdf^**](docs/ReportApi.md#generate_pdf) | Generates report for projectId and connectionId
[**generate_word^**](docs/ReportApi.md#generate_word) | Generates report for projectId and connectionId
  ### TemplateApi

  
  
  Method | Description
  ------------- | -------------
[**apply_template**](docs/TemplateApi.md#apply_template) | Apply the connection template applyTemplateParam on the connection connectionId in the project projectId
[**create_con_template**](docs/TemplateApi.md#create_con_template) | Create a template for the connection connectionId in the project projectId
[**get_default_template_mapping**](docs/TemplateApi.md#get_default_template_mapping) | Get the default mappings for the application of the connection template passed in templateToApply  on connectionId in the project projectId

<a id="documentation-for-models"></a>
## Documentation for Models

 - [ideastatica_connection_api.models.AnchorGrid](docs/AnchorGrid.md)
 - [ideastatica_connection_api.models.AnchorType](docs/AnchorType.md)
 - [ideastatica_connection_api.models.BaseTemplateConversion](docs/BaseTemplateConversion.md)
 - [ideastatica_connection_api.models.BeamData](docs/BeamData.md)
 - [ideastatica_connection_api.models.BendData](docs/BendData.md)
 - [ideastatica_connection_api.models.BoltGrid](docs/BoltGrid.md)
 - [ideastatica_connection_api.models.BoltShearType](docs/BoltShearType.md)
 - [ideastatica_connection_api.models.BucklingRes](docs/BucklingRes.md)
 - [ideastatica_connection_api.models.CheckResAnchor](docs/CheckResAnchor.md)
 - [ideastatica_connection_api.models.CheckResBolt](docs/CheckResBolt.md)
 - [ideastatica_connection_api.models.CheckResConcreteBlock](docs/CheckResConcreteBlock.md)
 - [ideastatica_connection_api.models.CheckResPlate](docs/CheckResPlate.md)
 - [ideastatica_connection_api.models.CheckResSummary](docs/CheckResSummary.md)
 - [ideastatica_connection_api.models.CheckResWeld](docs/CheckResWeld.md)
 - [ideastatica_connection_api.models.ConAnalysisTypeEnum](docs/ConAnalysisTypeEnum.md)
 - [ideastatica_connection_api.models.ConCalculationParameter](docs/ConCalculationParameter.md)
 - [ideastatica_connection_api.models.ConConnection](docs/ConConnection.md)
 - [ideastatica_connection_api.models.ConLoadEffect](docs/ConLoadEffect.md)
 - [ideastatica_connection_api.models.ConLoadEffectMemberLoad](docs/ConLoadEffectMemberLoad.md)
 - [ideastatica_connection_api.models.ConLoadEffectPositionEnum](docs/ConLoadEffectPositionEnum.md)
 - [ideastatica_connection_api.models.ConLoadEffectSectionLoad](docs/ConLoadEffectSectionLoad.md)
 - [ideastatica_connection_api.models.ConLoadSettings](docs/ConLoadSettings.md)
 - [ideastatica_connection_api.models.ConMember](docs/ConMember.md)
 - [ideastatica_connection_api.models.ConMprlCrossSection](docs/ConMprlCrossSection.md)
 - [ideastatica_connection_api.models.ConMprlElement](docs/ConMprlElement.md)
 - [ideastatica_connection_api.models.ConOperation](docs/ConOperation.md)
 - [ideastatica_connection_api.models.ConOperationCommonProperties](docs/ConOperationCommonProperties.md)
 - [ideastatica_connection_api.models.ConProductionCost](docs/ConProductionCost.md)
 - [ideastatica_connection_api.models.ConProject](docs/ConProject.md)
 - [ideastatica_connection_api.models.ConProjectData](docs/ConProjectData.md)
 - [ideastatica_connection_api.models.ConResultSummary](docs/ConResultSummary.md)
 - [ideastatica_connection_api.models.ConTemplateApplyParam](docs/ConTemplateApplyParam.md)
 - [ideastatica_connection_api.models.ConTemplateMappingGetParam](docs/ConTemplateMappingGetParam.md)
 - [ideastatica_connection_api.models.ConcreteBlock](docs/ConcreteBlock.md)
 - [ideastatica_connection_api.models.ConcreteBlockData](docs/ConcreteBlockData.md)
 - [ideastatica_connection_api.models.ConcreteSetup](docs/ConcreteSetup.md)
 - [ideastatica_connection_api.models.ConeBreakoutCheckType](docs/ConeBreakoutCheckType.md)
 - [ideastatica_connection_api.models.ConnectionCheckRes](docs/ConnectionCheckRes.md)
 - [ideastatica_connection_api.models.ConnectionData](docs/ConnectionData.md)
 - [ideastatica_connection_api.models.ConnectionSetup](docs/ConnectionSetup.md)
 - [ideastatica_connection_api.models.CrtCompCheckIS](docs/CrtCompCheckIS.md)
 - [ideastatica_connection_api.models.CutBeamByBeamData](docs/CutBeamByBeamData.md)
 - [ideastatica_connection_api.models.CutData](docs/CutData.md)
 - [ideastatica_connection_api.models.CutMethod](docs/CutMethod.md)
 - [ideastatica_connection_api.models.CutOrientation](docs/CutOrientation.md)
 - [ideastatica_connection_api.models.CutPart](docs/CutPart.md)
 - [ideastatica_connection_api.models.DistanceComparison](docs/DistanceComparison.md)
 - [ideastatica_connection_api.models.DrawData](docs/DrawData.md)
 - [ideastatica_connection_api.models.FoldedPlateData](docs/FoldedPlateData.md)
 - [ideastatica_connection_api.models.IGroup](docs/IGroup.md)
 - [ideastatica_connection_api.models.IdeaParameter](docs/IdeaParameter.md)
 - [ideastatica_connection_api.models.IdeaParameterUpdate](docs/IdeaParameterUpdate.md)
 - [ideastatica_connection_api.models.Line](docs/Line.md)
 - [ideastatica_connection_api.models.MessageNumber](docs/MessageNumber.md)
 - [ideastatica_connection_api.models.OpenElementId](docs/OpenElementId.md)
 - [ideastatica_connection_api.models.OpenMessage](docs/OpenMessage.md)
 - [ideastatica_connection_api.models.OpenMessages](docs/OpenMessages.md)
 - [ideastatica_connection_api.models.PinGrid](docs/PinGrid.md)
 - [ideastatica_connection_api.models.PlateData](docs/PlateData.md)
 - [ideastatica_connection_api.models.Point2D](docs/Point2D.md)
 - [ideastatica_connection_api.models.Point3D](docs/Point3D.md)
 - [ideastatica_connection_api.models.PolyLine2D](docs/PolyLine2D.md)
 - [ideastatica_connection_api.models.ReferenceElement](docs/ReferenceElement.md)
 - [ideastatica_connection_api.models.Region2D](docs/Region2D.md)
 - [ideastatica_connection_api.models.Segment2D](docs/Segment2D.md)
 - [ideastatica_connection_api.models.Selected](docs/Selected.md)
 - [ideastatica_connection_api.models.SelectedType](docs/SelectedType.md)
 - [ideastatica_connection_api.models.TemplateConversions](docs/TemplateConversions.md)
 - [ideastatica_connection_api.models.Text](docs/Text.md)
 - [ideastatica_connection_api.models.TextPosition](docs/TextPosition.md)
 - [ideastatica_connection_api.models.Vector3D](docs/Vector3D.md)
 - [ideastatica_connection_api.models.WeldData](docs/WeldData.md)
 - [ideastatica_connection_api.models.WeldEvaluation](docs/WeldEvaluation.md)
 - [ideastatica_connection_api.models.WeldType](docs/WeldType.md)



## Notes

This Python package is automatically generated by the [OpenAPI Generator](https://openapi-generator.tech) project:

- API version: 1.0
- Package version: 24.1.2.1474
- Generator version: 7.9.0
- Build package: org.openapitools.codegen.languages.PythonClientCodegen
For more information, please visit [https://github.com/idea-statica/ideastatica-public](https://github.com/idea-statica/ideastatica-public)
