namespace IdeaStatiCa.Plugin.Grpc
{
	public interface IGrpcServer : IGrpcCommunicator
	{
		Services.GrpcService GrpcService { get; }
	}
}
