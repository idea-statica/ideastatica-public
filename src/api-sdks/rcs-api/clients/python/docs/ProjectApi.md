# ProjectApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**close_project**](ProjectApi.md#close_project) | 
[**download_project**](ProjectApi.md#download_project) | Download the actual rcs project from the service. It includes all changes which were made by previous API calls.
[**get_active_project**](ProjectApi.md#get_active_project) | 
[**get_code_settings**](ProjectApi.md#get_code_settings) | 
[**import_iom**](ProjectApi.md#import_iom) | 
[**import_iom_file**](ProjectApi.md#import_iom_file) | 
[**open**](ProjectApi.md#open) | 
[**open_project**](ProjectApi.md#open_project) | Open Rcs project from rcsFile
[**update_code_settings**](ProjectApi.md#update_code_settings) | 


<a id="close_project"></a>
# **close_project**
> str close_project(project_id)



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 

### Return type

**str**

### Example


```python
import ideastatica_rcs_api
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
    api_instance = ideastatica_rcs_api.ProjectApi(api_client)
    project_id = 'project_id_example' # str | 

    try:
        api_response = api_instance.close_project(project_id)
        print("The response of ProjectApi->close_project:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ProjectApi->close_project: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/1/projects/{projectId}/close 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/xml, text/xml, application/json, text/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="download_project"></a>
# **download_project**
> download_project(project_id)

Download the actual rcs project from the service. It includes all changes which were made by previous API calls.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

void (empty response body)

### Example


```python
import ideastatica_rcs_api
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
    api_instance = ideastatica_rcs_api.ProjectApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Download the actual rcs project from the service. It includes all changes which were made by previous API calls.
        api_instance.download_project(project_id)
    except Exception as e:
        print("Exception when calling ProjectApi->download_project: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/download 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_active_project"></a>
# **get_active_project**
> RcsProject get_active_project()



### Parameters

This endpoint does not need any parameter.

### Return type

[**RcsProject**](RcsProject.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_project import RcsProject
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
    api_instance = ideastatica_rcs_api.ProjectApi(api_client)

    try:
        api_response = api_instance.get_active_project()
        print("The response of ProjectApi->get_active_project:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ProjectApi->get_active_project: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/active-project 

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

<a id="get_code_settings"></a>
# **get_code_settings**
> str get_code_settings(project_id)



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 

### Return type

**str**

### Example


```python
import ideastatica_rcs_api
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
    api_instance = ideastatica_rcs_api.ProjectApi(api_client)
    project_id = 'project_id_example' # str | 

    try:
        api_response = api_instance.get_code_settings(project_id)
        print("The response of ProjectApi->get_code_settings:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ProjectApi->get_code_settings: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/code-settings 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/xml

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="import_iom"></a>
# **import_iom**
> str import_iom()



### Parameters

This endpoint does not need any parameter.

### Return type

**str**

### Example


```python
import ideastatica_rcs_api
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
    api_instance = ideastatica_rcs_api.ProjectApi(api_client)

    try:
        api_response = api_instance.import_iom()
        print("The response of ProjectApi->import_iom:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ProjectApi->import_iom: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/import-iom 

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

<a id="import_iom_file"></a>
# **import_iom_file**
> RcsProject import_iom_file(iom_file=iom_file)



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **iom_file** | **bytearray**|  | [optional] 

### Return type

[**RcsProject**](RcsProject.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_project import RcsProject
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
    api_instance = ideastatica_rcs_api.ProjectApi(api_client)
    iom_file = None # bytearray |  (optional)

    try:
        api_response = api_instance.import_iom_file(iom_file=iom_file)
        print("The response of ProjectApi->import_iom_file:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ProjectApi->import_iom_file: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/import-iom-file 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: multipart/form-data
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="open"></a>
# **open**
> str open()



### Parameters

This endpoint does not need any parameter.

### Return type

**str**

### Example


```python
import ideastatica_rcs_api
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
    api_instance = ideastatica_rcs_api.ProjectApi(api_client)

    try:
        api_response = api_instance.open()
        print("The response of ProjectApi->open:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ProjectApi->open: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/open 

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

<a id="open_project"></a>
# **open_project**
> RcsProject open_project(rcs_file=rcs_file)

Open Rcs project from rcsFile

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **rcs_file** | **bytearray**|  | [optional] 

### Return type

[**RcsProject**](RcsProject.md)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_project import RcsProject
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
    api_instance = ideastatica_rcs_api.ProjectApi(api_client)
    rcs_file = None # bytearray |  (optional)

    try:
        # Open Rcs project from rcsFile
        api_response = api_instance.open_project(rcs_file=rcs_file)
        print("The response of ProjectApi->open_project:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ProjectApi->open_project: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/open-project 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: multipart/form-data
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_code_settings"></a>
# **update_code_settings**
> bool update_code_settings(project_id, rcs_setting=rcs_setting)



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **rcs_setting** | [**List[RcsSetting]**](RcsSetting.md)|  | [optional] 

### Return type

**bool**

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_setting import RcsSetting
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
    api_instance = ideastatica_rcs_api.ProjectApi(api_client)
    project_id = 'project_id_example' # str | 
    rcs_setting = [ideastatica_rcs_api.RcsSetting()] # List[RcsSetting] |  (optional)

    try:
        api_response = api_instance.update_code_settings(project_id, rcs_setting=rcs_setting)
        print("The response of ProjectApi->update_code_settings:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling ProjectApi->update_code_settings: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/1/projects/{projectId}/code-settings 

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

