using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaStatiCa.RcsClient.HttpWrapper;
using Newtonsoft.Json;

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
			var header = path switch
			{
				{ } when path.EndsWith(".IdeaRcs") => "application/octet-stream",
				{ } when path.EndsWith(".xml") => "application/xml",
				_ => throw new InvalidDataException("Non supported file type. Please send .IdeaRcs or IOM in .xml")
			};

			byte[] fileData = File.ReadAllBytes(path);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue(header);

			return await httpClient.PostAsyncStream<Guid>("Project/OpenProject", streamContent);

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

		public async Task<RcsProjectModel> GetProjectOverviewAsync(Guid projectId, CancellationToken token)
		{
			return  await httpClient.GetAsync<RcsProjectModel>($"Project/{projectId}/ProjectOverview");
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

		public IList<RcsSectionModel> GetProjectSections(Guid projectId, CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<IList<RcsSectionModel>>($"Project/{projectId}/ProjectSections"));
			return result.GetAwaiter().GetResult();
		}

		public IList<RcsCheckMemberModel> GetProjectMembers(Guid projectId, CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<IList<RcsCheckMemberModel>>($"Project/{projectId}/ProjectSections"));
			return result.GetAwaiter().GetResult();
		}

		public IList<ReinforcedCrossSectionModel> GetProjectReinforcedCrossSections(Guid projectId, CancellationToken token)
		{
			var result = Task.Run(async () => await httpClient.GetAsync<IList<ReinforcedCrossSectionModel>>($"Project/{projectId}/ProjectReinforcedCrossSections"));
			return result.GetAwaiter().GetResult();
		}
	}
}
