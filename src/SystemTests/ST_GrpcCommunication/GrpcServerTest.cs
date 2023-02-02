using FluentAssertions;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.ProjectContent;
using IdeaStatiCa.Plugin.Utilities;
using IdeaStatiCa.PluginLogger;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemTestService;

namespace ST_GrpcCommunication
{
	[TestFixture]
	public class GrpcServerTest
	{
#if DEBUG
		public const int StartTimeout = -1;
#else
		public const int StartTimeout = 1000*20;
#endif

		private int grpcServerPort;
		private IGrpcServer grpcServer;
		private readonly IPluginLogger nullLogger = new NullLogger();

		[OneTimeSetUp]
		public void StartGrpcServerTask()
		{
			IPluginLogger grpcServerLogger = LoggerProvider.GetLogger("grpcServerLogger");

			grpcServerPort = PortFinder.FindPort(50000, 50500);
			var fooService = new Service();
			var serverStartedEvent = new AutoResetEvent(false);

			Task.Run(() =>
			{
				// grpc server
				grpcServer = new GrpcReflectionServer(fooService, grpcServerLogger);
				
				// project content handler
				var projectContentInMemory = new ProjectContentInMemory();
				var contentHandler = new ProjectContentServerHandler(projectContentInMemory);
				grpcServer.GrpcService.RegisterHandler(IdeaStatiCa.Plugin.Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE, contentHandler);

				// grpc server start
				grpcServer.StartAsync(null, grpcServerPort);

				grpcServerLogger.LogDebug($"GrpcServer is listening on port '{grpcServerPort}'");

				serverStartedEvent.Set();
			});

			serverStartedEvent.WaitOne();
		}

		[OneTimeTearDown]
		public void StopGrpcServerTask()
		{
			grpcServer.StopAsync();
		}
		
		/// <summary>
		/// System test for invoking remote methods on GrpcServer
		/// </summary>
		/// <returns></returns>
		[Test]
		public async Task InvokeMethodTest()
		{
			string clientId = grpcServerPort.ToString();

			// create client of the service IService which runs on grpcServer
			GrpcServiceBasedReflectionClient<IService> grpcClient = new GrpcServiceBasedReflectionClient<IService>(nullLogger);

			var task = grpcClient.StartAsync(clientId, grpcServerPort);

			grpcClient.IsConnected.Should().BeTrue("The client should be connected");

			// get interface of the service
			IService serviceClient = grpcClient.Service;

			// invote method remotly
			string fooResult = serviceClient.Foo("IDEA StatiCa");

			fooResult.Should().BeEquivalentTo("Hi IDEA StatiCa");

			// try to send too string which exceeds Constants.GRPC_MAX_MSG_SIZE
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Constants.GRPC_MAX_MSG_SIZE; i++)
			{
				sb.Append("MP");
			}

			// invote method remotly
			string fooResult2 = serviceClient.Foo(sb.ToString());
			fooResult2.StartsWith("Hi MP").Should().BeTrue();

			await grpcClient.StopAsync();
		}

		/// <summary>
		/// System test of the implementation <see cref="ProjectContentClientHandler"/> which validates grpc communication with the GrpcServer
		/// </summary>
		/// <returns></returns>
		[Test]
		public async Task ProjectContentTest()
		{
			string clientId = grpcServerPort.ToString();

			// create client of the service IService which runs on grpcServer
			GrpcServiceBasedReflectionClient<IService> grpcClient = new GrpcServiceBasedReflectionClient<IService>(nullLogger);

			var projectContentHandler = new ProjectContentClientHandler(grpcClient, nullLogger);

			// add project content handler
			grpcClient.RegisterHandler(IdeaStatiCa.Plugin.Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE, projectContentHandler);

			var task = grpcClient.StartAsync(clientId, grpcServerPort);
			grpcClient.IsConnected.Should().BeTrue("The client should be connected");

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

			await grpcClient.StopAsync();
		}

		[Test]
		public async Task GrpcBlobStorageServiceTest()
		{
			using (var client = new GrpcBlobStorageClient(nullLogger, grpcServerPort))
			{
				await client.WriteAsync("testBlobStorage", "testFile1", new MemoryStream(new byte[] { 1, 2, 3 }));

			}
		}
	}

}