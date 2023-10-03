#### The example of controlling *IdeaConnection.exe* from another process - see project [ConnectionAutomationApp](https://github.com/idea-statica/iom-examples/tree/master/ConnCalcExamples/ConnectionAutomationApp)

This examples use [IdeaStatiCa.Plugin](https://github.com/idea-statica/ideastatica-plugin) for communication with *IdeaConnection.exe*. Running this example it requires IDEA StatiCa v 20.0 (or higher) on an user's PC. Free trial version version can be obtained [here](https://www.ideastatica.com/free-trial).

![ConnectionHiddenCalculation](https://github.com/idea-statica/iom-examples/blob/gh-pages/iom-steel-connections/Images/connection-app-automation.png?raw=true)

Starting *IdeaConnection.exe*

```C#
// it starts the new process of IdeaConnection.exe which is located in the directory ideaStatiCaDir
this.ConnectionController = IdeaConnectionController.Create(ideaStatiCaDir);
```

When *IdeaConnection.exe* is running the project can be open

```C#
// open selected idea connection project in the running application IdeaConnection.exe
onnectionController.ConnectionAppAutomation.OpenProject(openFileDialog.FileName);
```

Closing open project
```C#
// open selected idea connection project in the running application IdeaConnection.exe
ConnectionController.ConnectionAppAutomation.OpenProject(openFileDialog.FileName);
```