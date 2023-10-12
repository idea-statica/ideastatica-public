#if NET48

using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Responsible of hosting an automation service on net.pipe endpoint
	/// </summary>
	/// <typeparam name="MyInterface"></typeparam>
	/// <typeparam name="ClientInterface"></typeparam>
	public class AutomationHosting<MyInterface, ClientInterface> : IBIMPluginClient<ClientInterface>, IDisposable where MyInterface : class where ClientInterface : class
	{
		private Task hostingTask;
		private CancellationTokenSource serverStopRequestTokenSource;
		private ManualResetEvent serverStoppedEvent;
		private MyInterface automation;
		private IdeaStatiCaClient<ClientInterface> bimClient;
		private Process bimProcess = null;
		private int myAutomatingProcessId;
		private readonly string EventName;
		private readonly string ClientUrlFormat;
		private readonly string AutomationUrlFormat;
		private readonly IPluginLogger ideaLogger = null;
		string baseAddress;

#if DEBUG
		private readonly TimeSpan OpenServerTimeLimit = TimeSpan.MaxValue;
#else
		private readonly TimeSpan OpenServerTimeLimit = TimeSpan.FromMinutes(1);
#endif
			
		public AutomationHosting(MyInterface hostedService, 
			IPluginLogger logger = null,
			string eventName = Constants.DefaultPluginEventName,
			string clientUrlFormat = Constants.DefaultPluginUrlFormat,
			string automationUrlFormat = Constants.DefaultIdeaStaticaAutoUrlFormat)
		{
			ideaLogger = logger ?? new NullLogger();

			this.Status = AutomationStatus.Unknown;
			this.automation = hostedService;
			this.EventName = eventName;
			this.ClientUrlFormat = clientUrlFormat;
			this.AutomationUrlFormat = automationUrlFormat;
			serverStoppedEvent = new ManualResetEvent(false);
		}

		public event ISEventHandler BIMStatusChanged;

		public AutomationStatus Status { get; private set; }


		/// <inheritdoc cref="RunAsync(string)"/>
		public Task RunAsync(string id)
		{
			ideaLogger.LogDebug($"RunAsync id = '{id}'");

			if (hostingTask != null)
			{
				Debug.Fail("Task is running");
				return Task.CompletedTask;
			}

			serverStopRequestTokenSource = new CancellationTokenSource();
			var token = serverStopRequestTokenSource.Token;

			ideaLogger.LogDebug("Reseting the server stopped event.");
			serverStoppedEvent.Reset();

			HostingTask = Task.Run(() =>
			{
				try
				{
					ideaLogger.LogDebug("RunAsync - calling RunServer");
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
				ideaLogger.LogDebug("Setting the server stop request.");
				serverStopRequestTokenSource.Cancel();

				ideaLogger.LogDebug("Waiting for the server stopped event...");
				var stopRes = serverStoppedEvent.WaitOne();
				ideaLogger.LogDebug("Server stopped event was set.");
			}
		}

		protected virtual void RunServer(string id, System.Threading.CancellationToken cancellationToken)
		{
			ideaLogger.LogInformation($"Starting server processId = '{id}'");
			try
			{
				// if the process id to connect was provided
				if (!string.IsNullOrEmpty(id))
				{
					// try to attach to the service which is hosted in a BIM application
					try
					{
						ideaLogger.LogDebug($"RunServer - Connecting process '{id}'");

						myAutomatingProcessId = int.Parse(id);

						bimProcess = Process.GetProcessById(myAutomatingProcessId);
						bimProcess.EnableRaisingEvents = true;
						bimProcess.Exited += new EventHandler(BimProcess_Exited);

						// Connect to the pipe
						var feaPluginUrl = string.Format(ClientUrlFormat, id);

						ideaLogger.LogDebug($"RunServer - Connecting to windows pipe == '{feaPluginUrl}'");

						NetNamedPipeBinding pluginBinding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };

						bimClient = new IdeaStatiCaClient<ClientInterface>(pluginBinding, new EndpointAddress(feaPluginUrl));
						bimClient.Open();

						int counter = 0;
						while (bimClient.State != CommunicationState.Opened)
						{
							Thread.Sleep(100);
							if (counter > 200)
							{
								ideaLogger.LogInformation($"Could not open client '{feaPluginUrl}' within 20s timeout. Throwing an exception.");
								throw new InvalidOperationException("Could not open client '{feaPluginUrl}' within 20s timeout.");
							}
							counter++;
						}

						if (automation != null)
						{
							// service was injected
							ideaLogger.LogDebug($"RunServer - injected service '{automation.GetType().ToString()}'");
							if (automation is IClientBIM<ClientInterface> clientBIM)
							{
								clientBIM.BIM = bimClient.Service;
							}
						}

						Status |= AutomationStatus.IsClient;
					}
					catch (Exception e)
					{
						ideaLogger.LogError("Can not attach to BIM application", e);
						throw;
					}
				}
				else
				{
					ideaLogger.LogDebug($"RunServer - Initializing wihtout the process id");

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

				ServiceBaseAddress = string.Format(AutomationUrlFormat, myProcessId);
				ideaLogger.LogDebug($"RunServer - Starting Automation service listening on '{ServiceBaseAddress}'");

				// expose my IAutomation interface
				using (ServiceHost selfServiceHost = new ServiceHost(automation, new Uri(ServiceBaseAddress)))
				{
					((ServiceBehaviorAttribute)selfServiceHost.Description.
					Behaviors[typeof(ServiceBehaviorAttribute)]).InstanceContextMode
					= InstanceContextMode.Single;

					//Net named pipe
					NetNamedPipeBinding binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647 };
					binding.ReceiveTimeout = TimeSpan.MaxValue;
					selfServiceHost.AddServiceEndpoint(typeof(MyInterface), binding, ServiceBaseAddress);

					//MEX - Meta data exchange
					ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
					selfServiceHost.Description.Behaviors.Add(behavior);
					selfServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), ServiceBaseAddress + "/mex");

					selfServiceHost.Faulted += SelfServiceHost_Faulted;
					selfServiceHost.Opened += SelfServiceHost_Opened;
					selfServiceHost.Opening += SelfServiceHost_Opening;
					selfServiceHost.UnknownMessageReceived += SelfServiceHost_UnknownMessageReceived;

					selfServiceHost.Open(OpenServerTimeLimit);

					if (!string.IsNullOrEmpty(id))
					{
						// notify plugin that service is running
						string myEventName = string.Format("{0}{1}", EventName, id);
						ideaLogger.LogDebug($"RunServer - Successful start confirmation event name is {myEventName}.");

						// connect to the existing named event
						EventWaitHandle syncEvent;
						if (!EventWaitHandle.TryOpenExisting(myEventName, out syncEvent))
						{
							// if failed, throw an exception
							throw new Exception("Failed to connect to the successful start confirmation event.");
						}

						ideaLogger.LogDebug($"RunServer - Setting event {myEventName}.");
						syncEvent.Set();
						syncEvent.Dispose();
					}

					foreach (var endpoint in selfServiceHost.Description.Endpoints)
					{
						ideaLogger.LogTrace(string.Format("{0} ({1})", endpoint.Address.ToString(), endpoint.Binding.Name));
					}

					NotifyBIMStatusChanged(AppStatus.Started);

					ideaLogger.LogDebug("Waiting for the server stop request...");
					while (!cancellationToken.IsCancellationRequested)
					{
						Thread.Sleep(100);
					}

					ideaLogger.LogDebug($"Automation Service stop is requested.");

					try
					{
						if (bimClient != null)
						{
							// if the client is not already closed or in faulted state
							if (bimClient.State != CommunicationState.Closed && bimClient.State != CommunicationState.Faulted)
							{
								ideaLogger.LogTrace($"Closing client...");
								bimClient.Close();
							}
							else
							{
								ideaLogger.LogTrace($"Skipping client closing due to {bimClient.State} state.");
							}
							bimClient = null;
						}
					}
					catch (Exception ex)
					{
						ideaLogger.LogWarning($"Closing BIM client for processId = {id} failed", ex);
					}

					try
					{
						if (selfServiceHost != null)
						{
							ideaLogger.LogDebug("Connection with BIM application has been closed");

							selfServiceHost.Faulted -= SelfServiceHost_Faulted;
							selfServiceHost.Opened -= SelfServiceHost_Opened;
							selfServiceHost.Opening -= SelfServiceHost_Opening;
							selfServiceHost.UnknownMessageReceived -= SelfServiceHost_UnknownMessageReceived;

							selfServiceHost.Close();
						}
					}
					catch (Exception ex)
					{
						ideaLogger.LogWarning("Closing service host for processId = {id} failed", ex);
					}

					NotifyBIMStatusChanged(AppStatus.Finished);
				}
			}
			finally
			{
				// set the server stopped event at before the exit to indicate the server is not running anymore
				ideaLogger.LogDebug("Setting the server stopped event.");
				serverStoppedEvent.Set();

				ideaLogger.LogInformation("Server stopped.");
			}
		}

		private void SelfServiceHost_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
		{
			ideaLogger.LogWarning($"UnknownMessageReceived service '{ServiceBaseAddress}', message details = '{(e?.ToString())}'.");
		}

		private void SelfServiceHost_Opening(object sender, EventArgs e)
		{
			ideaLogger.LogDebug($"Opening service '{ServiceBaseAddress}'");
		}

		private void SelfServiceHost_Opened(object sender, EventArgs e)
		{
			ideaLogger.LogDebug($"Opened service '{ServiceBaseAddress}'");
		}

		private void SelfServiceHost_Faulted(object sender, EventArgs e)
		{
			ideaLogger.LogError($"Faulted service '{ServiceBaseAddress}', fault details = '{e?.ToString()}'.");
		}

		protected virtual void NotifyBIMStatusChanged(AppStatus newStatus)
		{
			BIMStatusChanged?.Invoke(this, new ISEventArgs() { Status = newStatus });
		}

		private void BimProcess_Exited(object sender, EventArgs e)
		{
			ideaLogger.LogDebug($"Connected BIM process with id {myAutomatingProcessId} has exited");

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
		public string ServiceBaseAddress { get => baseAddress; set => baseAddress = value; }

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				ideaLogger.LogDebug("Disposing server...");

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
							ideaLogger.LogWarning("Stopping of the server failed", ex);
						}

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
						catch (Exception ex)
						{
							ideaLogger.LogWarning("Disposing of the service failed", ex);
						}

						if (bimProcess != null)
						{
							bimProcess.Dispose();
							bimProcess = null;
						}

						serverStoppedEvent.Dispose();
						serverStopRequestTokenSource.Dispose();
					}
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				ideaLogger.LogDebug("Server disposed.");

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
#endif
