using Dlubal.RSTAB8;
using IdeaStatiCa.Plugin;
using System;
using System.Diagnostics;
using System.Reflection;
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

		private async static void PluginThread(object param)
		{
			Debug.Fail("Plugin for RSTAB is starting");
			//AppDomain domain = null;
			try
			{
				_logger.LogInformation("RSTAB Link started");

				string IdeaDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

				AppDomain.CurrentDomain.AssemblyResolve += Domain_AssemblyResolve; 
				PluginFactory pluginFactory = new PluginFactory((IModel)param, IdeaDirectory, _logger);

				// It will be used for gRPC communication
				var bimPluginHosting = new BIMPluginHostingGrpc(pluginFactory, _logger);
				//Run GRPC
				await bimPluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), pluginFactory.WorkingDirectory);				
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

				// house cleaning
				GC.Collect();
				GC.WaitForPendingFinalizers();

				// decrease a refcount on IModel
				Marshal.ReleaseComObject(param);
				_logger.LogInformation("RSTAB Link finished");


			}
		}

		private static Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string IdeaDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			if (args.Name.Contains("System.Runtime.CompilerServices.Unsafe")) //Only missing DLL
			{
				return Assembly.LoadFrom(System.IO.Path.Combine(IdeaDirectory, "System.Runtime.CompilerServices.Unsafe.dll")); //Resolve our missing DLL
			}
			return null;
		}
	}
}