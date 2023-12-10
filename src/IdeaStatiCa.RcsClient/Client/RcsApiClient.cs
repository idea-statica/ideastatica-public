using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
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
		public async Task<bool> OpenProjectAsync(string path, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.OpenProjectAsync path = '{path}'");

			var header = path switch
			{
				{ } when path.EndsWith(".IdeaRcs") => "application/octet-stream",
				{ } when path.EndsWith(".xml") => "application/xml",
				_ => throw new InvalidDataException("Non supported file type. Please send .IdeaRcs or IOM in .xml")
			};

			byte[] fileData = File.ReadAllBytes(path);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue(header);

			ActiveProjectId = await httpClient.PostAsyncStream<Guid>("Project/OpenProject", streamContent);

			pluginLogger.LogDebug($"RcsApiClient.OpenProjectAsync projectId = {ActiveProjectId}");

			return true;

		}
		/// <inheritdoc cref="IRcsApiController.OpenProjectFromModelAsync(OpenModel, CancellationToken) "/>
		public async Task<bool> OpenProjectFromModelAsync(OpenModel model, CancellationToken token)
		{
			pluginLogger.LogDebug("RcsApiClient.OpenProjectFromModelAsync");
			ActiveProjectId = await httpClient.PostAsync<Guid>("Project/OpenProjectFromModel", model);
			return true;
		}
		/// <inheritdoc cref="IRcsApiController.CalculateResultsAsync(RcsCalculationParameters, CancellationToken) "/>
		public async Task<List<RcsSectionResultOverview>> CalculateResultsAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.CalculateResultsAsync projectId = {ActiveProjectId}");
			var res = await httpClient.PostAsync<List<RcsSectionResultOverview>>($"Calculations/{ActiveProjectId}/CalculateResults", parameters, "application/xml");
			return res;
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectOverviewAsync(CancellationToken) "/>
		public async Task<RcsProjectModel> GetProjectOverviewAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectOverviewAsync projectId = {ActiveProjectId}");
			var res=  await httpClient.GetAsync<RcsProjectModel>($"Project/{ActiveProjectId}/ProjectOverview");
			return res;
		}
		/// <inheritdoc cref="IRcsApiController.DownloadAsync(CancellationToken) "/>
		public async Task<Stream> DownloadAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.Download projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<MemoryStream>($"Project/{ActiveProjectId}/Download");
			return result;
		}
		/// <inheritdoc cref="IRcsApiController.SectionDetailsAsync(RcsCalculationParameters) "/>
		public async Task<List<RcsCrossSectionDetailModel>> SectionDetailsAsync(RcsCalculationParameters parameters)
		{
			pluginLogger.LogDebug($"RcsApiClient.SectionDetailsAsync projectId = {ActiveProjectId}");
			var result =await httpClient.PostAsync<List<RcsCrossSectionDetailModel>>($"Calculations/{ActiveProjectId}/SectionDetails", parameters);
			return result;
		}

		/// <inheritdoc cref="IRcsApiController.GetResultsAsync(RcsCalculationParameters, CancellationToken)"/>
		public async Task<ProjectResult> GetResultsAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			return await httpClient.PostAsync<ProjectResult>($"Calculations/{ActiveProjectId}/GetResults", parameters, "application/xml");
		}

		public async Task<List<SectionConcreteCheckResult>> GetResultOnSectionsAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetResultOnSectionsAsync projectId = {ActiveProjectId}");
			var result = await CalculateProjectAsync(parameters, token);
			return result.Sections;
		}


		/// <inheritdoc cref="IRcsApiController.GetNonConformityIssuesAsync(RcsCalculationParameters, CancellationToken) "/>
		public async Task<List<NonConformityIssue>> GetNonConformityIssuesAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetNonConformityIssuesAsync projectId = {ActiveProjectId}");
			var result =  await CalculateProjectAsync(parameters, token);
			return result.Issues;
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectSectionsAsync(CancellationToken)  "/>
		public async Task<List<RcsSectionModel>> GetProjectSectionsAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectSectionsAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<List<RcsSectionModel>>($"Project/{ActiveProjectId}/ProjectSections");
			return result;
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectMembersAsync(CancellationToken) "/>
		public async Task<List<RcsCheckMemberModel>> GetProjectMembersAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectMembersAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<List<RcsCheckMemberModel>>($"Project/{ActiveProjectId}/ProjectSections");
			return result;
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectReinforcedCrossSectionsAsync(CancellationToken) "/>
		public async Task<List<ReinforcedCrossSectionModel>> GetProjectReinforcedCrossSectionsAsync(CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectReinforcedCrossSectionsAsync projectId = {ActiveProjectId}");
			var result = await httpClient.GetAsync<List<ReinforcedCrossSectionModel>>($"Project/{ActiveProjectId}/ProjectReinforcedCrossSections");
			return result;
		}

		public async Task<RcsSectionModel> SetReinforcementAsync(int sectionId, int reinforcedSectionId)
		{
			pluginLogger.LogDebug($"RcsApiClient.SetReinforcementAsync projectId = {ActiveProjectId} sectionId = {sectionId} reinforcedSectionId = {reinforcedSectionId}");

			RcsSectionModel sectionModel = new RcsSectionModel() { Id = sectionId, RCSId= reinforcedSectionId };
			var result = await httpClient.PutAsync<RcsSectionModel>($"Section/{ActiveProjectId}/SetReinforcedSection", sectionModel);
			return result;
		}

		private async Task<ProjectResult> CalculateProjectAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			return await httpClient.PostAsync<ProjectResult>($"Calculations/{ActiveProjectId}/CalculateResults", parameters, "application/xml");
		}
	}
}
