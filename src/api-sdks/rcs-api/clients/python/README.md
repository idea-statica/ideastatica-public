# ideastatica-rcs-api

The Python package for the RCS Rest API 1.0

- API version: 1.0
- Package version: 24.1.3.2377

IDEA StatiCa RCS API, used for the automated design and calculation of reinforced concrete sections.

## Requirements.

Python 3.7+

## Installation

### pip install 

We reccomend using pip to install the package into your environment.

```sh
pip install ideastatica_rcs_api
```

Then import the package in your project:
```python
import ideastatica_rcs_api
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

To start the service, manually navigate to the "C:\Program Files\IDEA StatiCa\StatiCa 24.0" folder. Using CLI:

```console
IdeaStatiCa.ConnectionRestApi.exe -port:5000
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
[**calculate**](docs/CalculationApi.md#calculate) | Calculate RCS project
[**get_results**](docs/CalculationApi.md#get_results) | Get calculated results
  ### CrossSectionApi

  
  
  Method | Description
  ------------- | -------------
[**import_reinforced_cross_section**](docs/CrossSectionApi.md#import_reinforced_cross_section) | Import reinforced cross-section
[**reinforced_cross_sections**](docs/CrossSectionApi.md#reinforced_cross_sections) | Get reinforced cross sections
  ### DesignMemberApi

  
  
  Method | Description
  ------------- | -------------
[**members**](docs/DesignMemberApi.md#members) | Get members
  ### InternalForcesApi

  
  
  Method | Description
  ------------- | -------------
[**get_section_loading**](docs/InternalForcesApi.md#get_section_loading) | Get section loading
[**set_section_loading**](docs/InternalForcesApi.md#set_section_loading) | Set section loading
  ### ProjectApi

  
  
  Method | Description
  ------------- | -------------
[**close_project**](docs/ProjectApi.md#close_project) | 
[**download_project**](docs/ProjectApi.md#download_project) | Download the actual rcs project from the service. It includes all changes which were made by previous API calls.
[**get_active_project**](docs/ProjectApi.md#get_active_project) | 
[**get_code_settings**](docs/ProjectApi.md#get_code_settings) | 
[**import_iom**](docs/ProjectApi.md#import_iom) | 
[**import_iom_file**](docs/ProjectApi.md#import_iom_file) | 
[**open**](docs/ProjectApi.md#open) | 
[**open_project**](docs/ProjectApi.md#open_project) | Open Rcs project from rcsFile
[**update_code_settings**](docs/ProjectApi.md#update_code_settings) | 
  ### SectionApi

  
  
  Method | Description
  ------------- | -------------
[**sections**](docs/SectionApi.md#sections) | Get sections
[**update_section**](docs/SectionApi.md#update_section) | Update a section in the RCS project

<a id="documentation-for-models"></a>
## Documentation for Models

 - [ideastatica_rcs_api.models.CalculationType](docs/CalculationType.md)
 - [ideastatica_rcs_api.models.CheckResult](docs/CheckResult.md)
 - [ideastatica_rcs_api.models.CheckResultType](docs/CheckResultType.md)
 - [ideastatica_rcs_api.models.ConcreteCheckResult](docs/ConcreteCheckResult.md)
 - [ideastatica_rcs_api.models.ConcreteCheckResultBase](docs/ConcreteCheckResultBase.md)
 - [ideastatica_rcs_api.models.ConcreteCheckResultOverall](docs/ConcreteCheckResultOverall.md)
 - [ideastatica_rcs_api.models.ConcreteCheckResultOverallItem](docs/ConcreteCheckResultOverallItem.md)
 - [ideastatica_rcs_api.models.ConcreteCheckResults](docs/ConcreteCheckResults.md)
 - [ideastatica_rcs_api.models.Loading](docs/Loading.md)
 - [ideastatica_rcs_api.models.LoadingType](docs/LoadingType.md)
 - [ideastatica_rcs_api.models.NonConformity](docs/NonConformity.md)
 - [ideastatica_rcs_api.models.NonConformityIssue](docs/NonConformityIssue.md)
 - [ideastatica_rcs_api.models.NonConformitySeverity](docs/NonConformitySeverity.md)
 - [ideastatica_rcs_api.models.RcsCalculationParameters](docs/RcsCalculationParameters.md)
 - [ideastatica_rcs_api.models.RcsCheckMember](docs/RcsCheckMember.md)
 - [ideastatica_rcs_api.models.RcsProject](docs/RcsProject.md)
 - [ideastatica_rcs_api.models.RcsProjectData](docs/RcsProjectData.md)
 - [ideastatica_rcs_api.models.RcsReinforcedCrossSection](docs/RcsReinforcedCrossSection.md)
 - [ideastatica_rcs_api.models.RcsReinforcedCrossSectionImportData](docs/RcsReinforcedCrossSectionImportData.md)
 - [ideastatica_rcs_api.models.RcsReinforcedCrosssSectionImportSetting](docs/RcsReinforcedCrosssSectionImportSetting.md)
 - [ideastatica_rcs_api.models.RcsResultParameters](docs/RcsResultParameters.md)
 - [ideastatica_rcs_api.models.RcsSection](docs/RcsSection.md)
 - [ideastatica_rcs_api.models.RcsSectionLoading](docs/RcsSectionLoading.md)
 - [ideastatica_rcs_api.models.RcsSectionResultDetailed](docs/RcsSectionResultDetailed.md)
 - [ideastatica_rcs_api.models.RcsSectionResultOverview](docs/RcsSectionResultOverview.md)
 - [ideastatica_rcs_api.models.RcsSetting](docs/RcsSetting.md)
 - [ideastatica_rcs_api.models.ResultOfInternalForces](docs/ResultOfInternalForces.md)
 - [ideastatica_rcs_api.models.ResultOfLoading](docs/ResultOfLoading.md)
 - [ideastatica_rcs_api.models.ResultOfLoadingItem](docs/ResultOfLoadingItem.md)
 - [ideastatica_rcs_api.models.SectionConcreteCheckResult](docs/SectionConcreteCheckResult.md)



## Notes

This Python package is automatically generated by the [OpenAPI Generator](https://openapi-generator.tech) project:

- API version: 1.0
- Package version: 24.1.3.2377
- Generator version: 7.9.0
- Build package: org.openapitools.codegen.languages.PythonClientCodegen
For more information, please visit [https://github.com/idea-statica/ideastatica-public](https://github.com/idea-statica/ideastatica-public)
