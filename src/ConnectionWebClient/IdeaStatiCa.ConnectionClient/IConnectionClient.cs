using IdeaRS.OpenModel.Connection;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient
{
	public interface IConnectionClient
	{
		Task<ConProjectInfo> OpenProjectAsync(Stream ideaConProject, CancellationToken cancellationToken);

		Task CloseProjectAsync(CancellationToken cancellationToken);

		Task<ConnectionCheckRes> GetPlasticBriefResultsAsync(int connectionId, CancellationToken cancellationToken);

		Task<string> GetPlasticDetailResultsJsonAsync(int connectionId, CancellationToken cancellationToken);

		Task<ConnectionCheckRes> GetBucklingBriefResultsAsync(int connectionId, CancellationToken cancellationToken);

		Task<string> GetBucklingDetailResultsJsonAsync(int connectionId, CancellationToken cancellationToken);
	}
}
