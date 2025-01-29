using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdeaApp.Models;
using System.IO;
using System.Reflection;
using System.Windows;

namespace IdeaStatiCa.RamToIdeaApp.Services
{
	/// <summary>
	/// Responsible for starting of communication between the plugin and the IDEA Checkbot.
	/// </summary>
	public class RamPluginFactory : IBIMPluginFactory
	{
		IProjectInfo _projectInfo;
		IPluginLogger _logger;
		IProgressMessaging _remoteApp;
		IProjectService _projectService;

		public string FeaAppName => "RAM";

		public string IdeaStaticaAppPath { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="projectInfo"></param>
		/// <param name="projectService"></param>
		/// <param name="logger"></param>
		/// <param name="remoteApp"></param>
		public RamPluginFactory(IProjectInfo projectInfo, IProjectService projectService, IPluginLogger logger, IProgressMessaging remoteApp)
		{
			_projectService = projectService;
			_remoteApp = remoteApp;
			_projectInfo = projectInfo;
			_logger = logger;

			IdeaStaticaAppPath = GetCheckbotAppPath();
			if (!File.Exists(IdeaStaticaAppPath))
			{
				MessageBox.Show($"IdeaCheckbot.exe not found on path {IdeaStaticaAppPath}. Using default path C:\\Program Files\\Idea StatiCa\\ StatiCa 24.1\\IdeaCheckbot.exe", "Warning",
					MessageBoxButton.OK, MessageBoxImage.Warning);
				logger.LogError($"File path {IdeaStaticaAppPath} does not exist");
				IdeaStaticaAppPath = @"C:\Program Files\IDEA StatiCa\StatiCa 24.1\IdeaCheckbot.exe";
			}
		}

		private string GetCheckbotAppPath()
		{
			var checkbotPath = FolderHelper.FindNetFolder(_logger, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Constants.CheckbotAppName);
			_logger.LogInformation($"Checkbot Path: {checkbotPath}");
			return checkbotPath;
		}

		/// <summary>
		/// Create the instance of the application
		/// </summary>
		/// <returns></returns>
		public IApplicationBIM Create()
		{
			IFilePersistence persistence = new JsonPersistence(_logger);
			IProject project = new Project(_logger, persistence, new ObjectRestorerDummy());

			return new RamFeaApplication(_projectInfo, project, persistence, _projectService, _remoteApp, _logger);
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
