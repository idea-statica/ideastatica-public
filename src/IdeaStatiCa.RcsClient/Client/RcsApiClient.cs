using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
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
		private readonly IHttpClientWrapper _httpClient;
		private readonly int _restApiProcessId;
		private readonly IPluginLogger _pluginLogger;

		private Dictionary<string, ProjectResult> _projectResults;

		public RcsApiClient(int processId, IPluginLogger logger, IHttpClientWrapper httpClientWrapper)
		{
			_pluginLogger = logger;
			_restApiProcessId = processId;
			_httpClient = httpClientWrapper;
			_projectResults = new Dictionary<string, ProjectResult>();
		}

		private ProjectResult GetProjectResult(RcsProjectInfo projectInfo)
		{
			if ((projectInfo.IdeaProjectPath != null && !_projectResults.ContainsKey(projectInfo.IdeaProjectPath)) ||
				(projectInfo.ProjectName != null && !_projectResults.ContainsKey(projectInfo.ProjectName)))
			{
				_pluginLogger.LogInformation("Project not found in cache, calling RCS API");
				var projectTask = Task.Run(async () => await _httpClient.PostAsync<ProjectResult>("LongCalculation", projectInfo, "application/xml"));

				var projectResult = projectTask.GetAwaiter().GetResult();

				if (!string.IsNullOrEmpty(projectInfo.IdeaProjectPath))
				{
					_projectResults.Add(projectInfo.IdeaProjectPath, projectResult);
				}

				if (!string.IsNullOrEmpty(projectInfo.ProjectName))
				{
					_projectResults.Add(projectInfo.ProjectName, projectResult);
				}

				return projectResult;
			}

			if (_projectResults.ContainsKey(projectInfo.IdeaProjectPath))
			{
				return _projectResults[projectInfo.IdeaProjectPath];
			}

			if (_projectResults.ContainsKey(projectInfo.ProjectName))
			{
				return _projectResults[projectInfo.ProjectName];
			}

			var msg = $"Project {projectInfo.IdeaProjectPath}, {projectInfo.ProjectName} was not found";
			_pluginLogger.LogError(msg);
			throw new KeyNotFoundException(msg);
		}

		public IEnumerable<NonConformityIssue> GetNonConformityIssues(RcsProjectInfo projectInfo)
		{
			return GetProjectResult(projectInfo).Issues;
		}

		public IEnumerable<SectionConcreteCheckResult> GetResultOnSections(RcsProjectInfo projectInfo)
		{
			return GetProjectResult(projectInfo).Sections;
		}

		public void Dispose()
		{
			var restApiProcess = Process.GetProcessById(_restApiProcessId);
			if (restApiProcess is { })
			{
				if (!restApiProcess.HasExited)
				{
					_pluginLogger.LogInformation($"Cleaning the API process with ID {_restApiProcessId}");
					restApiProcess.Kill();
				}
			}
		}
	}
}
