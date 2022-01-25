using IdeaStatiCa.Plugin.Grpc;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace IdeaStatiCa.Plugin.Tests.gRPC
{
	public class GrpcClientTest
	{
	//	[Fact]
	//	public async Task GrpcClientHandleMessagesTest()
	//	{
	//		const string clientId = "client1";

	//		var grpcClient = new GrpcClient(clientId, 80, new NullLogger());

	//		GrpcMessage handler1Msg = null;

	//		const string messageName1 = "msg1";
	//		var handler1 = Substitute.For<IGrpcMessageHandler>();
	//		handler1.HandleClientMessage(default, default).ReturnsForAnyArgs(t => {
	//			handler1Msg = t[0] as GrpcMessage;
	//			return Task.FromResult(new object());
	//		});
	//		grpcClient.RegisterHandler(messageName1, handler1);


	//		const string messageName2 = "msg2";
	//		GrpcMessage handler2Msg = null;
	//		var handler2 = Substitute.For<IGrpcMessageHandler>();
	//		handler2.HandleClientMessage(default, default).ReturnsForAnyArgs(t => {

	//			handler2Msg = t[0] as GrpcMessage;
	//			return Task.FromResult(new object());
	//		});
	//		grpcClient.RegisterHandler(messageName2, handler2);


	//		var msg1 = new GrpcMessage();
	//		msg1.ClientId = clientId;
	//		msg1.MessageName = messageName1;
	//		msg1.MessageType = GrpcMessage.Types.MessageType.Response;
	//		msg1.Data = "1";

	//		await grpcClient.HandleMessageAsync(msg1);

	//		Assert.NotNull(handler1Msg);
	//		Assert.Equal("1", handler1Msg.Data);
	//		Assert.Null(handler2Msg);
			
	//		handler1Msg = null;

	//		var msg2 = new GrpcMessage();
	//		msg2.ClientId = clientId;
	//		msg2.MessageName = messageName2;
	//		msg2.MessageType = GrpcMessage.Types.MessageType.Response;
	//		msg2.Data = "2";

	//		await grpcClient.HandleMessageAsync(msg2);
	//		Assert.NotNull(handler2Msg);
	//		Assert.Equal("2", handler2Msg.Data);
	//		Assert.Null(handler1Msg);
	//	}
	}
}
