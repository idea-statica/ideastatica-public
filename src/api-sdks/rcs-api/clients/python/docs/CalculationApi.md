# CalculationApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**calculate**](CalculationApi.md#calculate) | Calculate RCS project
[**get_results**](CalculationApi.md#get_results) | Get calculated results


<a id="calculate"></a>
# **calculate**
> List[RcsSectionResultOverview] calculate(project_id, rcs_calculation_parameters=rcs_calculation_parameters)

Calculate RCS project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Project Id | 
 **rcs_calculation_parameters** | [**RcsCalculationParameters**](RcsCalculationParameters.md)| Calculation parameters | [optional] 

### Return type

[**List[RcsSectionResultOverview]**](RcsSectionResultOverview.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_calculation_parameters import RcsCalculationParameters
from ideastatica_rcs_api.models.rcs_section_result_overview import RcsSectionResultOverview
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
    api_instance = ideastatica_rcs_api.CalculationApi(api_client)
    project_id = 'project_id_example' # str | Project Id
    rcs_calculation_parameters = ideastatica_rcs_api.RcsCalculationParameters() # RcsCalculationParameters | Calculation parameters (optional)

    try:
        # Calculate RCS project
        api_response = api_instance.calculate(project_id, rcs_calculation_parameters=rcs_calculation_parameters)
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

> **POST** /api/1/projects/{projectId}/calculate 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_results"></a>
# **get_results**
> List[RcsSectionResultDetailed] get_results(project_id, rcs_result_parameters=rcs_result_parameters)

Get calculated results

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Project Id | 
 **rcs_result_parameters** | [**RcsResultParameters**](RcsResultParameters.md)| Calculation parameters | [optional] 

### Return type

[**List[RcsSectionResultDetailed]**](RcsSectionResultDetailed.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_result_parameters import RcsResultParameters
from ideastatica_rcs_api.models.rcs_section_result_detailed import RcsSectionResultDetailed
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
    api_instance = ideastatica_rcs_api.CalculationApi(api_client)
    project_id = 'project_id_example' # str | Project Id
    rcs_result_parameters = ideastatica_rcs_api.RcsResultParameters() # RcsResultParameters | Calculation parameters (optional)

    try:
        # Get calculated results
        api_response = api_instance.get_results(project_id, rcs_result_parameters=rcs_result_parameters)
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

> **POST** /api/1/projects/{projectId}/get-results 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

