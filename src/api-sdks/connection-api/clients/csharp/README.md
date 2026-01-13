# IdeaStatiCa.ConnectionApi

The C# library for the Connection Rest API 3.0

- API version: 3.0
- SDK version: 25.1.3.1326

IDEA StatiCa Connection API, used for the automated design and calculation of steel connections.

<a id="frameworks-supported"></a>
## Frameworks supported
- .NET Standard >=2.0

<a id="installation"></a>
## Installation

### NuGet package

We recommend installing the package to your project using NuGet. This will ensure all dependencies are automatically installed.
See [IdeaStatiCa.ConnectionApi](https://www.nuget.org/packages/IdeaStatiCa.ConnectionApi/)

### Building from source

Generate the DLL using your preferred tool (e.g. `dotnet build`) or opening the avaliable Visual Studio solution (.sln) and building. See [ConRestApiClient.sln](ConRestApiClient.sln)

Projects can be created by the template or the the wizard in Visual Studio. See [Connection Rest API client wizard](project-template.md).

<a id="usage"></a>
## Usage

`ClientApiClientFactory` manages creation of clients on the running service. 
We currently only support connecting to a service running on a localhost (eg. 'http://localhost:5000/').

To start the service, manually navigate to the "C:\Program Files\IDEA StatiCa\StatiCa 25.1" folder. Using CLI:

```console
IdeaStatiCa.ConnectionRestApi.exe -port:5193
```

Parameter _-port:_ is optional. The default port is 5000.

```csharp
// Connect any new service to latest version of IDEA StatiCa.
ConnectionApiServiceAttacher clientFactory = new ConnectionApiServiceAttacher('http://localhost:5000/');
```

```csharp
IConnectionApiClient conClient = await clientFactory.CreateApiClient();
```


<a id="getting-started"></a>
## Getting Started

The below snippet shows a simple getting started example which opens an IDEA StatiCa Connection project and performs the calculation.

```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace Example
{
    public class Example
    {
        private static CancellationToken cancellationToken;

        public static async Task Main()
        {
            string ideaConFile = "test1.ideaCon";

            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 25.1"; // path to the IdeaStatiCa.ConnectionRestApi.exe

            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                // create ConnectionApiClient
                using (var conClient = await clientFactory.CreateApiClient())
                {
                    // open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile, cancellationToken);

                    if (!projData.Connections.Any())
                    {
                        return;
                    }

                    // request to run plastic CBFEM for all connections in the project
                    ConCalculationParameter conCalcParam = new ConCalculationParameter()
                    {
                        AnalysisType = ConAnalysisTypeEnum.Stress_Strain,
                        ConnectionIds = projData.Connections.Select(c => c.Id).ToList()
                    };

                    var cbfemResult = await conClient.Calculation.CalculateAsync(projData.ProjectId, conCalcParam, 0, cancellationToken);
                    await conClient.Project.CloseProjectAsync(projData.ProjectId);
                }
            }
        }
    }
}
```

<a id="documentation-for-api-endpoints"></a>
## Documentation for API Endpoints

The `ConnectionApiClient` wraps all API endpoing controllers into object based or action baseds API endpoints.

Methods marked with an **^** denote that they have an additional extension in the Client.

  ### CalculationApi

  
  
  Method | Description
  ------------- | -------------
[**Calculate**](docs/CalculationApi.md#calculate) | Runs CBFEM calculation and returns the summary of the results.
[**GetRawJsonResults**](docs/CalculationApi.md#getrawjsonresults) | Gets JSON string which represents raw CBFEM results (an instance of CheckResultsData).
[**GetResults**](docs/CalculationApi.md#getresults) | Gets detailed results of the CBFEM analysis.
  ### ClientApi

  
  
  Method | Description
  ------------- | -------------
[**ConnectClient**](docs/ClientApi.md#connectclient) | Connects a client to the ConnectionRestApi service and returns a unique identifier for the client.
[**GetVersion**](docs/ClientApi.md#getversion) | Gets the IdeaStatica API assembly version.
  ### ConnectionApi

  
  
  Method | Description
  ------------- | -------------
[**DeleteConnection**](docs/ConnectionApi.md#deleteconnection) | Deletes a specific connection from the project.
[**GetConnection**](docs/ConnectionApi.md#getconnection) | Gets data about a specific connection in the project.
[**GetConnectionTopology**](docs/ConnectionApi.md#getconnectiontopology) | Gets the topology of the connection in JSON format.
[**GetConnections**](docs/ConnectionApi.md#getconnections) | Gets data about all connections in the project.
[**GetProductionCost**](docs/ConnectionApi.md#getproductioncost) | Gets the production cost of the connection.
[**UpdateConnection**](docs/ConnectionApi.md#updateconnection) | Updates data of a specific connection in the project.
  ### ConnectionLibraryApi

  
  
  Method | Description
  ------------- | -------------
[**GetDesignItemPicture**](docs/ConnectionLibraryApi.md#getdesignitempicture) | Retrieves the picture associated with the specified design item as a PNG image.
[**GetDesignSets**](docs/ConnectionLibraryApi.md#getdesignsets) | Retrieves a list of design sets available for the user.
[**GetTemplate**](docs/ConnectionLibraryApi.md#gettemplate) | Retrieves the template associated with the specified design set and design item.
[**Propose**](docs/ConnectionLibraryApi.md#propose) | Proposes a list of design items for a specified connection within a project.
[**PublishConnection**](docs/ConnectionLibraryApi.md#publishconnection) | Publish template to Private or Company set
  ### ConversionApi

  
  
  Method | Description
  ------------- | -------------
[**ChangeCode**](docs/ConversionApi.md#changecode) | Changes the design code of the project.
[**GetConversionMapping**](docs/ConversionApi.md#getconversionmapping) | Gets default conversion mappings for converting the project to a different design code.
  ### ExportApi

  
  
  Method | Description
  ------------- | -------------
[**ExportIFC^**](docs/ExportApi.md#exportifc) | Exports the connection to IFC format.
[**ExportIom**](docs/ExportApi.md#exportiom) | Exports the connection to XML which includes the OpenModelContainer (https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs).
[**ExportIomConnectionData**](docs/ExportApi.md#exportiomconnectiondata) | Gets the ConnectionData for the specified connection (https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs).
  ### LoadEffectApi

  
  
  Method | Description
  ------------- | -------------
[**AddLoadEffect**](docs/LoadEffectApi.md#addloadeffect) | Adds a new load effect to the connection.
[**DeleteLoadEffect**](docs/LoadEffectApi.md#deleteloadeffect) | Delete load effect loadEffectId
[**GetLoadEffect**](docs/LoadEffectApi.md#getloadeffect) | Gets load impulses from the specified load effect.
[**GetLoadEffects**](docs/LoadEffectApi.md#getloadeffects) | Gets all load effects defined in the specified connection.
[**GetLoadSettings**](docs/LoadEffectApi.md#getloadsettings) | Get Load settings for connection in project
[**SetLoadSettings**](docs/LoadEffectApi.md#setloadsettings) | Set Load settings for connection in project
[**UpdateLoadEffect**](docs/LoadEffectApi.md#updateloadeffect) | Update load impulses in conLoading
  ### MaterialApi

  
  
  Method | Description
  ------------- | -------------
[**AddBoltAssembly**](docs/MaterialApi.md#addboltassembly) | Add bolt assembly to the project
[**AddCrossSection**](docs/MaterialApi.md#addcrosssection) | Add cross section to the project
[**AddMaterialBoltGrade**](docs/MaterialApi.md#addmaterialboltgrade) | Adds a material to the project.
[**AddMaterialConcrete**](docs/MaterialApi.md#addmaterialconcrete) | Adds a material to the project.
[**AddMaterialSteel**](docs/MaterialApi.md#addmaterialsteel) | Adds a material to the project.
[**AddMaterialWeld**](docs/MaterialApi.md#addmaterialweld) | Adds a material to the project.
[**GetAllMaterials**](docs/MaterialApi.md#getallmaterials) | Gets materials used in the specified project.
[**GetBoltAssemblies**](docs/MaterialApi.md#getboltassemblies) | Gets bolt assemblies used in the specified project.
[**GetBoltGradeMaterials**](docs/MaterialApi.md#getboltgradematerials) | Gets materials used in the specified project.
[**GetConcreteMaterials**](docs/MaterialApi.md#getconcretematerials) | Gets materials used in the specified project.
[**GetCrossSections**](docs/MaterialApi.md#getcrosssections) | Gets cross sections used in the specified project.
[**GetSteelMaterials**](docs/MaterialApi.md#getsteelmaterials) | Gets materials used in the specified project.
[**GetWeldingMaterials**](docs/MaterialApi.md#getweldingmaterials) | Gets materials used in the specified project.
  ### MemberApi

  
  
  Method | Description
  ------------- | -------------
[**GetMember**](docs/MemberApi.md#getmember) | Gets information about the specified member in the connection.
[**GetMembers**](docs/MemberApi.md#getmembers) | Gets information about all members in the connection.
[**SetBearingMember**](docs/MemberApi.md#setbearingmember) | Set bearing member for memberIt
[**UpdateMember**](docs/MemberApi.md#updatemember) | Updates the member in the connection with the provided data.
  ### OperationApi

  
  
  Method | Description
  ------------- | -------------
[**DeleteOperations**](docs/OperationApi.md#deleteoperations) | Delete all operations for the connection
[**GetCommonOperationProperties**](docs/OperationApi.md#getcommonoperationproperties) | Gets common operation properties.
[**GetOperations**](docs/OperationApi.md#getoperations) | Gets the list of operations for the connection.
[**PreDesignWelds**](docs/OperationApi.md#predesignwelds) | Pre-designs welds in the connection.
[**UpdateCommonOperationProperties**](docs/OperationApi.md#updatecommonoperationproperties) | Updates common properties for all operations.
  ### ParameterApi

  
  
  Method | Description
  ------------- | -------------
[**DeleteParameters**](docs/ParameterApi.md#deleteparameters) | Delete all parameters and parameter model links for the connection connectionId in the project projectId
[**EvaluateExpression**](docs/ParameterApi.md#evaluateexpression) | Evaluate the expression and return the result.  For more details see documentation about parameters:  https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html  or  https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html
[**GetParameters**](docs/ParameterApi.md#getparameters) | Gets all parameters defined for the specified project and connection.
[**Update**](docs/ParameterApi.md#update) | Updates parameters for the specified connection in the project with the values provided.
  ### PresentationApi

  
  
  Method | Description
  ------------- | -------------
[**GetDataScene3D**](docs/PresentationApi.md#getdatascene3d) | Returns data for Scene3D visualization.
[**GetDataScene3DText**](docs/PresentationApi.md#getdatascene3dtext) | Returns serialized data for Scene3D in JSON format.
  ### ProjectApi

  
  
  Method | Description
  ------------- | -------------
[**CloseProject**](docs/ProjectApi.md#closeproject) | Closes the project and releases resources in the service.
[**DownloadProject^**](docs/ProjectApi.md#downloadproject) | Downloads the current IdeaCon project from the service, including all changes made by previous API calls.
[**GetActiveProjects**](docs/ProjectApi.md#getactiveprojects) | Gets the list of projects in the service that were opened by the client connected via M:IdeaStatiCa.ConnectionRestApi.Controllers.ClientController.ConnectClient.
[**GetProjectData**](docs/ProjectApi.md#getprojectdata) | Get data of the project.
[**ImportIOM^**](docs/ProjectApi.md#importiom) | Create the IDEA Connection project from IOM provided in xml format.  The parameter 'containerXmlFile' passed in HTTP body represents :  [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  which is serialized to XML string by  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs)
[**OpenProject^**](docs/ProjectApi.md#openproject) | Opens an IdeaCon project from the provided file.
[**UpdateFromIOM^**](docs/ProjectApi.md#updatefromiom) | Update the IDEA Connection project by [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  (model and results).  IOM is passed in the body of the request as the xml string.  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs) should be used to generate the valid xml string
[**UpdateProjectData**](docs/ProjectApi.md#updateprojectdata) | Updates ConProjectData of project
  ### ReportApi

  
  
  Method | Description
  ------------- | -------------
[**GeneratePdf^**](docs/ReportApi.md#generatepdf) | Generates a report for the specified connection in PDF or Word format.
[**GeneratePdfForMutliple^**](docs/ReportApi.md#generatepdfformutliple) | Generates a report for multiple connections in PDF or Word format.
[**GenerateWord^**](docs/ReportApi.md#generateword) | Generates a report for the specified connection in PDF or Word format.
[**GenerateWordForMultiple^**](docs/ReportApi.md#generatewordformultiple) | Generates a report for multiple connections in PDF or Word format.
  ### SettingsApi

  
  
  Method | Description
  ------------- | -------------
[**GetSettings**](docs/SettingsApi.md#getsettings) | Gets setting values for the project.
[**UpdateSettings**](docs/SettingsApi.md#updatesettings) | Updates one or multiple setting values in the project.
  ### TemplateApi

  
  
  Method | Description
  ------------- | -------------
[**ApplyTemplate**](docs/TemplateApi.md#applytemplate) | Apply the connection template applyTemplateParam on the connection connectionId in the project projectId
[**CreateConTemplate**](docs/TemplateApi.md#createcontemplate) | Create a template for the connection connectionId in the project projectId
[**Delete**](docs/TemplateApi.md#delete) | Delete specific template
[**DeleteAll**](docs/TemplateApi.md#deleteall) | Delete all templates in connection
[**Explode**](docs/TemplateApi.md#explode) | Explode specific template (delete parameters, keep operations)
[**ExplodeAll**](docs/TemplateApi.md#explodeall) | Explode all templates (delete parameters, keep operations)
[**GetDefaultTemplateMapping**](docs/TemplateApi.md#getdefaulttemplatemapping) | Get the default mappings for the application of the connection template passed in templateToApply  on connectionId in the project projectId
[**GetTemplateCommonOperationProperties**](docs/TemplateApi.md#gettemplatecommonoperationproperties) | Get Common properties for specific template
[**GetTemplateInConnection**](docs/TemplateApi.md#gettemplateinconnection) | Retrieves a specific template by its ID for a given connection within a project.
[**GetTemplatesInConnection**](docs/TemplateApi.md#gettemplatesinconnection) | Retrieves a list of templates associated with a specific connection within a project.
[**LoadDefaults**](docs/TemplateApi.md#loaddefaults) | Load parameter defaults for specific template.
[**UpdateTemplateCommonOperationProperties**](docs/TemplateApi.md#updatetemplatecommonoperationproperties) | Set common properties for specific template

<a id="documentation-for-models"></a>
## Documentation for Models

 - [Model.AnchorGrid](docs/AnchorGrid.md)
 - [Model.AnchorType](docs/AnchorType.md)
 - [Model.BaseTemplateConversion](docs/BaseTemplateConversion.md)
 - [Model.BeamData](docs/BeamData.md)
 - [Model.BendData](docs/BendData.md)
 - [Model.BoltGrid](docs/BoltGrid.md)
 - [Model.BoltShearType](docs/BoltShearType.md)
 - [Model.BucklingRes](docs/BucklingRes.md)
 - [Model.CheckResAnchor](docs/CheckResAnchor.md)
 - [Model.CheckResBolt](docs/CheckResBolt.md)
 - [Model.CheckResConcreteBlock](docs/CheckResConcreteBlock.md)
 - [Model.CheckResPlate](docs/CheckResPlate.md)
 - [Model.CheckResSummary](docs/CheckResSummary.md)
 - [Model.CheckResWeld](docs/CheckResWeld.md)
 - [Model.ConAlignedPlate](docs/ConAlignedPlate.md)
 - [Model.ConAlignedPlateSideCodeEnum](docs/ConAlignedPlateSideCodeEnum.md)
 - [Model.ConAnalysisTypeEnum](docs/ConAnalysisTypeEnum.md)
 - [Model.ConConnection](docs/ConConnection.md)
 - [Model.ConConnectionLibrarySearchParameters](docs/ConConnectionLibrarySearchParameters.md)
 - [Model.ConConnectionTemplate](docs/ConConnectionTemplate.md)
 - [Model.ConConversionSettings](docs/ConConversionSettings.md)
 - [Model.ConDesignItem](docs/ConDesignItem.md)
 - [Model.ConDesignSet](docs/ConDesignSet.md)
 - [Model.ConDesignSetType](docs/ConDesignSetType.md)
 - [Model.ConItem](docs/ConItem.md)
 - [Model.ConLoadEffect](docs/ConLoadEffect.md)
 - [Model.ConLoadEffectMemberLoad](docs/ConLoadEffectMemberLoad.md)
 - [Model.ConLoadEffectPositionEnum](docs/ConLoadEffectPositionEnum.md)
 - [Model.ConLoadEffectSectionLoad](docs/ConLoadEffectSectionLoad.md)
 - [Model.ConLoadSettings](docs/ConLoadSettings.md)
 - [Model.ConMember](docs/ConMember.md)
 - [Model.ConMemberAlignmentTypeEnum](docs/ConMemberAlignmentTypeEnum.md)
 - [Model.ConMemberForcesInEnum](docs/ConMemberForcesInEnum.md)
 - [Model.ConMemberModel](docs/ConMemberModel.md)
 - [Model.ConMemberPlacementDefinitionTypeEnum](docs/ConMemberPlacementDefinitionTypeEnum.md)
 - [Model.ConMemberPlatePartTypeEnum](docs/ConMemberPlatePartTypeEnum.md)
 - [Model.ConMemberPosition](docs/ConMemberPosition.md)
 - [Model.ConMprlCrossSection](docs/ConMprlCrossSection.md)
 - [Model.ConMprlElement](docs/ConMprlElement.md)
 - [Model.ConNonConformityIssue](docs/ConNonConformityIssue.md)
 - [Model.ConNonConformityIssueSeverity](docs/ConNonConformityIssueSeverity.md)
 - [Model.ConOperation](docs/ConOperation.md)
 - [Model.ConOperationCommonProperties](docs/ConOperationCommonProperties.md)
 - [Model.ConProductionCost](docs/ConProductionCost.md)
 - [Model.ConProject](docs/ConProject.md)
 - [Model.ConProjectData](docs/ConProjectData.md)
 - [Model.ConResultSummary](docs/ConResultSummary.md)
 - [Model.ConTemplateApplyParam](docs/ConTemplateApplyParam.md)
 - [Model.ConTemplateApplyResult](docs/ConTemplateApplyResult.md)
 - [Model.ConTemplateMappingGetParam](docs/ConTemplateMappingGetParam.md)
 - [Model.ConTemplatePublishParam](docs/ConTemplatePublishParam.md)
 - [Model.ConWeldSizingMethodEnum](docs/ConWeldSizingMethodEnum.md)
 - [Model.ConcreteBlock](docs/ConcreteBlock.md)
 - [Model.ConcreteBlockData](docs/ConcreteBlockData.md)
 - [Model.ConnectionCheckRes](docs/ConnectionCheckRes.md)
 - [Model.ConnectionData](docs/ConnectionData.md)
 - [Model.ConversionMapping](docs/ConversionMapping.md)
 - [Model.CountryCode](docs/CountryCode.md)
 - [Model.CutBeamByBeamData](docs/CutBeamByBeamData.md)
 - [Model.CutData](docs/CutData.md)
 - [Model.CutMethod](docs/CutMethod.md)
 - [Model.CutOrientation](docs/CutOrientation.md)
 - [Model.CutPart](docs/CutPart.md)
 - [Model.DistanceComparison](docs/DistanceComparison.md)
 - [Model.DrawData](docs/DrawData.md)
 - [Model.FoldedPlateData](docs/FoldedPlateData.md)
 - [Model.IGroup](docs/IGroup.md)
 - [Model.IdeaParameter](docs/IdeaParameter.md)
 - [Model.IdeaParameterUpdate](docs/IdeaParameterUpdate.md)
 - [Model.IdeaParameterValidation](docs/IdeaParameterValidation.md)
 - [Model.IdeaParameterValidationResponse](docs/IdeaParameterValidationResponse.md)
 - [Model.Line](docs/Line.md)
 - [Model.MessageNumber](docs/MessageNumber.md)
 - [Model.OpenElementId](docs/OpenElementId.md)
 - [Model.OpenMessage](docs/OpenMessage.md)
 - [Model.OpenMessages](docs/OpenMessages.md)
 - [Model.ParameterUpdateResponse](docs/ParameterUpdateResponse.md)
 - [Model.PinGrid](docs/PinGrid.md)
 - [Model.PlateData](docs/PlateData.md)
 - [Model.Point2D](docs/Point2D.md)
 - [Model.Point3D](docs/Point3D.md)
 - [Model.PolyLine2D](docs/PolyLine2D.md)
 - [Model.ReferenceElement](docs/ReferenceElement.md)
 - [Model.Region2D](docs/Region2D.md)
 - [Model.SearchOption](docs/SearchOption.md)
 - [Model.Segment2D](docs/Segment2D.md)
 - [Model.Selected](docs/Selected.md)
 - [Model.SelectedType](docs/SelectedType.md)
 - [Model.TemplateConversions](docs/TemplateConversions.md)
 - [Model.Text](docs/Text.md)
 - [Model.TextPosition](docs/TextPosition.md)
 - [Model.Vector3D](docs/Vector3D.md)
 - [Model.WeldData](docs/WeldData.md)
 - [Model.WeldType](docs/WeldType.md)


<a id="documentation-for-authorization"></a>
## Documentation for Authorization

Endpoints do not require authorization.


## Notes

This C# SDK is automatically generated by the [OpenAPI Generator](https://openapi-generator.tech) project:

- API version: 3.0
- SDK version: 25.1.3.1326
- Generator version: 7.9.0
- Build package: org.openapitools.codegen.languages.CSharpClientCodegen
    For more information, please visit [https://github.com/idea-statica/ideastatica-public](https://github.com/idea-statica/ideastatica-public)
