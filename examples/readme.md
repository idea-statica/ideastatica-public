# IDEA StatiCa SDK Examples

We provide a comprehensive list of examples for getting started with IDEA StatiCa's API. The examples folder structure is similar to the split up of the developer website and is as below.

```
examples
├── iom
│   ├── Examples related to Idea Open Model generation for different applications
├── api
│   ├── dotnet
│   │   ├── Examples related to Design App Api's using .Net (csharp) 
│   ├── python
│   │   ├── Examples related to Design App Api's using Python
├── bimapi
│   ├── Examples related to creating FEA and CAD BimLinks with Checkbot
├── extensions
│   ├── Examples related to API extensions
```

## .Net Example Solution

Within the examples folder we provide a standalone visual studio solution which contains all the examples relevant  to .Net. To get access to this folder you should first clone the repository to your desktop and then navigate to the examples folder and open the **IdeaStatiCa-SDK-Examples.sln**.

Each example project has two debug configurations 'Debug' and 'Debug_NuGet'. The projects in the IdeaStatiCa-SDK-Examples.sln are set to Debug_Nuget which means they referencee avaliable Nuget packages on build. As we still actively develop some of the examples avaliable against our source code we use 'Debug' for internal.

## Python Examples

Python examples should be contained in a folder with everything you need to open and run a particular example. Python examples are not provided in the visual studio solution.



