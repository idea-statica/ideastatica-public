# ConnectionApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**get_connection**](ConnectionApi.md#get_connection) | Get data about a specific connection in the project
[**get_connections**](ConnectionApi.md#get_connections) | Get data about all connections in the project
[**get_production_cost**](ConnectionApi.md#get_production_cost) | Get production cost of the connection
[**update_connection**](ConnectionApi.md#update_connection) | Update data of a specific connection in the project


<a id="get_connection"></a>
# **get_connection**
> ConConnection get_connection(project_id, connection_id)

Get data about a specific connection in the project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| The id of the requested connection | 

### Return type

[**ConConnection**](ConConnection.md)

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection import ConConnection
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
    api_instance = ideastatica_connection_api.ConnectionApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | The id of the requested connection

    try:
        # Get data about a specific connection in the project
        api_response = api_instance.get_connection(project_id, connection_id)
        print("The response of ConnectionApi->get_connection:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ConnectionApi->get_connection: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/connections/{connectionId} 

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

<a id="get_connections"></a>
# **get_connections**
> List[ConConnection] get_connections(project_id)

Get data about all connections in the project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

[**List[ConConnection]**](ConConnection.md)

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection import ConConnection
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
    api_instance = ideastatica_connection_api.ConnectionApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get data about all connections in the project
        api_response = api_instance.get_connections(project_id)
        print("The response of ConnectionApi->get_connections:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ConnectionApi->get_connections: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/connections 

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

<a id="get_production_cost"></a>
# **get_production_cost**
> ConProductionCost get_production_cost(project_id, connection_id)

Get production cost of the connection

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the requested connection | 

### Return type

[**ConProductionCost**](ConProductionCost.md)

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_production_cost import ConProductionCost
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
    api_instance = ideastatica_connection_api.ConnectionApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the requested connection

    try:
        # Get production cost of the connection
        api_response = api_instance.get_production_cost(project_id, connection_id)
        print("The response of ConnectionApi->get_production_cost:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ConnectionApi->get_production_cost: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/connections/{connectionId}/production-cost 

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

<a id="update_connection"></a>
# **update_connection**
> ConConnection update_connection(project_id, connection_id, con_connection=con_connection)

Update data of a specific connection in the project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to be update | 
 **con_connection** | [**ConConnection**](ConConnection.md)| New connection data to be set | [optional] 

### Return type

[**ConConnection**](ConConnection.md)

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection import ConConnection
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
    api_instance = ideastatica_connection_api.ConnectionApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to be update
    con_connection = ideastatica_connection_api.ConConnection() # ConConnection | New connection data to be set (optional)

    try:
        # Update data of a specific connection in the project
        api_response = api_instance.update_connection(project_id, connection_id, con_connection=con_connection)
        print("The response of ConnectionApi->update_connection:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ConnectionApi->update_connection: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/1/projects/{projectId}/connections/{connectionId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

