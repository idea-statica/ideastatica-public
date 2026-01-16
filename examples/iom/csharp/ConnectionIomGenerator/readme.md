# Connection IOM Generator

A WPF desktop application for generating IDEA StatiCa Open Model (IOM) connection projects from JSON-based structural definitions. This tool allows structural engineers to quickly create steel connection models by defining members and loading in a simple JSON format, which is then converted to IOM for use in IDEA StatiCa Connection analysis.

## Overview

The Connection IOM Generator streamlines the process of creating connection models for IDEA StatiCa by:
- Accepting structural definitions in a user-friendly JSON format
- Automatically generating 3D geometry with proper coordinate systems
- Creating IOM (IDEA Open Model) compatible with IDEA StatiCa Connection
- Supporting both ended and continuous members
- Including load cases and internal forces for connection design
- Providing immediate XML preview of the generated IOM

## Architecture

The solution consists of three main projects:

### 1. ConnectionIomGenerator (Core Library)
**Target Framework:** .NET 8.0

The core business logic library containing:

#### Key Components

- **Model/** - Input data structures
  - `ConnectionInput.cs` - Defines member geometry and properties
- `Member.cs` - Individual member specifications (cross-section, orientation, continuity)
  - `LoadingInput.cs` - Load case definitions
  - `LoadCase.cs` - Individual load case data
  - `LoadImpulse.cs` - Internal forces at member positions

- **Fea/** - Finite Element Analysis model representation
  - `FeaModel.cs` - Intermediate FEA model containing nodes, elements, members, and connection points

- **Service/** - Core generation services
  - `IIomGenerator.cs` - Interface for IOM generation
  - `IomGenerator.cs` - Main IOM generation orchestrator using BimImporter
  - `FeaGenerator.cs` - Generates FEA model from connection input
  - `FeaModelWrapper.cs` - Wraps FeaModel to implement IIdeaModel interface
  - `InMemoryProject.cs` - In-memory project for ID mapping during import

**Dependencies:**
- IdeaRS.OpenModel - IDEA StatiCa Open Model definitions
- IdeaStatiCa.BimApiLink - BIM API integration for model conversion
- IdeaStatiCa.BimApi - BIM API interfaces

### 2. ConnectionIomGenerator.UI (UI Library)
**Target Framework:** .NET 8.0 Windows (WPF)

User interface library providing:

#### Key Components

- **ViewModels/**
  - `MainWindowViewModel.cs` - Main view model with commands for IOM generation
  - `ViewModelBase.cs` - Base class for MVVM pattern

- **Views/**
  - `ConnectionInputView.xaml` - Main UI with split-panel layout

- **Services/**
  - `IIomService.cs` - Interface for IOM operations
  - `IomService.cs` - IOM generation, serialization, and file operations
  - `IFileDialogService.cs` - File dialog abstraction
  - `FileDialogService.cs` - Windows file dialog implementation

- **Tools/**
  - `JsonTools.cs` - JSON serialization utilities

**UI Features:**
- **Left Panel (Split View):**
  - Connection Definition (JSON) - Define members and geometry
  - Loading Definition (JSON) - Define load cases and forces
  - Message/Log Area - Status and error messages

- **Right Panel:**
  - Generated IOM XML preview

- **Toolbar:**
  - **Generate IOM** - Creates IOM from JSON input
  - **Generate Loading** - Auto-generates simple loading for current geometry
  - **Save IOM** - Saves IOM to XML file

**Dependencies:**
- ConnectionIomGenerator (core library)
- CommunityToolkit.Mvvm - MVVM helpers
- Microsoft.Extensions.DependencyInjection - Dependency injection
- Newtonsoft.Json - JSON serialization

### 3. ConnectionIomGeneratorApp (Application Entry Point)
**Target Framework:** .NET 8.0 Windows (WPF)

The executable WPF application that:
- Configures dependency injection container
- Registers services and view models
- Initializes logging via IdeaStatiCa.PluginLogger
- Launches the main window

**Dependencies:**
- ConnectionIomGenerator.UI
- ConnectionIomGenerator
- IdeaStatiCa.PluginLogger - Structured logging with Serilog
- Microsoft.Extensions.Configuration - Configuration management
- Microsoft.Extensions.DependencyInjection - IoC container

## How It Works

### 1. Input Format

The application accepts two JSON inputs:

#### Connection Definition
```json
{
  "Material": "S 355",
  "Members": [
    {
      "Id": 1,
      "Name": "COL",
      "CrossSection": "HEA260",
      "Direction": 0,
      "Pitch": 90,
      "Rotation": 0,
      "IsContinuous": true
    },
    {
      "Id": 2,
      "Name": "M1",
      "CrossSection": "IPE360",
      "Direction": 0,
      "Pitch": 0,
      "Rotation": 0,
    "IsContinuous": false
    }
  ]
}
```

**Member Properties:**
- `Id` - Unique member identifier
- `Name` - Member name
- `CrossSection` - Cross-section name (must exist in IDEA StatiCa library)
- `Direction` - Rotation around Z-axis in degrees
- `Pitch` - Rotation around Y-axis in degrees (0° = horizontal, 90° = vertical)
- `Rotation` - Roll rotation in degrees
- `IsContinuous` - If true, member extends through connection point; if false, member ends at connection

#### Loading Definition (Optional)
```json
{
  "LoadCases": [
    {
      "Id": 1,
   "Name": "LC1",
      "LoadImpulses": [
      {
  "MemberId": 1,
          "Position": 0,
          "N": 100000,
          "Vy": 5000,
"Vz": 0,
          "Mx": 0,
          "My": 0,
          "Mz": 10000
        }
      ]
    }
  ]
}
```

**Load Properties:**
- `MemberId` - References member ID from connection definition
- `Position` - Element index (0 for first element, 1 for second if continuous)
- `N` - Axial force (N)
- `Vy`, `Vz` - Shear forces (N)
- `Mx`, `My`, `Mz` - Moments (N·m)

### 2. Generation Workflow

The IOM generation follows this multi-stage process:

```
User Input (JSON)
    ?
FeaGenerator.Generate()
    ?
FeaModel (Intermediate representation)
?? Materials (by name)
    ?? Nodes (3D points)
    ?? CrossSections (by name)
    ?? LineSegments (geometry)
    ?? Elements1D
    ?? Members1D
  ?? ConnectionPoints
    ?? Loading
    ?
FeaModelWrapper (IIdeaModel adapter)
    ?
BimImporter.ImportConnections()
    ?
OpenModel (IOM)
 ?? OriginSettings
    ?? Point3D (nodes)
    ?? MatSteel
    ?? CrossSection
    ?? Member1D
    ?? ConnectionPoint
    ?? Connections
    ?
OpenModelResult (Loading)
    ?? ResultOnMembers
    ?
OpenModelContainer
```

#### Stage 1: FEA Model Generation (`FeaGenerator`)

1. **Material Creation** - Creates a single steel material by name
2. **Cross-Section Mapping** - Maps unique cross-section names from input
3. **Geometry Calculation** - For each member:
   - Calculates direction vector from Direction and Pitch angles
   - Creates coordinate system (LCS) with proper orientation
   - Places member 3 meters from connection origin
   - For continuous members: creates two elements (both sides of connection)
   - For ended members: creates one element
4. **Node Creation** - Creates nodes at connection point and member ends
5. **Element Creation** - Creates 1D elements with geometry and cross-sections
6. **Connection Point** - Aggregates all members at origin with connectivity info
7. **Load Cases** - Creates load case objects if loading input provided

#### Stage 2: BIM Import (`IomGenerator`)

1. **Wrapping** - FeaModel wrapped in FeaModelWrapper to implement BIM API interfaces
2. **BIM Importer** - Uses IdeaStatiCa.BimImporter to convert to OpenModel
3. **ID Mapping** - InMemoryProject tracks ID correspondence between FEA and IOM
4. **Loading Assignment** - If loading provided:
 - Maps load cases from FEA to IOM using BIM IDs
   - Maps members and elements
   - Creates ResultOnMembers with internal forces
   - Assigns forces to element positions (0 = start, 1 = end)

#### Stage 3: Output

- **OpenModel** - Contains structural geometry ready for IDEA StatiCa
- **OpenModelResult** - Contains loading and results
- **OpenModelContainer** - Combines both for export

### 3. Coordinate Systems

The generator uses a right-handed coordinate system with the connection at origin (0, 0, 0):

- **X-axis** - Points right (default member direction when Direction = 0, Pitch = 0)
- **Y-axis** - Points up
- **Z-axis** - Points toward viewer

**Member Orientation:**
- `Direction` rotates around Z-axis (plan view rotation)
- `Pitch` rotates around Y-axis (elevation - 0° horizontal, 90° vertical)
- Each member extends 3 meters from the connection point
- Continuous members extend 3 meters in both directions

## Usage

### Running the Application

1. **Build** the solution in Visual Studio or using .NET CLI:
   ```
   dotnet build
   ```

2. **Run** ConnectionIomGeneratorApp:
   ```
   dotnet run --project ConnectionIomGeneratorApp
   ```

### Creating a Connection

1. **Define Connection** - Enter JSON in the left-top panel (Connection Definition)
   - Use default ECEN example or create custom definition
   - Specify material and members with orientations

2. **Define Loading** (Optional) - Enter JSON in the left-middle panel
   - Define load cases with internal forces on members
   - Or click "Generate Loading" for automatic simple loading

3. **Generate IOM** - Click "Generate IOM" button
   - View generated IOM XML in right panel
   - Check message area for any errors or warnings

4. **Save IOM** - Click "Save IOM" to export
   - Saves as `.xml` file compatible with IDEA StatiCa

### Example: Simple Beam-to-Column Connection

**Connection Definition:**
```json
{
  "Material": "S 355",
  "Members": [
    {
      "Id": 1,
      "Name": "Column",
      "CrossSection": "HEA260",
      "Direction": 0,
      "Pitch": 90,
      "Rotation": 0,
      "IsContinuous": true
    },
    {
      "Id": 2,
      "Name": "Beam",
      "CrossSection": "IPE360",
      "Direction": 0,
   "Pitch": 0,
      "Rotation": 0,
   "IsContinuous": false
    }
  ]
}
```

This creates:
- Vertical continuous column (Pitch = 90°) with HEA260
- Horizontal beam (Pitch = 0°) with IPE360
- Both along X-axis (Direction = 0°)
- Column extends 3m above and 3m below connection
- Beam extends 3m from connection

## Configuration

### appsettings.json

Currently minimal configuration. Future enhancements may include:
- Default material libraries
- Cross-section validation settings
- IOM export options

### Dependency Injection Setup (App.xaml.cs)

The application uses Microsoft.Extensions.DependencyInjection with:

**Registered Services:**
- `IPluginLogger` - Serilog-based logging (singleton)
- `MainWindowViewModel` - Main view model (singleton)
- `IFileDialogService` - File dialog service (transient)
- `IIomService` - IOM operations service (transient)
- `IIomGenerator` - IOM generator (transient)
- `MainWindow` - Main window (transient)

## Technical Details

### BIM API Integration

The application leverages IdeaStatiCa.BimApiLink to convert custom FEA models to IOM:

- **IIdeaModel** - Root interface implemented by FeaModelWrapper
- **IdeaNode** - 3D point representation
- **IdeaMember1D** - Structural member (beam, column)
- **IdeaElement1D** - Member subdivision with geometry
- **IdeaLineSegment3D** - Element geometry between nodes
- **IdeaCrossSectionByName** - Cross-section referenced by name
- **IdeaMaterialByName** - Material referenced by name
- **IdeaConnectionPoint** - Connection with connected members

### Key Algorithms

#### Member Direction Calculation
```csharp
// Convert angles to radians
float pitchRad = pitch * PI / 180f;
float directionRad = direction * PI / 180f;

// Create rotation matrices
Matrix4x4 rotY = CreateFromAxisAngle(Y_AXIS, pitchRad);
Matrix4x4 rotZ = CreateFromAxisAngle(Z_AXIS, directionRad);

// Combined rotation
Matrix4x4 rotation = rotY * rotZ;

// Apply to X-axis to get member direction
Vector3 memberDir = Transform(X_AXIS, rotation);
```

#### ID Mapping Between FEA and IOM
```csharp
// BIM API creates IDs like "LoadCase-1", "Member-2"
// Parser extracts original IDs for correlation
int originalId = ParseBimId("Member-2"); // Returns 2
```

## Output

The application generates:

### OpenModel XML Structure
```xml
<OpenModel>
  <OriginSettings>
    <CountryCode>ECEN</CountryCode>
    <!-- Settings -->
  </OriginSettings>
  <Point3D>
    <!-- Nodes -->
  </Point3D>
  <MatSteel>
    <!-- Materials -->
  </MatSteel>
  <CrossSection>
    <!-- Cross-sections -->
  </CrossSection>
  <Member1D>
    <!-- Members with elements -->
  </Member1D>
  <ConnectionPoint>
    <!-- Connection definitions -->
  </ConnectionPoint>
  <Connections>
    <!-- Connection data -->
  </Connections>
</OpenModel>
```

### Usage in IDEA StatiCa

The generated IOM XML can be:
1. **Imported** into IDEA StatiCa Connection as a new project
2. **Used via API** - See [IOM.SteelFrameDesktop](../../../SteelFrameExample/IOM.SteelFrameDesktop) for API usage examples
3. **Sent to Cloud** - See [IOM.SteelFrameWeb](../../../SteelFrameExample/IOM.SteelFrameWeb) for web service examples

## Related Examples

This application is part of the IDEA StatiCa IOM examples:

- **[IOM.SteelFrameExample](../../../SteelFrameExample/IOM.SteelFrameExample)** - Demonstrates creating complete steel frame IOM with hardcoded geometry
- **[IOM.SteelFrameDesktop](../../../SteelFrameExample/IOM.SteelFrameDesktop)** - Shows how to use Connection API locally with IDEA StatiCa installation
- **[IOM.SteelFrameWeb](../../../SteelFrameExample/IOM.SteelFrameWeb)** - Demonstrates using IDEA Cloud web service to create connections
- **[Connection API Examples](../../../../../src/api-sdks/connection-api/clients/csharp/examples)** - Advanced API usage examples

## Differences from Other IOM Examples

| Feature | ConnectionIomGenerator | SteelFrameExample | SteelFrameDesktop/Web |
|---------|----------------------|-------------------|---------------------|
| Input Method | JSON configuration | Hardcoded in C# | Hardcoded in C# |
| UI | WPF with live preview | Console | Console |
| Focus | Dynamic member configuration | Complete frame example | API integration |
| Complexity | Simple connections | Full frame with loads | API workflow |
| Use Case | Quick connection testing | Learning IOM structure | Production integration |

## Troubleshooting

### Common Issues

**Issue:** Cross-section not found in IDEA StatiCa
- **Solution:** Verify cross-section name matches IDEA StatiCa library exactly (case-sensitive)

**Issue:** Invalid JSON format
- **Solution:** Use JSON validator, check for missing commas, brackets, or quotes

**Issue:** Member orientation incorrect
- **Solution:** Review Direction, Pitch, Rotation values; remember coordinate system conventions

**Issue:** Loading not appearing in connection
- **Solution:** Verify MemberId matches connection definition, check Position index for continuous members

**Issue:** Generated IOM has errors in IDEA StatiCa
- **Solution:** Check message area for warnings, verify material and cross-sections exist in library

## Requirements

- **.NET 8.0 SDK** or later
- **Windows OS** (WPF application)
- **Visual Studio 2022** or later (recommended)
- **IDEA StatiCa** (for importing and using generated IOM files)

## Future Enhancements

Potential improvements:
- 3D visualization of generated connection
- Cross-section library browser
- Material library selection
- Load combination generator
- Template library for common connections
- Export to .ideaCon format directly
- Validation against IDEA StatiCa rules
- Batch processing of multiple connections

## License

This example is part of the IDEA StatiCa Public repository. Please refer to the repository license for usage terms.

## Support

For questions and issues:
- IDEA StatiCa Public repository: https://github.com/idea-statica/ideastatica-public
- IDEA StatiCa documentation: https://www.ideastatica.com/support-center
- Technical support: support@ideastatica.com

## Contributing

Contributions are welcome! Please follow the contribution guidelines in the main repository.

---

**Version:** 1.0  
**Last Updated:** 2024  
**Maintained by:** IDEA StatiCa
