# CrossSectionApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**import_reinforced_cross_section**](CrossSectionApi.md#import_reinforced_cross_section) | Import reinforced cross-section
[**reinforced_cross_sections**](CrossSectionApi.md#reinforced_cross_sections) | Get reinforced cross sections


<a id="import_reinforced_cross_section"></a>
# **import_reinforced_cross_section**
> RcsReinforcedCrossSection import_reinforced_cross_section(project_id, rcs_reinforced_cross_section_import_data=rcs_reinforced_cross_section_import_data)

Import reinforced cross-section

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **rcs_reinforced_cross_section_import_data** | [**RcsReinforcedCrossSectionImportData**](RcsReinforcedCrossSectionImportData.md)|  | [optional] 

### Return type

[**RcsReinforcedCrossSection**](RcsReinforcedCrossSection.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_reinforced_cross_section import RcsReinforcedCrossSection
from ideastatica_rcs_api.models.rcs_reinforced_cross_section_import_data import RcsReinforcedCrossSectionImportData
from ideastatica_rcs_api.rest import ApiException
from pprint import pprint

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_rcs_api.Configuration(
    host = "http://localhost"
)


# Enter a context with an instance of the API client
with ideastatica_rcs_api.ApiClient(configuration) as api_client:
    
    # Create an instance of the API class
    api_instance = ideastatica_rcs_api.CrossSectionApi(api_client)
    project_id = 'project_id_example' # str | 
    rcs_reinforced_cross_section_import_data = ideastatica_rcs_api.RcsReinforcedCrossSectionImportData() # RcsReinforcedCrossSectionImportData |  (optional)

    try:
        # Import reinforced cross-section
        api_response = api_instance.import_reinforced_cross_section(project_id, rcs_reinforced_cross_section_import_data=rcs_reinforced_cross_section_import_data)
        print("The response of CrossSectionApi->import_reinforced_cross_section:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling CrossSectionApi->import_reinforced_cross_section: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/cross-sections/import-reinforced-cross-section 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/xml, text/xml, application/*+xml, application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="reinforced_cross_sections"></a>
# **reinforced_cross_sections**
> List[RcsReinforcedCrossSection] reinforced_cross_sections(project_id)

Get reinforced cross sections

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 

### Return type

[**List[RcsReinforcedCrossSection]**](RcsReinforcedCrossSection.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_reinforced_cross_section import RcsReinforcedCrossSection
from ideastatica_rcs_api.rest import ApiException
from pprint import pprint

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_rcs_api.Configuration(
    host = "http://localhost"
)


# Enter a context with an instance of the API client
with ideastatica_rcs_api.ApiClient(configuration) as api_client:
    
    # Create an instance of the API class
    api_instance = ideastatica_rcs_api.CrossSectionApi(api_client)
    project_id = 'project_id_example' # str | 

    try:
        # Get reinforced cross sections
        api_response = api_instance.reinforced_cross_sections(project_id)
        print("The response of CrossSectionApi->reinforced_cross_sections:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling CrossSectionApi->reinforced_cross_sections: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/cross-sections/reinforced-cross-sections 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

