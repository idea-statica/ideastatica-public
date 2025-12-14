using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.Api.RCS.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IdeaStatiCa.Api.RCS
{
	public interface IRcsController : IAsyncDisposable
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
		/// <param name="modifiedSectionData">Data to set</param>
		/// <returns>Data of the modified section</returns>
		RcsSection UpdateSection(RcsSection modifiedSectionData);

		/// <summary>
		/// Import reinforced cross-section from template
		/// </summary>
		/// <param name="importSetting">Import settings</param>
		/// <param name="reinfCssTemplate">Template content</param>
		/// <returns>Created reinforced cross-section info</returns>
		RcsReinforcedCrossSection ImportReinforcedCrossSection(RcsReinforcedCrosssSectionImportSetting importSetting, string reinfCssTemplate);

		/// <summary>
		/// Get loading in <paramref name="sectionId"/> in XML format
		/// </summary>
		/// <param name="sectionId">Section ID</param>
		/// <returns>XML string which represent loading in a section</returns>
		string GetLoadingInSectionXML(int sectionId);

		/// <summary>
		/// Set loading from <paramref name="loadingXML"/> to section <paramref name="sectionId"/>
		/// </summary>
		/// <param name="sectionId">Id of a section to update</param>
		/// <param name="loadingXML">New loading data</param>
		void SetLoadingInSectionXML(int sectionId, string loadingXML);

		/// <summary>
		/// Add a new reinforced cross-section from embedded geometry data
		/// </summary>
		/// <param name="reinforcedCrossSection">Reinforced cross-section definition with geometry</param>
		/// <returns>Created reinforced cross-section info</returns>
		RcsReinforcedCrossSection AddReinforcedCrossSection(ReinforcedCrossSectionData reinforcedCrossSection);

		/// <summary>
		/// Get reinforced cross-section in IOM format with full geometry
		/// </summary>
		/// <param name="reinforcedCssSectionId">Reinforced cross-section ID</param>
		/// <returns>IOM ReinforcedCrossSection</returns>
		ReinforcedCrossSection GetReinforcedCrossSectionIOM(int reinforcedCssSectionId);

		/// <summary>
		/// Get materials from the project
		/// </summary>
		/// <param name="materialType">Type of material to get, or null for all materials</param>
		/// <returns>List of materials</returns>
		IEnumerable<RcsMaterial> GetMaterials(RcsMaterialType? materialType = null);

		/// <summary>
		/// Add material to project by MPRL name
		/// </summary>
		/// <param name="materialType">Type of material</param>
		/// <param name="mprlName">MPRL material name</param>
		/// <returns>Created material info</returns>
		RcsMaterial AddMaterial(RcsMaterialType materialType, string mprlName);
	}
}
