# IomEditorWindow - Dialog for Editing Connection Input

## Overview

The `IomEditorWindow` is a WPF dialog window that provides an interface for editing connection input parameters using the MVVM pattern. It **reuses the existing `MainWindowViewModel`** and `ConnectionInputView` to provide a consistent editing experience.

## Key Design Decision

Instead of duplicating the connection input editing logic, the `IomEditorWindow` **wraps and reuses** the `MainWindowViewModel`. This provides several benefits:
- **No code duplication** - Single source of truth for connection editing logic
- **Consistency** - Same editing experience as the main window
- **Maintainability** - Changes to MainWindowViewModel automatically benefit the dialog
- **Testability** - Reuse existing tests for MainWindowViewModel

## Architecture

### MVVM Components

```
Views/
  ??? IomEditorWindow.xaml / .cs        - Dialog wrapper window
  ??? ConnectionInputView.xaml / .cs    - Reused connection editor (JSON-based)
  
ViewModels/
  ??? IomEditorViewModel.cs             - Dialog ViewModel (wraps MainWindowViewModel)
  ??? MainWindowViewModel.cs       - Reused main ViewModel
```

## Usage

### Basic Usage

```csharp
using ConnectionIomGenerator.UI.Views;
using ConnectionIomGenerator.UI.Models;
using ConnectionIomGenerator.UI.Services;
using IdeaStatiCa.Plugin;

// Create required services
IPluginLogger logger = ...; // your logger implementation
IIomService iomService = new IomService(logger);
IFileDialogService fileDialogService = new FileDialogService();

// Create and show the dialog
var editorWindow = new IomEditorWindow(logger, iomService, fileDialogService)
{
    Owner = Application.Current.MainWindow
};

bool? result = editorWindow.ShowDialog();

if (result == true)
{
    // User clicked OK - get the result model
    IomGeneratorModel model = editorWindow.ResultModel;
    
    // Access the connection input
    var connectionInput = model.ConnectionInput;
    
    // Use the data...
    Console.WriteLine($"Material: {connectionInput.Material}");
    foreach (var member in connectionInput.Members)
    {
 Console.WriteLine($"Member: {member.Name}, CS: {member.CrossSection}");
    }
}
```

### Usage with Initial Data

```csharp
// Create a model with initial data
var initialModel = new IomGeneratorModel
{
    ConnectionInput = ConnectionInput.GetDefaultECEN()
};

// Open editor with initial data
var editorWindow = new IomEditorWindow(
    initialModel, 
    logger, 
    iomService, 
    fileDialogService)
{
    Owner = Application.Current.MainWindow
};

bool? result = editorWindow.ShowDialog();

if (result == true)
{
    // Get updated model
    var updatedModel = editorWindow.ResultModel;
    // ... use the updated data
}
```

## Constructor Parameters

The dialog requires dependency injection of services:

| Parameter | Type | Description |
|-----------|------|-------------|
| `logger` | `IPluginLogger` | Logger for tracking operations |
| `iomService` | `IIomService` | Service for IOM generation and serialization |
| `fileDialogService` | `IFileDialogService` | Service for file open/save dialogs |
| `model` | `IomGeneratorModel` | (Optional) Initial model data |

## Dialog Features

The dialog provides a JSON-based editor for connection input:
- **Connection Definition (JSON)**: Edit material and members
- **Loading Definition (JSON)**: Optional loading configuration
- **Generate Loading Button**: Auto-generate default loading
- **Validation**: JSON format validation before closing
- **Status Display**: Real-time status messages

## How It Works

1. `IomEditorViewModel` wraps `MainWindowViewModel`
2. `ConnectionInputView` is bound to the wrapped `MainViewModel` property
3. User edits connection input as JSON (same as main application)
4. On OK click:
   - Validates JSON format
   - Validates connection input data (material, members)
   - Returns `IomGeneratorModel` via `ResultModel` property
5. On Cancel: Returns `null`

## Validation

The dialog performs validation before allowing OK:

- **Connection Definition**: Must be valid JSON
- **Material**: Must not be empty
- **Members**: At least one member required
- **Member Properties**:
  - Name must not be empty
  - Cross-section must not be empty

Validation errors are displayed in a yellow banner above the buttons.

## Reused Components

### MainWindowViewModel
Provides all the core functionality:
- `ConnectionDefinitionJson` - JSON representation of connection
- `LoadingDefinitionJson` - JSON representation of loading
- `Status` - Status message display
- `GenerateLoadingCommand` - Command to generate default loading

### ConnectionInputView
The same view used in the main window, which provides:
- Side-by-side JSON editors for connection and loading
- Generate IOM button
- Generate Loading button
- Save IOM button
- Status display

## Benefits of Reusing MainWindowViewModel

1. **Consistency**: Same editing experience everywhere
2. **DRY Principle**: Don't Repeat Yourself - single source of logic
3. **Maintainability**: Fix bugs once, benefit everywhere
4. **Feature Parity**: New features in MainWindowViewModel automatically available
5. **Testing**: Test once, works everywhere

## Example Integration

```csharp
public class MyApplication
{
    private IPluginLogger _logger;
    private IIomService _iomService;
    private IFileDialogService _fileDialogService;

    public void Initialize()
    {
 _logger = new ConsoleLogger();
        _iomService = new IomService(_logger);
      _fileDialogService = new FileDialogService();
    }

  public void EditConnection()
  {
        // Create initial model
        var model = new IomGeneratorModel
     {
        ConnectionInput = ConnectionInput.GetDefaultECEN()
        };
        
        // Open editor dialog
        var editor = new IomEditorWindow(
     model, 
            _logger, 
            _iomService, 
         _fileDialogService);
  
        bool? result = editor.ShowDialog();
        
        if (result == true)
  {
            // User clicked OK - use the result
       var connectionInput = editor.ResultModel.ConnectionInput;
  
            // Generate IOM
            var iomContainer = await _iomService.GenerateIomAsync(
                connectionInput,
    editor.ResultModel.Loading);
         
            // Save IOM
string xml = _iomService.SerializeToXml(iomContainer.OpenModel);
        }
    }
}
```

## Dependencies

- .NET 8.0-windows
- WPF
- CommunityToolkit.Mvvm
- ConnectionIomGenerator (core library)
- ConnectionIomGenerator.UI.Services (IIomService, IFileDialogService)

## See Also

- `MainWindowViewModel` - The reused ViewModel
- `ConnectionInputView` - The reused View
- `IomService` - Implementation of IIomService
- `FileDialogService` - Implementation of IFileDialogService
