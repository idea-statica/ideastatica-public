using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	/// <summary>
	/// Provides an abstraction over the Connection API client, encapsulating
	/// connection lifecycle management, project operations, calculations,
	/// template handling, and export functionality.
	/// </summary>
	public interface IConnectionApiService : IDisposable
	{
		/// <summary>
		/// Gets a value indicating whether the service is currently connected to the API.
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Gets the base URL of the connected API service, or <see langword="null"/> if not connected.
		/// </summary>
		string? ServiceUrl { get; }

		/// <summary>
		/// Gets the unique client identifier assigned by the API service, or <see langword="null"/> if not connected.
		/// </summary>
		string? ClientId { get; }

		/// <summary>
		/// Provides direct access to the underlying <see cref="IConnectionApiClient"/> for UI services
		/// that need it (e.g., <see cref="ConnectionLibraryProposer"/>, <see cref="ExpressionProvider"/>).
		/// Returns <see langword="null"/> if not connected.
		/// </summary>
		IConnectionApiClient? Client { get; }

		/// <summary>
		/// Connects to the Connection API service by either starting a new server process or attaching to an existing endpoint.
		/// </summary>
		/// <param name="runServer">If <see langword="true"/>, starts a new API server process using the specified setup path;
		/// otherwise, attaches to an existing endpoint.</param>
		/// <param name="setupPath">The path to the IDEA StatiCa installation directory. Used only when <paramref name="runServer"/> is <see langword="true"/>.</param>
		/// <param name="endpoint">The URL of an existing API endpoint. Used only when <paramref name="runServer"/> is <see langword="false"/>.</param>
		/// <exception cref="InvalidOperationException">Thrown when already connected or when required parameters are missing.</exception>
		Task ConnectAsync(bool runServer, string? setupPath, string? endpoint);

		/// <summary>
		/// Disconnects from the API service and releases all associated resources.
		/// </summary>
		Task DisconnectAsync();

		/// <summary>
		/// Opens an existing IDEA Connection project file (.ideacon).
		/// </summary>
		/// <param name="filePath">The path to the .ideacon project file.</param>
		/// <returns>The project information including connections and metadata.</returns>
		Task<ConProject> OpenProjectAsync(string filePath);

		/// <summary>
		/// Creates a new project by importing an IOM (IDEA Open Model) file.
		/// </summary>
		/// <param name="filePath">The path to the .iom or .xml file.</param>
		/// <returns>The project information for the newly created project.</returns>
		Task<ConProject> ImportIomFileAsync(string filePath);

		/// <summary>
		/// Creates a new project by importing IOM data from a stream.
		/// </summary>
		/// <param name="stream">The stream containing the IOM XML data.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>The project information for the newly created project.</returns>
		Task<ConProject> ImportIomStreamAsync(Stream stream, CancellationToken ct);

		/// <summary>
		/// Closes an open project and releases its resources on the server.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project to close.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		Task CloseProjectAsync(Guid projectId, CancellationToken ct);

		/// <summary>
		/// Saves the project to a file on disk.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project to save.</param>
		/// <param name="filePath">The destination file path for the saved project.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		Task SaveProjectAsync(Guid projectId, string filePath, CancellationToken ct);

		/// <summary>
		/// Runs a structural analysis calculation on the specified connection.
		/// Updates the connection's analysis settings if they differ from the provided values.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection to calculate.</param>
		/// <param name="analysisType">The type of analysis to perform (e.g., Stress_Strain, Stiffness).</param>
		/// <param name="includeBuckling">Whether to include buckling analysis.</param>
		/// <param name="getRawResults">Whether to include raw XML results in the output.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A formatted JSON string containing the calculation results and optionally raw XML results.</returns>
		Task<string> CalculateAsync(Guid projectId, int connectionId,
			ConAnalysisTypeEnum analysisType, bool includeBuckling,
			bool getRawResults, CancellationToken ct);

		/// <summary>
		/// Gets the members (beams/columns) of a connection as a formatted JSON string.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A formatted JSON string of the members, or "No members" if none exist.</returns>
		Task<string> GetMembersJsonAsync(Guid projectId, int connectionId, CancellationToken ct);

		/// <summary>
		/// Gets the manufacturing operations of a connection as a formatted JSON string.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A formatted JSON string of the operations, or "No operations" if none exist.</returns>
		Task<string> GetOperationsJsonAsync(Guid projectId, int connectionId, CancellationToken ct);

		/// <summary>
		/// Deletes all manufacturing operations from a connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		Task DeleteOperationsAsync(Guid projectId, int connectionId, CancellationToken ct);

		/// <summary>
		/// Gets the topology (geometry and connectivity) of a connection, including the topology code.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A string containing the topology code and full topology JSON data.</returns>
		Task<string> GetTopologyJsonAsync(Guid projectId, int connectionId, CancellationToken ct);

		/// <summary>
		/// Gets the 3D scene data for a connection as a JSON string.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <returns>A JSON string representing the 3D scene data.</returns>
		Task<string> GetSceneDataJsonAsync(Guid projectId, int connectionId);

		/// <summary>
		/// Gets the 3D scene text representation for a connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A text representation of the 3D scene.</returns>
		Task<string> GetScene3DTextAsync(Guid projectId, int connectionId, CancellationToken ct);

		/// <summary>
		/// Creates a connection template XML from the specified connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection to create the template from.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>The connection template as an XML string.</returns>
		Task<string> CreateTemplateXmlAsync(Guid projectId, int connectionId, CancellationToken ct);

		/// <summary>
		/// Gets the default template mapping for applying a template to a connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the target connection.</param>
		/// <param name="param">Parameters specifying the template and optional member IDs.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>The default template conversions (member and cross-section mappings).</returns>
		Task<TemplateConversions> GetDefaultTemplateMappingAsync(Guid projectId, int connectionId,
			ConTemplateMappingGetParam param, CancellationToken ct);

		/// <summary>
		/// Applies a connection template to the specified connection with the given mapping.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the target connection.</param>
		/// <param name="param">The template content and mapping to apply.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A status message indicating the template was applied.</returns>
		Task<string> ApplyTemplateAsync(Guid projectId, int connectionId,
			ConTemplateApplyParam param, CancellationToken ct);

		/// <summary>
		/// Performs weld pre-design (sizing) on the specified connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="method">The weld sizing method to use (e.g., FullStrength).</param>
		/// <returns>The result of the weld sizing operation.</returns>
		Task<string> WeldSizingAsync(Guid projectId, int connectionId,
			IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum method);

		/// <summary>
		/// Gets the project settings as a formatted JSON string.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A formatted JSON string of the project settings.</returns>
		Task<string> GetSettingsJsonAsync(Guid projectId, CancellationToken ct);

		/// <summary>
		/// Updates the project settings from a JSON string.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="settingsJson">A JSON string containing the settings to update.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A formatted JSON string of the updated settings.</returns>
		Task<string> UpdateSettingsAsync(Guid projectId, string settingsJson, CancellationToken ct);

		/// <summary>
		/// Gets the load effects (forces and moments) for a connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A list of load effects defined on the connection.</returns>
		Task<List<ConLoadEffect>> GetLoadEffectsAsync(Guid projectId, int connectionId, CancellationToken ct);

		/// <summary>
		/// Updates the load effects for a connection by updating each load effect individually.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="loadEffects">The list of updated load effects.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		Task UpdateLoadEffectsAsync(Guid projectId, int connectionId,
			List<ConLoadEffect> loadEffects, CancellationToken ct);

		/// <summary>
		/// Evaluates a parametric expression in the context of a connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection providing the evaluation context.</param>
		/// <param name="expression">The expression to evaluate (e.g., "GetValue('B1', 'CrossSection.Bounds.Height')").</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>The result of the expression evaluation as a string.</returns>
		Task<string> EvaluateExpressionAsync(Guid projectId, int connectionId,
			string expression, CancellationToken ct);

		/// <summary>
		/// Gets the user-defined parameters of a connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		/// <returns>A list of parameters defined on the connection.</returns>
		Task<List<IdeaParameter>> GetParametersAsync(Guid projectId, int connectionId, CancellationToken ct);

		/// <summary>
		/// Updates the user-defined parameters of a connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="parameters">The list of parameter updates to apply.</param>
		/// <param name="ct">A cancellation token to cancel the operation.</param>
		Task UpdateParametersAsync(Guid projectId, int connectionId,
			List<IdeaParameterUpdate> parameters, CancellationToken ct);

		/// <summary>
		/// Exports the connection as an IOM (IDEA Open Model) XML string.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection to export.</param>
		/// <returns>The IOM XML content as a string.</returns>
		Task<string> ExportIomAsync(Guid projectId, int connectionId);

		/// <summary>
		/// Exports the connection as an IFC file.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection to export.</param>
		/// <param name="filePath">The destination file path for the IFC export.</param>
		Task ExportIfcAsync(Guid projectId, int connectionId, string filePath);

		/// <summary>
		/// Generates and saves a calculation report as a PDF file.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="filePath">The destination file path for the PDF report.</param>
		Task SaveReportPdfAsync(Guid projectId, int connectionId, string filePath);

		/// <summary>
		/// Generates and saves a calculation report as a Word document.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="filePath">The destination file path for the Word report.</param>
		Task SaveReportWordAsync(Guid projectId, int connectionId, string filePath);
	}
}
