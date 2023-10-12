#if NET48

using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;

namespace IdeaStatiCa.Plugin
{
	public interface IMemberCalculatorFactory
	{
		MemberHiddenCheckClient Create();
	}

	public class MemberHiddenClientFactory : IMemberCalculatorFactory
	{
		private readonly string IdeaInstallDir;
		private Process CalculatorProcess { get; set; }
		private Uri CalculatorUrl { get; set; }

#if DEBUG
		private int StartTimeout = -1;
#else
		int StartTimeout = 1000*20;
#endif

		public MemberHiddenClientFactory(string ideaInstallDir)
		{
			IdeaInstallDir = ideaInstallDir;
		}

		public MemberHiddenCheckClient Create()
		{
			RunCalculatorProcess();

			NetNamedPipeBinding pluginBinding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };

			MemberHiddenCheckClient calculatorClient = new MemberHiddenCheckClient(pluginBinding, new EndpointAddress(CalculatorUrl));
			calculatorClient.Open();

			return calculatorClient;
		}

		private void RunCalculatorProcess()
		{
			if (CalculatorProcess != null)
			{
				return;
			}

			int myProcessId = Process.GetCurrentProcess().Id;

			string eventName = string.Format(Constants.MemHiddenCalcChangedEventFormat, myProcessId);
			using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
			{
				string connChangedEventName = string.Format(Constants.MemHiddenCalcChangedEventFormat, myProcessId);
				string applicationExePath = Path.Combine(IdeaInstallDir, "IdeaStatiCa.MemberHiddenCalculator.exe");

				string cmdParams = $"-automation{myProcessId}";
				ProcessStartInfo psi = new ProcessStartInfo(applicationExePath, cmdParams);
				psi.WindowStyle = ProcessWindowStyle.Normal;
				psi.UseShellExecute = false;

#if DEBUG
				psi.CreateNoWindow = false;
#else
				psi.CreateNoWindow = true;
#endif

				CalculatorProcess = new Process();
				CalculatorProcess.StartInfo = psi;
				CalculatorProcess.EnableRaisingEvents = true;
				CalculatorProcess.Start();

				if (!syncEvent.WaitOne(StartTimeout))
				{
					throw new TimeoutException();
				}
			}

			MemberHiddenCheckClient.HiddenCalculatorId = CalculatorProcess.Id;
			CalculatorUrl = new Uri(string.Format(Constants.MemberHiddenCalculatorUrlFormat, CalculatorProcess.Id));
			CalculatorProcess.Exited += CalculatorProcess_Exited;
		}

		private void CalculatorProcess_Exited(object sender, EventArgs e)
		{
			if (CalculatorProcess == null)
			{
				return;
			}

			MemberHiddenCheckClient.HiddenCalculatorId = -1;
			CalculatorProcess.Dispose();
			CalculatorProcess = null;
			CalculatorUrl = null;
		}
	}
}

#endif
