using Dlubal.RSTAB8;
using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Factories;
using IdeaRstabPlugin.Geometry;
using IdeaRstabPlugin.Providers;
using IdeaStatiCa.Plugin;
using System.IO;

namespace IdeaRstabPlugin
{
	public class PluginFactory : IBIMPluginFactory
	{
		public string FeaAppName => "RSTAB";

		public string IdeaStaticaAppPath
		{
			get
			{
				string ideaInstallDir = string.Empty;
#if PUBLIC
				// get installation directory from windosw registry
				ideaInstallDir = IdeaStatiCa.Plugin.Utilities.IdeaStatiCaSetupTools.GetIdeaStatiCaInstallDir(Constants.IdeaStatiCaVersion);
#else
				// the plugin is located in installation directory of IdeaStatiCa
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
				ideaInstallDir = Path.GetDirectoryName(assembly.Location);
#endif
				return Path.Combine(ideaInstallDir, Constants.CheckbotAppName);
			}
		}

		public string WorkingDirectory { get; }

		private readonly IModel _model;
		private readonly IPluginLogger _logger;

		public PluginFactory(IModel model, IPluginLogger pluginLogger)
		{
			_model = model;
			_logger = pluginLogger;
			WorkingDirectory = Path.Combine(Path.GetDirectoryName(_model.GetPath()), "IdeaStatiCa-" + _model.GetName());
			if (!Directory.Exists(WorkingDirectory))
			{
				Directory.CreateDirectory(WorkingDirectory);
			}
		}

		public IApplicationBIM Create()
		{
			using (new LicenceLock(_model))
			{
				ImportSession importSession = new ImportSession(_model);
				ModelDataProvider modelDataProvider = new ModelDataProvider(_model);
				LinesAndNodes linesAndNodes = new LinesAndNodes(modelDataProvider);
				LoadsProvider loadsProvider = new LoadsProvider(_model);
				ResultsProvider resultsProvider = new ResultsProvider(loadsProvider, _model.GetCalculation());
				ObjectFactory objectFactory = new ObjectFactory(_model, modelDataProvider, linesAndNodes, loadsProvider,
					resultsProvider, importSession);
				RstabGeometry geometry = new RstabGeometry(linesAndNodes, objectFactory);
				RstabGeometryProvider geometryProvider = new RstabGeometryProvider(geometry);
				ObjectRestorer objectRestorer = new ObjectRestorer(objectFactory, linesAndNodes);
				RstabModel rstabModel = new RstabModel(_model, linesAndNodes, objectFactory, loadsProvider, importSession);

				RstabApplication application = new RstabApplication(_logger, rstabModel, objectRestorer, geometryProvider,
					resultsProvider, importSession, WorkingDirectory);

				application.AddDataCache(modelDataProvider);
				application.AddDataCache(loadsProvider);
				application.AddDataCache(objectFactory);
				application.AddDataCache(resultsProvider);

				return application;
			}
		}
	}
}