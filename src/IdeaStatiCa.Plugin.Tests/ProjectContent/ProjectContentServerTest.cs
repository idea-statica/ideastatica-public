using FluentAssertions;
using Grpc.Core;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.ProjectContent;
using IdeaStatiCa.Public;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Tests.ProjectContent
{
	[TestFixture]
	public class ProjectContentServerTest
	{
		/// <summary>
		/// Test of invocation <see cref="IProjectContent.GetContent"/> on the server
		/// </summary>
		/// <returns></returns>
		[Test]
		public async Task GetContentTest()
		{
			var grpcServer = new GrpcServer(new NullLogger());

			var streamReader = Substitute.For<IAsyncStreamReader<GrpcMessage>>();
			var streamWriter = Substitute.For<IServerStreamWriter<GrpcMessage>>();

			IProjectContent projectContentMock = Substitute.For<IProjectContent>();

			var projContentList = new List<ProjectDataItem>();
			projContentList.Add(new ProjectDataItem("File1.xml", ItemType.File));
			projContentList.Add(new ProjectDataItem("File2.xml", ItemType.File));
			projectContentMock.GetContent().Returns(projContentList);

			var context = Substitute.For<ServerCallContext>();

			List<GrpcMessage> handledMessages = new List<GrpcMessage>();

			var contentHandler = new ProjectContentServerHandler(projectContentMock);

			const string messageName = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;
			grpcServer.GrpcService.RegisterHandler(messageName, contentHandler);

			// prepare two messages to process
			List<GrpcMessage> inputMessages = new List<GrpcMessage>();

			var msg1 = new GrpcMessage();
			msg1.ClientId = "client1";
			msg1.MessageName = messageName;
			inputMessages.Add(msg1);

			var getContentMsgData = new GrpcReflectionInvokeData();
			getContentMsgData.MethodName = "GetContent";
			var parameters = new List<GrpcReflectionArgument>();
			getContentMsgData.Parameters = parameters;

			msg1.Data = JsonConvert.SerializeObject(getContentMsgData);

			ManualResetEvent mre = new ManualResetEvent(false);
			int readCounter = 0;
			int writeCounter = 0;

			var readEnumerator = inputMessages.GetEnumerator();

			streamReader.MoveNext().ReturnsForAnyArgs(t =>
			{
				if (readCounter < inputMessages.Count)
				{
					readCounter++;
					return readEnumerator.MoveNext();
				}
				else
				{
					// wait till the last message is processed
					mre.WaitOne();
					return false;
				}
			});

			streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

			GrpcMessage lastHandledMessage = null;

			streamWriter.WriteAsync(default).ReturnsForAnyArgs(t =>
			{
				writeCounter++;
				handledMessages.Add(t[0] as GrpcMessage);
				lastHandledMessage = t[0] as GrpcMessage;

				if (writeCounter == inputMessages.Count)
				{
					mre.Set();
				}
				return Task.FromResult(new object());
			});

			await grpcServer.GrpcService.ConnectAsync(streamReader, streamWriter, context);

			Assert.NotNull(lastHandledMessage);
			Assert.NotNull(lastHandledMessage.Data);

			var resData = JsonConvert.DeserializeObject<List<ProjectDataItem>>(lastHandledMessage.Data);
			Assert.NotNull(resData);
			resData.Should().BeEquivalentTo(projContentList);
		}

		///// <summary>
		///// Test of invocation <see cref="IProjectContent.Exist(string)"/> on the server
		///// </summary>
		///// <returns></returns>
		//[Test]
		//public async Task ExistTest()
		//{
		//	var grpcServer = new GrpcServer(new NullLogger());

		//	var streamReader = Substitute.For<IAsyncStreamReader<GrpcMessage>>();
		//	var streamWriter = Substitute.For<IServerStreamWriter<GrpcMessage>>();

		//	IProjectContent projectContentMock = Substitute.For<IProjectContent>();

		//	string item1 = "item1";
		//	string item2 = "item2";
		//	string item3 = "*";

		//	projectContentMock.Exist(default).ReturnsForAnyArgs(m =>
		//	{
		//		bool res = false;

		//		var param = m[0] as string;
		//		if (param == item1)
		//		{
		//			res = true;
		//		}
		//		else if (param == item2)
		//		{
		//			res = false;
		//		}
		//		else
		//		{
		//			throw new Exception("Invalid character");
		//		}

		//		return res;
		//	});


		//	var context = Substitute.For<ServerCallContext>();

		//	List<GrpcMessage> handledMessages = new List<GrpcMessage>();

		//	var contentHandler = new ProjectContentServerHandler(projectContentMock);

		//	const string messageName = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;
		//	grpcServer.RegisterHandler(messageName, contentHandler);

		//	// prepare two messages to process
		//	List<GrpcMessage> inputMessages = new List<GrpcMessage>();
		//	ManualResetEvent mre = new ManualResetEvent(false);
		//	int readCounter = 0;
		//	int writeCounter = 0;

		//	var msg1 = new GrpcMessage();
		//	msg1.ClientId = "client1";
		//	msg1.MessageName = messageName;
		//	inputMessages.Add(msg1);

		//	var getContentMsgData = new GrpcReflectionInvokeData();
		//	getContentMsgData.MethodName = "Exist";
		//	var parameters = new List<GrpcReflectionArgument>();

		//	var arg = new GrpcReflectionArgument();
		//	arg.FromVal(item1);

		//	parameters.Add(arg);
		//	getContentMsgData.Parameters = parameters;

		//	msg1.Data = JsonConvert.SerializeObject(getContentMsgData);

		//	GrpcMessage msg2 = new GrpcMessage(msg1);
		//	arg.FromVal(item2);
		//	msg2.Data = JsonConvert.SerializeObject(getContentMsgData);
		//	inputMessages.Add(msg2);

		//	GrpcMessage msg3 = new GrpcMessage(msg2);
		//	arg.FromVal(item3);
		//	msg3.Data = JsonConvert.SerializeObject(getContentMsgData);
		//	inputMessages.Add(msg3);

		//	var readEnumerator = inputMessages.GetEnumerator();

		//	streamReader.MoveNext().ReturnsForAnyArgs(t =>
		//	{
		//		if (readCounter < inputMessages.Count)
		//		{
		//			readCounter++;
		//			return readEnumerator.MoveNext();
		//		}
		//		else
		//		{
		//			// wait till the last message is processed
		//			mre.WaitOne();
		//			return false;
		//		}
		//	});

		//	streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

		//	GrpcMessage lastHandledMessage = null;

		//	streamWriter.WriteAsync(default).ReturnsForAnyArgs(t =>
		//	{
		//		writeCounter++;
		//		handledMessages.Add(t[0] as GrpcMessage);
		//		lastHandledMessage = t[0] as GrpcMessage;

		//		if (writeCounter == inputMessages.Count)
		//		{
		//			mre.Set();
		//		}
		//		return Task.CompletedTask;
		//	});

		//	await grpcServer.ConnectAsync(streamReader, streamWriter, context);

		//	Assert.NotNull(lastHandledMessage);
		//	Assert.NotNull(lastHandledMessage.Data);

		//	handledMessages[0].Data.Should().Be("true");
		//	handledMessages[1].Data.Should().Be("false");
		//	handledMessages[2].Data.Should().StartWith("Error");
		//}

		[Test]
		public async Task DeleteTest()
		{
			var grpcServer = new GrpcServer(new NullLogger());

			var streamReader = Substitute.For<IAsyncStreamReader<GrpcMessage>>();
			var streamWriter = Substitute.For<IServerStreamWriter<GrpcMessage>>();

			IProjectContent projectContentMock = Substitute.For<IProjectContent>();

			string item1 = "item1";
			string item2 = "*";

			var context = Substitute.For<ServerCallContext>();

			List<GrpcMessage> handledMessages = new List<GrpcMessage>();

			var contentHandler = new ProjectContentServerHandler(projectContentMock);

			const string messageName = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;
			grpcServer.GrpcService.RegisterHandler(messageName, contentHandler);

			// prepare two messages to process
			List<GrpcMessage> inputMessages = new List<GrpcMessage>();
			ManualResetEvent mre = new ManualResetEvent(false);
			int readCounter = 0;
			int writeCounter = 0;

			var msg1 = new GrpcMessage();
			msg1.ClientId = "client1";
			msg1.MessageName = messageName;
			inputMessages.Add(msg1);

			var getContentMsgData = new GrpcReflectionInvokeData();
			getContentMsgData.MethodName = "Delete";
			var parameters = new List<GrpcReflectionArgument>();

			var arg = new GrpcReflectionArgument();
			arg.FromVal(item1);

			parameters.Add(arg);
			getContentMsgData.Parameters = parameters;

			msg1.Data = JsonConvert.SerializeObject(getContentMsgData);

			GrpcMessage msg2 = new GrpcMessage(msg1);
			arg.FromVal(item2);
			msg2.Data = JsonConvert.SerializeObject(getContentMsgData);
			inputMessages.Add(msg2);

			var readEnumerator = inputMessages.GetEnumerator();

			streamReader.MoveNext().ReturnsForAnyArgs(t =>
			{
				if (readCounter < inputMessages.Count)
				{
					readCounter++;
					return readEnumerator.MoveNext();
				}
				else
				{
					// wait till the last message is processed
					mre.WaitOne();
					return false;
				}
			});

			streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

			GrpcMessage lastHandledMessage = null;

			streamWriter.WriteAsync(default).ReturnsForAnyArgs(t =>
			{
				writeCounter++;
				handledMessages.Add(t[0] as GrpcMessage);
				lastHandledMessage = t[0] as GrpcMessage;

				if (writeCounter == inputMessages.Count)
				{
					mre.Set();
				}

				return Task.CompletedTask;
			});

			await grpcServer.GrpcService.ConnectAsync(streamReader, streamWriter, context);

			Assert.NotNull(lastHandledMessage);
			Assert.NotNull(lastHandledMessage.Data);

			handledMessages[0].Data.Should().Be("OK");
			handledMessages[1].Data.Should().Be("OK");
		}

		[Test]
		public async Task WriteTest()
		{
			var grpcServer = new GrpcServer(new NullLogger());

			var streamReader = Substitute.For<IAsyncStreamReader<GrpcMessage>>();
			var streamWriter = Substitute.For<IServerStreamWriter<GrpcMessage>>();

			IProjectContent projectContentMock = Substitute.For<IProjectContent>();


			string item1 = "item1";

			using (var memStream = new MemoryStream())
			{
				var storage = Substitute.For<IProjectContentStorage>();
				storage.WriteData(default, default).ReturnsForAnyArgs(t =>
				{
					var arg1 = (t[0] as string);
					Stream st = (t[1] as Stream);
					memStream.Seek(0, SeekOrigin.Begin);
					st.CopyTo(memStream);
					memStream.Seek(0, SeekOrigin.Begin);
					return 1;
				});

				projectContentMock.Exist(item1).Returns(true);
				projectContentMock.Get(item1).ReturnsForAnyArgs(t =>
				{
					return new RemoteDataStream(item1, storage);
				});

				var context = Substitute.For<ServerCallContext>();

				List<GrpcMessage> handledMessages = new List<GrpcMessage>();

				var contentHandler = new ProjectContentServerHandler(projectContentMock);

				const string messageName = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;
				grpcServer.GrpcService.RegisterHandler(messageName, contentHandler);

				// prepare two messages to process
				List<GrpcMessage> inputMessages = new List<GrpcMessage>();
				ManualResetEvent mre = new ManualResetEvent(false);
				int readCounter = 0;
				int writeCounter = 0;

				var msg1 = new GrpcMessage();
				msg1.ClientId = "client1";
				msg1.MessageName = messageName;
				inputMessages.Add(msg1);

				var getContentMsgData = new GrpcReflectionInvokeData();
				getContentMsgData.MethodName = "Write";
				var parameters = new List<GrpcReflectionArgument>();

				var arg = new GrpcReflectionArgument();
				arg.FromVal(item1);

				parameters.Add(arg);
				getContentMsgData.Parameters = parameters;

				msg1.Data = JsonConvert.SerializeObject(getContentMsgData);

				int bufferSize = 5;
				byte[] buffer = new byte[bufferSize];
				for (int i = 0; i < bufferSize; i++)
				{
					buffer[i] = Convert.ToByte(i);
				}

				msg1.Buffer = Google.Protobuf.ByteString.CopyFrom(buffer);

				var readEnumerator = inputMessages.GetEnumerator();

				streamReader.MoveNext().ReturnsForAnyArgs(t =>
				{
					if (readCounter < inputMessages.Count)
					{
						readCounter++;
						return readEnumerator.MoveNext();
					}
					else
					{
						// wait till the last message is processed
						mre.WaitOne();
						return false;
					}
				});

				streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

				GrpcMessage lastHandledMessage = null;

				streamWriter.WriteAsync(default).ReturnsForAnyArgs(t =>
				{
					writeCounter++;
					handledMessages.Add(t[0] as GrpcMessage);
					lastHandledMessage = t[0] as GrpcMessage;

					if (writeCounter == inputMessages.Count)
					{
						mre.Set();
					}

					return Task.CompletedTask;
				});

				await grpcServer.GrpcService.ConnectAsync(streamReader, streamWriter, context);

				Assert.NotNull(lastHandledMessage);
				lastHandledMessage.Data.Should().Be("OK");

				memStream.Seek(0, SeekOrigin.Begin);
				memStream.Length.Should().Be(bufferSize);

				int count = 0;
				while (count < memStream.Length)
				{
					byte val = (byte)memStream.ReadByte();
					val.Should().Be(Convert.ToByte(count));
					count++;
				}
			}
		}


		[Test]
		public async Task ReadTest()
		{
			var grpcServer = new GrpcServer(new NullLogger());

			var streamReader = Substitute.For<IAsyncStreamReader<GrpcMessage>>();
			var streamWriter = Substitute.For<IServerStreamWriter<GrpcMessage>>();

			IProjectContent projectContentMock = Substitute.For<IProjectContent>();

			string item1 = "item1";
			using (var memStream = new MemoryStream())
			{
				// create the array of 5 bytes and write original values which will be checked 
				int bufferSize = 5;
				byte[] buffer = new byte[bufferSize];
				for (int i = 0; i < bufferSize; i++)
				{
					memStream.WriteByte(Convert.ToByte(i));
				}

				memStream.Seek(0, SeekOrigin.Begin);

				projectContentMock.Get(item1).Returns(memStream);
				projectContentMock.Exist(item1).Returns(true);

				var context = Substitute.For<ServerCallContext>();

				List<GrpcMessage> handledMessages = new List<GrpcMessage>();

				var contentHandler = new ProjectContentServerHandler(projectContentMock);

				const string messageName = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;
				grpcServer.GrpcService.RegisterHandler(messageName, contentHandler);

				// prepare two messages to process
				List<GrpcMessage> inputMessages = new List<GrpcMessage>();
				ManualResetEvent mre = new ManualResetEvent(false);
				int readCounter = 0;
				int writeCounter = 0;

				var msg1 = new GrpcMessage();
				msg1.ClientId = "client1";
				msg1.MessageName = messageName;
				inputMessages.Add(msg1);

				var getContentMsgData = new GrpcReflectionInvokeData();
				getContentMsgData.MethodName = "Read";
				var parameters = new List<GrpcReflectionArgument>();

				var arg = new GrpcReflectionArgument();
				arg.FromVal(item1);

				parameters.Add(arg);
				getContentMsgData.Parameters = parameters;

				msg1.Data = JsonConvert.SerializeObject(getContentMsgData);

				var readEnumerator = inputMessages.GetEnumerator();

				streamReader.MoveNext().ReturnsForAnyArgs(t =>
				{
					if (readCounter < inputMessages.Count)
					{
						readCounter++;
						return readEnumerator.MoveNext();
					}
					else
					{
						// wait till the last message is processed
						mre.WaitOne();
						return false;
					}
				});

				streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

				GrpcMessage lastHandledMessage = null;

				streamWriter.WriteAsync(default).ReturnsForAnyArgs(t =>
				{
					writeCounter++;
					handledMessages.Add(t[0] as GrpcMessage);
					lastHandledMessage = t[0] as GrpcMessage;

					if (writeCounter == inputMessages.Count)
					{
						mre.Set();
					}

					return Task.CompletedTask;
				});

				await grpcServer.GrpcService.ConnectAsync(streamReader, streamWriter, context);

				Assert.NotNull(lastHandledMessage);
				lastHandledMessage.Data.Should().Be("OK");

				var resByteArray = lastHandledMessage.Buffer.ToByteArray();
				resByteArray.Length.Should().Be(bufferSize);
				for (int i = 0; i < bufferSize; i++)
				{
					resByteArray[i].Should().Be(Convert.ToByte(i));
				}
			}
		}
	}
}
