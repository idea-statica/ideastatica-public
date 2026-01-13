# IdeaStatiCa.ConnectionApi

This library provides the core client components for interacting with the IDEA StatiCa Connection REST API. It includes factory classes to manage the API service lifecycle and a unified client interface for all API endpoints.

## Key Components

### ConnectionApiClient

The `ConnectionApiClient` is the main client class that provides access to all Connection REST API endpoints through a unified interface. It wraps all API endpoint controllers into a single, easy-to-use client for .NET applications.

#### Features

- **Unified API Access**: All API endpoints are accessible through a single client instance
- **IDisposable Pattern**: Supports both synchronous and asynchronous disposal (implements `IDisposable` and `IAsyncDisposable`)
- **Active Project Tracking**: Maintains the currently active project ID for seamless operations
- **Automatic Resource Cleanup**: Properly closes projects and cleans up resources on disposal

#### Available API Endpoints

The client provides access to the following API endpoints:

- **Calculation**: CBFEM calculations and results
- **Connection**: Connection data and topology management  
- **Export**: Export functionality for connections
- **LoadEffect**: Load effects management
- **Material**: Material properties and settings
- **Member**: Member data and operations
- **Operation**: Connection operations (cuts, welds, bolts, etc.)
- **Parameter**: Connection parameters
- **Presentation**: Visualization and presentation settings
- **Project**: Project lifecycle management (open, save, close)
- **Report**: Report generation
- **Template**: Connection templates
- **Conversion**: Conversion operations
- **Settings**: Application settings
- **ConnectionLibrary**: Connection library operations

#### Usage

The `ConnectionApiClient` is typically not instantiated directly. Instead, use one of the factory classes (`ConnectionApiServiceAttacher` or `ConnectionApiServiceRunner`) to create instances:

```csharp
// Using ConnectionApiServiceRunner (automatically starts the service)
using (var serviceRunner = new ConnectionApiServiceRunner(@"C:\Program Files\IDEA StatiCa\StatiCa 25.1"))
{
    using (var client = await serviceRunner.CreateApiClient())
    {
        // Access any API endpoint through the client
        var projectData = await client.Project.OpenProjectAsync("myproject.ideaCon");
        var connections = await client.Connection.GetConnectionsAsync(projectData.ProjectId);
        
        // Perform calculations
        var connectionIds = connections.Select(c => c.Id).ToList();
        var results = await client.Calculation.CalculateAsync(projectData.ProjectId, connectionIds);
        
        await client.Project.CloseProjectAsync(projectData.ProjectId);
    }
}
```

---

### ConnectionApiServiceAttacher

The `ConnectionApiServiceAttacher` is a factory class for creating `ConnectionApiClient` instances that connect to an **already running** IDEA StatiCa Connection REST API service.

#### When to Use

Use this class when:
- The REST API service is already running (started manually or by another process)
- You know the URL where the service is hosted
- You want to connect multiple clients to the same service instance
- You need to attach to a service running on a specific port

#### Constructor

```csharp
public ConnectionApiServiceAttacher(string baseUrl)
```

**Parameters:**
- `baseUrl`: The base URL of the running REST API service (e.g., `"http://localhost:5000/"`)

#### Methods

```csharp
public async Task<IConnectionApiClient> CreateApiClient()
```

Creates and returns a new `ConnectionApiClient` instance connected to the specified service.

**Returns:** An `IConnectionApiClient` instance ready to use

#### Example Usage

```csharp
// Assumes the service is already running on port 5000
var attacher = new ConnectionApiServiceAttacher("http://localhost:5000/");

using (var client = await attacher.CreateApiClient())
{
    // Use the client to interact with the API
    var version = await client.ClientApi.GetVersionAsync();
    Console.WriteLine($"Connected to API version: {version}");
    
    // Open a project and perform operations
    var projectData = await client.Project.OpenProjectAsync("connection.ideaCon");
    // ... perform operations ...
    await client.Project.CloseProjectAsync(projectData.ProjectId);
}
```

#### Starting the Service Manually

To manually start the REST API service:

**Using Command Line:**
```console
cd "C:\Program Files\IDEA StatiCa\StatiCa 25.1"
IdeaStatiCa.ConnectionRestApi.exe -port:5000
```

**Parameters:**
- `-port`: (Optional) The port number to run the service on. Default is 5000.

---

### ConnectionApiServiceRunner

The `ConnectionApiServiceRunner` is a factory class that **automatically starts** the IDEA StatiCa Connection REST API service and creates client instances connected to it. This is the recommended approach for most applications.

#### When to Use

Use this class when:
- You want automatic service lifecycle management (start and stop)
- You need a self-contained solution without manual service management
- You want to ensure the service is properly cleaned up when done
- You're building an automated workflow or batch processing application

#### Constructor

```csharp
public ConnectionApiServiceRunner(string setupDir)
```

