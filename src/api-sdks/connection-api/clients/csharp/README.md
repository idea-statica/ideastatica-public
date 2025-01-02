# IdeaStatiCa.ConnectionApi

The C# library for the Connection Rest API 1.0

- API version: 1.0
- SDK version: 24.1.2.1474

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


<a id="usage"></a>
## Usage

`ClientApiClientFactory` manages creation of clients on the running service. 
We currently only support connecting to a service running on a localhost (eg. 'http://localhost:5000/').

To start the service, manually navigate to the "C:\Program Files\IDEA StatiCa\StatiCa 24.0\net6.0-windows" folder. Using CLI:

```console
IdeaStatiCa.ConnectionRestApi.exe -port:5193
```

```csharp
// Connect any new service to latest version of IDEA StatiCa.
ConnectionApiServiceAttacher clientFactory = new ConnectionApiServiceAttacher('http://localhost:5000/');
```

```csharp
IConnectionApiClient conClient = await clientFactory.CreateApiClient();
```

A project can be created by the template or the the wizard in Visual Studio. See [Connection Rest API client wizard](project-template.md).


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
        public static void Main()
        {
            string ideaConFile = "test1.ideaCon";

            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 24.1"; // path to the IdeaStatiCa.ConnectionRestApi.exe

            using(clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                // create ConnectionApiClient
                using (var conClient = await _clientFactory.CreateApiClient())
                {
                    // open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile, cancellationToken);

                    if(!projData.Connections.Any())
                    {
                        return null;
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
[**Calculate**](docs/CalculationApi.md#calculate) | Run CBFEM caluclation and return the summary of the results
[**GetRawJsonResults**](docs/CalculationApi.md#getrawjsonresults) | Get json string which represents raw CBFEM results (an instance of CheckResultsData)
[**GetResults**](docs/CalculationApi.md#getresults) | Get detailed results of the CBFEM analysis
  ### ClientApi

  
  
  Method | Description
  ------------- | -------------
[**ConnectClient**](docs/ClientApi.md#connectclient) | Connect a client to the ConnectionRestApi service. Method returns a unique identifier of the client.
[**GetVersion**](docs/ClientApi.md#getversion) | Get the IdeaStatica version
  ### ConnectionApi

  
  
  Method | Description
  ------------- | -------------
[**GetConnection**](docs/ConnectionApi.md#getconnection) | Get data about a specific connection in the project
[**GetConnections**](docs/ConnectionApi.md#getconnections) | Get data about all connections in the project
[**GetProductionCost**](docs/ConnectionApi.md#getproductioncost) | Get production cost of the connection
[**UpdateConnection**](docs/ConnectionApi.md#updateconnection) | Update data of a specific connection in the project
  ### ExportApi

  
  
  Method | Description
  ------------- | -------------
[**ExportIFC^**](docs/ExportApi.md#exportifc) | Export connection to IFC format
[**ExportIom**](docs/ExportApi.md#exportiom) | Export connection to XML which includes https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs
[**ExportIomConnectionData**](docs/ExportApi.md#exportiomconnectiondata) | Get https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs for required connection
  ### LoadEffectApi

  
  
  Method | Description
  ------------- | -------------
[**AddLoadEffect**](docs/LoadEffectApi.md#addloadeffect) | Add new load effect to the connection
[**DeleteLoadEffect**](docs/LoadEffectApi.md#deleteloadeffect) | Delete load effect loadEffectId
[**GetLoadEffect**](docs/LoadEffectApi.md#getloadeffect) | Get load impulses from loadEffectId
[**GetLoadEffects**](docs/LoadEffectApi.md#getloadeffects) | Get all load effects which are defined in connectionId
[**GetLoadSettings**](docs/LoadEffectApi.md#getloadsettings) | Get Load settings for connection in project
[**SetLoadSettings**](docs/LoadEffectApi.md#setloadsettings) | Set Load settings for connection in project
[**UpdateLoadEffect**](docs/LoadEffectApi.md#updateloadeffect) | Update load impulses in conLoading
  ### MaterialApi

  
  
  Method | Description
  ------------- | -------------
[**AddBoltAssembly**](docs/MaterialApi.md#addboltassembly) | Add bolt assembly to the project
[**AddCrossSection**](docs/MaterialApi.md#addcrosssection) | Add cross section to the project
[**AddMaterialBoltGrade**](docs/MaterialApi.md#addmaterialboltgrade) | Add material to the project
[**AddMaterialConcrete**](docs/MaterialApi.md#addmaterialconcrete) | Add material to the project
[**AddMaterialSteel**](docs/MaterialApi.md#addmaterialsteel) | Add material to the project
[**AddMaterialWeld**](docs/MaterialApi.md#addmaterialweld) | Add material to the project
[**GetAllMaterials**](docs/MaterialApi.md#getallmaterials) | Get materials which are used in the project projectId
[**GetBoltAssemblies**](docs/MaterialApi.md#getboltassemblies) | Get bolt assemblies which are used in the project projectId
[**GetBoltGradeMaterials**](docs/MaterialApi.md#getboltgradematerials) | Get materials which are used in the project projectId
[**GetConcreteMaterials**](docs/MaterialApi.md#getconcretematerials) | Get materials which are used in the project projectId
[**GetCrossSections**](docs/MaterialApi.md#getcrosssections) | Get cross sections which are used in the project projectId
[**GetSteelMaterials**](docs/MaterialApi.md#getsteelmaterials) | Get materials which are used in the project projectId
[**GetWeldingMaterials**](docs/MaterialApi.md#getweldingmaterials) | Get materials which are used in the project projectId
  ### MemberApi

  
  
  Method | Description
  ------------- | -------------
[**GetMember**](docs/MemberApi.md#getmember) | Get information about the requires member in the connection
[**GetMembers**](docs/MemberApi.md#getmembers) | Get information about all members in the connection
[**SetBearingMember**](docs/MemberApi.md#setbearingmember) | Set bearing member for memberIt
[**UpdateMember**](docs/MemberApi.md#updatemember) | Update the member in the connection by newMemberData
  ### OperationApi

  
  
  Method | Description
  ------------- | -------------
[**DeleteOperations**](docs/OperationApi.md#deleteoperations) | Delete all operations for the connection
[**GetCommonOperationProperties**](docs/OperationApi.md#getcommonoperationproperties) | Get common operation properties
[**GetOperations**](docs/OperationApi.md#getoperations) | Get the list of operations for the connection
[**UpdateCommonOperationProperties**](docs/OperationApi.md#updatecommonoperationproperties) | Update common properties for all operations
  ### ParameterApi

  
  
  Method | Description
  ------------- | -------------
[**EvaluateExpression**](docs/ParameterApi.md#evaluateexpression) | Evaluate the expression and return the result
[**GetParameters**](docs/ParameterApi.md#getparameters) | Get all parameters which are defined for projectId and connectionId
[**UpdateParameters**](docs/ParameterApi.md#updateparameters) | Update parameters for the connection connectionId in the project projectId by values passed in parameters
  ### PresentationApi

  
  
  Method | Description
  ------------- | -------------
[**GetDataScene3D**](docs/PresentationApi.md#getdatascene3d) | Returns data for scene3D
[**GetDataScene3DText**](docs/PresentationApi.md#getdatascene3dtext) | Return serialized data for scene3D in json format
  ### ProjectApi

  
  
  Method | Description
  ------------- | -------------
[**CloseProject**](docs/ProjectApi.md#closeproject) | Close the project. Needed for releasing resources in the service.
[**DownloadProject^**](docs/ProjectApi.md#downloadproject) | Download the actual ideacon project from the service. It includes alle changes which were made by previous API calls.
[**GetActiveProjects**](docs/ProjectApi.md#getactiveprojects) | Get the list of projects in the service which were opened by the client which was connected by M:IdeaStatiCa.ConnectionRestApi.Controllers.ClientController.ConnectClient
[**GetProjectData**](docs/ProjectApi.md#getprojectdata) | Get data of the project.
[**GetSetup**](docs/ProjectApi.md#getsetup) | Get setup from project
[**ImportIOM^**](docs/ProjectApi.md#importiom) | Create the IDEA Connection project from IOM provided in xml format.  The parameter 'containerXmlFile' passed in HTTP body represents :  <see href=\"https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs\">IdeaRS.OpenModel.OpenModelContainer</see>  which is serialized to XML string by  <see href=\"https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs\">IdeaRS.OpenModel.Tools.OpenModelContainerToXml</see>
[**OpenProject^**](docs/ProjectApi.md#openproject) | Open ideacon project from ideaConFile
[**UpdateFromIOM^**](docs/ProjectApi.md#updatefromiom) | Update the IDEA Connection project by <see href=\"https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs\">IdeaRS.OpenModel.OpenModelContainer</see>  (model and results).  IOM is passed in the body of the request as the xml string.  <see href=\"https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs\">IdeaRS.OpenModel.Tools.OpenModelContainerToXml</see> should be used to generate the valid xml string
[**UpdateProjectData**](docs/ProjectApi.md#updateprojectdata) | Updates ConProjectData of project
[**UpdateSetup**](docs/ProjectApi.md#updatesetup) | Update setup of the project
  ### ReportApi

  
  
  Method | Description
  ------------- | -------------
[**GeneratePdf^**](docs/ReportApi.md#generatepdf) | Generates report for projectId and connectionId
[**GenerateWord^**](docs/ReportApi.md#generateword) | Generates report for projectId and connectionId
  ### TemplateApi

  
  
  Method | Description
  ------------- | -------------
[**ApplyTemplate**](docs/TemplateApi.md#applytemplate) | Apply the connection template applyTemplateParam on the connection connectionId in the project projectId
[**CreateConTemplate**](docs/TemplateApi.md#createcontemplate) | Create a template for the connection connectionId in the project projectId
[**GetDefaultTemplateMapping**](docs/TemplateApi.md#getdefaulttemplatemapping) | Get the default mappings for the application of the connection template passed in templateToApply  on connectionId in the project projectId

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
 - [Model.ConAnalysisTypeEnum](docs/ConAnalysisTypeEnum.md)
 - [Model.ConCalculationParameter](docs/ConCalculationParameter.md)
 - [Model.ConConnection](docs/ConConnection.md)
 - [Model.ConLoadEffect](docs/ConLoadEffect.md)
 - [Model.ConLoadEffectMemberLoad](docs/ConLoadEffectMemberLoad.md)
 - [Model.ConLoadEffectPositionEnum](docs/ConLoadEffectPositionEnum.md)
 - [Model.ConLoadEffectSectionLoad](docs/ConLoadEffectSectionLoad.md)
 - [Model.ConLoadSettings](docs/ConLoadSettings.md)
 - [Model.ConMember](docs/ConMember.md)
 - [Model.ConMprlCrossSection](docs/ConMprlCrossSection.md)
 - [Model.ConMprlElement](docs/ConMprlElement.md)
 - [Model.ConOperation](docs/ConOperation.md)
 - [Model.ConOperationCommonProperties](docs/ConOperationCommonProperties.md)
 - [Model.ConProductionCost](docs/ConProductionCost.md)
 - [Model.ConProject](docs/ConProject.md)
 - [Model.ConProjectData](docs/ConProjectData.md)
 - [Model.ConResultSummary](docs/ConResultSummary.md)
 - [Model.ConTemplateApplyParam](docs/ConTemplateApplyParam.md)
 - [Model.ConTemplateMappingGetParam](docs/ConTemplateMappingGetParam.md)
 - [Model.ConcreteBlock](docs/ConcreteBlock.md)
 - [Model.ConcreteBlockData](docs/ConcreteBlockData.md)
 - [Model.ConcreteSetup](docs/ConcreteSetup.md)
 - [Model.ConeBreakoutCheckType](docs/ConeBreakoutCheckType.md)
 - [Model.ConnectionCheckRes](docs/ConnectionCheckRes.md)
 - [Model.ConnectionData](docs/ConnectionData.md)
 - [Model.ConnectionSetup](docs/ConnectionSetup.md)
 - [Model.CrtCompCheckIS](docs/CrtCompCheckIS.md)
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
 - [Model.Line](docs/Line.md)
 - [Model.MessageNumber](docs/MessageNumber.md)
 - [Model.OpenElementId](docs/OpenElementId.md)
 - [Model.OpenMessage](docs/OpenMessage.md)
 - [Model.OpenMessages](docs/OpenMessages.md)
 - [Model.PinGrid](docs/PinGrid.md)
 - [Model.PlateData](docs/PlateData.md)
 - [Model.Point2D](docs/Point2D.md)
 - [Model.Point3D](docs/Point3D.md)
 - [Model.PolyLine2D](docs/PolyLine2D.md)
 - [Model.ReferenceElement](docs/ReferenceElement.md)
 - [Model.Region2D](docs/Region2D.md)
 - [Model.Segment2D](docs/Segment2D.md)
 - [Model.Selected](docs/Selected.md)
 - [Model.SelectedType](docs/SelectedType.md)
 - [Model.TemplateConversions](docs/TemplateConversions.md)
 - [Model.Text](docs/Text.md)
 - [Model.TextPosition](docs/TextPosition.md)
 - [Model.Vector3D](docs/Vector3D.md)
 - [Model.WeldData](docs/WeldData.md)
 - [Model.WeldEvaluation](docs/WeldEvaluation.md)
 - [Model.WeldType](docs/WeldType.md)


<a id="documentation-for-authorization"></a>
## Documentation for Authorization

Endpoints do not require authorization.


## Notes

This C# SDK is automatically generated by the [OpenAPI Generator](https://openapi-generator.tech) project:

- API version: 1.0
- SDK version: 24.1.3.0752
- Generator version: 7.9.0
- Build package: org.openapitools.codegen.languages.CSharpClientCodegen
    For more information, please visit [https://github.com/idea-statica/ideastatica-public](https://github.com/idea-statica/ideastatica-public)
