using Grpc.Core;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	public interface IGrpcService : IGrpcSender
	{
		bool IsConnected { get; }
		void RegisterHandler(string handlerId, IGrpcMessageHandler handler);
	}
}
