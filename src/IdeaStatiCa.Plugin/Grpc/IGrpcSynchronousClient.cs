namespace IdeaStatiCa.Plugin.Grpc
{
	public interface IGrpcSynchronousClient : IGrpcClient
	{
		GrpcMessage SendMessageDataSync(string messageName, string data);
		GrpcMessage SendMessageDataSync(GrpcMessage grpcMessage);
	}
}
