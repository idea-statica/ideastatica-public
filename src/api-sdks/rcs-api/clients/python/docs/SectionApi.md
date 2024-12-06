# SectionApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**sections**](SectionApi.md#sections) | Get sections
[**update_section**](SectionApi.md#update_section) | Update a section in the RCS project


<a id="sections"></a>
# **sections**
> List[RcsSection] sections(project_id)

Get sections

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 

### Return type

[**List[RcsSection]**](RcsSection.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_section import RcsSection
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
    api_instance = ideastatica_rcs_api.SectionApi(api_client)
    project_id = 'project_id_example' # str | 

    try:
        # Get sections
        api_response = api_instance.sections(project_id)
        print("The response of SectionApi->sections:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling SectionApi->sections: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/sections 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_section"></a>
# **update_section**
> RcsSection update_section(project_id, rcs_section=rcs_section)

Update a section in the RCS project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Id of the project in cache | 
 **rcs_section** | [**RcsSection**](RcsSection.md)| A new reinforced section data.The value !:RcsSectionModel.Id defines the section in project to update | [optional] 

### Return type

[**RcsSection**](RcsSection.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_section import RcsSection
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
    api_instance = ideastatica_rcs_api.SectionApi(api_client)
    project_id = 'project_id_example' # str | Id of the project in cache
    rcs_section = ideastatica_rcs_api.RcsSection() # RcsSection | A new reinforced section data.The value !:RcsSectionModel.Id defines the section in project to update (optional)

    try:
        # Update a section in the RCS project
        api_response = api_instance.update_section(project_id, rcs_section=rcs_section)
        print("The response of SectionApi->update_section:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling SectionApi->update_section: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/1/projects/{projectId}/sections 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/xml, text/xml, application/*+xml, application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

