# CalculationApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**calculate**](CalculationApi.md#calculate) | Runs CBFEM calculation and returns the summary of the results.
[**get_raw_json_results**](CalculationApi.md#get_raw_json_results) | Gets JSON string which represents raw CBFEM results (an instance of CheckResultsData).
[**get_results**](CalculationApi.md#get_results) | Gets detailed results of the CBFEM analysis.


<a id="calculate"></a>
# **calculate**
> List[ConResultSummary] calculate(project_id, request_body)

Runs CBFEM calculation and returns the summary of the results.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **request_body** | [**List[int]**](int.md)| List of connection IDs to calculate. | 

### Return type

[**List[ConResultSummary]**](ConResultSummary.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_result_summary import ConResultSummary
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def calculateExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    request_body = [56] # List[int] | List of connection IDs to calculate.

    try:
        # Runs CBFEM calculation and returns the summary of the results.
        api_response = api_client.calculation.calculate(project_id, request_body)
        print("The response of CalculationApi->calculate:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling CalculationApi->calculate: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/3/projects/{projectId}/connections/calculate 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_raw_json_results"></a>
# **get_raw_json_results**
> List[str] get_raw_json_results(project_id, request_body)

Gets JSON string which represents raw CBFEM results (an instance of CheckResultsData).

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **request_body** | [**List[int]**](int.md)| List of connection IDs to calculate. | 

### Return type

**List[str]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_raw_json_resultsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    request_body = [56] # List[int] | List of connection IDs to calculate.

    try:
        # Gets JSON string which represents raw CBFEM results (an instance of CheckResultsData).
        api_response = api_client.calculation.get_raw_json_results(project_id, request_body)
        print("The response of CalculationApi->get_raw_json_results:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling CalculationApi->get_raw_json_results: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/3/projects/{projectId}/connections/rawresults-text 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_results"></a>
# **get_results**
> List[ConnectionCheckRes] get_results(project_id, request_body)

Gets detailed results of the CBFEM analysis.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **request_body** | [**List[int]**](int.md)| List of connection IDs to calculate. | 

### Return type

[**List[ConnectionCheckRes]**](ConnectionCheckRes.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.connection_check_res import ConnectionCheckRes
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_resultsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    request_body = [56] # List[int] | List of connection IDs to calculate.

    try:
        # Gets detailed results of the CBFEM analysis.
        api_response = api_client.calculation.get_results(project_id, request_body)
        print("The response of CalculationApi->get_results:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling CalculationApi->get_results: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/3/projects/{projectId}/connections/results 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

