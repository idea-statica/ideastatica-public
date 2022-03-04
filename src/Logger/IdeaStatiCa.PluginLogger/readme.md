# IdeaStatiCa.PluginLogger

The logger must be initialized during initialization of the application

```c#
// initialize logger
SerilogFacade.Initialize();
Logger = LoggerProvider.GetLogger("feappexample");
```

[See configuration in FEAppExample_1](https://github.com/idea-statica/ideastatica-public/blob/main/src/Examples/CCM/FEAppExample_1/FEAppTestVM.cs)

The severity of the messages in log is taken from the configuration file of IDEA Diagnostics see value _DefaultLogLevel_ in %LOCALAPPDATA%\IDEA_RS\IdeaDiagnostics.config

The logfile can be found in directory %Temp%\IdeaStatiCa\Logs\ The name of the logfile is taken from the name of the Entry assembly or it san be set 

IdeaStatica.PluginLogger is availabe as nuget package