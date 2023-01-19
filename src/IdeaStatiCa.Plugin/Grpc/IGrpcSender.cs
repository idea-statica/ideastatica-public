using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	public interface IGrpcSender
	{
		Task SendMessageAsync(GrpcMessage message);
	}
}
