### ConHiddenCheckConsole

It is very simple console application example. *ConnCalculatorConsole.exe* requires the path to IDEA Statica installation directory and optionly the path to the idea connection project. If any project is passed the default 
project for this example is calculated.

The example uses [IdeaStatiCa.Plugin](../../../IdeaStatiCa.Plugin) for running CBFEM analysis. Running this example it requires IDEA StatiCa v 21.0 (or higher) on an user's PC. Free trial version version can be obtained [here](https://www.ideastatica.com/free-trial).

![ConnectionHiddenCalculation](../../../../Images/hidden-check-console.PNG?raw=true)

Running CBFEM from the commad line :

```
ConnCalculatorConsole.exe "C:\Program Files\IDEA StatiCa\StatiCa 21.0" "c:\test.ideaCon"
```