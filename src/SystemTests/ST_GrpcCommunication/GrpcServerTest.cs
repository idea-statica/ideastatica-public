using IdeaStatiCa.Plugin.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Xunit;
using FluentAssertions;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using System.Threading.Tasks;
using SystemTestService;

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
		public async Task InvokeMethodTest()
		{
			int grpcServerPort = PortFinder.FindPort(50000, 50500);

			Process grpcServerProc = new Process();
			string eventName = string.Format("GrpsServer_Start{0}", grpcServerPort);
			string applicationExePath = Path.Combine("GrpcServerHost.exe");

			try
			{
				using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
				{
					grpcServerProc.StartInfo = new ProcessStartInfo(applicationExePath, $"{IdeaStatiCa.Plugin.Constants.GrpcPortParam}:{grpcServerPort} -startEvent:{eventName}");
					grpcServerProc.EnableRaisingEvents = true;
					grpcServerProc.Start();

					if (!syncEvent.WaitOne(StartTimeout))
					{
						throw new TimeoutException($"Time out - process '{applicationExePath}' doesn't set the event '{eventName}'");
					}

					string clientId = grpcServerPort.ToString();

					// create claint of the service IService which runs on grpcServer
					GrpcServiceBasedReflectionClient<IService> grpcClient = new GrpcServiceBasedReflectionClient<IService>(clientId, grpcServerPort);

					await grpcClient.ConnectAsync();
					grpcClient.IsConnected.Should().BeTrue("The client shoul be connected");

					// get interface of the service
					IService serviceClient = grpcClient.Service;

					// invote method remotly
					string fooResult = serviceClient.Foo("IDEA StatiCa");

					fooResult.Should().BeEquivalentTo("Hi IDEA StatiCa");

					await grpcClient.DisconnectAsync();
				}
			}
			finally
			{
				if(grpcServerProc != null)
				{
					try
					{
						grpcServerProc.Kill();
						grpcServerProc.WaitForExit();
						grpcServerProc = null;
					}
					catch { }
				}
			}
		}
	}
}
