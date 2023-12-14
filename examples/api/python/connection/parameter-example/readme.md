# parameters-anchors

This example shows how to modify parameters of the IDEA Connection from Python. In this example, the bending moment, the thickness of the base plate and the length of the anchors are modified. Results are printed to the console.

How to set parameters in idea connection project is described on ths [page](../../../../../docs/docs/api/api_parameters_getting_started.md)

<!---![Python script in Visual Studio Code](../../../../Images/python-vs-code.png)--->

To able to run the script from the command line you need to navigate to the directory which includes the python script an run :

```python
python parameters-anchors.py
```

## Prerequisites

Install [Python v37](https://www.python.org/downloads/)

Install [Python.NET](http://pythonnet.github.io/) as it is described on [here](https://github.com/pythonnet/pythonnet/wiki/Installation)

Install Idea StatiCa v21 or higher

Build the release configuration of the solution ConnCalcExamples\ConnCalculationExamples.sln. The python script needs .net assemblies in  in the directory '..\ConnCalcExamples\ConnectionHiddenCalculation\bin\Release'

