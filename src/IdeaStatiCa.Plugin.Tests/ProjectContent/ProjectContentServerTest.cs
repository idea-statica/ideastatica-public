using FluentAssertions;
using Grpc.Core;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.ProjectContent;
using IdeaStatiCa.Public;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdeaStatiCa.Plugin.Tests.ProjectContent
{
	public class ProjectContentServerTest
	{
		[Fact]
		public async Task GetContentTest()
		{
			var grpcServer = new GrpcServer(80);

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
			grpcServer.RegisterHandler(messageName, contentHandler);

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


			var readEnumerator = inputMessages.GetEnumerator();

			streamReader.MoveNext().ReturnsForAnyArgs(t =>
			{
				return readEnumerator.MoveNext();
			});

			streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

			GrpcMessage lastHandledMessage = null;

			streamWriter.WriteAsync(default).ReturnsForAnyArgs(t => {
				handledMessages.Add(t[0] as GrpcMessage);
				lastHandledMessage = t[0] as GrpcMessage;
				return Task.CompletedTask;
			});

			await grpcServer.ConnectAsync(streamReader, streamWriter, context);

			Assert.NotNull(lastHandledMessage);
			Assert.NotNull(lastHandledMessage.Data);

			var resData = JsonConvert.DeserializeObject<List<ProjectDataItem>>(lastHandledMessage.Data);
			Assert.NotNull(resData);
			resData.Should().BeEquivalentTo(projContentList);
		}
	}
}
