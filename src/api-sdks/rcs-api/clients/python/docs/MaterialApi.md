# MaterialApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**add_concrete_material**](MaterialApi.md#add_concrete_material) | Add a new material to the project. The material type is determined by the endpoint path.
[**add_prestress_material**](MaterialApi.md#add_prestress_material) | Add a new material to the project. The material type is determined by the endpoint path.
[**add_reinforcement_material**](MaterialApi.md#add_reinforcement_material) | Add a new material to the project. The material type is determined by the endpoint path.
[**get_all_materials**](MaterialApi.md#get_all_materials) | Get materials from the project. Use specific path to filter by material type.
[**get_concrete_materials**](MaterialApi.md#get_concrete_materials) | Get materials from the project. Use specific path to filter by material type.
[**get_prestress_materials**](MaterialApi.md#get_prestress_materials) | Get materials from the project. Use specific path to filter by material type.
[**get_reinforcement_materials**](MaterialApi.md#get_reinforcement_materials) | Get materials from the project. Use specific path to filter by material type.


<a id="add_concrete_material"></a>
# **add_concrete_material**
> add_concrete_material(project_id, rcs_mprl_element=rcs_mprl_element)

Add a new material to the project. The material type is determined by the endpoint path.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Project ID | 
 **rcs_mprl_element** | [**RcsMprlElement**](RcsMprlElement.md)| Material data to add (MPRL name) | [optional] 

### Return type

void (empty response body)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_mprl_element import RcsMprlElement
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
    api_instance = ideastatica_rcs_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | Project ID
    rcs_mprl_element = ideastatica_rcs_api.RcsMprlElement() # RcsMprlElement | Material data to add (MPRL name) (optional)

    try:
        # Add a new material to the project. The material type is determined by the endpoint path.
        api_instance.add_concrete_material(project_id, rcs_mprl_element=rcs_mprl_element)
    except Exception as e:
        print("Exception when calling MaterialApi->add_concrete_material: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/materials/concrete 

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

<a id="add_prestress_material"></a>
# **add_prestress_material**
> add_prestress_material(project_id, rcs_mprl_element=rcs_mprl_element)

Add a new material to the project. The material type is determined by the endpoint path.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Project ID | 
 **rcs_mprl_element** | [**RcsMprlElement**](RcsMprlElement.md)| Material data to add (MPRL name) | [optional] 

### Return type

void (empty response body)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_mprl_element import RcsMprlElement
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
    api_instance = ideastatica_rcs_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | Project ID
    rcs_mprl_element = ideastatica_rcs_api.RcsMprlElement() # RcsMprlElement | Material data to add (MPRL name) (optional)

    try:
        # Add a new material to the project. The material type is determined by the endpoint path.
        api_instance.add_prestress_material(project_id, rcs_mprl_element=rcs_mprl_element)
    except Exception as e:
        print("Exception when calling MaterialApi->add_prestress_material: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/materials/prestress 

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

<a id="add_reinforcement_material"></a>
# **add_reinforcement_material**
> add_reinforcement_material(project_id, rcs_mprl_element=rcs_mprl_element)

Add a new material to the project. The material type is determined by the endpoint path.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Project ID | 
 **rcs_mprl_element** | [**RcsMprlElement**](RcsMprlElement.md)| Material data to add (MPRL name) | [optional] 

### Return type

void (empty response body)

### Example


```python
import ideastatica_rcs_api
from ideastatica_rcs_api.models.rcs_mprl_element import RcsMprlElement
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
    api_instance = ideastatica_rcs_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | Project ID
    rcs_mprl_element = ideastatica_rcs_api.RcsMprlElement() # RcsMprlElement | Material data to add (MPRL name) (optional)

    try:
        # Add a new material to the project. The material type is determined by the endpoint path.
        api_instance.add_reinforcement_material(project_id, rcs_mprl_element=rcs_mprl_element)
    except Exception as e:
        print("Exception when calling MaterialApi->add_reinforcement_material: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/materials/reinforcement 

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

<a id="get_all_materials"></a>
# **get_all_materials**
> List[object] get_all_materials(project_id)

Get materials from the project. Use specific path to filter by material type.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Project ID | 

### Return type

**List[object]**

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
    api_instance = ideastatica_rcs_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | Project ID

    try:
        # Get materials from the project. Use specific path to filter by material type.
        api_response = api_instance.get_all_materials(project_id)
        print("The response of MaterialApi->get_all_materials:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_all_materials: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/materials 

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

<a id="get_concrete_materials"></a>
# **get_concrete_materials**
> List[object] get_concrete_materials(project_id)

Get materials from the project. Use specific path to filter by material type.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Project ID | 

### Return type

**List[object]**

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
    api_instance = ideastatica_rcs_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | Project ID

    try:
        # Get materials from the project. Use specific path to filter by material type.
        api_response = api_instance.get_concrete_materials(project_id)
        print("The response of MaterialApi->get_concrete_materials:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_concrete_materials: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/materials/concrete 

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

<a id="get_prestress_materials"></a>
# **get_prestress_materials**
> List[object] get_prestress_materials(project_id)

Get materials from the project. Use specific path to filter by material type.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Project ID | 

### Return type

**List[object]**

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
    api_instance = ideastatica_rcs_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | Project ID

    try:
        # Get materials from the project. Use specific path to filter by material type.
        api_response = api_instance.get_prestress_materials(project_id)
        print("The response of MaterialApi->get_prestress_materials:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_prestress_materials: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/materials/prestress 

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

<a id="get_reinforcement_materials"></a>
# **get_reinforcement_materials**
> List[object] get_reinforcement_materials(project_id)

Get materials from the project. Use specific path to filter by material type.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| Project ID | 

### Return type

**List[object]**

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
    api_instance = ideastatica_rcs_api.MaterialApi(api_client)
    project_id = 'project_id_example' # str | Project ID

    try:
        # Get materials from the project. Use specific path to filter by material type.
        api_response = api_instance.get_reinforcement_materials(project_id)
        print("The response of MaterialApi->get_reinforcement_materials:\n")
        pprint(api_response)
    except Exception as e:
        print("Exception when calling MaterialApi->get_reinforcement_materials: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/1/projects/{projectId}/materials/reinforcement 

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

