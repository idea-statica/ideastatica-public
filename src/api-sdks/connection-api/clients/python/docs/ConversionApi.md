# ConversionApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**change_code**](ConversionApi.md#change_code) | Change design code of project.
[**get_conversion_mapping**](ConversionApi.md#get_conversion_mapping) | Get default conversions for converting the project to different design code.


<a id="change_code"></a>
# **change_code**
> str change_code(project_id, con_conversion_settings=con_conversion_settings)

Change design code of project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. (only ECEN projects are supported) | 
 **con_conversion_settings** | [**ConConversionSettings**](ConConversionSettings.md)| Conversion table for materials in the project. (pairs &#39;ECEN MATERIAL&#39; -&gt; &#39;TARGET DESIGN CODE MATERIAL&#39;) | [optional] 

### Return type

**str**

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_conversion_settings import ConConversionSettings
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
    api_instance = ideastatica_connection_api.ConversionApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service. (only ECEN projects are supported)
    con_conversion_settings = ideastatica_connection_api.ConConversionSettings() # ConConversionSettings | Conversion table for materials in the project. (pairs 'ECEN MATERIAL' -> 'TARGET DESIGN CODE MATERIAL') (optional)

    try:
        # Change design code of project.
        api_response = api_instance.change_code(project_id, con_conversion_settings=con_conversion_settings)
        print("The response of ConversionApi->change_code:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ConversionApi->change_code: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/change-code 

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

<a id="get_conversion_mapping"></a>
# **get_conversion_mapping**
> ConConversionSettings get_conversion_mapping(project_id, country_code=country_code)

Get default conversions for converting the project to different design code.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service. (only ECEN design code is supported) | 
 **country_code** | [**CountryCode**](.md)| Requested design code in the converted project. | [optional] 

### Return type

[**ConConversionSettings**](ConConversionSettings.md)

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_conversion_settings import ConConversionSettings
from ideastatica_connection_api.models.country_code import CountryCode
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
    api_instance = ideastatica_connection_api.ConversionApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service. (only ECEN design code is supported)
    country_code = ideastatica_connection_api.CountryCode() # CountryCode | Requested design code in the converted project. (optional)

    try:
        # Get default conversions for converting the project to different design code.
        api_response = api_instance.get_conversion_mapping(project_id, country_code=country_code)
        print("The response of ConversionApi->get_conversion_mapping:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ConversionApi->get_conversion_mapping: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/get-default-mapping 

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

