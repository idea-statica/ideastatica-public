# DesignMemberApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**members**](DesignMemberApi.md#members) | Get members


<a id="members"></a>
# **members**
> List[RcsCheckMember] members(project_id)

Get members

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 

### Return type

[**List[RcsCheckMember]**](RcsCheckMember.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_check_member import RcsCheckMember
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
    api_instance = ideastatica_rcs_api.DesignMemberApi(api_client)
    project_id = 'project_id_example' # str | 

    try:
        # Get members
        api_response = api_instance.members(project_id)
        print("The response of DesignMemberApi->members:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling DesignMemberApi->members: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/design-members 

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

