using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.RCS;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaStatiCa.RcsClient.HttpWrapper;
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
		public async Task<bool> OpenProjectAsync(string path, CancellationToken token = default)
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
		public async Task<bool> CreateProjectFromIOMFileAsync(string iomFilePath, CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.CreateProjectFromIOMFileAsync");

			byte[] fileData = File.ReadAllBytes(iomFilePath);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

			ActiveProjectId = await httpClient.PostAsyncStream<Guid>("Project/CreateProjectFromIOMFile", streamContent, token);
			pluginLogger.LogDebug($"RcsApiClient.CreateProjectFromIOMFile projectId = {ActiveProjectId}");

			return true;
		}

		/// <inheritdoc cref="IRcsApiController.CreateProjectFromIOMAsync(OpenModel, CancellationToken) "/>
		public async Task<bool> CreateProjectFromIOMAsync(OpenModel model, CancellationToken token = default)
		{
			pluginLogger.LogDebug("RcsApiClient.CreateProjectFromIOM");
			ActiveProjectId = await httpClient.PostAsync<Guid>("Project/CreateProjectFromIOM", model, token);
			return true;
		}

		/// <inheritdoc cref="IRcsApiController.CalculateAsync(RcsCalculationParameters, CancellationToken) "/>
		public async Task<List<RcsSectionResultOverview>> CalculateAsync(RcsCalculationParameters parameters, CancellationToken token = default)
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
		public async Task<RcsProjectSummary> GetProjectSummaryAsync(CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectSummaryAsync projectId = {ActiveProjectId}");
			var res=  await httpClient.GetAsync<RcsProjectSummary>($"Project/{ActiveProjectId}/ProjectSummary", token);
			return res;
		}

		/// <inheritdoc cref="IRcsApiController.GetProjectDataAsync(CancellationToken) "/>
		public async Task<RcsProjectData> GetProjectDataAsync(CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectDataAsync projectId = {ActiveProjectId}");
			var res = await httpClient.GetAsync<RcsProjectData>($"Project/{ActiveProjectId}/ProjectData", token);
			return res;
		}

		/// <inheritdoc cref="IRcsApiController.DownloadAsync(CancellationToken) "/>
		public async Task<Stream> DownloadProjectAsync(CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.DownloadProjectAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<MemoryStream>($"Project/{ActiveProjectId}/DownloadProject", token, "application/octet-stream");
			return result;
		}
		
		/// <inheritdoc cref="IRcsApiController.GetResultsAsync(RcsResultParameters, CancellationToken)"/>
		public async Task<List<RcsSectionResultDetailed>> GetResultsAsync(RcsResultParameters parameters, CancellationToken token = default)
		{
			return await httpClient.PostAsync<List<RcsSectionResultDetailed>>($"Calculations/{ActiveProjectId}/GetResults", parameters, token, "application/xml");
		}

		/// <inheritdoc cref="IRcsApiController.GetProjectSectionsAsync(CancellationToken)  "/>
		public async Task<List<RcsSection>> GetProjectSectionsAsync(CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectSectionsAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<List<RcsSection>>($"Project/{ActiveProjectId}/ProjectSections", token);
			return result;
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectMembersAsync(CancellationToken) "/>
		public async Task<List<RcsCheckMember>> GetProjectMembersAsync(CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectMembersAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<List<RcsCheckMember>>($"Project/{ActiveProjectId}/ProjectSections", token);
			return result;
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectReinforcedCrossSectionsAsync(CancellationToken) "/>
		public async Task<List<RcsReinforcedCrossSection>> GetProjectReinforcedCrossSectionsAsync(CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectReinforcedCrossSectionsAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<List<RcsReinforcedCrossSection>>($"Project/{ActiveProjectId}/ProjectReinforcedCrossSections", token);
			return result;
		}

		/// <inheritdoc cref="IRcsApiController.UpdateSectionAsync(RcsSection, CancellationToken)"/>
		public async Task<RcsSection> UpdateSectionAsync(RcsSection newSectionData, CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.UpdateSectionAsync projectId = {ActiveProjectId} sectionId = {newSectionData.Id} reinforcedSectionId = {newSectionData.RCSId}");
			var result = await httpClient.PutAsync<RcsSection>($"Section/{ActiveProjectId}/UpdateSection", newSectionData, token);
			return result;
		}

		/// <inheritdoc cref="IRcsApiController.ImportReinforcedCrossSectionAsync(RcsReinforcedCrosssSectionImportSetting, string)"/>
		public async Task<RcsReinforcedCrossSection> ImportReinforcedCrossSectionAsync(RcsReinforcedCrosssSectionImportSetting importSetting, string reinfCssTemplate, CancellationToken token = default)
		{
			var data = new RcsReinforcedCrossSectionImportData(){Setting = importSetting, Template = reinfCssTemplate };
			pluginLogger.LogDebug($"RcsApiClient.ImportReinforcedCrossSectionAsync projectId = {ActiveProjectId} reinfCssId = {importSetting?.ReinforcedCrossSectionId}");
			var result = await httpClient.PostAsync<RcsReinforcedCrossSection>($"Section/{ActiveProjectId}/ImportReinforcedCrossSection", data, token);
			return result;
		}

		/// <inheritdoc cref="IRcsApiController.SaveProjectAsync(string, CancellationToken)"/>
		public async Task SaveProjectAsync(string outputPath, CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.SaveProjectAsync projectId = {ActiveProjectId} outputPath = '{outputPath}'");
			using (var rcsProjectStream = await DownloadProjectAsync(token))
			{
				rcsProjectStream.Seek(0, System.IO.SeekOrigin.Begin);
				using (FileStream fileStream = File.Create(outputPath))
				{
					await rcsProjectStream.CopyToAsync(fileStream);
				}
				
			}
		}

		/// <inheritdoc cref="IRcsApiController.GetCodeSettings(CancellationToken)"/>
		public async Task<string> GetCodeSettings(CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetCodeSettings projectId = {ActiveProjectId}");
			return await httpClient.GetAsync<string>($"Project/{ActiveProjectId}/GetCodeSettings", token, "text/plain");
		}

		/// <inheritdoc cref="IRcsApiController.UpdateCodeSettings(List{RcsSetting}, CancellationToken)"/>
		public async Task<bool> UpdateCodeSettings(List<RcsSetting> setup, CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.UpdateCodeSettings projectId = {ActiveProjectId}");
			return await httpClient.PutAsync<bool>($"Project/{ActiveProjectId}/UpdateCodeSettings", setup, token);
		}

		/// <inheritdoc cref="IRcsApiController.GetLoadingInSectionAsync(int, CancellationToken)"/>
		public async Task<string> GetLoadingInSectionAsync(int sectionId, CancellationToken token = default)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetLoadingInSectionAsync projectId = {ActiveProjectId}, sectionId = {sectionId}");
			return await httpClient.GetAsync<string>($"Section/{ActiveProjectId}/GetLoadingInSection?sectionId={sectionId}", token, "text/plain");
		}

		/// <inheritdoc cref="IRcsApiController.SetLoadingInSectionAsync(int, string, CancellationToken)"/>
		public async Task SetLoadingInSectionAsync(int sectionId, string loadingXml, CancellationToken token = default)
		{
			var data = new RcsSectionLoading() { SectionId = sectionId, LoadingXml = loadingXml };
			pluginLogger.LogDebug($"RcsApiClient.SetLoadingInSectionAsync projectId = {ActiveProjectId} sectionId = {sectionId}");
			var result = await httpClient.PostAsync<string>($"Section/{ActiveProjectId}/SetLoadingInSection", data, token, "text/plain");
		}
	}
}
