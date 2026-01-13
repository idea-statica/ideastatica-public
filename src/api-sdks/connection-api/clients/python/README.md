# ideastatica-connection-api

The Python package for the Connection Rest API 3.0

- API version: 3.0
- Package version: 25.1.3.1326

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
[**calculate**](docs/CalculationApi.md#calculate) | Runs CBFEM calculation and returns the summary of the results.
[**get_raw_json_results**](docs/CalculationApi.md#get_raw_json_results) | Gets JSON string which represents raw CBFEM results (an instance of CheckResultsData).
[**get_results**](docs/CalculationApi.md#get_results) | Gets detailed results of the CBFEM analysis.
  ### ClientApi

  
  
  Method | Description
  ------------- | -------------
[**connect_client**](docs/ClientApi.md#connect_client) | Connects a client to the ConnectionRestApi service and returns a unique identifier for the client.
[**get_version**](docs/ClientApi.md#get_version) | Gets the IdeaStatica API assembly version.
  ### ConnectionApi

  
  
  Method | Description
  ------------- | -------------
[**delete_connection**](docs/ConnectionApi.md#delete_connection) | Deletes a specific connection from the project.
[**get_connection**](docs/ConnectionApi.md#get_connection) | Gets data about a specific connection in the project.
[**get_connection_topology**](docs/ConnectionApi.md#get_connection_topology) | Gets the topology of the connection in JSON format.
[**get_connections**](docs/ConnectionApi.md#get_connections) | Gets data about all connections in the project.
[**get_production_cost**](docs/ConnectionApi.md#get_production_cost) | Gets the production cost of the connection.
[**update_connection**](docs/ConnectionApi.md#update_connection) | Updates data of a specific connection in the project.
  ### ConnectionLibraryApi

  
  
  Method | Description
  ------------- | -------------
[**get_design_item_picture**](docs/ConnectionLibraryApi.md#get_design_item_picture) | Retrieves the picture associated with the specified design item as a PNG image.
[**get_design_sets**](docs/ConnectionLibraryApi.md#get_design_sets) | Retrieves a list of design sets available for the user.
[**get_template**](docs/ConnectionLibraryApi.md#get_template) | Retrieves the template associated with the specified design set and design item.
[**propose**](docs/ConnectionLibraryApi.md#propose) | Proposes a list of design items for a specified connection within a project.
[**publish_connection**](docs/ConnectionLibraryApi.md#publish_connection) | Publish template to Private or Company set
  ### ConversionApi

  
  
  Method | Description
  ------------- | -------------
[**change_code**](docs/ConversionApi.md#change_code) | Changes the design code of the project.
[**get_conversion_mapping**](docs/ConversionApi.md#get_conversion_mapping) | Gets default conversion mappings for converting the project to a different design code.
  ### ExportApi

  
  
  Method | Description
  ------------- | -------------
[**export_ifc^**](docs/ExportApi.md#export_ifc) | Exports the connection to IFC format.
[**export_iom**](docs/ExportApi.md#export_iom) | Exports the connection to XML which includes the OpenModelContainer (https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs).
[**export_iom_connection_data**](docs/ExportApi.md#export_iom_connection_data) | Gets the ConnectionData for the specified connection (https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs).
  ### LoadEffectApi

  
  
  Method | Description
  ------------- | -------------
[**add_load_effect**](docs/LoadEffectApi.md#add_load_effect) | Adds a new load effect to the connection.
[**delete_load_effect**](docs/LoadEffectApi.md#delete_load_effect) | Delete load effect loadEffectId
[**get_load_effect**](docs/LoadEffectApi.md#get_load_effect) | Gets load impulses from the specified load effect.
[**get_load_effects**](docs/LoadEffectApi.md#get_load_effects) | Gets all load effects defined in the specified connection.
[**get_load_settings**](docs/LoadEffectApi.md#get_load_settings) | Get Load settings for connection in project
[**set_load_settings**](docs/LoadEffectApi.md#set_load_settings) | Set Load settings for connection in project
[**update_load_effect**](docs/LoadEffectApi.md#update_load_effect) | Update load impulses in conLoading
  ### MaterialApi

  
  
  Method | Description
  ------------- | -------------
[**add_bolt_assembly**](docs/MaterialApi.md#add_bolt_assembly) | Add bolt assembly to the project
[**add_cross_section**](docs/MaterialApi.md#add_cross_section) | Add cross section to the project
[**add_material_bolt_grade**](docs/MaterialApi.md#add_material_bolt_grade) | Adds a material to the project.
[**add_material_concrete**](docs/MaterialApi.md#add_material_concrete) | Adds a material to the project.
[**add_material_steel**](docs/MaterialApi.md#add_material_steel) | Adds a material to the project.
[**add_material_weld**](docs/MaterialApi.md#add_material_weld) | Adds a material to the project.
[**get_all_materials**](docs/MaterialApi.md#get_all_materials) | Gets materials used in the specified project.
[**get_bolt_assemblies**](docs/MaterialApi.md#get_bolt_assemblies) | Gets bolt assemblies used in the specified project.
[**get_bolt_grade_materials**](docs/MaterialApi.md#get_bolt_grade_materials) | Gets materials used in the specified project.
[**get_concrete_materials**](docs/MaterialApi.md#get_concrete_materials) | Gets materials used in the specified project.
[**get_cross_sections**](docs/MaterialApi.md#get_cross_sections) | Gets cross sections used in the specified project.
[**get_steel_materials**](docs/MaterialApi.md#get_steel_materials) | Gets materials used in the specified project.
[**get_welding_materials**](docs/MaterialApi.md#get_welding_materials) | Gets materials used in the specified project.
  ### MemberApi

  
  
  Method | Description
  ------------- | -------------
[**get_member**](docs/MemberApi.md#get_member) | Gets information about the specified member in the connection.
[**get_members**](docs/MemberApi.md#get_members) | Gets information about all members in the connection.
[**set_bearing_member**](docs/MemberApi.md#set_bearing_member) | Set bearing member for memberIt
[**update_member**](docs/MemberApi.md#update_member) | Updates the member in the connection with the provided data.
  ### OperationApi

  
  
  Method | Description
  ------------- | -------------
[**delete_operations**](docs/OperationApi.md#delete_operations) | Delete all operations for the connection
[**get_common_operation_properties**](docs/OperationApi.md#get_common_operation_properties) | Gets common operation properties.
[**get_operations**](docs/OperationApi.md#get_operations) | Gets the list of operations for the connection.
[**pre_design_welds**](docs/OperationApi.md#pre_design_welds) | Pre-designs welds in the connection.
[**update_common_operation_properties**](docs/OperationApi.md#update_common_operation_properties) | Updates common properties for all operations.
  ### ParameterApi

  
  
  Method | Description
  ------------- | -------------
[**delete_parameters**](docs/ParameterApi.md#delete_parameters) | Delete all parameters and parameter model links for the connection connectionId in the project projectId
[**evaluate_expression**](docs/ParameterApi.md#evaluate_expression) | Evaluate the expression and return the result.  For more details see documentation about parameters:  https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html  or  https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html
[**get_parameters**](docs/ParameterApi.md#get_parameters) | Gets all parameters defined for the specified project and connection.
[**update**](docs/ParameterApi.md#update) | Updates parameters for the specified connection in the project with the values provided.
  ### PresentationApi

  
  
  Method | Description
  ------------- | -------------
[**get_data_scene3_d**](docs/PresentationApi.md#get_data_scene3_d) | Returns data for Scene3D visualization.
[**get_data_scene3_d_text**](docs/PresentationApi.md#get_data_scene3_d_text) | Returns serialized data for Scene3D in JSON format.
  ### ProjectApi

  
  
  Method | Description
  ------------- | -------------
[**close_project**](docs/ProjectApi.md#close_project) | Closes the project and releases resources in the service.
[**download_project^**](docs/ProjectApi.md#download_project) | Downloads the current IdeaCon project from the service, including all changes made by previous API calls.
[**get_active_projects**](docs/ProjectApi.md#get_active_projects) | Gets the list of projects in the service that were opened by the client connected via M:IdeaStatiCa.ConnectionRestApi.Controllers.ClientController.ConnectClient.
[**get_project_data**](docs/ProjectApi.md#get_project_data) | Get data of the project.
[**import_iom^**](docs/ProjectApi.md#import_iom) | Create the IDEA Connection project from IOM provided in xml format.  The parameter 'containerXmlFile' passed in HTTP body represents :  [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  which is serialized to XML string by  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs)
[**open_project^**](docs/ProjectApi.md#open_project) | Opens an IdeaCon project from the provided file.
[**update_from_iom^**](docs/ProjectApi.md#update_from_iom) | Update the IDEA Connection project by [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  (model and results).  IOM is passed in the body of the request as the xml string.  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs) should be used to generate the valid xml string
[**update_project_data**](docs/ProjectApi.md#update_project_data) | Updates ConProjectData of project
  ### ReportApi

  
  
  Method | Description
  ------------- | -------------
[**generate_pdf^**](docs/ReportApi.md#generate_pdf) | Generates a report for the specified connection in PDF or Word format.
[**generate_pdf_for_mutliple^**](docs/ReportApi.md#generate_pdf_for_mutliple) | Generates a report for multiple connections in PDF or Word format.
[**generate_word^**](docs/ReportApi.md#generate_word) | Generates a report for the specified connection in PDF or Word format.
[**generate_word_for_multiple^**](docs/ReportApi.md#generate_word_for_multiple) | Generates a report for multiple connections in PDF or Word format.
  ### SettingsApi

  
  
  Method | Description
  ------------- | -------------
[**get_settings**](docs/SettingsApi.md#get_settings) | Gets setting values for the project.
[**update_settings**](docs/SettingsApi.md#update_settings) | Updates one or multiple setting values in the project.
  ### TemplateApi

  
  
  Method | Description
  ------------- | -------------
[**apply_template**](docs/TemplateApi.md#apply_template) | Apply the connection template applyTemplateParam on the connection connectionId in the project projectId
[**create_con_template**](docs/TemplateApi.md#create_con_template) | Create a template for the connection connectionId in the project projectId
[**delete**](docs/TemplateApi.md#delete) | Delete specific template
[**delete_all**](docs/TemplateApi.md#delete_all) | Delete all templates in connection
[**explode**](docs/TemplateApi.md#explode) | Explode specific template (delete parameters, keep operations)
[**explode_all**](docs/TemplateApi.md#explode_all) | Explode all templates (delete parameters, keep operations)
[**get_default_template_mapping**](docs/TemplateApi.md#get_default_template_mapping) | Get the default mappings for the application of the connection template passed in templateToApply  on connectionId in the project projectId
[**get_template_common_operation_properties**](docs/TemplateApi.md#get_template_common_operation_properties) | Get Common properties for specific template
[**get_template_in_connection**](docs/TemplateApi.md#get_template_in_connection) | Retrieves a specific template by its ID for a given connection within a project.
[**get_templates_in_connection**](docs/TemplateApi.md#get_templates_in_connection) | Retrieves a list of templates associated with a specific connection within a project.
[**load_defaults**](docs/TemplateApi.md#load_defaults) | Load parameter defaults for specific template.
[**update_template_common_operation_properties**](docs/TemplateApi.md#update_template_common_operation_properties) | Set common properties for specific template

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
 - [ideastatica_connection_api.models.ConConnection](docs/ConConnection.md)
 - [ideastatica_connection_api.models.ConConnectionLibrarySearchParameters](docs/ConConnectionLibrarySearchParameters.md)
 - [ideastatica_connection_api.models.ConConnectionTemplate](docs/ConConnectionTemplate.md)
 - [ideastatica_connection_api.models.ConConversionSettings](docs/ConConversionSettings.md)
 - [ideastatica_connection_api.models.ConDesignItem](docs/ConDesignItem.md)
 - [ideastatica_connection_api.models.ConDesignSet](docs/ConDesignSet.md)
 - [ideastatica_connection_api.models.ConDesignSetType](docs/ConDesignSetType.md)
 - [ideastatica_connection_api.models.ConItem](docs/ConItem.md)
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
 - [ideastatica_connection_api.models.ConNonConformityIssue](docs/ConNonConformityIssue.md)
 - [ideastatica_connection_api.models.ConNonConformityIssueSeverity](docs/ConNonConformityIssueSeverity.md)
 - [ideastatica_connection_api.models.ConOperation](docs/ConOperation.md)
 - [ideastatica_connection_api.models.ConOperationCommonProperties](docs/ConOperationCommonProperties.md)
 - [ideastatica_connection_api.models.ConProductionCost](docs/ConProductionCost.md)
 - [ideastatica_connection_api.models.ConProject](docs/ConProject.md)
 - [ideastatica_connection_api.models.ConProjectData](docs/ConProjectData.md)
 - [ideastatica_connection_api.models.ConResultSummary](docs/ConResultSummary.md)
 - [ideastatica_connection_api.models.ConTemplateApplyParam](docs/ConTemplateApplyParam.md)
 - [ideastatica_connection_api.models.ConTemplateApplyResult](docs/ConTemplateApplyResult.md)
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
 - [ideastatica_connection_api.models.SearchOption](docs/SearchOption.md)
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

- API version: 3.0
- Package version: 25.1.3.1326
- Generator version: 7.9.0
- Build package: org.openapitools.codegen.languages.PythonClientCodegen
For more information, please visit [https://github.com/idea-statica/ideastatica-public](https://github.com/idea-statica/ideastatica-public)
