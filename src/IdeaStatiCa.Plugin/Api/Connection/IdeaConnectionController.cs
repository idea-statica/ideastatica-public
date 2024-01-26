
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Utilities;
using IdeaStatiCa.Public;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace IdeaStatiCa.Plugin
{
	/// <inheritdoc cref="IConnectionController."/>
	public class IdeaConnectionController : IDisposable, IConnectionController
	{
		private readonly string IdeaInstallDir;
		private Process IdeaStatiCaProcess { get; set; }
		private IConnectionAutomation ConnectionAutomation { get; set; }

		private IGrpcBlobStorageClient grpcBlobStorageClient { get; set; }

		/// <inheritdoc cref="IConnectionController.ConnectionAppExited"/>
		public event EventHandler ConnectionAppExited;

		private int GrpcPort { get; set; }

		private IPluginLogger Logger { get; set; }

		protected EventWaitHandle CurrentItemChangedEvent;

		protected AutomationHostingGrpc<IAutomation, IAutomation> GrpcClient { get; set; }

		protected virtual uint UserMode { get; } = 0;

		/// <inheritdoc cref="IConnectionController.IsConnected"/>
		bool IConnectionController.IsConnected => GrpcClient?.IsConnected == true;

#if DEBUG
		private int StartTimeout = -1;
#else
		int StartTimeout = 1000 * 20;
#endif

		protected IdeaConnectionController(string ideaInstallDir, IPluginLogger logger)
		{
			Logger = logger ?? new NullLogger();
			if (!Directory.Exists(ideaInstallDir))
			{
				throw new ArgumentException($"IdeaConnectionController.IdeaConnectionController - directory '{ideaInstallDir}' doesn't exist");
			}

			IdeaInstallDir = ideaInstallDir;
		}

		/// <summary>
		/// Creates connection and starts IDEA StatiCa connection application.
		/// Call OpenProjectAsync after this method to open specific project.
		/// </summary>
		/// <param name="ideaInstallDir">IDEA StatiCa installation directory.</param>
		/// <returns>A controller object.</returns>
		public static IConnectionController Create(string ideaInstallDir)
		{
			return Create(ideaInstallDir, new NullLogger());
		}

		/// <summary>
		/// Creates connection and starts IDEA StatiCa connection application.
		/// Call OpenProjectAsync after this method to open specific project.
		/// </summary>
		/// <param name="ideaInstallDir">IDEA StatiCa installation directory.</param>
		/// <param name="logger">The pluginLogger.</param>
		/// <returns>A controller object.</returns>
		public static IConnectionController Create(string ideaInstallDir, IPluginLogger logger)
		{
			IdeaConnectionController connectionController = new IdeaConnectionController(ideaInstallDir, logger);
			connectionController.OpenConnectionClient();
			return connectionController;
		}

		/// <inheritdoc cref="IConnectionController.OpenProject(string)"/>
		public int OpenProject(string fileName)
		{
			GrpcClient.MyBIM.OpenProject(fileName);
			return 1;
		}

		/// <inheritdoc cref="IConnectionController.CloseProject"/>
		public int CloseProject()
		{
			GrpcClient.MyBIM.CloseProject();
			return 1;
		}

		/// <inheritdoc cref="IConnectionController.GenerateReport(int, ReportSettings)"/>
		public IBlobStorage GenerateReport(int conId, ConnReportSettings settings)
		{
			var reportId = ConnectionAutomation.GenerateReport(conId, settings);
			var blobStorage = new BlobStorageGrpc(grpcBlobStorageClient, reportId);
			return blobStorage;
		}

		/// <inheritdoc cref="IConnectionController.GenerateWordReport(int, string, ConnReportSettings)"/>
		public void GenerateWordReport(int conId, string filePath, ConnReportSettings settings)
		{
			ConnectionAutomation.GenerateWordReport(conId, filePath, settings);
		}

		/// <inheritdoc cref="IConnectionController.GeneratePdfReport(int, string, ConnReportSettings)"/>
		public void GeneratePdfReport(int conId, string filePath, ConnReportSettings settings)
		{
			ConnectionAutomation.GeneratePdfReport(conId, filePath, settings);
		}

		protected void OpenConnectionClient()
		{
			OpenConnectionClientGrpc();
		}

		private void OpenConnectionClientGrpc()
		{
			int processId = Process.GetCurrentProcess().Id;
			GrpcPort = PortFinder.FindPort(Constants.MinGrpcPort, Constants.MaxGrpcPort);
			string connChangedEventName = string.Format(Constants.ConnectionChangedEventFormat, processId);
			CurrentItemChangedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, connChangedEventName);

			string applicationExePath = Path.Combine(IdeaInstallDir, Constants.IdeaConnectionAppName);

			if (!File.Exists(applicationExePath))
			{
				throw new ArgumentException($"IdeaConnectionController.OpenConnectionClient - file '{applicationExePath}' doesn't exist");
			}

			Process connectionProc = new Process();
			string eventName = string.Format("IdeaStatiCaEvent{0}", processId);
			using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
			{
				connectionProc.StartInfo = new ProcessStartInfo(applicationExePath, $"-cmd:automation-{processId} {IdeaStatiCa.Plugin.Constants.GrpcControlPortParam}:{GrpcPort} user-mode 192");
				connectionProc.EnableRaisingEvents = true;
				connectionProc.Start();

				if (!syncEvent.WaitOne(StartTimeout))
				{
					throw new TimeoutException($"Time out - process '{applicationExePath}' doesn't set the event '{eventName}'");
				}
			}

			IdeaStatiCaProcess = connectionProc;
			var grpcClient = new GrpcClient(Logger);

			var grpcClientTask = grpcClient.StartAsync(processId.ToString(), GrpcPort);

			GrpcClient = new AutomationHostingGrpc<IAutomation, IAutomation>(null, grpcClient, Logger);
			GrpcClient.RunAsync(processId.ToString());

			var grpcConnectionHandler = new Grpc.Reflection.GrpcMethodInvokerHandler(IdeaStatiCa.Plugin.Constants.GRPC_CONNECTION_HANDLER_MESSAGE, grpcClient, Logger);
			ConnectionAutomation = GrpcReflectionServiceFactory.CreateInstance<IConnectionAutomation>(grpcConnectionHandler);
			grpcClient.RegisterHandler(IdeaStatiCa.Plugin.Constants.GRPC_CONNECTION_HANDLER_MESSAGE, grpcConnectionHandler);

			grpcBlobStorageClient = new GrpcBlobStorageClient(Logger, GrpcPort);

			IdeaStatiCaProcess.Exited += CalculatorProcess_Exited;
		}

		private void CalculatorProcess_Exited(object sender, EventArgs e)
		{
			if (IdeaStatiCaProcess == null)
			{
				return;
			}

			IdeaStatiCaProcess.Dispose();
			IdeaStatiCaProcess = null;

			if (ConnectionAppExited != null)
			{
				ConnectionAppExited(this, e);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ConnectionControllerFactory()
		// {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
