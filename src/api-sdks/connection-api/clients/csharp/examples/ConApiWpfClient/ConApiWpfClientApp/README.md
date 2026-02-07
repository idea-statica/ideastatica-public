# ConApiWpfClientApp

A WPF desktop application demonstrating the IDEA StatiCa Connection API. It provides a graphical interface for opening connection projects, running structural analyses, exporting results, managing templates, and visualizing connections in 3D.

## Architecture

The application follows the **MVVM** (Model-View-ViewModel) pattern with **dependency injection** via `Microsoft.Extensions.DependencyInjection`.

```
ConApiWpfClientApp/
├── App.xaml(.cs)              # Entry point, DI container configuration
├── MainWindow.xaml(.cs)       # Main application window
├── appsettings.json           # Configuration (API endpoint, setup path)
├── Models/
│   ├── ProjectModel.cs        # Currently open project state
│   ├── ConnectionLibraryModel.cs  # Connection library search state
│   └── ExpressionModel.cs     # Expression evaluation input
├── Services/
│   ├── IConnectionApiService.cs   # Main API abstraction (interface)
│   ├── ConnectionApiService.cs    # API implementation wrapping IConnectionApiClient
│   ├── IExpressionProvider.cs     # Expression input provider (interface)
│   ├── ExpressionProvider.cs      # Builds default expressions from member data
│   ├── ITemplateProvider.cs       # Template loading abstraction (interface)
│   ├── TemplateProviderFile.cs    # Loads templates from .contemp files
│   ├── ConnectionLibraryProposer.cs # Browses connection library for templates
│   ├── JsonEditorService.cs       # Generic JSON object editor
│   ├── TextEditorService.cs       # Plain text editor
│   └── TemplateMappingSetter.cs   # Template member/cross-section mapping editor
├── ViewModels/
│   ├── ViewModelBase.cs           # Base class (ObservableObject)
│   ├── MainWindowViewModel.cs     # Main VM with all commands
│   ├── ConnectionViewModel.cs     # Wraps ConConnection for data binding
│   ├── JsonEditorViewModel.cs     # JSON editor dialog VM
│   ├── ConnectionLibraryViewModel.cs  # Connection library browser VM
│   └── ProposedCdiViewModel.cs    # Connection design item with lazy-loaded details
├── Views/
│   ├── JsonEditorWindow.xaml(.cs)          # JSON/text editor dialog
│   ├── ConnectionLibraryWindow.xaml(.cs)   # Connection library browser dialog
│   ├── DesignItemsView.xaml(.cs)           # Design item list control
│   └── DesignItemDetailView.xaml(.cs)      # Design item detail control
└── Tools/
    └── JsonTools.cs               # JSON formatting utilities
```

### Layer responsibilities

| Layer | Responsibility |
|-------|---------------|
| **Model** | Domain data (`ProjectModel` holds the `ConProject` returned by the API). No UI logic. |
| **ViewModel** | UI state (`IsBusy`, `Connections`, `SelectedConnection`, `OutputText`), relay commands, and orchestration of service calls. Uses CommunityToolkit.Mvvm source generators (`[ObservableProperty]`, `[RelayCommand]`). |
| **View** | XAML only. Data-binds to ViewModel properties and commands. No code-behind logic beyond `InitializeComponent`. |
| **Services** | All business logic and API communication. `IConnectionApiService` is the main abstraction; other services handle templates, expressions, JSON editing, and file dialogs. |

### Dependency injection

All services, models, and view models are registered in `App.xaml.cs`:

| Registration | Lifetime | Purpose |
|---|---|---|
| `IConfiguration` | Singleton | `appsettings.json` + environment variables |
| `IPluginLogger` | Singleton | Serilog-based diagnostic logging |
| `IConnectionApiService` / `ConnectionApiService` | Singleton | Connection API wrapper |
| `ProjectModel` | Singleton | Current project state |
| `MainWindowViewModel` | Singleton | Main view model |
| `ISceneController` / `SceneController` | Singleton | 3D scene rendering |
| `IFileDialogService` / `FileDialogService` | Transient | Open/save file dialogs |
| `JsonEditorViewModel` | Transient | JSON editor instances |
| `IomEditorWindowViewModel` | Singleton | IOM generator dialog |

