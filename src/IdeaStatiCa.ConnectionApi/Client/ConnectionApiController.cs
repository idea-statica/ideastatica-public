using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.Connection;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.Plugin;
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
using IdeaStatiCa.Api.Connection.Model.Material;
using IdeaStatiCa.Api.Connection.Model.Connection;
using Castle.MicroKernel.Registration;

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

		public ConnectionApiController(int restApiProcessId, IHttpClientWrapper httpClient, IPluginLogger pluginLogger = null)
		{
			this.restApiProcessId = restApiProcessId;
			_httpClient = httpClient;
			_pluginLogger = pluginLogger ?? new NullLogger();
		}

		public Tuple<string, string> GetConnectionInfo()
		{
			return new Tuple<string, string>(ClientId.ToString(), activeProjectId.ToString());
		}

		public async Task InitializeClientIdAsync(CancellationToken cancellationToken)
		{
			if (ClientId == Guid.Empty)
			{
				LogMethodCallToDebug();
				var clientIdResponse = await _httpClient.GetAsync<string>($"api/{ApiVersion}/{ConRestApiConstants.Client}/{ConRestApiConstants.ConnectClient}", cancellationToken, "text/plain");
				ClientId = Guid.Parse(clientIdResponse);
				_httpClient.AddRequestHeader("ClientId", ClientId.ToString());
			}
		}

		/// <inheritdoc cref="OpenProjectAsync(string, CancellationToken)" />
		public async Task<ConProject> OpenProjectAsync(string ideaConProject, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, message: $"path = '{ideaConProject}'");
			using (var formData = new MultipartFormDataContent())
			{
				// Add file content
				using (var fileStream = new FileStream(ideaConProject, FileMode.Open, FileAccess.Read))
				{
					var streamContent = new StreamContent(fileStream);
					streamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

					// Add the file stream content to the form data, with the form field name "ideaConFile"
					formData.Add(streamContent, "ideaConFile", Path.GetFileName(ideaConProject));

					// Send the POST request to the API
					// Send the request
					var response = await _httpClient.PostAsyncForm<ConProject>($"api/{ApiVersion}/{ConRestApiConstants.Projects}/open", formData, cancellationToken, false);
					activeProjectId = response.ProjectId;
					LogMethodCallToDebug(ClientId, activeProjectId);

					return response;
				}
			}			
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

			var response = await _httpClient.PostAsync<ConProject>($"api/{ApiVersion}/{ConRestApiConstants.Projects}/import-iom{query.ToString()}", model, cancellationToken);
			activeProjectId = response.ProjectId;

			LogMethodCallToDebug(ClientId, activeProjectId);

			return response;
		}

		public async Task<Stream> DownloadProjectAsync(CancellationToken token = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var result = await _httpClient.GetAsync<MemoryStream>($"{GetProjectRoute()}/download", token, "application/octet-stream");
			result.Seek(0, SeekOrigin.Begin);
			return result;
		}

		public async Task CloseProjectAsync(CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			try
			{
				await _httpClient.GetAsync<string>($"{GetProjectRoute()}/close", cancellationToken, "text/plain");
			}
			finally
			{
				activeProjectId = Guid.Empty;
			}
		}

		public async Task<ConProject> GetProjectAsync(CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.GetAsync<ConProject>($"{GetProjectRoute()}", cancellationToken);
			return response;
		}

		public async Task<List<ConConnection>> GetConnectionsAsync(CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.GetAsync<List<ConConnection>>($"{GetProjectRoute()}/{ConRestApiConstants.Connections}", cancellationToken);
			return response;
		}

		public async Task<ConConnection> GetConnectionAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<ConConnection>(GetConnectionRoute(connectionId), cancellationToken);
			return response;
		}

		public async Task<ConConnection> UpdateConnectionAsync(int connectionId, ConConnection connectionUpdate, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var result = await _httpClient.PutAsync<ConConnection>(GetConnectionRoute(connectionId), connectionUpdate, cancellationToken);
			return result;
		}

		public async Task<List<ConOperation>> GetOperationsAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<List<ConOperation>>($"{GetConnectionRoute(connectionId)}/operations", cancellationToken);
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.CalculateAsync(List{int}, ConAnalysisTypeEnum, CancellationToken)"/>
		public async Task<List<ConResultSummary>> CalculateAsync(List<int> conToCalculateIds, ConAnalysisTypeEnum analysisType = ConAnalysisTypeEnum.Stress_Strain, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var calculateParam = new ConCalculationParameter() { AnalysisType = analysisType, ConnectionIds = conToCalculateIds };
			var response = await _httpClient.PostAsync<List<ConResultSummary>>($"{GetProjectRoute()}/connections/calculate", calculateParam, cancellationToken, "application/json");
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.ResultsAsync(List{int}, CancellationToken)"/>
		public async Task<List<ConnectionCheckRes>> ResultsAsync(List<int> conToCalculateIds, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"Connections {string.Join(",", conToCalculateIds.Select(x => x))}");
			var calculateParam = new ConCalculationParameter() { ConnectionIds = conToCalculateIds };
			var response = await _httpClient.PostAsync<List<ConnectionCheckRes>>($"{GetProjectRoute()}/connections/results", calculateParam, cancellationToken, "application/json");
			return response;
		}

		public async Task<string> GetRawResultsAsync(List<int> conToCalculateIds, ConAnalysisTypeEnum analysisType = ConAnalysisTypeEnum.Stress_Strain, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var calculateParam = new ConCalculationParameter() { AnalysisType = analysisType, ConnectionIds = conToCalculateIds };
			var response = await _httpClient.PostAsync<string>($"{ConRestApiConstants.Projects}/{{projectId}}/connections/rawresults-text", calculateParam, cancellationToken, "text/plain");
			return response;
		}

		public async Task<TemplateConversions> GetTemplateMappingAsync(int connectionId, string templateXml, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			ConTemplateMappingGetParam getTempMappingParam = new ConTemplateMappingGetParam() { Template = templateXml };
			var response = await _httpClient.PostAsync<TemplateConversions>($"{GetConnectionRoute(connectionId)}/get-default-mapping", getTempMappingParam, cancellationToken);
			return response;
		}

		public async Task<ConTemplateApplyResult> ApplyConnectionTemplateAsync(int connectionId, string templateXml, TemplateConversions templateMapping, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var applyTemplateParam = new ConTemplateApplyParam() { ConnectionTemplate = templateXml, Mapping = templateMapping };
			var response = await _httpClient.PostAsync<ConTemplateApplyResult>($"{GetConnectionRoute(connectionId)}/apply-template", applyTemplateParam, cancellationToken);
			return response;
		}

		public async Task<List<ConMember>> GetMembersAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<List<ConMember>>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.Members}", cancellationToken);
			return response;
		}

		public async Task<ConMember> GetMemberAsync(int connectionId, int memberId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId, $"memberId = {memberId}");
			var response = await _httpClient.GetAsync<ConMember>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.Members}/{memberId}", cancellationToken);
			return response;
		}

		public async Task<ConMember> UpdateMemberAsync(int connectionId, int memberId, ConMember member, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId, $"memberId = {memberId}");
			var response = await _httpClient.PutAsync<ConMember>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.Members}/{memberId}", member, cancellationToken);
			return response;
		}

		public async Task<OpenModelContainer> ExportConnectionIomContainer(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await ExportConnectionIomContainerInternal(connectionId, cancellationToken);
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.ExportToIfcAsync(int, CancellationToken)"/>
		public async Task<Stream> ExportToIfcAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var result = await _httpClient.GetAsync<MemoryStream>($"{GetConnectionRoute(connectionId)}/export-ifc", cancellationToken, "application/octet-stream");
			return result;
		}

		private async Task<OpenModelContainer> ExportConnectionIomContainerInternal(int connectionId, CancellationToken cancellationToken = default)
		{
			return await _httpClient.GetAsync<OpenModelContainer>($"{GetConnectionRoute(connectionId)}/export-iom?Version={GetOpenModelVersion()}", cancellationToken, "application/xml");
		}

		public async Task<ConnectionData> ExportConnectionIomConnectionData(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var response = await _httpClient.GetAsync<ConnectionData>($"{GetConnectionRoute(connectionId)}/export-iom-connection-data", cancellationToken, "application/xml");
			return response;
		}

		public async Task<bool> UpdateProjectFromIomFileAsync(string iomXmlFileName, string iomResXmlFileName, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"path model = '{iomXmlFileName}'  path result model = '{iomResXmlFileName}'");

			var model = OpenModel.LoadFromXmlFile(iomXmlFileName);
			LogMethodCallToDebug(message: $"path model = '{iomXmlFileName}' deserialized");
			var result = OpenModelResult.LoadFromXmlFile(iomResXmlFileName);
			LogMethodCallToDebug(message: $"path result model = '{iomResXmlFileName}' deserialized");

			var container = new OpenModelContainer() { OpenModel = model, OpenModelResult = result };
			return await this.UpdateProjectFromIomContainerAsync(container, cancellationToken);
		}

		public async Task<bool> UpdateProjectFromIomContainerFileAsync(string iomContainerXmlFileName, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"path model = '{iomContainerXmlFileName}'");

			var container = IdeaRS.OpenModel.Tools.OpenModelContainerFromFile(iomContainerXmlFileName);

			LogMethodCallToDebug(message: $"path model = '{iomContainerXmlFileName}' deserialized");

			return await this.UpdateProjectFromIomContainerAsync(container, cancellationToken);
		}

		public async Task<bool> UpdateProjectFromIomModelAsync(OpenModel model, OpenModelResult result, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug();
			return await this.UpdateProjectFromIomContainerAsync(new OpenModelContainer() { OpenModel = model, OpenModelResult = result }, cancellationToken);
		}

		public async Task<bool> UpdateProjectFromIomContainerAsync(OpenModelContainer model, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug();
			var response = await _httpClient.PostAsync<bool>($"{GetProjectRoute()}/update-iom", model, cancellationToken);
			return response;
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
			if (clientId != null)
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
			var result = await _httpClient.GetAsync<ConProductionCost>($"{GetConnectionRoute(connectionId)}/production-cost", cancellationToken);
			return result;
		}

		/// <inheritdoc cref="IConnectionApiController.GetLoadEffectsAsync(int, bool, CancellationToken)(int, CancellationToken)" >
		public async Task<List<ConLoadEffect>> GetLoadEffectsAsync(int connectionId, bool isPercentage = false, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var result = await _httpClient.GetAsync<List<ConLoadEffect>>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.LoadEffect}?IsPercentage={isPercentage}", cancellationToken);
			return result;
		}

		/// <inheritdoc cref="IConnectionApiController.GetLoadEffectAsync(int, bool, CancellationToken)(int, CancellationToken)" >
		public async Task<ConLoadEffect> GetLoadEffectAsync(int connectionId, int loadEffectId, bool isPercentage = false, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			var result = await _httpClient.GetAsync<ConLoadEffect>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.LoadEffect}/{loadEffectId}?IsPercentage={isPercentage}", cancellationToken);
			return result;
		}

		/// <inheritdoc cref="IConnectionApiController.AddLoadEffectAsync(int, ConLoadEffect, CancellationToken)" >
		public async Task<ConLoadEffect> AddLoadEffectAsync(int connectionId, ConLoadEffect loadEffect, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId, message: $"Adding load effect = {loadEffect.Id}");
			var result = await _httpClient.PostAsync<ConLoadEffect>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.LoadEffect}", loadEffect, cancellationToken);
			return result;
		}

		/// <inheritdoc cref="IConnectionApiController.UpdateLoadEffectAsync(int, ConLoadEffect)" >
		public async Task<ConLoadEffect> UpdateLoadEffectAsync(int connectionId, ConLoadEffect le1)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId, message: $"Updating {le1.Id}");
			return await _httpClient.PutAsync<ConLoadEffect>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.LoadEffect}/{le1.Id}", le1, CancellationToken.None);
		}

		/// <inheritdoc cref="IConnectionApiController.DeleteLoadEffectAsync(int, int)" >
		public async Task DeleteLoadEffectAsync(int connectionId, int loadEffectId)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId, message: $"Deleting {loadEffectId}");
			await _httpClient.DeleteAsync<int>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.LoadEffect}/{loadEffectId}");
		}

		/// <inheritdoc cref="IConnectionApiController.GetConnectionSetupAsync(CancellationToken)"/>
		public async Task<ConnectionSetup> GetConnectionSetupAsync(CancellationToken cancellationToken)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.GetAsync<ConnectionSetup>($"{GetProjectRoute()}/connection-setup", cancellationToken, "application/json");
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.UpdateConnectionSetupAsync(ConnectionSetup, CancellationToken)"/>
		public async Task<ConnectionSetup> UpdateConnectionSetupAsync(ConnectionSetup connectionSetup, CancellationToken cancellationToken)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.PutAsync<ConnectionSetup>($"{GetProjectRoute()}/connection-setup", connectionSetup, cancellationToken);
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.GetMaterialsAsync(string)" >
		public async Task<List<object>> GetMaterialsAsync(string type = "all")
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"Get material type {type}");
			var materialType = type == "all" ? "" : $"/{type}";
			return await _httpClient.GetAsync<List<object>>($"{GetProjectRoute()}/{ConRestApiConstants.Materials}{materialType}", CancellationToken.None);
		}

		/// <inheritdoc cref="IConnectionApiController.GetCrossSectionsAsync()" >
		public async Task<List<object>> GetCrossSectionsAsync()
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"Get cross sections");
			return await _httpClient.GetAsync<List<object>>($"{GetProjectRoute()}/{ConRestApiConstants.Materials}/cross-sections", CancellationToken.None);
		}

		/// <inheritdoc cref="IConnectionApiController.GetBoltAssembliesAsync()" >
		public async Task<List<object>> GetBoltAssembliesAsync()
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"Get pins");
			return await _httpClient.GetAsync<List<object>>($"{GetProjectRoute()}/{ConRestApiConstants.Materials}/bolt-assemblies", CancellationToken.None);
		}

		/// <inheritdoc cref="IConnectionApiController.AddMaterialAsync(ConMprlElement, string)" >
		public async Task<ConMprlElement> AddMaterialAsync(ConMprlElement newMaterial, string materialType)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"Add material - {newMaterial.MprlName}");
			return await _httpClient.PostAsync<ConMprlElement>($"{GetProjectRoute()}/{ConRestApiConstants.Materials}/{materialType}", newMaterial, CancellationToken.None);
		}

		/// <inheritdoc cref="IConnectionApiController.AddCrossSectionAsync(ConMprlCrossSection)" >
		public async Task<ConMprlCrossSection> AddCrossSectionAsync(ConMprlCrossSection newCrossSection)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"Add cross section {newCrossSection.MprlName}");
			return await _httpClient.PostAsync<ConMprlCrossSection>($"{GetProjectRoute()}/{ConRestApiConstants.Materials}/cross-sections", newCrossSection, CancellationToken.None);
		}

		/// <inheritdoc cref="IConnectionApiController.AddBoltAssemblyAsync(ConMprlElement)"/>
		public async Task<ConMprlElement> AddBoltAssemblyAsync(ConMprlElement newBoltAssembly)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, message: $"Add bolt assembly {newBoltAssembly.MprlName}");
			return await _httpClient.PostAsync<ConMprlElement>($"{GetProjectRoute()}/{ConRestApiConstants.Materials}/bolt-assemblies", newBoltAssembly, CancellationToken.None);
		}

		/// <inheritdoc cref="IConnectionApiController.GetParametersAsync(int)"/>
		public async Task<List<IdeaParameter>> GetParametersAsync(int connectionId, bool includeHidden)
		{
			LogMethodCallToDebug(ClientId, activeProjectId, connectionId);
			return await _httpClient.GetAsync<List<IdeaParameter>>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.Parameters}?includeHidden={includeHidden.ToString()}", CancellationToken.None);
		}

		/// <inheritdoc cref="IConnectionApiController.GetVersionAsync"/>
		public async Task<string> GetVersionAsync()
		{
			return await _httpClient.GetAsync<string>($"api/{ApiVersion}/version", CancellationToken.None);
		}

		public async Task<string> GetDataScene3DAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.GetAsync<string>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.Presentation}", cancellationToken, "text/plain");
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.GetConnectionTemplateAsync(int, CancellationToken)"/>
		public async Task<string> GetConnectionTemplateAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.GetAsync<string>($"{GetConnectionRoute(connectionId)}/get-template", cancellationToken, "text/plain");
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.SetBearingMemberAsync(int, int, CancellationToken)"/>
		public async Task<ConMember> SetBearingMemberAsync(int connectionId, int memberId, CancellationToken cancellationToken)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.PutAsync<ConMember>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.Members}/{memberId}/set-bearing-member", new { }, cancellationToken);
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.GetLoadEffectLoadSettingsAsync(int, CancellationToken)"/>
		public async Task<ConLoadSettings> GetLoadEffectLoadSettingsAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.GetAsync<ConLoadSettings>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.LoadEffect}/get-load-settings", cancellationToken);
			return response;
		}

		/// <inheritdoc cref="IConnectionApiController.SetLoadEffectLoadSettingsAsync(int, ConLoadSettings, CancellationToken)"/>
		public async Task<ConLoadSettings> SetLoadEffectLoadSettingsAsync(int connectionId, ConLoadSettings settings, CancellationToken cancellationToken = default)
		{
			LogMethodCallToDebug(ClientId, activeProjectId);
			var response = await _httpClient.PostAsync<ConLoadSettings>($"{GetConnectionRoute(connectionId)}/{ConRestApiConstants.LoadEffect}/set-load-settings", settings, cancellationToken);
			return response;
		}

		private string GetProjectRoute()
		=> $"api/{ApiVersion}/{ConRestApiConstants.Projects}/{activeProjectId}";

		private string GetConnectionRoute(int connectionId)
		=> $"api/{ApiVersion}/{ConRestApiConstants.Projects}/{activeProjectId}/{ConRestApiConstants.Connections}/{connectionId}";

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
					if (activeProjectId != Guid.Empty)
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

								// TODO - I suppose Kill process doesn't release resources properly (temp files on a disk)
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
