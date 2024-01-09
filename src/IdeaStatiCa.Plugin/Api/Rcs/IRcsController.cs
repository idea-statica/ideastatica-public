using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaRS.OpenModel.Concrete.CheckResult;
using System.Threading;
using System.Collections.Generic;
using System;

namespace IdeaStatiCa.Plugin.Api.RCS
{
	public interface IRcsController : IDisposable
	{
		bool OpenIdeaProject(string ideaProjectPath);
		bool OpenIdeaProjectFromIdeaOpenModel(string ideaOpenModelProjectPath, string projectName, string ideaOpenMessagesPath);
		bool OpenIdeaProjectFromIdeaOpenModel(OpenModel ideaOpenModel, string projectName, out OpenMessages messages);

		RcsProjectSummary GetProjectSummary(RcsProjectEnum projectEnum);
		RcsProjectData GetProjectData();

		object GetSettings();
		object SetSettings(List<RcsSetting> changes);

		void SaveAsIdeaProjectFile(string ideaProjectPath);

		bool Calculate(IEnumerable<int> sections);
		IEnumerable<SectionConcreteCheckResult> GetResultOnSections(CancellationToken cancellationToken, params int[] sections);
		IEnumerable<NonConformityIssue> GetNonConformityIssues(CancellationToken cancellationToken, params Guid[] issues);

		/// <summary>
		/// Update data of a RCS section in the RCS project. The section is taken according to pass value in <paramref name="modifiedSectionData"/>
		/// <see cref="RcsSection.Id"/>
		/// </summary>
		/// <param name="modifiedData">Data to set</param>
		/// <returns>Data of the modified section</returns>
		RcsSection UpdateSection(RcsSection modifiedSectionData);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="importSetting"></param>
		/// <param name="reinfCssTemplate"></param>
		/// <returns></returns>
		RcsReinforcedCrossSection ImportReinforcedCrossSection(RcsReinforcedCrosssSectionImportSetting importSetting, string reinfCssTemplate);
	}
}
