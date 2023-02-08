using FluentAssertions;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.ProjectContent;
using IdeaStatiCa.Plugin.Utilities;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.Public;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemTestService;

namespace ST_GrpcCommunication
{
	/// <summary>
	/// gRPC communication tests
	/// </summary>
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
		private const int chunkSize = 10;
		private IPluginLogger logger;

		private readonly string blobStorageName = "testBlobStorage";
		private IBlobStorageProvider blobStorageProviderMock;
		private IBlobStorage blobStorageMock;
		private MemoryStream writtenContent;
		private Dictionary<string, byte[]> readDataCases;

		/// <summary>
		/// Starts gRPC server in different task before all tests
		/// </summary>
		[OneTimeSetUp]
		public void StartGrpcServerTask()
		{
			logger = LoggerProvider.GetLogger("not used name");

			grpcServerPort = PortFinder.FindPort(50000, 50500);
			var fooService = new Service();
			CreateBlobStorageMocks();
			var serverStartedEvent = new AutoResetEvent(false);

			Task.Run(() =>
			{
				// grpc server
				grpcServer = new GrpcReflectionServer(fooService, logger, blobStorageProviderMock, chunkSize: chunkSize);

				// project content handler
				var projectContentInMemory = new ProjectContentInMemory();
				var contentHandler = new ProjectContentServerHandler(projectContentInMemory);
				grpcServer.GrpcService.RegisterHandler(IdeaStatiCa.Plugin.Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE, contentHandler);

				// grpc server start
				grpcServer.StartAsync(null, grpcServerPort);

				logger.LogDebug($"GrpcServer is listening on port '{grpcServerPort}'");

				serverStartedEvent.Set();
			});

			if (!serverStartedEvent.WaitOne(StartTimeout))
			{
				throw new TimeoutException($"Time out - gRPC server doesn't start within {TimeSpan.FromMilliseconds(StartTimeout).TotalSeconds} seconds.");
			}
		}

