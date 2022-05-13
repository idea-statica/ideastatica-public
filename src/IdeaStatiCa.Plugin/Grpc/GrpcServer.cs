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
	public class GrpcServer : GrpcService.GrpcServiceBase, IGrpcCommunicator
	{
		#region Private fields
		private Server server;
		private string currentClientId;
		private IServerStreamWriter<GrpcMessage> currentClientStream = null;
		private Dictionary<string, IGrpcMessageHandler> handlers = new Dictionary<string, IGrpcMessageHandler>();
		protected readonly IPluginLogger Logger;
		private readonly int MaxDataLength;
		#endregion

		#region Properties & Events
		/// <summary>
		/// Triggered every time client connects. Sends a client ID in args.
		/// </summary>
		public event EventHandler<string> ClientConnected;

		/// <summary>
		/// Triggered when client has disconnected. Sends a client ID in args.
		/// </summary>
		public event EventHandler<string> ClientDisconnected;

		/// <summary>   
		/// Determines whether the client is connected.
		/// </summary>
		public bool IsConnected { get; set; }

		/// <summary>
		/// Port on which the server will communicate.
		/// </summary>
		public int Port { get; private set; }

		public string Host { get; private set; }

		/// <summary>
		/// Returns ID of currently connected client.
		/// </summary>
		public string ClientID { get; private set; }
		#endregion

		#region ctor
		/// <summary>
		/// Initializes the IdeaStatiCa Grpc server.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="maxDataLength">The maximal size of GrpcMessage.data in grpc message</param>
		public GrpcServer(IPluginLogger logger, int maxDataLength = Constants.GRPC_MAX_MSG_SIZE)
		{
			Debug.Assert(logger != null);
			MaxDataLength = maxDataLength;
			this.Logger = logger;
			Host = "localhost";
		}
		#endregion

		#region Methods
		/// <summary>
		/// Initializes server on specified port.
		/// </summary>
		private void Start()
		{
			Logger.LogDebug($"GrpcServer.Start listening on port {Host}:{Port}");
			server = new Server(CommunicationTools.GetChannelOptions(MaxDataLength + 1024*30))
			{
				Services = { GrpcService.BindService(this) },
				Ports = { new ServerPort(Host, Port, ServerCredentials.Insecure) }
			};

			server.Start();
		}

		/// <summary>
		/// Starts the server.
		/// </summary>
		/// <param name="clientId">Current client ID (PID)</param>
		/// <param name="port">Port on which the server is running.</param>
		public void Connect(string clientId, int port)
		{
			Logger.LogDebug("GrpcServer.ConnectAsync");
			ClientID = clientId;
			Port = port;
			Start();
		}

		public Task StartAsync()
		{
			Logger.LogDebug("GrpcServer.StartAsync");
			return Task.CompletedTask;
		}

		/// <summary>
		/// Requests server to shutdown
		/// </summary>
		/// <returns></returns>
		public async Task DisconnectAsync()
		{
			Logger.LogDebug("GrpcServer.DisconnectAsync");
			if (server != null)
			{
				try
				{
					await server.ShutdownAsync();
				}
				catch(Exception ex)
				{
					Logger.LogWarning("GrpcServer.DisconnectAsync failed", ex);
				}
			}
		}

		/// <summary>
		/// Registers a message handler.
		/// </summary>
		/// <param name="handlerId">UniqueID of the handler.</param>
		/// <param name="handler">Handler implementation.</param>
		public void RegisterHandler(string handlerId, IGrpcMessageHandler handler)
		{
			Logger.LogDebug($"GrpcServer.RegisterHandler handlerId = '{handlerId}' handler = '{handler.GetType().Name}'");
			if (handlers.ContainsKey(handlerId))
			{
				throw new ArgumentException($"Handler with ID {handlerId} is already registered.");
			}

			handlers.Add(handlerId, handler);
		}

		/// <summary>
		/// Establishes a two way connection between server and client.
		/// </summary>
		/// <param name="requestStream">Data incoming from client</param>
		/// <param name="responseStream">Data sent to client</param>
		/// <returns></returns>
		public async override Task ConnectAsync(IAsyncStreamReader<GrpcMessage> requestStream, IServerStreamWriter<GrpcMessage> responseStream, ServerCallContext context)
		{
			Logger.LogDebug("GrpcServer.ConnectAsync");

			if (!await requestStream.MoveNext())
			{
				Logger.LogDebug("GrpcServer.ConnectAsync MoveNext returned false");
				return;
			}

			do
			{
				// do not allow connection from multiple clients.
				if (IsConnected && currentClientId != requestStream.Current.ClientId)
				{

					Logger.LogDebug("GrpcServer.ConnectAsync error = Client already connected");
					var errorMsg = new GrpcMessage()
					{
						OperationId = "Error",
						MessageName = "Client already connected",
						MessageType = GrpcMessage.Types.MessageType.Response,
						DataType = typeof(string).Name
					};

					await SendMessageAsync(errorMsg);
				}
				else
				{

					// Handle first connection
					if (!IsConnected)
					{
						currentClientId = requestStream.Current.ClientId;
						Logger.LogDebug($"GrpcServer.ConnectAsync - first connection currentClientId = {currentClientId}, ");
						currentClientStream = responseStream;

						IsConnected = true;

						ClientConnected?.Invoke(this, currentClientId);
					}

					Logger.LogTrace($"GrpcServer.ConnectAsync - reading message ");

					var message = requestStream.Current;


					Logger.LogTrace($"GrpcServer.ConnectAsync - calling RunHandler");
					RunHandler(message);
				}

			} while (await requestStream.MoveNext());

			ClientDisconnected?.Invoke(this, currentClientId);

			IsConnected = false;

			currentClientStream = null;
			currentClientId = null;
		}

		private void RunHandler(GrpcMessage message)
		{
			Logger.LogDebug($"GrpcServer.RunHandler : clientID = '{message?.ClientId}', operationId = {message?.OperationId}");
			Task.Run(() =>
			{
				// handle incoming message
				HandleMessageAsync(message);
			});
		}

		/// <summary>
		/// Send response to the client
		/// </summary>
		/// <param name="message">Message to send</param>
		/// <returns>Task which is responsible for sending message</returns>
		/// <exception cref="Exception">Exception is thrown if sending data failed</exception>
		public async Task SendMessageAsync(GrpcMessage message)
		{
			int dataStringLength = string.IsNullOrEmpty(message.Data) ? 0 : message.Data.Length;
			Logger.LogDebug($"GrpcServer.SendMessageAsync operationId = '{message.OperationId}, messageName = '{message.MessageName}', dataLength = {dataStringLength}");
			
			// check if the size of the sending data is not bigger than size of the buffer
			if((2 * dataStringLength) > MaxDataLength)
			{
				// data are too large - compress them
				message.Compression = true;
				message.Data = CommunicationTools.Zip(message.Data);
				Logger.LogTrace($"Compressing data origSize = {dataStringLength}, compressedSize = {message.Data.Length}");
			}

			if (IsConnected)
			{
				await currentClientStream.WriteAsync(message);
			}
			else
			{
				throw new Exception("Client disconnected.");
			}
		}

		/// <summary>
		/// Handle incoming messages.
		/// </summary>
		/// <param name="message">Message incoming from client.</param>
		/// <returns></returns>
		protected virtual Task HandleMessageAsync(GrpcMessage message)
		{
			Logger.LogDebug($"GrpcServer.HandleMessageAsync : clientID = '{message?.ClientId}', operationId = '{message?.OperationId}', MessageName = '{message.MessageName}', Compression = {message.Compression}");

			if (message.Compression)
			{
				// data are compressed
				message.Data = CommunicationTools.Unzip(message.Data);
				message.Compression = false;
			}

			var handler = handlers.ContainsKey(message.MessageName) ? handlers[message.MessageName] : null;

			if (handler != null)
			{
				if (message?.MessageType == GrpcMessage.Types.MessageType.Response)
				{
					handler.HandleClientMessage(message, null);
				}
				else
				{
					handler.HandleServerMessage(message, this);
				}

			}
			else
			{
				throw new ApplicationException($"GrpcServer.HandleMessageAsync  error. Message handler '{message.MessageName}' is not registered!");
			}

			return Task.CompletedTask;
			//MessageReceived?.Invoke(this, message);
		}
		#endregion
	}
}
