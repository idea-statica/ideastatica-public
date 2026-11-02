using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	public interface IGrpcCommunicator
	{
		/// <summary>
		/// Port on which the communicator will communicate.
		/// </summary>
		int Port { get; }

		/// <summary>
		/// The host
		/// </summary>
		string Host { get; }

		/// <summary>
		/// Starts gRPC communication
		/// </summary>
		/// <param name="clientId">Current client ID (PID)</param>
		/// <param name="port">Port on which the server is running.</param>
		/// <returns></returns>
		Task StartAsync(string clientId, int port);

		/// <summary>
		/// Stopos gRPC communication
		/// </summary>
		/// <returns></returns>
		Task StopAsync();
	}
}
