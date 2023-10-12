#if NET48

using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;

namespace IdeaStatiCa.Plugin
{
	public interface IConnCalculatorFactory
	{
		ConnectionHiddenCheckClient Create();
	}

	public class ConnHiddenClientFactory : IDisposable, IConnCalculatorFactory
	{
		private readonly IPluginLogger pluginLogger;
		private readonly string IdeaInstallDir;
		private Process CalculatorProcess { get; set; }
		private Uri CalculatorUrl { get; set; }

#if DEBUG
		private int StartTimeout = -1;
#else
		int StartTimeout = 1000*40;
#endif

		public ConnHiddenClientFactory(string ideaInstallDir)
		{
			this.pluginLogger = new IdeaStatiCa.Plugin.NullLogger();
			IdeaInstallDir = ideaInstallDir;
			pluginLogger.LogInformation($"Constructor ConnHiddenClientFactory ideaInstallDir :'{ideaInstallDir}'");
		}

		public ConnHiddenClientFactory(string ideaInstallDir, IPluginLogger logger)
		{
			this.pluginLogger = logger;
			IdeaInstallDir = ideaInstallDir;
			pluginLogger.LogInformation($"Constructor ConnHiddenClientFactory ideaInstallDir :'{ideaInstallDir}'");
		}

		public ConnectionHiddenCheckClient Create()
		{
			pluginLogger.LogInformation("ConnHiddenClientFactory.Create");
			RunCalculatorProcess();

			NetNamedPipeBinding pluginBinding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };

			ConnectionHiddenCheckClient calculatorClient = new ConnectionHiddenCheckClient(pluginBinding, new EndpointAddress(CalculatorUrl), pluginLogger);
			calculatorClient.Open();

			return calculatorClient;
		}

		private void RunCalculatorProcess()
		{
			if (CalculatorProcess != null)
			{
				pluginLogger.LogDebug($"ConnHiddenClientFactory.RunCalculatorProcess : CalculatorProcess is running processId = {CalculatorProcess.Id} ");
				return;
			}

			int myProcessId = Process.GetCurrentProcess().Id;

			string eventName = string.Format(Constants.ConCalculatorChangedEventFormat, myProcessId);
			using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
			{
				string connChangedEventName = string.Format(Constants.ConCalculatorChangedEventFormat, myProcessId);
				string applicationExePath = Path.Combine(IdeaInstallDir, "IdeaStatiCa.ConnHiddenCalculator.exe");

				if(!File.Exists(applicationExePath))
				{
					pluginLogger.LogWarning($"ConnHiddenClientFactory.RunCalculatorProcess 'applicationExePath' doesn't exists");
					throw new ArgumentException($"RunCalculatorProcess - file '{applicationExePath}' doesn't exists");
				}

				string cmdParams = $"-automation{myProcessId}";
				ProcessStartInfo psi = new ProcessStartInfo(applicationExePath, cmdParams);
				psi.WindowStyle = ProcessWindowStyle.Normal;
				psi.UseShellExecute = false;

#if DEBUG
				psi.CreateNoWindow = false;
#else
				psi.CreateNoWindow = true;
#endif
				Stopwatch stopWatch = new Stopwatch();
				stopWatch.Start();

				pluginLogger.LogInformation($"ConnHiddenClientFactory.RunCalculatorProcess starting '{applicationExePath} {cmdParams}'");
				CalculatorProcess = new Process();
				CalculatorProcess.StartInfo = psi;
				CalculatorProcess.EnableRaisingEvents = true;
				CalculatorProcess.Start();

				pluginLogger.LogInformation($"ConnHiddenClientFactory.RunCalculatorProcess waiting for event '{eventName}'");
				if (!syncEvent.WaitOne(StartTimeout))
				{
					stopWatch.Stop();
					var errMsg = $"Timeout the process '{applicationExePath}' doesn't set the event '{eventName}' in {stopWatch.Elapsed.Seconds} s (limit is {StartTimeout} s)";
					pluginLogger.LogWarning(errMsg);
					throw new TimeoutException(errMsg);
				}
				else
				{
					stopWatch.Stop();
					pluginLogger.LogInformation($"ConnHiddenClientFactory.RunCalculatorProcess : processId {CalculatorProcess.Id} running, Starttime : {stopWatch.Elapsed.Seconds} s");
				}

			}

			

			ConnectionHiddenCheckClient.HiddenCalculatorId = CalculatorProcess.Id;
			CalculatorUrl = new Uri(string.Format(Constants.ConnHiddenCalculatorUrlFormat, CalculatorProcess.Id));
			CalculatorProcess.Exited += CalculatorProcess_Exited;
		}

		private void CalculatorProcess_Exited(object sender, EventArgs e)
		{
			if (CalculatorProcess == null)
			{
				pluginLogger.LogInformation($"ConnHiddenClientFactory.CalculatorProcess_Exited but CalculatorProcess is null");
				return;
			}

			ConnectionHiddenCheckClient.HiddenCalculatorId = -1;
			CalculatorProcess.Dispose();
			CalculatorProcess = null;
			CalculatorUrl = null;
		}

#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					try
					{
						if (CalculatorProcess != null)
						{
							CalculatorProcess.EnableRaisingEvents = false;
							CalculatorProcess.Kill();
							CalculatorProcess = null;
							CalculatorUrl = null;
						}
					}
					catch(Exception e)
					{
					pluginLogger.LogWarning("ConnHiddenClientFactory.Dispose failed", e);
					}
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ConnHiddenClientFactory()
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

#endregion IDisposable Support
	}
}
#endif