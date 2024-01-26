# RCS API Client Basics

Once you have [installed](rcs_api_getting_started.md) the RCS API client follow the below to understand the basics of the API.

Simple examples for both python and charp can be found at the links below:
[.Net (csharp)](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/readme.md)
[Python](../../../../examples/api/python/rcs/scripts/simple_rcs_calculation/readme.md)

When creating an application, the basic steps are as follows.

1. Initializing the API client - start the communication with RCS.
2. Open and existing or create a new RCS project.
3. Performing possible operations on the loaded project using the API.
4. Cleaning or disposing of the API client.

## Initializing the Client

To start we first need to establish an API client which we can use to interact with RCS.

# [.Net](#tab/dotnet)

The _RcsClientFactory_ can provide us with a client which are linked to the selected installed version of IDEA StatiCa.

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#create_client)]

The code above uses using statements which will automatically dispose the client once you have finished performing operations. Alternatively you can dispose of the client manually by calling `client.dispose()`.

# [Python](#tab/python)

[!code-python[](../../../../examples/api/python/rcs/code_snippets/script_template.py#L5-L26)]

In python, once we have finished performing operations with the client we can dispose it using:

[!code-python[](../../../../examples/api/python/rcs/code_snippets/script_template.py#L56-L57)]
---

We can now used the active client to interact with RCS Application.

## Open an RCS project

Once the connection with RCS is established we need to first open or create an RCS project (.ideaRcs).

### Open existing project

In most cases will want to open an existing project that is avaliable on our computer.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#open_existing)]

# [Python](#tab/python)

[!code-python[](../../../../examples/api/python/rcs/code_snippets/script_template.py#L33-L36)]

---

### Create project from IOM

We can also **Create** a new RCS Project. To create a new RCS project file, we will need to define the RCS Model using IDEA Open Model(IOM). See [here](../../../../examples/iom/iom-rcs/IomToRcsExamples/RcsReinforcedBeam/rcs_reinforced_beam.md) for examples of creating IDEA Open Model for RCS can be found.

> [!NOTE]
> There is not currently a way to create projects from scratch without first defining an IOM Model.

When creating a project from IOM, we can choose to create directly from a `OpenModel` class in memory:

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#open_from_model)]

# [Python](#tab/python)

TO DO.

---
or from a previously saved IOM.xml filepath.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#openfromiomfile)]

# [Python](#tab/python)

[!code-python[](../../../../examples/api/python/rcs/code_snippets/script_template.py#L38-L41)]

---

> [!IMPORTANT]
> If the project was generated from an IOM file or IOM Model then the _SaveProject_ command will need to be called to save the generated RCS project to disk.

## API Operations

Once a project is open the client can work with it using the avaliable methods avaliable in the API.

### Calculating Project Sections

Once a project is active it can be calculated. We can simply call the _Calculate_ command. We can select which sections to calculate through the _CalculationParameters_ all the sections avaliable in the project. By default all sections will be calculated. 

Once the calcualtion has been performed, results are stored for retrieval at a later time. Brief results are provided as a response from the Calculate call, this is to allow the user to get a quick understanding on whether the calculation for each section has been succesful without having to retrieve more detailed results.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#calculate_project)]

# [Python](#tab/python)

```python
    # Get IDs of all sections in the RCS project.
    secIds = []
    for s in rcsClient.Project.Sections.values():
        secIds.append(s.Id)

    # Calculate all sections in the RCS project.
    calc1_briefResults = rcsClient.Calculate(secIds)

    #Print the brief results.
    print(calc1_briefResults)

```

---

### Getting Section Results

After sections have been calcuated we can retrieve more detailed results for all or only particular sections.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#section_results)]

# [Python](#tab/python)

```python
    
    # get IDs of all sections in the rcs project
    secIds = []
    for s in rcsClient.Project.Sections.values():
        secIds.append(s.Id)

    # Calculate all sections in the rcs project
    calc1_briefResults = rcsClient.Calculate(secIds)

    # Get detail results of all calculated rcs sections
    detailResults = rcsClient.GetResults(secIds)
    print(detailResults)

```

---

### Saving the Project

Any modification that we perform on the project using the API are cached (temporaily saved). 

If we have performed modifications that we want to save back to our local file, we will need to call the _SaveProject_ command. 

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#saveproject)]

# [Python](#tab/python)

```python

newFilePath = "updatedRCSProject.ideaRcs"
client.SaveProject(newFilePath)

```

---

> [!TIP]
> Looking for a code sample? Please post a question on our [discussion forum](https://github.com/idea-statica/ideastatica-public/discussions/categories/help-q-a).