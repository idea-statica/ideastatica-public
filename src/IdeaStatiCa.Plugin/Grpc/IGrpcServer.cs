namespace IdeaStatiCa.Plugin.Grpc
{
	public interface IGrpcServer : IGrpcCommunicator
	{
		IGrpcService GrpcService { get; }
	}
}
