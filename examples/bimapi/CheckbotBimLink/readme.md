# Checkbot BIM Link Example for an FEA Application.

## Intro

Here we provied two simple examples on how to create a link with IDEA StatiCa Checkbot and either a FEA or CAD Application using our BimApiLink Framework. 

There are typically some requirements to ensure a connection can be completed successfully with Checkbot. In these examples we provide Fake/Mock/Simulation API's for both the CAD and FEA applications, which should be replaced by your application API.

You can read more about this [here](https://github.com/idea-statica/ideastatica-public/wiki/Linking-an-Application-with-Checkbot).

Before creating your own link we always encourage you to get in-touch with us to discuss co-operation. 

## Example

### Example requirements

- Visual Studio 2022 or Greater

### Architecture

The Below relates to the FEA example, however the CAD example follows the exact same logic.

There are two main projects in each example:

1. **BimApiLinkFEAExample** - The primary Class Library project which provides linking and conversion with the Checkbot application.  

<details>
  <summary>Look First</summary>
  
	Look at TestPlugin.cs to get started. The Run() method is the primary method which will be called from your application. 

### Dependency Injection
Our BimLinkFramework relys on automatic dependency injection. We use [Autofac](https://autofac.readthedocs.io/en/latest/integration/aspnetcore.html) for this. Within the BuildContainer() method we define the instances used in the plugin.

</details>

2. **FeaExampleApi*** - A class library project which provides some dumby structural geometry and other inputs to the *BimApiLinkFEAExample*. It emulates a typical API of an FEA software and provides some dumby structural information. 

> *When creating your own link only the *BimApiLinkExample* would be required. The *BimApiExample* should be replaced by a command interface or other small app which allows your program to host and run the link. *FeaExampleApi* should be replaced with the third party API object.


3. **BimLinkExampleRunner*** - Simple windows form app for running and testing the *BimApiLinkFEAExample* or *BimApiLineCadExample*. Implements the Run() command provided from the TestPlugin.

> *This is only used for demonstrating in the test enviroment. This will be replaced by on the third-party developers side with either another simple app runner or a command interface.

<details>
  <summary>Look First</summary>
  
	Navigate to the folder ViewModels > MainWindowViewModel.cs. The MainWindowViewModel class provides a method called OnRunCheckbot() which implements the TestPlugin.Run() method. The FEAExampleApi is provide to the TestPlugin. 

</details>


### Building the example

1. Clone the [idea-statica/public](https://github.com/idea-statica/ideastatica-public) repository. Some typical instructions [here](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository?tool=desktop).
2. In windows explorer navigate to the repository location `/src` folder and open the solution *IdeaStatiCa.Public.sln* in visual studio.
3. Build the entire solution, which may take a minute or two. There should be no errors. 
4. Navigate to the example location and right click on the *BimLinkExampleRunner* and select set as start-up project.

### Running the example

This simple example will run Checkbot and allow you to import some simple Member and Node objects which we have defined in the *FakeFEAApi*.

1. Run the *BimLinkExampleRunner* project. A form should appear. 
2. We need to provide the link to the Checkbot executable. This allows you to select which version of IDEA StatiCa you would like to run. For example:

```text
C:\Program Files\IDEA StatiCa\StatiCa 23.1\IdeaCheckbot.exe
```

3. Now we can *Run IDEA StatiCa Checkbot*. This triggers the `TestPlug.Run()` command. 
4. [info] A new Checkbot project folder will automatically be created (if one has not already been created) in the the /bin folder. 
5. The new project screen will appear. You will be prompted to select a Country Code (Select **EU** for this example) and Create the Checkbot project. Checkbot should now open to a black screen.

> On creating the Checkbot project, a project file *IdeaStatiCa.proj* is saved into the checkbot folder. Next time checkbot is run it will detect this file and the project will be opened automatically wher you left off. To reset the test, simply delete the created Checkbot folder.

6. In the top left hand corner of the ribbon, click 'Connections'. This triggers the `GetUserSelection()` method in the 'Model' class of *Model.cs*. Here we provide the indentifiers of the objects that we want to import.
7. One column (M7) and one beam (M13) should be imported, with three connections (C 9, C 15, C 16). 
8. END OF EXAMPLE

### Copying the example to create a new Link 

To start work on a link of your own, you can copy the BimApiLinkFeaExample or BimApiLinkCadExample project and rename it. You will need to bulk rename the namespace in the example. A typical naming structure is [AppName]Link. 

You can use the BimApiExample to test your duplicated plugin by changing the project reference from the *BimApiLinkFEAExample* to your created project.

> When creating your own link only the BimApiLinkExample would be required, BimLinkExampleRunner should be replaced by a command interface or other small app which allows your program to host and run the link. FeaExampleApi should be replaced with the third party API Model object.

#### References

As you will eventually deploy your plugin outside of the test environment, it is best to reference required modules via Nuget and remove current project dependencies.  

Simply double click on your project in the solution explorer and modify the xml as below. We also highly reccommend that you update to the latest NuGet packages at this point.

```xml
	<ItemGroup>
		<PackageReference Include="Autofac" Version="6.4.0" />
		<PackageReference Include="Autofac.Extensions.DependencyInjection" Version="8.0.0" />

		<PackageReference Include="IdeaStatiCa.BimApiLink" Version="22.1.0.3519" />
		<PackageReference Include="IdeaStatiCa.BimImporter" Version="22.1.0.3519" />
		<PackageReference Include="IdeaStatiCa.Plugin" Version="22.1.0.3519" />
	</ItemGroup>
```

You should also remove the current reference to the FeaExampleApi and inject your own applications Api DLLs. 

#### Further Information

In a fully functioning link, there are typically a number of different importers and further conversion utilities which are outside the scope of this example. We will continue to improve on this example where required. 

Please comment in the [discussion forums](https://github.com/idea-statica/ideastatica-public/discussions/categories/bim-api-link-disscusion) if you require further clarification or information.







