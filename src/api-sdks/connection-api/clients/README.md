# Prerequisites

Clients communicate with the _Connection REST API_, which is implemented in _IdeaStatiCa.ConnectionRestApi.exe_ as an ASP.NET service. It requires installation of the .NET framework 'Microsoft.AspNetCore.App' (version 8.0.0) on the PC, but it is not part of the IDEA StatiCa setup. Users need to install it from the Microsoft webpage.

[ASP.NET Core 8.0 Runtime (v8.0.22) - Windows x64 Installer](https://aka.ms/dotnet-core-applaunch?framework=Microsoft.AspNetCore.App&framework_version=8.0.0&arch=x64&rid=win-x64&os=win10)

Users can verify the service is running correctly by running:

_C:\Program Files\IDEA StatiCa\StatiCa 25.1\IdeaStatiCa.ConnectionRestApi.exe_

from the Windows command line. It will start the service, which should listen on TCP port 5000 by default.

http://localhost:5000/index.html should display the OpenAPI page of the service.