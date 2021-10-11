using Grpc.Core;
using IdeaStatiCa.Plugin.Grpc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var context = Substitute.For<ServerCallContext>();

			List<GrpcMessage> handledMessages = new List<GrpcMessage>();


			var handler1 = Substitute.For<IGrpcMessageHandler>();
			handler1.HandleServerMessage(default, default).ReturnsForAnyArgs(t => {
				handledMessages.Add(t[0] as GrpcMessage);
				return Task.FromResult(new object());
			});

			const string messageName = Constants.GRPC_REFLECTION_HANDLER_MESSAGE;

			grpcServer.RegisterHandler(messageName, handler1);


			// prepare two messages to process
			List<GrpcMessage> inputMessages = new List<GrpcMessage>();

			var msg1 = new GrpcMessage();
			msg1.ClientId = "client1";
			msg1.MessageName = messageName;
			inputMessages.Add(msg1);

			var msg2 = new GrpcMessage();
			msg2.ClientId = "client1";
			msg2.MessageName = messageName;
			inputMessages.Add(msg2);

			var readEnumerator = inputMessages.GetEnumerator();

			streamReader.MoveNext().ReturnsForAnyArgs(t =>
			{
				return readEnumerator.MoveNext();
			});

			streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

			await grpcServer.ConnectAsync(streamReader, streamWriter, context);

			// message handler should be called two times
			Assert.Equal(2, handledMessages.Count);

		}
	}
}
