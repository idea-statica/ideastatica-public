using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.Plugin.Api.RCS.Model;

namespace IdeaStatiCa.Plugin.Api.Rcs
{
	public interface IRcsApiController : IDisposable
	{
		//Section for Grasshopper

		/// <summary>
		/// Open project from ideaRcs file, or IOM in xml
		/// </summary>
		/// <param name="project">Project information</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Guid OpenProject(string path, CancellationToken token);

		/// <summary>
		/// Open project from Open Model object
		/// </summary>
		/// <param name="model">Project in open model format</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Guid OpenProjectFromModel(OpenModel model, CancellationToken token);

		/// <summary>
		/// Calculates RCS project for given project id
		/// When no sections are specified, everything is calculated
		/// When no non-conformities are specified, nothing is returned
		/// </summary>
		/// <param name="projectId">Id of the project</param>
		/// <param name="parameters">Parameters to specify sections and nonconformities</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<ProjectResult> CalculateProjectAsync(Guid projectId, RcsCalculationParameters parameters, CancellationToken token);

		/// <summary>
		/// Get overall information about Project
		/// </summary>
		/// <param name="projectId">Id of the project</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		RcsModelOverview GetProjectOverview(Guid projectId, CancellationToken token);

		/// <summary>
		/// Get information about sections in Project
		/// </summary>
		/// <param name="projectId">Id of the project</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		IList<RcsCrossSectionOverviewModel> GetProjectSections(Guid projectId, CancellationToken token);

		/// <summary>
		/// Get information about members in Project
		/// </summary>
		/// <param name="projectId">Id of the project</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		IList<RcsCheckMemberModel> GetProjectMembers(Guid projectId, CancellationToken token);

		/// <summary>
		/// Get information about reinforced cross sections in Project
		/// </summary>
		/// <param name="projectId">Id of the project</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		IList<RcsReinforcedCrossSectionModel> GetProjectReinforcedCrossSections(Guid projectId, CancellationToken token);

		/// <summary>
		/// Return open project as file stream (*.idearcs)
		/// </summary>
		/// <param name="projectId">Id of the project</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		MemoryStream Download(Guid projectId, CancellationToken token);

		/// <summary>
		/// Return collection of section details of opened project
		/// When no sections are specified, nothing is returned
		/// </summary>
		/// <param name="projectId">Id of the project</param>
		/// <param name="parameters">Parameters to specify the sections</param>
		/// <returns></returns>
		IEnumerable<RcsCrossSectionDetailModel> SectionDetails(Guid projectId, RcsCalculationParameters parameters);


		// Section for current implementations using direct RCS class
		/// <summary>
		/// Return calculated sections for given project input
		/// </summary>
		/// <param name="projectId">Id of the project</param>
		/// <param name="parameters">Parameters to specify the sections</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		IEnumerable<SectionConcreteCheckResult> GetResultOnSections(Guid projectId, RcsCalculationParameters parameters, CancellationToken token);

		/// <summary>
		/// Returns nonconformity issues for specified GUID values
		/// </summary>
		/// <param name="projectId">Id of the project</param>
		/// <param name="parameters">Parameters to specify the sections</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		IEnumerable<NonConformityIssue> GetNonConformityIssues(Guid projectId, RcsCalculationParameters parameters, CancellationToken token);
	}
}
