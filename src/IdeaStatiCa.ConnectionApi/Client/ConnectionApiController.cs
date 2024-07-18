﻿using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Common;
using IdeaStatiCa.Plugin.Api.ConnectionRest;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Result;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Settings;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Template;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi.Client
{
	public class ConnectionApiController : IConnectionApiController
	{
#if DEBUG
		public static readonly int TimeOut = -1;
#else
		public static readonly int TimeOut = 20 * 1000;
#endif

		public static readonly string ApiVersion = "1";

		private bool disposedValue = false;
		private readonly int restApiProcessId;
		private Guid activeProjectId;
		private Guid ClientId;

		private readonly IHttpClientWrapper _httpClient;
		private readonly IPluginLogger _pluginLogger;

		public static readonly string ConProjectController = "ConProject";
		public static readonly string ConnectionController = "ConConnection";
		public static readonly string ConParameterController = "ConParameter";
		public static readonly string ConTemplateController = "ConTemplate";
		public static readonly string ConCalculateController = "ConCalculation";
		public static readonly string ConLoadEffectController = "ConLoadEffect";

		public ConnectionApiController(int restApiProcessId, IHttpClientWrapper httpClient, IPluginLogger pluginLogger = null)
		{
			this.restApiProcessId = restApiProcessId;
			_httpClient = httpClient;
			_pluginLogger = pluginLogger ?? new NullLogger();
		}

		public async Task InitializeClientIdAsync(CancellationToken cancellationToken)
		{
			if (ClientId == Guid.Empty)
			{
				LogMethodCallToDebug();
				var clientIdResponse = await _httpClient.GetAsync<string>($"api/{ApiVersion}/{ConProjectController}/ConnectClient", cancellationToken, "text/plain");
				ClientId = Guid.Parse(clientIdResponse);
				_httpClient.AddRequestHeader("ClientId", ClientId.ToString());
			}
		}

		/// <inheritdoc cref="OpenProjectAsync(string, CancellationToken)" />
		public async Task<ConProject> OpenProjectAsync(string ideaConProject, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, message: $"path = '{ideaConProject}'");
			byte[] fileData = File.ReadAllBytes(ideaConProject);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

			var response = await _httpClient.PostAsyncStream<ConProject>($"api/{ApiVersion}/{ConProjectController}/Project", streamContent, cancellationToken);
			activeProjectId = response.ProjectId;
			LogMethodCallToDebug(ClientId, activeProjectId);
			return response;
		}

		public async Task<ConProject> CreateProjectFromIomFileAsync(string iomXmlFileName, string iomResXmlFileName, ConIomImportOptions options, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(message: $"path model = '{iomXmlFileName}' path result model = '{iomResXmlFileName}'");

			var model = OpenModel.LoadFromXmlFile(iomXmlFileName);
			LogMethodCallToDebug(message: $"path model = '{iomXmlFileName}' deserialized");
			var result = OpenModelResult.LoadFromXmlFile(iomResXmlFileName);
			LogMethodCallToDebug(message: $"path result model = '{iomResXmlFileName}' deserialized");

			return await this.CreateProjectFromIomContainerAsync(new OpenModelContainer() { OpenModel = model, OpenModelResult = result }, options, cancellationToken);
		}

		public async Task<ConProject> CreateProjectFromIomContainerFileAsync(string iomContainerXmlFileName, ConIomImportOptions options, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(message: $"path model = '{iomContainerXmlFileName}'");

			var model = IdeaRS.OpenModel.Tools.OpenModelContainerFromFile(iomContainerXmlFileName);
			LogMethodCallToDebug(message: $"path model = '{iomContainerXmlFileName}' deserialized");

			return await this.CreateProjectFromIomContainerAsync(model, options, cancellationToken);
		}

		public async Task<ConProject> CreateProjectFromIomModelAsync(OpenModel model, OpenModelResult result, ConIomImportOptions options, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId);
			return await this.CreateProjectFromIomContainerAsync(new OpenModelContainer() { OpenModel = model, OpenModelResult = result }, options, cancellationToken);
		}

		public async Task<ConProject> CreateProjectFromIomContainerAsync(OpenModelContainer model, ConIomImportOptions options, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId);

			// Build the full URL with query parameters
			var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

			if (options.ConnectionsToCreate != null)
			{
				foreach (var connection in options.ConnectionsToCreate)
				{
					query.Add("ConnectionsToCreate", connection.ToString());
				}
			}

			var response = await _httpClient.PostAsync<ConProject>($"api/{ApiVersion}/{ConProjectController}/CreateProjectFromIOM{query.ToString()}", model, cancellationToken, "application/xml");
			activeProjectId = response.ProjectId;

			LogMethodCallToDebug(ClientId, activeProjectId);

			return response;
		}

		public async Task<Stream> DownloadProjectAsync(CancellationToken token = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var result = await _httpClient.GetAsync<MemoryStream>($"api/{ApiVersion}/{ConProjectController}/{activeProjectId}/Download", token, "application/octet-stream");
			result.Seek(0, SeekOrigin.Begin);
			return result;
		}

		public async Task CloseProjectAsync(CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			try
			{
				await _httpClient.GetAsync<string>($"api/{ApiVersion}/{ConProjectController}/{activeProjectId}/Close", cancellationToken, "text/plain");
			}
			finally
			{
				activeProjectId = Guid.Empty;
			}
		}

		public async Task<ConProjectData> GetProjectDataAsync(CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.GetAsync<ConProjectData>($"api/{ApiVersion}/{ConProjectController}/{activeProjectId}/ProjectData", cancellationToken);
			return response;
		}

		public async Task<List<ConConnection>> GetConnectionsAsync(CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.GetAsync<List<ConConnection>>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/Connection", cancellationToken);
			return response;
		}

		public async Task<ConConnection> GetConnectionAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<List<ConConnection>>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/Connection?connectionId={connectionId}", cancellationToken);
			return response.FirstOrDefault();
		}

		public async Task<ConConnection> UpdateConnectionAsync(int connectionId, ConConnection connectionUpdate, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var result = await _httpClient.PutAsync<ConConnection>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/Connection", connectionUpdate, cancellationToken);
			return result;
		}

		public async Task<List<ConOperation>> GetOperationsAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<List<ConOperation>>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/Operations", cancellationToken);
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.CalculateAsync(List{int}, ConAnalysisTypeEnum, CancellationToken)"/>
		public async Task<List<ConResultSummary>> CalculateAsync(List<int> conToCalculateIds, ConAnalysisTypeEnum analysisType = ConAnalysisTypeEnum.Stress_Strain, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var calculateParam = new ConCalculationParameter() { AnalysisType = analysisType, ConnectionIds = conToCalculateIds };
			var response = await _httpClient.PostAsync<List<ConResultSummary>>($"api/{ApiVersion}/{ConCalculateController}/{activeProjectId}/Calculate", calculateParam, cancellationToken);
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.ResultsAsync(List{int}, CancellationToken)"/>
		public async Task<List<ConnectionCheckRes>> ResultsAsync(List<int> conToCalculateIds, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"Connections {string.Join(",", conToCalculateIds.Select(x => x))}");
			var calculateParam = new ConCalculationParameter() { ConnectionIds = conToCalculateIds };
			var response = await _httpClient.PostAsync<List<ConnectionCheckRes>>($"api/{ApiVersion}/{ConCalculateController}/{activeProjectId}/Results", calculateParam, cancellationToken, "application/xml");
			return response;
		}

		public async Task<string> GetRawResultsAsync(List<int> conToCalculateIds, ConAnalysisTypeEnum analysisType = ConAnalysisTypeEnum.Stress_Strain, CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.GetRawResultsAsync clientId = {ClientId} projectId = {activeProjectId}");
			var calculateParam = new ConCalculationParameter() { AnalysisType = analysisType, ConnectionIds = conToCalculateIds };
			var response = await _httpClient.PostAsync<string>($"api/{ApiVersion}/{ConCalculateController}/{activeProjectId}/rawresults-text", calculateParam, cancellationToken, "text/plain");
			return response;
		}

		public async Task<TemplateConversions> GetTemplateMappingAsync(int connectionId, string templateXml, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			ConTemplateMappingGetParam getTempMappingParam = new ConTemplateMappingGetParam() { Template = templateXml };
			var response = await _httpClient.PostAsync<TemplateConversions>($"api/{ApiVersion}/{ConTemplateController}/{activeProjectId}/{connectionId}/ConnectionTemplateMapping", getTempMappingParam, cancellationToken);
			return response;
		}

		public async Task<ConTemplateApplyResult> ApplyConnectionTemplateAsync(int connectionId, string templateXml, TemplateConversions templateMapping, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var applyTemplateParam = new ConTemplateApplyParam() { ConnectionTemplate = templateXml, Mapping = templateMapping };
			var response = await _httpClient.PostAsync<ConTemplateApplyResult>($"api/{ApiVersion}/{ConTemplateController}/{activeProjectId}/{connectionId}/ApplyConnectionTemplate", applyTemplateParam, cancellationToken);
			return response;
		}

		public async Task<List<ConMember>> GetMembersAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<List<ConMember>>($"api/{ApiVersion}/projects/{activeProjectId}/connections/{connectionId}/members", cancellationToken);
			return response;
		}

		public async Task<ConMember> GetMemberAsync(int connectionId, int memberId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId, $"memberId = {memberId}");
			var response = await _httpClient.GetAsync<ConMember>($"api/{ApiVersion}/projects/{activeProjectId}/connections/{connectionId}/members/{memberId}", cancellationToken);
			return response;
		}

		public async Task<ConMember> UpdateMemberAsync(int connectionId, int memberId, ConMember member, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId, $"memberId = {memberId}");
			var response = await _httpClient.PutAsync<ConMember>($"api/{ApiVersion}/projects/{activeProjectId}/connections/{connectionId}/members/{memberId}", member, cancellationToken);
			return response;
		}

		public async Task<OpenModel> ExportConnectionIomModel(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<OpenModelContainer>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/ExportIOMModel?Version={GetOpenModelVersion()}", cancellationToken, "application/xml");
			return response.OpenModel;
		}

		public async Task<OpenModelResult> ExportConnectionIomResults(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<OpenModelContainer>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/ExportIOMModel?Version={GetOpenModelVersion()}", cancellationToken, "application/xml");
			return response.OpenModelResult;
		}

		public async Task<OpenModelContainer> ExportConnectionIomContainer(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<OpenModelContainer>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/ExportIOMModel?Version={GetOpenModelVersion()}", cancellationToken, "application/xml");
			return response;
		}

		public async Task<ConnectionData> ExportConnectionIomConnectionData(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<ConnectionData>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/ExportIOMConnectionData", cancellationToken, "application/xml");
			return response;
		}

		public async Task<bool> UpdateProjectFromIomFileAsync(int connectionId, string iomXmlFileName, string iomResXmlFileName, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId, message: $"path model = '{iomXmlFileName}'  path result model = '{iomResXmlFileName}'");

			var model = OpenModel.LoadFromXmlFile(iomXmlFileName);
			LogMethodCallToDebug(message: $"path model = '{iomXmlFileName}' deserialized");
			var result = OpenModelResult.LoadFromXmlFile(iomResXmlFileName);
			LogMethodCallToDebug(message: $"path result model = '{iomResXmlFileName}' deserialized");

			return await this.UpdateProjectFromIomContainerAsync(connectionId, new OpenModelContainer() { OpenModel = model, OpenModelResult = result }, cancellationToken);
		}

		public async Task<bool> UpdateProjectFromIomContainerFileAsync(int connectionId, string iomContainerXmlFileName, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId, $"path model = '{iomContainerXmlFileName}'");

			var model = IdeaRS.OpenModel.Tools.OpenModelContainerFromFile(iomContainerXmlFileName);

			LogMethodCallToDebug(message: $"path model = '{iomContainerXmlFileName}' deserialized");

			return await this.UpdateProjectFromIomContainerAsync(connectionId, model, cancellationToken);
		}

		public async Task<bool> UpdateProjectFromIomModelAsync(int connectionId, OpenModel model, OpenModelResult result, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug();
			return await this.UpdateProjectFromIomContainerAsync(connectionId, new OpenModelContainer() { OpenModel = model, OpenModelResult = result }, cancellationToken);
		}

		public async Task<bool> UpdateProjectFromIomContainerAsync(int connectionId, OpenModelContainer model, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug();
			var response = await _httpClient.PostAsync<bool>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/UpdateProjectFromIOM", model, cancellationToken, "application/xml");
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.ExportToIfcAsync(int, CancellationToken)"/>
		public async Task<Stream> ExportToIfcAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var result = await _httpClient.GetAsync<MemoryStream>($"api/{ApiVersion}/{ConProjectController}/{activeProjectId}/{connectionId}/Ifc", cancellationToken, "application/octet-stream");
			return result;
		}

		private void LogMethodCallToDebug(
			Guid? clientId = null, 
			Guid? projectId = null, 
			int? connectionId = null, 
			string message = "",
			[CallerMemberName] string methodName = "")
		{
			var sb = new StringBuilder();
			sb.Append($"{GetType().Name}.{methodName}");
			if(clientId != null) 
			{
				sb.Append($" clientId = {clientId}"); 
			}
			if (projectId != null) 
			{ 
				sb.Append($" projectId = {projectId}");
			}
			if (connectionId != null)
			{
				sb.Append($" connectionId = {connectionId}"); 
			}
			if (!string.IsNullOrWhiteSpace(message))
			{
				sb.Append(" ");
				sb.Append(message);
			}
			_pluginLogger.LogDebug(sb.ToString());
		}

		/// <inheritdoc cref="IConnectionApiController.GetProductionCostAsync(int, CancellationToken)"/>
		public async Task<ConProductionCost> GetProductionCostAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var result = await _httpClient.GetAsync<ConProductionCost>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/production-cost", cancellationToken);
			return result;
		}

		/// <inheritdoc cref="IConnectionApiController.GetLoadEffectsAsync(int, bool, CancellationToken)(int, CancellationToken)" >
		public async Task<List<ConLoadEffect>> GetLoadEffectsAsync(int id, bool isPercentage = false, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, id);
			var result = await _httpClient.GetAsync<List<ConLoadEffect>>($"api/{ApiVersion}/{ConLoadEffectController}/{activeProjectId}/{id}/LoadEffect?IsPercentage={isPercentage}", cancellationToken);
			return result;
		}

		/// <inheritdoc cref="IConnectionApiController.AddLoadEffectAsync(int, ConLoadEffect, CancellationToken)" >
		public async Task<ConLoadEffect> AddLoadEffectAsync(int id, ConLoadEffect loadEffect,CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, id, message: $"Adding load effect = {loadEffect.Id}");
			var result = await _httpClient.PostAsync<ConLoadEffect>($"api/{ApiVersion}/{ConLoadEffectController}/{activeProjectId}/{id}/LoadEffect", loadEffect, cancellationToken);
			return result;
		}

		/// <inheritdoc cref="IConnectionApiController.DeleteLoadEffectAsync(int, int)" >
		public async Task DeleteLoadEffectAsync(int id, int loadEffectId)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, id, message: $"Deleting {loadEffectId}");
			await _httpClient.DeleteAsync<int>($"api/{ApiVersion}/{ConLoadEffectController}/{activeProjectId}/{id}/LoadEffect/{loadEffectId}");
		}

		/// <inheritdoc cref="IConnectionApiController.UpdateLoadEffectAsync(int, ConLoadEffect)" >
		public async Task<ConLoadEffect> UpdateLoadEffectAsync(int id, ConLoadEffect le1)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, id, message: $"Updating {le1.Id}");
			return await _httpClient.PutAsync<ConLoadEffect>($"api/{ApiVersion}/{ConLoadEffectController}/{activeProjectId}/{id}/LoadEffect", le1, CancellationToken.None);
		}

		private Version GetOpenModelVersion()
		{
			var op = new OpenModel();

			if (op.Version is int)
			{
				return Version.Parse($"{op.Version.ToString()}.0.0");
			}
			else
			{
				return Version.Parse(op.Version.ToString());
			}
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if(activeProjectId != Guid.Empty)
					{
						try
						{
							var closeTask = Task.Run(async () => await CloseProjectAsync());

							if (closeTask.Wait(TimeOut))
							{
								_pluginLogger.LogInformation($"Project with ID {activeProjectId} was closed successfully");
							}
							else
							{
								_pluginLogger.LogWarning($"Project with ID {activeProjectId} was not closed in time");
							}
						}
						catch (Exception ex)
						{
							_pluginLogger.LogWarning("Error during closing the project", ex);
						}

					}
					if (restApiProcessId != -1)
					{
						var restApiProcess = Process.GetProcessById(restApiProcessId);
						if (restApiProcess is { })
						{
							if (!restApiProcess.HasExited)
							{

								_pluginLogger.LogInformation($"Cleaning the API process with ID {restApiProcessId}");

								// TODO - I suppose Kill process does't release resources properly (temp files on a disk)
								restApiProcess.Kill();
							}
						}
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}


	}
}
