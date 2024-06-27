using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest
{
	public interface IConnectionApiController : IDisposable
	{
		/// <summary>
		/// Open idea project in the service
		/// </summary>
		/// <param name="ideaConProject">Idea Connection project filename</param>
		/// <param name="cancellationToken"></param>
		Task<ConProject> OpenProjectAsync(string ideaConProject, CancellationToken cancellationToken = default);

		/// <summary>
		/// Cloase the active project
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task CloseProjectAsync(CancellationToken cancellationToken = default);

		Task<ConProjectData> GetProjectDataAsync(CancellationToken cancellationToken = default);

		Task<List<ConConnection>> GetConnectionsAsync(CancellationToken token = default);

		Task<ConConnection> GetConnectionAsync(int connectionId, CancellationToken token = default);
		Task<Stream> DownloadProjectAsync(CancellationToken token = default);

		Task<ConConnection> UpdateConnectionAsync(int connectionId, ConConnection connectionUpdate, CancellationToken cancellationToken = default);
	}
}
