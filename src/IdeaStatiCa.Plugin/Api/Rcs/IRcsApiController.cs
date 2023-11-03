using System;
using System.Collections.Generic;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;

namespace IdeaStatiCa.Plugin.Api.Rcs
{
	public interface IRcsApiController : IDisposable
	{
		/// <summary>
		/// Calculation of sections for given RcsProjectInfo
		/// When no sections are specified, everything is calculated
		/// </summary>
		/// <param name="projectInfo"></param>
		/// <returns></returns>
		IEnumerable<SectionConcreteCheckResult> GetResultOnSections(RcsProjectInfo projectInfo);

		/// <summary>
		/// Get NonConformity issues for given RcsProjectInfo
		/// When no non-conformities are specified, nothing is returned
		/// </summary>
		/// <param name="projectInfo"></param>
		/// <returns></returns>
		IEnumerable<NonConformityIssue> GetNonConformityIssues(RcsProjectInfo projectInfo);
	}
}
