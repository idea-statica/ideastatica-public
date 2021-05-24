## Diagnostics in Idea CCM ##

![CCM Diagnostics](../Images/ccm-diagnostics.png)

### Setting of the severity of messages in the log file ###
The severity level of messages which are written into log file can be configured in the file **_IdeaDiagnostics.config_** which can be found in the IdeaStatiCa temp folder e.g. _C:\Users\YOUR-USER-NAME\AppData\Local\IDEA_RS\IdeaDiagnostics.config_

```xml
<IdeaDiagnosticsSettings>
	<DefaultLogLevel loglevel="Debug"/>
	<!-- <LoggerLogLevel loggername="app.program" loglevel="Debug"/> -->
	<!-- <DebugView active="false"/> -->
</IdeaDiagnosticsSettings>
```

The default value of Severity is **Information**. If it is changed to **Debug** or **Trace** more details are written to log file.

### CCM log file location ###

The log file can be opened directly from our example application - see the button **Show CCM Log**. The log file is located in the users's temp folder _C:\Users\USER NAME\AppData\Local\Temp\IdeaStatiCa\Logs\IdeaStatiCaCodeCheckManager.log_. Our diagnostics uses rolling file sink. If the size of a log file exceeds a limit a new log file is added e.g. _IdeaStatiCaCodeCheckManager_001.log_.

