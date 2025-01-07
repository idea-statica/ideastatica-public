using Dlubal.RSTAB8;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Utilities;
using IdeaStatiCa.PluginLogger;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace IdeaRstabPlugin
{
	[ComVisible(true)]
	[Guid("E3FC6C61-64D2-479F-A103-0ADD30F89730")]
	public class CheckbotCommand : IExternalCommand
	{
		private readonly static IPluginLogger _logger;

		public CheckbotCommand()
		{
			AppDomain.CurrentDomain.AssemblyResolve += IdeaStatiCa.Public.Tools.AssemblyResolver.Domain_AssemblyResolve;
		}

		static CheckbotCommand()
		{
			// set the name of the logfile
			SerilogFacade.Initialize("IdeaRstabPlugin.log");
			_logger = LoggerProvider.GetLogger("bim.rstab.bimapi");
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
			//Debug.Fail("Plugin for RSTAB is starting");

			try
			{
				_logger.LogInformation("RSTAB Link started");

				string IdeaDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

				PluginFactory pluginFactory = new PluginFactory((IModel)param, _logger);

				int clientId = Process.GetCurrentProcess().Id;
				int grpcPort = PortFinder.FindPort(Constants.MinGrpcPort, Constants.MaxGrpcPort);

				// run gRPC server
				var grpcServer = new GrpcServer(_logger, new IdeaStatiCa.Plugin.Grpc.Services.GrpcService(_logger), null);

				var gRPCtask = grpcServer.StartAsync(clientId.ToString(), grpcPort);

				var bimPluginHosting = new BIMPluginHostingGrpc(pluginFactory, grpcServer, _logger);

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
	}
}