namespace IdeaStatiCa.Plugin
{
	public interface IBimHostingFactory
	{
		IBIMPluginHosting Create(IBIMPluginFactory pluginFactory, IPluginLogger logger);
	}
}