		/// <summary>
		/// Stops gRPC server after all the tests are finished
		/// </summary>
		[OneTimeTearDown]
		public async Task StopGrpcServerTask()
		{
			await grpcServer.StopAsync();
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
			GrpcServiceBasedReflectionClient<IService> grpcClient = new GrpcServiceBasedReflectionClient<IService>(logger);

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
			GrpcServiceBasedReflectionClient<IService> grpcClient = new GrpcServiceBasedReflectionClient<IService>(logger);

			var projectContentHandler = new ProjectContentClientHandler(grpcClient, logger);

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

		/// <summary>
		/// Tests writing to blob storage through gRPC. Tests different lengths of memory streams.
		/// </summary>
		/// <param name="dataLength">Length of written stream</param>
		[TestCase(0)]
		[TestCase(3)]
		[TestCase(10)]
		[TestCase(11)]
		[TestCase(1579)]
		public async Task GrpcWriteBlobStorageTest(int dataLength)
		{
			var contentId = "testFile1";
			using (var content = new MemoryStream(GenerateRandomByteArray(dataLength)))
			{
				using (var client = new GrpcBlobStorageClient(logger, grpcServerPort, chunkSize: chunkSize))
				{
					using (writtenContent = new MemoryStream(dataLength))
					{
						await client.WriteAsync(blobStorageName, contentId, content);
						blobStorageMock.Received().Write(Arg.Any<MemoryStream>(), contentId);
						Assert.IsTrue(AreStreamsEqual(content, writtenContent), "Sent and received streams are different.");
					}
				}
			}
		}

		/// <summary>
		/// Tests reading from blob storage through gRPC. Tests different lengths of memory streams.
		/// </summary>
		/// <param name="contentId">Id of the content in blob storage</param>
		[TestCase("zero")]
		[TestCase("one")]
		[TestCase("ten")]
		[TestCase("two hundred and ninety nine")]
		public async Task GrpcReadBlobStorageTest(string contentId)
		{
			using (var client = new GrpcBlobStorageClient(logger, grpcServerPort))
			{
				using (var readStream = await client.ReadAsync(blobStorageName, contentId))
				{
					using (var sentStream = new MemoryStream(readDataCases[contentId]))
					{
						Assert.IsTrue(AreStreamsEqual(sentStream, readStream), "Sent and read streams are different.");
					}
				}
			}
		}

		/// <summary>
		/// Tests if content exists in blob storage through gRPC.
		/// </summary>
		/// <param name="contentId">Id of the content in blob storage</param>
		[TestCase("exists", ExpectedResult = true)]
		[TestCase("does not exist", ExpectedResult = false)]
		public async Task<bool> GrpcExistBlobStorageTest(string contentId)
		{
			using (var client = new GrpcBlobStorageClient(logger, grpcServerPort))
			{
				var exist = await client.ExistAsync(blobStorageName, contentId);
				return exist;
			}
		}

		/// <summary>
		/// Tests deletion of content in blob storage through gRPC.
		/// </summary>
		[Test]
		public async Task GrpcDeleteBlobStorageTest()
		{
			var contentId = "test delete";
			using (var client = new GrpcBlobStorageClient(logger, grpcServerPort))
			{
				await client.DeleteAsync(blobStorageName, contentId);
			}

			blobStorageMock.Received(1).Delete(contentId);
		}

		/// <summary>
		/// Tests returning all items in blob storage through gRPC.
		/// </summary>
		[Test]
		public async Task GrpcGetEntriesBlobStorageTest()
		{
			var entries = new List<string>() { "one", "two", "three", "four" };
			blobStorageMock.GetEntries().Returns(entries);

			using (var client = new GrpcBlobStorageClient(logger, grpcServerPort))
			{
				var returnedEntries = await client.GetEntriesAsync(blobStorageName);

				blobStorageMock.Received(1).GetEntries();
				CollectionAssert.AreEqual(entries, returnedEntries);
			}
		}

		/// <summary>
		/// Creates blob storage mock and blob storage provider mock and defines some methods before tests running
		/// </summary>
		private void CreateBlobStorageMocks()
		{
			// blob storage & blob storage provider
			blobStorageMock = Substitute.For<IBlobStorage>();
			blobStorageProviderMock = Substitute.For<IBlobStorageProvider>();

			blobStorageProviderMock.GetBlobStorage(blobStorageName).Returns(blobStorageMock);

			// Write
			// copy stream due to dispose in server Write method
			blobStorageMock.Write(
				Arg.Do<MemoryStream>(c =>
				{
					c.CopyTo(writtenContent);
					writtenContent.Seek(0, SeekOrigin.Begin);
				}),
				Arg.Any<string>());

			// Read
			readDataCases = new Dictionary<string, byte[]>
			{
				["zero"] = GenerateRandomByteArray(0),
				["one"] = GenerateRandomByteArray(1),
				["ten"] = GenerateRandomByteArray(10),
				["two hundred and ninety nine"] = GenerateRandomByteArray(299),
			};

			blobStorageMock.Read(Arg.Is<string>(c => readDataCases.ContainsKey(c))).Returns(x => new MemoryStream(readDataCases[x.Arg<string>()]));
			blobStorageMock.Read(Arg.Is<string>(c => !readDataCases.ContainsKey(c))).Throws(x => new ArgumentException($"Unknow parameter '{x[0]}'."));

			// Exist
			blobStorageMock.Exist(Arg.Is("exists")).Returns(true);
			blobStorageMock.Exist(Arg.Is("does not exist")).Returns(false);
		}

		private byte[] GenerateRandomByteArray(int length)
		{
			var random = new Random();
			var byteArray = new byte[length];
			random.NextBytes(byteArray);
			return byteArray;
		}

		private bool AreStreamsEqual(Stream stream, Stream other)
		{
			const int bufferSize = 2048;
			if (other.Length != stream.Length)
			{
				return false;
			}

			byte[] buffer = new byte[bufferSize];
			byte[] otherBuffer = new byte[bufferSize];
			while ((_ = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				var _ = other.Read(otherBuffer, 0, otherBuffer.Length);

				if (!otherBuffer.SequenceEqual(buffer))
				{
					stream.Seek(0, SeekOrigin.Begin);
					other.Seek(0, SeekOrigin.Begin);
					return false;
				}
			}
			stream.Seek(0, SeekOrigin.Begin);
			other.Seek(0, SeekOrigin.Begin);
			return true;
		}
	}

}