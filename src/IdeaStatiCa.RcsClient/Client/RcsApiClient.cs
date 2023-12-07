using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaStatiCa.RcsClient.HttpWrapper;

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
		/// <inheritdoc cref="IRcsApiController.OpenProject(string, CancellationToken)"/>
		public bool OpenProject(string path, CancellationToken token)
		{
			var header = path switch
			{
				{ } when path.EndsWith(".IdeaRcs") => "application/octet-stream",
				{ } when path.EndsWith(".xml") => "application/xml",
				_ => throw new InvalidDataException("Non supported file type. Please send .IdeaRcs or IOM in .xml")
			};

			byte[] fileData = File.ReadAllBytes(path);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue(header);

			var result = Task.Run(async () => await httpClient.PostAsyncStream<Guid>("Project/OpenProject", streamContent));
			ActiveProjectId = result.GetAwaiter().GetResult();

			return true;
		}
		/// <inheritdoc cref="IRcsApiController.OpenProjectFromModel(OpenModel, CancellationToken)"/>
		public bool OpenProjectFromModel(OpenModel model, CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.PostAsync<Guid>("Project/OpenProjectFromModel", model));
			ActiveProjectId = result.GetAwaiter().GetResult();
			return true;
		}
		/// <inheritdoc cref="IRcsApiController.CalculateResultsAsync(RcsCalculationParameters, CancellationToken)"/>
		public async Task<List<RcsSectionResultOverview>> CalculateResultsAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			return await httpClient.PostAsync<List<RcsSectionResultOverview>>($"Calculations/{ActiveProjectId}/CalculateResults", parameters, "application/xml");
		}
		/// <inheritdoc cref="IRcsApiController.GetResultsAsync(RcsCalculationParameters, CancellationToken)"/>
		public async Task<ProjectResult> GetResultsAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			return await httpClient.PostAsync<ProjectResult>($"Calculations/{ActiveProjectId}/GetResults", parameters, "application/xml");
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectOverview(CancellationToken)"/>
		public RcsModelOverview GetProjectOverview(CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<RcsModelOverview>($"Project/{ActiveProjectId}/ProjectOverview"));
			return result.GetAwaiter().GetResult();
		}
		/// <inheritdoc cref="IRcsApiController.Download(CancellationToken)"/>
		public MemoryStream Download(CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<MemoryStream>($"Project/{ActiveProjectId}/Download"));
			return result.GetAwaiter().GetResult();
		}
		/// <inheritdoc cref="IRcsApiController.SectionDetails(RcsCalculationParameters)"/>
		public List<RcsCrossSectionDetailModel> SectionDetails(RcsCalculationParameters parameters)
		{
			var result = Task.Run(async () => await httpClient.PostAsync<List<RcsCrossSectionDetailModel>>($"Calculations/{ActiveProjectId}/SectionDetails", parameters));
			return result.GetAwaiter().GetResult();
		}
		/// <inheritdoc cref="IRcsApiController.GetResultOnSections(RcsCalculationParameters, CancellationToken)"/>
		public List<SectionConcreteCheckResult> GetResultOnSections(RcsCalculationParameters parameters, CancellationToken token)
		{
			var calculationTask = Task.Run(async () => await CalculateProjectAsync(parameters, token));
			var result = calculationTask.GetAwaiter().GetResult();
			return result.Sections;
		}
		/// <inheritdoc cref="IRcsApiController.GetNonConformityIssues(RcsCalculationParameters, CancellationToken)"/>
		public List<NonConformityIssue> GetNonConformityIssues(RcsCalculationParameters parameters, CancellationToken token)
		{
			var calculationTask = Task.Run(async () => await CalculateProjectAsync(parameters, token));
			var result = calculationTask.GetAwaiter().GetResult();
			return result.Issues;
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectSections(CancellationToken)"/>
		public List<RcsCrossSectionOverviewModel> GetProjectSections(CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<List<RcsCrossSectionOverviewModel>>($"Project/{ActiveProjectId}/ProjectSections"));
			return result.GetAwaiter().GetResult();
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectMembers(CancellationToken)"/>
		public List<RcsCheckMemberModel> GetProjectMembers(CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<List<RcsCheckMemberModel>>($"Project/{ActiveProjectId}/ProjectSections"));
			return result.GetAwaiter().GetResult();
		}
		/// <inheritdoc cref="IRcsApiController.GetProjectReinforcedCrossSections(CancellationToken token)"/>
		public List<RcsReinforcedCrossSectionModel> GetProjectReinforcedCrossSections(CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<List<RcsReinforcedCrossSectionModel>>($"Project/{ActiveProjectId}/ProjectReinforcedCrossSections"));
			return result.GetAwaiter().GetResult();
		}
		private async Task<ProjectResult> CalculateProjectAsync(RcsCalculationParameters parameters, CancellationToken token)
		{
			return await httpClient.PostAsync<ProjectResult>($"Calculations/{ActiveProjectId}/CalculateResults", parameters, "application/xml");
		}
	}
}