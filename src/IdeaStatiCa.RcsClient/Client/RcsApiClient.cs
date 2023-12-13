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

			var header = path switch
			{
				{ } when path.EndsWith(".IdeaRcs", StringComparison.InvariantCultureIgnoreCase) => "application/octet-stream",
				{ } when path.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase) => "application/xml",
				_ => throw new InvalidDataException("Non supported file type. Please send .IdeaRcs or IOM in .xml")
			};

			byte[] fileData = File.ReadAllBytes(path);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue(header);

			ActiveProjectId = await httpClient.PostAsyncStream<Guid>("Project/OpenProject", streamContent, token);

			pluginLogger.LogDebug($"RcsApiClient.OpenProjectAsync projectId = {ActiveProjectId}");

			return true;

		}
		/// <inheritdoc cref="IRcsApiController.OpenProjectFromModelAsync(OpenModel, CancellationToken) "/>
		public async Task<bool> OpenProjectFromModelAsync(OpenModel model, CancellationToken token)
		{
			pluginLogger.LogDebug("RcsApiClient.OpenProjectFromModelAsync");
			ActiveProjectId = await httpClient.PostAsync<Guid>("Project/OpenProjectFromModel", model, token);
			return true;
		}
		/// <inheritdoc cref="IRcsApiController.CalculateResultsAsync(RcsCalculationParameters, CancellationToken) "/>
		public async Task<List<RcsSectionResultOverview>> CalculateResultsAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.CalculateResultsAsync projectId = {ActiveProjectId}");
			try
			{
				var res = await httpClient.PostAsync<List<RcsSectionResultOverview>>($"Calculations/{ActiveProjectId}/CalculateResults", parameters, token, "application/xml");
				return res;
			}
			catch(OperationCanceledException ex)
			{
				pluginLogger.LogDebug($"{ex.Message}");
				throw ex;
			}
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectOverviewAsync(CancellationToken) "/>
		public async Task<RcsProjectModel> GetProjectOverviewAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectOverviewAsync projectId = {ActiveProjectId}");
			var res=  await httpClient.GetAsync<RcsProjectModel>($"Project/{ActiveProjectId}/ProjectOverview", token);
			return res;
		}
		/// <inheritdoc cref="IRcsApiController.DownloadAsync(CancellationToken) "/>
		public async Task<Stream> DownloadAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.Download projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<MemoryStream>($"Project/{ActiveProjectId}/Download", token, "application/octet-stream");
			return result;
		}
		/// <inheritdoc cref="IRcsApiController.SectionDetailsAsync(RcsCalculationParameters, CancellationToken) "/>
		public async Task<List<RcsCrossSectionDetailModel>> SectionDetailsAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.SectionDetailsAsync projectId = {ActiveProjectId}");
			var result =await httpClient.PostAsync<List<RcsCrossSectionDetailModel>>($"Calculations/{ActiveProjectId}/SectionDetails", parameters, token);
			return result;
		}

		/// <inheritdoc cref="IRcsApiController.GetResultsAsync(RcsCalculationParameters, CancellationToken)"/>
		public async Task<ProjectResult> GetResultsAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			return await httpClient.PostAsync<ProjectResult>($"Calculations/{ActiveProjectId}/GetResults", parameters, token, "application/xml");
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
		public async Task<RcsSectionModel> ImportReinfCssAsync(ReinfCssImportSetting importSetting, string reinfCssTemplate, CancellationToken token)
		{
			var data = new ReinfCssImportData(){Setting = importSetting, Template = reinfCssTemplate };
			pluginLogger.LogDebug($"RcsApiClient.ImportReinfCssAsync projectId = {ActiveProjectId} reinfCssId = {importSetting?.ReinfCssId}");
			var result = await httpClient.PostAsync<RcsSectionModel>($"Section/{ActiveProjectId}/ImportReinfCss", data, token);
			return result;
		}

		private async Task<ProjectResult> CalculateProjectAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			return await httpClient.PostAsync<ProjectResult>($"Calculations/{ActiveProjectId}/CalculateResults", parameters, token, "application/xml");
		}
	}
}
