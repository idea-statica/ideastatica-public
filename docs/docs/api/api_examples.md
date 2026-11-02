# IDEA StatiCa API Examples

We provide a comprehensive list of examples for getting started with IDEA StatiCa's APIs and Frameworks.

## API Examples

Design API Examples can be found under the corresponding api-sdk.
```
src
├──api-sdks
    ├──connection-api
        ├──python
        ├──csharp
    ├──rcs-api
        ├──python
        ├──csharp
```
### C# examples

Each API has a seperate Visual Studio solution (.sln) which contains all the avaliable examples.

- [Connection API Examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/csharp/examples)
- [RCS API Examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/rcs-api/clients/csharp/examples)

> [!NOTE]
> Each example solution has two debug configurations 'Debug' and 'Debug_NuGet'. Projects in the IdeaStatiCa.###.Examples.sln are generally set to Debug_Nuget which means they referencee avaliable Nuget packages on build. As we still actively develop some of the examples avaliable against our source code we use 'Debug' for internal.


### Python examples

For python, we provide two relevant example folders, examples and examples-pip. examples-pip requires the user to install the api client to their python enviroment. However the examples provided are the same.

Within each example folder a list of folders (one for each example provided).

- [Connection API Examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/connection-api/clients/python/examples)
- [RCS API Examples](https://github.com/idea-statica/ideastatica-public/tree/main/src/api-sdks/rcs-api/clients/python/examples-pip)






