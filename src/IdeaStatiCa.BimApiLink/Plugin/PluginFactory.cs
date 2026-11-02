using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	internal class PluginFactory : IBIMPluginFactoryWithArguments
	{
		public string FeaAppName { get; }

		public string IdeaStaticaAppPath { get; }

		/// <inheritdoc />
		public string IdeaStaticaAppArguments { get; }

		private readonly IApplicationBIM _application;

		public PluginFactory(IApplicationBIM application, string applicationName, string ideaStatiCaPath, string ideaStatiCaArguments = null)
		{
			_application = application;
			FeaAppName = applicationName;
			IdeaStaticaAppPath = ideaStatiCaPath;
			IdeaStaticaAppArguments = ideaStatiCaArguments;
		}

		public IApplicationBIM Create()
			=> _application;
	}
}