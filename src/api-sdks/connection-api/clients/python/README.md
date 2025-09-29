# ideastatica-connection-api

The Python package for the Connection Rest API 2.0

- API version: 2.0
- Package version: 25.1.0.2640

IDEA StatiCa Connection API, used for the automated design and calculation of steel connections.

## Requirements.

Python 3.7+

## Installation

### pip install 

We recommend using pip to install the package into your environment.

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

To start the service, manually navigate to the "C:\Program Files\IDEA StatiCa\StatiCa 25.1" folder. Using CLI:

```console
IdeaStatiCa.ConnectionRestApi.exe -port:5193
```

Parameter _-port:_ is optional. The default port is 5000.

```python
# Connect any new service to latest version of IDEA StatiCa.
client_factory = ConnectionApiClientFactory('http://localhost:5000/')
```

```python
conClient = client_factory.create_connection_api_client();
```

Alternatively, you can leverage the `ConnectionApiServiceAttacher`.
`ConnectionApiServiceAttacher` manages the creation of client instances that connect to an already running service:

```python
import logging
import sys
import os
import json
from pprint import pprint
from urllib.parse import urljoin

# Get the parent directory
parent_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))

# Add the parent directory to sys.path
sys.path.append(parent_dir)

import ideastatica_connection_api
import ideastatica_connection_api.connection_api_service_attacher as connection_api_service_attacher

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

baseUrl = "http://localhost:5000"

dir_path = os.path.dirname(os.path.realpath(__file__))
project_file_path = os.path.join(dir_path, '..\projects', 'HSS_norm_cond.ideaCon')
print(project_file_path)


# Create client attached to already running service
with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
    try:
        # Open project
        uploadRes = api_client.project.open_project_from_filepath(project_file_path)

        # Get the project data
        project_data = api_client.project.get_project_data(api_client.project.active_project_id)
        pprint(project_data)

        # Get list of all connections in the project
        connections_in_project = api_client.connection.get_connections(api_client.project.active_project_id)

        # first connection in the project 
        connection1 = connections_in_project[0]
        pprint(connection1)

        # run stress-strain CBFEM analysis for the connection id = 1
        calcParams = ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional)
        calcParams.connection_ids = [connection1.id]

        # run stress-strain analysis for the connection
        con1_cbfem_results = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)
        pprint(con1_cbfem_results)
        
        # get detailed results. Results are list of strings
        # the number of strings in the list correspond to the number of calculated connections
        results_text = api_client.calculation.get_raw_json_results(api_client.project.active_project_id, calcParams)
        firstConnectionResult = results_text[0]
        pprint(firstConnectionResult)

        raw_results = json.loads(firstConnectionResult)
        pprint(raw_results)

        detailed_results = api_client.calculation.get_results(api_client.project.active_project_id, calcParams)
        pprint(detailed_results)

        # get connection setup
        connection_setup =  api_client.project.get_setup(api_client.project.active_project_id)
        pprint(connection_setup)

        # modify setup
        connection_setup.hss_limit_plastic_strain = 0.02
        modifiedSetup = api_client.project.update_setup(api_client.project.active_project_id, connection_setup)

        # recalculate connection
        recalculate_results = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)
        pprint(recalculate_results)

    except Exception as e:
        print("Operation failed : %s\n" % e)
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
[**calculate**](docs/CalculationApi.md#calculate) | 
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
[**delete_connection**](docs/ConnectionApi.md#delete_connection) | 
[**get_connection**](docs/ConnectionApi.md#get_connection) | Get data about a specific connection in the project
[**get_connections**](docs/ConnectionApi.md#get_connections) | Get data about all connections in the project
[**get_production_cost**](docs/ConnectionApi.md#get_production_cost) | Get production cost of the connection
[**update_connection**](docs/ConnectionApi.md#update_connection) | Update data of a specific connection in the project
  ### ConversionApi

  
  
  Method | Description
  ------------- | -------------
[**change_code**](docs/ConversionApi.md#change_code) | Change design code of project.
[**get_conversion_mapping**](docs/ConversionApi.md#get_conversion_mapping) | Get default conversions for converting the project to different design code.
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
[**pre_design_welds**](docs/OperationApi.md#pre_design_welds) | Predesign welds
[**update_common_operation_properties**](docs/OperationApi.md#update_common_operation_properties) | Update common properties for all operations
  ### ParameterApi

  
  
  Method | Description
  ------------- | -------------
[**clear_parameters**](docs/ParameterApi.md#clear_parameters) | Clear parameters and links for the connection connectionId in the project projectId
[**evaluate_expression**](docs/ParameterApi.md#evaluate_expression) | Evaluate the expression and return the result
[**get_parameters**](docs/ParameterApi.md#get_parameters) | Get all parameters which are defined for projectId and connectionId
[**update**](docs/ParameterApi.md#update) | Update parameters for the connection connectionId in the project projectId by values passed in parameters
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
[**import_iom^**](docs/ProjectApi.md#import_iom) | Create the IDEA Connection project from IOM provided in xml format.  The parameter 'containerXmlFile' passed in HTTP body represents :  [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  which is serialized to XML string by  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs)
[**open_project^**](docs/ProjectApi.md#open_project) | Open ideacon project from ideaConFile
[**update_from_iom^**](docs/ProjectApi.md#update_from_iom) | Update the IDEA Connection project by [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  (model and results).  IOM is passed in the body of the request as the xml string.  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs) should be used to generate the valid xml string
[**update_project_data**](docs/ProjectApi.md#update_project_data) | Updates ConProjectData of project
  ### ReportApi

  
  
  Method | Description
  ------------- | -------------
[**generate_pdf^**](docs/ReportApi.md#generate_pdf) | Generates report for projectId and connectionId
[**generate_pdf_for_mutliple^**](docs/ReportApi.md#generate_pdf_for_mutliple) | 
[**generate_word^**](docs/ReportApi.md#generate_word) | Generates report for projectId and connectionId
[**generate_word_for_multiple^**](docs/ReportApi.md#generate_word_for_multiple) | 
  ### SettingsApi

  
  
  Method | Description
  ------------- | -------------
[**get_settings**](docs/SettingsApi.md#get_settings) | 
[**update_settings**](docs/SettingsApi.md#update_settings) | 
  ### TemplateApi

  
  
  Method | Description
  ------------- | -------------
[**apply_template**](docs/TemplateApi.md#apply_template) | Apply the connection template applyTemplateParam on the connection connectionId in the project projectId
[**clear_design**](docs/TemplateApi.md#clear_design) | Clear the design of the connection connectionId in the project projectId
[**create_con_template**](docs/TemplateApi.md#create_con_template) | Create a template for the connection connectionId in the project projectId
[**get_connection_topology**](docs/TemplateApi.md#get_connection_topology) | Get topology of the connection in json format
[**get_default_template_mapping**](docs/TemplateApi.md#get_default_template_mapping) | Get the default mappings for the application of the connection template passed in templateToApply  on connectionId in the project projectId
[**publish_connection**](docs/TemplateApi.md#publish_connection) | 

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
 - [ideastatica_connection_api.models.ConAlignedPlate](docs/ConAlignedPlate.md)
 - [ideastatica_connection_api.models.ConAlignedPlateSideCodeEnum](docs/ConAlignedPlateSideCodeEnum.md)
 - [ideastatica_connection_api.models.ConAnalysisTypeEnum](docs/ConAnalysisTypeEnum.md)
 - [ideastatica_connection_api.models.ConCalculationParameter](docs/ConCalculationParameter.md)
 - [ideastatica_connection_api.models.ConConnection](docs/ConConnection.md)
 - [ideastatica_connection_api.models.ConConversionSettings](docs/ConConversionSettings.md)
 - [ideastatica_connection_api.models.ConDesignSetType](docs/ConDesignSetType.md)
 - [ideastatica_connection_api.models.ConLoadEffect](docs/ConLoadEffect.md)
 - [ideastatica_connection_api.models.ConLoadEffectMemberLoad](docs/ConLoadEffectMemberLoad.md)
 - [ideastatica_connection_api.models.ConLoadEffectPositionEnum](docs/ConLoadEffectPositionEnum.md)
 - [ideastatica_connection_api.models.ConLoadEffectSectionLoad](docs/ConLoadEffectSectionLoad.md)
 - [ideastatica_connection_api.models.ConLoadSettings](docs/ConLoadSettings.md)
 - [ideastatica_connection_api.models.ConMember](docs/ConMember.md)
 - [ideastatica_connection_api.models.ConMemberAlignmentTypeEnum](docs/ConMemberAlignmentTypeEnum.md)
 - [ideastatica_connection_api.models.ConMemberForcesInEnum](docs/ConMemberForcesInEnum.md)
 - [ideastatica_connection_api.models.ConMemberModel](docs/ConMemberModel.md)
 - [ideastatica_connection_api.models.ConMemberPlacementDefinitionTypeEnum](docs/ConMemberPlacementDefinitionTypeEnum.md)
 - [ideastatica_connection_api.models.ConMemberPlatePartTypeEnum](docs/ConMemberPlatePartTypeEnum.md)
 - [ideastatica_connection_api.models.ConMemberPosition](docs/ConMemberPosition.md)
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
 - [ideastatica_connection_api.models.ConTemplatePublishParam](docs/ConTemplatePublishParam.md)
 - [ideastatica_connection_api.models.ConWeldSizingMethodEnum](docs/ConWeldSizingMethodEnum.md)
 - [ideastatica_connection_api.models.ConcreteBlock](docs/ConcreteBlock.md)
 - [ideastatica_connection_api.models.ConcreteBlockData](docs/ConcreteBlockData.md)
 - [ideastatica_connection_api.models.ConnectionCheckRes](docs/ConnectionCheckRes.md)
 - [ideastatica_connection_api.models.ConnectionData](docs/ConnectionData.md)
 - [ideastatica_connection_api.models.ConversionMapping](docs/ConversionMapping.md)
 - [ideastatica_connection_api.models.CountryCode](docs/CountryCode.md)
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
 - [ideastatica_connection_api.models.IdeaParameterValidation](docs/IdeaParameterValidation.md)
 - [ideastatica_connection_api.models.IdeaParameterValidationResponse](docs/IdeaParameterValidationResponse.md)
 - [ideastatica_connection_api.models.Line](docs/Line.md)
 - [ideastatica_connection_api.models.MessageNumber](docs/MessageNumber.md)
 - [ideastatica_connection_api.models.OpenElementId](docs/OpenElementId.md)
 - [ideastatica_connection_api.models.OpenMessage](docs/OpenMessage.md)
 - [ideastatica_connection_api.models.OpenMessages](docs/OpenMessages.md)
 - [ideastatica_connection_api.models.ParameterUpdateResponse](docs/ParameterUpdateResponse.md)
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
 - [ideastatica_connection_api.models.WeldType](docs/WeldType.md)



## Notes

This Python package is automatically generated by the [OpenAPI Generator](https://openapi-generator.tech) project:

- API version: 2.0
- Package version: 25.1.0.2640
- Generator version: 7.9.0
- Build package: org.openapitools.codegen.languages.PythonClientCodegen
For more information, please visit [https://github.com/idea-statica/ideastatica-public](https://github.com/idea-statica/ideastatica-public)
