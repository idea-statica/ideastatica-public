using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Responsible for controlling the connected BIM application to IS
	/// </summary>
	/// <typeparam name="T">Type of the plugin's service contract</typeparam>
	public interface IBIMPluginClient<T>
	{
		/// <summary>
		/// Used for calling methods of connected BIM application
		/// </summary>
		T MyBIM { get; }

		/// <summary>
		/// Notification about events in the connected BIM application
		/// </summary>
		event ISEventHandler BIMStatusChanged;

		/// <summary>
		/// Starts BIM application
		/// </summary>
		/// <param name="id">Identified of </param>
		/// <returns></returns>
		Task RunAsync(string id);

		/// <summary>
		/// Stops BIM application
		/// </summary>
		void Stop();

		/// <summary>
		/// Get status of the BIM application
		/// </summary>
		AutomationStatus Status
		{
			get;
		}
	}

	/// <summary>
	/// Responsible of hosting an automation service on net.pipe endpoint
	/// </summary>
	/// <typeparam name="MyInterface"></typeparam>
	/// <typeparam name="ClientInterface"></typeparam>
	public class AutomationHosting<MyInterface, ClientInterface> : IBIMPluginClient<ClientInterface>, IDisposable where MyInterface : class where ClientInterface : class
	{
		private Task hostingTask;
		private CancellationTokenSource tokenSource;
		private ManualResetEvent mre;
		private MyInterface automation;
		private IdeaStatiCaClient<ClientInterface> bimClient;
		private Process bimProcess = null;
		private int myAutomatingProcessId;
		private readonly string EventName;
		private readonly string ClientUrlFormat;
		private readonly string AutomationUrlFormat;
		private readonly ILogger ideaLogger = null;

#if DEBUG
		private readonly TimeSpan OpenServerTimeLimit = TimeSpan.MaxValue;
#else
		private readonly TimeSpan OpenServerTimeLimit = TimeSpan.FromMinutes(1);
#endif

		public AutomationHosting(MyInterface hostedService, 
			ILogger logger = null,
			string eventName = Constants.DefaultPluginEventName,
			string clientUrlFormat = Constants.DefaultPluginUrlFormat,
			string automationUrlFormat = Constants.DefaultIdeaStaticaAutoUrlFormat)
		{
			ideaLogger = logger ?? NullLogger.Instance;

			//ideaLogger = Diagnostics.IdeaDiagnostics.GetLogger("ideastatica.plugin.automationhosting");
			this.Status = AutomationStatus.Unknown;
			this.automation = hostedService;
			this.EventName = eventName;
			this.ClientUrlFormat = clientUrlFormat;
			this.AutomationUrlFormat = automationUrlFormat;
			mre = new ManualResetEvent(false);
		}

		public event ISEventHandler BIMStatusChanged;

		public AutomationStatus Status { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">The identifier if BIM application (its process id)</param>
		/// <returns>Running task. </returns>
		public Task RunAsync(string id)
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
				try
				{
					RunServer(id, token);
				}
				catch(Exception e)
				{
					ideaLogger.LogError("RunAsync  RunServer failed", e);
					throw;
				}
			}, token);

			return HostingTask;
		}

		public ClientInterface MyBIM
		{
			get
			{
				if (bimClient == null || bimClient.State != CommunicationState.Opened)
				{
					return null;
				}

				return bimClient.Service;
			}
		}

		public void Stop()
		{
			if (hostingTask != null)
			{
				tokenSource.Cancel();
				var stopRes = mre.WaitOne();
				Debug.Assert(stopRes, "Can not stop");
			}
		}

		protected virtual void RunServer(string id, System.Threading.CancellationToken cancellationToken)
		{
			ideaLogger.LogInformation("Calling RunServer");

			mre.Reset();

			bool isBimRunning = false;
			if (!string.IsNullOrEmpty(id))
			{
				// try to attach to the service which is hosted in a BIM application
				try
				{
					myAutomatingProcessId = int.Parse(id);
					ideaLogger.LogInformation($"RunServer - processId == '{myAutomatingProcessId}'");

					bimProcess = Process.GetProcessById(myAutomatingProcessId);
					bimProcess.EnableRaisingEvents = true;
					bimProcess.Exited += new EventHandler(BimProcess_Exited);

					// Connect to the pipe
					var feaPluginUrl = string.Format(ClientUrlFormat, id);

					NetNamedPipeBinding pluginBinding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };

					bimClient = new IdeaStatiCaClient<ClientInterface>(pluginBinding, new EndpointAddress(feaPluginUrl));
					bimClient.Open();

					int counter = 0;
					while (bimClient.State != CommunicationState.Opened)
					{
						Thread.Sleep(100);
						if (counter > 200)
						{
							throw new CommunicationException("Can not open client");
						}
						counter++;
					}

					if (automation != null)
					{
						// service was injected
						if (automation is IClientBIM<ClientInterface> clientBIM)
						{
							clientBIM.BIM = bimClient.Service;
						}
					}

					Status |= AutomationStatus.IsClient;
					isBimRunning = true;
				}
				catch (Exception e)
				{
					ideaLogger.LogError("Can not attach to BIM application", e);
					throw;
				}
			}

			if(!isBimRunning)
			{
				bimProcess = null;
				myAutomatingProcessId = -1;
				if (automation != null)
				{
					// service was injected, set client's interface
					if (automation is IClientBIM<ClientInterface> clientBIM)
					{
						clientBIM.BIM = null;
					}
				}
			}

			var myProcess = Process.GetCurrentProcess();
			int myProcessId = myProcess.Id;

			string baseAddress = string.Format(AutomationUrlFormat, myProcessId);
			ideaLogger.LogInformation($"RunServer - Starting Automation service listening on '{baseAddress}'");

			// expose my IAutomation interface
			using (ServiceHost selfServiceHost = new ServiceHost(automation, new Uri(baseAddress)))
			{
				((ServiceBehaviorAttribute)selfServiceHost.Description.
				Behaviors[typeof(ServiceBehaviorAttribute)]).InstanceContextMode
				= InstanceContextMode.Single;

				//Net named pipe
				NetNamedPipeBinding binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647 };
				binding.ReceiveTimeout = TimeSpan.MaxValue;
				selfServiceHost.AddServiceEndpoint(typeof(MyInterface), binding, baseAddress);

				//MEX - Meta data exchange
				ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
				selfServiceHost.Description.Behaviors.Add(behavior);
				selfServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), baseAddress + "/mex");

				selfServiceHost.Open(OpenServerTimeLimit);

				if (!string.IsNullOrEmpty(id))
				{
					// notify plugin that service is running
					string myEventName = string.Format("{0}{1}", EventName, id);
					EventWaitHandle syncEvent;
					if (EventWaitHandle.TryOpenExisting(myEventName, out syncEvent))
					{
						syncEvent.Set();
						syncEvent.Dispose();
					}
				}

