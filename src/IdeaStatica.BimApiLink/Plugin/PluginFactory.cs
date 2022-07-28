using IdeaStatiCa.Plugin;

namespace IdeaStatica.BimApiLink.Plugin
{
	internal class PluginFactory : IBIMPluginFactory
	{
		public string FeaAppName { get; }

		public string IdeaStaticaAppPath { get; }

		private readonly IApplicationBIM _application;

		public PluginFactory(IApplicationBIM application, string applicationName, string ideaStatiCaPath)
		{
			_application = application;
			FeaAppName = applicationName;
			IdeaStaticaAppPath = ideaStatiCaPath;
		}

		public IApplicationBIM Create()
			=> _application;
	}
}