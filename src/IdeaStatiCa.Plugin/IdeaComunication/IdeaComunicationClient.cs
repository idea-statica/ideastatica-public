using IdeaStatiCa.Plugin.Grpc;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.IdeaComunication
{
	public class IdeaComunicationClient : IIdeaComunicationClient
	{
		private GrpcClient client;

		public IdeaComunicationClient(IPluginLogger logger, int maxDataLength = Constants.GRPC_MAX_MSG_SIZE)
		{
			client = new GrpcClient(logger, maxDataLength);
		}
		public void Connect(string clientId, int port)
		{
			client.Connect(clientId, port);
		}

		public async Task DisconnectAsync()
		{
			await client.DisconnectAsync();
		}

		public void RegisterHandler(string handlerId, IGrpcMessageHandler handler)
		{
			client.RegisterHandler(handlerId, handler);
		}

		public async Task SendMessageAsync(GrpcMessage message)
		{
			await client.SendMessageAsync(message);
		}

		public async Task StartAsync()
		{
			await client.StartAsync();
		}
	}
}
