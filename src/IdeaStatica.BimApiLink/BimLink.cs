using IdeaStatica.BimApiLink.Importers;
using IdeaStatica.BimApiLink.Persistence;
using IdeaStatica.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using System.Diagnostics;

namespace IdeaStatica.BimApiLink
{
	public abstract class BimLink
	{
		protected string ApplicationName => _applicationName;

		private string? _ideaStatiCaPath;
		private IPluginLogger? _pluginLogger;

		private readonly string _applicationName;
		private readonly string _projectPath;
		private GrpcBimHostingFactory _bimHosting;

		private readonly ImportersConfiguration _importersConfiguration = new();

		public static BimLink Create(string applicationName, string checkbotProjectPath)
		{
			return new FeaBimLink(applicationName, checkbotProjectPath);
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

		public IProgressMessaging InitHostingClient(IPluginLogger pluginLogger)
		{
			_bimHosting = new GrpcBimHostingFactory();

			return _bimHosting.InitGrpcClient(pluginLogger);
		}

		public Task Run(IFeaModel feaModel)
		{
			ImporterDispatcher importerDispatcher = new(_importersConfiguration.Manager);

			IPluginLogger pluginLogger = _pluginLogger ?? new NullLogger();


			IApplicationBIM applicationBIM = Create(pluginLogger, importerDispatcher, _projectPath, this.InitHostingClient(pluginLogger), feaModel);

			PluginFactory pluginFactory = new(
				applicationBIM,
				_applicationName,
				_ideaStatiCaPath);

			var pluginHosting = _bimHosting.Create(pluginFactory, pluginLogger);

			string pid = Environment.ProcessId.ToString();
			return pluginHosting.RunAsync(pid, _projectPath);
		}

		protected abstract IApplicationBIM Create(IPluginLogger logger, IBimApiImporter bimApiImporter, string projectPath, IProgressMessaging remoteApp, IFeaModel feaModel);
	}

	internal class FeaBimLink : BimLink
	{
		public FeaBimLink(string applicationName, string projectPath) : base(applicationName, projectPath) { }

		protected override IApplicationBIM Create(IPluginLogger logger, IBimApiImporter bimApiImporter, string projectPath, IProgressMessaging remoteApp, IFeaModel feaModel)
		{
			JsonPersistence jsonPersistence = new();
			JsonProjectStorage projectStorage = new(jsonPersistence, projectPath);
			Project project = new(logger, jsonPersistence);
			ProjectAdapter projectAdapter = new(project, bimApiImporter);
			FeaModelAdapter feaModelAdapter = new(bimApiImporter, feaModel);
			IBimImporter bimImporter = BimImporter.Create(feaModelAdapter, projectAdapter, logger, remoteApp);

			return new FeaApplication(ApplicationName, projectAdapter, projectStorage, bimImporter, bimApiImporter);
		}
	}
}