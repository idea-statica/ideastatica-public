# ParameterApi

| Method  | Description |
|--------|-------------|
| [**CreateParameterAsync**](ParameterApi.md#createparameterasync) | Creates a parameter in the connection.    The expression is evaluated before the parameter is stored, and the request is rejected if it  cannot be evaluated - an unknown identifier, a syntax error, a reference to the parameter being  created. It must therefore reference only parameters that already exist, so a set of dependent  parameters is created driver-first.  Bounds are checked, not enforced: a value outside the bounds given in the same request is  created and applied, and reported with a warning status and a message saying which bound it  exceeds - the same answer a later update of that value gives. |
| [**CreateParameterLinkAsync**](ParameterApi.md#createparameterlinkasync) | Links a parameter to a model property so the parameter drives it. Name the owner exactly as the  linkable-properties catalog reports it and pass its propertyId unchanged.    A link is deliberately type-agnostic: the parameter&#39;s type is not checked against the property&#39;s  &#x60;valueType&#x60;, because an Expression parameter can evaluate to any type and the value a  parameter yields is only known once it is evaluated. A parameter whose value the property cannot  take is reported by the engine when the parameters are applied, not when the link is created -  so match the catalog&#39;s &#x60;valueType&#x60; when choosing which parameter to link. |
| [**DeleteParameterAsync**](ParameterApi.md#deleteparameterasync) | Deletes one parameter and every link through which it drove a model property. |
| [**DeleteParameterLinkAsync**](ParameterApi.md#deleteparameterlinkasync) | Removes a parameter-to-property link. The parameter is kept, and the property retains the value  the parameter last applied. |
| [**DeleteParametersAsync**](ParameterApi.md#deleteparametersasync) | Delete all parameters and parameter model links for the connection connectionId in the project projectId. |
| [**EvaluateExpressionAsync**](ParameterApi.md#evaluateexpressionasync) | Evaluate the expression and return the result.  For more details see documentation about parameters:  https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html  or  https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html |
| [**GetLinkablePropertiesAsync**](ParameterApi.md#getlinkablepropertiesasync) | Lists every model property of the connection that a parameter can be linked to, across  operations, members and the library items the connection edits (cross-sections, materials,  bolt assemblies). Each row names its owner.    The list is not filtered by what the Design tab currently shows: deciding that needs the  desktop editor context, which the service does not have. Some rows therefore belong to a  property the application hides for the operation&#39;s present configuration, and linking a  parameter to one of those drives a value the design does not use. |
| [**GetMemberLinkablePropertiesAsync**](ParameterApi.md#getmemberlinkablepropertiesasync) | Lists the model properties of one member that a parameter can be linked to. |
| [**GetOperationLinkablePropertiesAsync**](ParameterApi.md#getoperationlinkablepropertiesasync) | Lists the model properties of one operation that a parameter can be linked to. |
| [**GetParameterLinksAsync**](ParameterApi.md#getparameterlinksasync) | Lists the parameter-to-property links of the connection. |
| [**GetParametersAsync**](ParameterApi.md#getparametersasync) | Gets all parameters defined for the specified project and connection. |
| [**UpdateAsync**](ParameterApi.md#updateasync) | Updates parameters for the specified connection in the project with the values provided. |

<a id="createparameter"></a>
## **CreateParameterAsync**
> **IdeaParameter CreateParameterAsync (Guid projectId, int connectionId, ConParameterCreate conParameterCreate = null)**

Creates a parameter in the connection.    The expression is evaluated before the parameter is stored, and the request is rejected if it  cannot be evaluated - an unknown identifier, a syntax error, a reference to the parameter being  created. It must therefore reference only parameters that already exist, so a set of dependent  parameters is created driver-first.  Bounds are checked, not enforced: a value outside the bounds given in the same request is  created and applied, and reported with a warning status and a message saying which bound it  exceeds - the same answer a later update of that value gives.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection the parameter is added to. |  |
| **conParameterCreate** | [**ConParameterCreate**](ConParameterCreate.md) | Identifier, type and value or expression of the new parameter. | [optional]  |

### Return type

[**IdeaParameter**](IdeaParameter.md)

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
    public class CreateParameterAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection the parameter is added to.
                    var conParameterCreate = new ConParameterCreate(); // ConParameterCreate | Identifier, type and value or expression of the new parameter. (optional) 

                    try
                    {
                        // Creates a parameter in the connection.    The expression is evaluated before the parameter is stored, and the request is rejected if it  cannot be evaluated - an unknown identifier, a syntax error, a reference to the parameter being  created. It must therefore reference only parameters that already exist, so a set of dependent  parameters is created driver-first.  Bounds are checked, not enforced: a value outside the bounds given in the same request is  created and applied, and reported with a warning status and a message saying which bound it  exceeds - the same answer a later update of that value gives.
                        IdeaParameter result = await conClient.Parameter.CreateParameterAsync(projectId, connectionId, conParameterCreate);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.CreateParameterAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/CreateParameter.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/4/projects/{projectId}/connections/{connectionId}/parameters 

#### Using the CreateParameterWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Creates a parameter in the connection.    The expression is evaluated before the parameter is stored, and the request is rejected if it  cannot be evaluated - an unknown identifier, a syntax error, a reference to the parameter being  created. It must therefore reference only parameters that already exist, so a set of dependent  parameters is created driver-first.  Bounds are checked, not enforced: a value outside the bounds given in the same request is  created and applied, and reported with a warning status and a message saying which bound it  exceeds - the same answer a later update of that value gives.
    ApiResponse<IdeaParameter> response = conClient.Parameter.CreateParameterWithHttpInfo(projectId, connectionId, conParameterCreate);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.CreateParameterWithHttpInfo: " + e.Message);
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
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **422** | Unprocessable Content |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="createparameterlink"></a>
## **CreateParameterLinkAsync**
> **ConParameterLink CreateParameterLinkAsync (Guid projectId, int connectionId, ConParameterLinkCreate conParameterLinkCreate = null)**

Links a parameter to a model property so the parameter drives it. Name the owner exactly as the  linkable-properties catalog reports it and pass its propertyId unchanged.    A link is deliberately type-agnostic: the parameter's type is not checked against the property's  `valueType`, because an Expression parameter can evaluate to any type and the value a  parameter yields is only known once it is evaluated. A parameter whose value the property cannot  take is reported by the engine when the parameters are applied, not when the link is created -  so match the catalog's `valueType` when choosing which parameter to link.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection. |  |
| **conParameterLinkCreate** | [**ConParameterLinkCreate**](ConParameterLinkCreate.md) | Parameter identifier, property owner and property id. | [optional]  |

### Return type

[**ConParameterLink**](ConParameterLink.md)

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
    public class CreateParameterLinkAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection.
                    var conParameterLinkCreate = new ConParameterLinkCreate(); // ConParameterLinkCreate | Parameter identifier, property owner and property id. (optional) 

                    try
                    {
                        // Links a parameter to a model property so the parameter drives it. Name the owner exactly as the  linkable-properties catalog reports it and pass its propertyId unchanged.    A link is deliberately type-agnostic: the parameter's type is not checked against the property's  `valueType`, because an Expression parameter can evaluate to any type and the value a  parameter yields is only known once it is evaluated. A parameter whose value the property cannot  take is reported by the engine when the parameters are applied, not when the link is created -  so match the catalog's `valueType` when choosing which parameter to link.
                        ConParameterLink result = await conClient.Parameter.CreateParameterLinkAsync(projectId, connectionId, conParameterLinkCreate);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.CreateParameterLinkAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/CreateParameterLink.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/4/projects/{projectId}/connections/{connectionId}/parameters/links 

#### Using the CreateParameterLinkWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Links a parameter to a model property so the parameter drives it. Name the owner exactly as the  linkable-properties catalog reports it and pass its propertyId unchanged.    A link is deliberately type-agnostic: the parameter's type is not checked against the property's  `valueType`, because an Expression parameter can evaluate to any type and the value a  parameter yields is only known once it is evaluated. A parameter whose value the property cannot  take is reported by the engine when the parameters are applied, not when the link is created -  so match the catalog's `valueType` when choosing which parameter to link.
    ApiResponse<ConParameterLink> response = conClient.Parameter.CreateParameterLinkWithHttpInfo(projectId, connectionId, conParameterLinkCreate);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.CreateParameterLinkWithHttpInfo: " + e.Message);
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
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **422** | Unprocessable Content |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="deleteparameter"></a>
## **DeleteParameterAsync**
> **ConParameterDeleteResult DeleteParameterAsync (Guid projectId, int connectionId, string key)**

Deletes one parameter and every link through which it drove a model property.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection. |  |
| **key** | **string** | Identifier of the parameter to delete. |  |

### Return type

[**ConParameterDeleteResult**](ConParameterDeleteResult.md)

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
    public class DeleteParameterAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection.
                    key = "key_example";  // string | Identifier of the parameter to delete.

                    try
                    {
                        // Deletes one parameter and every link through which it drove a model property.
                        ConParameterDeleteResult result = await conClient.Parameter.DeleteParameterAsync(projectId, connectionId, key);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.DeleteParameterAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/DeleteParameter.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/4/projects/{projectId}/connections/{connectionId}/parameters/{key} 

#### Using the DeleteParameterWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Deletes one parameter and every link through which it drove a model property.
    ApiResponse<ConParameterDeleteResult> response = conClient.Parameter.DeleteParameterWithHttpInfo(projectId, connectionId, key);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.DeleteParameterWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **422** | Unprocessable Content |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="deleteparameterlink"></a>
## **DeleteParameterLinkAsync**
> **void DeleteParameterLinkAsync (Guid projectId, int connectionId, int linkId)**

Removes a parameter-to-property link. The parameter is kept, and the property retains the value  the parameter last applied.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection. |  |
| **linkId** | **int** | Id of the link to remove. |  |

### Return type

void (empty response body)

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
    public class DeleteParameterLinkAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection.
                    linkId = 56;  // int | Id of the link to remove.

                    try
                    {
                        // Removes a parameter-to-property link. The parameter is kept, and the property retains the value  the parameter last applied.
                        conClient.Parameter.DeleteParameterLinkAsync(projectId, connectionId, linkId);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.DeleteParameterLinkAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/DeleteParameterLink.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/4/projects/{projectId}/connections/{connectionId}/parameters/links/{linkId} 

#### Using the DeleteParameterLinkWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Removes a parameter-to-property link. The parameter is kept, and the property retains the value  the parameter last applied.
    conClient.Parameter.DeleteParameterLinkWithHttpInfo(projectId, connectionId, linkId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.DeleteParameterLinkWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | No Content |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **422** | Unprocessable Content |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="deleteparameters"></a>
## **DeleteParametersAsync**
> **void DeleteParametersAsync (Guid projectId, int connectionId)**

Delete all parameters and parameter model links for the connection connectionId in the project projectId.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection where to delete parameters. |  |

### Return type

void (empty response body)

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
    public class DeleteParametersAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection where to delete parameters.

                    try
                    {
                        // Delete all parameters and parameter model links for the connection connectionId in the project projectId.
                        conClient.Parameter.DeleteParametersAsync(projectId, connectionId);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.DeleteParametersAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/DeleteParameters.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **DELETE** /api/4/projects/{projectId}/connections/{connectionId}/parameters 

#### Using the DeleteParametersWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Delete all parameters and parameter model links for the connection connectionId in the project projectId.
    conClient.Parameter.DeleteParametersWithHttpInfo(projectId, connectionId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.DeleteParametersWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | No Content |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="evaluateexpression"></a>
## **EvaluateExpressionAsync**
> **string EvaluateExpressionAsync (Guid projectId, int connectionId, string body = null)**

Evaluate the expression and return the result.  For more details see documentation about parameters:  https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html  or  https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection to use for evaluation expression. |  |
| **body** | **string** | Expression to evaluate. See the API documentation for supported syntax and examples: https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html | [optional]  |

### Return type

**string**

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
    public class EvaluateExpressionAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection to use for evaluation expression.
                    body = "body_example";  // string | Expression to evaluate. See the API documentation for supported syntax and examples: https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html (optional) 

                    try
                    {
                        // Evaluate the expression and return the result.  For more details see documentation about parameters:  https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html  or  https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html
                        string result = await conClient.Parameter.EvaluateExpressionAsync(projectId, connectionId, body);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.EvaluateExpressionAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/EvaluateExpression.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **POST** /api/4/projects/{projectId}/connections/{connectionId}/evaluate-expression 

#### Using the EvaluateExpressionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Evaluate the expression and return the result.  For more details see documentation about parameters:  https://developer.ideastatica.com/docs/api/api_parameters_getting_started.html  or  https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html
    ApiResponse<string> response = conClient.Parameter.EvaluateExpressionWithHttpInfo(projectId, connectionId, body);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.EvaluateExpressionWithHttpInfo: " + e.Message);
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
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **422** | Unprocessable Content |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getlinkableproperties"></a>
## **GetLinkablePropertiesAsync**
> **List&lt;ConLinkableProperty&gt; GetLinkablePropertiesAsync (Guid projectId, int connectionId, string valueType = null)**

Lists every model property of the connection that a parameter can be linked to, across  operations, members and the library items the connection edits (cross-sections, materials,  bolt assemblies). Each row names its owner.    The list is not filtered by what the Design tab currently shows: deciding that needs the  desktop editor context, which the service does not have. Some rows therefore belong to a  property the application hides for the operation's present configuration, and linking a  parameter to one of those drives a value the design does not use.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection. |  |
| **valueType** | **string** | Keep only properties of this parameter type, e.g. &#x60;Float&#x60;. | [optional]  |

### Return type

[**List&lt;ConLinkableProperty&gt;**](ConLinkableProperty.md)

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
    public class GetLinkablePropertiesAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection.
                    valueType = "valueType_example";  // string | Keep only properties of this parameter type, e.g. `Float`. (optional) 

                    try
                    {
                        // Lists every model property of the connection that a parameter can be linked to, across  operations, members and the library items the connection edits (cross-sections, materials,  bolt assemblies). Each row names its owner.    The list is not filtered by what the Design tab currently shows: deciding that needs the  desktop editor context, which the service does not have. Some rows therefore belong to a  property the application hides for the operation's present configuration, and linking a  parameter to one of those drives a value the design does not use.
                        List<ConLinkableProperty> result = await conClient.Parameter.GetLinkablePropertiesAsync(projectId, connectionId, valueType);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.GetLinkablePropertiesAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/GetLinkableProperties.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/parameters/linkable-properties 

#### Using the GetLinkablePropertiesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Lists every model property of the connection that a parameter can be linked to, across  operations, members and the library items the connection edits (cross-sections, materials,  bolt assemblies). Each row names its owner.    The list is not filtered by what the Design tab currently shows: deciding that needs the  desktop editor context, which the service does not have. Some rows therefore belong to a  property the application hides for the operation's present configuration, and linking a  parameter to one of those drives a value the design does not use.
    ApiResponse<List<ConLinkableProperty>> response = conClient.Parameter.GetLinkablePropertiesWithHttpInfo(projectId, connectionId, valueType);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.GetLinkablePropertiesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getmemberlinkableproperties"></a>
## **GetMemberLinkablePropertiesAsync**
> **List&lt;ConLinkableProperty&gt; GetMemberLinkablePropertiesAsync (Guid projectId, int connectionId, int memberId, string valueType = null)**

Lists the model properties of one member that a parameter can be linked to.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection. |  |
| **memberId** | **int** | Id of the member whose properties are listed. |  |
| **valueType** | **string** | Keep only properties of this parameter type, e.g. &#x60;Float&#x60;. | [optional]  |

### Return type

[**List&lt;ConLinkableProperty&gt;**](ConLinkableProperty.md)

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
    public class GetMemberLinkablePropertiesAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection.
                    memberId = 56;  // int | Id of the member whose properties are listed.
                    valueType = "valueType_example";  // string | Keep only properties of this parameter type, e.g. `Float`. (optional) 

                    try
                    {
                        // Lists the model properties of one member that a parameter can be linked to.
                        List<ConLinkableProperty> result = await conClient.Parameter.GetMemberLinkablePropertiesAsync(projectId, connectionId, memberId, valueType);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.GetMemberLinkablePropertiesAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/GetMemberLinkableProperties.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/members/{memberId}/linkable-properties 

#### Using the GetMemberLinkablePropertiesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Lists the model properties of one member that a parameter can be linked to.
    ApiResponse<List<ConLinkableProperty>> response = conClient.Parameter.GetMemberLinkablePropertiesWithHttpInfo(projectId, connectionId, memberId, valueType);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.GetMemberLinkablePropertiesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getoperationlinkableproperties"></a>
## **GetOperationLinkablePropertiesAsync**
> **List&lt;ConLinkableProperty&gt; GetOperationLinkablePropertiesAsync (Guid projectId, int connectionId, int operationId, string valueType = null)**

Lists the model properties of one operation that a parameter can be linked to.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection. |  |
| **operationId** | **int** | Id of the operation whose properties are listed. |  |
| **valueType** | **string** | Keep only properties of this parameter type, e.g. &#x60;Float&#x60;. | [optional]  |

### Return type

[**List&lt;ConLinkableProperty&gt;**](ConLinkableProperty.md)

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
    public class GetOperationLinkablePropertiesAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection.
                    operationId = 56;  // int | Id of the operation whose properties are listed.
                    valueType = "valueType_example";  // string | Keep only properties of this parameter type, e.g. `Float`. (optional) 

                    try
                    {
                        // Lists the model properties of one operation that a parameter can be linked to.
                        List<ConLinkableProperty> result = await conClient.Parameter.GetOperationLinkablePropertiesAsync(projectId, connectionId, operationId, valueType);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.GetOperationLinkablePropertiesAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/GetOperationLinkableProperties.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/operations/{operationId}/linkable-properties 

#### Using the GetOperationLinkablePropertiesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Lists the model properties of one operation that a parameter can be linked to.
    ApiResponse<List<ConLinkableProperty>> response = conClient.Parameter.GetOperationLinkablePropertiesWithHttpInfo(projectId, connectionId, operationId, valueType);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.GetOperationLinkablePropertiesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getparameterlinks"></a>
## **GetParameterLinksAsync**
> **List&lt;ConParameterLink&gt; GetParameterLinksAsync (Guid projectId, int connectionId)**

Lists the parameter-to-property links of the connection.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | Id of the connection. |  |

### Return type

[**List&lt;ConParameterLink&gt;**](ConParameterLink.md)

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
    public class GetParameterLinksAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | Id of the connection.

                    try
                    {
                        // Lists the parameter-to-property links of the connection.
                        List<ConParameterLink> result = await conClient.Parameter.GetParameterLinksAsync(projectId, connectionId);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.GetParameterLinksAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/GetParameterLinks.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/parameters/links 

#### Using the GetParameterLinksWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Lists the parameter-to-property links of the connection.
    ApiResponse<List<ConParameterLink>> response = conClient.Parameter.GetParameterLinksWithHttpInfo(projectId, connectionId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.GetParameterLinksWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getparameters"></a>
## **GetParametersAsync**
> **List&lt;IdeaParameter&gt; GetParametersAsync (Guid projectId, int connectionId, bool? includeHidden = null)**

Gets all parameters defined for the specified project and connection.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | The ID of the connection from which to retrieve parameters. |  |
| **includeHidden** | **bool?** | If true, includes hidden parameters in the result. | [optional] [default to false] |

### Return type

[**List&lt;IdeaParameter&gt;**](IdeaParameter.md)

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
    public class GetParametersAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | The ID of the connection from which to retrieve parameters.
                    includeHidden = false;  // bool? | If true, includes hidden parameters in the result. (optional)  (default to false)

                    try
                    {
                        // Gets all parameters defined for the specified project and connection.
                        List<IdeaParameter> result = await conClient.Parameter.GetParametersAsync(projectId, connectionId, includeHidden);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.GetParametersAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/GetParameters.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **GET** /api/4/projects/{projectId}/connections/{connectionId}/parameters 

#### Using the GetParametersWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Gets all parameters defined for the specified project and connection.
    ApiResponse<List<IdeaParameter>> response = conClient.Parameter.GetParametersWithHttpInfo(projectId, connectionId, includeHidden);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.GetParametersWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

#### Authorization

No authorization required

#### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


#### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="update"></a>
## **UpdateAsync**
> **ParameterUpdateResponse UpdateAsync (Guid projectId, int connectionId, List<IdeaParameterUpdate> ideaParameterUpdate = null)**

Updates parameters for the specified connection in the project with the values provided.



### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service. |  |
| **connectionId** | **int** | The ID of the connection to update. |  |
| **ideaParameterUpdate** | [**List&lt;IdeaParameterUpdate&gt;**](IdeaParameterUpdate.md) | New values of parameters to apply. | [optional]  |

### Return type

[**ParameterUpdateResponse**](ParameterUpdateResponse.md)

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
    public class UpdateAsyncExample
    {
        public static async Task Main()
        {
            string ideaConFile = "testCon.ideaCon";
            
            string ideaStatiCaPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.0"; // Path to the IdeaStatiCa.ConnectionRestApi.exe
            
            using (var clientFactory = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                using (var conClient = await clientFactory.CreateApiClient())
                {

                    // Open the project and get its id
                    var projData = await conClient.Project.OpenProjectAsync(ideaConFile);
                    Guid projectId = projData.ProjectId;
                    
                    // (Required) Select parameters
                    connectionId = 56;  // int | The ID of the connection to update.
                    var ideaParameterUpdate = new List<IdeaParameterUpdate>(); // List<IdeaParameterUpdate> | New values of parameters to apply. (optional) 

                    try
                    {
                        // Updates parameters for the specified connection in the project with the values provided.
                        ParameterUpdateResponse result = await conClient.Parameter.UpdateAsync(projectId, connectionId, ideaParameterUpdate);
                        Debug.WriteLine(result);
                    }
                    catch (ApiException  e)
                    {
                        Console.WriteLine("Exception when calling Parameter.UpdateAsync: " + e.Message);
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
}
```

### Code Samples

[!code-csharp[](../examples/CodeSamples/Samples/Update.cs)]

Looking for a code sample? request some help on our [discussion](https://github.com/idea-statica/ideastatica-public/discussions) page. 

### REST Usage

#### Http Request

All URIs are relative to *http://localhost*

> **PUT** /api/4/projects/{projectId}/connections/{connectionId}/parameters 

#### Using the UpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Updates parameters for the specified connection in the project with the values provided.
    ApiResponse<ParameterUpdateResponse> response = conClient.Parameter.UpdateWithHttpInfo(projectId, connectionId, ideaParameterUpdate);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ParameterApi.UpdateWithHttpInfo: " + e.Message);
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
| **200** | OK |  -  |
| **401** | Unauthorized |  -  |
| **404** | Not Found |  -  |
| **422** | Unprocessable Content |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

