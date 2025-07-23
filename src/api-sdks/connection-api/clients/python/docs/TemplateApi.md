# TemplateApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**apply_template**](TemplateApi.md#apply_template) | Apply the connection template applyTemplateParam on the connection connectionId in the project projectId
[**clear_design**](TemplateApi.md#clear_design) | Clear the design of the connection connectionId in the project projectId
[**create_con_template**](TemplateApi.md#create_con_template) | Create a template for the connection connectionId in the project projectId
[**get_connection_topology**](TemplateApi.md#get_connection_topology) | Get topology of the connection in json format
[**get_default_template_mapping**](TemplateApi.md#get_default_template_mapping) | Get the default mappings for the application of the connection template passed in templateToApply  on connectionId in the project projectId


<a id="apply_template"></a>
# **apply_template**
> object apply_template(project_id, connection_id, con_template_apply_param=con_template_apply_param)

Apply the connection template applyTemplateParam on the connection connectionId in the project projectId

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 
 **connection_id** | **int**| Id of the connection where to apply the template | 
 **con_template_apply_param** | [**ConTemplateApplyParam**](ConTemplateApplyParam.md)| Template to apply | [optional] 

### Return type

**object**

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_template_apply_param import ConTemplateApplyParam
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
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="clear_design"></a>
# **clear_design**
> clear_design(project_id, connection_id)

Clear the design of the connection connectionId in the project projectId

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
        # Clear the design of the connection connectionId in the project projectId
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
**200** | Success |  -  |

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
**200** | Success |  -  |

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
**200** | Success |  -  |

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
**200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

