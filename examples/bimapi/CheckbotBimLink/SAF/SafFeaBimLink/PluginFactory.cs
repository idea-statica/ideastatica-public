using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;

namespace SafFeaBimLink
{
	internal class PluginFactory : IBIMPluginFactory
	{
		public string FeaAppName => "SAF-FEA-APP";

		public string IdeaStaticaAppPath
		{
			get
			{
				return Path.Combine("C:\\Program Files\\IDEA StatiCa\\StatiCa 24.1", Constants.CheckbotAppName);
			}
		}

		private readonly IPluginLogger _pluginLogger;
		private readonly IProgressMessaging _remoteApp;
		
		private readonly ISafDataSource _safDataSource;
		
		private readonly string _workingDirectory;

		public string WorkingDirectory { get { return _workingDirectory; } }

		public PluginFactory(IPluginLogger pluginLogger, ISafDataSource safDataSource, string workingDirectory, IProgressMessaging remoteApp)
		{
			_pluginLogger = pluginLogger;
			_safDataSource = safDataSource;
			_workingDirectory = workingDirectory;
			_remoteApp = remoteApp;
		}

		public IApplicationBIM Create()
		{
			IFilePersistence persistence = new JsonPersistence(_pluginLogger);
			IProject project = new Project(_pluginLogger, persistence, new ObjectRestorerDummy());
			ISAFConverter converter = new SAFConverter(_pluginLogger, project, _remoteApp);

			return new SafFeaApplication(_safDataSource, converter, persistence, project, _workingDirectory, _remoteApp, _pluginLogger);
		}

		private class ObjectRestorerDummy : IObjectRestorer
		{
			public IIdeaPersistentObject Restore(IIdeaPersistenceToken token)
			{
				// This method should never be called.
				throw new System.NotImplementedException();
			}
		}
	}
}
