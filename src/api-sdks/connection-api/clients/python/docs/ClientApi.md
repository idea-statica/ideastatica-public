# ideastatica_connection_api.ClientApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**connect_client**](ClientApi.md#connect_client) | **GET** /api/1/clients/connect-client | Connect a client to the ConnectionRestApi service. Method returns a unique identifier of the client.
[**get_version**](ClientApi.md#get_version) | **GET** /api/1/clients/idea-service-version | Get the IdeaStatica version


# **connect_client**
> str connect_client()

Connect a client to the ConnectionRestApi service. Method returns a unique identifier of the client.

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
    api_instance = ideastatica_connection_api.ClientApi(api_client)

    try:
        # Connect a client to the ConnectionRestApi service. Method returns a unique identifier of the client.
        api_response = api_instance.connect_client()
        print("The response of ClientApi->connect_client:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ClientApi->connect_client: %s\n" % e)
```



### Parameters

This endpoint does not need any parameter.

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

# **get_version**
> str get_version()

Get the IdeaStatica version

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
    api_instance = ideastatica_connection_api.ClientApi(api_client)

    try:
        # Get the IdeaStatica version
        api_response = api_instance.get_version()
        print("The response of ClientApi->get_version:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ClientApi->get_version: %s\n" % e)
```



### Parameters

This endpoint does not need any parameter.

### Return type

**str**

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

