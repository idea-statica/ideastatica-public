using Grpc.Core;
using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Grpc
{
	internal class CommunicationTools
	{
		/// <summary>
		/// Createsoptions for gRPC communication
		/// </summary>
		/// <param name="bufferSize">The maximal size of GrpcMessage.data in grpc message</param>
		/// <returns></returns>
		internal static List<ChannelOption> GetChannelOptions(int bufferSize = Constants.GRPC_MAX_MSG_SIZE)
		{
			var res = new List<ChannelOption>() {
						new ChannelOption(ChannelOptions.MaxReceiveMessageLength, bufferSize),
						new ChannelOption(ChannelOptions.MaxSendMessageLength, bufferSize)
				};
			return res;
		}
	}
}