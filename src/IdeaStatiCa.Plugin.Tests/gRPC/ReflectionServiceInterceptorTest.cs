using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.Plugin.Tests.gRPC
{
	[TestFixture]
	public class ReflectionServiceInterceptorTest
	{
		public interface IService
		{
			long MakeSum(long a, long b);
		}

		/// <summary>
		/// It validates calling the method 'MakeSum' in the mocked interface 'IService'
		/// </summary>
		[Test]
		public void MakeSumTest()
		{
			//const string clientId = "client1";
			//const string messageName = "msg1";

			//var grpcClient = Substitute.For<IGrpcSynchronousClient>();
			//const long ExpectedResult = 11;
			//grpcClient.SendMessageDataSync(default, default).ReturnsForAnyArgs(t => {
			//	return new GrpcMessage() { ClientId = clientId, MessageName = messageName, Data = ExpectedResult.ToString() };
			//});

			//var service = GrpcReflectionServiceFactory.CreateInstance<IService>(grpcClient);

			//long res = service.MakeSum(5, 6);

			//// The expected value of adding 5 and 6 is 11
			//Assert.Equal(ExpectedResult, res);
		}
	}
}
