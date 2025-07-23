# MemberApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**get_member**](MemberApi.md#get_member) | Get information about the requires member in the connection
[**get_members**](MemberApi.md#get_members) | Get information about all members in the connection
[**set_bearing_member**](MemberApi.md#set_bearing_member) | Set bearing member for memberIt
[**update_member**](MemberApi.md#update_member) | Update the member in the connection by newMemberData


<a id="get_member"></a>
# **get_member**
> ConMember get_member(project_id, connection_id, member_id)

Get information about the requires member in the connection

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to get its member | 
 **member_id** | **int**| Id of the requested member in the connection | 

### Return type

[**ConMember**](ConMember.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_member import ConMember
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_memberExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to get its member
    member_id = 56 # int | Id of the requested member in the connection

    try:
        # Get information about the requires member in the connection
        api_response = api_client.member.get_member(project_id, connection_id, member_id)
        print("The response of MemberApi->get_member:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MemberApi->get_member: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/members/{memberId} 

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

<a id="get_members"></a>
# **get_members**
> List[ConMember] get_members(project_id, connection_id)

Get information about all members in the connection

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to get its members | 

### Return type

[**List[ConMember]**](ConMember.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_member import ConMember
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_membersExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to get its members

    try:
        # Get information about all members in the connection
        api_response = api_client.member.get_members(project_id, connection_id)
        print("The response of MemberApi->get_members:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MemberApi->get_members: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/members 

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

<a id="set_bearing_member"></a>
# **set_bearing_member**
> ConMember set_bearing_member(project_id, connection_id, member_id)

Set bearing member for memberIt

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **member_id** | **int**|  | 

### Return type

[**ConMember**](ConMember.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_member import ConMember
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def set_bearing_memberExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    member_id = 56 # int | 

    try:
        # Set bearing member for memberIt
        api_response = api_client.member.set_bearing_member(project_id, connection_id, member_id)
        print("The response of MemberApi->set_bearing_member:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MemberApi->set_bearing_member: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/2/projects/{projectId}/connections/{connectionId}/members/{memberId}/set-bearing-member 

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

<a id="update_member"></a>
# **update_member**
> ConMember update_member(project_id, connection_id, con_member=con_member)

Update the member in the connection by newMemberData

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to to update is member newMemberData | 
 **con_member** | [**ConMember**](ConMember.md)| New member data | [optional] 

### Return type

[**ConMember**](ConMember.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_member import ConMember
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_memberExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to to update is member newMemberData
    con_member = ideastatica_connection_api.ConMember() # ConMember | New member data (optional)

    try:
        # Update the member in the connection by newMemberData
        api_response = api_client.member.update_member(project_id, connection_id, con_member=con_member)
        print("The response of MemberApi->update_member:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MemberApi->update_member: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/2/projects/{projectId}/connections/{connectionId}/members 

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

