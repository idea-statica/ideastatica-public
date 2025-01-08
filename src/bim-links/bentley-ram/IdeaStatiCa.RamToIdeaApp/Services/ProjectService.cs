using IdeaRS.OpenModel;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdea;
using IdeaStatiCa.RamToIdeaApp.Models;
using Serilog.Core;
using System.IO;
using System.Runtime.InteropServices;

namespace IdeaStatiCa.RamToIdeaApp.Services
{
	public class ProjectService : IProjectService
	{
		private readonly IPluginLogger _logger;

		public ProjectService(IPluginLogger logger)
		{
			this._logger = logger;
		}

		public ModelBIM GetModel(IProjectInfo projectInfo, IProject project, CountryCode countryCode)
		{
			_logger.LogDebug($"ProjectService.GetModel : Opening database '{projectInfo.RamDbFileName}'");
			using (RamDatabase ramDatabase = new RamDatabase(projectInfo.RamDbFileName, countryCode))
			{
				try
				{
					_logger.LogDebug($"ProjectService.GetModel : Database is open, Calling ramDatabase.GetModel()");
					var ideaModel = ramDatabase.GetModel();

					_logger.LogDebug($"ProjectService.GetModel : creating BimImporter");
					IBimImporter importer = BimImporter.BimImporter.Create(ideaModel, project, _logger);
					_logger.LogDebug($"ProjectService.GetModel : starting import of connections");
					var modelBim = importer.ImportConnections(countryCode);
					_logger.LogDebug($"ProjectService.GetModel : modelBIM is generated");
					return modelBim;
				}
				catch (COMException)
				{
					ramDatabase.GetLastError(out string shortError, out string longError, out int errorId);
					throw new System.Exception(longError);
				}
			}
		}

		public bool IsAvailable()
		{
			return RamDatabase.IsInstalled();
		}

		private static void LoadMapping(JsonPersistence persistence, string persistencePath)
		{
			if (File.Exists(persistencePath))
			{
				using (FileStream fs = File.OpenRead(persistencePath))
				{
					persistence.Load(fs);
				}
			}
		}

		private string GetPersistencePath(string ramDbFileName)
		{
			return string.Empty;
		}
	}
}
