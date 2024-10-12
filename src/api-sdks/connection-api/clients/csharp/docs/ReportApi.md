# IdeaStatiCa.ConnectionApi.Api.ReportApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GeneratePdf**](ReportApi.md#generatepdf) | **GET** /api/1/projects/{projectId}/reports/{connectionId}/pdf | Generates report for projectId and connectionId |
| [**GenerateWord**](ReportApi.md#generateword) | **GET** /api/1/projects/{projectId}/reports/{connectionId}/word | Generates report for projectId and connectionId |

<a id="generatepdf"></a>
# **GeneratePdf**
> void GeneratePdf (Guid projectId, int connectionId)

Generates report for projectId and connectionId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GeneratePdfExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ReportApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 

            try
            {
                // Generates report for projectId and connectionId
                apiInstance.GeneratePdf(projectId, connectionId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ReportApi.GeneratePdf: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GeneratePdfWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Generates report for projectId and connectionId
    apiInstance.GeneratePdfWithHttpInfo(projectId, connectionId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ReportApi.GeneratePdfWithHttpInfo: " + e.Message);
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

<a id="generateword"></a>
# **GenerateWord**
> void GenerateWord (Guid projectId, int connectionId)

Generates report for projectId and connectionId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GenerateWordExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ReportApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 

            try
            {
                // Generates report for projectId and connectionId
                apiInstance.GenerateWord(projectId, connectionId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ReportApi.GenerateWord: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GenerateWordWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Generates report for projectId and connectionId
    apiInstance.GenerateWordWithHttpInfo(projectId, connectionId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ReportApi.GenerateWordWithHttpInfo: " + e.Message);
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

