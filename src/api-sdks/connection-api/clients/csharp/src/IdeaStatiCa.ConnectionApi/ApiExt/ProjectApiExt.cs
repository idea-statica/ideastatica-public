using IdeaStatiCa.ConnectionApi.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi.Api
{
	public interface IProjectApiExtAsync : IProjectApiAsync
	{
		/// <summary>
		/// 
		/// </summary>
		Guid ProjectId { get; }

		/// <summary>
		/// 
		/// </summary>
		ConProject ActiveProjectData { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
		/// <returns></returns>
		Task<ConProject> OpenProjectAsync(string filePath, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

		Task SaveProjectAsync(Guid projectId, string fileName, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="iomFilePath"></param>
		/// <param name="connectionsToCreate"></param>
		/// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
		/// <returns></returns>
		Task<ConProject> CreateProjectFromIomFileAsync(string iomFilePath, List<int> connectionsToCreate = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

		Task<ConProject> UpdateProjectFromIomFileAsync(Guid projectId, string iomFilePath, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
	}

	/// <summary>
	/// 
	/// </summary>
	public class ProjectApiExt : ProjectApi, IProjectApiExtAsync
	{
		private readonly IConnectionApiClient _connectionApiClient;

		public ProjectApiExt(IConnectionApiClient connectionApiClient, IdeaStatiCa.ConnectionApi.Client.ISynchronousClient client, IdeaStatiCa.ConnectionApi.Client.IAsynchronousClient asyncClient, IdeaStatiCa.ConnectionApi.Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
		{
			this._connectionApiClient = connectionApiClient;
		}

		/// <inheritdoc cref="IConnectionApiClient.ProjectId"/>/>
		public Guid ProjectId
		{
			get => ActiveProjectData == null ? Guid.Empty : ActiveProjectData.ProjectId;
		}

		/// <summary>
		/// Data about the active project
		/// </summary>
		public ConProject ActiveProjectData { get; private set; } = null;

		public async Task<ConProject> OpenProjectAsync(string path, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{
			//await CreateAsync();

			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
			{
				using (var ms = new System.IO.MemoryStream())
				{
					await fs.CopyToAsync(ms);
					ms.Seek(0, System.IO.SeekOrigin.Begin);
					var conProject = await OpenProjectAsync(ms, 0, cancellationToken);
					this.ActiveProjectData = conProject;
				}
			}

			return this.ActiveProjectData;
		}

		public async Task SaveProjectAsync(Guid projectId, string fileName, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{
			var response = await base.DownloadProjectWithHttpInfoAsync(projectId, "application/octet-stream", 0, cancellationToken);
			byte[] buffer = (byte[])response.Data;
			using (var fileStream = System.IO.File.Create(fileName))
			{
				await fileStream.WriteAsync(buffer, 0, buffer.Length);
			}
		}

		public async Task<ConProject> CreateProjectFromIomFileAsync(string fileName, List<int> connectionsToCreate = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{
			string xmlString = string.Empty;
#if NETSTANDARD2_1_OR_GREATER
			xmlString = await System.IO.File.ReadAllTextAsync(fileName);
#else
			xmlString = System.IO.File.ReadAllText(fileName);

#endif
			xmlString = xmlString.Replace("utf-16", "utf-8");

			using (var memoryStream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
			{
				memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

				var conProject = await ImportIOMWithHttpInfoAsync(memoryStream, connectionsToCreate, null, 0, cancellationToken);
				this.ActiveProjectData = conProject.Data;
			}

			return this.ActiveProjectData;
		}

		public async Task<ConProject> UpdateProjectFromIomFileAsync(Guid projectId, string iomFilePath, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{
			string xmlString = string.Empty;
#if NETSTANDARD2_1_OR_GREATER
			xmlString = await System.IO.File.ReadAllTextAsync(iomFilePath);
#else
			xmlString = System.IO.File.ReadAllText(iomFilePath);

#endif
			xmlString = xmlString.Replace("utf-16", "utf-8");

			using (var iomStream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
			{
				iomStream.Seek(0, System.IO.SeekOrigin.Begin);

				var response = await UpdateFromIOMAsync(projectId, iomStream, 0, cancellationToken);

				return response;
			}
		}

		/// <inheritdoc cref="ProjectApi.GetSetupAsync(Guid, int, System.Threading.CancellationToken)"/>
		public new async System.Threading.Tasks.Task<ConnectionSetup> GetSetupAsync(Guid projectId, int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{
				var response = await GetSetupWithHttpInfoAsync(projectId, null, 0, cancellationToken);
				var setup = JsonConvert.DeserializeObject<ConnectionSetup>(response.RawContent, IdeaJsonSerializerSetting.GetJsonSettingIdea());
				return setup;
		}

		/// <inheritdoc cref="ProjectApi.UpdateSetupAsync(Guid, ConnectionSetup, int, System.Threading.CancellationToken)"/>
		public new async System.Threading.Tasks.Task<ConnectionSetup> UpdateSetupAsync(Guid projectId, ConnectionSetup connectionSetup = default(ConnectionSetup), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{

			IdeaStatiCa.ConnectionApi.Client.RequestOptions localVarRequestOptions = new IdeaStatiCa.ConnectionApi.Client.RequestOptions();

			var localVarContentType = "application/json";

			var localVarAccept = "application/json";

			localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

			localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

			localVarRequestOptions.PathParameters.Add("projectId", IdeaStatiCa.ConnectionApi.Client.ClientUtils.ParameterToString(projectId)); // path parameter
			localVarRequestOptions.Data = connectionSetup;

			localVarRequestOptions.Operation = "ProjectApi.UpdateSetup";
			localVarRequestOptions.OperationIndex = operationIndex;

			localVarRequestOptions.Data = JsonConvert.SerializeObject(connectionSetup, IdeaJsonSerializerSetting.GetJsonSettingIdea());

			// make the HTTP request
			var localVarResponse = await this.AsynchronousClient.PutAsync<ConnectionSetup>("/api/1/projects/{projectId}/connection-setup", localVarRequestOptions, this.Configuration, cancellationToken);

			if (this.ExceptionFactory != null)
			{
				Exception _exception = this.ExceptionFactory("UpdateSetup", localVarResponse);
				if (_exception != null)
				{
					throw _exception;
				}
			}

			var setup = JsonConvert.DeserializeObject<ConnectionSetup>(localVarResponse.RawContent, IdeaJsonSerializerSetting.GetJsonSettingIdea());
			return setup;
		}
	}
}
