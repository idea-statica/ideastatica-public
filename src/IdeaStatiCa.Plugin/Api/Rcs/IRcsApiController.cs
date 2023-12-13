using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Concrete;
using IdeaStatiCa.Plugin.Api.RCS.Model;

namespace IdeaStatiCa.Plugin.Api.Rcs
{
	public interface IRcsApiController : IDisposable
	{
		/// <summary>
		/// Open project from IdeaRcs file
		/// </summary>
		/// <param name="rscFilePath">Local path of the IdeaRcs file</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<bool> OpenProjectAsync(string rscFilePath, CancellationToken token);

		/// <summary>
		/// Create project from Open Model object
		/// </summary>
		/// <param name="model">Project in open model format</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<bool> CreateProjectFromIOMAsync(OpenModel model, CancellationToken token);

		/// <summary>
		/// Create project from Open Model file
		/// </summary>
		/// <param name="iomFilePath">Local path of the XML file</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<bool> CreateProjectFromIOMFileAsync(string iomFilePath, CancellationToken token);

		/// <summary>
		/// Calculates RCS project
		/// When no sections are specified, everything is calculated
		/// </summary>
		/// <param name="parameters">Parameters to specify sections</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<List<RcsSectionResultOverview>> CalculateAsync(RcsCalculationParameters parameters, CancellationToken token);

		// CalculateAsync

		/// <summary>
		/// Get calculated results for given project id
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		Task<List<RcsDetailedResultForSection>> GetResultsAsync(RcsResultParameters parameters, CancellationToken token);

		/// <summary>
		/// Get information summary about the Project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<RcsProjectSummaryModel> GetProjectSummaryAsync(CancellationToken token);

		/// <summary>
		/// Get project data
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<RcsProjectData> GetProjectDataAsync(CancellationToken token);

		/// <summary>
		/// Get information about sections in Project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<List<RcsSectionModel>> GetProjectSectionsAsync(CancellationToken token);

		/// <summary>
		/// Get information about members in Project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<List<RcsCheckMemberModel>> GetProjectMembersAsync(CancellationToken token);

		/// <summary>
		/// Get information about reinforced cross sections in Project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<List<ReinforcedCrossSectionModel>> GetProjectReinforcedCrossSectionsAsync(CancellationToken token);

		/// <summary>
		/// Return open project as file stream (*.idearcs)
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<Stream> DownloadProjectAsync(CancellationToken token); // for the REST API

		/// <summary>
		/// Saves the loaded project as .IdeaRcs file on disk
		/// </summary>
		/// <param name="outputPath"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		Task SaveProjectAsync(string outputPath, CancellationToken token); // for the REST API

		/// <summary>
		/// Get the code settings
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		Task<string> GetCodeSettings(CancellationToken token);

		/// <summary>
		/// Update the code settings
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		Task<bool> UpdateCodeSettings(List<RcsSettingModel> setup);
	}
}
