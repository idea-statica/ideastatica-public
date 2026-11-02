using FluentAssertions;
using IdeaStatiCa.Plugin.Grpc;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Tests.gRPC
{
	[TestFixture]
	public class GrpcClientTest
	{
		[Test]
		public void GrpcClientHandleMessagesTest()
		{
			const string clientId = "client1";

			var grpcClient = new GrpcClient(new NullLogger());

			GrpcMessage handler1Msg = null;

			const string messageName1 = "msg1";
			var handler1 = Substitute.For<IGrpcMessageHandler>();
			handler1.HandleClientMessage(default, default).ReturnsForAnyArgs(t =>
			{
				handler1Msg = t[0] as GrpcMessage;
				return Task.FromResult(new object());
			});
			grpcClient.RegisterHandler(messageName1, handler1);


			const string messageName2 = "msg2";
			GrpcMessage handler2Msg = null;
			var handler2 = Substitute.For<IGrpcMessageHandler>();
			handler2.HandleClientMessage(default, default).ReturnsForAnyArgs(t =>
			{

				handler2Msg = t[0] as GrpcMessage;
				return Task.FromResult(new object());
			});
			grpcClient.RegisterHandler(messageName2, handler2);


			var msg1 = new GrpcMessage();
			msg1.ClientId = clientId;
			msg1.MessageName = messageName1;
			msg1.MessageType = GrpcMessage.Types.MessageType.Response;
			msg1.Data = "1";

			grpcClient.HandleMessageAsync(msg1);

			Assert.NotNull(handler1Msg);
			handler1Msg.Data.Should().Be("1");
			Assert.Null(handler2Msg);

			handler1Msg = null;

			var msg2 = new GrpcMessage();
			msg2.ClientId = clientId;
			msg2.MessageName = messageName2;
			msg2.MessageType = GrpcMessage.Types.MessageType.Response;
			msg2.Data = "2";

			grpcClient.HandleMessageAsync(msg2);
			Assert.NotNull(handler2Msg);
			handler2Msg.Data.Should().Be("2");
			Assert.Null(handler1Msg);
		}
	}
}
