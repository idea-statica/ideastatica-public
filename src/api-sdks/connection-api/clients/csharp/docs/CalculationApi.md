# IdeaStatiCa.ConnectionApi.Api.CalculationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**Calculate**](CalculationApi.md#calculate) | **POST** /api/1/projects/{projectId}/connections/calculate | Run CBFEM caluclation and return the summary of the results |
| [**GetRawJsonResults**](CalculationApi.md#getrawjsonresults) | **POST** /api/1/projects/{projectId}/connections/rawresults-text | Get json string which represents raw CBFEM results (an instance of CheckResultsData) |
| [**GetResults**](CalculationApi.md#getresults) | **POST** /api/1/projects/{projectId}/connections/results | Get detailed results of the CBFEM analysis |

<a id="calculate"></a>
# **Calculate**
> List&lt;ConResultSummary&gt; Calculate (Guid projectId, ConCalculationParameter conCalculationParameter = null)

Run CBFEM caluclation and return the summary of the results

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class CalculateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new CalculationApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service
            var conCalculationParameter = new ConCalculationParameter(); // ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional) 

            try
            {
                // Run CBFEM caluclation and return the summary of the results
                List<ConResultSummary> result = apiInstance.Calculate(projectId, conCalculationParameter);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CalculationApi.Calculate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CalculateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Run CBFEM caluclation and return the summary of the results
    ApiResponse<List<ConResultSummary>> response = apiInstance.CalculateWithHttpInfo(projectId, conCalculationParameter);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CalculationApi.CalculateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conCalculationParameter** | [**ConCalculationParameter**](ConCalculationParameter.md) | List of connections to calculate and a type of CBFEM analysis | [optional]  |

### Return type

[**List&lt;ConResultSummary&gt;**](ConResultSummary.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getrawjsonresults"></a>
# **GetRawJsonResults**
> List&lt;string&gt; GetRawJsonResults (Guid projectId, ConCalculationParameter conCalculationParameter = null)

Get json string which represents raw CBFEM results (an instance of CheckResultsData)

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetRawJsonResultsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new CalculationApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened connection in the ConnectionRestApi service
            var conCalculationParameter = new ConCalculationParameter(); // ConCalculationParameter | Type of requested analysis and connection to calculate (optional) 

            try
            {
                // Get json string which represents raw CBFEM results (an instance of CheckResultsData)
                List<string> result = apiInstance.GetRawJsonResults(projectId, conCalculationParameter);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CalculationApi.GetRawJsonResults: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetRawJsonResultsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get json string which represents raw CBFEM results (an instance of CheckResultsData)
    ApiResponse<List<string>> response = apiInstance.GetRawJsonResultsWithHttpInfo(projectId, conCalculationParameter);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CalculationApi.GetRawJsonResultsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened connection in the ConnectionRestApi service |  |
| **conCalculationParameter** | [**ConCalculationParameter**](ConCalculationParameter.md) | Type of requested analysis and connection to calculate | [optional]  |

### Return type

**List<string>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getresults"></a>
# **GetResults**
> List&lt;ConnectionCheckRes&gt; GetResults (Guid projectId, ConCalculationParameter conCalculationParameter = null)

Get detailed results of the CBFEM analysis

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetResultsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new CalculationApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service
            var conCalculationParameter = new ConCalculationParameter(); // ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional) 

            try
            {
                // Get detailed results of the CBFEM analysis
                List<ConnectionCheckRes> result = apiInstance.GetResults(projectId, conCalculationParameter);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CalculationApi.GetResults: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetResultsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get detailed results of the CBFEM analysis
    ApiResponse<List<ConnectionCheckRes>> response = apiInstance.GetResultsWithHttpInfo(projectId, conCalculationParameter);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CalculationApi.GetResultsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conCalculationParameter** | [**ConCalculationParameter**](ConCalculationParameter.md) | List of connections to calculate and a type of CBFEM analysis | [optional]  |

### Return type

[**List&lt;ConnectionCheckRes&gt;**](ConnectionCheckRes.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

