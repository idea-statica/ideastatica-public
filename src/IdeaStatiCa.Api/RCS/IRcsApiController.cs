using IdeaRS.OpenModel;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.Api.RCS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Api.RCS
{
	/// <summary>
	/// Defines the contract for controlling and interacting with an RCS (Reinforced Concrete Structure) project, including
	/// project management, calculation, data retrieval, and settings operations.
	/// </summary>
	/// <remarks>This interface provides asynchronous methods for opening, creating, calculating, and managing RCS
	/// projects and their associated data. Implementations are expected to manage the lifecycle of a project and support
	/// cancellation via provided tokens. The interface inherits from <see cref="IDisposable"/>, indicating that resources
	/// should be released when the controller is no longer needed.</remarks>
	public interface IRcsApiController : IDisposable
	{
		/// <summary>
		/// Identifier of the active RCS project on the backend
		/// </summary>
		Guid ActiveProjectId {  get; }

		#region Project Management

		/// <summary>
		/// Open project from IdeaRcs file
		/// </summary>
		/// <param name="rscFilePath">Local path of the IdeaRcs file</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<bool> OpenProjectAsync(string rscFilePath, CancellationToken token = default);

		Task CloseProjectAsync(CancellationToken token = default);

		/// <summary>
		/// Create project from Open Model object
		/// </summary>
		/// <param name="model">Project in open model format</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<bool> CreateProjectFromIOMAsync(OpenModel model, CancellationToken token = default);

		/// <summary>
		/// Create project from Open Model file
		/// </summary>
		/// <param name="iomFilePath">Local path of the XML file</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<bool> CreateProjectFromIOMFileAsync(string iomFilePath, CancellationToken token = default);

		/// <summary>
		/// Return open project as file stream (*.idearcs)
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<Stream> DownloadProjectAsync(CancellationToken token = default);

		/// <summary>
		/// Saves the loaded project as .IdeaRcs file on disk
		/// </summary>
		/// <param name="outputPath"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		Task SaveProjectAsync(string outputPath, CancellationToken token = default);

		#endregion

		#region Calculation

		/// <summary>
		/// Calculates RCS project
		/// When no sections are specified, everything is calculated
		/// </summary>
		/// <param name="parameters">Parameters to specify sections</param>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<List<RcsSectionResultOverview>> CalculateAsync(RcsCalculationParameters parameters, CancellationToken token = default);

		/// <summary>
		/// Get calculated results for given project id
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		Task<List<RcsSectionResultDetailed>> GetResultsAsync(RcsResultParameters parameters, CancellationToken token = default);

		#endregion

		#region Project Data

		/// <summary>
		/// Get information summary about the Project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<RcsProjectSummary> GetProjectSummaryAsync(CancellationToken token = default);

		/// <summary>
		/// Get project data
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<RcsProjectData> GetProjectDataAsync(CancellationToken token = default);

		/// <summary>
		/// Get information about sections in Project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<List<RcsSection>> GetProjectSectionsAsync(CancellationToken token = default);

		/// <summary>
		/// Get information about members in Project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<List<RcsCheckMember>> GetProjectMembersAsync(CancellationToken token = default);

		/// <summary>
		/// Get information about reinforced cross sections in Project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns></returns>
		Task<List<RcsReinforcedCrossSection>> GetProjectReinforcedCrossSectionsAsync(CancellationToken token = default);

		#endregion

		#region Settings

		/// <summary>
		/// Get the code settings
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		Task<string> GetCodeSettings(CancellationToken token = default);

		/// <summary>
		/// Update the code settings
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		Task<bool> UpdateCodeSettings(List<RcsSetting> setup, CancellationToken token = default);

		#endregion

		#region Sections

		/// <summary>
		/// Update data of the section in the RCS project.
		/// The section to modify is defined by property ID passed in <paramref name="newSectionData"/>
		/// </summary>
		/// <param name="newSectionData">New data of the section. A valid value of the section Id is requiered.</param>
		/// <param name="token">Cancellation token</param>
		/// <returns>Chanded data</returns>
		Task<RcsSection> UpdateSectionAsync(RcsSection newSectionData, CancellationToken token = default);

		#endregion

		#region Cross-Sections

		/// <summary>
		/// Import reinforced cross-section from template
		/// </summary>
		/// <param name="importSetting">Import settings</param>
		/// <param name="reinfCssTemplate">Template content</param>
		/// <param name="token">Cancellation token</param>
		/// <returns>Information about the imported reinforced cross-section</returns>
		Task<RcsReinforcedCrossSection> ImportReinforcedCrossSectionAsync(RcsReinforcedCrosssSectionImportSetting importSetting, string reinfCssTemplate, CancellationToken token = default);

		/// <summary>
		/// Add a new reinforced cross-section to the project from embedded geometry
		/// </summary>
		/// <param name="reinforcedCrossSection">Reinforced cross-section data with embedded geometry</param>
		/// <param name="token">Cancellation token</param>
		/// <returns>Information about the created reinforced cross-section</returns>
		Task<RcsReinforcedCrossSection> AddReinforcedCrossSectionAsync(ReinforcedCrossSectionData reinforcedCrossSection, CancellationToken token = default);

		/// <summary>
		/// Get reinforced cross-section with full geometry (IOM format)
		/// </summary>
		/// <param name="reinforcedCssSectionId">Reinforced cross-section ID</param>
		/// <param name="token">Cancellation token</param>
		/// <returns>Reinforced cross-section in IOM format with full geometry</returns>
		Task<ReinforcedCrossSection> GetReinforcedCrossSectionDataAsync(int reinforcedCssSectionId, CancellationToken token = default);

		#endregion

		#region Internal Forces

		/// <summary>
		/// Get loading in <paramref name="sectionId"/>
		/// </summary>
		/// <param name="sectionId">Id of a rcs section to get loading</param>
		/// <param name="token">Cancellation token</param>
		/// <returns>XML string which represents loading in <paramref name="sectionId"/></returns>
		Task<string> GetLoadingInSectionAsync(int sectionId, CancellationToken token = default);

		/// <summary>
		/// Set loading to <paramref name="sectionId"/>
		/// </summary>
		/// <param name="sectionId">Id of a rcs section to set loading</param>
		/// <param name="loadingXml">xml representation of loading in a section (list of extremes)</param>
		/// <param name="token">Cancellation token</param>
		Task SetLoadingInSectionAsync(int sectionId, string loadingXml, CancellationToken token = default);

		#endregion

		#region Materials

		/// <summary>
		/// Get all concrete materials from the project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns>List of concrete materials</returns>
		Task<List<object>> GetConcreteMaterialsAsync(CancellationToken token = default);

		/// <summary>
		/// Get all reinforcement materials from the project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns>List of reinforcement materials</returns>
		Task<List<object>> GetReinforcementMaterialsAsync(CancellationToken token = default);

		/// <summary>
		/// Get all prestress materials from the project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns>List of prestress materials</returns>
		Task<List<object>> GetPrestressMaterialsAsync(CancellationToken token = default);

		/// <summary>
		/// Get all materials from the project
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns>List of all materials</returns>
		Task<List<object>> GetAllMaterialsAsync(CancellationToken token = default);

		/// <summary>
		/// Add a new concrete material to the project
		/// </summary>
		/// <param name="mprlName">Material MPRL name</param>
		/// <param name="token">Cancellation token</param>
		/// <returns>The added material</returns>
		Task<object> AddConcreteMaterialAsync(string mprlName, CancellationToken token = default);

		/// <summary>
		/// Add a new reinforcement material to the project
		/// </summary>
		/// <param name="mprlName">Material MPRL name</param>
		/// <param name="token">Cancellation token</param>
		/// <returns>The added material</returns>
		Task<object> AddReinforcementMaterialAsync(string mprlName, CancellationToken token = default);

		/// <summary>
		/// Add a new prestress material to the project
		/// </summary>
		/// <param name="mprlName">Material MPRL name</param>
		/// <param name="token">Cancellation token</param>
		/// <returns>The added material</returns>
		Task<object> AddPrestressMaterialAsync(string mprlName, CancellationToken token = default);

		#endregion
	}
}
