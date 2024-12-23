# IdeaStatiCa.RcsApi

The C# library for the RCS Rest API 1.0

- API version: 1.0
- SDK version: 24.1.2.1474

IDEA StatiCa RCS API, used for the automated design and calculation of reinforced concrete sections.

<a id="frameworks-supported"></a>
## Frameworks supported
- .NET Standard >=2.0

<a id="installation"></a>
## Installation

### NuGet package

We recommend installing the package to your project using NuGet. This will ensure all dependencies are automatically installed.
See [IdeaStatiCa.RcsApi](https://www.nuget.org/packages/IdeaStatiCa.RcsApi/)

### Building from source

Generate the DLL using your preferred tool (e.g. `dotnet build`) or opening the avaliable Visual Studio solution (.sln) and building. See [RcsRestApiClient.sln](RcsRestApiClient.sln)

Projects can be created by the template or the the wizard in Visual Studio. See [Rcs Rest API client wizard](project-template.md).



<a id="usage"></a>
## Usage

`RcsApiServiceAttacher` manages creation of clients on the running service. 
We currently only support connecting to a service running on a localhost (eg. 'http://localhost:5000/').

To start the service, manually navigate to the "C:\Program Files\IDEA StatiCa\StatiCa 24.0\net6.0-windows" folder. Using CLI:

```console
IdeaStatiCa.RcsRestApi.exe -port:5000
```

```csharp
// Connect any new service to latest version of IDEA StatiCa.
RcsApiServiceAttacher clientFactory = new RcsApiServiceAttacher('http://localhost:5000/');
```

```csharp
IRcsApiClient conClient = await clientFactory.CreateApiClient();
```


<a id="getting-started"></a>
## Getting Started

The below snippet shows a simple getting started example which opens an IDEA StatiCa Connection project and performs the calculation.

```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.RcsApi;

namespace Example
{
    public class Example
    {
        public static void Main()
        {
            string rcsFile = "myRcsProject.ideaRcs"; // path to the RCS project file
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 24.1"; // path to the IdeaStatiCa.ConnectionRestApi.exe

            // Create client factory object. The service will be automatically started at the latest version of IDEA StatiCa.  
            using(var clientFactory = new RcsApiServiceRunner(ideaStatiCaPath))
            {  
                //Creates automatically configured API client.
                using (var rcsClient = await clientFactory.CreateApiClient())
                {
                    // open the project and get its id
                    var projData = await rcsClient.Project.OpenProjectAsync(rcsFile, cancellationToken);

                    if(!projData.Sections.Any())
                    {
                        return null;
                    }

                    RcsCalculationParameters rcsCalcParam = new RcsCalculationParameters()
                    {
                        Sections = projData.Sections.Select(s => s.Id).ToList()
                    };
                    
                    var rcsSectResults = await rcsClient.Calculation.CalculateAsync(projData.ProjectId, rcsCalcParam, 0, cancellationToken);

                    await rcsClient.Project.CloseProjectAsync(projData.ProjectId);
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
[**Calculate**](docs/CalculationApi.md#calculate) | Calculate RCS project
[**GetResults**](docs/CalculationApi.md#getresults) | Get calculated results
  ### CrossSectionApi

  
  
  Method | Description
  ------------- | -------------
[**ImportReinforcedCrossSection**](docs/CrossSectionApi.md#importreinforcedcrosssection) | Import reinforced cross-section
[**ReinforcedCrossSections**](docs/CrossSectionApi.md#reinforcedcrosssections) | Get reinforced cross sections
  ### DesignMemberApi

  
  
  Method | Description
  ------------- | -------------
[**Members**](docs/DesignMemberApi.md#members) | Get members
  ### InternalForcesApi

  
  
  Method | Description
  ------------- | -------------
[**GetSectionLoading**](docs/InternalForcesApi.md#getsectionloading) | Get section loading
[**SetSectionLoading**](docs/InternalForcesApi.md#setsectionloading) | Set section loading
  ### ProjectApi

  
  
  Method | Description
  ------------- | -------------
[**CloseProject**](docs/ProjectApi.md#closeproject) | 
[**DownloadProject**](docs/ProjectApi.md#downloadproject) | Download the actual rcs project from the service. It includes all changes which were made by previous API calls.
[**GetActiveProject**](docs/ProjectApi.md#getactiveproject) | 
[**GetCodeSettings**](docs/ProjectApi.md#getcodesettings) | 
[**ImportIOM**](docs/ProjectApi.md#importiom) | 
[**ImportIOMFile**](docs/ProjectApi.md#importiomfile) | 
[**Open**](docs/ProjectApi.md#open) | 
[**OpenProject**](docs/ProjectApi.md#openproject) | Open Rcs project from rcsFile
[**UpdateCodeSettings**](docs/ProjectApi.md#updatecodesettings) | 
  ### SectionApi

  
  
  Method | Description
  ------------- | -------------
[**Sections**](docs/SectionApi.md#sections) | Get sections
[**UpdateSection**](docs/SectionApi.md#updatesection) | Update a section in the RCS project

<a id="documentation-for-models"></a>
## Documentation for Models

 - [Model.CalculationType](docs/CalculationType.md)
 - [Model.CheckResult](docs/CheckResult.md)
 - [Model.CheckResultType](docs/CheckResultType.md)
 - [Model.ConcreteCheckResult](docs/ConcreteCheckResult.md)
 - [Model.ConcreteCheckResultBase](docs/ConcreteCheckResultBase.md)
 - [Model.ConcreteCheckResultOverall](docs/ConcreteCheckResultOverall.md)
 - [Model.ConcreteCheckResultOverallItem](docs/ConcreteCheckResultOverallItem.md)
 - [Model.ConcreteCheckResults](docs/ConcreteCheckResults.md)
 - [Model.Loading](docs/Loading.md)
 - [Model.LoadingType](docs/LoadingType.md)
 - [Model.NonConformity](docs/NonConformity.md)
 - [Model.NonConformityIssue](docs/NonConformityIssue.md)
 - [Model.NonConformitySeverity](docs/NonConformitySeverity.md)
 - [Model.RcsCalculationParameters](docs/RcsCalculationParameters.md)
 - [Model.RcsCheckMember](docs/RcsCheckMember.md)
 - [Model.RcsProject](docs/RcsProject.md)
 - [Model.RcsProjectData](docs/RcsProjectData.md)
 - [Model.RcsReinforcedCrossSection](docs/RcsReinforcedCrossSection.md)
 - [Model.RcsReinforcedCrossSectionImportData](docs/RcsReinforcedCrossSectionImportData.md)
 - [Model.RcsReinforcedCrosssSectionImportSetting](docs/RcsReinforcedCrosssSectionImportSetting.md)
 - [Model.RcsResultParameters](docs/RcsResultParameters.md)
 - [Model.RcsSection](docs/RcsSection.md)
 - [Model.RcsSectionLoading](docs/RcsSectionLoading.md)
 - [Model.RcsSectionResultDetailed](docs/RcsSectionResultDetailed.md)
 - [Model.RcsSectionResultOverview](docs/RcsSectionResultOverview.md)
 - [Model.RcsSetting](docs/RcsSetting.md)
 - [Model.ResultOfInternalForces](docs/ResultOfInternalForces.md)
 - [Model.ResultOfLoading](docs/ResultOfLoading.md)
 - [Model.ResultOfLoadingItem](docs/ResultOfLoadingItem.md)
 - [Model.SectionConcreteCheckResult](docs/SectionConcreteCheckResult.md)


<a id="documentation-for-authorization"></a>
## Documentation for Authorization

Endpoints do not require authorization.


## Notes

This C# SDK is automatically generated by the [OpenAPI Generator](https://openapi-generator.tech) project:

- API version: 1.0
- SDK version: 24.1.3.0764
- Generator version: 7.9.0
- Build package: org.openapitools.codegen.languages.CSharpClientCodegen
    For more information, please visit [https://github.com/idea-statica/ideastatica-public](https://github.com/idea-statica/ideastatica-public)
