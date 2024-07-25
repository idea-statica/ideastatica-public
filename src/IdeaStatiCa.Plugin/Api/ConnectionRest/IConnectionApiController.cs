using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Result;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Settings;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest
{
	public interface IConnectionApiController : IDisposable
	{
		/// <summary>
		/// Open idea project in the service
		/// </summary>
		/// <param name="ideaConProject">Idea Connection project filename</param>
		/// <param name="cancellationToken"></param>
		Task<ConProject> OpenProjectAsync(string ideaConProject, CancellationToken cancellationToken = default);

		/// <summary>
		/// Cloase the active project in the service
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task CloseProjectAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Get project data from the active project
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConProjectData> GetProjectDataAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Gel list of all <see cref="ConConnection"> in the active project
		/// </summary>
		/// <param name="token"></param>
		/// <returns>Connections in the project</returns>
		Task<List<ConConnection>> GetConnectionsAsync(CancellationToken token = default);

		/// <summary>
		/// Geat data fro <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Id of the requested connection in the active project</param>
		/// <param name="token"></param>
		/// <returns></returns>
		Task<ConConnection> GetConnectionAsync(int connectionId, CancellationToken token = default);
		Task<Stream> DownloadProjectAsync(CancellationToken token = default);

		/// <summary>
		/// Updates the data of the connection with <paramref name="connectionId"/> in the active project by <paramref name="connectionUpdate"/>
		/// </summary>
		/// <param name="connectionId">Id of the connection to update</param>
		/// <param name="connectionUpdate">New connection data</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConConnection> UpdateConnectionAsync(int connectionId, ConConnection connectionUpdate, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get list of all operations for the connection with <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Id of the requested connection</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of operations</returns>
		Task<List<ConOperation>> GetOperationsAsync(int connectionId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Run requested type of CBFEM analysis for <paramref name="conToCalculateIds"/>
		/// </summary>
		/// <param name="conToCalculateIds">Lits of connections in the active project to calculate</param>
		/// <param name="analysisType">Type of CBFEM analysis to run</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<ConResultSummary>> CalculateAsync(List<int> conToCalculateIds, ConAnalysisTypeEnum analysisType = ConAnalysisTypeEnum.Stress_Strain, CancellationToken cancellationToken = default);

		Task<string> GetRawResultsAsync(List<int> conToCalculateIds, ConAnalysisTypeEnum analysisType = ConAnalysisTypeEnum.Stress_Strain, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get detailed calculation results for  <paramref name="conToCalculateIds"/>
		/// </summary>
		/// <param name="conToCalculateIds">Lits of connections in the active project</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Detailed results if calculated, otherwise empty</returns>
		Task<List<ConnectionCheckRes>> ResultsAsync(List<int> conToCalculateIds, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get mapping for connection template <paramref name="templateXml"/> on connection with <paramref name="connectionId"/>
		/// in the active project.
		/// </summary>
		/// <param name="connectionId">Id of the connection to apply template</param>
		/// <param name="templateXml">Connection template in xml string</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Mapping of the provided connection on the requested connection</returns>
		Task<TemplateConversions> GetTemplateMappingAsync(int connectionId, string templateXml, CancellationToken cancellationToken = default);

		Task<ConTemplateApplyResult> ApplyConnectionTemplateAsync(int connectionId, string templateXml, TemplateConversions templateMapping, CancellationToken cancellationToken = default);


		/// Creates Idea connection project from given <paramref name="iomContainerXmlFileName"/> and projects
		/// </summary>
		/// <param name="iomXmlFileName">Filename of a given IOM xml file</param>
		Task<ConProject> CreateProjectFromIomContainerFileAsync(string iomContainerXmlFileName, ConIomImportOptions options, CancellationToken cancellationToken = default);

		/// Creates Idea connection project from given <paramref name="iomXmlFileName"/>, <paramref name="iomResXmlFileName"/> and projects
		/// </summary>
		/// <param name="iomXmlFileName">Filename of a given IOM xml file</param>
		/// <param name="iomResXmlFileName">Filename of a given IOM Result xml file</param>
		Task<ConProject> CreateProjectFromIomFileAsync(string iomXmlFileName, string iomResXmlFileName, ConIomImportOptions options, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates an IDEA Connection project based on Open Model and Open Model Result)
		/// </summary>
		/// <param name="model"></param>
		/// <param name="result"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Task<ConProject> CreateProjectFromIomModelAsync(OpenModel model, OpenModelResult result, ConIomImportOptions options, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates an IDEA Connection project based on OpenModelContainer (model and results)
		/// </summary>
		/// <param name="model"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Task<ConProject> CreateProjectFromIomContainerAsync(OpenModelContainer model, ConIomImportOptions options, CancellationToken cancellationToken = default);

		/// <summary>
		/// Export Connection IomContainer
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="version"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<OpenModelContainer> ExportConnectionIomContainer(int connectionId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Export Connection IomConnectionData
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="version"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConnectionData> ExportConnectionIomConnectionData(int connectionId, CancellationToken cancellationToken = default);

		/// Update Idea connection project from given <paramref name="iomContainerXmlFileName"/> and projects
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="iomXmlFileName">Filename of a given IOM xml file</param>
		Task<bool> UpdateProjectFromIomContainerFileAsync(string iomContainerXmlFileName, CancellationToken cancellationToken = default);

		/// Update Idea connection project from given <paramref name="iomXmlFileName"/>, <paramref name="iomResXmlFileName"/> and projects
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="iomXmlFileName">Filename of a given IOM xml file</param>
		/// <param name="iomResXmlFileName">Filename of a given IOM Result xml file</param>
		Task<bool> UpdateProjectFromIomFileAsync(string iomXmlFileName, string iomResXmlFileName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update an IDEA Connection project based on Open Model and Open Model Result)
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="model"></param>
		/// <param name="result"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Task<bool> UpdateProjectFromIomModelAsync(OpenModel model, OpenModelResult result, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update an IDEA Connection project based on OpenModelContainer (model and results)
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="model"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Task<bool> UpdateProjectFromIomContainerAsync(OpenModelContainer model, CancellationToken cancellationToken = default);

		/// <summary>
		/// Export <paramref name="connectionId"/> to IFC
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>Strea of IFC data</returns>
		Task<Stream> ExportToIfcAsync(int connectionId, CancellationToken cancellationToken = default);
		/// <summary>
		/// Get all members in connection
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<ConMember>> GetMembersAsync(int connectionId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get one member in connection
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConMember> GetMemberAsync(int connectionId, int memberId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update connection's member values
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="memberId"></param>
		/// <param name="member"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConMember> UpdateMemberAsync(int connectionId, int memberId, ConMember member, CancellationToken cancellationToken = default);
		
		/// <summary>
		/// Get production cost
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConProductionCost> GetProductionCostAsync(int connectionId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get load effects for connection
		/// </summary>
		/// <param name="connectionId">Connection Id</param>
		/// <param name="none"></param>
		/// <returns></returns>
		Task<List<ConLoadEffect>> GetLoadEffectsAsync(int connectionId, bool isPercentage = false, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get connection's load effect specified by id
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="loadEffectId"></param>
		/// <param name="isPercentage"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConLoadEffect> GetLoadEffectAsync(int connectionId, int loadEffectId, bool isPercentage = false, CancellationToken cancellationToken = default);

		/// <summary>
		/// Add Load effect for connection
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="newLe"></param>
		/// <param name="none"></param>
		/// <returns></returns>
		Task<ConLoadEffect> AddLoadEffectAsync(int connectionId, ConLoadEffect newLe, CancellationToken none);

		/// <summary>
		/// Delete load effect
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="loadEffectId"></param>
		/// <returns></returns>
		Task DeleteLoadEffectAsync(int connectionId, int loadEffectId);

		/// <summary>
		/// Update existing load effect
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="le1"></param>
		/// <returns></returns>
		Task<ConLoadEffect> UpdateLoadEffectAsync(int connectionId, ConLoadEffect le1);

		/// <summary>
		/// Get the connection setup from the active project
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConnectionSetup> GetConnectionSetupAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Update the connection setup in the active project by <paramref name="connectionSetup"/>
		/// </summary>
		/// <param name="connectionSetup"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConnectionSetup> UpdateConnectionSetupAsync(ConnectionSetup connectionSetup, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the materials in project
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="type">Concrete, Steel, Reinforcement, Prestress, BoltGrade, WeldingMaterial, Timber, All  </param>
		/// <returns></returns>
		Task<List<ProjMaterial>> GetMaterialsAsync(int connectionId, string type = "All");

		/// <summary>
		/// Get project cross sections
		/// </summary>
		/// <param name="connectionId"></param>
		/// <returns></returns>
		Task<List<ProjCrossSection>> GetCrossSectionsAsync(int connectionId);

		/// <summary>
		/// Get project pins
		/// </summary>
		/// <param name="connectionId"></param>
		/// <returns></returns>
		Task<List<ProjPin>> GetPinsAsync(int connectionId);

		/// <summary>
		/// Get bolt assemblies from project
		/// </summary>
		/// <param name="connectionId"></param>
		/// <returns></returns>
		Task<List<ProjBoltAssembly>> GetBoltAssembliesAsync(int connectionId);

		/// <summary>
		/// Add pin to project data
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="newPin"></param>
		/// <returns></returns>
		Task<ProjPin> AddPinAsync(int connectionId, ProjPin newPin);

		/// <summary>
		/// Add material to project data
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="newMaterial"></param>
		/// <returns></returns>
		Task<ProjMaterial> AddMaterialAsync(int connectionId, ProjMaterial newMaterial);

		/// <summary>
		/// Add cross section to project data
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="newCrossSection"></param>
		/// <returns></returns>
		Task<ProjCrossSection> AddCrossSectionAsync(int connectionId, ProjCrossSection newCrossSection);

		/// <summary>
		/// Add bolt assembly to project data
		/// </summary>
		/// <param name="id"></param>
		/// <param name="newBa"></param>
		/// <returns></returns>
		Task<ProjBoltAssembly> AddBoltAssemblyAsync(int id, ProjBoltAssembly newBa);
	}
}
