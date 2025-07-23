# MaterialApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**add_bolt_assembly**](MaterialApi.md#add_bolt_assembly) | Add bolt assembly to the project
[**add_cross_section**](MaterialApi.md#add_cross_section) | Add cross section to the project
[**add_material_bolt_grade**](MaterialApi.md#add_material_bolt_grade) | Add material to the project
[**add_material_concrete**](MaterialApi.md#add_material_concrete) | Add material to the project
[**add_material_steel**](MaterialApi.md#add_material_steel) | Add material to the project
[**add_material_weld**](MaterialApi.md#add_material_weld) | Add material to the project
[**get_all_materials**](MaterialApi.md#get_all_materials) | Get materials which are used in the project projectId
[**get_bolt_assemblies**](MaterialApi.md#get_bolt_assemblies) | Get bolt assemblies which are used in the project projectId
[**get_bolt_grade_materials**](MaterialApi.md#get_bolt_grade_materials) | Get materials which are used in the project projectId
[**get_concrete_materials**](MaterialApi.md#get_concrete_materials) | Get materials which are used in the project projectId
[**get_cross_sections**](MaterialApi.md#get_cross_sections) | Get cross sections which are used in the project projectId
[**get_steel_materials**](MaterialApi.md#get_steel_materials) | Get materials which are used in the project projectId
[**get_welding_materials**](MaterialApi.md#get_welding_materials) | Get materials which are used in the project projectId


<a id="add_bolt_assembly"></a>
# **add_bolt_assembly**
> add_bolt_assembly(project_id, con_mprl_element=con_mprl_element)

Add bolt assembly to the project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new bolt assemby to be added to the project | [optional] 

### Return type

void (empty response body)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_bolt_assemblyExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new bolt assemby to be added to the project (optional)

    try:
        # Add bolt assembly to the project
        api_client.material.add_bolt_assembly(project_id, con_mprl_element=con_mprl_element)
    except Exception as e:
        print("Exception when calling MaterialApi->add_bolt_assembly: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/materials/bolt-assemblies 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_cross_section"></a>
# **add_cross_section**
> add_cross_section(project_id, con_mprl_cross_section=con_mprl_cross_section)

Add cross section to the project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_cross_section** | [**ConMprlCrossSection**](ConMprlCrossSection.md)| Definition of a new cross-section to be added to the project | [optional] 

### Return type

void (empty response body)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_cross_section import ConMprlCrossSection
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_cross_sectionExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_cross_section = ideastatica_connection_api.ConMprlCrossSection() # ConMprlCrossSection | Definition of a new cross-section to be added to the project (optional)

    try:
        # Add cross section to the project
        api_client.material.add_cross_section(project_id, con_mprl_cross_section=con_mprl_cross_section)
    except Exception as e:
        print("Exception when calling MaterialApi->add_cross_section: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/materials/cross-sections 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_material_bolt_grade"></a>
# **add_material_bolt_grade**
> add_material_bolt_grade(project_id, con_mprl_element=con_mprl_element)

Add material to the project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new material to be added to the project | [optional] 

### Return type

void (empty response body)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_material_bolt_gradeExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new material to be added to the project (optional)

    try:
        # Add material to the project
        api_client.material.add_material_bolt_grade(project_id, con_mprl_element=con_mprl_element)
    except Exception as e:
        print("Exception when calling MaterialApi->add_material_bolt_grade: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/materials/bolt-grade 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_material_concrete"></a>
# **add_material_concrete**
> add_material_concrete(project_id, con_mprl_element=con_mprl_element)

Add material to the project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new material to be added to the project | [optional] 

### Return type

void (empty response body)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_material_concreteExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new material to be added to the project (optional)

    try:
        # Add material to the project
        api_client.material.add_material_concrete(project_id, con_mprl_element=con_mprl_element)
    except Exception as e:
        print("Exception when calling MaterialApi->add_material_concrete: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/materials/concrete 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_material_steel"></a>
# **add_material_steel**
> add_material_steel(project_id, con_mprl_element=con_mprl_element)

Add material to the project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new material to be added to the project | [optional] 

### Return type

void (empty response body)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_material_steelExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new material to be added to the project (optional)

    try:
        # Add material to the project
        api_client.material.add_material_steel(project_id, con_mprl_element=con_mprl_element)
    except Exception as e:
        print("Exception when calling MaterialApi->add_material_steel: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/materials/steel 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="add_material_weld"></a>
# **add_material_weld**
> add_material_weld(project_id, con_mprl_element=con_mprl_element)

Add material to the project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **con_mprl_element** | [**ConMprlElement**](ConMprlElement.md)| Definition of a new material to be added to the project | [optional] 

### Return type

void (empty response body)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_mprl_element import ConMprlElement
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def add_material_weldExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    con_mprl_element = ideastatica_connection_api.ConMprlElement() # ConMprlElement | Definition of a new material to be added to the project (optional)

    try:
        # Add material to the project
        api_client.material.add_material_weld(project_id, con_mprl_element=con_mprl_element)
    except Exception as e:
        print("Exception when calling MaterialApi->add_material_weld: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/materials/welding 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="get_all_materials"></a>
# **get_all_materials**
> List[object] get_all_materials(project_id)

Get materials which are used in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_all_materialsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_client.material.get_all_materials(project_id)
        print("The response of MaterialApi->get_all_materials:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MaterialApi->get_all_materials: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/materials 

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

<a id="get_bolt_assemblies"></a>
# **get_bolt_assemblies**
> List[object] get_bolt_assemblies(project_id)

Get bolt assemblies which are used in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_bolt_assembliesExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get bolt assemblies which are used in the project projectId
        api_response = api_client.material.get_bolt_assemblies(project_id)
        print("The response of MaterialApi->get_bolt_assemblies:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MaterialApi->get_bolt_assemblies: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/materials/bolt-assemblies 

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

<a id="get_bolt_grade_materials"></a>
# **get_bolt_grade_materials**
> List[object] get_bolt_grade_materials(project_id)

Get materials which are used in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_bolt_grade_materialsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_client.material.get_bolt_grade_materials(project_id)
        print("The response of MaterialApi->get_bolt_grade_materials:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MaterialApi->get_bolt_grade_materials: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/materials/bolt-grade 

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

<a id="get_concrete_materials"></a>
# **get_concrete_materials**
> List[object] get_concrete_materials(project_id)

Get materials which are used in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_concrete_materialsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_client.material.get_concrete_materials(project_id)
        print("The response of MaterialApi->get_concrete_materials:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MaterialApi->get_concrete_materials: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/materials/concrete 

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

<a id="get_cross_sections"></a>
# **get_cross_sections**
> List[object] get_cross_sections(project_id)

Get cross sections which are used in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_cross_sectionsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get cross sections which are used in the project projectId
        api_response = api_client.material.get_cross_sections(project_id)
        print("The response of MaterialApi->get_cross_sections:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MaterialApi->get_cross_sections: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/materials/cross-sections 

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

<a id="get_steel_materials"></a>
# **get_steel_materials**
> List[object] get_steel_materials(project_id)

Get materials which are used in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_steel_materialsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_client.material.get_steel_materials(project_id)
        print("The response of MaterialApi->get_steel_materials:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MaterialApi->get_steel_materials: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/materials/steel 

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

<a id="get_welding_materials"></a>
# **get_welding_materials**
> List[object] get_welding_materials(project_id)

Get materials which are used in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

### Return type

**List[object]**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_welding_materialsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Get materials which are used in the project projectId
        api_response = api_client.material.get_welding_materials(project_id)
        print("The response of MaterialApi->get_welding_materials:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling MaterialApi->get_welding_materials: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/materials/welding 

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

