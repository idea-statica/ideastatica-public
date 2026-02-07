using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	public interface IConnectionApiService : IDisposable
	{
		// Connection state
		bool IsConnected { get; }
		string? ServiceUrl { get; }
		string? ClientId { get; }

		/// <summary>
		/// Provides direct access to the API client for UI services that need it
		/// (e.g. ConnectionLibraryProposer, ExpressionProvider).
		/// </summary>
		IConnectionApiClient? Client { get; }

		// Lifecycle
		Task ConnectAsync(bool runServer, string? setupPath, string? endpoint);
		Task DisconnectAsync();

		// Project
		Task<ConProject> OpenProjectAsync(string filePath);
		Task<ConProject> ImportIomFileAsync(string filePath);
		Task<ConProject> ImportIomStreamAsync(Stream stream, CancellationToken ct);
		Task CloseProjectAsync(Guid projectId, CancellationToken ct);
		Task SaveProjectAsync(Guid projectId, string filePath, CancellationToken ct);

		// Calculation
		Task<string> CalculateAsync(Guid projectId, int connectionId,
			ConAnalysisTypeEnum analysisType, bool includeBuckling,
			bool getRawResults, CancellationToken ct);

		// Members & Operations
		Task<string> GetMembersJsonAsync(Guid projectId, int connectionId, CancellationToken ct);
		Task<string> GetOperationsJsonAsync(Guid projectId, int connectionId, CancellationToken ct);
		Task DeleteOperationsAsync(Guid projectId, int connectionId, CancellationToken ct);

		// Topology & Scene
		Task<string> GetTopologyJsonAsync(Guid projectId, int connectionId, CancellationToken ct);
		Task<string> GetSceneDataJsonAsync(Guid projectId, int connectionId);
		Task<string> GetScene3DTextAsync(Guid projectId, int connectionId, CancellationToken ct);

		// Templates
		Task<string> CreateTemplateXmlAsync(Guid projectId, int connectionId, CancellationToken ct);
		Task<TemplateConversions> GetDefaultTemplateMappingAsync(Guid projectId, int connectionId,
			ConTemplateMappingGetParam param, CancellationToken ct);
		Task<string> ApplyTemplateAsync(Guid projectId, int connectionId,
			ConTemplateApplyParam param, CancellationToken ct);

		// Weld Sizing
		Task<string> WeldSizingAsync(Guid projectId, int connectionId,
			IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum method);

		// Settings
		Task<string> GetSettingsJsonAsync(Guid projectId, CancellationToken ct);
		Task<string> UpdateSettingsAsync(Guid projectId, string settingsJson, CancellationToken ct);

		// Load Effects
		Task<List<ConLoadEffect>> GetLoadEffectsAsync(Guid projectId, int connectionId, CancellationToken ct);
		Task UpdateLoadEffectsAsync(Guid projectId, int connectionId,
			List<ConLoadEffect> loadEffects, CancellationToken ct);

		// Expression & Parameters
		Task<string> EvaluateExpressionAsync(Guid projectId, int connectionId,
			string expression, CancellationToken ct);
		Task<List<IdeaParameter>> GetParametersAsync(Guid projectId, int connectionId, CancellationToken ct);
		Task UpdateParametersAsync(Guid projectId, int connectionId,
			List<IdeaParameterUpdate> parameters, CancellationToken ct);

		// Export & Report
		Task<string> ExportIomAsync(Guid projectId, int connectionId);
		Task ExportIfcAsync(Guid projectId, int connectionId, string filePath);
		Task SaveReportPdfAsync(Guid projectId, int connectionId, string filePath);
		Task SaveReportWordAsync(Guid projectId, int connectionId, string filePath);
	}
}
