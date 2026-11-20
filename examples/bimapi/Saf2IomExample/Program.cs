using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.SAF2IOM;
using System;
using System.Windows;

namespace Saf2IomExample
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			SerilogFacade.Initialize("Saf2IomExample.log");
			IPluginLogger logger = LoggerProvider.GetLogger("Saf2IomExample");

			// SAF2IOM writes to this log what it converts, and what it skips.
			SafIomLogger.Init(logger);

			new Application().Run(new MainWindow(logger));
		}
	}
}
