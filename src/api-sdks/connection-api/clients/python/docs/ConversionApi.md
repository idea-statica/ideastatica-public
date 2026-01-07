# ConversionApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**change_code**](ConversionApi.md#change_code) | Changes the design code of the project.
[**get_conversion_mapping**](ConversionApi.md#get_conversion_mapping) | Gets default conversion mappings for converting the project to a different design code.


<a id="change_code"></a>
# **change_code**
> str change_code(project_id, con_conversion_settings=con_conversion_settings)

Changes the design code of the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service (only ECEN projects are supported). | 
 **con_conversion_settings** | [**ConConversionSettings**](ConConversionSettings.md)| Conversion table for materials in the project (pairs &#39;ECEN MATERIAL&#39; to &#39;TARGET DESIGN CODE MATERIAL&#39;). | [optional] 

### Return type

**str**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_conversion_settings import ConConversionSettings
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def change_codeExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service (only ECEN projects are supported).
    con_conversion_settings = ideastatica_connection_api.ConConversionSettings() # ConConversionSettings | Conversion table for materials in the project (pairs 'ECEN MATERIAL' to 'TARGET DESIGN CODE MATERIAL'). (optional)

    try:
        # Changes the design code of the project.
        api_response = api_client.conversion.change_code(project_id, con_conversion_settings=con_conversion_settings)
        print("The response of ConversionApi->change_code:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConversionApi->change_code: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/3/projects/{projectId}/change-code 

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

<a id="get_conversion_mapping"></a>
# **get_conversion_mapping**
> ConConversionSettings get_conversion_mapping(project_id, country_code=country_code)

Gets default conversion mappings for converting the project to a different design code.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service (only ECEN design code is supported). | 
 **country_code** | [**CountryCode**](.md)| The requested design code for the converted project. | [optional] 

### Return type

[**ConConversionSettings**](ConConversionSettings.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_conversion_settings import ConConversionSettings
from ideastatica_connection_api.models.country_code import CountryCode
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_conversion_mappingExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service (only ECEN design code is supported).
    country_code = ideastatica_connection_api.CountryCode() # CountryCode | The requested design code for the converted project. (optional)

    try:
        # Gets default conversion mappings for converting the project to a different design code.
        api_response = api_client.conversion.get_conversion_mapping(project_id, country_code=country_code)
        print("The response of ConversionApi->get_conversion_mapping:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConversionApi->get_conversion_mapping: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/3/projects/{projectId}/get-default-mapping 

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

