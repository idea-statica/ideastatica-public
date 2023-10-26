using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimApiLink
{
	public abstract class BimLink
	{
		protected string ApplicationName { get; }

		private string _ideaStatiCaPath;
		private IPluginLogger _pluginLogger;
		private ResultsImportersConfiguration _resultsImportersConfiguration;
		private BimImporterConfiguration _bimImporterConfiguration;
		private readonly string _projectPath;
		private IBimUserDataSource _bimUserDataSource = new NullBimUserDataSource();
		private TaskScheduler _taskScheduler = TaskScheduler.Default;
		private IBimHostingFactory _bimHostingFactory = new GrpcBimHostingFactory();
		private IProgressMessaging _progressMessaging;

		private readonly ImportersConfiguration _importersConfiguration = new ImportersConfiguration();
		private readonly HookManagers _hookManagers = new HookManagers();

		protected BimLink(string applicationName, string projectPath)
		{
			ApplicationName = applicationName;
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

		public BimLink WithResultsImporters(Action<ResultsImportersConfiguration> func)
		{
			ResultsImportersConfiguration conf = new ResultsImportersConfiguration();
			func(conf);
			_resultsImportersConfiguration = conf;

			return this;
		}

		public BimLink WithBimImporterConfiguration(BimImporterConfiguration configuration)
		{
			_bimImporterConfiguration = configuration;
			return this;
		}

		public BimLink WithImporterHook(IImporterHook hook)
		{
			_hookManagers.ImporterHookManager.Add(hook);
			return this;
		}

		public BimLink WithPluginHook(IPluginHook hook)
		{
			_hookManagers.PluginHookManager.Add(hook);
			return this;
		}

		public BimLink WithScopeHook(IScopeHook hook)
		{
			_hookManagers.ScopeHookManager.Add(hook);
			return this;
		}

		public BimLink WithUserDataSource(IBimUserDataSource userDataSource)
		{
			_bimUserDataSource = userDataSource;
			return this;
		}

		public BimLink WithTaskScheduler(TaskScheduler taskScheduler)
		{
			_taskScheduler = taskScheduler;
			return this;
		}

		public BimLink WithBimHostingFactory(IBimHostingFactory bimHostingFactory)
		{
			_bimHostingFactory = bimHostingFactory;
			return this;
		}

		public BimLink WithProgressMessaging(IProgressMessaging progressMessaging)
		{
			_progressMessaging = progressMessaging;
			return this;
		}

		public IApplicationBimRunnable Create(IModel model)
		{
			if (model is null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			IPluginLogger pluginLogger = _pluginLogger ?? new NullLogger();
			IBimHostingFactory bimHostingFactory = _bimHostingFactory ?? new GrpcBimHostingFactory();
			IProgressMessaging progressMessaging = _progressMessaging;

			if (progressMessaging is null && bimHostingFactory is GrpcBimHostingFactory grpcBimHostingFactory)
			{
				progressMessaging = grpcBimHostingFactory.InitGrpcClient(pluginLogger);
			}

			IApplicationBIM applicationBIM = CreateApplicationBIM(model, pluginLogger, progressMessaging);

			return new ApplicationBimRunnable(
				applicationBIM,
				bimHostingFactory,
				pluginLogger,
				ApplicationName,
				_ideaStatiCaPath,
				_projectPath);
		}

		private IApplicationBIM CreateApplicationBIM(IModel model, IPluginLogger pluginLogger, IProgressMessaging progressMessaging = null)
		{
			ImporterDispatcher importerDispatcher = new ImporterDispatcher(
							_importersConfiguration.Manager,
							_hookManagers.ImporterHookManager);

			IBimResultsProvider resultsProvider = _resultsImportersConfiguration?.ResultsProvider ?? new DefaultResultsProvider();
			BimImporterConfiguration bimImporterConfiguration = _bimImporterConfiguration ?? new BimImporterConfiguration();

			IApplicationBIM applicationBIM = Create(
				pluginLogger,
				importerDispatcher,
				_projectPath,
				bimImporterConfiguration,
				progressMessaging,
				resultsProvider,
				_hookManagers.PluginHookManager,
				_hookManagers.ScopeHookManager,
				model,
				_bimUserDataSource,
				_taskScheduler);
			return applicationBIM;
		}

		protected abstract IApplicationBIM Create(
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
			TaskScheduler taskScheduler);

		private sealed class NullBimUserDataSource : IBimUserDataSource
		{
			public object GetUserData() => null;
		}
	}
}