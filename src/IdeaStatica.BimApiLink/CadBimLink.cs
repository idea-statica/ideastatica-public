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
	public class CadBimLink : BimLink
	{
		public CadBimLink(string applicationName, string projectPath) : base(applicationName, projectPath)
		{
		}
		public static BimLink Create(string applicationName, string checkbotProjectPath) => new CadBimLink(applicationName, checkbotProjectPath);

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
			CadModelAdapter cadModelAdapter = new CadModelAdapter(bimApiImporter, feaModel as ICadModel);
			IBimImporter bimImporter = BimImporter.Create(
				cadModelAdapter,
				projectAdapter,
				logger,
				null,
				bimImporterConfiguration,
				remoteApp,
				resultsProvider);
			return CreateApplicationInstace(bimApiImporter, pluginHook, userDataSource, projectStorage, projectAdapter, bimImporter);
		}

		protected virtual IApplicationBIM CreateApplicationInstace(IBimApiImporter bimApiImporter, IPluginHook pluginHook, IBimUserDataSource userDataSource, IProjectStorage projectStorage, IProject projectAdapter, IBimImporter bimImporter)
		{
			return new CadApplication(
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
