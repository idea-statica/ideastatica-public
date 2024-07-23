using IdeaStatiCa.Plugin.Grpc;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	public interface IMemberCalculatorFactory
	{
		Task<IMemberHiddenCheck> GetAsync();
	}

	public class MemberHiddenClientFactory : IMemberCalculatorFactory
	{
		private readonly string IdeaInstallDir;
		private Process CalculatorProcess { get; set; }

		private int GrpcPort { get; set; }

		private readonly IPluginLogger pluginLogger;

		private readonly int myProcessId;

		private AutomationHostingGrpc<IMemberHiddenCheck, IMemberHiddenCheck> memberCalculatorClientHosting;

#if DEBUG
		private int StartTimeout = -1;
#else
		int StartTimeout = 1000*20;
#endif

		public MemberHiddenClientFactory(string ideaInstallDir, IPluginLogger pluginLogger)
		{
			this.pluginLogger = pluginLogger;
			myProcessId = Process.GetCurrentProcess().Id;
			IdeaInstallDir = ideaInstallDir;
		}

		public async Task<IMemberHiddenCheck> GetAsync()
		{
			RunCalculatorProcess();

			var grpcClient = new GrpcClient(pluginLogger);
			await grpcClient.StartAsync(myProcessId.ToString(), GrpcPort);

			memberCalculatorClientHosting = new AutomationHostingGrpc<IMemberHiddenCheck, IMemberHiddenCheck>(null, grpcClient);
			_ = Task.Run(
				async () =>
				{
					await memberCalculatorClientHosting.RunAsync(myProcessId.ToString());
				});

			return memberCalculatorClientHosting.MyBIM;
		}

		private void RunCalculatorProcess()
		{
			if (CalculatorProcess != null)
			{
				return;
			}

			string eventName = string.Format(Constants.MemHiddenCalcChangedEventFormat, myProcessId);
			using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
			{
				string connChangedEventName = string.Format(Constants.MemHiddenCalcChangedEventFormat, myProcessId);
				string applicationExePath = Path.Combine(IdeaInstallDir, "IdeaStatiCa.MemberHiddenCalculator.exe");

				string cmdParams = $"-automation{myProcessId} {Plugin.Constants.GrpcControlPortParam}:{GrpcPort}";
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

			CalculatorProcess.Exited += CalculatorProcess_Exited;
		}

		private void CalculatorProcess_Exited(object sender, EventArgs e)
		{
			if (CalculatorProcess == null)
			{
				return;
			}

			CalculatorProcess.Dispose();
			CalculatorProcess = null;

			memberCalculatorClientHosting.Dispose();
			memberCalculatorClientHosting = null;
		}
	}
}
