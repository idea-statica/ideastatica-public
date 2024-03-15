# Simple RCS Calculation 

This example is a simple example which opens and existing RCS Project and runs the calculation of all the sections in the project.

The brief results for each section are then printed to the console.

Below is the primary Program.cs file of the Console Application.

[code-csharp](Program.cs)

The solution for this example can be also created by [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/) or in Microsoft Visual Studio. It requires to install 

It installs nuget package
[IdeaStatiCa.Dotnet.Templates](https://www.nuget.org/packages/IdeaStatiCa.Dotnet.Templates/)

Open CMD terminal (or Powershell) and do following 

1. Create a directory for your project.

2. install IdeaStatiCa project templates (if not installed)
```
dotnet new install IdeaStatiCa.Dotnet.Templates
```

3. create a new solution in the project directory (in this case _MyRcsClientApp.sln_) for .NET Framework 4.8
```
dotnet new rcsclientconsole -n MyRcsClientApp -F net48
```

for .NET 6.0
```
dotnet new rcsclientconsole -n MyRcsClientApp -F net6.0
```

4. Open it in Visual studio or build and run it by dotnet CLI

```
dotnet restore .\MyRcsClientApp.sln
dotnet build .\MyRcsClientApp.sln -c Release
MyRcsClientApp\bin\Release\net48\MyRcsClientApp.exe
```
