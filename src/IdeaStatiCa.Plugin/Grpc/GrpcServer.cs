using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	/// <summary>
	/// Server implementation for the Grpc connection.
	/// </summary>
	public class GrpcServer : IGrpcServer
	{
		private Server server;
		public readonly IPluginLogger Logger;
		private readonly int MaxDataLength;

		/// <summary>
		/// Port on which the server will communicate.
		/// </summary>
		public int Port { get; private set; }

		public string Host { get; private set; }

		/// <summary>
		/// Returns ID of currently connected client.
		/// </summary>
		public string ClientID { get; private set; }

		/// <summary>
		/// grpc service for bi-directional streaming messaging
		/// </summary>
		public Services.GrpcService GrpcService { get; }

		/// <summary>
		/// Initializes the IdeaStatiCa Grpc server.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="maxDataLength">The maximal size of GrpcMessage.data in grpc message</param>
		public GrpcServer(IPluginLogger logger, int maxDataLength = Constants.GRPC_MAX_MSG_SIZE)
		{
			Debug.Assert(logger != null);
			this.Logger = logger;
			MaxDataLength = maxDataLength;
			Host = "localhost";
			GrpcService = new Services.GrpcService(Logger, MaxDataLength);
		}

		/// <summary>
		/// Initializes server on specified port.
		/// </summary>
		private void Start()
		{
			Logger.LogDebug($"GrpcServer.Start listening on port {Host}:{Port}");
			server = new Server(CommunicationTools.GetChannelOptions(MaxDataLength))
			{
				Services =
				{
					Grpc.GrpcService.BindService(GrpcService),
					Grpc.GrpcBlobStorageService.BindService(new Services.GrpcBlobStorageService())
				},
				Ports = { new ServerPort(Host, Port, ServerCredentials.Insecure) }
			};

			server.Start();
		}

		/// <summary>
		/// Starts the server.
		/// </summary>
		/// <param name="clientId">Current client ID (PID)</param>
		/// <param name="port">Port on which the server is running.</param>
		public Task StartAsync(string clientId, int port)
		{
			Logger.LogDebug("GrpcServer.StartAsync");

			ClientID = clientId;
			Port = port;
			Start();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Requests server to shutdown
		/// </summary>
		/// <returns></returns>
		public async Task StopAsync()
		{
			Logger.LogDebug("GrpcServer.DisconnectAsync");
			if (server != null)
			{
				try
				{
					await server.ShutdownAsync();
				}
				catch (Exception ex)
				{
					Logger.LogWarning("GrpcServer.DisconnectAsync failed", ex);
				}
			}
		}
	}
}
