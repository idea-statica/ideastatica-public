# OperationApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**delete_operations**](OperationApi.md#delete_operations) | Delete all operations for the connection
[**get_common_operation_properties**](OperationApi.md#get_common_operation_properties) | Get common operation properties
[**get_operations**](OperationApi.md#get_operations) | Get the list of operations for the connection
[**pre_design_welds**](OperationApi.md#pre_design_welds) | Predesign welds
[**update_common_operation_properties**](OperationApi.md#update_common_operation_properties) | Update common properties for all operations


<a id="delete_operations"></a>
# **delete_operations**
> delete_operations(project_id, connection_id)

Delete all operations for the connection

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to be modified | 

### Return type

void (empty response body)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def delete_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to be modified

    try:
        # Delete all operations for the connection
        api_client.operation.delete_operations(project_id, connection_id)
    except Exception as e:
        print("Exception when calling OperationApi->delete_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/2/projects/{projectId}/connections/{connectionId}/operations 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_common_operation_properties"></a>
# **get_common_operation_properties**
> ConOperationCommonProperties get_common_operation_properties(project_id, connection_id)

Get common operation properties

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 

### Return type

[**ConOperationCommonProperties**](ConOperationCommonProperties.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_operation_common_properties import ConOperationCommonProperties
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_common_operation_propertiesExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Get common operation properties
        api_response = api_client.operation.get_common_operation_properties(project_id, connection_id)
        print("The response of OperationApi->get_common_operation_properties:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_common_operation_properties: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/operations/common-properties 

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

<a id="get_operations"></a>
# **get_operations**
> List[ConOperation] get_operations(project_id, connection_id)

Get the list of operations for the connection

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the requested connection | 

### Return type

[**List[ConOperation]**](ConOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_operation import ConOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the requested connection

    try:
        # Get the list of operations for the connection
        api_response = api_client.operation.get_operations(project_id, connection_id)
        print("The response of OperationApi->get_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/operations 

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

<a id="pre_design_welds"></a>
# **pre_design_welds**
> str pre_design_welds(project_id, connection_id, design_type=design_type)

Predesign welds

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **design_type** | [**ConWeldSizingMethodEnum**](.md)|  | [optional] 

### Return type

**str**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_weld_sizing_method_enum import ConWeldSizingMethodEnum
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def pre_design_weldsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    design_type = ideastatica_connection_api.ConWeldSizingMethodEnum() # ConWeldSizingMethodEnum |  (optional)

    try:
        # Predesign welds
        api_response = api_client.operation.pre_design_welds(project_id, connection_id, design_type=design_type)
        print("The response of OperationApi->pre_design_welds:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->pre_design_welds: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/operations/weld-sizing 

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

<a id="update_common_operation_properties"></a>
# **update_common_operation_properties**
> update_common_operation_properties(project_id, connection_id, con_operation_common_properties=con_operation_common_properties)

Update common properties for all operations

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **con_operation_common_properties** | [**ConOperationCommonProperties**](ConOperationCommonProperties.md)| Specify id of material, or keep as null | [optional] 

### Return type

void (empty response body)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_operation_common_properties import ConOperationCommonProperties
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_common_operation_propertiesExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    con_operation_common_properties = ideastatica_connection_api.ConOperationCommonProperties() # ConOperationCommonProperties | Specify id of material, or keep as null (optional)

    try:
        # Update common properties for all operations
        api_client.operation.update_common_operation_properties(project_id, connection_id, con_operation_common_properties=con_operation_common_properties)
    except Exception as e:
        print("Exception when calling OperationApi->update_common_operation_properties: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/2/projects/{projectId}/connections/{connectionId}/operations/common-properties 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

