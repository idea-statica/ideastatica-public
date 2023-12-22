# RCS API

Welcome to the IDEA StatiCa RCS API (Application Programming Interface).

The RCS API can be used to interact with IDEA StatiCa RCS to create and optimize designs of reinforced cross-sections.

> [!IMPORTANT]
> As of version **23.1.3** a new RCS API is avaliable.

## API Architecture

The RCS API is built on REST Open API architecture and runs over a http protocal. The current version of the API creates REST server which is hosted locally on a users computer. However, in the future we may also provide the possibility to run calculations on remote machines.

Users can interact with the RCS API using one of the provided clients or by calling the REST API directly from any programming language. We recommend using one of the provided IDEA StatiCa wrapper clients for **.Net** or **Python**.

## Client Installation

# [.Net](#tab/dotnet)

To install the RCS API in your .Net project you should first add the _IdeaStatiCa.RcsApiClient_ NuGet package to your project. See [here](../../../articles/nugetpackages.md) for installing NuGet packages in Visual Studio. Note that _IdeaStatiCa.Plugin_ is a dependency of the _IdeaStatiCa.RcsApiClient_ and should automatically be installed, if not it should also be added to your project through NuGet. 

Once the NuGet packages are referenced in your project the following using statements should be avaliable:

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/RcsCodeSamples.cs#rcsusings)]

> [!NOTE]
> The .Net RcsApiClient is compatible with projects using .Net6.0 or greater.
> [!IMPORTANT]
> The .Net client is set up to work asyncroniously.

# [Python](#tab/python)

TO DO 

---