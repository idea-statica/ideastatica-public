# CalculationApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**calculate**](CalculationApi.md#calculate) | Run CBFEM caluclation and return the summary of the results
[**get_raw_json_results**](CalculationApi.md#get_raw_json_results) | Get json string which represents raw CBFEM results (an instance of CheckResultsData)
[**get_results**](CalculationApi.md#get_results) | Get detailed results of the CBFEM analysis


<a id="calculate"></a>
# **calculate**
> List[ConResultSummary] calculate(project_id, con_calculation_parameter=con_calculation_parameter)

Run CBFEM caluclation and return the summary of the results

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_calculation_parameter** | [**ConCalculationParameter**](ConCalculationParameter.md)| List of connections to calculate and a type of CBFEM analysis | [optional] 

### Return type

[**List[ConResultSummary]**](ConResultSummary.md)

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_calculation_parameter import ConCalculationParameter
from ideastatica_connection_api.models.con_result_summary import ConResultSummary
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
    api_instance = ideastatica_connection_api.CalculationApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_calculation_parameter = ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional)

    try:
        # Run CBFEM caluclation and return the summary of the results
        api_response = api_instance.calculate(project_id, con_calculation_parameter=con_calculation_parameter)
        print("The response of CalculationApi->calculate:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling CalculationApi->calculate: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/connections/calculate 

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

<a id="get_raw_json_results"></a>
# **get_raw_json_results**
> List[str] get_raw_json_results(project_id, con_calculation_parameter=con_calculation_parameter)

Get json string which represents raw CBFEM results (an instance of CheckResultsData)

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened connection in the ConnectionRestApi service | 
 **con_calculation_parameter** | [**ConCalculationParameter**](ConCalculationParameter.md)| Type of requested analysis and connection to calculate | [optional] 

### Return type

**List[str]**

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_calculation_parameter import ConCalculationParameter
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
    api_instance = ideastatica_connection_api.CalculationApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened connection in the ConnectionRestApi service
    con_calculation_parameter = ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | Type of requested analysis and connection to calculate (optional)

    try:
        # Get json string which represents raw CBFEM results (an instance of CheckResultsData)
        api_response = api_instance.get_raw_json_results(project_id, con_calculation_parameter=con_calculation_parameter)
        print("The response of CalculationApi->get_raw_json_results:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling CalculationApi->get_raw_json_results: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/connections/rawresults-text 

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

<a id="get_results"></a>
# **get_results**
> List[ConnectionCheckRes] get_results(project_id, con_calculation_parameter=con_calculation_parameter)

Get detailed results of the CBFEM analysis

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_calculation_parameter** | [**ConCalculationParameter**](ConCalculationParameter.md)| List of connections to calculate and a type of CBFEM analysis | [optional] 

### Return type

[**List[ConnectionCheckRes]**](ConnectionCheckRes.md)

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_calculation_parameter import ConCalculationParameter
from ideastatica_connection_api.models.connection_check_res import ConnectionCheckRes
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
    api_instance = ideastatica_connection_api.CalculationApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_calculation_parameter = ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional)

    try:
        # Get detailed results of the CBFEM analysis
        api_response = api_instance.get_results(project_id, con_calculation_parameter=con_calculation_parameter)
        print("The response of CalculationApi->get_results:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling CalculationApi->get_results: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/connections/results 

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

