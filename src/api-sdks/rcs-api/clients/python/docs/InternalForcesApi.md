# InternalForcesApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**get_section_loading**](InternalForcesApi.md#get_section_loading) | Get section loading
[**set_section_loading**](InternalForcesApi.md#set_section_loading) | Set section loading


<a id="get_section_loading"></a>
# **get_section_loading**
> str get_section_loading(project_id, section_id)

Get section loading

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **section_id** | **int**|  | 

### Return type

**str**

### Example


```python
import ideastatica_rcs_api
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
    api_instance = ideastatica_rcs_api.InternalForcesApi(api_client)
    project_id = 'project_id_example' # str | 
    section_id = 56 # int | 

    try:
        # Get section loading
        api_response = api_instance.get_section_loading(project_id, section_id)
        print("The response of InternalForcesApi->get_section_loading:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling InternalForcesApi->get_section_loading: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/sections/{sectionId}/internal-forces 

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

<a id="set_section_loading"></a>
# **set_section_loading**
> str set_section_loading(project_id, section_id, rcs_section_loading=rcs_section_loading)

Set section loading

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **section_id** | **int**|  | 
 **rcs_section_loading** | [**RcsSectionLoading**](RcsSectionLoading.md)|  | [optional] 

### Return type

**str**

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_section_loading import RcsSectionLoading
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
    api_instance = ideastatica_rcs_api.InternalForcesApi(api_client)
    project_id = 'project_id_example' # str | 
    section_id = 56 # int | 
    rcs_section_loading = ideastatica_rcs_api.RcsSectionLoading() # RcsSectionLoading |  (optional)

    try:
        # Set section loading
        api_response = api_instance.set_section_loading(project_id, section_id, rcs_section_loading=rcs_section_loading)
        print("The response of InternalForcesApi->set_section_loading:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling InternalForcesApi->set_section_loading: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/sections/{sectionId}/internal-forces 

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

