# ideastatica_connection_api.MemberApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**get_member**](MemberApi.md#get_member) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/members/{memberId} | Get information about the requires member in the connection
[**get_members**](MemberApi.md#get_members) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/members | Get information about all members in the connection
[**set_bearing_member**](MemberApi.md#set_bearing_member) | **PUT** /api/1/projects/{projectId}/connections/{connectionId}/members/{memberId}/set-bearing-member | Set bearing member for memberIt
[**update_member**](MemberApi.md#update_member) | **PUT** /api/1/projects/{projectId}/connections/{connectionId}/members | Update the member in the connection by newMemberData


# **get_member**
> ConMember get_member(project_id, connection_id, member_id)

Get information about the requires member in the connection

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_member import ConMember
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
    api_instance = ideastatica_connection_api.MemberApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to get its member
    member_id = 56 # int | Id of the requested member in the connection

    try:
        # Get information about the requires member in the connection
        api_response = api_instance.get_member(project_id, connection_id, member_id)
        print("The response of MemberApi->get_member:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MemberApi->get_member: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to get its member | 
 **member_id** | **int**| Id of the requested member in the connection | 

### Return type

[**ConMember**](ConMember.md)

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

# **get_members**
> List[ConMember] get_members(project_id, connection_id)

Get information about all members in the connection

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_member import ConMember
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
    api_instance = ideastatica_connection_api.MemberApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to get its members

    try:
        # Get information about all members in the connection
        api_response = api_instance.get_members(project_id, connection_id)
        print("The response of MemberApi->get_members:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MemberApi->get_members: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to get its members | 

### Return type

[**List[ConMember]**](ConMember.md)

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

# **set_bearing_member**
> ConMember set_bearing_member(project_id, connection_id, member_id)

Set bearing member for memberIt

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_member import ConMember
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
    api_instance = ideastatica_connection_api.MemberApi(api_client)
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    member_id = 56 # int | 

    try:
        # Set bearing member for memberIt
        api_response = api_instance.set_bearing_member(project_id, connection_id, member_id)
        print("The response of MemberApi->set_bearing_member:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MemberApi->set_bearing_member: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **member_id** | **int**|  | 

### Return type

[**ConMember**](ConMember.md)

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

# **update_member**
> ConMember update_member(project_id, connection_id, con_member=con_member)

Update the member in the connection by newMemberData

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_member import ConMember
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
    api_instance = ideastatica_connection_api.MemberApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to to update is member newMemberData
    con_member = ideastatica_connection_api.ConMember() # ConMember | New member data (optional)

    try:
        # Update the member in the connection by newMemberData
        api_response = api_instance.update_member(project_id, connection_id, con_member=con_member)
        print("The response of MemberApi->update_member:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MemberApi->update_member: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to to update is member newMemberData | 
 **con_member** | [**ConMember**](ConMember.md)| New member data | [optional] 

### Return type

[**ConMember**](ConMember.md)

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

