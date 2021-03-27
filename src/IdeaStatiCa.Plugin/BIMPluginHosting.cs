using IdeaStatiCa.Plugin.Grpc;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	public interface IBIMPluginHosting
	{
		Task RunAsync(string id, string workingDirectory);

		event ISEventHandler AppStatusChanged;

		IApplicationBIM Service { get; }
	}

	public interface IBIMPluginFactory
	{
		IApplicationBIM Create();

		string FeaAppName { get; }

		string IdeaStaticaAppPath { get; }
	}

	public class BIMPluginHosting : IBIMPluginHosting, IDisposable
	{
		private Task hostingTask;
		private CancellationTokenSource tokenSource;
		private ManualResetEvent mre;
		private IApplicationBIM bimAppService;
		private readonly IBIMPluginFactory bimPluginFactory;
		private string clientId = string.Empty;
		private string workingDirectory = string.Empty;
		private readonly string EventName;
		private readonly string PluginUrlFormat;
		private IPluginLogger ideaLogger;
		string baseAddress;

#if DEBUG
		private readonly int OpenServerTimeLimit = -1;
#else
		readonly TimeSpan OpenServerTimeLimit = TimeSpan.FromMinutes(1);
#endif

		internal Process IdeaStaticaApp { get; private set; }

		internal string ServiceBaseAddress { get => baseAddress; set => baseAddress = value; }

		public BIMPluginHosting(IBIMPluginFactory factory, IPluginLogger logger = null, string eventName = Constants.DefaultPluginEventName, string pluginUrlFormat = Constants.DefaultPluginUrlFormat)
		{
			//Debug.Assert(false);
			mre = new ManualResetEvent(false);
			this.bimPluginFactory = factory;
			this.EventName = eventName;
			this.PluginUrlFormat = pluginUrlFormat;
			ideaLogger = logger ?? new NullLogger();
		}

		public event ISEventHandler AppStatusChanged;

		public Task RunAsync(string id, string workingDirectory)
		{
			if (hostingTask != null)
			{
				Debug.Fail("Task is running");
				return Task.CompletedTask;
			}

			tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			HostingTask = Task.Run(() =>
			{
				RunServer(id, workingDirectory, token);
			}, token);

			return HostingTask;
		}

		public void Stop()
		{
			if (hostingTask != null)
			{
				tokenSource.Cancel();
				var stopRes = mre.WaitOne();
				Debug.Assert(stopRes, "Can not stop");
				hostingTask = null;
				NotifyAppStatusChanged(AppStatus.Finished);
			}
		}

		private void RunServer(string id, string workingDirectory, System.Threading.CancellationToken cancellationToken)
		{
			clientId = id;
			this.workingDirectory = workingDirectory;
			// create the communication pipe for getting commands from IDEA StatiCa
			mre.Reset();
			bimAppService = bimPluginFactory?.Create();

			ServiceBaseAddress = string.Format(PluginUrlFormat, id);
			using (ServiceHost selfServiceHost = new ServiceHost(Service, new Uri(ServiceBaseAddress)))
			{
				((ServiceBehaviorAttribute)selfServiceHost.Description.
				Behaviors[typeof(ServiceBehaviorAttribute)]).InstanceContextMode
				= InstanceContextMode.Single;

				//Net named pipe
				NetNamedPipeBinding binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647 };
				binding.ReceiveTimeout = TimeSpan.MaxValue;
				selfServiceHost.AddServiceEndpoint(typeof(IApplicationBIM), binding, ServiceBaseAddress);

				//BasicHttpBinding httpBinding = new BasicHttpBinding { MaxReceivedMessageSize = 2147483647 };
				//httpBinding.ReceiveTimeout = TimeSpan.MaxValue;
				//selfServiceHost.AddServiceEndpoint(typeof(IApplicationBIM), httpBinding, "http://localhost/bim_p1");

				//MEX - Meta data exchange
				ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
				selfServiceHost.Description.Behaviors.Add(behavior);
				selfServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), ServiceBaseAddress + "/mex/");

				selfServiceHost.Opened += new EventHandler(SelfServiceHost_Opened);
				selfServiceHost.Faulted += SelfServiceHost_Faulted;
				selfServiceHost.Opening += SelfServiceHost_Opening;
				selfServiceHost.UnknownMessageReceived += SelfServiceHost_UnknownMessageReceived;
				selfServiceHost.Closing += SelfServiceHost_Closing;
				selfServiceHost.Closed += SelfServiceHost_Closed;

				selfServiceHost.Open(new TimeSpan(0, 0, 10));

				while (!cancellationToken.IsCancellationRequested)
				{
					Thread.Sleep(100);
				}

				try
				{
					selfServiceHost.Close();
					mre.Set();
				}
				catch { }
			}
		}

		private void SelfServiceHost_Closed(object sender, EventArgs e)
		{
			ideaLogger.LogDebug($"SelfServiceHost_Closed service '{ServiceBaseAddress}'");
			ServiceHost selfServiceHost = (ServiceHost)sender;
			if(selfServiceHost != null)
			{
				selfServiceHost.Opened -= new EventHandler(SelfServiceHost_Opened);
				selfServiceHost.Faulted -= SelfServiceHost_Faulted;
				selfServiceHost.Opening -= SelfServiceHost_Opening;
				selfServiceHost.UnknownMessageReceived -= SelfServiceHost_UnknownMessageReceived;
				selfServiceHost.Closing -= SelfServiceHost_Closing;
				selfServiceHost.Closed -= SelfServiceHost_Closed;
			}
		}

		private void SelfServiceHost_Closing(object sender, EventArgs e)
		{
			ideaLogger.LogDebug($"SelfServiceHost_Closing service '{ServiceBaseAddress}'");
		}

		private void SelfServiceHost_Opened(object sender, EventArgs e)
		{
			ideaLogger.LogDebug($"SelfServiceHost_Opened service '{ServiceBaseAddress}'");

			// run IDEA StatiCa
			IdeaStaticaApp = RunIdeaIdeaStatiCa(bimPluginFactory.IdeaStaticaAppPath, clientId);

			if (IdeaStaticaApp != null)
			{
				IdeaStaticaApp.Exited += new EventHandler(IS_Exited);
				NotifyAppStatusChanged(AppStatus.Started);

				if (bimAppService is ApplicationBIM appBim)
				{
					appBim.Id = IdeaStaticaApp.Id;
				}
			}
		}

		private void SelfServiceHost_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
		{
			ideaLogger.LogDebug($"SelfServiceHost_UnknownMessageReceived service '{ServiceBaseAddress}'");
		}

		private void SelfServiceHost_Opening(object sender, EventArgs e)
		{
			ideaLogger.LogDebug($"SelfServiceHost_Opening service '{ServiceBaseAddress}'");
		}

		private void SelfServiceHost_Faulted(object sender, EventArgs e)
		{
			ideaLogger.LogError($"SelfServiceHost_Faulted service '{ServiceBaseAddress}'", new Exception());
		}

		private void IS_Exited(object sender, EventArgs e)
		{
			try
			{
				IdeaStaticaApp.Exited -= new EventHandler(IS_Exited);
				IdeaStaticaApp.Dispose(); 
			}
			catch { }
			IdeaStaticaApp = null;

			Stop();
		}

		protected void NotifyAppStatusChanged(AppStatus newStatus)
		{
			AppStatusChanged?.Invoke(this, new ISEventArgs() { Status = newStatus });
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		public Task HostingTask { get => hostingTask; set => hostingTask = value; }
		public IApplicationBIM Service { get => bimAppService; }

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
				connectionProc.StartInfo = new ProcessStartInfo(exePath, $"{Constants.AutomationParam}:{id} {Constants.ProjectParam}:\"{workingDirectory}\"");
				connectionProc.EnableRaisingEvents = true;
				connectionProc.Start();

				if (!syncEvent.WaitOne(OpenServerTimeLimit))
				{
					syncEvent.Close();
					throw new CommunicationException(string.Format("Cannot start '{0}'", exePath));
				}
				syncEvent.Close();
			}

			return connectionProc;
		}

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
							tokenSource.Cancel();
							//hostingTask.Dispose();
						}
						catch { }

						try
						{
							//feaAppService.Dispose();
							//feaAppService = null;
						}
						catch { }
						mre.Dispose();
						tokenSource.Dispose();
					}
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

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

		#endregion IDisposable Support
	}
}