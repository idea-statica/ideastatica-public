# CalculationApi

| Method  | Description |
|--------|-------------|
| [**Calculate**](CalculationApi.md#calculate) | Run CBFEM caluclation and return the summary of the results |
| [**GetRawJsonResults**](CalculationApi.md#getrawjsonresults) | Get json string which represents raw CBFEM results (an instance of CheckResultsData) |
| [**GetResults**](CalculationApi.md#getresults) | Get detailed results of the CBFEM analysis |

<a id="calculate"></a>
## **Calculate**
> **List&lt;ConResultSummary&gt; Calculate (Guid projectId, ConCalculationParameter conCalculationParameter = null)**

Run CBFEM caluclation and return the summary of the results



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conCalculationParameter** | [**ConCalculationParameter**](ConCalculationParameter.md) | List of connections to calculate and a type of CBFEM analysis | [optional]  |

### Return type

[**List&lt;ConResultSummary&gt;**](ConResultSummary.md)

### Example

Note: this example is autogenerated.

```csharp
using System;
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
            // Create the client which is connected to the service.
            ConnectionApiClientFactory clientFactory = new ConnectionApiClientFactory("http://localhost:5000");
            using (var conClient = await clientFactory.CreateConnectionApiClient())
            {
                var project = await conClient.Project.Open("myProject.ideaCon"); //Open a project
                Guid projectId = project.ProjectId; //Get projectId Guid
                
                var conCalculationParameter = new ConCalculationParameter(); // ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional) 

                try
                {
                    // Run CBFEM caluclation and return the summary of the results
                    List<ConResultSummary> result = conClient.Calculation.Calculate(projectId, conCalculationParameter);
                    Debug.WriteLine(result);
                }
                catch (ApiException  e)
                {
                    Console.WriteLine("Exception when calling Calculation.Calculate: " + e.Message);
                    Console.WriteLine("Status Code: " + e.ErrorCode);
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    await conClient.Project.CloseProjectAsync(projectId);
                }
            }
        }
    }
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/Calculate.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/connections/calculate 

#### Using the CalculateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Run CBFEM caluclation and return the summary of the results
    ApiResponse<List<ConResultSummary>> response = conClient.Calculation.CalculateWithHttpInfo(projectId, conCalculationParameter);
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

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getrawjsonresults"></a>
## **GetRawJsonResults**
> **List&lt;string&gt; GetRawJsonResults (Guid projectId, ConCalculationParameter conCalculationParameter = null)**

Get json string which represents raw CBFEM results (an instance of CheckResultsData)



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened connection in the ConnectionRestApi service |  |
| **conCalculationParameter** | [**ConCalculationParameter**](ConCalculationParameter.md) | Type of requested analysis and connection to calculate | [optional]  |

### Return type

**List<string>**

### Example

Note: this example is autogenerated.

```csharp
using System;
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
            // Create the client which is connected to the service.
            ConnectionApiClientFactory clientFactory = new ConnectionApiClientFactory("http://localhost:5000");
            using (var conClient = await clientFactory.CreateConnectionApiClient())
            {
                var project = await conClient.Project.Open("myProject.ideaCon"); //Open a project
                Guid projectId = project.ProjectId; //Get projectId Guid
                
                var conCalculationParameter = new ConCalculationParameter(); // ConCalculationParameter | Type of requested analysis and connection to calculate (optional) 

                try
                {
                    // Get json string which represents raw CBFEM results (an instance of CheckResultsData)
                    List<string> result = conClient.Calculation.GetRawJsonResults(projectId, conCalculationParameter);
                    Debug.WriteLine(result);
                }
                catch (ApiException  e)
                {
                    Console.WriteLine("Exception when calling Calculation.GetRawJsonResults: " + e.Message);
                    Console.WriteLine("Status Code: " + e.ErrorCode);
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    await conClient.Project.CloseProjectAsync(projectId);
                }
            }
        }
    }
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/GetRawJsonResults.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/connections/rawresults-text 

#### Using the GetRawJsonResultsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get json string which represents raw CBFEM results (an instance of CheckResultsData)
    ApiResponse<List<string>> response = conClient.Calculation.GetRawJsonResultsWithHttpInfo(projectId, conCalculationParameter);
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

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getresults"></a>
## **GetResults**
> **List&lt;ConnectionCheckRes&gt; GetResults (Guid projectId, ConCalculationParameter conCalculationParameter = null)**

Get detailed results of the CBFEM analysis



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conCalculationParameter** | [**ConCalculationParameter**](ConCalculationParameter.md) | List of connections to calculate and a type of CBFEM analysis | [optional]  |

### Return type

[**List&lt;ConnectionCheckRes&gt;**](ConnectionCheckRes.md)

### Example

Note: this example is autogenerated.

```csharp
using System;
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
            // Create the client which is connected to the service.
            ConnectionApiClientFactory clientFactory = new ConnectionApiClientFactory("http://localhost:5000");
            using (var conClient = await clientFactory.CreateConnectionApiClient())
            {
                var project = await conClient.Project.Open("myProject.ideaCon"); //Open a project
                Guid projectId = project.ProjectId; //Get projectId Guid
                
                var conCalculationParameter = new ConCalculationParameter(); // ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional) 

                try
                {
                    // Get detailed results of the CBFEM analysis
                    List<ConnectionCheckRes> result = conClient.Calculation.GetResults(projectId, conCalculationParameter);
                    Debug.WriteLine(result);
                }
                catch (ApiException  e)
                {
                    Console.WriteLine("Exception when calling Calculation.GetResults: " + e.Message);
                    Console.WriteLine("Status Code: " + e.ErrorCode);
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    await conClient.Project.CloseProjectAsync(projectId);
                }
            }
        }
    }
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/GetResults.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/1/projects/{projectId}/connections/results 

#### Using the GetResultsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get detailed results of the CBFEM analysis
    ApiResponse<List<ConnectionCheckRes>> response = conClient.Calculation.GetResultsWithHttpInfo(projectId, conCalculationParameter);
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

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

