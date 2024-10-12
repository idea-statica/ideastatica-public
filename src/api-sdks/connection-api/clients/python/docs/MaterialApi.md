# ideastatica_connection_api.MaterialApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**add_bolt_assembly**](MaterialApi.md#add_bolt_assembly) | **POST** /api/1/projects/{projectId}/materials/bolt-assemblies | Add bolt assembly to the project
[**add_cross_section**](MaterialApi.md#add_cross_section) | **POST** /api/1/projects/{projectId}/materials/cross-sections | Add cross section to the project
[**add_material_bolt_grade**](MaterialApi.md#add_material_bolt_grade) | **POST** /api/1/projects/{projectId}/materials/bolt-grade | Add material to the project
[**add_material_concrete**](MaterialApi.md#add_material_concrete) | **POST** /api/1/projects/{projectId}/materials/concrete | Add material to the project
[**add_material_steel**](MaterialApi.md#add_material_steel) | **POST** /api/1/projects/{projectId}/materials/steel | Add material to the project
[**add_material_weld**](MaterialApi.md#add_material_weld) | **POST** /api/1/projects/{projectId}/materials/welding | Add material to the project
[**get_all_materials**](MaterialApi.md#get_all_materials) | **GET** /api/1/projects/{projectId}/materials | Get materials which are used in the project projectId
[**get_blot_grade_materials**](MaterialApi.md#get_blot_grade_materials) | **GET** /api/1/projects/{projectId}/materials/bolt-grade | Get materials which are used in the project projectId
[**get_bolt_assemblies**](MaterialApi.md#get_bolt_assemblies) | **GET** /api/1/projects/{projectId}/materials/bolt-assemblies | Get bolt assemblies which are used in the project projectId
[**get_concrete_materials**](MaterialApi.md#get_concrete_materials) | **GET** /api/1/projects/{projectId}/materials/concrete | Get materials which are used in the project projectId
[**get_cross_sections**](MaterialApi.md#get_cross_sections) | **GET** /api/1/projects/{projectId}/materials/cross-sections | Get cross sections which are used in the project projectId
[**get_steel_materials**](MaterialApi.md#get_steel_materials) | **GET** /api/1/projects/{projectId}/materials/steel | Get materials which are used in the project projectId
[**get_welding_materials**](MaterialApi.md#get_welding_materials) | **GET** /api/1/projects/{projectId}/materials/welding | Get materials which are used in the project projectId


# **add_bolt_assembly**
> ConMprlElement add_bolt_assembly(project_id, con_mprl_element=con_mprl_element)

Add bolt assembly to the project

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new bolt assemby to be added to the project (optional)

    try:
        # Add bolt assembly to the project
        api_response = api_instance.add_bolt_assembly(project_id, con_mprl_element=con_mprl_element)
        print("The response of MaterialApi->add_bolt_assembly:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->add_bolt_assembly: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new bolt assemby to be added to the project | [optional] 

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

# **add_cross_section**
> ConMprlCrossSection add_cross_section(project_id, con_mprl_cross_section=con_mprl_cross_section)

Add cross section to the project

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_cross_section import ConMprlCrossSection
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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_cross_section = ideastatica_connection_api.ConMprlCrossSection() # ConMprlCrossSection | Definition of a new cross-section to be added to the project (optional)

    try:
        # Add cross section to the project
        api_response = api_instance.add_cross_section(project_id, con_mprl_cross_section=con_mprl_cross_section)
        print("The response of MaterialApi->add_cross_section:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->add_cross_section: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_cross_section** | [**ConMprlCrossSection**](ConMprlCrossSection.md)| Definition of a new cross-section to be added to the project | [optional] 

### Return type

[**ConMprlCrossSection**](ConMprlCrossSection.md)

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

# **add_material_bolt_grade**
> ConMprlElement add_material_bolt_grade(project_id, con_mprl_element=con_mprl_element)

Add material to the project

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new material to be added to the project (optional)

    try:
        # Add material to the project
        api_response = api_instance.add_material_bolt_grade(project_id, con_mprl_element=con_mprl_element)
        print("The response of MaterialApi->add_material_bolt_grade:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->add_material_bolt_grade: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new material to be added to the project | [optional] 

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

# **add_material_concrete**
> ConMprlElement add_material_concrete(project_id, con_mprl_element=con_mprl_element)

Add material to the project

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new material to be added to the project (optional)

    try:
        # Add material to the project
        api_response = api_instance.add_material_concrete(project_id, con_mprl_element=con_mprl_element)
        print("The response of MaterialApi->add_material_concrete:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->add_material_concrete: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new material to be added to the project | [optional] 

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

# **add_material_steel**
> ConMprlElement add_material_steel(project_id, con_mprl_element=con_mprl_element)

Add material to the project

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new material to be added to the project (optional)

    try:
        # Add material to the project
        api_response = api_instance.add_material_steel(project_id, con_mprl_element=con_mprl_element)
        print("The response of MaterialApi->add_material_steel:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->add_material_steel: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new material to be added to the project | [optional] 

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

# **add_material_weld**
> ConMprlElement add_material_weld(project_id, con_mprl_element=con_mprl_element)

Add material to the project

### Example


```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new material to be added to the project (optional)

    try:
        # Add material to the project
        api_response = api_instance.add_material_weld(project_id, con_mprl_element=con_mprl_element)
        print("The response of MaterialApi->add_material_weld:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->add_material_weld: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new material to be added to the project | [optional] 

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

# **get_all_materials**
> List[object] get_all_materials(project_id)

Get materials which are used in the project projectId

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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_instance.get_all_materials(project_id)
        print("The response of MaterialApi->get_all_materials:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_all_materials: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

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

# **get_blot_grade_materials**
> List[object] get_blot_grade_materials(project_id)

Get materials which are used in the project projectId

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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_instance.get_blot_grade_materials(project_id)
        print("The response of MaterialApi->get_blot_grade_materials:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_blot_grade_materials: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

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

# **get_bolt_assemblies**
> List[object] get_bolt_assemblies(project_id)

Get bolt assemblies which are used in the project projectId

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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get bolt assemblies which are used in the project projectId
        api_response = api_instance.get_bolt_assemblies(project_id)
        print("The response of MaterialApi->get_bolt_assemblies:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_bolt_assemblies: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

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

# **get_concrete_materials**
> List[object] get_concrete_materials(project_id)

Get materials which are used in the project projectId

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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_instance.get_concrete_materials(project_id)
        print("The response of MaterialApi->get_concrete_materials:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_concrete_materials: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

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

# **get_cross_sections**
> List[object] get_cross_sections(project_id)

Get cross sections which are used in the project projectId

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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get cross sections which are used in the project projectId
        api_response = api_instance.get_cross_sections(project_id)
        print("The response of MaterialApi->get_cross_sections:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_cross_sections: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

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

# **get_steel_materials**
> List[object] get_steel_materials(project_id)

Get materials which are used in the project projectId

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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_instance.get_steel_materials(project_id)
        print("The response of MaterialApi->get_steel_materials:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_steel_materials: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

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

# **get_welding_materials**
> List[object] get_welding_materials(project_id)

Get materials which are used in the project projectId

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
    api_instance = ideastatica_connection_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_instance.get_welding_materials(project_id)
        print("The response of MaterialApi->get_welding_materials:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_welding_materials: %s\n" % e)
```



### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

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

