using System;
using System.Collections.Generic;
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
		/// Calculates RCS project for given input parameter
		/// When no sections are specified, everything is calculated
		/// When no non-conformities are specified, nothing is returned
		/// </summary>
		/// <param name="projectInfo">Project info with path</param>
		/// <returns></returns>
		Task<ProjectResult> CalculateProjectAsync(RcsProjectInfo projectInfo, CancellationToken token);

		/// <summary>
		/// Calculates RCS project for given input parameter
		/// When no sections are specified, everything is calculated
		/// When no non-conformities are specified, nothing is returned
		/// </summary>
		/// <param name="projectOpenModel">Project specified in OpenModel format</param>
		/// <returns></returns>
		Task<ProjectResult> CalculateProjectOpenModelAsync(OpenModel projectOpenModel, CancellationToken token);

		/// <summary>
		/// Get information about Project
		/// </summary>
		/// <param name="projectInfo"></param>
		/// <returns></returns>
		RcsModelOverview GetProjectOverview(RcsProjectInfo projectInfo, CancellationToken token);

		/// <summary>
		/// Get section detail
		/// </summary>
		/// <param name="projectInfo">Based on specified sections</param>
		/// <returns>Secton detail model</returns>
		IEnumerable<RcsCrossSectionDetailModel> GetSectionDetails(RcsProjectInfo projectInfo);


		// Section for current implementations using direct RCS class
		/// <summary>
		/// Return calculated sections for given project input
		/// </summary>
		/// <param name="projectInfo">Project information</param>
		/// <param name="token">Token for cancellation</param>
		/// <returns>Collection of calculated sections</returns>
		IEnumerable<SectionConcreteCheckResult> GetResultOnSections(RcsProjectInfo projectInfo, CancellationToken token);

		/// <summary>
		/// Returns nonconformity issues for specified GUID values
		/// </summary>
		/// <param name="projectInfo">Project information</param>
		/// <param name="token">Token for cancellation</param>
		/// <returns>Collection of nonconformity issues</returns>
		IEnumerable<NonConformityIssue> GetNonConformityIssues(RcsProjectInfo projectInfo, CancellationToken token);
	}
}
