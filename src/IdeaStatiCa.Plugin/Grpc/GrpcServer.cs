using Grpc.Core;
using IdeaStatiCa.Public;
using System;
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
		private readonly IBlobStorageProvider blobStorageProvider;
		private readonly int MaxDataLength;
		private readonly int chunkSize;

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
		public IGrpcService GrpcService { get; }

		///// <summary>
		///// Constructor
		///// </summary>
		///// <param name="pluginLogger">Logger</param>
		public GrpcServer(IPluginLogger logger)
		{
			this.Logger = logger;
			this.blobStorageProvider = null;
			MaxDataLength = Constants.GRPC_MAX_MSG_SIZE;
			this.chunkSize = Constants.GRPC_CHUNK_SIZE;
			Host = "localhost";
			GrpcService = new Services.GrpcService(Logger);
		}

		/// <summary>
		/// Initializes the IdeaStatiCa Grpc server.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="grpcService"></param>
		/// <param name="blobStorageProvider">Provider of blob storages</param>
		public GrpcServer(IPluginLogger logger, IGrpcService grpcService, IBlobStorageProvider blobStorageProvider)
		{
			Debug.Assert(logger != null);
			this.Logger = logger;
			this.blobStorageProvider = blobStorageProvider;
			MaxDataLength = Constants.GRPC_MAX_MSG_SIZE;
			this.chunkSize = Constants.GRPC_CHUNK_SIZE;
			Host = "localhost";
			GrpcService = grpcService;
		}

		/// <summary>
		/// Initializes server on specified port.
		/// </summary>
		private void Start()
		{
			Logger.LogDebug($"GrpcServer.Start listening on port {Host}:{Port}");

			Grpc.GrpcService.GrpcServiceBase grpcServiceBase = (Grpc.GrpcService.GrpcServiceBase)GrpcService;
			server = new Server(CommunicationTools.GetChannelOptions(MaxDataLength))
			{
				Services =
				{
					Grpc.GrpcService.BindService(grpcServiceBase),
					Grpc.GrpcBlobStorageService.BindService(new Services.GrpcBlobStorageService(Logger, blobStorageProvider, chunkSize))
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
