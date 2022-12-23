using IdeaStatiCa.Plugin.Grpc;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.IdeaComunication
{
	public interface IIdeaComunication
	{
		Task StartAsync();

		void Connect(string clientId, int port);

		void RegisterHandler(string handlerId, IGrpcMessageHandler handler);

		Task DisconnectAsync();

		Task SendMessageAsync(GrpcMessage message);
	}
}
