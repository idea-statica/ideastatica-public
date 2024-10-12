# IdeaStatiCa.ConnectionApi.Api.ExportApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ExportConnectionData**](ExportApi.md#exportconnectiondata) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/export-iom-connection-data | Get https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs for required connection |
| [**ExportConnectionIFC**](ExportApi.md#exportconnectionifc) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/export-ifc | Export connection to IFC format |
| [**ExportIomXml**](ExportApi.md#exportiomxml) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/export-iom | Export connection to XML which includes https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs |

<a id="exportconnectiondata"></a>
# **ExportConnectionData**
> ConnectionData ExportConnectionData (Guid projectId, int connectionId)

Get https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs for required connection

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class ExportConnectionDataExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ExportApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 

            try
            {
                // Get https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs for required connection
                ConnectionData result = apiInstance.ExportConnectionData(projectId, connectionId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ExportApi.ExportConnectionData: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ExportConnectionDataWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Connection/ConnectionData.cs for required connection
    ApiResponse<ConnectionData> response = apiInstance.ExportConnectionDataWithHttpInfo(projectId, connectionId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ExportApi.ExportConnectionDataWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** |  |  |
| **connectionId** | **int** |  |  |

### Return type

[**ConnectionData**](ConnectionData.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/xml


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="exportconnectionifc"></a>
# **ExportConnectionIFC**
> void ExportConnectionIFC (Guid projectId, int connectionId)

Export connection to IFC format

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class ExportConnectionIFCExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ExportApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 

            try
            {
                // Export connection to IFC format
                apiInstance.ExportConnectionIFC(projectId, connectionId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ExportApi.ExportConnectionIFC: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ExportConnectionIFCWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Export connection to IFC format
    apiInstance.ExportConnectionIFCWithHttpInfo(projectId, connectionId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ExportApi.ExportConnectionIFCWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** |  |  |
| **connectionId** | **int** |  |  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="exportiomxml"></a>
# **ExportIomXml**
> void ExportIomXml (Guid projectId, int connectionId, string version = null)

Export connection to XML which includes https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class ExportIomXmlExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ExportApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 
            var version = "version_example";  // string |  (optional) 

            try
            {
                // Export connection to XML which includes https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs
                apiInstance.ExportIomXml(projectId, connectionId, version);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ExportApi.ExportIomXml: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ExportIomXmlWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Export connection to XML which includes https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/OpenModelContainer.cs
    apiInstance.ExportIomXmlWithHttpInfo(projectId, connectionId, version);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ExportApi.ExportIomXmlWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** |  |  |
| **connectionId** | **int** |  |  |
| **version** | **string** |  | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

