# IdeaStatiCa.ConnectionApi.Api.MaterialApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AddBoltAssembly**](MaterialApi.md#addboltassembly) | **POST** /api/1/projects/{projectId}/materials/bolt-assemblies | Add bolt assembly to the project |
| [**AddCrossSection**](MaterialApi.md#addcrosssection) | **POST** /api/1/projects/{projectId}/materials/cross-sections | Add cross section to the project |
| [**AddMaterialBoltGrade**](MaterialApi.md#addmaterialboltgrade) | **POST** /api/1/projects/{projectId}/materials/bolt-grade | Add material to the project |
| [**AddMaterialConcrete**](MaterialApi.md#addmaterialconcrete) | **POST** /api/1/projects/{projectId}/materials/concrete | Add material to the project |
| [**AddMaterialSteel**](MaterialApi.md#addmaterialsteel) | **POST** /api/1/projects/{projectId}/materials/steel | Add material to the project |
| [**AddMaterialWeld**](MaterialApi.md#addmaterialweld) | **POST** /api/1/projects/{projectId}/materials/welding | Add material to the project |
| [**GetAllMaterials**](MaterialApi.md#getallmaterials) | **GET** /api/1/projects/{projectId}/materials | Get materials which are used in the project projectId |
| [**GetBlotGradeMaterials**](MaterialApi.md#getblotgradematerials) | **GET** /api/1/projects/{projectId}/materials/bolt-grade | Get materials which are used in the project projectId |
| [**GetBoltAssemblies**](MaterialApi.md#getboltassemblies) | **GET** /api/1/projects/{projectId}/materials/bolt-assemblies | Get bolt assemblies which are used in the project projectId |
| [**GetConcreteMaterials**](MaterialApi.md#getconcretematerials) | **GET** /api/1/projects/{projectId}/materials/concrete | Get materials which are used in the project projectId |
| [**GetCrossSections**](MaterialApi.md#getcrosssections) | **GET** /api/1/projects/{projectId}/materials/cross-sections | Get cross sections which are used in the project projectId |
| [**GetSteelMaterials**](MaterialApi.md#getsteelmaterials) | **GET** /api/1/projects/{projectId}/materials/steel | Get materials which are used in the project projectId |
| [**GetWeldingMaterials**](MaterialApi.md#getweldingmaterials) | **GET** /api/1/projects/{projectId}/materials/welding | Get materials which are used in the project projectId |

<a id="addboltassembly"></a>
# **AddBoltAssembly**
> ConMprlElement AddBoltAssembly (Guid projectId, ConMprlElement conMprlElement = null)

Add bolt assembly to the project

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class AddBoltAssemblyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service
            var conMprlElement = new ConMprlElement(); // ConMprlElement | Definition of a new bolt assemby to be added to the project (optional) 

            try
            {
                // Add bolt assembly to the project
                ConMprlElement result = apiInstance.AddBoltAssembly(projectId, conMprlElement);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.AddBoltAssembly: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AddBoltAssemblyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Add bolt assembly to the project
    ApiResponse<ConMprlElement> response = apiInstance.AddBoltAssemblyWithHttpInfo(projectId, conMprlElement);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.AddBoltAssemblyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conMprlElement** | [**ConMprlElement**](ConMprlElement.md) | Definition of a new bolt assemby to be added to the project | [optional]  |

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

<a id="addcrosssection"></a>
# **AddCrossSection**
> ConMprlCrossSection AddCrossSection (Guid projectId, ConMprlCrossSection conMprlCrossSection = null)

Add cross section to the project

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class AddCrossSectionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service
            var conMprlCrossSection = new ConMprlCrossSection(); // ConMprlCrossSection | Definition of a new cross-section to be added to the project (optional) 

            try
            {
                // Add cross section to the project
                ConMprlCrossSection result = apiInstance.AddCrossSection(projectId, conMprlCrossSection);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.AddCrossSection: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AddCrossSectionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Add cross section to the project
    ApiResponse<ConMprlCrossSection> response = apiInstance.AddCrossSectionWithHttpInfo(projectId, conMprlCrossSection);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.AddCrossSectionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conMprlCrossSection** | [**ConMprlCrossSection**](ConMprlCrossSection.md) | Definition of a new cross-section to be added to the project | [optional]  |

### Return type

[**ConMprlCrossSection**](ConMprlCrossSection.md)

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

<a id="addmaterialboltgrade"></a>
# **AddMaterialBoltGrade**
> ConMprlElement AddMaterialBoltGrade (Guid projectId, ConMprlElement conMprlElement = null)

Add material to the project

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class AddMaterialBoltGradeExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service
            var conMprlElement = new ConMprlElement(); // ConMprlElement | Definition of a new material to be added to the project (optional) 

            try
            {
                // Add material to the project
                ConMprlElement result = apiInstance.AddMaterialBoltGrade(projectId, conMprlElement);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.AddMaterialBoltGrade: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AddMaterialBoltGradeWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Add material to the project
    ApiResponse<ConMprlElement> response = apiInstance.AddMaterialBoltGradeWithHttpInfo(projectId, conMprlElement);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.AddMaterialBoltGradeWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conMprlElement** | [**ConMprlElement**](ConMprlElement.md) | Definition of a new material to be added to the project | [optional]  |

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

<a id="addmaterialconcrete"></a>
# **AddMaterialConcrete**
> ConMprlElement AddMaterialConcrete (Guid projectId, ConMprlElement conMprlElement = null)

Add material to the project

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class AddMaterialConcreteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service
            var conMprlElement = new ConMprlElement(); // ConMprlElement | Definition of a new material to be added to the project (optional) 

            try
            {
                // Add material to the project
                ConMprlElement result = apiInstance.AddMaterialConcrete(projectId, conMprlElement);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.AddMaterialConcrete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AddMaterialConcreteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Add material to the project
    ApiResponse<ConMprlElement> response = apiInstance.AddMaterialConcreteWithHttpInfo(projectId, conMprlElement);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.AddMaterialConcreteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conMprlElement** | [**ConMprlElement**](ConMprlElement.md) | Definition of a new material to be added to the project | [optional]  |

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

<a id="addmaterialsteel"></a>
# **AddMaterialSteel**
> ConMprlElement AddMaterialSteel (Guid projectId, ConMprlElement conMprlElement = null)

Add material to the project

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class AddMaterialSteelExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service
            var conMprlElement = new ConMprlElement(); // ConMprlElement | Definition of a new material to be added to the project (optional) 

            try
            {
                // Add material to the project
                ConMprlElement result = apiInstance.AddMaterialSteel(projectId, conMprlElement);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.AddMaterialSteel: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AddMaterialSteelWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Add material to the project
    ApiResponse<ConMprlElement> response = apiInstance.AddMaterialSteelWithHttpInfo(projectId, conMprlElement);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.AddMaterialSteelWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conMprlElement** | [**ConMprlElement**](ConMprlElement.md) | Definition of a new material to be added to the project | [optional]  |

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

<a id="addmaterialweld"></a>
# **AddMaterialWeld**
> ConMprlElement AddMaterialWeld (Guid projectId, ConMprlElement conMprlElement = null)

Add material to the project

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class AddMaterialWeldExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service
            var conMprlElement = new ConMprlElement(); // ConMprlElement | Definition of a new material to be added to the project (optional) 

            try
            {
                // Add material to the project
                ConMprlElement result = apiInstance.AddMaterialWeld(projectId, conMprlElement);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.AddMaterialWeld: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AddMaterialWeldWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Add material to the project
    ApiResponse<ConMprlElement> response = apiInstance.AddMaterialWeldWithHttpInfo(projectId, conMprlElement);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.AddMaterialWeldWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |
| **conMprlElement** | [**ConMprlElement**](ConMprlElement.md) | Definition of a new material to be added to the project | [optional]  |

### Return type

[**ConMprlElement**](ConMprlElement.md)

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

<a id="getallmaterials"></a>
# **GetAllMaterials**
> List&lt;Object&gt; GetAllMaterials (Guid projectId)

Get materials which are used in the project projectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetAllMaterialsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service

            try
            {
                // Get materials which are used in the project projectId
                List<Object> result = apiInstance.GetAllMaterials(projectId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.GetAllMaterials: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetAllMaterialsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get materials which are used in the project projectId
    ApiResponse<List<Object>> response = apiInstance.GetAllMaterialsWithHttpInfo(projectId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.GetAllMaterialsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |

### Return type

**List<Object>**

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

<a id="getblotgradematerials"></a>
# **GetBlotGradeMaterials**
> List&lt;Object&gt; GetBlotGradeMaterials (Guid projectId)

Get materials which are used in the project projectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetBlotGradeMaterialsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service

            try
            {
                // Get materials which are used in the project projectId
                List<Object> result = apiInstance.GetBlotGradeMaterials(projectId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.GetBlotGradeMaterials: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetBlotGradeMaterialsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get materials which are used in the project projectId
    ApiResponse<List<Object>> response = apiInstance.GetBlotGradeMaterialsWithHttpInfo(projectId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.GetBlotGradeMaterialsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |

### Return type

**List<Object>**

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

<a id="getboltassemblies"></a>
# **GetBoltAssemblies**
> List&lt;Object&gt; GetBoltAssemblies (Guid projectId)

Get bolt assemblies which are used in the project projectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetBoltAssembliesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service

            try
            {
                // Get bolt assemblies which are used in the project projectId
                List<Object> result = apiInstance.GetBoltAssemblies(projectId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.GetBoltAssemblies: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetBoltAssembliesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get bolt assemblies which are used in the project projectId
    ApiResponse<List<Object>> response = apiInstance.GetBoltAssembliesWithHttpInfo(projectId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.GetBoltAssembliesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |

### Return type

**List<Object>**

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

<a id="getconcretematerials"></a>
# **GetConcreteMaterials**
> List&lt;Object&gt; GetConcreteMaterials (Guid projectId)

Get materials which are used in the project projectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetConcreteMaterialsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service

            try
            {
                // Get materials which are used in the project projectId
                List<Object> result = apiInstance.GetConcreteMaterials(projectId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.GetConcreteMaterials: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetConcreteMaterialsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get materials which are used in the project projectId
    ApiResponse<List<Object>> response = apiInstance.GetConcreteMaterialsWithHttpInfo(projectId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.GetConcreteMaterialsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |

### Return type

**List<Object>**

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

<a id="getcrosssections"></a>
# **GetCrossSections**
> List&lt;Object&gt; GetCrossSections (Guid projectId)

Get cross sections which are used in the project projectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetCrossSectionsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service

            try
            {
                // Get cross sections which are used in the project projectId
                List<Object> result = apiInstance.GetCrossSections(projectId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.GetCrossSections: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetCrossSectionsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get cross sections which are used in the project projectId
    ApiResponse<List<Object>> response = apiInstance.GetCrossSectionsWithHttpInfo(projectId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.GetCrossSectionsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |

### Return type

**List<Object>**

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

<a id="getsteelmaterials"></a>
# **GetSteelMaterials**
> List&lt;Object&gt; GetSteelMaterials (Guid projectId)

Get materials which are used in the project projectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetSteelMaterialsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service

            try
            {
                // Get materials which are used in the project projectId
                List<Object> result = apiInstance.GetSteelMaterials(projectId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.GetSteelMaterials: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetSteelMaterialsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get materials which are used in the project projectId
    ApiResponse<List<Object>> response = apiInstance.GetSteelMaterialsWithHttpInfo(projectId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.GetSteelMaterialsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |

### Return type

**List<Object>**

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

<a id="getweldingmaterials"></a>
# **GetWeldingMaterials**
> List&lt;Object&gt; GetWeldingMaterials (Guid projectId)

Get materials which are used in the project projectId

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.ConnectionApi.Model;

namespace Example
{
    public class GetWeldingMaterialsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new MaterialApi(config);
            var projectId = "projectId_example";  // Guid | The unique identifier of the opened project in the ConnectionRestApi service

            try
            {
                // Get materials which are used in the project projectId
                List<Object> result = apiInstance.GetWeldingMaterials(projectId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MaterialApi.GetWeldingMaterials: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetWeldingMaterialsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get materials which are used in the project projectId
    ApiResponse<List<Object>> response = apiInstance.GetWeldingMaterialsWithHttpInfo(projectId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MaterialApi.GetWeldingMaterialsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | The unique identifier of the opened project in the ConnectionRestApi service |  |

### Return type

**List<Object>**

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

