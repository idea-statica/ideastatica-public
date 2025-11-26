# TemplateApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**apply_template**](TemplateApi.md#apply_template) | Apply the connection template applyTemplateParam on the connection connectionId in the project projectId
[**clear_design**](TemplateApi.md#clear_design) | Clears the entire design of the specified connection, including all operations  and parameters, in the given project. This reset is performed regardless of  whether the design originated from a template or was created manually.
[**create_con_template**](TemplateApi.md#create_con_template) | Create a template for the connection connectionId in the project projectId
[**delete**](TemplateApi.md#delete) | Delete specific template
[**delete_all**](TemplateApi.md#delete_all) | Delete all templates in connection
[**explode**](TemplateApi.md#explode) | Explode specific template (delete parameters, keep operations)
[**explode_all**](TemplateApi.md#explode_all) | Explode all templates (delete parameters, keep operations)
[**get_connection_topology**](TemplateApi.md#get_connection_topology) | Get topology of the connection in json format
[**get_default_template_mapping**](TemplateApi.md#get_default_template_mapping) | Get the default mappings for the application of the connection template passed in templateToApply  on connectionId in the project projectId
[**get_template_common_operation_properties**](TemplateApi.md#get_template_common_operation_properties) | Get Common properties for specific template
[**get_templates_for_connection**](TemplateApi.md#get_templates_for_connection) | Retrieves a list of templates associated with a specific connection within a project.
[**load_defaults**](TemplateApi.md#load_defaults) | Load parameter defaults for specific template.
[**publish_connection**](TemplateApi.md#publish_connection) | Publish template to Private or Company set
[**update_template_common_operation_properties**](TemplateApi.md#update_template_common_operation_properties) | Set common properties for specific template


<a id="apply_template"></a>
# **apply_template**
> ConTemplateApplyResult apply_template(project_id, connection_id, con_template_apply_param=con_template_apply_param)

Apply the connection template applyTemplateParam on the connection connectionId in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection where to apply the template | 
 **con_template_apply_param** | [**ConTemplateApplyParam**](ConTemplateApplyParam.md)| Template to apply | [optional] 

### Return type

[**ConTemplateApplyResult**](ConTemplateApplyResult.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_template_apply_param import ConTemplateApplyParam
from ideastatica_connection_api.models.con_template_apply_result import ConTemplateApplyResult
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def apply_templateExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection where to apply the template
    con_template_apply_param = ideastatica_connection_api.ConTemplateApplyParam() # ConTemplateApplyParam | Template to apply (optional)

    try:
        # Apply the connection template applyTemplateParam on the connection connectionId in the project projectId
        api_response = api_client.template.apply_template(project_id, connection_id, con_template_apply_param=con_template_apply_param)
        print("The response of TemplateApi->apply_template:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling TemplateApi->apply_template: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/apply-template 

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

<a id="clear_design"></a>
# **clear_design**
> clear_design(project_id, connection_id)

Clears the entire design of the specified connection, including all operations  and parameters, in the given project. This reset is performed regardless of  whether the design originated from a template or was created manually.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection where to clear the design | 

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
def clear_designExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection where to clear the design

    try:
        # Clears the entire design of the specified connection, including all operations  and parameters, in the given project. This reset is performed regardless of  whether the design originated from a template or was created manually.
        api_client.template.clear_design(project_id, connection_id)
    except Exception as e:
        print("Exception when calling TemplateApi->clear_design: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/clear-design 

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

<a id="create_con_template"></a>
# **create_con_template**
> str create_con_template(project_id, connection_id)

Create a template for the connection connectionId in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to be converted to a template | 

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
def create_con_templateExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to be converted to a template

    try:
        # Create a template for the connection connectionId in the project projectId
        api_response = api_client.template.create_con_template(project_id, connection_id)
        print("The response of TemplateApi->create_con_template:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling TemplateApi->create_con_template: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/get-template 

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

<a id="delete"></a>
# **delete**
> delete(project_id, connection_id, template_id)

Delete specific template

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **template_id** | **str**|  | 

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
def deleteExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    template_id = 'template_id_example' # str | 

    try:
        # Delete specific template
        api_client.template.delete(project_id, connection_id, template_id)
    except Exception as e:
        print("Exception when calling TemplateApi->delete: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/2/projects/{projectId}/connections/{connectionId}/templates/{templateId} 

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

<a id="delete_all"></a>
# **delete_all**
> delete_all(project_id, connection_id)

Delete all templates in connection

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
def delete_allExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Delete all templates in connection
        api_client.template.delete_all(project_id, connection_id)
    except Exception as e:
        print("Exception when calling TemplateApi->delete_all: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/2/projects/{projectId}/connections/{connectionId}/templates 

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

<a id="explode"></a>
# **explode**
> explode(project_id, connection_id, template_id)

Explode specific template (delete parameters, keep operations)

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **template_id** | **str**|  | 

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
def explodeExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    template_id = 'template_id_example' # str | 

    try:
        # Explode specific template (delete parameters, keep operations)
        api_client.template.explode(project_id, connection_id, template_id)
    except Exception as e:
        print("Exception when calling TemplateApi->explode: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/templates/{templateId}/explode 

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

<a id="explode_all"></a>
# **explode_all**
> explode_all(project_id, connection_id)

Explode all templates (delete parameters, keep operations)

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
def explode_allExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 

    try:
        # Explode all templates (delete parameters, keep operations)
        api_client.template.explode_all(project_id, connection_id)
    except Exception as e:
        print("Exception when calling TemplateApi->explode_all: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/templates/explode 

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

<a id="get_connection_topology"></a>
# **get_connection_topology**
> str get_connection_topology(project_id, connection_id)

Get topology of the connection in json format

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection where to clear the design | 

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
def get_connection_topologyExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection where to clear the design

    try:
        # Get topology of the connection in json format
        api_response = api_client.template.get_connection_topology(project_id, connection_id)
        print("The response of TemplateApi->get_connection_topology:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling TemplateApi->get_connection_topology: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/get-topology 

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

<a id="get_default_template_mapping"></a>
# **get_default_template_mapping**
> TemplateConversions get_default_template_mapping(project_id, connection_id, con_template_mapping_get_param=con_template_mapping_get_param)

Get the default mappings for the application of the connection template passed in templateToApply  on connectionId in the project projectId

The result IdeaStatiCa.Api.Connection.Model.TemplateConversionsDefault mapping to apply the passed template.  It can be modified by a user and used for the application of a template M:IdeaStatiCa.ConnectionRestApi.Controllers.TemplateController.ApplyConnectionTemplateAsync(System.Guid,System.Int32,IdeaStatiCa.Api.Connection.Model.ConTemplateApplyParam) method.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection to get default mapping | 
 **con_template_mapping_get_param** | [**ConTemplateMappingGetParam**](ConTemplateMappingGetParam.md)| Data of the template to get default mapping | [optional] 

### Return type

[**TemplateConversions**](TemplateConversions.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_template_mapping_get_param import ConTemplateMappingGetParam
from ideastatica_connection_api.models.template_conversions import TemplateConversions
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_default_template_mappingExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service
    connection_id = 56 # int | Id of the connection to get default mapping
    con_template_mapping_get_param = ideastatica_connection_api.ConTemplateMappingGetParam() # ConTemplateMappingGetParam | Data of the template to get default mapping (optional)

    try:
        # Get the default mappings for the application of the connection template passed in templateToApply  on connectionId in the project projectId
        api_response = api_client.template.get_default_template_mapping(project_id, connection_id, con_template_mapping_get_param=con_template_mapping_get_param)
        print("The response of TemplateApi->get_default_template_mapping:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling TemplateApi->get_default_template_mapping: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/get-default-mapping 

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

<a id="get_template_common_operation_properties"></a>
# **get_template_common_operation_properties**
> ConOperationCommonProperties get_template_common_operation_properties(project_id, connection_id, template_id)

Get Common properties for specific template

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **template_id** | **str**|  | 

### Return type

[**ConOperationCommonProperties**](ConOperationCommonProperties.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_operation_common_properties import ConOperationCommonProperties
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_template_common_operation_propertiesExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    template_id = 'template_id_example' # str | 

    try:
        # Get Common properties for specific template
        api_response = api_client.template.get_template_common_operation_properties(project_id, connection_id, template_id)
        print("The response of TemplateApi->get_template_common_operation_properties:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling TemplateApi->get_template_common_operation_properties: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/templates/{templateId}/common-properties 

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

<a id="get_templates_for_connection"></a>
# **get_templates_for_connection**
> List[ConConnectionTemplateModel] get_templates_for_connection(project_id, connection_id)

Retrieves a list of templates associated with a specific connection within a project.

This method fetches the templates applied to a connection within a project. Each template              includes details such as its ID within the project, template id, members, operations, parameters, and associated common properties.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the project containing the connection. | 
 **connection_id** | **int**| The identifier of the connection for which templates are to be retrieved. | 

### Return type

[**List[ConConnectionTemplateModel]**](ConConnectionTemplateModel.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_connection_template_model import ConConnectionTemplateModel
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_templates_for_connectionExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the project containing the connection.
    connection_id = 56 # int | The identifier of the connection for which templates are to be retrieved.

    try:
        # Retrieves a list of templates associated with a specific connection within a project.
        api_response = api_client.template.get_templates_for_connection(project_id, connection_id)
        print("The response of TemplateApi->get_templates_for_connection:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling TemplateApi->get_templates_for_connection: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/connections/{connectionId}/templates 

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

<a id="load_defaults"></a>
# **load_defaults**
> ParameterUpdateResponse load_defaults(project_id, connection_id, template_id)

Load parameter defaults for specific template.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **template_id** | **str**|  | 

### Return type

[**ParameterUpdateResponse**](ParameterUpdateResponse.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.parameter_update_response import ParameterUpdateResponse
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def load_defaultsExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    template_id = 'template_id_example' # str | 

    try:
        # Load parameter defaults for specific template.
        api_response = api_client.template.load_defaults(project_id, connection_id, template_id)
        print("The response of TemplateApi->load_defaults:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling TemplateApi->load_defaults: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/templates/{templateId}/load-defaults 

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

<a id="publish_connection"></a>
# **publish_connection**
> bool publish_connection(project_id, connection_id, con_template_publish_param=con_template_publish_param)

Publish template to Private or Company set

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **con_template_publish_param** | [**ConTemplatePublishParam**](ConTemplatePublishParam.md)|  | [optional] 

### Return type

**bool**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_template_publish_param import ConTemplatePublishParam
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def publish_connectionExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    con_template_publish_param = ideastatica_connection_api.ConTemplatePublishParam() # ConTemplatePublishParam |  (optional)

    try:
        # Publish template to Private or Company set
        api_response = api_client.template.publish_connection(project_id, connection_id, con_template_publish_param=con_template_publish_param)
        print("The response of TemplateApi->publish_connection:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling TemplateApi->publish_connection: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/connections/{connectionId}/publish 

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

<a id="update_template_common_operation_properties"></a>
# **update_template_common_operation_properties**
> update_template_common_operation_properties(project_id, connection_id, template_id, con_operation_common_properties=con_operation_common_properties)

Set common properties for specific template

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **connection_id** | **int**|  | 
 **template_id** | **str**|  | 
 **con_operation_common_properties** | [**ConOperationCommonProperties**](ConOperationCommonProperties.md)|  | [optional] 

### Return type

void (empty response body)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_operation_common_properties import ConOperationCommonProperties
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_template_common_operation_propertiesExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    connection_id = 56 # int | 
    template_id = 'template_id_example' # str | 
    con_operation_common_properties = ideastatica_connection_api.ConOperationCommonProperties() # ConOperationCommonProperties |  (optional)

    try:
        # Set common properties for specific template
        api_client.template.update_template_common_operation_properties(project_id, connection_id, template_id, con_operation_common_properties=con_operation_common_properties)
    except Exception as e:
        print("Exception when calling TemplateApi->update_template_common_operation_properties: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/2/projects/{projectId}/connections/{connectionId}/templates/{templateId}/common-properties 

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

