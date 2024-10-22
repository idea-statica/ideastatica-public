# Connection API

Welcome to the IDEA StatiCa Connection API (Application Programming Interface).

The Connection API can be used to interact with IDEA StatiCa Connection to create and optimize steel connection designs, plus many other things.

> [!IMPORTANT]
> As of version **24.1** a new Connection API is avaliable. This new API **replaces** the older ConnHiddenCalculator API which was avaliable in IdeaStatiCa.Plugin.

## API Architecture

The Connection API is built on REST OpenAPI architecture and runs over a http protocal. The current version of the API creates REST server which is hosted locally on a users computer. In the future we would also like provide the possibility to run calculations on remote machines.

Users can interact with the Connection API using one of the provided clients or by calling the REST API directly from any programming language. We recommend using one of the provided IDEA StatiCa clients for **.Net** or **Python**.

## Client Installation

# [.Net](#tab/dotnet)

To install the API in your .Net project you should first add the _IdeaStatiCa.ConnectionApi_ NuGet package to your project. See [here](../../../articles/nugetpackages.md) for installing NuGet packages in Visual Studio.

Once the NuGet packages are referenced in your project the following using statements should be avaliable:

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/RcsCodeSamples.cs#rcsusings)]

> [!NOTE]
> The .Net RcsApiClient is compatible with projects using .Net6.0 or greater.
> [!IMPORTANT]
> The .Net client is set up to work asyncroniously.

# [Python](#tab/python)

To use the RCS API in your Python project first install the IDEA StatiCa RCS Python client into your python environment.

The python client can be installed using `pip` install the [ideastatica-rcs-client](https://pypi.org/project/ideastatica-rcs-client/) for Python

To install the python client open the command line and type the following.

```powershell

pip install ideastatica-rcs-client
```

Once installed in the python environment we can use the follow import statements to install in our python project

```python

from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import ideastatica_rcs_client

```
---