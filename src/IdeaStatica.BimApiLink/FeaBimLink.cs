using IdeaStatica.BimApiLink.Hooks;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatica.BimApiLink.Persistence;
using IdeaStatica.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;

namespace IdeaStatica.BimApiLink
{
	internal class FeaBimLink : BimLink
	{
		public FeaBimLink(string applicationName, string projectPath) : base(applicationName, projectPath)
		{
		}

		protected override IApplicationBIM Create(
			IPluginLogger logger,
			IBimApiImporter bimApiImporter,
			string projectPath,
			BimImporterConfiguration bimImporterConfiguration,
			IProgressMessaging remoteApp,
			IBimResultsProvider resultsProvider,
			IPluginHook pluginHook,
			IModel feaModel,
			IBimUserDataSource userDataSource)
		{
			JsonPersistence jsonPersistence = new JsonPersistence();
			JsonProjectStorage projectStorage = new JsonProjectStorage(jsonPersistence, projectPath);
			Project project = new Project(logger, jsonPersistence);
			ProjectAdapter projectAdapter = new ProjectAdapter(project, bimApiImporter);
			FeaModelAdapter feaModelAdapter = new FeaModelAdapter(bimApiImporter, feaModel as IFeaModel);
			IBimImporter bimImporter = BimImporter.Create(
				feaModelAdapter,
				projectAdapter,
				logger,
				null,
				bimImporterConfiguration,
				remoteApp,
				resultsProvider);

			return new FeaApplication(
				ApplicationName,
				projectAdapter,
				projectStorage,
				bimImporter,
				bimApiImporter,
				pluginHook,
				userDataSource);
		}
	}
}
