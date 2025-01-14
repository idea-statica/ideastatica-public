using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdeaApp.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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

		public string IdeaStaticaAppPath
		{
			get
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
				Version version = new Version(versionInfo.ProductVersion ?? throw new InvalidDataException("Version info does not contain product version."));
				return @$"C:\Program Files\IDEA StatiCa\StatiCa {version.Major}.{version.Minor}\IdeaCheckbot.exe";
			}
		}

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

			if (!File.Exists(IdeaStaticaAppPath))
			{
				logger.LogError($"File path {IdeaStaticaAppPath} does not exist");
				throw new FileNotFoundException($"IdeaCheckbot.exe file not found on path {IdeaStaticaAppPath}");
			}
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
