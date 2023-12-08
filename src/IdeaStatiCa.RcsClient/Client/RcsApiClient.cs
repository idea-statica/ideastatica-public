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

		public async Task<Guid> OpenProjectAsync(string path, CancellationToken token)
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

			var res = await httpClient.PostAsyncStream<Guid>("Project/OpenProject", streamContent);

			pluginLogger.LogDebug($"RcsApiClient.OpenProjectAsync projectId = {res}");

			return res;

		}

		public async Task<Guid> OpenProjectFromModelAsync(OpenModel model, CancellationToken token)
		{
			pluginLogger.LogDebug("RcsApiClient.OpenProjectFromModelAsync");
			var result =  await httpClient.PostAsync<Guid>("Project/OpenProjectFromModel", model);
			return result;
		}

		public async Task<ProjectResult> CalculateProjectAsync(Guid projectId, RcsCalculationParameters parameters, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.CalculateProjectAsync projectId = {projectId}");
			var res = await httpClient.PostAsync<ProjectResult>($"Calculations/{projectId}/CalculateResults", parameters, "application/xml");
			return res;
		}

		public async Task<RcsProjectModel> GetProjectOverviewAsync(Guid projectId, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectOverviewAsync projectId = {projectId}");
			var res=  await httpClient.GetAsync<RcsProjectModel>($"Project/{projectId}/ProjectOverview");
			return res;
		}

		public async Task<Stream> DownloadAsync(Guid projectId, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.Download projectId = {projectId}");
			var result = await httpClient.GetAsync<MemoryStream>($"Project/{projectId}/Download");
			return result;
		}

		public async Task<IEnumerable<RcsCrossSectionDetailModel>> SectionDetailsAsync(Guid projectId, RcsCalculationParameters parameters)
		{
			pluginLogger.LogDebug($"RcsApiClient.SectionDetailsAsync projectId = {projectId}");
			var result =await httpClient.PostAsync<IEnumerable<RcsCrossSectionDetailModel>>($"Calculations/{projectId}/SectionDetails", parameters);
			return result;
		}

		public async Task<IEnumerable<SectionConcreteCheckResult>> GetResultOnSectionsAsync(Guid projectId, RcsCalculationParameters parameters, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetResultOnSectionsAsync projectId = {projectId}");
			var result = await CalculateProjectAsync(projectId, parameters, token);

			return result.Sections;

		}

		public async Task<IEnumerable<NonConformityIssue>> GetNonConformityIssuesAsync(Guid projectId, RcsCalculationParameters parameters, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetNonConformityIssuesAsync projectId = {projectId}");
			var result =  await CalculateProjectAsync(projectId, parameters, token);
			return result.Issues;
		}

		public async Task<IList<RcsSectionModel>> GetProjectSectionsAsync(Guid projectId, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectSectionsAsync projectId = {projectId}");
			var result = await httpClient.GetAsync<IList<RcsSectionModel>>($"Project/{projectId}/ProjectSections");
			return result;
		}

		public async Task<IList<RcsCheckMemberModel>> GetProjectMembersAsync(Guid projectId, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectMembersAsync projectId = {projectId}");
			var result = await httpClient.GetAsync<IList<RcsCheckMemberModel>>($"Project/{projectId}/ProjectSections");
			return result;
		}

		public async Task<IList<ReinforcedCrossSectionModel>> GetProjectReinforcedCrossSectionsAsync(Guid projectId, CancellationToken token)
		{
			pluginLogger.LogDebug($"RcsApiClient.GetProjectReinforcedCrossSectionsAsync projectId = {projectId}");
			var result = await httpClient.GetAsync<IList<ReinforcedCrossSectionModel>>($"Project/{projectId}/ProjectReinforcedCrossSections");
			return result;
		}
	}
}
