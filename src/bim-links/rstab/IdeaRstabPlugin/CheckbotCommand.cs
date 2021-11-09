using Dlubal.RSTAB8;
using IdeaRstabPlugin.BimApi;
using IdeaStatiCa.Diagnostics;
using IdeaStatiCa.Diagnostics.PluginAdapter;
using IdeaStatiCa.Plugin;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace IdeaRstabPlugin
{
	[ComVisible(true)]
	public class CheckbotCommand : IExternalCommand
	{
		private readonly static IIdeaLogger _logger = IdeaDiagnostics.GetLogger("ideastatica.IdeaRstabPlugin");

		public CheckbotCommand()
		{
			IdeaDiagnostics.Init(
				logToFileName: "IdeaRstabPlugin.log",
				sentryDsn: "https://1dd058bc68994ea78f1b7d1125225e69@o330948.ingest.sentry.io/5465617",
				logToGoogleAnalytics: true,
				applicationName: "IdeaRstabPlugin",
				applicationId: "IdeaRstabPlugin");
		}

		public void Execute(object Model, string Params)
		{
			IdeaStatiCa.Diagnostics.Tools.AttachDebugger("IDEA_RSTAB_IMPORT");

			if (!(Model is IModel rstabModel))
			{
				throw new ArgumentException($"{nameof(Model)} must be instance of {nameof(IModel)}.");
			}

			// RSTAB is blocked during execution of this method so we start a new thread
			// where we can do whatever we need to.
			Thread pluginThread = new Thread(PluginThread)
			{
				IsBackground = true
			};
			pluginThread.Start(rstabModel);
		}

		private static async void PluginThread(object param)
		{
			try
			{
				_logger.LogEventInformation(new ApplicationStartedEvent());

				PluginFactory pluginFactory = new PluginFactory((IModel)param, new PluginLogger(_logger));
				using (BIMPluginHosting pluginHosting = new BIMPluginHosting(pluginFactory))
				{
					await pluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), pluginFactory.WorkingDirectory);
				}
			}
			catch (Exception e)
			{
				_logger.LogCritical("RSTAB link crashed", e);
			}
			finally
			{
				// Here we need to manually release the COM object or RSTAB will hang on exit.

				// This is most likely because the GC thinks (and is probably right)
				// that the COM object is held by some native code (RSTAB in this case)
				// so it never decreases the refcount. This prevents RSTAB from exiting properly.

				// decrease a refcount on IModel
				Marshal.ReleaseComObject(param);

				// house cleaning
				GC.Collect();
				GC.WaitForPendingFinalizers();

				_logger.LogEventInformation(new ApplicationExitedEvent(0));
			}
		}
	}
}