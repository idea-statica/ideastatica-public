# CalculationJobsApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**cancel_calculation_job**](CalculationJobsApi.md#cancel_calculation_job) | Requests cancellation of an asynchronous calculation job: the in-flight solver process is  killed and remaining connections are skipped. Connections whose solve was interrupted stay  not-calculated; already completed connections keep their results. Idempotent — cancelling  a finished job leaves it unchanged.
[**get_calculation_job**](CalculationJobsApi.md#get_calculation_job) | Gets the current state of an asynchronous calculation job: its status, the connection  currently being calculated with its per-load-case solver progress, and — once finished —  the result summaries.
[**start_calculation**](CalculationJobsApi.md#start_calculation) | Starts an asynchronous CBFEM calculation job for the given connections.
[**start_connection_calculation**](CalculationJobsApi.md#start_connection_calculation) | Starts an asynchronous CBFEM calculation job for a single connection.


<a id="cancel_calculation_job"></a>
# **cancel_calculation_job**
> ConCalculationJob cancel_calculation_job(project_id, job_id)

Requests cancellation of an asynchronous calculation job: the in-flight solver process is  killed and remaining connections are skipped. Connections whose solve was interrupted stay  not-calculated; already completed connections keep their results. Idempotent — cancelling  a finished job leaves it unchanged.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **job_id** | **str**| The id of the job returned by the calculate-async endpoints. | 

### Return type

[**ConCalculationJob**](ConCalculationJob.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_calculation_job import ConCalculationJob
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def cancel_calculation_jobExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    job_id = 'job_id_example' # str | The id of the job returned by the calculate-async endpoints.

    try:
        # Requests cancellation of an asynchronous calculation job: the in-flight solver process is  killed and remaining connections are skipped. Connections whose solve was interrupted stay  not-calculated; already completed connections keep their results. Idempotent — cancelling  a finished job leaves it unchanged.
        api_response = api_client.calculationjobs.cancel_calculation_job(project_id, job_id)
        print("The response of CalculationJobsApi->cancel_calculation_job:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling CalculationJobsApi->cancel_calculation_job: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/4/projects/{projectId}/calculation-jobs/{jobId} 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**202** | Accepted |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_calculation_job"></a>
# **get_calculation_job**
> ConCalculationJob get_calculation_job(project_id, job_id)

Gets the current state of an asynchronous calculation job: its status, the connection  currently being calculated with its per-load-case solver progress, and — once finished —  the result summaries.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **job_id** | **str**| The id of the job returned by the calculate-async endpoints. | 

### Return type

[**ConCalculationJob**](ConCalculationJob.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_calculation_job import ConCalculationJob
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_calculation_jobExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    job_id = 'job_id_example' # str | The id of the job returned by the calculate-async endpoints.

    try:
        # Gets the current state of an asynchronous calculation job: its status, the connection  currently being calculated with its per-load-case solver progress, and — once finished —  the result summaries.
        api_response = api_client.calculationjobs.get_calculation_job(project_id, job_id)
        print("The response of CalculationJobsApi->get_calculation_job:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling CalculationJobsApi->get_calculation_job: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/calculation-jobs/{jobId} 

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

<a id="start_calculation"></a>
# **start_calculation**
> ConCalculationJob start_calculation(project_id, request_body)

Starts an asynchronous CBFEM calculation job for the given connections.

Returns 202 with the accepted job immediately; poll it with the calculation-jobs GET  endpoint and cancel it with DELETE. At most one job can be active per project (409  otherwise). Connection ids are validated upfront — an unknown id rejects the start with  404 before any job is registered. Like the synchronous bulk calculate, a connection that  fails to calculate produces a failed summary row in the job's results rather than  failing the job.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **request_body** | [**List[int]**](int.md)| List of connection IDs to calculate. | 

### Return type

[**ConCalculationJob**](ConCalculationJob.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_calculation_job import ConCalculationJob
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def start_calculationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    request_body = [56] # List[int] | List of connection IDs to calculate.

    try:
        # Starts an asynchronous CBFEM calculation job for the given connections.
        api_response = api_client.calculationjobs.start_calculation(project_id, request_body)
        print("The response of CalculationJobsApi->start_calculation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling CalculationJobsApi->start_calculation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/4/projects/{projectId}/connections/calculate-async 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**202** | Accepted |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**409** | Conflict |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="start_connection_calculation"></a>
# **start_connection_calculation**
> ConCalculationJob start_connection_calculation(project_id, connection_id, load_effect_ids=load_effect_ids)

Starts an asynchronous CBFEM calculation job for a single connection.

Returns 202 with the accepted job immediately; poll it with the calculation-jobs GET  endpoint and cancel it with DELETE. At most one job can be active per project (409  otherwise).

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. | 
 **connection_id** | **int**| The ID of the connection to calculate. | 
 **load_effect_ids** | [**List[int]**](int.md)| Optional subset of load-effect ids of this connection to solve. When set,              exactly these load effects are analysed - their Active flags are ignored and nothing is persisted;              results - including subsequent results, raw-results, result-mesh and report reads - reflect only              this subset until the next calculation. When omitted, all active load effects are solved; an empty              list is treated as omitted. Unknown ids are rejected with 422. | [optional] 

### Return type

[**ConCalculationJob**](ConCalculationJob.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_calculation_job import ConCalculationJob
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def start_connection_calculationExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service.
    connection_id = 56 # int | The ID of the connection to calculate.
    load_effect_ids = [56] # List[int] | Optional subset of load-effect ids of this connection to solve. When set,              exactly these load effects are analysed - their Active flags are ignored and nothing is persisted;              results - including subsequent results, raw-results, result-mesh and report reads - reflect only              this subset until the next calculation. When omitted, all active load effects are solved; an empty              list is treated as omitted. Unknown ids are rejected with 422. (optional)

    try:
        # Starts an asynchronous CBFEM calculation job for a single connection.
        api_response = api_client.calculationjobs.start_connection_calculation(project_id, connection_id, load_effect_ids=load_effect_ids)
        print("The response of CalculationJobsApi->start_connection_calculation:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling CalculationJobsApi->start_connection_calculation: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/4/projects/{projectId}/connections/{connectionId}/calculate-async 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**202** | Accepted |  -  |
**401** | Unauthorized |  -  |
**404** | Not Found |  -  |
**409** | Conflict |  -  |
**422** | Unprocessable Content |  -  |
**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

