using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimApiLink
{
	public class FeaBimLink : BimLink
	{
		public FeaBimLink(string applicationName, string projectPath) : base(applicationName, projectPath)
		{
		}

		public static BimLink Create(string applicationName, string checkbotProjectPath) 
			=> new FeaBimLink(applicationName, checkbotProjectPath);

		protected override IApplicationBIM Create(
			IPluginLogger logger,
			IBimApiImporter bimApiImporter,
			string projectPath,
			BimImporterConfiguration bimImporterConfiguration,
			IProgressMessaging remoteApp,
			IBimResultsProvider resultsProvider,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IModel model,
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler)
		{
			JsonPersistence jsonPersistence = new JsonPersistence(logger);
			JsonProjectStorage projectStorage = new JsonProjectStorage(jsonPersistence, projectPath);
			Project project = new Project(logger, jsonPersistence);
			ProjectAdapter projectAdapter = new ProjectAdapter(project, bimApiImporter);
			FeaModelAdapter feaModelAdapter = new FeaModelAdapter(bimApiImporter, model as IFeaModel);

			IBimImporter bimImporter = BimImporter.BimImporter.Create(
				feaModelAdapter,
				projectAdapter,
				logger,
				null,
				bimImporterConfiguration,
				remoteApp,
				resultsProvider);

			return new FeaApplication(
				ApplicationName,
				logger,
				projectAdapter,
				projectStorage,
				bimImporter,
				bimApiImporter,
				pluginHook,
				scopeHook,
				userDataSource,
				taskScheduler);
		}
	}
}