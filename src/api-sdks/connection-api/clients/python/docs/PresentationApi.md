# PresentationApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**get_data_scene3_d**](PresentationApi.md#get_data_scene3_d) | Returns data for Scene3D visualization.
[**get_data_scene3_d_text**](PresentationApi.md#get_data_scene3_d_text) | Returns serialized data for Scene3D in JSON format.


<a id="get_data_scene3_d"></a>
# **get_data_scene3_d**
> DrawData get_data_scene3_d(project_id, connection_id)

Returns data for Scene3D visualization.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection to be presented in Scene3D. | 

### Return type

[**DrawData**](DrawData.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.draw_data import DrawData
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_data_scene3_dExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection to be presented in Scene3D.

    try:
        # Returns data for Scene3D visualization.
        api_response = api_client.presentation.get_data_scene3_d(project_id, connection_id)
        print("The response of PresentationApi->get_data_scene3_d:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling PresentationApi->get_data_scene3_d: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/3/projects/{projectId}/connections/{connectionId}/presentations 

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

<a id="get_data_scene3_d_text"></a>
# **get_data_scene3_d_text**
> str get_data_scene3_d_text(project_id, connection_id)

Returns serialized data for Scene3D in JSON format.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project. | 
 **connection_id** | **int**| The ID of the connection to be presented. | 

### Return type

**str**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_data_scene3_d_textExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project.
    connection_id = 56 # int | The ID of the connection to be presented.

    try:
        # Returns serialized data for Scene3D in JSON format.
        api_response = api_client.presentation.get_data_scene3_d_text(project_id, connection_id)
        print("The response of PresentationApi->get_data_scene3_d_text:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling PresentationApi->get_data_scene3_d_text: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/3/projects/{projectId}/connections/{connectionId}/presentations/text 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

