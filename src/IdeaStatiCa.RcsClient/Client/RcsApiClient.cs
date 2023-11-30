using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

		public Guid OpenProject(RcsProjectInfo project, CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.PostAsync<Guid> ("Project/OpenProject", project));
			return result.GetAwaiter().GetResult();
		}

		public Guid OpenProjectFromModel(OpenModel model, CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.PostAsync<Guid>("Project/OpenProjectFromModel", model));
			return result.GetAwaiter().GetResult();
		}

		public async Task<ProjectResult> CalculateProjectAsync(Guid projectId, RcsCalculationParameters parameters, CancellationToken token)
		{
			return await httpClient.PostAsync<ProjectResult>($"Calculations/{projectId}/CalculateResults", parameters, "application/xml");
		}

		public RcsModelOverview GetProjectOverview(Guid projectId, CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<RcsModelOverview>($"Project/{projectId}/ProjectOverview"));
			return result.GetAwaiter().GetResult();
		}

		public MemoryStream Download(Guid projectId, CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<MemoryStream>($"Project/{projectId}/Download"));
			return result.GetAwaiter().GetResult();
		}

		public IEnumerable<RcsCrossSectionDetailModel> SectionDetails(Guid projectId, RcsCalculationParameters parameters)
		{
			var result = Task.Run(async () => await httpClient.PostAsync<IEnumerable<RcsCrossSectionDetailModel>>($"Calculations/{projectId}/SectionDetails", parameters));
			return result.GetAwaiter().GetResult();
		}

		public IEnumerable<SectionConcreteCheckResult> GetResultOnSections(Guid projectId, RcsCalculationParameters parameters, CancellationToken token)
		{
			var calculationTask = Task.Run(async () => await CalculateProjectAsync(projectId, parameters, token));
			var result = calculationTask.GetAwaiter().GetResult();

			return result.Sections;

		}

		public IEnumerable<NonConformityIssue> GetNonConformityIssues(Guid projectId, RcsCalculationParameters parameters, CancellationToken token)
		{
			var calculationTask = Task.Run(async () => await CalculateProjectAsync(projectId, parameters, token));
			var result = calculationTask.GetAwaiter().GetResult();

			return result.Issues;
		}
	}
}
