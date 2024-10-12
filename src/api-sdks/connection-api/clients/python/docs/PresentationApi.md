# ideastatica_connection_api.PresentationApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**get_data_scene3_d**](PresentationApi.md#get_data_scene3_d) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/presentations | Returns data for scene3D
[**get_data_scene3_d_text**](PresentationApi.md#get_data_scene3_d_text) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/presentations/text | Return serialized data for scene3D in json format


# **get_data_scene3_d**
> DrawData get_data_scene3_d(project_id, connection_id)

Returns data for scene3D

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.draw_data import DrawData
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_connection_api.Configuration(
    host = "http://localhost"
)


# Enter a context with an instance of the API client
with ideastatica_connection_api.ApiClient(configuration) as api_client:
    # Create an instance of the API class
    api_instance = ideastatica_connection_api.PresentationApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the open project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to be presented to scene3D

    try:
        # Returns data for scene3D
        api_response = api_instance.get_data_scene3_d(project_id, connection_id)
        print("The response of PresentationApi->get_data_scene3_d:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling PresentationApi->get_data_scene3_d: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the open project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to be presented to scene3D | 

### Return type

[**DrawData**](DrawData.md)

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

# **get_data_scene3_d_text**
> str get_data_scene3_d_text(project_id, connection_id)

Return serialized data for scene3D in json format

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_connection_api.Configuration(
    host = "http://localhost"
)


# Enter a context with an instance of the API client
with ideastatica_connection_api.ApiClient(configuration) as api_client:
    # Create an instance of the API class
    api_instance = ideastatica_connection_api.PresentationApi(api_client)
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Return serialized data for scene3D in json format
        api_response = api_instance.get_data_scene3_d_text(project_id, connection_id)
        print("The response of PresentationApi->get_data_scene3_d_text:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling PresentationApi->get_data_scene3_d_text: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 

### Return type

**str**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

