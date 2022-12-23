using Grpc.Core;
using IdeaStatiCa.Plugin.Grpc;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.IdeaComunication
{
	public class IdeaComunicationServer : IIdeaComunicationServer
	{
		private GrpcServer server;

		public IdeaComunicationServer(IPluginLogger logger, int maxDataLength = Constants.GRPC_MAX_MSG_SIZE)
		{
			server = new GrpcServer(logger, maxDataLength);
		}

		public void Connect(string clientId, int port)
		{
			server.Connect(clientId, port);
		}

		public async Task ConnectAsync(IAsyncStreamReader<GrpcMessage> requestStream, IServerStreamWriter<GrpcMessage> responseStream, ServerCallContext context)
		{
			await server.ConnectAsync(requestStream, responseStream, context);
		}

		public async Task DisconnectAsync()
		{
			await server.DisconnectAsync();
		}

		public void RegisterHandler(string handlerId, IGrpcMessageHandler handler)
		{
			server.RegisterHandler(handlerId, handler);
		}

		public async Task SendMessageAsync(GrpcMessage message)
		{
			await server.SendMessageAsync(message);
		}

		public async Task StartAsync()
		{
			await server.StartAsync();
		}
	}
}
