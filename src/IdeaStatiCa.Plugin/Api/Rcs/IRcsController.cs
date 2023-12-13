using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IdeaStatiCa.Plugin.Api.Rcs
{
	public interface IRcsController : IDisposable
	{
		bool OpenIdeaProject(string ideaProjectPath);
		bool OpenIdeaProjectFromIdeaOpenModel(string ideaOpenModelProjectPath, string projectName, string ideaOpenMessagesPath);
		bool OpenIdeaProjectFromIdeaOpenModel(OpenModel ideaOpenModel, string projectName, out OpenMessages messages);

		int[] GetProjectSections();
		RcsProjectModel GetProjectOverview(RcsProjectEnum projectEnum);
		RcsCrossSectionDetailModel GetCrossSectionModel(int sectionId);


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
