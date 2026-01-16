# ClientApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**get_version**](ClientApi.md#get_version) | Get the IdeaStatica version


<a id="get_version"></a>
# **get_version**
> str get_version()

Get the IdeaStatica version

### Parameters

This endpoint does not need any parameter.

### Return type

**str**

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.rest import ApiException
from pprint import pprint

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_rcs_api.Configuration(
    host = "http://localhost"
)


# Enter a context with an instance of the API client
with ideastatica_rcs_api.ApiClient(configuration) as api_client:
    
    # Create an instance of the API class
    api_instance = ideastatica_rcs_api.ClientApi(api_client)

    try:
        # Get the IdeaStatica version
        api_response = api_instance.get_version()
        print("The response of ClientApi->get_version:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ClientApi->get_version: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/clients/idea-service-version 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/xml, text/xml, application/json, text/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

