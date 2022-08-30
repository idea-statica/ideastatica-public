using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;

namespace IdeaStatiCa.Plugin
{
	public interface IBimHostingFactory
	{
		IBIMPluginHosting Create(GrpcServiceClient<IIdeaStaticaApp> checkBotClient = null, GrpcServer grpcServer = null);
	}
}
