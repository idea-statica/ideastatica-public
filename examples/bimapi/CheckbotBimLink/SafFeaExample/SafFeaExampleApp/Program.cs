using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System;
using System.Windows.Forms;

namespace SafFeaExampleApp
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			SerilogFacade.Initialize("SafFeaExampleApp.log");
			IPluginLogger logger = LoggerProvider.GetLogger("SafFeaExampleApp");

			ApplicationConfiguration.Initialize();
			Application.Run(new MainForm(new FakeFeaApplication(), logger));
		}
	}
}
