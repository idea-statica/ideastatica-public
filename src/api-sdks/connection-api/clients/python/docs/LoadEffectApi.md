# ideastatica_connection_api.LoadEffectApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**add_load_effect**](LoadEffectApi.md#add_load_effect) | **POST** /api/1/projects/{projectId}/connections/{connectionId}/load-effects | Add new load effect to the connection
[**delete_load_effect**](LoadEffectApi.md#delete_load_effect) | **DELETE** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/{loadEffectId} | Delete load effect loadEffectId
[**get_load_effect**](LoadEffectApi.md#get_load_effect) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/{loadEffectId} | Get load impulses from loadEffectId
[**get_load_effects**](LoadEffectApi.md#get_load_effects) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/load-effects | Get all load effects which are defined in connectionId
[**get_load_settings**](LoadEffectApi.md#get_load_settings) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/get-load-settings | Get Load settings for connection in project
[**set_load_settings**](LoadEffectApi.md#set_load_settings) | **POST** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/set-load-settings | Set Load settings for connection in project
[**update_load_effect**](LoadEffectApi.md#update_load_effect) | **PUT** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/{loadEffectId} | Update load impulses in loadEffectId


# **add_load_effect**
> LoadEffectData add_load_effect(project_id, connection_id, con_load_effect=con_load_effect)

Add new load effect to the connection

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_effect import ConLoadEffect
from ideastatica_connection_api.models.load_effect_data import LoadEffectData
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
    api_instance = ideastatica_connection_api.LoadEffectApi(api_client)
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    con_load_effect = ideastatica_connection_api.ConLoadEffect() # ConLoadEffect |  (optional)

    try:
        # Add new load effect to the connection
        api_response = api_instance.add_load_effect(project_id, connection_id, con_load_effect=con_load_effect)
        print("The response of LoadEffectApi->add_load_effect:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling LoadEffectApi->add_load_effect: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **con_load_effect** | [**ConLoadEffect**](ConLoadEffect.md)|  | [optional] 

### Return type

[**LoadEffectData**](LoadEffectData.md)

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

# **delete_load_effect**
> int delete_load_effect(project_id, connection_id, load_effect_id)

Delete load effect loadEffectId

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
    api_instance = ideastatica_connection_api.LoadEffectApi(api_client)
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    load_effect_id = 56 # int | 

    try:
        # Delete load effect loadEffectId
        api_response = api_instance.delete_load_effect(project_id, connection_id, load_effect_id)
        print("The response of LoadEffectApi->delete_load_effect:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling LoadEffectApi->delete_load_effect: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **load_effect_id** | **int**|  | 

### Return type

**int**

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

# **get_load_effect**
> ConLoadEffect get_load_effect(project_id, connection_id, load_effect_id, is_percentage=is_percentage)

Get load impulses from loadEffectId

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_effect import ConLoadEffect
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
    api_instance = ideastatica_connection_api.LoadEffectApi(api_client)
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    load_effect_id = 56 # int | 
    is_percentage = True # bool |  (optional)

    try:
        # Get load impulses from loadEffectId
        api_response = api_instance.get_load_effect(project_id, connection_id, load_effect_id, is_percentage=is_percentage)
        print("The response of LoadEffectApi->get_load_effect:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling LoadEffectApi->get_load_effect: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **load_effect_id** | **int**|  | 
 **is_percentage** | **bool**|  | [optional] 

### Return type

[**ConLoadEffect**](ConLoadEffect.md)

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

# **get_load_effects**
> List[ConLoadEffect] get_load_effects(project_id, connection_id, is_percentage=is_percentage)

Get all load effects which are defined in connectionId

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_effect import ConLoadEffect
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
    api_instance = ideastatica_connection_api.LoadEffectApi(api_client)
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    is_percentage = True # bool |  (optional)

    try:
        # Get all load effects which are defined in connectionId
        api_response = api_instance.get_load_effects(project_id, connection_id, is_percentage=is_percentage)
        print("The response of LoadEffectApi->get_load_effects:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling LoadEffectApi->get_load_effects: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **is_percentage** | **bool**|  | [optional] 

### Return type

[**List[ConLoadEffect]**](ConLoadEffect.md)

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

# **get_load_settings**
> ConLoadSettings get_load_settings(project_id, connection_id)

Get Load settings for connection in project

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_settings import ConLoadSettings
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
    api_instance = ideastatica_connection_api.LoadEffectApi(api_client)
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Get Load settings for connection in project
        api_response = api_instance.get_load_settings(project_id, connection_id)
        print("The response of LoadEffectApi->get_load_settings:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling LoadEffectApi->get_load_settings: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 

### Return type

[**ConLoadSettings**](ConLoadSettings.md)

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

# **set_load_settings**
> ConLoadSettings set_load_settings(project_id, connection_id, con_load_settings=con_load_settings)

Set Load settings for connection in project

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_settings import ConLoadSettings
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
    api_instance = ideastatica_connection_api.LoadEffectApi(api_client)
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    con_load_settings = ideastatica_connection_api.ConLoadSettings() # ConLoadSettings |  (optional)

    try:
        # Set Load settings for connection in project
        api_response = api_instance.set_load_settings(project_id, connection_id, con_load_settings=con_load_settings)
        print("The response of LoadEffectApi->set_load_settings:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling LoadEffectApi->set_load_settings: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **con_load_settings** | [**ConLoadSettings**](ConLoadSettings.md)|  | [optional] 

### Return type

[**ConLoadSettings**](ConLoadSettings.md)

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

# **update_load_effect**
> ConLoadEffect update_load_effect(project_id, connection_id, load_effect_id, con_load_effect=con_load_effect)

Update load impulses in loadEffectId

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_effect import ConLoadEffect
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
    api_instance = ideastatica_connection_api.LoadEffectApi(api_client)
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    load_effect_id = 56 # int | 
    con_load_effect = ideastatica_connection_api.ConLoadEffect() # ConLoadEffect |  (optional)

    try:
        # Update load impulses in loadEffectId
        api_response = api_instance.update_load_effect(project_id, connection_id, load_effect_id, con_load_effect=con_load_effect)
        print("The response of LoadEffectApi->update_load_effect:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling LoadEffectApi->update_load_effect: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **load_effect_id** | **int**|  | 
 **con_load_effect** | [**ConLoadEffect**](ConLoadEffect.md)|  | [optional] 

### Return type

[**ConLoadEffect**](ConLoadEffect.md)

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

