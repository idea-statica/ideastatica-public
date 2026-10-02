# OperationApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**add_anchor_grid_operation**](OperationApi.md#add_anchor_grid_operation) | Adds a new anchor grid operation to the connection.
[**add_bolt_grid_operation**](OperationApi.md#add_bolt_grid_operation) | Adds a new bolt grid operation to the connection.
[**add_contact_grid_operation**](OperationApi.md#add_contact_grid_operation) | Adds a new contact grid operation to the connection.
[**add_contact_operation**](OperationApi.md#add_contact_operation) | Adds a new contact operation to the connection.
[**add_cut_operation**](OperationApi.md#add_cut_operation) | Adds a new cut operation to the connection.
[**add_negative_member_operation**](OperationApi.md#add_negative_member_operation) | Adds a new negative member operation to the connection.
[**add_negative_plate_operation**](OperationApi.md#add_negative_plate_operation) | Adds a new negative plate operation to the connection.
[**add_pin_grid_operation**](OperationApi.md#add_pin_grid_operation) | Adds a new pin grid operation to the connection.
[**add_plate_cut_operation**](OperationApi.md#add_plate_cut_operation) | Adds a new plate cut operation to the connection.
[**add_stiffening_member**](OperationApi.md#add_stiffening_member) | Adds a new stiffening member operation to the connection.
[**add_stiffening_plate**](OperationApi.md#add_stiffening_plate) | Adds a new stiffening plate operation to the connection.
[**add_weld_operation**](OperationApi.md#add_weld_operation) | Adds a new weld operation to the connection.
[**add_work_plane_operation**](OperationApi.md#add_work_plane_operation) | Adds a new work plane operation to the connection.
[**delete_operations**](OperationApi.md#delete_operations) | Delete all operations for the connection.
[**get_anchor_grid_operation**](OperationApi.md#get_anchor_grid_operation) | Returns the Anchor Grid operation with the given id, with all fields populated.
[**get_anchor_grid_operations**](OperationApi.md#get_anchor_grid_operations) | Returns all Anchor Grid operations in the connection.
[**get_bolt_grid_operation**](OperationApi.md#get_bolt_grid_operation) | Returns the Bolt Grid operation with the given id, with all fields populated.
[**get_bolt_grid_operations**](OperationApi.md#get_bolt_grid_operations) | Returns all Bolt Grid operations in the connection.
[**get_common_operation_properties**](OperationApi.md#get_common_operation_properties) | Gets common operation properties.
[**get_contact_grid_operation**](OperationApi.md#get_contact_grid_operation) | Returns the Contact Grid operation with the given id, with all fields populated.
[**get_contact_grid_operations**](OperationApi.md#get_contact_grid_operations) | Returns all Contact Grid operations in the connection.
[**get_contact_operation**](OperationApi.md#get_contact_operation) | Returns the Contact operation with the given id, with all fields populated.
[**get_contact_operations**](OperationApi.md#get_contact_operations) | Returns all Contact operations in the connection.
[**get_cut_operation**](OperationApi.md#get_cut_operation) | Returns the Cut operation with the given id, with all fields populated.
[**get_cut_operations**](OperationApi.md#get_cut_operations) | Returns all Cut operations in the connection.
[**get_negative_member_operation**](OperationApi.md#get_negative_member_operation) | Returns the Negative Member operation with the given id, with all fields populated.
[**get_negative_member_operations**](OperationApi.md#get_negative_member_operations) | Returns all Negative Member operations in the connection.
[**get_negative_plate_operation**](OperationApi.md#get_negative_plate_operation) | Returns the Negative Plate operation with the given id, with all fields populated.
[**get_negative_plate_operations**](OperationApi.md#get_negative_plate_operations) | Returns all Negative Plate operations in the connection.
[**get_operations**](OperationApi.md#get_operations) | Gets the list of operations for the connection.
[**get_pin_grid_operation**](OperationApi.md#get_pin_grid_operation) | Returns the Pin Grid operation with the given id, with all fields populated.
[**get_pin_grid_operations**](OperationApi.md#get_pin_grid_operations) | Returns all Pin Grid operations in the connection.
[**get_plate_cut_operation**](OperationApi.md#get_plate_cut_operation) | Returns the Plate Cut operation with the given id, with all fields populated.
[**get_plate_cut_operations**](OperationApi.md#get_plate_cut_operations) | Returns all Plate Cut operations in the connection.
[**get_stiffening_member_operation**](OperationApi.md#get_stiffening_member_operation) | Returns the Stiffening Member operation with the given id.
[**get_stiffening_member_operations**](OperationApi.md#get_stiffening_member_operations) | Returns all Stiffening Member operations in the connection.
[**get_stiffening_plate_operation**](OperationApi.md#get_stiffening_plate_operation) | Returns the Stiffening Plate operation with the given id, with all fields populated.
[**get_stiffening_plate_operations**](OperationApi.md#get_stiffening_plate_operations) | Returns all Stiffening Plate operations in the connection.
[**get_weld_operation**](OperationApi.md#get_weld_operation) | Returns the Weld operation with the given id, with all fields populated.
[**get_weld_operations**](OperationApi.md#get_weld_operations) | Returns all Weld operations in the connection.
[**get_work_plane_operation**](OperationApi.md#get_work_plane_operation) | Returns the Work Plane operation with the given id, with all fields populated.
[**get_work_plane_operations**](OperationApi.md#get_work_plane_operations) | Returns all Work Plane operations in the connection.
[**pre_design_welds**](OperationApi.md#pre_design_welds) | Pre-designs welds in the connection.
[**update_anchor_grid_operation**](OperationApi.md#update_anchor_grid_operation) | Replaces an Anchor Grid operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_bolt_grid_operation**](OperationApi.md#update_bolt_grid_operation) | Replaces a Bolt Grid operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_common_operation_properties**](OperationApi.md#update_common_operation_properties) | Updates common properties for all operations.
[**update_contact_grid_operation**](OperationApi.md#update_contact_grid_operation) | Replaces a Contact Grid operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_contact_operation**](OperationApi.md#update_contact_operation) | Replaces a Contact operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_cut_operation**](OperationApi.md#update_cut_operation) | Replaces a Cut operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_negative_member_operation**](OperationApi.md#update_negative_member_operation) | Replaces a Negative Member operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_negative_plate_operation**](OperationApi.md#update_negative_plate_operation) | Replaces a Negative Plate operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_pin_grid_operation**](OperationApi.md#update_pin_grid_operation) | Replaces a Pin Grid operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_plate_cut_operation**](OperationApi.md#update_plate_cut_operation) | Replaces a Plate Cut operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_stiffening_member_operation**](OperationApi.md#update_stiffening_member_operation) | Replaces a Stiffening Member operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_stiffening_plate_operation**](OperationApi.md#update_stiffening_plate_operation) | Replaces a Stiffening Plate operation (PUT semantics). Target id is taken from  &#x60;request.Id&#x60;; returns 404 when no operation with that id exists in the connection.
[**update_weld_operation**](OperationApi.md#update_weld_operation) | Replaces a Weld operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.
[**update_work_plane_operation**](OperationApi.md#update_work_plane_operation) | Replaces a Work Plane operation (PUT semantics). Target id is taken from &#x60;request.Id&#x60;;  returns 404 when no operation with that id exists in the connection.


<a id="add_anchor_grid_operation"></a>
# **add_anchor_grid_operation**
> ConAddOperationResult add_anchor_grid_operation(project_id, connection_id, con_anchor_grid_operation=con_anchor_grid_operation)

Adds a new anchor grid operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_anchor_grid_operation** | [**ConAnchorGridOperation**](ConAnchorGridOperation.md)| Anchor grid operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_anchor_grid_operation import ConAnchorGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_anchor_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_anchor_grid_operation = ideastatica_connection_api.ConAnchorGridOperation() # ConAnchorGridOperation | Anchor grid operation definition. (optional)

    try:
        # Adds a new anchor grid operation to the connection.
        api_response = api_client.operation.add_anchor_grid_operation(project_id, connection_id, con_anchor_grid_operation=con_anchor_grid_operation)
        print("The response of OperationApi->add_anchor_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_anchor_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/anchor-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_bolt_grid_operation"></a>
# **add_bolt_grid_operation**
> ConAddOperationResult add_bolt_grid_operation(project_id, connection_id, con_bolt_grid_operation=con_bolt_grid_operation)

Adds a new bolt grid operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_bolt_grid_operation** | [**ConBoltGridOperation**](ConBoltGridOperation.md)| Bolt grid operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_bolt_grid_operation import ConBoltGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_bolt_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_bolt_grid_operation = ideastatica_connection_api.ConBoltGridOperation() # ConBoltGridOperation | Bolt grid operation definition. (optional)

    try:
        # Adds a new bolt grid operation to the connection.
        api_response = api_client.operation.add_bolt_grid_operation(project_id, connection_id, con_bolt_grid_operation=con_bolt_grid_operation)
        print("The response of OperationApi->add_bolt_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_bolt_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/bolt-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_contact_grid_operation"></a>
# **add_contact_grid_operation**
> ConAddOperationResult add_contact_grid_operation(project_id, connection_id, con_contact_grid_operation=con_contact_grid_operation)

Adds a new contact grid operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_contact_grid_operation** | [**ConContactGridOperation**](ConContactGridOperation.md)| Contact grid operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_contact_grid_operation import ConContactGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_contact_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_contact_grid_operation = ideastatica_connection_api.ConContactGridOperation() # ConContactGridOperation | Contact grid operation definition. (optional)

    try:
        # Adds a new contact grid operation to the connection.
        api_response = api_client.operation.add_contact_grid_operation(project_id, connection_id, con_contact_grid_operation=con_contact_grid_operation)
        print("The response of OperationApi->add_contact_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_contact_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/contact-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_contact_operation"></a>
# **add_contact_operation**
> ConAddOperationResult add_contact_operation(project_id, connection_id, con_contact_operation=con_contact_operation)

Adds a new contact operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_contact_operation** | [**ConContactOperation**](ConContactOperation.md)| Contact operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_contact_operation import ConContactOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_contact_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_contact_operation = ideastatica_connection_api.ConContactOperation() # ConContactOperation | Contact operation definition. (optional)

    try:
        # Adds a new contact operation to the connection.
        api_response = api_client.operation.add_contact_operation(project_id, connection_id, con_contact_operation=con_contact_operation)
        print("The response of OperationApi->add_contact_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_contact_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/contact 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_cut_operation"></a>
# **add_cut_operation**
> ConAddOperationResult add_cut_operation(project_id, connection_id, con_cut_operation=con_cut_operation)

Adds a new cut operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_cut_operation** | [**ConCutOperation**](ConCutOperation.md)| Cut operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_cut_operation import ConCutOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_cut_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_cut_operation = ideastatica_connection_api.ConCutOperation() # ConCutOperation | Cut operation definition. (optional)

    try:
        # Adds a new cut operation to the connection.
        api_response = api_client.operation.add_cut_operation(project_id, connection_id, con_cut_operation=con_cut_operation)
        print("The response of OperationApi->add_cut_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_cut_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/cut 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_negative_member_operation"></a>
# **add_negative_member_operation**
> ConAddOperationResult add_negative_member_operation(project_id, connection_id, con_negative_member_operation=con_negative_member_operation)

Adds a new negative member operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_negative_member_operation** | [**ConNegativeMemberOperation**](ConNegativeMemberOperation.md)| Negative member operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_negative_member_operation import ConNegativeMemberOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_negative_member_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_negative_member_operation = ideastatica_connection_api.ConNegativeMemberOperation() # ConNegativeMemberOperation | Negative member operation definition. (optional)

    try:
        # Adds a new negative member operation to the connection.
        api_response = api_client.operation.add_negative_member_operation(project_id, connection_id, con_negative_member_operation=con_negative_member_operation)
        print("The response of OperationApi->add_negative_member_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_negative_member_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/negative-member 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_negative_plate_operation"></a>
# **add_negative_plate_operation**
> ConAddOperationResult add_negative_plate_operation(project_id, connection_id, con_negative_plate_operation=con_negative_plate_operation)

Adds a new negative plate operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_negative_plate_operation** | [**ConNegativePlateOperation**](ConNegativePlateOperation.md)| Negative plate operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_negative_plate_operation import ConNegativePlateOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_negative_plate_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_negative_plate_operation = ideastatica_connection_api.ConNegativePlateOperation() # ConNegativePlateOperation | Negative plate operation definition. (optional)

    try:
        # Adds a new negative plate operation to the connection.
        api_response = api_client.operation.add_negative_plate_operation(project_id, connection_id, con_negative_plate_operation=con_negative_plate_operation)
        print("The response of OperationApi->add_negative_plate_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_negative_plate_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/negative-plate 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_pin_grid_operation"></a>
# **add_pin_grid_operation**
> ConAddOperationResult add_pin_grid_operation(project_id, connection_id, con_pin_grid_operation=con_pin_grid_operation)

Adds a new pin grid operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_pin_grid_operation** | [**ConPinGridOperation**](ConPinGridOperation.md)| Pin grid operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_pin_grid_operation import ConPinGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_pin_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_pin_grid_operation = ideastatica_connection_api.ConPinGridOperation() # ConPinGridOperation | Pin grid operation definition. (optional)

    try:
        # Adds a new pin grid operation to the connection.
        api_response = api_client.operation.add_pin_grid_operation(project_id, connection_id, con_pin_grid_operation=con_pin_grid_operation)
        print("The response of OperationApi->add_pin_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_pin_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/pin-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_plate_cut_operation"></a>
# **add_plate_cut_operation**
> ConAddOperationResult add_plate_cut_operation(project_id, connection_id, con_plate_cut_operation=con_plate_cut_operation)

Adds a new plate cut operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_plate_cut_operation** | [**ConPlateCutOperation**](ConPlateCutOperation.md)| Plate cut operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_plate_cut_operation import ConPlateCutOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_plate_cut_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_plate_cut_operation = ideastatica_connection_api.ConPlateCutOperation() # ConPlateCutOperation | Plate cut operation definition. (optional)

    try:
        # Adds a new plate cut operation to the connection.
        api_response = api_client.operation.add_plate_cut_operation(project_id, connection_id, con_plate_cut_operation=con_plate_cut_operation)
        print("The response of OperationApi->add_plate_cut_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_plate_cut_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/plate-cut 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_stiffening_member"></a>
# **add_stiffening_member**
> ConAddOperationResult add_stiffening_member(project_id, connection_id, con_stiffening_member_operation=con_stiffening_member_operation)

Adds a new stiffening member operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_stiffening_member_operation** | [**ConStiffeningMemberOperation**](ConStiffeningMemberOperation.md)| Stiffening member operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_stiffening_member_operation import ConStiffeningMemberOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_stiffening_memberExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_stiffening_member_operation = ideastatica_connection_api.ConStiffeningMemberOperation() # ConStiffeningMemberOperation | Stiffening member operation definition. (optional)

    try:
        # Adds a new stiffening member operation to the connection.
        api_response = api_client.operation.add_stiffening_member(project_id, connection_id, con_stiffening_member_operation=con_stiffening_member_operation)
        print("The response of OperationApi->add_stiffening_member:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_stiffening_member: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/stiffening-member 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_stiffening_plate"></a>
# **add_stiffening_plate**
> ConAddOperationResult add_stiffening_plate(project_id, connection_id, con_stiffening_plate_operation=con_stiffening_plate_operation)

Adds a new stiffening plate operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_stiffening_plate_operation** | [**ConStiffeningPlateOperation**](ConStiffeningPlateOperation.md)| Stiffening plate operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_stiffening_plate_operation import ConStiffeningPlateOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_stiffening_plateExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_stiffening_plate_operation = ideastatica_connection_api.ConStiffeningPlateOperation() # ConStiffeningPlateOperation | Stiffening plate operation definition. (optional)

    try:
        # Adds a new stiffening plate operation to the connection.
        api_response = api_client.operation.add_stiffening_plate(project_id, connection_id, con_stiffening_plate_operation=con_stiffening_plate_operation)
        print("The response of OperationApi->add_stiffening_plate:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_stiffening_plate: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/stiffening-plate 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_weld_operation"></a>
# **add_weld_operation**
> ConAddOperationResult add_weld_operation(project_id, connection_id, con_weld_operation=con_weld_operation)

Adds a new weld operation to the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_weld_operation** | [**ConWeldOperation**](ConWeldOperation.md)| Weld operation definition. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_weld_operation import ConWeldOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_weld_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_weld_operation = ideastatica_connection_api.ConWeldOperation() # ConWeldOperation | Weld operation definition. (optional)

    try:
        # Adds a new weld operation to the connection.
        api_response = api_client.operation.add_weld_operation(project_id, connection_id, con_weld_operation=con_weld_operation)
        print("The response of OperationApi->add_weld_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_weld_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/weld 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_work_plane_operation"></a>
# **add_work_plane_operation**
> ConAddOperationResult add_work_plane_operation(project_id, connection_id, con_work_plane_operation=con_work_plane_operation)

Adds a new work plane operation to the connection.

A work plane is virtual reference geometry used by other operations (e.g. cuts) and for fatigue analysis.  It has no structural effect by itself.  Supports all three methods (ByAngles, ByNormalVector, ByIntersection) and all  coordinate system origins (Joint, Member, Plate).

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the operation will be added to. | 
 **con_work_plane_operation** | [**ConWorkPlaneOperation**](ConWorkPlaneOperation.md)| Work plane definition (origin, normal, method). | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_work_plane_operation import ConWorkPlaneOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_work_plane_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the operation will be added to.
    con_work_plane_operation = ideastatica_connection_api.ConWorkPlaneOperation() # ConWorkPlaneOperation | Work plane definition (origin, normal, method). (optional)

    try:
        # Adds a new work plane operation to the connection.
        api_response = api_client.operation.add_work_plane_operation(project_id, connection_id, con_work_plane_operation=con_work_plane_operation)
        print("The response of OperationApi->add_work_plane_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->add_work_plane_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/work-plane 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="delete_operations"></a>
# **delete_operations**
> delete_operations(project_id, connection_id)

Delete all operations for the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project. | 
 **connection_id** | **int**| The ID of the connection. | 

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
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Delete all operations for the connection.
        api_client.operation.delete_operations(project_id, connection_id)
    except Exception as e:
        print("Exception when calling OperationApi->delete_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/5/projects/{projectId}/connections/{connectionId}/operations 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**204** | No Content |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_anchor_grid_operation"></a>
# **get_anchor_grid_operation**
> ConAnchorGridOperation get_anchor_grid_operation(project_id, connection_id, operation_id)

Returns the Anchor Grid operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConAnchorGridOperation**](ConAnchorGridOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_anchor_grid_operation import ConAnchorGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_anchor_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Anchor Grid operation with the given id, with all fields populated.
        api_response = api_client.operation.get_anchor_grid_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_anchor_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_anchor_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/anchor-grid/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_anchor_grid_operations"></a>
# **get_anchor_grid_operations**
> List[ConAnchorGridOperation] get_anchor_grid_operations(project_id, connection_id)

Returns all Anchor Grid operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConAnchorGridOperation]**](ConAnchorGridOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_anchor_grid_operation import ConAnchorGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_anchor_grid_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Anchor Grid operations in the connection.
        api_response = api_client.operation.get_anchor_grid_operations(project_id, connection_id)
        print("The response of OperationApi->get_anchor_grid_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_anchor_grid_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/anchor-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_bolt_grid_operation"></a>
# **get_bolt_grid_operation**
> ConBoltGridOperation get_bolt_grid_operation(project_id, connection_id, operation_id)

Returns the Bolt Grid operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConBoltGridOperation**](ConBoltGridOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_bolt_grid_operation import ConBoltGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_bolt_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Bolt Grid operation with the given id, with all fields populated.
        api_response = api_client.operation.get_bolt_grid_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_bolt_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_bolt_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/bolt-grid/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_bolt_grid_operations"></a>
# **get_bolt_grid_operations**
> List[ConBoltGridOperation] get_bolt_grid_operations(project_id, connection_id)

Returns all Bolt Grid operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConBoltGridOperation]**](ConBoltGridOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_bolt_grid_operation import ConBoltGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_bolt_grid_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Bolt Grid operations in the connection.
        api_response = api_client.operation.get_bolt_grid_operations(project_id, connection_id)
        print("The response of OperationApi->get_bolt_grid_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_bolt_grid_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/bolt-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_common_operation_properties"></a>
# **get_common_operation_properties**
> ConOperationCommonProperties get_common_operation_properties(project_id, connection_id)

Gets common operation properties.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project. | 
 **connection_id** | **int**| The ID of the connection. | 

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
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Gets common operation properties.
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

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/common-properties 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_contact_grid_operation"></a>
# **get_contact_grid_operation**
> ConContactGridOperation get_contact_grid_operation(project_id, connection_id, operation_id)

Returns the Contact Grid operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConContactGridOperation**](ConContactGridOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_contact_grid_operation import ConContactGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_contact_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Contact Grid operation with the given id, with all fields populated.
        api_response = api_client.operation.get_contact_grid_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_contact_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_contact_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/contact-grid/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_contact_grid_operations"></a>
# **get_contact_grid_operations**
> List[ConContactGridOperation] get_contact_grid_operations(project_id, connection_id)

Returns all Contact Grid operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConContactGridOperation]**](ConContactGridOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_contact_grid_operation import ConContactGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_contact_grid_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Contact Grid operations in the connection.
        api_response = api_client.operation.get_contact_grid_operations(project_id, connection_id)
        print("The response of OperationApi->get_contact_grid_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_contact_grid_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/contact-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_contact_operation"></a>
# **get_contact_operation**
> ConContactOperation get_contact_operation(project_id, connection_id, operation_id)

Returns the Contact operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConContactOperation**](ConContactOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_contact_operation import ConContactOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_contact_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Contact operation with the given id, with all fields populated.
        api_response = api_client.operation.get_contact_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_contact_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_contact_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/contact/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_contact_operations"></a>
# **get_contact_operations**
> List[ConContactOperation] get_contact_operations(project_id, connection_id)

Returns all Contact operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConContactOperation]**](ConContactOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_contact_operation import ConContactOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_contact_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Contact operations in the connection.
        api_response = api_client.operation.get_contact_operations(project_id, connection_id)
        print("The response of OperationApi->get_contact_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_contact_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/contact 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_cut_operation"></a>
# **get_cut_operation**
> ConCutOperation get_cut_operation(project_id, connection_id, operation_id)

Returns the Cut operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConCutOperation**](ConCutOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_cut_operation import ConCutOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_cut_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Cut operation with the given id, with all fields populated.
        api_response = api_client.operation.get_cut_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_cut_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_cut_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/cut/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_cut_operations"></a>
# **get_cut_operations**
> List[ConCutOperation] get_cut_operations(project_id, connection_id)

Returns all Cut operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConCutOperation]**](ConCutOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_cut_operation import ConCutOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_cut_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Cut operations in the connection.
        api_response = api_client.operation.get_cut_operations(project_id, connection_id)
        print("The response of OperationApi->get_cut_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_cut_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/cut 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_negative_member_operation"></a>
# **get_negative_member_operation**
> ConNegativeMemberOperation get_negative_member_operation(project_id, connection_id, operation_id)

Returns the Negative Member operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConNegativeMemberOperation**](ConNegativeMemberOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_negative_member_operation import ConNegativeMemberOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_negative_member_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Negative Member operation with the given id, with all fields populated.
        api_response = api_client.operation.get_negative_member_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_negative_member_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_negative_member_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/negative-member/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_negative_member_operations"></a>
# **get_negative_member_operations**
> List[ConNegativeMemberOperation] get_negative_member_operations(project_id, connection_id)

Returns all Negative Member operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConNegativeMemberOperation]**](ConNegativeMemberOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_negative_member_operation import ConNegativeMemberOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_negative_member_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Negative Member operations in the connection.
        api_response = api_client.operation.get_negative_member_operations(project_id, connection_id)
        print("The response of OperationApi->get_negative_member_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_negative_member_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/negative-member 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_negative_plate_operation"></a>
# **get_negative_plate_operation**
> ConNegativePlateOperation get_negative_plate_operation(project_id, connection_id, operation_id)

Returns the Negative Plate operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConNegativePlateOperation**](ConNegativePlateOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_negative_plate_operation import ConNegativePlateOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_negative_plate_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Negative Plate operation with the given id, with all fields populated.
        api_response = api_client.operation.get_negative_plate_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_negative_plate_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_negative_plate_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/negative-plate/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_negative_plate_operations"></a>
# **get_negative_plate_operations**
> List[ConNegativePlateOperation] get_negative_plate_operations(project_id, connection_id)

Returns all Negative Plate operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConNegativePlateOperation]**](ConNegativePlateOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_negative_plate_operation import ConNegativePlateOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_negative_plate_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Negative Plate operations in the connection.
        api_response = api_client.operation.get_negative_plate_operations(project_id, connection_id)
        print("The response of OperationApi->get_negative_plate_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_negative_plate_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/negative-plate 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_operations"></a>
# **get_operations**
> List[ConOperation] get_operations(project_id, connection_id)

Gets the list of operations for the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the requested connection. | 

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
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the requested connection.

    try:
        # Gets the list of operations for the connection.
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

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_pin_grid_operation"></a>
# **get_pin_grid_operation**
> ConPinGridOperation get_pin_grid_operation(project_id, connection_id, operation_id)

Returns the Pin Grid operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConPinGridOperation**](ConPinGridOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_pin_grid_operation import ConPinGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_pin_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Pin Grid operation with the given id, with all fields populated.
        api_response = api_client.operation.get_pin_grid_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_pin_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_pin_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/pin-grid/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_pin_grid_operations"></a>
# **get_pin_grid_operations**
> List[ConPinGridOperation] get_pin_grid_operations(project_id, connection_id)

Returns all Pin Grid operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConPinGridOperation]**](ConPinGridOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_pin_grid_operation import ConPinGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_pin_grid_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Pin Grid operations in the connection.
        api_response = api_client.operation.get_pin_grid_operations(project_id, connection_id)
        print("The response of OperationApi->get_pin_grid_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_pin_grid_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/pin-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_plate_cut_operation"></a>
# **get_plate_cut_operation**
> ConPlateCutOperation get_plate_cut_operation(project_id, connection_id, operation_id)

Returns the Plate Cut operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConPlateCutOperation**](ConPlateCutOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_plate_cut_operation import ConPlateCutOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_plate_cut_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Plate Cut operation with the given id, with all fields populated.
        api_response = api_client.operation.get_plate_cut_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_plate_cut_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_plate_cut_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/plate-cut/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_plate_cut_operations"></a>
# **get_plate_cut_operations**
> List[ConPlateCutOperation] get_plate_cut_operations(project_id, connection_id)

Returns all Plate Cut operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConPlateCutOperation]**](ConPlateCutOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_plate_cut_operation import ConPlateCutOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_plate_cut_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Plate Cut operations in the connection.
        api_response = api_client.operation.get_plate_cut_operations(project_id, connection_id)
        print("The response of OperationApi->get_plate_cut_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_plate_cut_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/plate-cut 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_stiffening_member_operation"></a>
# **get_stiffening_member_operation**
> ConStiffeningMemberOperation get_stiffening_member_operation(project_id, connection_id, operation_id)

Returns the Stiffening Member operation with the given id.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConStiffeningMemberOperation**](ConStiffeningMemberOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_stiffening_member_operation import ConStiffeningMemberOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_stiffening_member_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Stiffening Member operation with the given id.
        api_response = api_client.operation.get_stiffening_member_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_stiffening_member_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_stiffening_member_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/stiffening-member/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_stiffening_member_operations"></a>
# **get_stiffening_member_operations**
> List[ConStiffeningMemberOperation] get_stiffening_member_operations(project_id, connection_id)

Returns all Stiffening Member operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConStiffeningMemberOperation]**](ConStiffeningMemberOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_stiffening_member_operation import ConStiffeningMemberOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_stiffening_member_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Stiffening Member operations in the connection.
        api_response = api_client.operation.get_stiffening_member_operations(project_id, connection_id)
        print("The response of OperationApi->get_stiffening_member_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_stiffening_member_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/stiffening-member 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_stiffening_plate_operation"></a>
# **get_stiffening_plate_operation**
> ConStiffeningPlateOperation get_stiffening_plate_operation(project_id, connection_id, operation_id)

Returns the Stiffening Plate operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConStiffeningPlateOperation**](ConStiffeningPlateOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_stiffening_plate_operation import ConStiffeningPlateOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_stiffening_plate_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Stiffening Plate operation with the given id, with all fields populated.
        api_response = api_client.operation.get_stiffening_plate_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_stiffening_plate_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_stiffening_plate_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/stiffening-plate/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_stiffening_plate_operations"></a>
# **get_stiffening_plate_operations**
> List[ConStiffeningPlateOperation] get_stiffening_plate_operations(project_id, connection_id)

Returns all Stiffening Plate operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConStiffeningPlateOperation]**](ConStiffeningPlateOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_stiffening_plate_operation import ConStiffeningPlateOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_stiffening_plate_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Stiffening Plate operations in the connection.
        api_response = api_client.operation.get_stiffening_plate_operations(project_id, connection_id)
        print("The response of OperationApi->get_stiffening_plate_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_stiffening_plate_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/stiffening-plate 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_weld_operation"></a>
# **get_weld_operation**
> ConWeldOperation get_weld_operation(project_id, connection_id, operation_id)

Returns the Weld operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConWeldOperation**](ConWeldOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_weld_operation import ConWeldOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_weld_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Weld operation with the given id, with all fields populated.
        api_response = api_client.operation.get_weld_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_weld_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_weld_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/weld/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_weld_operations"></a>
# **get_weld_operations**
> List[ConWeldOperation] get_weld_operations(project_id, connection_id)

Returns all Weld operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConWeldOperation]**](ConWeldOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_weld_operation import ConWeldOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_weld_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Weld operations in the connection.
        api_response = api_client.operation.get_weld_operations(project_id, connection_id)
        print("The response of OperationApi->get_weld_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_weld_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/weld 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_work_plane_operation"></a>
# **get_work_plane_operation**
> ConWorkPlaneOperation get_work_plane_operation(project_id, connection_id, operation_id)

Returns the Work Plane operation with the given id, with all fields populated.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection containing the operation. | 
 **operation_id** | **int**| The ID of the operation in the connection. | 

### Return type

[**ConWorkPlaneOperation**](ConWorkPlaneOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_work_plane_operation import ConWorkPlaneOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_work_plane_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection containing the operation.
    operation_id = 56 # int | The ID of the operation in the connection.

    try:
        # Returns the Work Plane operation with the given id, with all fields populated.
        api_response = api_client.operation.get_work_plane_operation(project_id, connection_id, operation_id)
        print("The response of OperationApi->get_work_plane_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_work_plane_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/work-plane/{operationId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_work_plane_operations"></a>
# **get_work_plane_operations**
> List[ConWorkPlaneOperation] get_work_plane_operations(project_id, connection_id)

Returns all Work Plane operations in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 

### Return type

[**List[ConWorkPlaneOperation]**](ConWorkPlaneOperation.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_work_plane_operation import ConWorkPlaneOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_work_plane_operationsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.

    try:
        # Returns all Work Plane operations in the connection.
        api_response = api_client.operation.get_work_plane_operations(project_id, connection_id)
        print("The response of OperationApi->get_work_plane_operations:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->get_work_plane_operations: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/5/projects/{projectId}/connections/{connectionId}/operations/work-plane 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="pre_design_welds"></a>
# **pre_design_welds**
> str pre_design_welds(project_id, connection_id, design_type=design_type)

Pre-designs welds in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project. | 
 **connection_id** | **int**| The ID of the connection. | 
 **design_type** | [**ConWeldSizingMethodEnum**](.md)| The weld sizing method to apply (default is FullStrength). | [optional] 

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
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project.
    connection_id = 56 # int | The ID of the connection.
    design_type = ideastatica_connection_api.ConWeldSizingMethodEnum() # ConWeldSizingMethodEnum | The weld sizing method to apply (default is FullStrength). (optional)

    try:
        # Pre-designs welds in the connection.
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

> **POST** /api/5/projects/{projectId}/connections/{connectionId}/operations/weld-sizing 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |
**501** | Not Implemented |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_anchor_grid_operation"></a>
# **update_anchor_grid_operation**
> ConAddOperationResult update_anchor_grid_operation(project_id, connection_id, con_anchor_grid_operation=con_anchor_grid_operation)

Replaces an Anchor Grid operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_anchor_grid_operation** | [**ConAnchorGridOperation**](ConAnchorGridOperation.md)| The anchor grid data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_anchor_grid_operation import ConAnchorGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_anchor_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_anchor_grid_operation = ideastatica_connection_api.ConAnchorGridOperation() # ConAnchorGridOperation | The anchor grid data to apply. (optional)

    try:
        # Replaces an Anchor Grid operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_anchor_grid_operation(project_id, connection_id, con_anchor_grid_operation=con_anchor_grid_operation)
        print("The response of OperationApi->update_anchor_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_anchor_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/anchor-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_bolt_grid_operation"></a>
# **update_bolt_grid_operation**
> ConAddOperationResult update_bolt_grid_operation(project_id, connection_id, con_bolt_grid_operation=con_bolt_grid_operation)

Replaces a Bolt Grid operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_bolt_grid_operation** | [**ConBoltGridOperation**](ConBoltGridOperation.md)| The bolt grid data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_bolt_grid_operation import ConBoltGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_bolt_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_bolt_grid_operation = ideastatica_connection_api.ConBoltGridOperation() # ConBoltGridOperation | The bolt grid data to apply. (optional)

    try:
        # Replaces a Bolt Grid operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_bolt_grid_operation(project_id, connection_id, con_bolt_grid_operation=con_bolt_grid_operation)
        print("The response of OperationApi->update_bolt_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_bolt_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/bolt-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_common_operation_properties"></a>
# **update_common_operation_properties**
> update_common_operation_properties(project_id, connection_id, con_operation_common_properties=con_operation_common_properties)

Updates common properties for all operations.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_operation_common_properties** | [**ConOperationCommonProperties**](ConOperationCommonProperties.md)| Common properties to apply (specify material IDs, or keep as null). | [optional] 

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
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project.
    connection_id = 56 # int | The ID of the connection.
    con_operation_common_properties = ideastatica_connection_api.ConOperationCommonProperties() # ConOperationCommonProperties | Common properties to apply (specify material IDs, or keep as null). (optional)

    try:
        # Updates common properties for all operations.
        api_client.operation.update_common_operation_properties(project_id, connection_id, con_operation_common_properties=con_operation_common_properties)
    except Exception as e:
        print("Exception when calling OperationApi->update_common_operation_properties: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/common-properties 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**204** | No Content |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_contact_grid_operation"></a>
# **update_contact_grid_operation**
> ConAddOperationResult update_contact_grid_operation(project_id, connection_id, con_contact_grid_operation=con_contact_grid_operation)

Replaces a Contact Grid operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_contact_grid_operation** | [**ConContactGridOperation**](ConContactGridOperation.md)| The contact grid data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_contact_grid_operation import ConContactGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_contact_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_contact_grid_operation = ideastatica_connection_api.ConContactGridOperation() # ConContactGridOperation | The contact grid data to apply. (optional)

    try:
        # Replaces a Contact Grid operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_contact_grid_operation(project_id, connection_id, con_contact_grid_operation=con_contact_grid_operation)
        print("The response of OperationApi->update_contact_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_contact_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/contact-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_contact_operation"></a>
# **update_contact_operation**
> ConAddOperationResult update_contact_operation(project_id, connection_id, con_contact_operation=con_contact_operation)

Replaces a Contact operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_contact_operation** | [**ConContactOperation**](ConContactOperation.md)| The contact data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_contact_operation import ConContactOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_contact_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_contact_operation = ideastatica_connection_api.ConContactOperation() # ConContactOperation | The contact data to apply. (optional)

    try:
        # Replaces a Contact operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_contact_operation(project_id, connection_id, con_contact_operation=con_contact_operation)
        print("The response of OperationApi->update_contact_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_contact_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/contact 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_cut_operation"></a>
# **update_cut_operation**
> ConAddOperationResult update_cut_operation(project_id, connection_id, con_cut_operation=con_cut_operation)

Replaces a Cut operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_cut_operation** | [**ConCutOperation**](ConCutOperation.md)| The cut data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_cut_operation import ConCutOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_cut_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_cut_operation = ideastatica_connection_api.ConCutOperation() # ConCutOperation | The cut data to apply. (optional)

    try:
        # Replaces a Cut operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_cut_operation(project_id, connection_id, con_cut_operation=con_cut_operation)
        print("The response of OperationApi->update_cut_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_cut_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/cut 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_negative_member_operation"></a>
# **update_negative_member_operation**
> ConAddOperationResult update_negative_member_operation(project_id, connection_id, con_negative_member_operation=con_negative_member_operation)

Replaces a Negative Member operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_negative_member_operation** | [**ConNegativeMemberOperation**](ConNegativeMemberOperation.md)| The negative member data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_negative_member_operation import ConNegativeMemberOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_negative_member_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_negative_member_operation = ideastatica_connection_api.ConNegativeMemberOperation() # ConNegativeMemberOperation | The negative member data to apply. (optional)

    try:
        # Replaces a Negative Member operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_negative_member_operation(project_id, connection_id, con_negative_member_operation=con_negative_member_operation)
        print("The response of OperationApi->update_negative_member_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_negative_member_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/negative-member 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_negative_plate_operation"></a>
# **update_negative_plate_operation**
> ConAddOperationResult update_negative_plate_operation(project_id, connection_id, con_negative_plate_operation=con_negative_plate_operation)

Replaces a Negative Plate operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_negative_plate_operation** | [**ConNegativePlateOperation**](ConNegativePlateOperation.md)| The negative plate data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_negative_plate_operation import ConNegativePlateOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_negative_plate_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_negative_plate_operation = ideastatica_connection_api.ConNegativePlateOperation() # ConNegativePlateOperation | The negative plate data to apply. (optional)

    try:
        # Replaces a Negative Plate operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_negative_plate_operation(project_id, connection_id, con_negative_plate_operation=con_negative_plate_operation)
        print("The response of OperationApi->update_negative_plate_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_negative_plate_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/negative-plate 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_pin_grid_operation"></a>
# **update_pin_grid_operation**
> ConAddOperationResult update_pin_grid_operation(project_id, connection_id, con_pin_grid_operation=con_pin_grid_operation)

Replaces a Pin Grid operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_pin_grid_operation** | [**ConPinGridOperation**](ConPinGridOperation.md)| The pin grid data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_pin_grid_operation import ConPinGridOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_pin_grid_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_pin_grid_operation = ideastatica_connection_api.ConPinGridOperation() # ConPinGridOperation | The pin grid data to apply. (optional)

    try:
        # Replaces a Pin Grid operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_pin_grid_operation(project_id, connection_id, con_pin_grid_operation=con_pin_grid_operation)
        print("The response of OperationApi->update_pin_grid_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_pin_grid_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/pin-grid 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_plate_cut_operation"></a>
# **update_plate_cut_operation**
> ConAddOperationResult update_plate_cut_operation(project_id, connection_id, con_plate_cut_operation=con_plate_cut_operation)

Replaces a Plate Cut operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_plate_cut_operation** | [**ConPlateCutOperation**](ConPlateCutOperation.md)| The plate cut data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_plate_cut_operation import ConPlateCutOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_plate_cut_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_plate_cut_operation = ideastatica_connection_api.ConPlateCutOperation() # ConPlateCutOperation | The plate cut data to apply. (optional)

    try:
        # Replaces a Plate Cut operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_plate_cut_operation(project_id, connection_id, con_plate_cut_operation=con_plate_cut_operation)
        print("The response of OperationApi->update_plate_cut_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_plate_cut_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/plate-cut 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_stiffening_member_operation"></a>
# **update_stiffening_member_operation**
> ConAddOperationResult update_stiffening_member_operation(project_id, connection_id, con_stiffening_member_operation=con_stiffening_member_operation)

Replaces a Stiffening Member operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_stiffening_member_operation** | [**ConStiffeningMemberOperation**](ConStiffeningMemberOperation.md)| The stiffening member data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_stiffening_member_operation import ConStiffeningMemberOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_stiffening_member_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_stiffening_member_operation = ideastatica_connection_api.ConStiffeningMemberOperation() # ConStiffeningMemberOperation | The stiffening member data to apply. (optional)

    try:
        # Replaces a Stiffening Member operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_stiffening_member_operation(project_id, connection_id, con_stiffening_member_operation=con_stiffening_member_operation)
        print("The response of OperationApi->update_stiffening_member_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_stiffening_member_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/stiffening-member 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_stiffening_plate_operation"></a>
# **update_stiffening_plate_operation**
> ConAddOperationResult update_stiffening_plate_operation(project_id, connection_id, con_stiffening_plate_operation=con_stiffening_plate_operation)

Replaces a Stiffening Plate operation (PUT semantics). Target id is taken from  `request.Id`; returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_stiffening_plate_operation** | [**ConStiffeningPlateOperation**](ConStiffeningPlateOperation.md)| The stiffening plate data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_stiffening_plate_operation import ConStiffeningPlateOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_stiffening_plate_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_stiffening_plate_operation = ideastatica_connection_api.ConStiffeningPlateOperation() # ConStiffeningPlateOperation | The stiffening plate data to apply. (optional)

    try:
        # Replaces a Stiffening Plate operation (PUT semantics). Target id is taken from  `request.Id`; returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_stiffening_plate_operation(project_id, connection_id, con_stiffening_plate_operation=con_stiffening_plate_operation)
        print("The response of OperationApi->update_stiffening_plate_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_stiffening_plate_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/stiffening-plate 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_weld_operation"></a>
# **update_weld_operation**
> ConAddOperationResult update_weld_operation(project_id, connection_id, con_weld_operation=con_weld_operation)

Replaces a Weld operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_weld_operation** | [**ConWeldOperation**](ConWeldOperation.md)| The weld data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_weld_operation import ConWeldOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_weld_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_weld_operation = ideastatica_connection_api.ConWeldOperation() # ConWeldOperation | The weld data to apply. (optional)

    try:
        # Replaces a Weld operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_weld_operation(project_id, connection_id, con_weld_operation=con_weld_operation)
        print("The response of OperationApi->update_weld_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_weld_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/weld 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_work_plane_operation"></a>
# **update_work_plane_operation**
> ConAddOperationResult update_work_plane_operation(project_id, connection_id, con_work_plane_operation=con_work_plane_operation)

Replaces a Work Plane operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection. | 
 **con_work_plane_operation** | [**ConWorkPlaneOperation**](ConWorkPlaneOperation.md)| The work plane data to apply. | [optional] 

### Return type

[**ConAddOperationResult**](ConAddOperationResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult
from ideastatica_connection_api.models.con_work_plane_operation import ConWorkPlaneOperation
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_work_plane_operationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection.
    con_work_plane_operation = ideastatica_connection_api.ConWorkPlaneOperation() # ConWorkPlaneOperation | The work plane data to apply. (optional)

    try:
        # Replaces a Work Plane operation (PUT semantics). Target id is taken from `request.Id`;  returns 404 when no operation with that id exists in the connection.
        api_response = api_client.operation.update_work_plane_operation(project_id, connection_id, con_work_plane_operation=con_work_plane_operation)
        print("The response of OperationApi->update_work_plane_operation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling OperationApi->update_work_plane_operation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/5/projects/{projectId}/connections/{connectionId}/operations/work-plane 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

