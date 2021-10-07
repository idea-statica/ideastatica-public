using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdeaStatiCa.Plugin.Tests.Grpc
{
	public class GrpcTests
	{
		[Fact]
		public void Test1()
		{

		}

		//[Fact]
		//public async Task TestGrcpClientServerCommunication()
		//{
		//	var clientId = "fakeClient";
		//	var port = PortFinder.FindPort(50000, 50500);
		//	var grpcServer = new GrpcServer(port);
		//	var grpcClient = new GrpcClient(clientId, port);
		//	var operationId = Guid.NewGuid().ToString();
		//	var clientMessageReceived = new TaskCompletionSource<GrpcMessage>();
		//	var serverMessageReceived = new TaskCompletionSource<GrpcMessage>();

		//	grpcServer.Start();

		//	await grpcClient.ConnectAsync();

		//	GrpcMessage serverMessage;
		//	GrpcMessage clientMessage = new GrpcMessage()
		//	{
		//		ClientId = clientId,
		//		Data = "Some data ...",
		//		MessageName = "fakeMessage",
		//		OperationId = operationId
		//	};

		//	grpcServer.MessageReceived += async (s, a) =>
		//	{
		//		serverMessage = a;

		//		Assert.Equal(a, clientMessage);

		//		serverMessageReceived.SetResult(a);

		//		await grpcServer.SendMessageAsync(operationId, "Callback");
		//	};
		//	grpcClient.MessageReceived += (s, a) =>
		//	{
		//		Assert.Equal("Callback", a.MessageName);

		//		clientMessageReceived.SetResult(a);
		//	};

		//	await grpcClient.SendMessageAsync(clientMessage);

		//	await serverMessageReceived.Task;
		//	await clientMessageReceived.Task;
		//}

		//[Fact]
		//public async Task TestGrcpReflectionService()
		//{
		//	var clientId = "fakeClient";
		//	var port = PortFinder.FindPort(50000, 50500);
		//	var instance = new GrpcMockClass()
		//	{
		//		Value = "SampleValue"
		//	};
		//	var grpcServer = new GrpcReflectionServer(instance, port);
		//	var grpcClient = new GrpcServiceBasedReflectionClient<IGrpcMockClass>(clientId, port);

		//	grpcServer.Start();
		//	await grpcClient.ConnectAsync();

		//	var value = await grpcClient.Service.TestMethod("param", int.MaxValue, short.MaxValue, true, GrpcMockEnum.Value2, new GrpcMockSubClass()
		//	{
		//		Title = "First",
		//		ID = 1,
		//		Child = new GrpcMockSubClass()
		//		{
		//			ID = 2,
		//			Title = "Second"
		//		}
		//	});

		//	var anotherValue = grpcClient.Service.SampleValue();

		//	Assert.Equal("SampleValue", value);
		//	Assert.Equal("Here", anotherValue);
		//}
	}
}
