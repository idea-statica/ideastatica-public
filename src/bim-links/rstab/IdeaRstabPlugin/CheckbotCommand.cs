using Dlubal.RSTAB8;
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
		private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

		public CheckbotCommand()
		{
		}

		public void Execute(object Model, string Params)
		{
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
			//Debug.Fail("Plugin for RSTAB is starting");
			try
			{
				_logger.LogInformation("RSTAB Link started");

				PluginFactory pluginFactory = new PluginFactory((IModel)param, _logger);

				// TODO - it will be used for gRPC communication
				//using (var bimPluginHosting = new BIMPluginHostingGrpc(pluginFactory, _logger))
				//{
				//	await bimPluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), pluginFactory.WorkingDirectory);
				//}

				using (BIMPluginHosting pluginHosting = new BIMPluginHosting(pluginFactory))
				{
					await pluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), pluginFactory.WorkingDirectory);
				}
			}
			catch (Exception e)
			{
				_logger.LogError("RSTAB link crashed", e);
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

				_logger.LogInformation("RSTAB Link finished");
			}
		}
	}
}