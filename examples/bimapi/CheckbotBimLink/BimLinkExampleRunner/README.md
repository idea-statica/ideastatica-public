# BimApiLink example runner

A small window that starts the [CAD link example](../CadExample/README.md) or the
[FEA link example](../FeaExample/README.md), so you can try them without a CAD or FEA application. It shows the log
of the link while Checkbot is open.

## What it shows

- Starting a link from an application: the window calls `CheckbotLink.Run` of the link from a command.
- Running the link on a pool thread. `CheckbotLink.Run` waits a few seconds for Checkbot to start before it returns
  its task, and the task ends when the user closes Checkbot. The command stays off until then.
- Showing the log of the link: `AppLogger` implements `IPluginLogger` and adds each message to the window.

Start reading at `ViewModels/MainWindowViewModel.cs`, then `ViewModels/AppLogger.cs`. The rest is the window.

## Requirements

- Windows with IDEA StatiCa installed, in the version the README of the link asks for.
- The .NET 10 SDK.

The runner targets .NET Framework 4.8 and .NET 10. All packages come from nuget.org.

## Run it

1. Build `BimLinkExampleRunner`. It references the CAD and the FEA link, so both are built with it.
2. Start `BimLinkExampleRunner`.
3. Choose **CAD** or **FEA** in the list.
4. Check the Checkbot location. It starts as `C:\Program Files\IDEA StatiCa\StatiCa 26.1\IdeaCheckbot.exe`.
   **Run IDEA StatiCa Checkbot** is off until the location is an existing file.
5. Click **Run IDEA StatiCa Checkbot**. Checkbot opens after a few seconds. The button stays off until you close
   Checkbot.
6. In Checkbot, go on with the README of the link: [CAD](../CadExample/README.md#run-it) or
   [FEA](../FeaExample/README.md#run-it).

The log shows errors in red, warnings in orange, information in blue and debug messages in grey. Ctrl+C copies the
selected message with its exception, for example to report a problem. **Clear log** empties the log.

The runner keeps the link you chose and the Checkbot location for its next start. The Checkbot project of each link is
kept next to the runner, in the folder `CadExampleCheckbotProject` or `FeaExampleCheckbotProject`, and the next run
opens it again. Delete the folder to start over.

## In your own application

Start the link the same way: call `CheckbotLink.Run` from a command, on a pool thread, and keep the command off until
its task ends. Give it a logger of your own, for example `LoggerProvider.GetLogger` of the
[IdeaStatiCa.PluginLogger](https://www.nuget.org/packages/IdeaStatiCa.PluginLogger) package, which writes a log file.
The [SafFeaExample](../SafFeaExample/README.md) uses it.
