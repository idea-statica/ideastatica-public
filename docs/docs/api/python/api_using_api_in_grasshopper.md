Grasshopper is a great way to utilize the IDEA StatiCa API to perform optimization and retrieve connection results. Any of the scripting components available in out-of-the-box grasshopper can be used to Automate IDEA through the API. 

![Grasshopper scripting components](https://github.com/idea-statica/ideastatica-public/blob/main/docs/Images/wiki/gh_scripting_components.png)

As most of the output that is provided by the API can be serialized or represented in a JSON format it is easy to retrieve output by using available plug-ins in Grasshopper. [jSwan](https://www.food4rhino.com/en/app/jswan) is a great plugin for working with JSON strings.

## GhPython

GhPython is a powerful scripting component that allows a user to run python code within Grasshopper. Most of the Python examples provided should work when used inside GhPython. 

The example code below provides an example of how an IDEA Connection file can be run and the bolt forces extracted using GhPython. jSwan is then used to filter specific results from the JSON output.

![bolt forces](https://github.com/idea-statica/ideastatica-public/blob/main/docs/Images/wiki/gh_scripting_python_complete.png)

Copy and paste the following code into a GhPython component. Note: The trigger component is used to stop the component from running consistently as calculation may take some time.

![bolt forces code](https://github.com/idea-statica/ideastatica-public/blob/main/docs/Images/wiki/gh_scripting_python.png)

Before running the script:
* Change the `ideaCon_filename` to a prepared .ideaCon file which has some bolts in it. Make sure the Connection file is located in the same folder as the Grasshopper script and the grasshopper script is saved. The GH script is used to find the file automatically.

```python

"""Provides a scripting component.
    Inputs:
        x: The x script variable
        y: The y script variable
    Output:
        a: The a output variable"""

__author__ = "NathanLuke"
__version__ = "2022.04.12"

import Grasshopper as gh
import rhinoscriptsyntax as rs
import clr
import sys
import json
import math
import os

# the path to the idea connection installation directory
idea_path = r"C:\Program Files\IDEA StatiCa\StatiCa 21.1"

# modify path to be able to load .net assemblies
sys.path.append(idea_path)

# load the assembly IdeaStatiCa.Plugin which is responsible for communication with IdeaStatiCa 
clr.AddReference('IdeaStatiCa.Plugin')
from IdeaStatiCa.Plugin import ConnHiddenClientFactory

# find the directory of this script to fetch idea file.
ghfilepath = ghenv.LocalScope.ghdoc.Path
script_dir = os.path.dirname(ghfilepath)

# the name of idea connection project which is used in this script
ideaCon_filename = r"BoltResultTest.ideaCon"

connection_project_path = os.path.join(script_dir, ideaCon_filename) 

# create the instance of the client which communicates with IdeaStatiCa
factory = ConnHiddenClientFactory(idea_path)
ideaConnectionClient = factory.Create()

# open idea connection project 
ideaConnectionClient.OpenProject(connection_project_path)

# get information about connections in the project and print them
projectInfo = ideaConnectionClient.GetProjectInfo()
connections = projectInfo.Connections

for conn in connections:
    print(f'{conn.Name} { conn.Identifier}')

# Get Connection Info of First Connection in file
firstCon = connections[0]

# Runs Calculation
briefResults = ideaConnectionClient.Calculate(firstCon.Identifier)

# Get Detailed Results Info of the check of the first connection
checkResults_json_string = ideaConnectionClient.GetCheckResultsJSON(firstCon.Identifier)

#Convert Result to JSON
pyJSON = json.loads(checkResults_json_string)

#Find Bolt Results
boltJSON = pyJSON['bolts']

#Return Bolt Results to GhPython Component Output Parameter
a = json.dumps(boltJSON, indent = 4)

# Close idea connection project
ideaConnectionClient.CloseProject()
ideaConnectionClient.Close()

```
