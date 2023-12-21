# RCS API Client Basics

Once you have [installed](rcs_api_getting_started.md) the RCS API client follow the below to understand the basics of the API.

## Initializing the Client

To start we first need to establish an API client which we can use to interact with RCS. The _RcsClientFactory_ can provide us with a client which are linked to the selected installed version of IDEA StatiCa.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#createclient)]

# [Python](#tab/python)

TO DO 

---

We can now used the active client to interact with RCS Application.

## Open an RCS project

Once the connection with RCS is established we need to first open or create an RCS project (.ideaRcs) 

### Open existing project

In most cases will want to open an existing project that is avaliable on our computer.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#openexisting)]

# [Python](#tab/python)

TO DO 

---

### Create project from IOM

We can also **Create** a new RCS Project. To create a new RCS project file, we will need to define the RCS Model using IDEA Open Model(IOM). See [here](../../../../examples/iom/iom-rcs/IomToRcsExamples/RcsReinforcedBeam/rcs_reinforced_beam.md) for examples of creating IDEA Open Model for RCS can be found.

> [!NOTE]
> There is not currently a way to create projects from scratch without first defining an IOM Model.

When creating a project from IOM, we can choose to create directly from a `OpenModel` class in memory 


# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#openfrommodel)]

# [Python](#tab/python)

TO DO

---

Or from a previously saved IOM.xml filepath.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#openfromiomfile)]

# [Python](#tab/python)

Content for Windows...

---

Once a project is open the client can work with it using the avaliable methods avaliable in the API.

## Updating Code Settings

We allow the updating of Code setting through the API. 

# [.Net](#tab/dotnet)

TO DO

# [Python](#tab/python)

TO DO

---

## Calculating Project Sections

Once a project is active it can be calculated. We can simply call the _Calculate_ command. We can select which sections to calculate through the _CalculationParameters_ all the sections avaliable in the project. By default all sections will be calculated.

Once the calcualtion has been performed, results are stored for retrieval at a later time.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#calculateproject)]

# [Python](#tab/python)

TO DO

---

## Getting Section Results

After sections have been calcuated we can retrieve more detailed results for all or only particular sections.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#sectionresults)]

# [Python](#tab/python)

TO DO

---

## Saving the Project

Any modification that we perform on the project using the API are cached (temporaily saved). 

If we have performed modifications that we want to save back to our local file, we will need to call the _SaveProject_ command.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#saveproject)]

# [Python](#tab/python)

TO DO

---

## Disposing the Client

Once we have completed all the operations we want we can dispose of the client and the client factory.

# [.Net](#tab/dotnet)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/Program.cs#dispose)]

# [Python](#tab/python)

TO DO

---