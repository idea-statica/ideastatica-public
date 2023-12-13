using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin.Api.RCS.Model;

namespace IdeaStatiCa.Plugin.Api.Rcs
{
	public interface IRcsApiController : IDisposable
	{
		/// <summary>
		/// Open project from ideaRcs file, or IOM in xml
		/// </summary>
		/// <param name="project">Project information</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<bool> OpenProjectAsync(string path, CancellationToken token);

		/// <summary>
		/// Open project from Open Model object
		/// </summary>
		/// <param name="model">Project in open model format</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<bool> OpenProjectFromModelAsync(OpenModel model, CancellationToken token);

		/// <summary>
		/// Calculates RCS project
		/// When no sections are specified, everything is calculated
		/// </summary>
		/// <param name="parameters">Parameters to specify sections</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<List<RcsSectionResultOverview>> CalculateResultsAsync(RcsCalculationParameters parameters, CancellationToken token);

		/// <summary>
		/// Get calculated results for given project id
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		Task<ProjectResult> GetResultsAsync(RcsCalculationParameters parameters, CancellationToken token);

		/// <summary>
		/// Get overall information about Project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<RcsProjectModel> GetProjectOverviewAsync(CancellationToken token);

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
		Task<Stream> DownloadAsync(CancellationToken token);

		/// <summary>
		/// Return collection of section details of opened project
		/// When no sections are specified, nothing is returned
		/// </summary>
		/// <param name="parameters">Parameters to specify the sections</param>
		/// <returns></returns>
		Task<List<RcsCrossSectionDetailModel>> SectionDetailsAsync(RcsCalculationParameters parameters, CancellationToken token);


		/// <summary>
		/// Update data of the section in the RCS project.
		/// The section to modify is defined by property ID passed in <paramref name="newSectionData"/>
		/// </summary>
		/// <param name="newSectionData">New data of the section. A valid value of the section Id is requiered.</param>
		/// <param name="token">Cancellation token</param>
		/// <returns>Chanded data</returns>
		Task<RcsSectionModel> UpdateSectionAsync(RcsSectionModel newSectionData, CancellationToken token);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <param name="reinfCssTemplate"></param>
		/// <returns></returns>
		Task<RcsSectionModel> ImportReinfCssAsync(ReinfCssImportOptions options, string reinfCssTemplate);

	}
}
