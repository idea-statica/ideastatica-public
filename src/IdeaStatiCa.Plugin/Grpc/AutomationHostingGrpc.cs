using IdeaStatiCa.Plugin.Grpc.Reflection;
using Nito.AsyncEx.Synchronous;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Grpc Hosting implementation of <see cref="AutomationHosting{MyInterface, ClientInterface}"/>
	/// </summary>
	public class AutomationHostingGrpc<MyInterface, ClientInterface> : IBIMPluginClient<ClientInterface>, IDisposable where MyInterface : class where ClientInterface : class
	{
		private Task hostingTask;
		private CancellationTokenSource tokenSource;
		private ManualResetEvent mre;
		private MyInterface automation;
		private Process bimProcess = null;
		private int myAutomatingProcessId;
		protected string EventName { get; set; }
		private GrpcServiceBasedReflectionClient<ClientInterface> grpcClient;
		private readonly IPluginLogger ideaLogger = null;

		/// <summary>
		/// My BIM object.
		/// </summary>
		public ClientInterface MyBIM
		{
			get
			{
				if (grpcClient == null || !grpcClient.IsConnected)
				{
					return null;
				}

				return grpcClient.Service;
			}
		}

		/// <summary>
		/// Port on which the Grpc server is running.
		/// </summary>
		public int GrpcPort { get; private set; }

		/// <summary>
		/// Current automation status.
		/// </summary>
		public AutomationStatus Status { get; private set; }

		/// <summary>
		/// Determines whether automation hosting is connected to Grpc server.
		/// </summary>
		public bool IsConnected { get { return grpcClient?.IsConnected == true; } }

		/// <summary>
		/// Triggered when BIM status changes.
		/// </summary>
		public event ISEventHandler BIMStatusChanged;

		public AutomationHostingGrpc(MyInterface hostedService,
				IPluginLogger logger = null,
				string eventName = Constants.DefaultPluginEventName)
		{
			ideaLogger = logger ?? new NullLogger();
			Status = AutomationStatus.Unknown;
			automation = hostedService;
			EventName = eventName;
			mre = new ManualResetEvent(false);
		}

		/// <inheritdoc cref="RunAsync(string, string)"/>
		public Task RunAsync(string id, string gRpcPort)
		{
			if (hostingTask != null)
			{
				Debug.Fail("Task is running");
				return Task.CompletedTask;
			}

			Debug.Assert(!string.IsNullOrEmpty(gRpcPort));

			GrpcPort = int.Parse(gRpcPort);

			ideaLogger.LogDebug($"AutomationHostingGrpc.RunAsync id = '{id}");

			tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			// initialize grpc client
			grpcClient = new GrpcServiceBasedReflectionClient<ClientInterface>(id, GrpcPort, ideaLogger);

			if (automation != null)
			{
				// register handler which serves MyInterface requests
				grpcClient.RegisterHandler(Constants.GRPC_CHECKBOT_HANDLER_MESSAGE, new GrpcReflectionMessageHandler(automation));
			}

			grpcClient.ConnectAsync().WaitAndUnwrapException();

			hostingTask = Task.Run(() =>
			{
				try
				{
					RunServer(id, token);
				}
				catch (Exception e)
				{
					ideaLogger.LogError("RunAsync  RunServer failed", e);
					throw;
				}
			}, token);

			return hostingTask;
		}

		public async void Stop()
		{
			if (hostingTask != null)
			{
				tokenSource.Cancel();
				var stopRes = mre.WaitOne();

				try
				{
					await grpcClient.DisconnectAsync();
				}
				catch
				{
				}
			}
		}

		protected virtual void RunServer(string id, System.Threading.CancellationToken cancellationToken)
		{
			ideaLogger.LogInformation("Calling RunServer");

			try
			{
				mre.Reset();

				bool isBimRunning = false;

				if (!string.IsNullOrEmpty(id))
				{
					myAutomatingProcessId = int.Parse(id);
					ideaLogger.LogInformation($"RunServer - processId == '{myAutomatingProcessId}'");

					bimProcess = Process.GetProcessById(myAutomatingProcessId);
					bimProcess.EnableRaisingEvents = true;
					bimProcess.Exited += new EventHandler(BimProcess_Exited);

					if (!string.IsNullOrEmpty(id))
					{
						string eventName = string.Format("{0}{1}", EventName, id);

						// notify plugin that service is running
						EventWaitHandle syncEvent;
						ideaLogger.LogDebug($"RunServer - tryprocessId == '{myAutomatingProcessId}'");
						if (EventWaitHandle.TryOpenExisting(eventName, out syncEvent))
						{
							syncEvent.Set();
							syncEvent.Dispose();
						}
					}

					Status |= AutomationStatus.IsClient;
					isBimRunning = true;
				}

				if (!isBimRunning)
				{
					ideaLogger.LogInformation($"AutomationHostingGrpc.RunServer - processId == '{myAutomatingProcessId}' is not running");
					bimProcess = null;
					myAutomatingProcessId = -1;
				}

				NotifyBIMStatusChanged(AppStatus.Started);

				while (!cancellationToken.IsCancellationRequested)
				{
					Thread.Sleep(100);
				}

				ideaLogger.LogInformation($"RunServer - Automation Service has been stopped");
			}
			catch(Exception ex)
			{
				ideaLogger.LogWarning("RunServer failed", ex);
			}
			finally
			{
				grpcClient?.DisconnectAsync().WaitAndUnwrapException();

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

		#region IDisposable

		private bool disposedValue = false;

		public void Dispose()
		{
			Dispose(true);
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
							Stop();
						}
						catch (Exception ex)
						{
							ideaLogger.LogDebug("Stopping task AutomationHostingGrpc failed", ex);
						}

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
		#endregion
	}
}
