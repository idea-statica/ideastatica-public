using IdeaStatica.BimApiLink.Importers;
using IdeaStatica.BimApiLink.Persistence;
using IdeaStatica.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;

namespace IdeaStatica.BimApiLink
{
	public abstract class BimLink
	{
		protected string ApplicationName => _applicationName;

		private string? _ideaStatiCaPath;
		private IPluginLogger? _pluginLogger;

		private readonly string _applicationName;
		private readonly string _projectPath;

		private readonly ImportersConfiguration _importersConfiguration = new();

		public static BimLink Create(string applicationName, string checkbotProjectPath, IFeaModel feaModel)
		{
			return new FeaBimLink(applicationName, checkbotProjectPath, feaModel);
		}

		protected BimLink(string applicationName, string projectPath)
		{
			_applicationName = applicationName;
			_projectPath = projectPath;
		}

		public BimLink WithIdeaStatiCa(string path)
		{
			_ideaStatiCaPath = path;
			return this;
		}

		public BimLink WithLogger(IPluginLogger pluginLogger)
		{
			_pluginLogger = pluginLogger;
			return this;
		}

		public BimLink WithImporters(Action<ImportersConfiguration> func)
		{
			func(_importersConfiguration);
			return this;
		}

		public Task Run()
		{
			ImporterDispatcher importerDispatcher = new(_importersConfiguration.Manager);

			IPluginLogger pluginLogger = _pluginLogger ?? new NullLogger();
			IApplicationBIM applicationBIM = Create(pluginLogger, importerDispatcher, _projectPath);

			PluginFactory pluginFactory = new(
				applicationBIM,
				_applicationName,
				_ideaStatiCaPath);

			IBIMPluginHosting pluginHosting = new GrpcBimHostingFactory(pluginFactory, pluginLogger)
				.Create();

			string pid = Environment.ProcessId.ToString();
			return pluginHosting.RunAsync(pid, _projectPath);
		}

		protected abstract IApplicationBIM Create(IPluginLogger logger, IBimApiImporter bimApiImporter, string projectPath);
	}

	internal class FeaBimLink : BimLink
	{
		private readonly IFeaModel _feaModel;

		public FeaBimLink(string applicationName, string projectPath, IFeaModel feaModel)
			: base(applicationName, projectPath)
		{
			_feaModel = feaModel;
		}

		protected override IApplicationBIM Create(IPluginLogger logger, IBimApiImporter bimApiImporter, string projectPath)
		{
			JsonPersistence jsonPersistence = new();
			JsonProjectStorage projectStorage = new(jsonPersistence, projectPath);
			Project project = new(logger, jsonPersistence);
			ProjectAdapter projectAdapter = new(project, bimApiImporter);
			FeaModelAdapter feaModelAdapter = new(bimApiImporter, _feaModel);
			IBimImporter bimImporter = BimImporter.Create(feaModelAdapter, projectAdapter, logger);

			return new FeaApplication(ApplicationName, projectAdapter, projectStorage, bimImporter, bimApiImporter);
		}
	}
}