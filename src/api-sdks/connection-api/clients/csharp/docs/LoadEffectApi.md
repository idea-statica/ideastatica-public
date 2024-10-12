# IdeaStatiCa.ConnectionApi.Api.LoadEffectApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AddLoadEffect**](LoadEffectApi.md#addloadeffect) | **POST** /api/1/projects/{projectId}/connections/{connectionId}/load-effects | Add new load effect to the connection |
| [**DeleteLoadEffect**](LoadEffectApi.md#deleteloadeffect) | **DELETE** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/{loadEffectId} | Delete load effect loadEffectId |
| [**GetLoadEffect**](LoadEffectApi.md#getloadeffect) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/{loadEffectId} | Get load impulses from loadEffectId |
| [**GetLoadEffects**](LoadEffectApi.md#getloadeffects) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/load-effects | Get all load effects which are defined in connectionId |
| [**GetLoadSettings**](LoadEffectApi.md#getloadsettings) | **GET** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/get-load-settings | Get Load settings for connection in project |
| [**SetLoadSettings**](LoadEffectApi.md#setloadsettings) | **POST** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/set-load-settings | Set Load settings for connection in project |
| [**UpdateLoadEffect**](LoadEffectApi.md#updateloadeffect) | **PUT** /api/1/projects/{projectId}/connections/{connectionId}/load-effects/{loadEffectId} | Update load impulses in loadEffectId |

<a id="addloadeffect"></a>
# **AddLoadEffect**
> LoadEffectData AddLoadEffect (Guid projectId, int connectionId, ConLoadEffect conLoadEffect = null)

Add new load effect to the connection

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class AddLoadEffectExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new LoadEffectApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 
            var conLoadEffect = new ConLoadEffect(); // ConLoadEffect |  (optional) 

            try
            {
                // Add new load effect to the connection
                LoadEffectData result = apiInstance.AddLoadEffect(projectId, connectionId, conLoadEffect);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LoadEffectApi.AddLoadEffect: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AddLoadEffectWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Add new load effect to the connection
    ApiResponse<LoadEffectData> response = apiInstance.AddLoadEffectWithHttpInfo(projectId, connectionId, conLoadEffect);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LoadEffectApi.AddLoadEffectWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** |  |  |
| **connectionId** | **int** |  |  |
| **conLoadEffect** | [**ConLoadEffect**](ConLoadEffect.md) |  | [optional]  |

### Return type

[**LoadEffectData**](LoadEffectData.md)

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

<a id="deleteloadeffect"></a>
# **DeleteLoadEffect**
> int DeleteLoadEffect (Guid projectId, int connectionId, int loadEffectId)

Delete load effect loadEffectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class DeleteLoadEffectExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new LoadEffectApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 
            var loadEffectId = 56;  // int | 

            try
            {
                // Delete load effect loadEffectId
                int result = apiInstance.DeleteLoadEffect(projectId, connectionId, loadEffectId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LoadEffectApi.DeleteLoadEffect: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DeleteLoadEffectWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Delete load effect loadEffectId
    ApiResponse<int> response = apiInstance.DeleteLoadEffectWithHttpInfo(projectId, connectionId, loadEffectId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LoadEffectApi.DeleteLoadEffectWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** |  |  |
| **connectionId** | **int** |  |  |
| **loadEffectId** | **int** |  |  |

### Return type

**int**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getloadeffect"></a>
# **GetLoadEffect**
> ConLoadEffect GetLoadEffect (Guid projectId, int connectionId, int loadEffectId, bool? isPercentage = null)

Get load impulses from loadEffectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetLoadEffectExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new LoadEffectApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 
            var loadEffectId = 56;  // int | 
            var isPercentage = true;  // bool? |  (optional) 

            try
            {
                // Get load impulses from loadEffectId
                ConLoadEffect result = apiInstance.GetLoadEffect(projectId, connectionId, loadEffectId, isPercentage);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LoadEffectApi.GetLoadEffect: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetLoadEffectWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get load impulses from loadEffectId
    ApiResponse<ConLoadEffect> response = apiInstance.GetLoadEffectWithHttpInfo(projectId, connectionId, loadEffectId, isPercentage);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LoadEffectApi.GetLoadEffectWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** |  |  |
| **connectionId** | **int** |  |  |
| **loadEffectId** | **int** |  |  |
| **isPercentage** | **bool?** |  | [optional]  |

### Return type

[**ConLoadEffect**](ConLoadEffect.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getloadeffects"></a>
# **GetLoadEffects**
> List&lt;ConLoadEffect&gt; GetLoadEffects (Guid projectId, int connectionId, bool? isPercentage = null)

Get all load effects which are defined in connectionId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetLoadEffectsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new LoadEffectApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 
            var isPercentage = true;  // bool? |  (optional) 

            try
            {
                // Get all load effects which are defined in connectionId
                List<ConLoadEffect> result = apiInstance.GetLoadEffects(projectId, connectionId, isPercentage);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LoadEffectApi.GetLoadEffects: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetLoadEffectsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get all load effects which are defined in connectionId
    ApiResponse<List<ConLoadEffect>> response = apiInstance.GetLoadEffectsWithHttpInfo(projectId, connectionId, isPercentage);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LoadEffectApi.GetLoadEffectsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** |  |  |
| **connectionId** | **int** |  |  |
| **isPercentage** | **bool?** |  | [optional]  |

### Return type

[**List&lt;ConLoadEffect&gt;**](ConLoadEffect.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getloadsettings"></a>
# **GetLoadSettings**
> ConLoadSettings GetLoadSettings (Guid projectId, int connectionId)

Get Load settings for connection in project

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetLoadSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new LoadEffectApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 

            try
            {
                // Get Load settings for connection in project
                ConLoadSettings result = apiInstance.GetLoadSettings(projectId, connectionId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LoadEffectApi.GetLoadSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetLoadSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get Load settings for connection in project
    ApiResponse<ConLoadSettings> response = apiInstance.GetLoadSettingsWithHttpInfo(projectId, connectionId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LoadEffectApi.GetLoadSettingsWithHttpInfo: " + e.Message);
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

[**ConLoadSettings**](ConLoadSettings.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="setloadsettings"></a>
# **SetLoadSettings**
> ConLoadSettings SetLoadSettings (Guid projectId, int connectionId, ConLoadSettings conLoadSettings = null)

Set Load settings for connection in project

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class SetLoadSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new LoadEffectApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 
            var conLoadSettings = new ConLoadSettings(); // ConLoadSettings |  (optional) 

            try
            {
                // Set Load settings for connection in project
                ConLoadSettings result = apiInstance.SetLoadSettings(projectId, connectionId, conLoadSettings);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LoadEffectApi.SetLoadSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SetLoadSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Set Load settings for connection in project
    ApiResponse<ConLoadSettings> response = apiInstance.SetLoadSettingsWithHttpInfo(projectId, connectionId, conLoadSettings);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LoadEffectApi.SetLoadSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** |  |  |
| **connectionId** | **int** |  |  |
| **conLoadSettings** | [**ConLoadSettings**](ConLoadSettings.md) |  | [optional]  |

### Return type

[**ConLoadSettings**](ConLoadSettings.md)

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

<a id="updateloadeffect"></a>
# **UpdateLoadEffect**
> ConLoadEffect UpdateLoadEffect (Guid projectId, int connectionId, int loadEffectId, ConLoadEffect conLoadEffect = null)

Update load impulses in loadEffectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class UpdateLoadEffectExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new LoadEffectApi(config);
            var projectId = "projectId_example";  // Guid | 
            var connectionId = 56;  // int | 
            var loadEffectId = 56;  // int | 
            var conLoadEffect = new ConLoadEffect(); // ConLoadEffect |  (optional) 

            try
            {
                // Update load impulses in loadEffectId
                ConLoadEffect result = apiInstance.UpdateLoadEffect(projectId, connectionId, loadEffectId, conLoadEffect);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LoadEffectApi.UpdateLoadEffect: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the UpdateLoadEffectWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Update load impulses in loadEffectId
    ApiResponse<ConLoadEffect> response = apiInstance.UpdateLoadEffectWithHttpInfo(projectId, connectionId, loadEffectId, conLoadEffect);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LoadEffectApi.UpdateLoadEffectWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** |  |  |
| **connectionId** | **int** |  |  |
| **loadEffectId** | **int** |  |  |
| **conLoadEffect** | [**ConLoadEffect**](ConLoadEffect.md) |  | [optional]  |

### Return type

[**ConLoadEffect**](ConLoadEffect.md)

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