#if DEBUG
				foreach (var endpoint in selfServiceHost.Description.Endpoints)
				{
					Debug.WriteLine("{0} ({1})", endpoint.Address.ToString(), endpoint.Binding.Name);
				}
#endif

				NotifyBIMStatusChanged(AppStatus.Started);

				while (!cancellationToken.IsCancellationRequested)
				{
					Thread.Sleep(100);
				}

				ideaLogger.LogInformation($"RunServer - Automation Service has been stopped");

				try
				{
					if (bimClient != null)
					{
						bimClient.Close();
						bimClient = null;
					}
				}
				catch {}

				try
				{
					if (selfServiceHost != null)
					{
						selfServiceHost.Close();
					}
				}
				catch { }

				NotifyBIMStatusChanged(AppStatus.Finished);

				mre.Set();
			}
		}

		protected virtual void NotifyBIMStatusChanged(AppStatus newStatus)
		{
			BIMStatusChanged?.Invoke(this, new ISEventArgs() { Status = newStatus });
		}

		private void BimProcess_Exited(object sender, EventArgs e)
		{
			bimProcess.Exited -= new EventHandler(BimProcess_Exited);
			Status &= ~AutomationStatus.IsClient;
			bimProcess.Dispose();
			bimProcess = null;
			myAutomatingProcessId = -1;

			Stop();
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		public Task HostingTask { get => hostingTask; set => hostingTask = value; }
		public MyInterface Service { get => automation; }

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
						catch { }

						try
						{
							IDisposable disp = Service as IDisposable;
							if (disp != null)
							{
								disp.Dispose();
							}
							//feaAppService.Dispose();
							//feaAppService = null;
						}
						catch { }

						if (bimProcess != null)
						{
							bimProcess.Dispose();
							bimProcess = null;
						}

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