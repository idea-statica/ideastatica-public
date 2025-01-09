using IdeaRS.OpenModel;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdea;
using IdeaStatiCa.RamToIdeaApp.Models;
using System.Runtime.InteropServices;

namespace IdeaStatiCa.RamToIdeaApp.Services
{
	/// <summary>
	/// The logic of converting Ram model to <see cref="ModelBIM"/>"
	/// </summary>
	public class ProjectService : IProjectService
	{
		private readonly IPluginLogger _logger;

		public ProjectService(IPluginLogger logger)
		{
			this._logger = logger;
		}

		/// <summary>
		/// Convert all Ram model to <see cref="ModelBIM"/>
		/// </summary>
		/// <param name="projectInfo"></param>
		/// <param name="project"></param>
		/// <param name="countryCode"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
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
	}
}