**Parameters:**
- `setupDir`: The directory path where `IdeaStatiCa.ConnectionRestApi.exe` is located  
  (e.g., `@"C:\Program Files\IDEA StatiCa\StatiCa 25.1"`)

#### Methods

```csharp
public async Task<IConnectionApiClient> CreateApiClient()
```

Starts the REST API service (if not already running) and returns a new `ConnectionApiClient` instance connected to it.

**Returns:** An `IConnectionApiClient` instance ready to use

#### Lifecycle Management

The `ConnectionApiServiceRunner` implements `IDisposable` and automatically manages the service process:

- **On `CreateApiClient()`**: 
  - Finds an available port
  - Starts `IdeaStatiCa.ConnectionRestApi.exe` with the selected port
  - Waits for the service to be ready (up to 50 seconds)
  - Returns a connected client

- **On `Dispose()`**:
  - Terminates the REST API service process
  - Releases all resources

#### Example Usage

**Basic Usage:**

```csharp
string ideaStatiCaDir = @"C:\Program Files\IDEA StatiCa\StatiCa 25.1";

using (var serviceRunner = new ConnectionApiServiceRunner(ideaStatiCaDir))
{
    using (var client = await serviceRunner.CreateApiClient())
    {
        // The service is now running and the client is connected
        
        var projectData = await client.Project.OpenProjectAsync("myproject.ideaCon");
        
        // Perform operations
        var connections = await client.Connection.GetConnectionsAsync(projectData.ProjectId);
        
        await client.Project.CloseProjectAsync(projectData.ProjectId);
    }
    // Client is disposed here
}
// Service is automatically stopped and cleaned up here
```

**Complete Example with Calculation:**

```csharp
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace ConnectionApiExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string ideaConFile = "connection.ideaCon";
            string ideaStatiCaPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.1";
            
            // ServiceRunner automatically starts and manages the REST API service
            using (var serviceRunner = new ConnectionApiServiceRunner(ideaStatiCaPath))
            {
                // Create a connected client
                using (var client = await serviceRunner.CreateApiClient())
                {
                    // Open project
                    var projectData = await client.Project.OpenProjectAsync(ideaConFile);
                    Console.WriteLine($"Opened project: {projectData.ProjectId}");
                    
                    if (projectData.Connections.Any())
                    {
                        // Setup calculation parameters - list of connection IDs
                        var connectionIds = projectData.Connections.Select(c => c.Id).ToList();
                        
                        // Run calculation
                        var results = await client.Calculation.CalculateAsync(
                            projectData.ProjectId, 
                            connectionIds);
                        
                        Console.WriteLine($"Calculation completed. Results: {results.Count} connections");
                        
                        // Get detailed results
                        var detailedResults = await client.Calculation.GetResultsAsync(
                            projectData.ProjectId, 
                            connectionIds);
                        
                        for (int i = 0; i < projectData.Connections.Count; i++)
                        {
                            Console.WriteLine($"Connection {projectData.Connections[i].Name}: " +
                                            $"Status = {detailedResults[i].ConnectionCheckRes?.Status}");
                        }
                    }
                    
                    // Close project
                    await client.Project.CloseProjectAsync(projectData.ProjectId);
                    Console.WriteLine("Project closed");
                }
            }
            
            Console.WriteLine("Service stopped and resources cleaned up");
        }
    }
}
```

#### Error Handling

The `ConnectionApiServiceRunner` performs validation and provides detailed error messages:

- **Missing executable**: Throws `FileNotFoundException` if `IdeaStatiCa.ConnectionRestApi.exe` is not found
- **Failed to start**: Throws `InvalidOperationException` if the process cannot start
- **Missing .NET runtime**: Provides guidance about required .NET 8.0 ASP.NET Core runtime
- **No available ports**: Throws `InvalidOperationException` if no ports are available
- **Service startup timeout**: Returns false if service doesn't respond within 50 seconds

---

## Choosing Between ServiceAttacher and ServiceRunner

| Feature | ConnectionApiServiceAttacher | ConnectionApiServiceRunner |
|---------|------------------------------|----------------------------|
| **Service Management** | Connects to existing service | Automatically starts/stops service |
| **Use Case** | Service already running | Automated workflows |
| **Resource Cleanup** | Manual (stop service separately) | Automatic via `Dispose()` |
| **Port Selection** | Manual (specify in URL) | Automatic (finds available port) |
| **Complexity** | Lower (just connection) | Higher (full lifecycle) |
| **Recommended For** | Development/debugging | Production/automation |

**Quick Decision Guide:**
- Use **ServiceAttacher** if you manually started the service or it's managed elsewhere
- Use **ServiceRunner** for automated scripts, batch processing, or production applications

---

## See Also

- [Connection REST API Documentation](../../docs/)
- [API Examples](../../examples/)
- [Getting Started Guide](../../README.md)
