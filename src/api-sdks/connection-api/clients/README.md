# Prerequisites

Clients communicate with the _Connection REST API_, which is implemented in _IdeaStatiCa.ConnectionRestApi.exe_ as an ASP.NET service. Since IDEA StatiCa migrated to .NET 10, it requires the .NET framework 'Microsoft.AspNetCore.App' (version 10.0.0) on the PC. If it is missing, install it from the Microsoft webpage.

[ASP.NET Core 10.0 Runtime - Windows x64 Installer](https://aka.ms/dotnet-core-applaunch?framework=Microsoft.AspNetCore.App&framework_version=10.0.0&arch=x64&rid=win-x64&os=win10)

Users can verify the service is running correctly by running:

_C:\Program Files\IDEA StatiCa\StatiCa 26.0\IdeaStatiCa.ConnectionRestApi.exe_

from the Windows command line. It will start the service, which should listen on TCP port 5000 by default.

http://localhost:5000/index.html should display the OpenAPI page of the service.