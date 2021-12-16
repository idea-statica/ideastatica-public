using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	/// <summary>
	/// Handles communication over Grpc.
	/// </summary>
	public interface IGrpcMessageHandler<T> : IGrpcMessageHandler where T : class
	{
		/// <summary>
		/// Handles request incoming from the client.
		/// </summary>
		/// <param name="message">Message sent by client.</param>
		/// <returns></returns>
		new Task<T> HandleServerMessage(GrpcMessage message, IGrpcSender server);

		/// <summary>
		/// Handles response incoming from the server.
		/// </summary>
		/// <param name="message">Message sent by server.</param>
		/// <returns></returns>
		new Task<T> HandleClientMessage(GrpcMessage message, IGrpcSender client);
	}

	/// <summary>
	/// Non generic implementation of <see cref="IGrpcMessageHandler{T}"/>
	/// </summary>
	public interface IGrpcMessageHandler
	{
		/// <summary>
		/// Handles request incoming from the client.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="grpcSender"></param>
		/// <returns></returns>
		Task<object> HandleServerMessage(GrpcMessage message, IGrpcSender grpcSender);

		/// <summary>
		/// Handles response incoming from the server.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="grpcSender"></param>
		/// <returns></returns>
		Task<object> HandleClientMessage(GrpcMessage message, IGrpcSender grpcSender);
	}
}
