# Checkbot BIM link examples

Examples of a link between IDEA StatiCa Checkbot and another application. Three are built on the
[IdeaStatiCa.BimApiLink](https://www.nuget.org/packages/IdeaStatiCa.BimApiLink) package: one for a CAD application,
and two for an FEA application, in C# and in C++. The fourth links an FEA application through SAF files, with the
[IdeaStatiCa.SAF2IOM](https://www.nuget.org/packages/IdeaStatiCa.SAF2IOM) package. Each comes with a fake version of
that application, which your own application replaces.

You can read more about this [here](https://github.com/idea-statica/ideastatica-public/wiki/Linking-an-Application-with-Checkbot).

Before creating your own link we always encourage you to get in touch with us to discuss co-operation.

## Examples

| Folder | What it is |
|---|---|
| [CadExample](CadExample/README.md) | A link for a CAD application: a connection with its plates, bolts and cuts. |
| [FeaExample](FeaExample/README.md) | A link for an FEA application: members, loads and internal forces. |
| [CppFeaExample](CppFeaExample/README.md) | The FEA link in C++/CLI, for an FEA application written in native C++. It has its own fake application, a native MFC window. |
| [SafFeaExample](SafFeaExample/README.md) | A link for an FEA application that saves its model as SAF files: connections, members, and a sync after the model changes. It has its own fake application, a small window. |
| `BimLinkExampleRunner` | A small window that starts the CAD or the FEA link, so you can try them without a CAD or FEA application. |

Each README says what the example shows and how to run it.

## Creating BIM Link by IDEA StatiCa project templates

Since the IDEA StatiCa v24 BIM links projects can be created by project [dotnet templates](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates). Templates help users to create a IDEA StatiCa BIM link projects. It requires to install [IdeaStatiCa.Dotnet.Templates](https://www.nuget.org/packages/IdeaStatiCa.Dotnet.Templates/). Open CMD terminal (or Powershell) and run :

```powershell
dotnet new install IdeaStatiCa.Dotnet.Templates
```

To create a new BIM Fea plugin do following :


1. Open CMD terminal (or Powershell) and create a directory for your project. Make sure template is intalled by running the command

```powershell
dotnet new list
```

If templates are correctly these templates should be in the list

```
IDEA StatiCa Checkbot Client CAD App          bimapicadapp                [C#]        ideastatica/checkbot/cad
IDEA StatiCa Checkbot Client FEA App          bimapifeaapp                [C#]        ideastatica/checkbot/fea
IDEA StatiCa RCS Rest API client console App  rcsclientconsole            [C#]        ideastatica/rcs/api
```

2. create a new solution in the project directory (in this case _MyRcsClientApp.sln_) for .NET Framework 4.8
```
dotnet new bimapifeaapp -n MyFeaBIMApp -F net48
```

for .NET 6.0 windows
```
dotnet new bimapifeaapp -n MyFeaBIMApp -F net6.0-windows
```

4. Open it in Visual studio or build and run it by dotnet CLI

```
dotnet restore .\MyFeaBIMApp.sln
dotnet build .\MyFeaBIMApp.sln -c Release
MyFeaBIMApp\bin\Release\net6.0-windows\MyFeaBIMApp.exe
```

__Creating BIM Link in Visual Studio__

![VS Wizard](../../..//docs/Images/vs-idea-templates.png?raw=true "VS Wizard")

![Project name](../../..//docs/Images/sln-fea-configuration.png?raw=true "Project name")

![Framework definition](../../..//docs/Images/framework-definition.png?raw=true "Framework definition")

![fea-running-example](../../..//docs/Images/fea-running-example.png?raw=true "fea-running-example")

## Further Information

In a fully functioning link, there are typically a number of different importers and further conversion utilities which are outside the scope of this example. We will continue to improve on this example where required. 

Please comment in the [discussion forums](https://github.com/idea-statica/ideastatica-public/discussions/categories/bim-api-link-disscusion) if you require further clarification or information.







