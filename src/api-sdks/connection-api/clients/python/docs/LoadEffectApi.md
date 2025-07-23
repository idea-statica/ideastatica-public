# LoadEffectApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**add_load_effect**](LoadEffectApi.md#add_load_effect) | Add new load effect to the connection
[**delete_load_effect**](LoadEffectApi.md#delete_load_effect) | Delete load effect loadEffectId
[**get_load_effect**](LoadEffectApi.md#get_load_effect) | Get load impulses from loadEffectId
[**get_load_effects**](LoadEffectApi.md#get_load_effects) | Get all load effects which are defined in connectionId
[**get_load_settings**](LoadEffectApi.md#get_load_settings) | Get Load settings for connection in project
[**set_load_settings**](LoadEffectApi.md#set_load_settings) | Set Load settings for connection in project
[**update_load_effect**](LoadEffectApi.md#update_load_effect) | Update load impulses in conLoading


<a id="add_load_effect"></a>
# **add_load_effect**
> ConLoadEffect add_load_effect(project_id, connection_id, con_load_effect=con_load_effect)

Add new load effect to the connection

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **con_load_effect** | [**ConLoadEffect**](ConLoadEffect.md)|  | [optional] 

### Return type

[**ConLoadEffect**](ConLoadEffect.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_effect import ConLoadEffect
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_load_effectExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    con_load_effect = ideastatica_connection_api.ConLoadEffect() # ConLoadEffect |  (optional)

    try:
        # Add new load effect to the connection
        api_response = api_client.loadeffect.add_load_effect(project_id, connection_id, con_load_effect=con_load_effect)
        print("The response of LoadEffectApi->add_load_effect:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling LoadEffectApi->add_load_effect: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/load-effects 

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

<a id="delete_load_effect"></a>
# **delete_load_effect**
> int delete_load_effect(project_id, connection_id, load_effect_id)

Delete load effect loadEffectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **load_effect_id** | **int**|  | 

### Return type

**int**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def delete_load_effectExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    load_effect_id = 56 # int | 

    try:
        # Delete load effect loadEffectId
        api_response = api_client.loadeffect.delete_load_effect(project_id, connection_id, load_effect_id)
        print("The response of LoadEffectApi->delete_load_effect:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling LoadEffectApi->delete_load_effect: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/2/projects/{projectId}/connections/{connectionId}/load-effects/{loadEffectId} 

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

<a id="get_load_effect"></a>
# **get_load_effect**
> ConLoadEffect get_load_effect(project_id, connection_id, load_effect_id, is_percentage=is_percentage)

Get load impulses from loadEffectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **load_effect_id** | **int**|  | 
 **is_percentage** | **bool**|  | [optional] 

### Return type

[**ConLoadEffect**](ConLoadEffect.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_effect import ConLoadEffect
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_load_effectExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    load_effect_id = 56 # int | 
    is_percentage = True # bool |  (optional)

    try:
        # Get load impulses from loadEffectId
        api_response = api_client.loadeffect.get_load_effect(project_id, connection_id, load_effect_id, is_percentage=is_percentage)
        print("The response of LoadEffectApi->get_load_effect:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling LoadEffectApi->get_load_effect: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/load-effects/{loadEffectId} 

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

<a id="get_load_effects"></a>
# **get_load_effects**
> List[ConLoadEffect] get_load_effects(project_id, connection_id, is_percentage=is_percentage)

Get all load effects which are defined in connectionId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **is_percentage** | **bool**|  | [optional] 

### Return type

[**List[ConLoadEffect]**](ConLoadEffect.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_effect import ConLoadEffect
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_load_effectsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    is_percentage = True # bool |  (optional)

    try:
        # Get all load effects which are defined in connectionId
        api_response = api_client.loadeffect.get_load_effects(project_id, connection_id, is_percentage=is_percentage)
        print("The response of LoadEffectApi->get_load_effects:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling LoadEffectApi->get_load_effects: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/load-effects 

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

<a id="get_load_settings"></a>
# **get_load_settings**
> ConLoadSettings get_load_settings(project_id, connection_id)

Get Load settings for connection in project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 

### Return type

[**ConLoadSettings**](ConLoadSettings.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_settings import ConLoadSettings
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_load_settingsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Get Load settings for connection in project
        api_response = api_client.loadeffect.get_load_settings(project_id, connection_id)
        print("The response of LoadEffectApi->get_load_settings:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling LoadEffectApi->get_load_settings: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/load-effects/get-load-settings 

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

<a id="set_load_settings"></a>
# **set_load_settings**
> ConLoadSettings set_load_settings(project_id, connection_id, con_load_settings=con_load_settings)

Set Load settings for connection in project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **con_load_settings** | [**ConLoadSettings**](ConLoadSettings.md)|  | [optional] 

### Return type

[**ConLoadSettings**](ConLoadSettings.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_settings import ConLoadSettings
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def set_load_settingsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    con_load_settings = ideastatica_connection_api.ConLoadSettings() # ConLoadSettings |  (optional)

    try:
        # Set Load settings for connection in project
        api_response = api_client.loadeffect.set_load_settings(project_id, connection_id, con_load_settings=con_load_settings)
        print("The response of LoadEffectApi->set_load_settings:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling LoadEffectApi->set_load_settings: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/load-effects/set-load-settings 

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

<a id="update_load_effect"></a>
# **update_load_effect**
> ConLoadEffect update_load_effect(project_id, connection_id, con_load_effect=con_load_effect)

Update load impulses in conLoading

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **con_load_effect** | [**ConLoadEffect**](ConLoadEffect.md)|  | [optional] 

### Return type

[**ConLoadEffect**](ConLoadEffect.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_load_effect import ConLoadEffect
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_load_effectExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    con_load_effect = ideastatica_connection_api.ConLoadEffect() # ConLoadEffect |  (optional)

    try:
        # Update load impulses in conLoading
        api_response = api_client.loadeffect.update_load_effect(project_id, connection_id, con_load_effect=con_load_effect)
        print("The response of LoadEffectApi->update_load_effect:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling LoadEffectApi->update_load_effect: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/2/projects/{projectId}/connections/{connectionId}/load-effects 

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