The ViewModel never instantiates services or dialogs directly -- everything is injected through the constructor.

## Configuration

Edit `appsettings.json` to match your environment:

```json
{
  "CONNECTION_API_ENDPOINT": "http://localhost:5000",
  "CONNECTION_API_RUNSERVER": "true",
  "IdeaStatiCaSetupPath": "C:\\Program Files\\IDEA StatiCa\\StatiCa 25.1"
}
```

| Setting | Description |
|---------|-------------|
| `CONNECTION_API_ENDPOINT` | REST API base URL when attaching to an existing service |
| `CONNECTION_API_RUNSERVER` | `"true"` to launch a new API server process, `"false"` to attach to `CONNECTION_API_ENDPOINT` |
| `IdeaStatiCaSetupPath` | Path to the IDEA StatiCa installation directory (used when `CONNECTION_API_RUNSERVER` is `"true"`) |

## Available commands

The main window toolbar exposes all operations, each backed by an async relay command with CanExecute guards:

### Connection lifecycle

| Command | Description |
|---------|-------------|
| **Start service** | Connect to the API (start new server or attach to endpoint) |
| **Open Project** | Open an `.ideacon` project file |
| **Import IOM** | Import an `.iom` or `.xml` Open Model file |
| **Generate connection** | Open the IOM generator dialog to define connection geometry |
| **Close Project** | Close the current project |
| **Download** | Save the current project to a file |

### Analysis and results

| Command | Description |
|---------|-------------|
| **Calculate** | Run structural analysis (Stress-Strain or Stiffness, with optional buckling) |
| **Get Members** | Retrieve beams and columns as JSON |
| **Get Operations** | Retrieve manufacturing operations (bolts, welds, etc.) |
| **Delete Operations** | Remove all operations from the selected connection |
| **Get topology** | Retrieve geometry and connectivity data |
| **Scene3D Data** | Retrieve raw 3D scene JSON |
| **Evaluate Expression** | Evaluate a parametric expression against the connection |

### Export and reporting

| Command | Description |
|---------|-------------|
| **Report PDF / Word** | Generate a calculation report |
| **Export IOM / IFC** | Export the connection to IOM XML or IFC format |

### Templates

| Command | Description |
|---------|-------------|
| **Create template** | Export the selected connection as a `.contemp` template |
| **Apply template** | Load a template from file and apply it to the selected connection |
| **Apply from CL** | Browse the connection library, select a design, and apply it |

### Other

| Command | Description |
|---------|-------------|
| **Get/Update Settings** | Read and write project-level settings as JSON |
| **Update loading** | Edit load effects (forces and moments) via a JSON editor |
| **Edit Parameters** | Edit user-defined connection parameters |
| **WeldSizing** | Run weld pre-design using the selected sizing method |
| **Logs** | Open the IDEA StatiCa logs folder in Explorer |
| **Diagnostics** | Open `IdeaDiagnostics.config` in Notepad |

## Technology stack

- **.NET 8.0** (WPF, `net8.0-windows`)
- **CommunityToolkit.Mvvm** -- source-generated `ObservableProperty`, `RelayCommand`
- **Microsoft.Extensions.DependencyInjection** -- constructor injection
- **Microsoft.Extensions.Configuration** -- JSON + environment variable config
- **Newtonsoft.Json** -- JSON serialization/formatting
- **IdeaStatiCa.ConnectionApi** -- REST client for the Connection API
- **IdeaStatiCa.ConRestApiClientUI** -- 3D scene rendering (WebGL)
- **IdeaStatiCa.PluginLogger** -- Serilog logging facade
