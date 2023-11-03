using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.RcsClient.HttpWrapper;

namespace IdeaStatiCa.RcsClient.Client
{
	public class RcsApiClient : IRcsApiController
	{
		private readonly IHttpClientWrapper httpClient;
		private readonly int restApiProcessId;
		private readonly IPluginLogger pluginLogger;

		private Dictionary<string, ProjectResult> projectResults;

		public RcsApiClient(int processId, IPluginLogger logger, IHttpClientWrapper httpClientWrapper)
		{
			pluginLogger = logger;
			this.restApiProcessId = processId;
			this.httpClient = httpClientWrapper;
			projectResults = new Dictionary<string, ProjectResult>();
		}

		private ProjectResult GetProjectResult(RcsProjectInfo projectInfo)
		{
			if ((projectInfo.IdeaProjectPath != null && !projectResults.ContainsKey(projectInfo.IdeaProjectPath)) ||
				(projectInfo.ProjectName != null && !projectResults.ContainsKey(projectInfo.ProjectName)))
			{
				pluginLogger.LogInformation("Project not found in cache, calling RCS API");
				var projectTask = Task.Run(async () => await httpClient.PostAsync<ProjectResult>("LongCalculation", projectInfo, "application/xml"));

				var projectResult = projectTask.GetAwaiter().GetResult();

				if (!string.IsNullOrEmpty(projectInfo.IdeaProjectPath))
				{
					projectResults.Add(projectInfo.IdeaProjectPath, projectResult);
				}

				if (!string.IsNullOrEmpty(projectInfo.ProjectName))
				{
					projectResults.Add(projectInfo.ProjectName, projectResult);
				}

				return projectResult;
			}

			if (projectResults.ContainsKey(projectInfo.IdeaProjectPath))
			{
				return projectResults[projectInfo.IdeaProjectPath];
			}

			if (projectResults.ContainsKey(projectInfo.ProjectName))
			{
				return projectResults[projectInfo.ProjectName];
			}

			var msg = $"Project {projectInfo.IdeaProjectPath}, {projectInfo.ProjectName} was not found";
			pluginLogger.LogError(msg);
			throw new KeyNotFoundException(msg);
		}

		/// <summary>
		/// Gets the non conformity issues specified in request for given project
		/// Method checks first cache, after that calls the calculation
		/// </summary>
		/// <param name="projectInfo"></param>
		/// <returns>Collection of nonconformity issues</returns>
		public IEnumerable<NonConformityIssue> GetNonConformityIssues(RcsProjectInfo projectInfo)
		{
			return GetProjectResult(projectInfo).Issues;
		}

		/// <summary>
		/// Gets the section concrete check results specified in request for given project
		/// Method checks first cache, after that calls the calculation
		/// If no sections are specified in request, every section will be calculated
		/// </summary>
		/// <param name="projectInfo"></param>
		/// <returns>Collection of SectionConcrete check results</returns>
		public IEnumerable<SectionConcreteCheckResult> GetResultOnSections(RcsProjectInfo projectInfo)
		{
			return GetProjectResult(projectInfo).Sections;
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
	}
}
