# ProjectApi

All URIs are relative to *http://localhost*

Method | Description
------------- | -------------
[**close_project**](ProjectApi.md#close_project) | Close the project. Needed for releasing resources in the service.
[**download_project**](ProjectApi.md#download_project) | Download the actual ideacon project from the service. It includes alle changes which were made by previous API calls.
[**get_active_projects**](ProjectApi.md#get_active_projects) | Get the list of projects in the service which were opened by the client which was connected by M:IdeaStatiCa.ConnectionRestApi.Controllers.ClientController.ConnectClient
[**get_project_data**](ProjectApi.md#get_project_data) | Get data of the project.
[**import_iom**](ProjectApi.md#import_iom) | Create the IDEA Connection project from IOM provided in xml format.  The parameter &#39;containerXmlFile&#39; passed in HTTP body represents :  [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  which is serialized to XML string by  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs)
[**open_project**](ProjectApi.md#open_project) | Open ideacon project from ideaConFile
[**update_from_iom**](ProjectApi.md#update_from_iom) | Update the IDEA Connection project by [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  (model and results).  IOM is passed in the body of the request as the xml string.  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs) should be used to generate the valid xml string
[**update_project_data**](ProjectApi.md#update_project_data) | Updates ConProjectData of project


<a id="close_project"></a>
# **close_project**
> str close_project(project_id)

Close the project. Needed for releasing resources in the service.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the project to be closed | 

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
def close_projectExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the project to be closed

    try:
        # Close the project. Needed for releasing resources in the service.
        api_response = api_client.project.close_project(project_id)
        print("The response of ProjectApi->close_project:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ProjectApi->close_project: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/close 

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

<a id="download_project"></a>
# **download_project**
> download_project(project_id)

Download the actual ideacon project from the service. It includes alle changes which were made by previous API calls.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service | 

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
def download_projectExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service

    try:
        # Download the actual ideacon project from the service. It includes alle changes which were made by previous API calls.
        api_client.project.download_project(project_id)
    except Exception as e:
        print("Exception when calling ProjectApi->download_project: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId}/download 

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

<a id="get_active_projects"></a>
# **get_active_projects**
> List[ConProject] get_active_projects()

Get the list of projects in the service which were opened by the client which was connected by M:IdeaStatiCa.ConnectionRestApi.Controllers.ClientController.ConnectClient

### Parameters

This endpoint does not need any parameter.

### Return type

[**List[ConProject]**](ConProject.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_project import ConProject
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_active_projectsExampleFunc(api_client):
    

    try:
        # Get the list of projects in the service which were opened by the client which was connected by M:IdeaStatiCa.ConnectionRestApi.Controllers.ClientController.ConnectClient
        api_response = api_client.project.get_active_projects()
        print("The response of ProjectApi->get_active_projects:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ProjectApi->get_active_projects: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects 

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

<a id="get_project_data"></a>
# **get_project_data**
> ConProject get_project_data(project_id)

Get data of the project.

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the requested project | 

### Return type

[**ConProject**](ConProject.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_project import ConProject
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def get_project_dataExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the requested project

    try:
        # Get data of the project.
        api_response = api_client.project.get_project_data(project_id)
        print("The response of ProjectApi->get_project_data:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ProjectApi->get_project_data: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/2/projects/{projectId} 

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

<a id="import_iom"></a>
# **import_iom**
> ConProject import_iom(container_xml_file=container_xml_file, connections_to_create=connections_to_create)

Create the IDEA Connection project from IOM provided in xml format.  The parameter 'containerXmlFile' passed in HTTP body represents :  [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  which is serialized to XML string by  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs)

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **container_xml_file** | **bytearray**| IdeaRS.OpenModel.OpenModelContainer serialized to xml | [optional] 
 **connections_to_create** | [**List[int]**](int.md)|  | [optional] 

### Return type

[**ConProject**](ConProject.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_project import ConProject
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def import_iomExampleFunc(api_client):
    
    container_xml_file = None # bytearray | IdeaRS.OpenModel.OpenModelContainer serialized to xml (optional)
    connections_to_create = [56] # List[int] |  (optional)

    try:
        # Create the IDEA Connection project from IOM provided in xml format.  The parameter 'containerXmlFile' passed in HTTP body represents :  [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  which is serialized to XML string by  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs)
        api_response = api_client.project.import_iom(container_xml_file=container_xml_file, connections_to_create=connections_to_create)
        print("The response of ProjectApi->import_iom:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ProjectApi->import_iom: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/import-iom-file 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: multipart/form-data
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="open_project"></a>
# **open_project**
> ConProject open_project(idea_con_file=idea_con_file)

Open ideacon project from ideaConFile

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **idea_con_file** | **bytearray**| Ideacon file | [optional] 

### Return type

[**ConProject**](ConProject.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_project import ConProject
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def open_projectExampleFunc(api_client):
    
    idea_con_file = None # bytearray | Ideacon file (optional)

    try:
        # Open ideacon project from ideaConFile
        api_response = api_client.project.open_project(idea_con_file=idea_con_file)
        print("The response of ProjectApi->open_project:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ProjectApi->open_project: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/open 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: multipart/form-data
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_from_iom"></a>
# **update_from_iom**
> ConProject update_from_iom(project_id, container_xml_file=container_xml_file)

Update the IDEA Connection project by [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  (model and results).  IOM is passed in the body of the request as the xml string.  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs) should be used to generate the valid xml string

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**| The unique identifier of the opened project in the ConnectionRestApi service to be updated | 
 **container_xml_file** | **bytearray**| IdeaRS.OpenModel.OpenModelContainer serialized to xml | [optional] 

### Return type

[**ConProject**](ConProject.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_project import ConProject
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_from_iomExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | The unique identifier of the opened project in the ConnectionRestApi service to be updated
    container_xml_file = None # bytearray | IdeaRS.OpenModel.OpenModelContainer serialized to xml (optional)

    try:
        # Update the IDEA Connection project by [IdeaRS.OpenModel.OpenModelContainer](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs)  (model and results).  IOM is passed in the body of the request as the xml string.  [IdeaRS.OpenModel.Tools.OpenModelContainerToXml](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Tools.cs) should be used to generate the valid xml string
        api_response = api_client.project.update_from_iom(project_id, container_xml_file=container_xml_file)
        print("The response of ProjectApi->update_from_iom:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ProjectApi->update_from_iom: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/2/projects/{projectId}/update-iom-file 

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: multipart/form-data
 - **Accept**: application/json

### HTTP response details

| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update_project_data"></a>
# **update_project_data**
> ConProject update_project_data(project_id, con_project_data=con_project_data)

Updates ConProjectData of project

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **project_id** | **str**|  | 
 **con_project_data** | [**ConProjectData**](ConProjectData.md)|  | [optional] 

### Return type

[**ConProject**](ConProject.md)

### Example

Required Imports
```python
import ideastatica_connection_api
from ideastatica_connection_api.models.con_project import ConProject
from ideastatica_connection_api.models.con_project_data import ConProjectData
from ideastatica_connection_api.rest import ApiException
from pprint import pprint

```

For client instantiation instructions, refer to the [[README]](../README.md) documentation. 

```python
def update_project_dataExampleFunc(api_client):
    
    project_id = 'project_id_example' # str | 
    con_project_data = ideastatica_connection_api.ConProjectData() # ConProjectData |  (optional)

    try:
        # Updates ConProjectData of project
        api_response = api_client.project.update_project_data(project_id, con_project_data=con_project_data)
        print("The response of ProjectApi->update_project_data:\n")
        pprint(api_response)
        return api_response
    except Exception as e:
        print("Exception when calling ProjectApi->update_project_data: %s\n" % e)
```



### Code Samples

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/2/projects/{projectId} 

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

