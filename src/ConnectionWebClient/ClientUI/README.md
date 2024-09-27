# ConnectionWebClient 

enables to connect to _IdeaStatiCa.ConnectionRestApi.exe_ which hosts Web API. The server listens on the port 5000 but it can be changed by the parameter  

```
IdeaStatiCa.ConnectionRestApi.exe port=5000
```

To be able to present connections in a browser users need to unzip _client-ui.zip_ to _wwwroot_ subdirectory. The parent directory should include the file _IdeaStatiCa.ConnectionRestApi.exe_ which is the part of IDEA StatiCa setup (since v24.0).

It allows to visualize an open idea connection project in webbrowser which is connected to to IdeaStatiCa.ConnectionRestApi
