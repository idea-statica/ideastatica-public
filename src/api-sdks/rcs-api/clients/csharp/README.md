# IdeaStatiCa.RcsApi

The C# library for the RCS Rest API 1.0

- API version: 1.0
- SDK version: 24.1.2.1474

IDEA StatiCa RCS API, used for the automated design and calculation of reinforced concrete sections.

<a id="frameworks-supported"></a>
## Frameworks supported
- .NET Core >=1.0
- .NET Framework >=4.6
- Mono/Xamarin >=vNext

<a id="dependencies"></a>
## Dependencies

- [RestSharp](https://www.nuget.org/packages/RestSharp) - 106.13.0 or later
- [Json.NET](https://www.nuget.org/packages/Newtonsoft.Json/) - 13.0.2 or later
- [JsonSubTypes](https://www.nuget.org/packages/JsonSubTypes/) - 1.8.0 or later

The DLLs included in the package may not be the latest version. We recommend using [NuGet](https://docs.nuget.org/consume/installing-nuget) to obtain the latest version of the packages:
```
Install-Package RestSharp
Install-Package Newtonsoft.Json
Install-Package JsonSubTypes
```

NOTE: RestSharp versions greater than 105.1.0 have a bug which causes file uploads to fail. See [RestSharp#742](https://github.com/restsharp/RestSharp/issues/742).
NOTE: RestSharp for .Net Core creates a new socket for each api call, which can lead to a socket exhaustion problem. See [RestSharp#1406](https://github.com/restsharp/RestSharp/issues/1406).

<a id="installation"></a>
## Installation

### NuGet package

We recommend installing the package to your project using NuGet. This will ensure all dependencies are automatically installed.

### Building from source

Generate the DLL using your preferred tool (e.g. `dotnet build`) or opening the avaliable Visual Studio solution (.sln) and building.

Then include the DLL (under the `bin` folder) in the C# project.

Once included in your project, add the following namespaces:
```csharp
using IdeaStatiCa.RcsApi.Api;
using IdeaStatiCa.RcsApi.Client;
using IdeaStatiCa.RcsApi.Model;
```


<a id="usage"></a>
## Usage

`ClientApiClientFactory` manages creation of clients on the running service. 
We currently only support connecting to a service running on a localhost (eg. 'http://localhost:5000/').

To start the service, manually navigate to the "C:\Program Files\IDEA StatiCa\StatiCa 24.0\net6.0-windows" folder. Using CLI:

```console
IdeaStatiCa.ConnectionRestApi.exe -port:5000
```

```csharp
// Connect any new service to latest version of IDEA StatiCa.
ConnectionApiClientFactory clientFactory = new ConnectionApiClientFactory('http://localhost:5000/');
```

```csharp
IConnectionApiClient conClient = await clientFactory.CreateConnectionApiClient();
```


<a id="getting-started"></a>
## Getting Started

The below snippet shows a simple getting started example which opens an IDEA StatiCa Connection project and performs the calculation.

```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.RcsApi.Api;
using IdeaStatiCa.RcsApi.Client;
using IdeaStatiCa.RcsApi.Model;

namespace Example
{
    public class Example
    {
        public static void Main()
        {
            // Create client factory object. The service will be automatically started at the latest version of IDEA StatiCa.  
            ConnectionApiClientFactory clientFactory = new ConnectionApiClientFactory();
            
            //Creates automatically configured API client.
            using(IConnectionApiClient conClient = await clientFactory.CreateConnectionApiClient())
            {
                //Use project controller to open a project.
                var project = await conClient.Project.Open("myProject.ideaCon");

                //Get projectId Guid
			    Guid projectId = conProject.ProjectId;

                var connections = await conClient.Connection.GetConnectionsAsync(projectId);
			    int connectionId = connections[0].Id;

                //Close the project
                await conClient.Project.Close();

                try
                {
                    ConCalculationParameter calculationParams = new ConCalculationParameter();
                    calculationParams.ConnectionIds = new List<int> { connectionId };

                    //Calculate the first connection of the project.
                    List<ConResultSummary> results = await conClient.Calculation.CalculateAsync(projectId, calculationParams);
                }
                catch (ApiException e)
                {
                    Debug.Print("Exception when calling calculation of the connection: " + e.Message );
                    Debug.Print("Status Code: "+ e.ErrorCode);
                    Debug.Print(e.StackTrace);
                }
                finally
                {
                    await conClient.Project.CloseProjectAsync(projectId);
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
- SDK version: 24.1.2.1474
- Generator version: 7.9.0
- Build package: org.openapitools.codegen.languages.CSharpClientCodegen
    For more information, please visit [https://github.com/idea-statica/ideastatica-public](https://github.com/idea-statica/ideastatica-public)
