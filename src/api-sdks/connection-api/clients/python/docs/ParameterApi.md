# ParameterApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**evaluate_expression**](ParameterApi.md#evaluate_expression) | Evaluate the expression and return the result
[**get_parameters**](ParameterApi.md#get_parameters) | Get all parameters which are defined for projectId and connectionId
[**update_parameters**](ParameterApi.md#update_parameters) | Update parameters for the connection connectionId in the project projectId by values passed in parameters


<a id="evaluate_expression"></a>
# **evaluate_expression**
> str evaluate_expression(project_id, connection_id, body=body)

Evaluate the expression and return the result

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to use for evaluation expression | 
 **body** | **str**| Expression to evaluate | [optional] 

### Return type

**str**

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
    api_instance = ideastatica_connection_api.ParameterApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to use for evaluation expression
    body = 'body_example' # str | Expression to evaluate (optional)

    try:
        # Evaluate the expression and return the result
        api_response = api_instance.evaluate_expression(project_id, connection_id, body=body)
        print("The response of ParameterApi->evaluate_expression:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ParameterApi->evaluate_expression: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/connections/{connectionId}/evaluate-expression 

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

<a id="get_parameters"></a>
# **get_parameters**
> List[IdeaParameter] get_parameters(project_id, connection_id, include_hidden=include_hidden)

Get all parameters which are defined for projectId and connectionId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to get its parameters | 
 **include_hidden** | **bool**| Include also hidden parameters | [optional] [default to False]

### Return type

[**List[IdeaParameter]**](IdeaParameter.md)

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.idea_parameter import IdeaParameter
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
    api_instance = ideastatica_connection_api.ParameterApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to get its parameters
    include_hidden = False # bool | Include also hidden parameters (optional) (default to False)

    try:
        # Get all parameters which are defined for projectId and connectionId
        api_response = api_instance.get_parameters(project_id, connection_id, include_hidden=include_hidden)
        print("The response of ParameterApi->get_parameters:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ParameterApi->get_parameters: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/connections/{connectionId}/parameters 

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

<a id="update_parameters"></a>
# **update_parameters**
> List[IdeaParameter] update_parameters(project_id, connection_id, idea_parameter_update=idea_parameter_update)

Update parameters for the connection connectionId in the project projectId by values passed in parameters

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to apply template | 
 **idea_parameter_update** | [**List[IdeaParameterUpdate]**](IdeaParameterUpdate.md)| New values of parameters | [optional] 

### Return type

[**List[IdeaParameter]**](IdeaParameter.md)

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.idea_parameter import IdeaParameter
from ideastatica_connection_api.models.idea_parameter_update import IdeaParameterUpdate
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
    api_instance = ideastatica_connection_api.ParameterApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to apply template
    idea_parameter_update = [ideastatica_connection_api.IdeaParameterUpdate()] # List[IdeaParameterUpdate] | New values of parameters (optional)

    try:
        # Update parameters for the connection connectionId in the project projectId by values passed in parameters
        api_response = api_instance.update_parameters(project_id, connection_id, idea_parameter_update=idea_parameter_update)
        print("The response of ParameterApi->update_parameters:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ParameterApi->update_parameters: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/1/projects/{projectId}/connections/{connectionId}/parameters 

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

