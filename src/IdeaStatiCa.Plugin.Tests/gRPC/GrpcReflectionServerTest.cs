using Grpc.Core;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdeaStatiCa.Plugin.Tests.gRPC
{
	public interface ITestService
	{
		int MakeSum(int a, int b);
	}

	public class GrpcReflectionServerTest
	{
		/// <summary>
		/// Test of invocation of method by GrpcReflectionServer
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task GrpcServerHandleMessagesTest()
		{
			const int ExpectedResult = 11;

			var serviceMock = Substitute.For<ITestService>();
			serviceMock.MakeSum(5, 6).Returns(11);

			var streamReader = Substitute.For<IAsyncStreamReader<GrpcMessage>>();
			var streamWriter = Substitute.For<IServerStreamWriter<GrpcMessage>>();
			var context = Substitute.For<ServerCallContext>();

			var reflexionServer = new GrpcReflectionServer(serviceMock, 80);

			List<GrpcMessage> handledMessages = new List<GrpcMessage>();

			const string messageName = GrpcReflectionMessageHandler.GRPC_REFLECTION_HANDLER_MESSAGE;

			List<GrpcMessage> inputMessages = new List<GrpcMessage>();

			var msg1Data = new GrpcReflectionInvokeData();
			msg1Data.MethodName = "MakeSum";
			var parameters = new List<GrpcReflectionArgument>();
			const int val1 = 5;
			const int val2 = 6;
			parameters.Add(new GrpcReflectionArgument().FromVal(val1));
			parameters.Add(new GrpcReflectionArgument().FromVal(val2));
			msg1Data.Parameters = parameters;

			string dataString = JsonConvert.SerializeObject(msg1Data);

			var msg1 = new GrpcMessage();
			msg1.ClientId = "client1";
			msg1.MessageName = messageName;
			msg1.Data = dataString;
			inputMessages.Add(msg1);

			var readEnumerator = inputMessages.GetEnumerator();

			streamReader.MoveNext().ReturnsForAnyArgs(t =>
			{
				return readEnumerator.MoveNext();
			});

			streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

			GrpcMessage writtenMessage = null;

			streamWriter.WriteAsync(default).ReturnsForAnyArgs(t => {
				writtenMessage = t[0] as GrpcMessage;
				return Task.CompletedTask;
			});

			await reflexionServer.ConnectAsync(streamReader, streamWriter, context);

			// response message should be written
			Assert.NotNull(writtenMessage);

			// data should be exists
			Assert.NotNull(writtenMessage.Data);

			// expected value is 11
			int returnedVal = JsonConvert.DeserializeObject<int>(writtenMessage.Data);
			Assert.Equal(ExpectedResult, returnedVal);
		}
	}
}
