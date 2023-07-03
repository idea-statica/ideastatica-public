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


		Task<ConnectionCheckRes> CalculateConnectionAsync(int connectionId, CancellationToken cancellationToken);
	}
}
