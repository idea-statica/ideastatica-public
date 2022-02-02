using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.Utilities;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Grpc Implementation of <see cref="BIMPluginHosting"/>
	/// </summary>
	public class BIMPluginHostingGrpc : IBIMPluginHosting, IDisposable
	{
		public GrpcReflectionServer GrpcServer { get; private set; }
		private CancellationTokenSource tokenSource;
		private ManualResetEvent mre;
		private IApplicationBIM bimAppService;
		private Task hostingTask;
		private bool disposedValue = false;

		private readonly IBIMPluginFactory bimPluginFactory;
		private string clientId = string.Empty;
		private string workingDirectory = string.Empty;
		private IPluginLogger ideaLogger;

		/// <summary>
		/// Triggered when app status changes.
		/// </summary>
		public event ISEventHandler AppStatusChanged;

		/// <summary>
		/// App process
		/// </summary>
		internal Process IdeaStaticaApp { get; private set; }

		internal IGrpcCommunicator GrpcCommunicator { get; private set; }

		/// <summary>
		/// BIM Service.
		/// </summary>
		public IApplicationBIM Service
		{
			get => bimAppService;
			set => bimAppService = value;
		}

		/// <summary>
		/// System event name.
		/// </summary>
		public string EventName { get; set; }

#if DEBUG
		private readonly int OpenServerTimeLimit = -1;
#else
		readonly TimeSpan OpenServerTimeLimit = TimeSpan.FromMinutes(1);
#endif

		public BIMPluginHostingGrpc(IBIMPluginFactory factory, IGrpcCommunicator grpcCommunicator, IPluginLogger logger = null, string eventName = Constants.DefaultPluginEventName)
		{
			this.EventName = eventName;
			this.GrpcCommunicator = grpcCommunicator;
			mre = new ManualResetEvent(false);
			bimPluginFactory = factory;
			ideaLogger = logger ?? new NullLogger();
			tokenSource = new CancellationTokenSource();
			hostingTask = null;
			Service = bimPluginFactory.Create();
			grpcCommunicator.RegisterHandler(Constants.GRPC_REFLECTION_HANDLER_MESSAGE, new GrpcReflectionMessageHandler(Service, logger));
		}

		public Task RunAsync(string id, string workingDirectory)
		{
			ideaLogger.LogDebug($"RunAsync id = {id}, workingDirectory = '{workingDirectory}'");

			if (hostingTask != null)
			{
				Debug.Fail("Task is running");

				ideaLogger.LogInformation("Starting BIM Plugin Hosting");

				return Task.CompletedTask;
			}

			tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			hostingTask = Task.Run(() =>
			{
				RunServer(id, workingDirectory, token);
			}, token);

			return hostingTask;
		}

		/// <summary>
		/// Stops the plugin host.
		/// </summary>
		public void Stop()
		{
			if (hostingTask != null)
			{
				tokenSource.Cancel();

				var stopRes = mre.WaitOne();

				Debug.Assert(stopRes, "Can not stop");

				ideaLogger.LogInformation("Stopping BIM Plugin Hosting");

				hostingTask = null;

				RaiseAppStatusChanged(AppStatus.Finished);
			}
		}

		private void RunServer(string id, string workingDirectory, System.Threading.CancellationToken cancellationToken)
		{
			ideaLogger.LogInformation($"BIMPluginHostingGrpc id = '{id}', workingDirectory = '{workingDirectory}'");

			clientId = id;
			this.workingDirectory = workingDirectory;

			mre.Reset();

			// Open IDEA StatiCa
			IdeaStaticaApp = RunIdeaIdeaStatiCa(bimPluginFactory.IdeaStaticaAppPath, clientId);

			if (IdeaStaticaApp != null)
			{
				IdeaStaticaApp.Exited += OnIdeaStatiCaAppExit;

				RaiseAppStatusChanged(AppStatus.Started);

				if (bimAppService is ApplicationBIM appBim)
				{
					appBim.Id = IdeaStaticaApp.Id;
				}
			}

			// Handle cancellation token
			while (!cancellationToken.IsCancellationRequested)
			{
				Thread.Sleep(100);
			}

			mre.Set();
		}

		private void OnIdeaStatiCaAppExit(object sender, EventArgs e)
		{
			try
			{
				IdeaStaticaApp.Dispose();
			}
			catch (Exception ex)
			{
				ideaLogger.LogDebug("Disposing IdeaStaticaApp failed", ex);
			}
			IdeaStaticaApp = null;

			Stop();
		}

		/// <summary>
		/// Runs the IDEA StatiCA app.
		/// </summary>
		/// <param name="exePath">Path to executable.</param>
		/// <param name="id">Client id.</param>
		/// <returns></returns>
		private Process RunIdeaIdeaStatiCa(string exePath, string id)
		{
			if (exePath == null)
			{
				return null;
			}

			Process connectionProc = new Process();

			string eventName = string.Format("{0}{1}", EventName, id);
			using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
			{
				// disable only recent files
				connectionProc.StartInfo = new ProcessStartInfo(exePath, $"{Constants.AutomationParam}:{id} {Constants.ProjectParam}:\"{workingDirectory}\" {Constants.GrpcPortParam}:{GrpcCommunicator.Port}");
				connectionProc.EnableRaisingEvents = true;
				connectionProc.Start();

				if (!syncEvent.WaitOne(OpenServerTimeLimit))
				{
					syncEvent.Close();

					ideaLogger.LogDebug($"Cannot start '{exePath}', throwing exception.");

					throw new CommunicationException($"Cannot start '{exePath}'.");
				}
				syncEvent.Close();
			}

			return connectionProc;
		}

		/// <summary>
		/// Triggers the <see cref="AppStatusChanged"/> event.
		/// </summary>
		/// <param name="status"></param>
		private void RaiseAppStatusChanged(AppStatus status)
		{
			AppStatusChanged?.Invoke(this, new ISEventArgs() { Status = status });
		}

		#region IDisposable
		/// <summary>
		/// Disposes current plugin host.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (hostingTask != null)
					{
						try
						{
							Stop();
						}
						catch (Exception ex)
						{
							ideaLogger.LogDebug("Canceling thread failed", ex);
						}
						mre.Dispose();
						tokenSource.Dispose();
					}
				}
				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~AutomationHosting() {
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
