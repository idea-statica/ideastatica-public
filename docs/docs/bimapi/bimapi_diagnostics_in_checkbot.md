
Within the checkbot framework, we provide the ability to log events within diagnostics files associated with your projects. It should provide users with information about limitations and restrictions to the plug-in. 

It is important to enable this feature to give users the ability to drill down into problems they may be facing when importing or running a third-party plug-in. 

## Accessing Diagnostics Files

The log file is located in the users's temp folder:
```txt
 _C:\Users\USER NAME\AppData\Local\Temp\IdeaStatiCa\Logs\IdeaStatiCaCodeCheckManager.log_. 
```
Our diagnostics uses a rolling file sink. If the size of a log file exceeds a limit a new log file is added e.g. _IdeaStatiCaCodeCheckManager_001.log_.

The diagnostics log files can also be opened directly from our [example application](https://github.com/idea-statica/ideastatica-public/tree/main/src/Examples/CCM/FEAppExample_1) - see the button **Show CCM Log**.

![CCM Diagnostics](https://github.com/idea-statica/ideastatica-public/blob/main/Images/ccm-diagnostics.png)
## Setting of the severity of messages in the log file ##
The severity level of messages which are written into a log file can be configured in the file **_IdeaDiagnostics.config_** which can be found in the IdeaStatiCa temp folder:

```txt
C:\Users\USER NAME\AppData\Local\IDEA_RS\IdeaDiagnostics.config
```

```xml
<IdeaDiagnosticsSettings>
	<DefaultLogLevel loglevel="Debug"/>
	<!-- <LoggerLogLevel loggername="app.program" loglevel="Debug"/> -->
	<!-- <DebugView active="false"/> -->
</IdeaDiagnosticsSettings>
```

The default value of Severity is **Information**. If it is changed to **Debug** or **Trace** more details are written to the log file.

