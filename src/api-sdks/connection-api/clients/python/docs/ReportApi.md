# ReportApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**generate_pdf**](ReportApi.md#generate_pdf) | Generates report for projectId and connectionId
[**generate_pdf_for_mutliple**](ReportApi.md#generate_pdf_for_mutliple) | 
[**generate_word**](ReportApi.md#generate_word) | Generates report for projectId and connectionId
[**generate_word_for_multiple**](ReportApi.md#generate_word_for_multiple) | 


<a id="generate_pdf"></a>
# **generate_pdf**
> generate_pdf(project_id, connection_id)

Generates report for projectId and connectionId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 

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
def generate_pdfExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Generates report for projectId and connectionId
        api_client.report.generate_pdf(project_id, connection_id)
    except Exception as e:
        print("Exception when calling ReportApi->generate_pdf: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/reports/pdf 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="generate_pdf_for_mutliple"></a>
# **generate_pdf_for_mutliple**
> generate_pdf_for_mutliple(project_id, request_body)



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **request_body** | [**List[int]**](int.md)|  | 

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
def generate_pdf_for_mutlipleExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    request_body = [56] # List[int] | 

    try:
        api_client.report.generate_pdf_for_mutliple(project_id, request_body)
    except Exception as e:
        print("Exception when calling ReportApi->generate_pdf_for_mutliple: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/reports/pdf 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="generate_word"></a>
# **generate_word**
> generate_word(project_id, connection_id)

Generates report for projectId and connectionId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 

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
def generate_wordExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Generates report for projectId and connectionId
        api_client.report.generate_word(project_id, connection_id)
    except Exception as e:
        print("Exception when calling ReportApi->generate_word: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/reports/word 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="generate_word_for_multiple"></a>
# **generate_word_for_multiple**
> generate_word_for_multiple(project_id, request_body)



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **request_body** | [**List[int]**](int.md)|  | 

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
def generate_word_for_multipleExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    request_body = [56] # List[int] | 

    try:
        api_client.report.generate_word_for_multiple(project_id, request_body)
    except Exception as e:
        print("Exception when calling ReportApi->generate_word_for_multiple: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/reports/word 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

