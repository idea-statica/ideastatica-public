using Grpc.Core;
using IdeaStatiCa.Plugin.Grpc;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.IdeaComunication
{
	public interface IIdeaComunicationServer : IIdeaComunication
	{
		Task ConnectAsync(IAsyncStreamReader<GrpcMessage> requestStream, IServerStreamWriter<GrpcMessage> responseStream, ServerCallContext context);
	}
}
