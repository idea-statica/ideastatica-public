# SettingsApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**get_settings**](SettingsApi.md#get_settings) | Gets setting values for the project.
[**update_settings**](SettingsApi.md#update_settings) | Updates one or multiple setting values in the project.


<a id="get_settings"></a>
# **get_settings**
> Dict[str, object] get_settings(project_id, search=search)

Gets setting values for the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the project. | 
 **search** | **str**| Optional parameter to search for keywords in settings. | [optional] 

### Return type

**Dict[str, object]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_settingsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the project.
    search = 'search_example' # str | Optional parameter to search for keywords in settings. (optional)

    try:
        # Gets setting values for the project.
        api_response = api_client.settings.get_settings(project_id, search=search)
        print("The response of SettingsApi->get_settings:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling SettingsApi->get_settings: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/3/projects/{projectId}/settings 

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

<a id="update_settings"></a>
# **update_settings**
> Dict[str, object] update_settings(project_id, request_body=request_body)

Updates one or multiple setting values in the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the project. | 
 **request_body** | [**Dict[str, object]**](object.md)| Dictionary of key-value pairs representing settings to update. | [optional] 

### Return type

**Dict[str, object]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_settingsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the project.
    request_body = None # Dict[str, object] | Dictionary of key-value pairs representing settings to update. (optional)

    try:
        # Updates one or multiple setting values in the project.
        api_response = api_client.settings.update_settings(project_id, request_body=request_body)
        print("The response of SettingsApi->update_settings:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling SettingsApi->update_settings: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/3/projects/{projectId}/settings 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

