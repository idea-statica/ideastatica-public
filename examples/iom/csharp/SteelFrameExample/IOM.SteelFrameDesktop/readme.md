# IOM.SteelFrameDesktop

This is a desktop console application example that demonstrates how to create IDEA StatiCa Connection projects from IOM (IDEA Open Model) using the Connection API on a machine with IDEA StatiCa installed locally.

## Overview

This example shows how to:
- Generate steel frame models using IOM (IDEA Open Model)
- Use the Connection API to create IDEA StatiCa Connection projects
- Work with the locally installed IDEA StatiCa desktop application
- Save generated projects to the local file system

## Configuration

Before running the application, you need to configure the IDEA StatiCa installation directory:

1. Open [App.config](App.config)
2. Update the `IdeaInstallDir` setting to match your IDEA StatiCa installation path:

```xml
<setting name="IdeaInstallDir" serializeAs="String">
    <value>C:\Program Files\IDEA StatiCa\StatiCa 25.1</value>
</setting>
```

## Available Examples

When you run the application, you can choose from the following steel frame examples:

1. **Steel frame ECEN** - A steel frame designed according to European (Eurocode) standards
2. **Simple frame AUS** - A simple steel frame designed according to Australian standards

## How It Works

The application performs the following steps:

1. **Generate IOM**: Creates an OpenModel object representing the steel structure
2. **Generate Results**: Creates an OpenModelResult object with analysis results (optional)
3. **Create Container**: Combines the model and results into an OpenModelContainer
4. **Export to XML**: Saves the container to an XML file (`example.xml`)
5. **Start API Service**: Automatically starts the Connection API service using `ConnectionApiServiceRunner`
6. **Create Project**: Calls the Connection API to create a `.ideaCon` project from the IOM file
7. **Save Project**: Saves the generated project to your desktop
8. **Clean Up**: Closes the project on the server

## Usage

1. Build and run the project
2. Select the desired example (1 or 2) when prompted
3. The application will generate the IOM and create a Connection project
4. The resulting `.ideaCon` file will be saved to your desktop with the name `connectionFromIOM-local.ideaCon`
5. You can open this file in IDEA StatiCa Connection application

## Key Components

### Dependencies

- **IdeaStatiCa.ConnectionApi** - NuGet package for interacting with IDEA StatiCa Connection
- **IOM.SteelFrameExample** - Project reference containing the IOM generator classes

### Main Classes

- `Program` - Main entry point that orchestrates the workflow
- `SteelFrameExample` - Generator for ECEN steel frame (from referenced project)
- `SimpleFrameAUS` - Generator for Australian steel frame (from referenced project)
- `ConnectionApiServiceRunner` - Service runner for the Connection API

## Output

The application creates:
- `example.xml` - The IOM XML file in the application directory
- `connectionFromIOM-local.ideaCon` - The IDEA StatiCa Connection project on your desktop

## Related Examples

- [IOM.SteelFrameExample](../IOM.SteelFrameExample) - Contains the IOM generator logic
- [IOM.SteelFrameWeb](../IOM.SteelFrameWeb) - Demonstrates creating projects using IDEA Cloud web service
- [Parent README](../readme.md) - Detailed tutorial on creating IOM for steel frames

## Additional Resources

- [IDEA StatiCa Connection API Documentation](../../../../../src/api-sdks/connection-api/clients/csharp/src/IdeaStatiCa.ConnectionApi)
- [OpenModel Documentation](../../../../../src/IdeaRS.OpenModel)

## Notes

- This example requires a local installation of IDEA StatiCa
- The API service is automatically started and stopped by the application
- If you encounter any issues, verify that your IDEA StatiCa installation path is correct in App.config
