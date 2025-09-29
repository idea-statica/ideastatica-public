# ExportApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**export_ifc**](ExportApi.md#export_ifc) | Export connection to IFC format
[**export_iom**](ExportApi.md#export_iom) | Export connection to XML which includes https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs
[**export_iom_connection_data**](ExportApi.md#export_iom_connection_data) | Get https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs for required connection


<a id="export_ifc"></a>
# **export_ifc**
> str export_ifc(project_id, connection_id)

Export connection to IFC format

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 

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
def export_ifcExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Export connection to IFC format
        api_response = api_client.export.export_ifc(project_id, connection_id)
        print("The response of ExportApi->export_ifc:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ExportApi->export_ifc: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/export-ifc 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="export_iom"></a>
# **export_iom**
> str export_iom(project_id, connection_id, version=version)

Export connection to XML which includes https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **version** | **str**|  | [optional] 

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
def export_iomExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    version = 'version_example' # str |  (optional)

    try:
        # Export connection to XML which includes https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs
        api_response = api_client.export.export_iom(project_id, connection_id, version=version)
        print("The response of ExportApi->export_iom:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ExportApi->export_iom: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/export-iom 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/xml, text/plain

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="export_iom_connection_data"></a>
# **export_iom_connection_data**
> ConnectionData export_iom_connection_data(project_id, connection_id)

Get https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs for required connection

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 

### Return type

[**ConnectionData**](ConnectionData.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.connection_data import ConnectionData
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def export_iom_connection_dataExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Get https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs for required connection
        api_response = api_client.export.export_iom_connection_data(project_id, connection_id)
        print("The response of ExportApi->export_iom_connection_data:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ExportApi->export_iom_connection_data: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/export-iom-connection-data 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/xml

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

