using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	/// <summary>
	/// Default implementation of <see cref="IConnectionApiService"/> that manages the lifecycle of
	/// the Connection API client and delegates business logic to the underlying API endpoints.
	/// </summary>
	public class ConnectionApiService : IConnectionApiService
	{
		private readonly IPluginLogger _logger;
		private IApiServiceFactory<IConnectionApiClient>? _clientFactory;
		private IConnectionApiClient? _client;
		private bool _disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionApiService"/> class.
		/// </summary>
		/// <param name="logger">The logger used for diagnostic output.</param>
		public ConnectionApiService(IPluginLogger logger)
		{
			_logger = logger;
		}

		/// <inheritdoc/>
		public bool IsConnected => _client != null;

		/// <inheritdoc/>
		public string? ServiceUrl => _client?.ClientApi?.Configuration?.BasePath;

		/// <inheritdoc/>
		public string? ClientId => _client?.ClientId;

		/// <inheritdoc/>
		public IConnectionApiClient? Client => _client;

		/// <inheritdoc/>
		public async Task ConnectAsync(bool runServer, string? setupPath, string? endpoint)
		{
			_logger.LogInformation("ConnectionApiService.ConnectAsync");

			if (_client != null)
			{
				throw new InvalidOperationException("Already connected to the API service");
			}

			if (runServer)
			{
				_clientFactory = new ConnectionApiServiceRunner(setupPath);
				_client = await _clientFactory.CreateApiClient();
			}
			else
			{
				if (string.IsNullOrEmpty(endpoint))
				{
					throw new InvalidOperationException("API endpoint is not set");
				}

				_clientFactory = new ConnectionApiServiceAttacher(endpoint);
				_client = await _clientFactory.CreateApiClient();
			}

			if (_client == null)
			{
				throw new InvalidOperationException("Failed to create API client");
			}
		}

		/// <inheritdoc/>
		public Task DisconnectAsync()
		{
			return Task.Run(() =>
			{
				if (_client != null)
				{
					_client.Dispose();
					_client = null;
				}

				if (_clientFactory is IDisposable disposable)
				{
					disposable.Dispose();
					_clientFactory = null;
				}
			});
		}

		/// <inheritdoc/>
		public async Task<ConProject> OpenProjectAsync(string filePath)
		{
			EnsureConnected();
			return await _client!.Project.OpenProjectAsync(filePath);
		}

		/// <inheritdoc/>
		public async Task<ConProject> ImportIomFileAsync(string filePath)
		{
			EnsureConnected();
			return await _client!.Project.CreateProjectFromIomFileAsync(filePath);
		}

		/// <inheritdoc/>
		public async Task<ConProject> ImportIomStreamAsync(Stream stream, CancellationToken ct)
		{
			EnsureConnected();
			var response = await _client!.Project.ImportIOMWithHttpInfoAsync(
				containerXmlFile: stream,
				connectionsToCreate: null,
				cancellationToken: ct);
			return response.Data;
		}

		/// <inheritdoc/>
		public async Task CloseProjectAsync(Guid projectId, CancellationToken ct)
		{
			EnsureConnected();
			await _client!.Project.CloseProjectAsync(projectId, 0, ct);
		}

		/// <inheritdoc/>
		public async Task SaveProjectAsync(Guid projectId, string filePath, CancellationToken ct)
		{
			EnsureConnected();
			await _client!.Project.SaveProjectAsync(projectId, filePath, ct);
		}

		/// <inheritdoc/>
		public async Task<string> CalculateAsync(Guid projectId, int connectionId,
			ConAnalysisTypeEnum analysisType, bool includeBuckling,
			bool getRawResults, CancellationToken ct)
		{
			EnsureConnected();

			var connectionIdList = new List<int> { connectionId };

			var selectedConData = await _client!.Connection.GetConnectionAsync(projectId, connectionId, 0, ct);
			if (selectedConData.AnalysisType != analysisType ||
				selectedConData.IncludeBuckling != includeBuckling)
			{
				selectedConData.AnalysisType = analysisType;
				selectedConData.IncludeBuckling = includeBuckling;
				await _client.Connection.UpdateConnectionAsync(projectId, connectionId, selectedConData, 0, ct);
			}

			var calculationResults = await _client.Calculation.CalculateAsync(projectId, connectionIdList, 0, ct);

			string rawResultsXml = string.Empty;
			if (getRawResults)
			{
				var rawResults = await _client.Calculation.GetRawJsonResultsAsync(projectId, connectionIdList, 0, ct);
				rawResultsXml = rawResults!.Any() ? rawResults[0] : string.Empty;
			}

			return $"{Tools.JsonTools.ToFormatedJson(calculationResults)}\n\n{rawResultsXml}";
		}

		/// <inheritdoc/>
		public async Task<string> GetMembersJsonAsync(Guid projectId, int connectionId, CancellationToken ct)
		{
			EnsureConnected();
			var members = await _client!.Member.GetMembersAsync(projectId, connectionId, 0, ct);
			return members == null ? "No members" : Tools.JsonTools.ToFormatedJson(members);
		}

		/// <inheritdoc/>
		public async Task<string> GetOperationsJsonAsync(Guid projectId, int connectionId, CancellationToken ct)
		{
			EnsureConnected();
			var operations = await _client!.Operation.GetOperationsAsync(projectId, connectionId, 0, ct);
			return operations == null ? "No operations" : Tools.JsonTools.ToFormatedJson(operations);
		}

		/// <inheritdoc/>
		public async Task DeleteOperationsAsync(Guid projectId, int connectionId, CancellationToken ct)
		{
			EnsureConnected();
			await _client!.Operation.DeleteOperationsAsync(projectId, connectionId, 0, ct);
		}

		/// <inheritdoc/>
		public async Task<string> GetTopologyJsonAsync(Guid projectId, int connectionId, CancellationToken ct)
		{
			EnsureConnected();
			var topologyJsonString = await _client!.Connection.GetConnectionTopologyAsync(projectId, connectionId, 0, ct);

			if (string.IsNullOrEmpty(topologyJsonString))
			{
				return topologyJsonString ?? string.Empty;
			}

			dynamic? typology = JsonConvert.DeserializeObject(topologyJsonString);
			if (typology != null)
			{
				var topologyCode = typology["typologyCode_V2"];
				return $"typologyCode_V2 = '{topologyCode}'\n\nConnection topology :\n{topologyJsonString}";
			}

			return "Error";
		}

		/// <inheritdoc/>
		public async Task<string> GetSceneDataJsonAsync(Guid projectId, int connectionId)
		{
			EnsureConnected();
			return await _client!.Presentation.GetDataScene3DTextAsync(projectId, connectionId);
		}

		/// <inheritdoc/>
		public async Task<string> GetScene3DTextAsync(Guid projectId, int connectionId, CancellationToken ct)
		{
			EnsureConnected();
			return await _client!.Presentation.GetDataScene3DTextAsync(projectId, connectionId, 0, ct);
		}

		/// <inheritdoc/>
		public async Task<string> CreateTemplateXmlAsync(Guid projectId, int connectionId, CancellationToken ct)
		{
			EnsureConnected();
			return await _client!.Template.CreateConTemplateAsync(projectId, connectionId, 0, ct);
		}

		/// <inheritdoc/>
		public async Task<TemplateConversions> GetDefaultTemplateMappingAsync(Guid projectId, int connectionId,
			ConTemplateMappingGetParam param, CancellationToken ct)
		{
			EnsureConnected();
			return await _client!.Template.GetDefaultTemplateMappingAsync(projectId, connectionId, param, 0, ct);
		}

		/// <inheritdoc/>
		public async Task<string> ApplyTemplateAsync(Guid projectId, int connectionId,
			ConTemplateApplyParam param, CancellationToken ct)
		{
			EnsureConnected();
			await _client!.Template.ApplyTemplateAsync(projectId, connectionId, param, 0, ct);
			return "Template was applied";
		}

		/// <inheritdoc/>
		public async Task<string> WeldSizingAsync(Guid projectId, int connectionId,
			IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum method)
		{
			EnsureConnected();
			return await _client!.Operation.PreDesignWeldsAsync(projectId, connectionId, method);
		}

		/// <inheritdoc/>
		public async Task<string> GetSettingsJsonAsync(Guid projectId, CancellationToken ct)
		{
			EnsureConnected();
			var settings = await _client!.Settings.GetSettingsAsync(projectId, null, 0, ct);
			return Tools.JsonTools.ToFormatedJson(settings);
		}

		/// <inheritdoc/>
		public async Task<string> UpdateSettingsAsync(Guid projectId, string settingsJson, CancellationToken ct)
		{
			EnsureConnected();
			var settingsMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(
				settingsJson, IdeaJsonSerializerSetting.GetTypeNameSerializerSetting());
			var newSettings = await _client!.Settings.UpdateSettingsAsync(projectId, settingsMap, 0, ct);
			return Tools.JsonTools.ToFormatedJson(newSettings);
		}

		/// <inheritdoc/>
		public async Task<List<ConLoadEffect>> GetLoadEffectsAsync(Guid projectId, int connectionId, CancellationToken ct)
		{
			EnsureConnected();
			return await _client!.LoadEffect.GetLoadEffectsAsync(projectId, connectionId, false, 0, ct);
		}

		/// <inheritdoc/>
		public async Task UpdateLoadEffectsAsync(Guid projectId, int connectionId,
			List<ConLoadEffect> loadEffects, CancellationToken ct)
		{
			EnsureConnected();
			foreach (var loadEffect in loadEffects)
			{
				await _client!.LoadEffect.UpdateLoadEffectAsync(projectId, connectionId, loadEffect, 0, ct);
			}
		}

		/// <inheritdoc/>
		public async Task<string> EvaluateExpressionAsync(Guid projectId, int connectionId,
			string expression, CancellationToken ct)
		{
			EnsureConnected();
			string expressionText = $"\"{expression}\"";
			return await _client!.Parameter.EvaluateExpressionAsync(projectId, connectionId, expressionText, 0, ct);
		}

		/// <inheritdoc/>
		public async Task<List<IdeaParameter>> GetParametersAsync(Guid projectId, int connectionId, CancellationToken ct)
		{
			EnsureConnected();
			return await _client!.Parameter.GetParametersAsync(projectId, connectionId, null, 0, ct);
		}

		/// <inheritdoc/>
		public async Task UpdateParametersAsync(Guid projectId, int connectionId,
			List<IdeaParameterUpdate> parameters, CancellationToken ct)
		{
			EnsureConnected();
			await _client!.Parameter.UpdateAsync(projectId, connectionId, parameters, 0, ct);
		}

		/// <inheritdoc/>
		public async Task<string> ExportIomAsync(Guid projectId, int connectionId)
		{
			EnsureConnected();
			return await _client!.Export.ExportIomAsync(projectId, connectionId);
		}

		/// <inheritdoc/>
		public async Task ExportIfcAsync(Guid projectId, int connectionId, string filePath)
		{
			EnsureConnected();
			await _client!.Export.ExportIfcFileAsync(projectId, connectionId, filePath);
		}

		/// <inheritdoc/>
		public async Task SaveReportPdfAsync(Guid projectId, int connectionId, string filePath)
		{
			EnsureConnected();
			await _client!.Report.SaveReportPdfAsync(projectId, connectionId, filePath);
		}

		/// <inheritdoc/>
		public async Task SaveReportWordAsync(Guid projectId, int connectionId, string filePath)
		{
			EnsureConnected();
			await _client!.Report.SaveReportWordAsync(projectId, connectionId, filePath);
		}

		/// <summary>
		/// Releases all resources used by the <see cref="ConnectionApiService"/>.
		/// Disposes of the API client and client factory.
		/// </summary>
		public void Dispose()
		{
			if (!_disposed)
			{
				_client?.Dispose();
				_client = null;

				if (_clientFactory is IDisposable disposable)
				{
					disposable.Dispose();
				}
				_clientFactory = null;

				_disposed = true;
			}
		}

		/// <summary>
		/// Throws <see cref="InvalidOperationException"/> if the API client is not connected.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when <see cref="_client"/> is <see langword="null"/>.</exception>
		private void EnsureConnected()
		{
			if (_client == null)
			{
				throw new InvalidOperationException("Not connected to the API service");
			}
		}
	}
}
