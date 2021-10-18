using FluentAssertions;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.ProjectContent;
using IdeaStatiCa.Plugin.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SystemTestService;
using Xunit;

namespace ST_GrpcCommunication
{
	public class GrpcServerTest
	{
#if DEBUG
		public const int StartTimeout = -1;
#else
		public const int StartTimeout = 1000*20;
#endif

		/// <summary>
		/// System test for invoking remote methods on GrpcServer
		/// </summary>
		/// <returns></returns>
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
				if (grpcServerProc != null)
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


		/// <summary>
		/// System test of the implementation <see cref="ProjectContentClientHandler"/> which validates grpc communication with the GrpcServer
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task ProjectContentTest()
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

					var projectContentHandler = new ProjectContentClientHandler(grpcClient.GrpcSyncClient);

					// add project content handler
					grpcClient.RegisterHandler(IdeaStatiCa.Plugin.Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE, projectContentHandler);

					await grpcClient.ConnectAsync();
					grpcClient.IsConnected.Should().BeTrue("The client shoul be connected");

					projectContentHandler = new ProjectContentClientHandler(grpcClient.GrpcSyncClient);

					const string item1Id = "Item1";
					var isItem1 = projectContentHandler.Exist(item1Id);
					isItem1.Should().BeFalse("Item1 should not exist");

					// it should return stream but it still should not exist on the server
					using (var item1Stream = projectContentHandler.Get(item1Id))
					{
						item1Stream.Should().NotBeNull();

						isItem1 = projectContentHandler.Exist(item1Id);
						isItem1.Should().BeFalse("Item1 should not still  exist");

						// write there two bytes
						item1Stream.WriteByte(1);
						item1Stream.WriteByte(2);

						// disposing stream should send data to server
					}

					isItem1 = projectContentHandler.Exist(item1Id);
					isItem1.Should().BeTrue("Item1 should  exist");


					using (var item1Stream = projectContentHandler.Get(item1Id))
					{
						item1Stream.Should().NotBeNull();
						item1Stream.Length.Should().Be(2);

						item1Stream.WriteByte(3);
					}


					const string item2Id = "Item2";
					var isItem2 = projectContentHandler.Exist(item2Id);
					isItem2.Should().BeFalse("Item2 should not exist");


					// it should return stream but it still should not exist on the server
					using (var item2Stream = projectContentHandler.Get(item2Id))
					{
						item2Stream.Should().NotBeNull();

						// write there two bytes
						item2Stream.WriteByte(1);

						// disposing stream should send data to server
					}

					isItem2 = projectContentHandler.Exist(item2Id);
					isItem2.Should().BeTrue("Item2 should  exist");

					using (var item1Stream = (MemoryStream)projectContentHandler.Get(item1Id))
					{
						item1Stream.Should().NotBeNull();
						var streamLength = item1Stream.Length;
						streamLength.Should().Be(3);

						var buffer = item1Stream.GetBuffer();

						for (byte i = 1; i <= streamLength; i++)
						{
							buffer[i - 1].Should().Be(i);
						}
					}

					await grpcClient.DisconnectAsync();
				}
			}
			finally
			{
				if (grpcServerProc != null)
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
