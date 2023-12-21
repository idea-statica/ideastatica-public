Python is a simple and powerful programing language that continues to grow in popularity in the AEC industry.

All IDEA StatiCa packages are developed on the .NET platform. Using [Python.NET](http://pythonnet.github.io/) the libraries can be accessed and used by Python programmers.

# Installing Python (Desktop Environment)

One of the simplest ways to install Python is using Anaconda. Anaconda automatically installs Python and most of the packages that are used in python programming. It also automatically manages package versions for you! To install Anaconda please follow the instructions here:

* [Installing Python and JupyterLab using Anaconda](api_install_python_and_jupyterlab_using_anaconda.md)

For those using other Python installs, typical prerequisites for using Python on a desktop environment: 
* Install [Python v37](https://www.python.org/downloads/)
* Install [Python.NET](http://pythonnet.github.io/) as it is described on [here](https://github.com/pythonnet/pythonnet/wiki/Installation). Latest versions are strongly recommened (Greater than python .NET 3.0).

## Code Editors
### Visual Studio Code

[Visual Studio Code](https://code.visualstudio.com/) is a free lightweight code editor that is popular for python developments and provides a range of extensions and tools for developing in Python. 

[Auto-completion in Visual Studio Code](api_python_create_python_stubs.md) - Provides a guide on how to create python stub files for IDEA StatiCa modules to enable auto-completion.

### Jupyter Lab
[Jupyter Lab](https://jupyter.org/) is a tool used by many data scientists and engineers to create documents that can run code in modulated steps. Combined with [Pandas](https://pandas.pydata.org/) data frames and other powerful python visualization modules such as [matplotlib](https://matplotlib.org/) this can be an extremely powerful tool. 

### Notepad++
[Notepad++](https://notepad-plus-plus.org/downloads/) Can also be used to write simple Python code in addition to its many other benefits for text manipulation.

### GhPython
Python code can be run directly in the visual programming tool Grasshopper using the GhPython component. 
* [Guide to Getting started with GhPython](api_using_api_in_grasshopper.md)



## Referencing the IDEA Libraries

Typically required references when working with the IDEA Packages in Python: 

```python
# Typical Python References
import clr
import sys
import json
import math
import os
```

Add the path location of the DLLs to the system path. Typically this may be from the IdeaStatiCa installation directory:

```python
idea_path = "C:\Program Files\IDEA StatiCa\StatiCa 21.1"
sys.path.append(idea_path);
```

.Net libraries can then be referenced using the common language runtime (CLR). 

```python
clr.AddReference('IdeaStatiCa.Plugin')
clr.AddReference('IdeaRS.OpenModel')
```
Import required libraries or classes from the modules.

```python
from IdeaStatiCa.Plugin import ConnHiddenClientFactory
from IdeaRS.OpenModel import OpenModel
```
## Examples
* [Create IOM with Python Steel Frame](../../../examples/iom/IOM/readme.md)

## Further Resources
* Steven Verner has created a Python project to simplify the use of the IDEA StatiCa Plugin API for the optimization of Steel connections. Link to the [GitHub repository](https://github.com/stevenverwer/ideastaticapy) and instructions on installing.
* [VIKTOR](https://www.viktor.ai/) provides a web-based application that allows integration with IDEA StatiCa online apps through a python based environment.