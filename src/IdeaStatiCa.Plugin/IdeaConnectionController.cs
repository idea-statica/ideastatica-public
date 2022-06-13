#if NET48

using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;

namespace IdeaStatiCa.Plugin
{
	public class IdeaConnectionController : IDisposable, IConnectionController
	{
		private readonly string IdeaInstallDir;
		private Process IdeaStatiCaProcess { get; set; }
		private Uri CalculatorUrl { get; set; }

		protected EventWaitHandle CurrentItemChangedEvent;

		public event EventHandler ConnectionAppExited;

		protected IdeaStatiCaClient<IAutomation> ConnectionAppClient { get; set; }
		protected virtual uint UserMode { get; } = 0;

		private string BaseAddress { get; set; }

		public bool IsConnected => ConnectionAppClient?.Service != null;

#if DEBUG
		private int StartTimeout = -1;
#else
		int StartTimeout = 1000*20;
#endif

		public int OpenProject(string fileName)
		{
			ConnectionAppClient.Service.OpenProject(fileName);
			return 0;
		}

		public int CloseProject()
		{
			ConnectionAppClient.Service.CloseProject();
			return 0;
		}


		private IdeaConnectionController(string ideaInstallDir)
		{
			if(!Directory.Exists(ideaInstallDir))
			{
				throw new ArgumentException($"IdeaConnectionController.IdeaConnectionController - directory '{ideaInstallDir}' doesn't exist");
			}

			IdeaInstallDir = ideaInstallDir;
		}

		public static IConnectionController Create(string ideaInstallDir)
		{
			IdeaConnectionController connectionController = new IdeaConnectionController(ideaInstallDir);
			connectionController.OpenConnectionClient();
			return connectionController;
		}

		private void OpenConnectionClient()
		{
			int processId = Process.GetCurrentProcess().Id;
			string connChangedEventName = string.Format(Constants.ConnectionChangedEventFormat, processId);
			CurrentItemChangedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, connChangedEventName);

			string applicationExePath = Path.Combine(IdeaInstallDir, "ideaconnection.exe");

			if(!File.Exists(applicationExePath))
			{
				throw new ArgumentException($"IdeaConnectionController.OpenConnectionClient - file '{applicationExePath}' doesn't exist");
			}

			Process connectionProc = new Process();
			string eventName = string.Format("IdeaStatiCaEvent{0}", processId);
			using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
			{
				connectionProc.StartInfo = new ProcessStartInfo(applicationExePath, $"-cmd:automation-{processId}  user-mode 192");
				connectionProc.EnableRaisingEvents = true;
				connectionProc.Start();

				if(!syncEvent.WaitOne(StartTimeout))
				{
					throw new TimeoutException($"Time out - process '{applicationExePath}' doesn't set the event '{eventName}'");
				}
			}

			IdeaStatiCaProcess = connectionProc;

			BaseAddress = $"net.pipe://localhost/ConnectioService{connectionProc.Id}";

			NetNamedPipeBinding binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };
			ConnectionAppClient = new IdeaStatiCaClient<IAutomation>(binding, new EndpointAddress(BaseAddress));
			ConnectionAppClient.Open();

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
			CalculatorUrl = null;
			ConnectionAppClient = null;

			if(ConnectionAppExited != null)
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
#endif
