using IdeaStatiCa.Plugin.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Xunit;
using FluentAssertions;

namespace ST_GrpcCommunication
{
	public class GrpcServerTest
	{
#if DEBUG
		public const int StartTimeout = -1;
#else
		public const int StartTimeout = 1000*20;
#endif

		[Fact]
		public void InvokeMethodTest()
		{
			int grpcServerPort = PortFinder.FindPort(50000, 50500);

			Process grpcServerProc = new Process();
			string eventName = string.Format("GrpsServer_Start{0}", grpcServerPort);
			string applicationExePath = Path.Combine("GrpcServerHost.exe");
			//string applicationExePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "GrpcServerHost.exe");
			//bool isExeFile = File.Exists(applicationExePath);
			//isExeFile.Should().BeTrue($"Missing file '{applicationExePath}'");

			using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
			{
				grpcServerProc.StartInfo = new ProcessStartInfo(applicationExePath, $"{IdeaStatiCa.Plugin.Constants.GrpcPortParam}:{grpcServerPort} -startEvent:{eventName}" );
				grpcServerProc.EnableRaisingEvents = true;
				grpcServerProc.Start();

				if (!syncEvent.WaitOne(StartTimeout))
				{
					throw new TimeoutException($"Time out - process '{applicationExePath}' doesn't set the event '{eventName}'");
				}

				grpcServerProc.Kill();

				grpcServerProc.WaitForExit();
			}
		}
	}
}
