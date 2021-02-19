## Beta version

To be able to define and modify parameters in idea connection project you need to modify the IdeaConnection.exe.config which is located in the installation directory (C:\Program Files\IDEA StatiCa\StatiCa 21.0) - add (or modify) the section appSettings. There should be value UserMode = 16

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
	<appSettings>
		<add key="UserMode" value="16" />
	</appSettings>
```

IdeaConnection.exe is run in the developer mode and user can define and set parameters for connection. See the project python-examples\projects\parameters-anchors.ideaCon

![Parameters in Idea Connection](images/parameters-in-idea-connection.png)

