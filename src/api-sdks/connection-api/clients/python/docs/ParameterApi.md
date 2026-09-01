# ParameterApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**create_parameter**](ParameterApi.md#create_parameter) | Creates a parameter in the connection.    The expression is evaluated before the parameter is stored, and the request is rejected if it  cannot be evaluated - an unknown identifier, a syntax error, a reference to the parameter being  created. It must therefore reference only parameters that already exist, so a set of dependent  parameters is created driver-first.  Bounds are checked, not enforced: a value outside the bounds given in the same request is  created and applied, and reported with a warning status and a message saying which bound it  exceeds - the same answer a later update of that value gives.
[**create_parameter_link**](ParameterApi.md#create_parameter_link) | Links a parameter to a model property so the parameter drives it. Name the owner exactly as the  linkable-properties catalog reports it and pass its propertyId unchanged.    A link is deliberately type-agnostic: the parameter&#39;s type is not checked against the property&#39;s  &#x60;valueType&#x60;, because an Expression parameter can evaluate to any type and the value a  parameter yields is only known once it is evaluated. A parameter whose value the property cannot  take is reported by the engine when the parameters are applied, not when the link is created -  so match the catalog&#39;s &#x60;valueType&#x60; when choosing which parameter to link.
[**delete_parameter**](ParameterApi.md#delete_parameter) | Deletes one parameter and every link through which it drove a model property.
[**delete_parameter_link**](ParameterApi.md#delete_parameter_link) | Removes a parameter-to-property link. The parameter is kept, and the property retains the value  the parameter last applied.
[**delete_parameters**](ParameterApi.md#delete_parameters) | Delete all parameters and parameter model links for the connection connectionId in the project projectId.
[**evaluate_expression**](ParameterApi.md#evaluate_expression) | Evaluate the expression and return the result.  For more details see documentation about parameters:  https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html  or  https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html
[**get_linkable_properties**](ParameterApi.md#get_linkable_properties) | Lists every model property of the connection that a parameter can be linked to, across  operations, members and the library items the connection edits (cross-sections, materials,  bolt assemblies). Each row names its owner.    The list is not filtered by what the Design tab currently shows: deciding that needs the  desktop editor context, which the service does not have. Some rows therefore belong to a  property the application hides for the operation&#39;s present configuration, and linking a  parameter to one of those drives a value the design does not use.
[**get_member_linkable_properties**](ParameterApi.md#get_member_linkable_properties) | Lists the model properties of one member that a parameter can be linked to.
[**get_operation_linkable_properties**](ParameterApi.md#get_operation_linkable_properties) | Lists the model properties of one operation that a parameter can be linked to.
[**get_parameter_links**](ParameterApi.md#get_parameter_links) | Lists the parameter-to-property links of the connection.
[**get_parameters**](ParameterApi.md#get_parameters) | Gets all parameters defined for the specified project and connection.
[**update**](ParameterApi.md#update) | Updates parameters for the specified connection in the project with the values provided.


<a id="create_parameter"></a>
# **create_parameter**
> IdeaParameter create_parameter(project_id, connection_id, con_parameter_create=con_parameter_create)

Creates a parameter in the connection.    The expression is evaluated before the parameter is stored, and the request is rejected if it  cannot be evaluated - an unknown identifier, a syntax error, a reference to the parameter being  created. It must therefore reference only parameters that already exist, so a set of dependent  parameters is created driver-first.  Bounds are checked, not enforced: a value outside the bounds given in the same request is  created and applied, and reported with a warning status and a message saying which bound it  exceeds - the same answer a later update of that value gives.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection the parameter is added to. | 
 **con_parameter_create** | [**ConParameterCreate**](ConParameterCreate.md)| Identifier, type and value or expression of the new parameter. | [optional] 

### Return type

[**IdeaParameter**](IdeaParameter.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_parameter_create import ConParameterCreate
from ideastatica_connection_api.models.idea_parameter import IdeaParameter
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def create_parameterExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection the parameter is added to.
    con_parameter_create = ideastatica_connection_api.ConParameterCreate() # ConParameterCreate | Identifier, type and value or expression of the new parameter. (optional)

    try:
        # Creates a parameter in the connection.    The expression is evaluated before the parameter is stored, and the request is rejected if it  cannot be evaluated - an unknown identifier, a syntax error, a reference to the parameter being  created. It must therefore reference only parameters that already exist, so a set of dependent  parameters is created driver-first.  Bounds are checked, not enforced: a value outside the bounds given in the same request is  created and applied, and reported with a warning status and a message saying which bound it  exceeds - the same answer a later update of that value gives.
        api_response = api_client.parameter.create_parameter(project_id, connection_id, con_parameter_create=con_parameter_create)
        print("The response of ParameterApi->create_parameter:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->create_parameter: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/4/projects/{projectId}/connections/{connectionId}/parameters 

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

<a id="create_parameter_link"></a>
# **create_parameter_link**
> ConParameterLink create_parameter_link(project_id, connection_id, con_parameter_link_create=con_parameter_link_create)

Links a parameter to a model property so the parameter drives it. Name the owner exactly as the  linkable-properties catalog reports it and pass its propertyId unchanged.    A link is deliberately type-agnostic: the parameter's type is not checked against the property's  `valueType`, because an Expression parameter can evaluate to any type and the value a  parameter yields is only known once it is evaluated. A parameter whose value the property cannot  take is reported by the engine when the parameters are applied, not when the link is created -  so match the catalog's `valueType` when choosing which parameter to link.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection. | 
 **con_parameter_link_create** | [**ConParameterLinkCreate**](ConParameterLinkCreate.md)| Parameter identifier, property owner and property id. | [optional] 

### Return type

[**ConParameterLink**](ConParameterLink.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_parameter_link import ConParameterLink
from ideastatica_connection_api.models.con_parameter_link_create import ConParameterLinkCreate
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def create_parameter_linkExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection.
    con_parameter_link_create = ideastatica_connection_api.ConParameterLinkCreate() # ConParameterLinkCreate | Parameter identifier, property owner and property id. (optional)

    try:
        # Links a parameter to a model property so the parameter drives it. Name the owner exactly as the  linkable-properties catalog reports it and pass its propertyId unchanged.    A link is deliberately type-agnostic: the parameter's type is not checked against the property's  `valueType`, because an Expression parameter can evaluate to any type and the value a  parameter yields is only known once it is evaluated. A parameter whose value the property cannot  take is reported by the engine when the parameters are applied, not when the link is created -  so match the catalog's `valueType` when choosing which parameter to link.
        api_response = api_client.parameter.create_parameter_link(project_id, connection_id, con_parameter_link_create=con_parameter_link_create)
        print("The response of ParameterApi->create_parameter_link:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->create_parameter_link: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/4/projects/{projectId}/connections/{connectionId}/parameters/links 

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

<a id="delete_parameter"></a>
# **delete_parameter**
> ConParameterDeleteResult delete_parameter(project_id, connection_id, key)

Deletes one parameter and every link through which it drove a model property.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection. | 
 **key** | **str**| Identifier of the parameter to delete. | 

### Return type

[**ConParameterDeleteResult**](ConParameterDeleteResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_parameter_delete_result import ConParameterDeleteResult
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def delete_parameterExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection.
    key = 'key_example' # str | Identifier of the parameter to delete.

    try:
        # Deletes one parameter and every link through which it drove a model property.
        api_response = api_client.parameter.delete_parameter(project_id, connection_id, key)
        print("The response of ParameterApi->delete_parameter:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->delete_parameter: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/4/projects/{projectId}/connections/{connectionId}/parameters/{key} 

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

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="delete_parameter_link"></a>
# **delete_parameter_link**
> delete_parameter_link(project_id, connection_id, link_id)

Removes a parameter-to-property link. The parameter is kept, and the property retains the value  the parameter last applied.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection. | 
 **link_id** | **int**| Id of the link to remove. | 

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
def delete_parameter_linkExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection.
    link_id = 56 # int | Id of the link to remove.

    try:
        # Removes a parameter-to-property link. The parameter is kept, and the property retains the value  the parameter last applied.
        api_client.parameter.delete_parameter_link(project_id, connection_id, link_id)
    except Exception as e:
        print("Exception when calling ParameterApi->delete_parameter_link: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/4/projects/{projectId}/connections/{connectionId}/parameters/links/{linkId} 

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
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="delete_parameters"></a>
# **delete_parameters**
> delete_parameters(project_id, connection_id)

Delete all parameters and parameter model links for the connection connectionId in the project projectId.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection where to delete parameters. | 

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
def delete_parametersExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection where to delete parameters.

    try:
        # Delete all parameters and parameter model links for the connection connectionId in the project projectId.
        api_client.parameter.delete_parameters(project_id, connection_id)
    except Exception as e:
        print("Exception when calling ParameterApi->delete_parameters: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/4/projects/{projectId}/connections/{connectionId}/parameters 

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

<a id="evaluate_expression"></a>
# **evaluate_expression**
> str evaluate_expression(project_id, connection_id, body=body)

Evaluate the expression and return the result.  For more details see documentation about parameters:  https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html  or  https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection to use for evaluation expression. | 
 **body** | **str**| Expression to evaluate. See the API documentation for supported syntax and examples: https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html | [optional] 

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
def evaluate_expressionExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection to use for evaluation expression.
    body = 'body_example' # str | Expression to evaluate. See the API documentation for supported syntax and examples: https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html (optional)

    try:
        # Evaluate the expression and return the result.  For more details see documentation about parameters:  https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html  or  https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html
        api_response = api_client.parameter.evaluate_expression(project_id, connection_id, body=body)
        print("The response of ParameterApi->evaluate_expression:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->evaluate_expression: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/4/projects/{projectId}/connections/{connectionId}/evaluate-expression 

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

<a id="get_linkable_properties"></a>
# **get_linkable_properties**
> List[ConLinkableProperty] get_linkable_properties(project_id, connection_id, value_type=value_type)

Lists every model property of the connection that a parameter can be linked to, across  operations, members and the library items the connection edits (cross-sections, materials,  bolt assemblies). Each row names its owner.    The list is not filtered by what the Design tab currently shows: deciding that needs the  desktop editor context, which the service does not have. Some rows therefore belong to a  property the application hides for the operation's present configuration, and linking a  parameter to one of those drives a value the design does not use.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection. | 
 **value_type** | **str**| Keep only properties of this parameter type, e.g. &#x60;Float&#x60;. | [optional] 

### Return type

[**List[ConLinkableProperty]**](ConLinkableProperty.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_linkable_property import ConLinkableProperty
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_linkable_propertiesExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection.
    value_type = 'value_type_example' # str | Keep only properties of this parameter type, e.g. `Float`. (optional)

    try:
        # Lists every model property of the connection that a parameter can be linked to, across  operations, members and the library items the connection edits (cross-sections, materials,  bolt assemblies). Each row names its owner.    The list is not filtered by what the Design tab currently shows: deciding that needs the  desktop editor context, which the service does not have. Some rows therefore belong to a  property the application hides for the operation's present configuration, and linking a  parameter to one of those drives a value the design does not use.
        api_response = api_client.parameter.get_linkable_properties(project_id, connection_id, value_type=value_type)
        print("The response of ParameterApi->get_linkable_properties:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->get_linkable_properties: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/parameters/linkable-properties 

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

<a id="get_member_linkable_properties"></a>
# **get_member_linkable_properties**
> List[ConLinkableProperty] get_member_linkable_properties(project_id, connection_id, member_id, value_type=value_type)

Lists the model properties of one member that a parameter can be linked to.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection. | 
 **member_id** | **int**| Id of the member whose properties are listed. | 
 **value_type** | **str**| Keep only properties of this parameter type, e.g. &#x60;Float&#x60;. | [optional] 

### Return type

[**List[ConLinkableProperty]**](ConLinkableProperty.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_linkable_property import ConLinkableProperty
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_member_linkable_propertiesExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection.
    member_id = 56 # int | Id of the member whose properties are listed.
    value_type = 'value_type_example' # str | Keep only properties of this parameter type, e.g. `Float`. (optional)

    try:
        # Lists the model properties of one member that a parameter can be linked to.
        api_response = api_client.parameter.get_member_linkable_properties(project_id, connection_id, member_id, value_type=value_type)
        print("The response of ParameterApi->get_member_linkable_properties:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->get_member_linkable_properties: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/members/{memberId}/linkable-properties 

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

<a id="get_operation_linkable_properties"></a>
# **get_operation_linkable_properties**
> List[ConLinkableProperty] get_operation_linkable_properties(project_id, connection_id, operation_id, value_type=value_type)

Lists the model properties of one operation that a parameter can be linked to.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection. | 
 **operation_id** | **int**| Id of the operation whose properties are listed. | 
 **value_type** | **str**| Keep only properties of this parameter type, e.g. &#x60;Float&#x60;. | [optional] 

### Return type

[**List[ConLinkableProperty]**](ConLinkableProperty.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_linkable_property import ConLinkableProperty
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_operation_linkable_propertiesExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection.
    operation_id = 56 # int | Id of the operation whose properties are listed.
    value_type = 'value_type_example' # str | Keep only properties of this parameter type, e.g. `Float`. (optional)

    try:
        # Lists the model properties of one operation that a parameter can be linked to.
        api_response = api_client.parameter.get_operation_linkable_properties(project_id, connection_id, operation_id, value_type=value_type)
        print("The response of ParameterApi->get_operation_linkable_properties:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->get_operation_linkable_properties: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/operations/{operationId}/linkable-properties 

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

<a id="get_parameter_links"></a>
# **get_parameter_links**
> List[ConParameterLink] get_parameter_links(project_id, connection_id)

Lists the parameter-to-property links of the connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| Id of the connection. | 

### Return type

[**List[ConParameterLink]**](ConParameterLink.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_parameter_link import ConParameterLink
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_parameter_linksExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | Id of the connection.

    try:
        # Lists the parameter-to-property links of the connection.
        api_response = api_client.parameter.get_parameter_links(project_id, connection_id)
        print("The response of ParameterApi->get_parameter_links:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->get_parameter_links: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/parameters/links 

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

<a id="get_parameters"></a>
# **get_parameters**
> List[IdeaParameter] get_parameters(project_id, connection_id, include_hidden=include_hidden)

Gets all parameters defined for the specified project and connection.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection from which to retrieve parameters. | 
 **include_hidden** | **bool**| If true, includes hidden parameters in the result. | [optional] [default to False]

### Return type

[**List[IdeaParameter]**](IdeaParameter.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.idea_parameter import IdeaParameter
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_parametersExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection from which to retrieve parameters.
    include_hidden = False # bool | If true, includes hidden parameters in the result. (optional) (default to False)

    try:
        # Gets all parameters defined for the specified project and connection.
        api_response = api_client.parameter.get_parameters(project_id, connection_id, include_hidden=include_hidden)
        print("The response of ParameterApi->get_parameters:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->get_parameters: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/parameters 

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

<a id="update"></a>
# **update**
> ParameterUpdateResponse update(project_id, connection_id, idea_parameter_update=idea_parameter_update)

Updates parameters for the specified connection in the project with the values provided.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection to update. | 
 **idea_parameter_update** | [**List[IdeaParameterUpdate]**](IdeaParameterUpdate.md)| New values of parameters to apply. | [optional] 

### Return type

[**ParameterUpdateResponse**](ParameterUpdateResponse.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.idea_parameter_update import IdeaParameterUpdate
from ideastatica_connection_api.models.parameter_update_response import ParameterUpdateResponse
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def updateExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection to update.
    idea_parameter_update = [ideastatica_connection_api.IdeaParameterUpdate()] # List[IdeaParameterUpdate] | New values of parameters to apply. (optional)

    try:
        # Updates parameters for the specified connection in the project with the values provided.
        api_response = api_client.parameter.update(project_id, connection_id, idea_parameter_update=idea_parameter_update)
        print("The response of ParameterApi->update:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ParameterApi->update: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/4/projects/{projectId}/connections/{connectionId}/parameters 

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

