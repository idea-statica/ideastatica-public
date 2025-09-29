# ClientApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**connect_client**](ClientApi.md#connect_client) | Connect a client to the ConnectionRestApi service. Method returns a unique identifier of the client.
[**get_version**](ClientApi.md#get_version) | Get the IdeaStatica version


<a id="connect_client"></a>
# **connect_client**
> str connect_client()

Connect a client to the ConnectionRestApi service. Method returns a unique identifier of the client.

### Parameters

This endpoint does not need any parameter.

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
def connect_clientExampleFunc(api_client):
    

    try:
        # Connect a client to the ConnectionRestApi service. Method returns a unique identifier of the client.
        api_response = api_client.client.connect_client()
        print("The response of ClientApi->connect_client:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ClientApi->connect_client: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/clients/connect-client 

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

<a id="get_version"></a>
# **get_version**
> str get_version()

Get the IdeaStatica version

### Parameters

This endpoint does not need any parameter.

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
def get_versionExampleFunc(api_client):
    

    try:
        # Get the IdeaStatica version
        api_response = api_client.client.get_version()
        print("The response of ClientApi->get_version:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ClientApi->get_version: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/clients/idea-service-version 

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

