# BimApiLink console example

A console application that starts the [CAD link example](../CadExample/README.md) without a window. It is the
smallest application that runs a link: it opens Checkbot, writes the log of the link to the console, and ends when
the user closes Checkbot.

## What it shows

- Running a link from a console: `Main` awaits `CheckbotLink.Run` of the link. `CheckbotLink.Run` waits a few
  seconds for Checkbot to start before it returns its task, and the task ends when the user closes Checkbot. A
  console has no window to keep responsive, so `Main` simply waits. An application with a window starts the link on
  a pool thread instead, as the [BimLinkExampleRunner](../BimLinkExampleRunner/README.md) does.
- Writing the log of the link: `ConsoleLogger` implements `IPluginLogger` and writes each message to the console.

Start reading at `Program.cs`, then `ConsoleLogger.cs`.

## Requirements

- Windows with IDEA StatiCa installed, in the version the [README of the CAD link](../CadExample/README.md#requirements)
  asks for.
- The .NET 10 SDK.

The app targets .NET Framework 4.8 and .NET 10. All packages come from nuget.org.

## Run it

1. Build `BimLinkExampleConsoleApp`. It references the CAD link, so the link is built with it.
2. Start `BimLinkExampleConsoleApp`, from Visual Studio or from a console in its output folder. It looks for Checkbot
   at `C:\Program Files\IDEA StatiCa\StatiCa 26.1\IdeaCheckbot.exe`. If yours is elsewhere, give the full path to
   `IdeaCheckbot.exe` as the first argument:

   ```
   BimLinkExampleConsoleApp.exe "D:\IDEA StatiCa\StatiCa 26.1\IdeaCheckbot.exe"
   ```

3. Checkbot opens after a few seconds. Go on with the [README of the CAD link](../CadExample/README.md#run-it), from
   the step where Checkbot opens.
4. Close Checkbot. The app ends.

The console shows the log of the link: each error, warning, information and debug message with its time. The app
ends with exit code 0 when the user closes Checkbot, and 1 when Checkbot is not found or the link fails.

The Checkbot project is kept next to the app, in the folder `CadExampleCheckbotProject`, and the next run opens it
again. Delete the folder to start over.

## In your own application

Your CAD application calls `CheckbotLink.Run` from a command, as the
[BimLinkExampleRunner](../BimLinkExampleRunner/README.md) does. Give the link a logger of your own, for example
`LoggerProvider.GetLogger` of the [IdeaStatiCa.PluginLogger](https://www.nuget.org/packages/IdeaStatiCa.PluginLogger)
package, which writes a log file. The [SafFeaExample](../SafFeaExample/README.md) uses it.
