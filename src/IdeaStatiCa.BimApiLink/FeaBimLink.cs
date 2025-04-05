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
			TaskScheduler taskScheduler,
			bool highlightSelection = true)
		{
			JsonPersistence jsonPersistence = new JsonPersistence(logger);
			JsonProjectStorage projectStorage = new JsonProjectStorage(jsonPersistence, projectPath);
			Project project = new Project(logger, jsonPersistence);
			ProjectAdapter projectAdapter = new ProjectAdapter(project, bimApiImporter);
			FeaModelAdapter feaModelAdapter = new FeaModelAdapter(model as IFeaModel);

			IBimImporter bimImporter = BimImporter.BimImporter.Create(
				feaModelAdapter,
				projectAdapter,
				logger,
				null,
				bimImporterConfiguration,
				remoteApp,
				resultsProvider);

			var appBim =  CreateAppBim(logger,
				bimApiImporter,
				pluginHook,
				scopeHook,
				userDataSource,
				taskScheduler, 
				projectStorage, 
				projectAdapter, 
				bimImporter,
				model as IFeaModel,
				highlightSelection);

			return appBim;
		}

		protected virtual IApplicationBIM CreateAppBim(IPluginLogger logger,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler,
			IProjectStorage projectStorage,
			IProject projectAdapter,
			IBimImporter bimImporter,
			IFeaModel model,
			bool highlightSelection)
		{
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
				taskScheduler,
				highlightSelection);
		}
	}
}