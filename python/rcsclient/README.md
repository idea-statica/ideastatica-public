# Python client of IDEA StatiCa RCS API

To restore the dependencies from the requirements.txt file, use the following command:
```
pip install -r requirements.txt
```

the example __project1-calc-example.py__ shows how to open an RCS project [Project1.IdeaRcs](projects/Project1.IdeaRcs), calculate it, import a new reinforced cross-section from a NAV template and recalculate the rcs project

The example can be run by :
```
py .\project1-calc-example.py
```

the example shows how to compare capacity check values for different reinforced cross-sections

see in the log :

_Section 'S 1' capacity 0.30313778969222727_<br>
_Section 'S 2' capacity 0.27789957268651766_<br>
_Calc 1 : 0.41684935902977666 Calc 2 :  0.30313778969222727_

the example __update-loading-example.py__ shows how to open an RCS project [Project2.IdeaRcs](projects/Project2.IdeaRcs) calculate the capacity of the reinforced cross-section in bending. Loading is icreasing  up to capacity value in a loop and a crack width is calculated for each step.

The example can be run by :
```
py .\update-loading-example.py
```