using System.Threading.Tasks;

namespace IdeaStatica.Communication
{
	public interface IGrpcCommunicationCommonInterface
	{
		Task RunAsync(string id);
	}
}
