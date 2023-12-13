using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaStatiCa.RcsClient.HttpWrapper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsClient.Client
{
	public class RcsApiClient : IRcsApiController
	{
		private readonly IHttpClientWrapper httpClient;
		private readonly int restApiProcessId;
		private readonly IPluginLogger pluginLogger;

		private Guid ActiveProjectId;

		public RcsApiClient(int processId, IPluginLogger logger, IHttpClientWrapper httpClientWrapper)
		{
			pluginLogger = logger;
			this.restApiProcessId = processId;
			this.httpClient = httpClientWrapper;
		}

		/// <summary>
		/// Disposing the object along with related API instance
		/// </summary>
		public void Dispose()
		{
			var restApiProcess = Process.GetProcessById(restApiProcessId);
			if (restApiProcess is { })
			{
				if (!restApiProcess.HasExited)
				{
					pluginLogger.LogInformation($"Cleaning the API process with ID {restApiProcessId}");
					restApiProcess.Kill();
				}
			}
		}

		/// <inheritdoc cref="IRcsApiController.OpenProjectAsync(string, CancellationToken)"/>
		public async Task<bool> OpenProjectAsync(string path, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.OpenProjectAsync path = '{path}'");

			byte[] fileData = File.ReadAllBytes(path);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

			ActiveProjectId = await httpClient.PostAsyncStream<Guid>("Project/OpenProject", streamContent, token);
			pluginLogger.LogDebug($"RcsApiClient.OpenProjectAsync projectId = {ActiveProjectId}");

			return true;
		}

		/// <inheritdoc cref="IRcsApiController.CreateProjectFromIOMFileAsync(string, CancellationToken)"/>
		public async Task<bool> CreateProjectFromIOMFileAsync(string iomFilePath, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.CreateProjectFromIOMFileAsync");

			byte[] fileData = File.ReadAllBytes(iomFilePath);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

			ActiveProjectId = await httpClient.PostAsyncStream<Guid>("Project/CreateProjectFromIOMFile", streamContent, token);
			pluginLogger.LogDebug($"RcsApiClient.OpenProjectAsync projectId = {ActiveProjectId}");

			return true;
		}

		/// <inheritdoc cref="IRcsApiController.CreateProjectFromIOMAsync(OpenModel, CancellationToken) "/>
		public async Task<bool> CreateProjectFromIOMAsync(OpenModel model, CancellationToken token)
		{
			pluginLogger.LogDebug("RcsApiClient.OpenProjectFromModelAsync");
			ActiveProjectId = await httpClient.PostAsync<Guid>("Project/OpenProjectFromModel", model, token);
			return true;
		}

		/// <inheritdoc cref="IRcsApiController.CalculateAsync(RcsCalculationParameters, CancellationToken) "/>
		public async Task<List<RcsSectionResultOverview>> CalculateAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.CalculateResultsAsync projectId = {ActiveProjectId}");
			try
			{
				var res = await httpClient.PostAsync<List<RcsSectionResultOverview>>($"Calculations/{ActiveProjectId}/Calculate", parameters, token, "application/xml");
				return res;
			}
			catch(OperationCanceledException ex)
			{
				pluginLogger.LogDebug($"{ex.Message}");
				throw ex;
			}
		}

		/// <inheritdoc cref="IRcsApiController.GetProjectSummaryAsync(CancellationToken) "/>
		public async Task<RcsProjectSummaryModel> GetProjectSummaryAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectSummaryAsync projectId = {ActiveProjectId}");
			var res=  await httpClient.GetAsync<RcsProjectSummaryModel>($"Project/{ActiveProjectId}/ProjectSummary", token);
			return res;
		}

		/// <inheritdoc cref="IRcsApiController.GetProjectDataAsync(CancellationToken) "/>
		public async Task<RcsProjectData> GetProjectDataAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectDataAsync projectId = {ActiveProjectId}");
			var res = await httpClient.GetAsync<RcsProjectData>($"Project/{ActiveProjectId}/ProjectData", token);
			return res;
		}

		/// <inheritdoc cref="IRcsApiController.DownloadAsync(CancellationToken) "/>
		public async Task<Stream> DownloadProjectAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.DownloadProjectAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<MemoryStream>($"Project/{ActiveProjectId}/DownloadProject", token, "application/octet-stream");
			return result;
		}
		
		/// <inheritdoc cref="IRcsApiController.GetResultsAsync(RcsResultParameters, CancellationToken)"/>
		public async Task<List<RcsDetailedResultForSection>> GetResultsAsync(RcsResultParameters parameters, CancellationToken token)
		{
			return await httpClient.PostAsync<List<RcsDetailedResultForSection>>($"Calculations/{ActiveProjectId}/GetResults", parameters, token, "application/xml");
		}

		/// <inheritdoc cref="IRcsApiController.GetProjectSectionsAsync(CancellationToken)  "/>
		public async Task<List<RcsSectionModel>> GetProjectSectionsAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectSectionsAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<List<RcsSectionModel>>($"Project/{ActiveProjectId}/ProjectSections", token);
			return result;
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectMembersAsync(CancellationToken) "/>
		public async Task<List<RcsCheckMemberModel>> GetProjectMembersAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectMembersAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<List<RcsCheckMemberModel>>($"Project/{ActiveProjectId}/ProjectSections", token);
			return result;
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectReinforcedCrossSectionsAsync(CancellationToken) "/>
		public async Task<List<ReinforcedCrossSectionModel>> GetProjectReinforcedCrossSectionsAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectReinforcedCrossSectionsAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<List<ReinforcedCrossSectionModel>>($"Project/{ActiveProjectId}/ProjectReinforcedCrossSections", token);
			return result;
		}

		/// <inheritdoc cref="IRcsApiController.UpdateSectionAsync(RcsSectionModel, CancellationToken)"/>
		public async Task<RcsSectionModel> UpdateSectionAsync(RcsSectionModel newSectionData, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.UpdateSectionAsync projectId = {ActiveProjectId} sectionId = {newSectionData.Id} reinforcedSectionId = {newSectionData.RCSId}");
			var result = await httpClient.PutAsync<RcsSectionModel>($"Section/{ActiveProjectId}/UpdateSection", newSectionData, token);
			return result;
		}

		/// <inheritdoc cref="IRcsApiController.ImportReinfCssAsync(ReinfCssImportSetting, string)"/>
		public async Task<ReinforcedCrossSectionModel> ImportReinfCssAsync(ReinfCssImportSetting importSetting, string reinfCssTemplate, CancellationToken token)
		{
			var data = new ReinfCssImportData(){Setting = importSetting, Template = reinfCssTemplate };
			pluginLogger.LogDebug($"RcsApiClient.ImportReinfCssAsync projectId = {ActiveProjectId} reinfCssId = {importSetting?.ReinfCssId}");
			var result = await httpClient.PostAsync<ReinforcedCrossSectionModel>($"Section/{ActiveProjectId}/ImportReinfCss", data, token);
			return result;
		}

		/// <inheritdoc cref="IRcsApiController.SaveProjectAsync(string, CancellationToken)"/>
		public async Task SaveProjectAsync(string outputPath, CancellationToken token)
		{
			var memoryStream = await httpClient.GetAsync<MemoryStream>($"Project/{ActiveProjectId}/DownloadProject", token);
			// Ensure the MemoryStream is at the beginning
			memoryStream.Seek(0, SeekOrigin.Begin);

			// Create a FileStream and copy the MemoryStream data to it
			using (FileStream fileStream = File.Create(outputPath))
			{
				memoryStream.CopyTo(fileStream);
			}
		}

		/// <inheritdoc cref="IRcsApiController.GetCodeSettings(CancellationToken)"/>
		public async Task<string> GetCodeSettings(CancellationToken token)
		{
			return await httpClient.GetAsync<string>($"Project/{ActiveProjectId}/GetCodeSettings", token, "text/plain");
		}

		/// <inheritdoc cref="IRcsApiController.UpdateCodeSettings(List{RcsSettingModel}, CancellationToken)"/>
		public async Task<bool> UpdateCodeSettings(List<RcsSettingModel> setup, CancellationToken token)
		{
			return await httpClient.PutAsync<bool>($"Project/{ActiveProjectId}/UpdateCodeSettings", setup, token);
		}
	}
}
