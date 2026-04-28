# ConnectionApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**copy_connection**](ConnectionApi.md#copy_connection) | Creates a copy of an existing connection in the project.
[**create_empty_connection**](ConnectionApi.md#create_empty_connection) | Adds a new empty connection to the project.
[**delete_connection**](ConnectionApi.md#delete_connection) | Deletes a specific connection from the project.
[**get_connection**](ConnectionApi.md#get_connection) | Gets data about a specific connection in the project.
[**get_connection_topology**](ConnectionApi.md#get_connection_topology) | Gets the topology of the connection in JSON format.
[**get_connections**](ConnectionApi.md#get_connections) | Gets data about all connections in the project.
[**get_production_cost**](ConnectionApi.md#get_production_cost) | Gets the production cost of the connection.
[**update_connection**](ConnectionApi.md#update_connection) | Updates data of a specific connection in the project.


<a id="copy_connection"></a>
# **copy_connection**
> ConConnection copy_connection(project_id, connection_id, name=name)

Creates a copy of an existing connection in the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the source connection to copy. | 
 **name** | **str**| Optional name for the new connection. If null or empty, a unique name is derived from the source connection name. | [optional] 

### Return type

[**ConConnection**](ConConnection.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection import ConConnection
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def copy_connectionExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the source connection to copy.
    name = 'name_example' # str | Optional name for the new connection. If null or empty, a unique name is derived from the source connection name. (optional)

    try:
        # Creates a copy of an existing connection in the project.
        api_response = api_client.connection.copy_connection(project_id, connection_id, name=name)
        print("The response of ConnectionApi->copy_connection:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionApi->copy_connection: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/3/projects/{projectId}/connections/{connectionId}/copy 

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

<a id="create_empty_connection"></a>
# **create_empty_connection**
> ConConnection create_empty_connection(project_id, name=name)

Adds a new empty connection to the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **name** | **str**| Optional connection name. If null or empty, a default name &#x60;CON{newId}&#x60; is assigned. | [optional] 

### Return type

[**ConConnection**](ConConnection.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection import ConConnection
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def create_empty_connectionExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    name = 'name_example' # str | Optional connection name. If null or empty, a default name `CON{newId}` is assigned. (optional)

    try:
        # Adds a new empty connection to the project.
        api_response = api_client.connection.create_empty_connection(project_id, name=name)
        print("The response of ConnectionApi->create_empty_connection:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionApi->create_empty_connection: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/3/projects/{projectId}/connections 

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

<a id="delete_connection"></a>
# **delete_connection**
> List[ConConnection] delete_connection(project_id, connection_id)

Deletes a specific connection from the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection to delete. | 

### Return type

[**List[ConConnection]**](ConConnection.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection import ConConnection
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def delete_connectionExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection to delete.

    try:
        # Deletes a specific connection from the project.
        api_response = api_client.connection.delete_connection(project_id, connection_id)
        print("The response of ConnectionApi->delete_connection:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionApi->delete_connection: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/3/projects/{projectId}/connections/{connectionId} 

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

<a id="get_connection"></a>
# **get_connection**
> ConConnection get_connection(project_id, connection_id)

Gets data about a specific connection in the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the requested connection. | 

### Return type

[**ConConnection**](ConConnection.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection import ConConnection
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_connectionExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the requested connection.

    try:
        # Gets data about a specific connection in the project.
        api_response = api_client.connection.get_connection(project_id, connection_id)
        print("The response of ConnectionApi->get_connection:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionApi->get_connection: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/3/projects/{projectId}/connections/{connectionId} 

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

<a id="get_connection_topology"></a>
# **get_connection_topology**
> str get_connection_topology(project_id, connection_id)

Gets the topology of the connection in JSON format.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection for which to retrieve the topology. | 

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
def get_connection_topologyExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection for which to retrieve the topology.

    try:
        # Gets the topology of the connection in JSON format.
        api_response = api_client.connection.get_connection_topology(project_id, connection_id)
        print("The response of ConnectionApi->get_connection_topology:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionApi->get_connection_topology: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/3/projects/{projectId}/connections/{connectionId}/get-topology 

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

<a id="get_connections"></a>
# **get_connections**
> List[ConConnection] get_connections(project_id)

Gets data about all connections in the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 

### Return type

[**List[ConConnection]**](ConConnection.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection import ConConnection
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_connectionsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.

    try:
        # Gets data about all connections in the project.
        api_response = api_client.connection.get_connections(project_id)
        print("The response of ConnectionApi->get_connections:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionApi->get_connections: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/3/projects/{projectId}/connections 

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

<a id="get_production_cost"></a>
# **get_production_cost**
> ConProductionCost get_production_cost(project_id, connection_id)

Gets the production cost of the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the requested connection. | 

### Return type

[**ConProductionCost**](ConProductionCost.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_production_cost import ConProductionCost
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_production_costExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the requested connection.

    try:
        # Gets the production cost of the connection.
        api_response = api_client.connection.get_production_cost(project_id, connection_id)
        print("The response of ConnectionApi->get_production_cost:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionApi->get_production_cost: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/3/projects/{projectId}/connections/{connectionId}/production-cost 

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

<a id="update_connection"></a>
# **update_connection**
> ConConnection update_connection(project_id, connection_id, con_connection=con_connection)

Updates data of a specific connection in the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection to be updated. | 
 **con_connection** | [**ConConnection**](ConConnection.md)| New connection data to be applied. | [optional] 

### Return type

[**ConConnection**](ConConnection.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection import ConConnection
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_connectionExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection to be updated.
    con_connection = ideastatica_connection_api.ConConnection() # ConConnection | New connection data to be applied. (optional)

    try:
        # Updates data of a specific connection in the project.
        api_response = api_client.connection.update_connection(project_id, connection_id, con_connection=con_connection)
        print("The response of ConnectionApi->update_connection:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionApi->update_connection: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/3/projects/{projectId}/connections/{connectionId} 

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

