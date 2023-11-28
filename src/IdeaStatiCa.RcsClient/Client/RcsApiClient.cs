using System.Collections.Generic;
using System.Diagnostics;
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
		/// Gets the non conformity issues specified in request for given project
		/// Method checks first cache, after that calls the calculation
		/// </summary>
		/// <param name="projectInfo"></param>
		/// <returns>Collection of nonconformity issues</returns>
		public IEnumerable<NonConformityIssue> GetNonConformityIssues(RcsProjectInfo projectInfo, CancellationToken token)
		{
			var calculationTask = Task.Run(async () => await httpClient.PostAsync<ProjectResult>("Calculations/CalculateResults", projectInfo, "application/xml"));
			var result = calculationTask.GetAwaiter().GetResult();

			return result.Issues;
		}

		/// <summary>
		/// Gets the section concrete check results specified in request for given project
		/// Method checks first cache, after that calls the calculation
		/// If no sections are specified in request, every section will be calculated
		/// </summary>
		/// <param name="projectInfo"></param>
		/// <returns>Collection of SectionConcrete check results</returns>
		public IEnumerable<SectionConcreteCheckResult> GetResultOnSections(RcsProjectInfo projectInfo, CancellationToken token)
		{
			var calculationTask = Task.Run(async () => await httpClient.PostAsync<ProjectResult>("Calculations/CalculateResults", projectInfo, "application/xml"));
			var result = calculationTask.GetAwaiter().GetResult();

			return result.Sections;
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

		public async Task<ProjectResult> CalculateProjectAsync(RcsProjectInfo projectInfo, CancellationToken token)
		{
			//use the cancellation token
			return await httpClient.PostAsync<ProjectResult>("Calculations/CalculateResults", projectInfo, "application/xml");
		}

		public RcsModelOverview GetProjectOverview(RcsProjectInfo projectInfo, CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.PostAsync<RcsModelOverview>("Project/ProjectOverview", projectInfo));
			return result.GetAwaiter().GetResult();
		}

		public IEnumerable<RcsCrossSectionDetailModel> GetSectionDetails(RcsProjectInfo projectInfo)
		{
			var result = Task.Run(async () => await httpClient.PostAsync<IEnumerable<RcsCrossSectionDetailModel>>("Calculations/SectionDetails", projectInfo));
			return result.GetAwaiter().GetResult();
		}

		public async Task<ProjectResult> CalculateProjectOpenModelAsync(OpenModel projectOpenModel, CancellationToken token)
		{
			return await httpClient.PostAsync<ProjectResult>("Calculations/CalculateResultsOpenModel", projectOpenModel, "application/xml");
		}
	}
}
