# ConnectionLibraryApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**get_design_item_picture**](ConnectionLibraryApi.md#get_design_item_picture) | Retrieves the picture associated with the specified design item as a PNG image.
[**get_design_sets**](ConnectionLibraryApi.md#get_design_sets) | Retrieves a list of design sets available for the user.
[**get_template**](ConnectionLibraryApi.md#get_template) | Retrieves the template associated with the specified design set and design item.
[**propose**](ConnectionLibraryApi.md#propose) | Proposes a list of design items for a specified connection within a project.


<a id="get_design_item_picture"></a>
# **get_design_item_picture**
> get_design_item_picture(design_item_id=design_item_id)

Retrieves the picture associated with the specified design item as a PNG image.

This method is mapped to API version 2 and produces a PNG image. The image is returned as a file              stream result with the file name set to the design item's ID.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **design_item_id** | **str**| The unique identifier of the design item whose picture is to be retrieved. | [optional] 

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
def get_design_item_pictureExampleFunc(api_client):
    
    design_item_id = 'design_item_id_example' # str | The unique identifier of the design item whose picture is to be retrieved. (optional)

    try:
        # Retrieves the picture associated with the specified design item as a PNG image.
        api_client.connectionlibrary.get_design_item_picture(design_item_id=design_item_id)
    except Exception as e:
        print("Exception when calling ConnectionLibraryApi->get_design_item_picture: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/connection-library/get-picture 

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

<a id="get_design_sets"></a>
# **get_design_sets**
> List[ConDesignSet] get_design_sets()

Retrieves a list of design sets available for the user.

This method returns a collection of design sets that are mapped and ready for use. It throws an              exception if no design sets are available for the user.

### Parameters

This endpoint does not need any parameter.

### Return type

[**List[ConDesignSet]**](ConDesignSet.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_design_set import ConDesignSet
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_design_setsExampleFunc(api_client):
    

    try:
        # Retrieves a list of design sets available for the user.
        api_response = api_client.connectionlibrary.get_design_sets()
        print("The response of ConnectionLibraryApi->get_design_sets:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionLibraryApi->get_design_sets: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/connection-library/get-design-sets 

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

<a id="get_template"></a>
# **get_template**
> str get_template(design_set_id=design_set_id, design_item_id=design_item_id)

Retrieves the template associated with the specified design set and design item.

This method is mapped to API version 2 and produces a plain text response. It is intended to be              used in scenarios where the template of a design item needs to be retrieved for further processing or              display.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **design_set_id** | **str**| The unique identifier of the design set. | [optional] 
 **design_item_id** | **str**| The unique identifier of the design item for which the template is requested. | [optional] 

### Return type

**str**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_templateExampleFunc(api_client):
    
    design_set_id = 'design_set_id_example' # str | The unique identifier of the design set. (optional)
    design_item_id = 'design_item_id_example' # str | The unique identifier of the design item for which the template is requested. (optional)

    try:
        # Retrieves the template associated with the specified design set and design item.
        api_response = api_client.connectionlibrary.get_template(design_set_id=design_set_id, design_item_id=design_item_id)
        print("The response of ConnectionLibraryApi->get_template:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionLibraryApi->get_template: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/connection-library/get-template 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="propose"></a>
# **propose**
> List[ConDesignItem] propose(project_id, connection_id, con_connection_library_search_parameters=con_connection_library_search_parameters)

Proposes a list of design items for a specified connection within a project.

This method retrieves the connection model from the specified project and classifies its              typology. It then filters and proposes design items based on the connection's typology and design code.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the project containing the connection. | 
 **connection_id** | **int**| The identifier of the connection for which design items are proposed. | 
 **con_connection_library_search_parameters** | [**ConConnectionLibrarySearchParameters**](ConConnectionLibrarySearchParameters.md)| Parameters used to filter and refine the search for proposed connection design items, such as set membership and required connection features. | [optional] 

### Return type

[**List[ConDesignItem]**](ConDesignItem.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection_library_search_parameters import ConConnectionLibrarySearchParameters
from ideastatica_connection_api.models.con_design_item import ConDesignItem
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def proposeExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the project containing the connection.
    connection_id = 56 # int | The identifier of the connection for which design items are proposed.
    con_connection_library_search_parameters = ideastatica_connection_api.ConConnectionLibrarySearchParameters() # ConConnectionLibrarySearchParameters | Parameters used to filter and refine the search for proposed connection design items, such as set membership and required connection features. (optional)

    try:
        # Proposes a list of design items for a specified connection within a project.
        api_response = api_client.connectionlibrary.propose(project_id, connection_id, con_connection_library_search_parameters=con_connection_library_search_parameters)
        print("The response of ConnectionLibraryApi->propose:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ConnectionLibraryApi->propose: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/connection-library/projects/{projectId}/connections/{connectionId}/propose 

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

