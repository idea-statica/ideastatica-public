# CalculationApi

| Method  | Description |
|--------|-------------|
| [**Calculate**](CalculationApi.md#calculate) | Calculate RCS project |
| [**GetResults**](CalculationApi.md#getresults) | Get calculated results |

<a id="calculate"></a>
## **Calculate**
> **List&lt;RcsSectionResultOverview&gt; Calculate (Guid projectId, RcsCalculationParameters rcsCalculationParameters = null)**

Calculate RCS project



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | Project Id |  |
| **rcsCalculationParameters** | [**RcsCalculationParameters**](RcsCalculationParameters.md) | Calculation parameters | [optional]  |

### Return type

[**List&lt;RcsSectionResultOverview&gt;**](RcsSectionResultOverview.md)

### Example

Note: this example is autogenerated.

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.RcsApi.Api;
using IdeaStatiCa.RcsApi.Client;
using IdeaStatiCa.RcsApi.Model;

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
                
                var rcsCalculationParameters = new RcsCalculationParameters(); // RcsCalculationParameters | Calculation parameters (optional) 

                try
                {
                    // Calculate RCS project
                    List<RcsSectionResultOverview> result = conClient.Calculation.Calculate(projectId, rcsCalculationParameters);
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

> **POST** /api/1/projects/{projectId}/calculate 

#### Using the CalculateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Calculate RCS project
    ApiResponse<List<RcsSectionResultOverview>> response = conClient.Calculation.CalculateWithHttpInfo(projectId, rcsCalculationParameters);
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

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getresults"></a>
## **GetResults**
> **List&lt;RcsSectionResultDetailed&gt; GetResults (Guid projectId, RcsResultParameters rcsResultParameters = null)**

Get calculated results



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | Project Id |  |
| **rcsResultParameters** | [**RcsResultParameters**](RcsResultParameters.md) | Calculation parameters | [optional]  |

### Return type

[**List&lt;RcsSectionResultDetailed&gt;**](RcsSectionResultDetailed.md)

### Example

Note: this example is autogenerated.

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.RcsApi.Api;
using IdeaStatiCa.RcsApi.Client;
using IdeaStatiCa.RcsApi.Model;

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
                
                var rcsResultParameters = new RcsResultParameters(); // RcsResultParameters | Calculation parameters (optional) 

                try
                {
                    // Get calculated results
                    List<RcsSectionResultDetailed> result = conClient.Calculation.GetResults(projectId, rcsResultParameters);
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

> **POST** /api/1/projects/{projectId}/get-results 

#### Using the GetResultsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get calculated results
    ApiResponse<List<RcsSectionResultDetailed>> response = conClient.Calculation.GetResultsWithHttpInfo(projectId, rcsResultParameters);
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

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

