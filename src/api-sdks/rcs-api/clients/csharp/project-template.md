#### Creating the Rcs Rest API Client by IDEA StatiCa project templates

Since the IDEA StatiCa v24 BIM links projects can be created by project [dotnet templates](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates). Templates help users to create a IDEA StatiCa BIM link projects. It requires to install [IdeaStatiCa.Dotnet.Templates](https://www.nuget.org/packages/IdeaStatiCa.Dotnet.Templates/). Open CMD terminal (or Powershell) and run :

```powershell
dotnet new install IdeaStatiCa.Dotnet.Templates
```

To create a new BIM Fea plugin do following :


1. Open CMD terminal (or Powershell) and create a directory for your project. Make sure template is intalled by running the command

```powershell
dotnet new list
```

If templates are correctly this template should be in the list

```
IDEA StatiCa RCS Rest API client console App     rcsclientconsole            [C#]        ideastatica/rcs/api
```

2. create a new solution in the project directory (in this case MyRcsApiClientApp.sln_) for .NET 8.0


```
dotnet new rcsclientconsole -n MyRcsApiClientApp -F net8.0
```

for .NET Framework 4.8
```
dotnet new rcsclientconsole -n MyRcsApiClientApp -F net48
```

4. Open it in Visual studio or build and run it by dotnet CLI

```
dotnet restore .\MyRcsApiClientApp.sln
dotnet build .\MyRcsApiClientApp.sln -c Release
.\MyRcsApiClientApp\bin\Release\net8.0\MyRcsApiClientApp.exe
```

__Creating BIM Link in Visual Studio__

To create the console client application for _IdeaStatiCa.RcsRestApi.dll_ select the template _IDEA StatiCa RCS Rest API client console App_. Similar to creating the BIM API FEA link :

![VS Wizard](../../../../..//docs/Images/vs-idea-templates.png?raw=true "VS Wizard")

![Project name](../../../../..//docs/Images/sln-fea-configuration.png?raw=true "Project name")

![Framework definition](../../../../..//docs/Images/framework-definition.png?raw=true "Framework definition")

![fea-running-example](../../../../..//docs/Images/fea-running-example.png?raw=true "fea-running-example")
