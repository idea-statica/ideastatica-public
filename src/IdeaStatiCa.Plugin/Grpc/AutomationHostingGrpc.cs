using IdeaStatica.Communication;
using IdeaStatiCa.Plugin.Grpc;
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
	public class AutomationHostingGrpc<MyInterface, ClientInterface> : IGrpcCommunicationCommonInterface, IBIMPluginClient<ClientInterface>, IDisposable where MyInterface : class where ClientInterface : class
	{
		private Task hostingTask;
		private CancellationTokenSource tokenSource;
		private ManualResetEvent mre;
		private MyInterface automation;
		private Process bimProcess = null;
		private int myAutomatingProcessId;
		protected string EventName { get; set; }
		private readonly IPluginLogger ideaLogger = null;
		public IGrpcClient GrpcClient { get; private set; }

		/// <summary>
		/// My BIM object.
		/// </summary>
		public ClientInterface MyBIM {get; set;}

		/// <summary>
		/// Current automation status.
		/// </summary>
		public AutomationStatus Status { get; private set; }

		/// <summary>
		/// Determines whether BIM application is connected.
		/// </summary>
		public bool IsConnected { get { return MyBIM != null; } }

		/// <summary>
		/// Triggered when BIM status changes.
		/// </summary>
		public event ISEventHandler BIMStatusChanged;

		public AutomationHostingGrpc(MyInterface hostedService,
			IGrpcClient grpcClient,
			IPluginLogger logger = null,
			string eventName = Constants.DefaultPluginEventName)
		{
			ideaLogger = logger ?? new NullLogger();
			Status = AutomationStatus.Unknown;
			automation = hostedService;
			GrpcClient = grpcClient;
			EventName = eventName;
			mre = new ManualResetEvent(false);
			ideaLogger.LogDebug($"AutomationHostingGrpc EventName = '{EventName}'");
		}

		/// <inheritdoc cref="RunAsync(string)"/>
		public Task RunAsync(string id)
		{
			if (hostingTask != null)
			{
				throw new Exception("AutomationHostingGrpc.RunAsync - task is already");
			}

			ideaLogger.LogDebug($"AutomationHostingGrpc.RunAsync id = '{id}");

			tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			if(!string.IsNullOrEmpty(id))
			{
				// bim link
				var grpcReflectionHandler = new GrpcMethodInvokerHandler(IdeaStatiCa.Plugin.Constants.GRPC_REFLECTION_HANDLER_MESSAGE, GrpcClient, ideaLogger);
				MyBIM = GrpcReflectionServiceFactory.CreateInstance<ClientInterface>(grpcReflectionHandler);
				GrpcClient.RegisterHandler(IdeaStatiCa.Plugin.Constants.GRPC_REFLECTION_HANDLER_MESSAGE, grpcReflectionHandler);
			}

			if (automation != null)
			{
				// register handler which serves MyInterface requests
				GrpcClient.RegisterHandler(Constants.GRPC_CHECKBOT_HANDLER_MESSAGE, new GrpcReflectionMessageHandler(automation, ideaLogger));
			}

			mre.Reset();

			bool isBimRunning = false;

			if (!string.IsNullOrEmpty(id))
			{
				myAutomatingProcessId = int.Parse(id);
				ideaLogger.LogInformation($"AutomationHostingGrpc.RunServer - processId == '{myAutomatingProcessId}'");

				bimProcess = Process.GetProcessById(myAutomatingProcessId);
				bimProcess.EnableRaisingEvents = true;
				bimProcess.Exited += new EventHandler(BimProcess_Exited);

				if (!string.IsNullOrEmpty(id))
				{
					string eventName = string.Format("{0}{1}", EventName, id);

					// notify plugin that service is running
					EventWaitHandle syncEvent;
					ideaLogger.LogDebug($"AutomationHostingGrpc.RunServer - tryprocessId == '{myAutomatingProcessId}'");
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
				ideaLogger.LogDebug($"AutomationHostingGrpc.RunServer - processId == '{myAutomatingProcessId}' is not running");
				bimProcess = null;
				myAutomatingProcessId = -1;
			}

			NotifyBIMStatusChanged(AppStatus.Started);

			ideaLogger.LogDebug($"AutomationHostingGrpc.RunServer - starting hosting task");
			hostingTask = Task.Run(() =>
			{
				try
				{
					RunServer(id, token);
				}
				catch (Exception e)
				{
					ideaLogger.LogError("AutomationHostingGrpc.RunAsync  RunServer failed", e);
					throw;
				}
			}, token);

			return hostingTask;
		}

		public async void Stop()
		{
			ideaLogger.LogDebug("AutomationHostingGrpc.Stop");
			if (hostingTask != null)
			{
				tokenSource.Cancel();
				var stopRes = mre.WaitOne();
				MyBIM = null;

				try
				{
					ideaLogger.LogDebug("AutomationHostingGrpc.Stop - disconnecting GrpcCommunicator");
					await GrpcClient.StopAsync();
				}
				catch
				{
				}
			}
		}

		protected virtual void RunServer(string id, System.Threading.CancellationToken cancellationToken)
		{
			ideaLogger.LogDebug($"AutomationHostingGrpc.RunServer id = {id}");

			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					Thread.Sleep(100);
				}

				ideaLogger.LogInformation($"AutomationHostingGrpc.RunServer - Automation Service has been stopped");
			}
			catch(Exception ex)
			{
				ideaLogger.LogWarning("RunServer failed", ex);
			}
			finally
			{
				GrpcClient?.StopAsync().WaitAndUnwrapException();

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
			ideaLogger.LogInformation("AutomationHostingGrpc.BimProcess_Exited");
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
			ideaLogger.LogInformation($"AutomationHostingGrpc.Dispose disposing = {disposing}");
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
