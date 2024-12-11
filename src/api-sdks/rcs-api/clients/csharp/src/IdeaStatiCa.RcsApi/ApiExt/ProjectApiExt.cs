using IdeaStatiCa.Api.RCS.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsApi.Api
{
	/// <summary>
	/// 
	/// </summary>

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
		Task<RcsProject> OpenProjectAsync(string filePath, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

		Task SaveProjectAsync(Guid projectId, string fileName, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="iomFilePath"></param>
		/// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
		/// <returns></returns>
		Task<RcsProject> CreateProjectFromIomFileAsync(string iomFilePath, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
	}

	/// <summary>
	/// 
	/// </summary>
	public class ProjectApiExt : ProjectApi, IProjectApiExtAsync
	{
		private readonly IRcsApiClient rcsApiClient;
		private Guid activeProjectId;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rcsApiClient"></param>
		/// <param name="client"></param>
		/// <param name="asyncClient"></param>
		/// <param name="configuration"></param>
		public ProjectApiExt(IRcsApiClient rcsApiClient, IdeaStatiCa.RcsApi.Client.ISynchronousClient client, IdeaStatiCa.RcsApi.Client.IAsynchronousClient asyncClient, IdeaStatiCa.RcsApi.Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
		{
			
			this.rcsApiClient = rcsApiClient;
		}

		/// <inheritdoc cref="IRcsApiClient.ProjectId"/>/>
		public Guid ProjectId
		{
			get => activeProjectId;
			private set => activeProjectId = value;
		}

		/// <inheritdoc cref="IProjectApiExtAsync.OpenProjectAsync(string, System.Threading.CancellationToken)"/>
		public async Task<RcsProject> OpenProjectAsync(string path, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
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

		public async Task<RcsProject> CreateProjectFromIomFileAsync(string path, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
			{
				using (var ms = new System.IO.MemoryStream())
				{
					await fs.CopyToAsync(ms);
					ms.Seek(0, System.IO.SeekOrigin.Begin);

					var rcsProject = await ImportIOMFileAsync(ms, 0, cancellationToken);
					this.ProjectId = rcsProject.ProjectId;
					return rcsProject;
				}
			}
		}
	}
}
