using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;

namespace IdeaStatiCa.Plugin
{
	public interface IBimHostingFactory
	{
		IBIMPluginHosting Create(IBIMPluginFactory pluginFactory, IPluginLogger logger);
	}
}
