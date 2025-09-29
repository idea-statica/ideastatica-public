using IdeaRS.OpenModel;
using IdeaStatiCa.Api.Connection.Model;
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
		private Guid activeProjectId;

		public ProjectApiExt(IConnectionApiClient connectionApiClient, IdeaStatiCa.ConnectionApi.Client.ISynchronousClient client, IdeaStatiCa.ConnectionApi.Client.IAsynchronousClient asyncClient, IdeaStatiCa.ConnectionApi.Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
		{
			this._connectionApiClient = connectionApiClient;
		}

		/// <inheritdoc cref="IConnectionApiClient.ProjectId"/>/>
		public Guid ProjectId
		{
			get => activeProjectId;
			private set => activeProjectId = value;
		}

		public async Task<ConProject> OpenProjectAsync(string path, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
			{
				using (var ms = new System.IO.MemoryStream())
				{
					await fs.CopyToAsync(ms);
					ms.Seek(0, System.IO.SeekOrigin.Begin);
					var conProject = await OpenProjectAsync(ms, 0, cancellationToken);
					this.ProjectId = conProject.ProjectId;
					return conProject;
				}
			}
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
				this.ProjectId = conProject.Data.ProjectId;
				return conProject.Data;
			}
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

	}
}
