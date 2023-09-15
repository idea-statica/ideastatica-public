The following explains how to enable auto-completion of code in Visual Studio Code (VSC). 

In order to get auto-completion working for imported IDEA Modules in Visual Studio Code when using Python, you need to create a Python 'stubs' folder for IDEA StatiCa modules. Stubs folders contain the skeleton information of a module that can be read by VSC to provide auto-completion. 

### Software Requirements:
* Visual Studio
* GitHub Desktop (Optional)

# Create Python Stubs Files

There are a number of tools that can create stub files for a .Net assembly: A good opensource tool we have found is developed by McNeel called PyStubler. This is used by McNeel to generate the stubs for their own .Net assemblies. We will use it here, to generate stubs for `IdeaStatiCa.Plugin` and `IdeaRS.OpenModel`.

1. Clone the repository of [pythonstubs](https://github.com/mcneel/pythonstubs) to your computer. If you are a Github beginner, we recommend downloading Github desktop.
2. Open the pythonstubs Repository folder and open the **PyStublerNet.sln** solution which is located in the 'builder' folder.   
3. When open, go the the PyStubbler projects properties. Replace the command line arguments with the location of the Idea .dlls you want to create stubs for. 
![properties](https://github.com/idea-statica/ideastatica-public/blob/main/docs/Images/wiki/pythonstubs_setlinks.png) 
```
--dest=".\Idea" --search="C:\\Program Files\\IDEA StatiCa\\StatiCa 21.1" "C:\\Program Files\\IDEA StatiCa\\StatiCa 21.1\\IdeaStatiCa.Plugin.dll" "C:\\Program Files\\IDEA StatiCa\\StatiCa 21.1\\IdeaRS.OpenModel.dll"
```
4. Build and then Run the PyStubbler project.
5. The following folders will be generated in the project bin folder. 
![stubs folders](https://github.com/idea-statica/ideastatica-public/blob/main/docs/Images/wiki/folders.png) 
6. Copy and paste these folders into the folder you are setting as the system path directory for the Reference Assemblies. In the case of Python, this is likely the IDEA StatiCa Programs Files directory. 
7. Restart Visual Studio Code to ensure the auto-completion takes effect. 
![VSC auto-complete](https://github.com/idea-statica/ideastatica-public/blob/main/docs/Images/wiki/VSCautocomplete.png) 


> Note: For each update of the API or program the stubs should be updated to avoid conflicts or not provide auto-complete for new updates!
 


