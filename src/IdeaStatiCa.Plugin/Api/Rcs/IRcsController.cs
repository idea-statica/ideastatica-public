using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaRS.OpenModel.Concrete.CheckResult;
using System.Threading;
using System.Collections.Generic;
using System;

namespace IdeaStatiCa.Plugin.Api.Rcs
{
	public interface IRcsController : IDisposable
	{
		bool OpenIdeaProject(string ideaProjectPath);
		bool OpenIdeaProjectFromIdeaOpenModel(string ideaOpenModelProjectPath, string projectName, string ideaOpenMessagesPath);
		bool OpenIdeaProjectFromIdeaOpenModel(OpenModel ideaOpenModel, string projectName, out OpenMessages messages);

		RcsProjectSummaryModel GetProjectSummary(RcsProjectEnum projectEnum);
		RcsProjectData GetProjectData();

		object GetSettings();
		object SetSettings(List<RcsSettingModel> changes);

		void SaveAsIdeaProjectFile(string ideaProjectPath);

		bool Calculate(IEnumerable<int> sections);
		IEnumerable<SectionConcreteCheckResult> GetResultOnSections(CancellationToken cancellationToken, params int[] sections);
		IEnumerable<NonConformityIssue> GetNonConformityIssues(CancellationToken cancellationToken, params Guid[] issues);

		/// <summary>
		/// Update data of a RCS section
		/// </summary>
		/// <param name="modifiedData">The section </param>
		/// <returns></returns>
		RcsSectionModel UpdateSection(RcsSectionModel modifiedSectionData);
	}
}
