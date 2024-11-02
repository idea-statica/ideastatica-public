## How to create this project by a  VS project template

This is related to IDEA StatiCa 24.1.0 and higher

1. Install or update the templates [IdeaStatiCa.Dotnet.Templates](https://www.nuget.org/packages/IdeaStatiCa.Dotnet.Templates/). The required version is 1.0.40 or higer

```
dotnet new install IdeaStatiCa.Dotnet.Templates  
```

Create a directory for your project and run th command
```
dotnet new bimapifeaapp
```

it will create a new VS solution in the working directory.

All options can be shown by
```
dotnet new bimapifeaapp --help
```
