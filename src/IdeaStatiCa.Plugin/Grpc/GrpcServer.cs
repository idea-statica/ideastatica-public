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
	public class GrpcServer : GrpcService.GrpcServiceBase
	{
		#region Private fields
		private Server server;
		private string currentClientId;
		private IServerStreamWriter<GrpcMessage> currentClientStream = null;
		private Dictionary<string, IGrpcMessageHandler> handlers = new Dictionary<string, IGrpcMessageHandler>();
		protected readonly IPluginLogger Logger;

		private List<ChannelOption> channelOptions = new List<ChannelOption>()
				{
						new ChannelOption(ChannelOptions.MaxReceiveMessageLength, Constants.GRPC_MAX_MSG_SIZE),
						new ChannelOption(ChannelOptions.MaxSendMessageLength, Constants.GRPC_MAX_MSG_SIZE)
				};
		#endregion

		#region Properties & Events
		/// <summary>
		/// Triggered every time a message is received from client.
		/// </summary>
		public event EventHandler<GrpcMessage> MessageReceived;

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
		protected int Port { get; private set; }

		/// <summary>
		/// Returns ID of currently connected client.
		/// </summary>
		public string ClientID { get; private set; }
		#endregion

		#region ctor
		/// <summary>
		/// Initializes the IdeaStatiCa Grpc server.
		/// </summary>
		/// <param name="port">Sets the <see cref="Port"/></param>
		/// <param name="logger">Logger</param>
		public GrpcServer(int port, IPluginLogger logger)
		{
			Debug.Assert(logger != null);

			this.Logger = logger;
			Port = port;
			string host = "localhost";

			Logger.LogDebug($"GrpcServer listening on port ${host}:${port}");
			server = new Server(channelOptions)
			{
				Services = { GrpcService.BindService(this) },
				Ports = { new ServerPort(host, Port, ServerCredentials.Insecure) }
			};
		}
		#endregion

		#region Methods
		/// <summary>
		/// Initializes server on specified port.
		/// </summary>
		public void Start()
		{
			Logger.LogDebug("GrpcServer.Start");
			server.Start();
		}

		/// <summary>
		/// Requests server to shutdown
		/// </summary>
		/// <returns></returns>
		public async Task StopAsync()
		{
			Logger.LogDebug("GrpcServer.StopAsync");
			if (server != null)
			{
				await server.ShutdownAsync();
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
				IsConnected = false;
				return;
			}

			do
			{
				// do not allow connection from multiple clients.
				if (IsConnected && currentClientId != requestStream.Current.ClientId)
				{
	
					Logger.LogDebug("GrpcServer.ConnectAsync error = Client already connected");
					await SendMessageAsync("Error", "Client already connected");
				}
				else
				{
					// Handle first connection
					if (!IsConnected)
					{
						currentClientId = requestStream.Current.ClientId;
						currentClientStream = responseStream;

						IsConnected = true;

						ClientConnected?.Invoke(this, currentClientId);
					}

					// handle incoming message 
					await HandleMessageAsync(requestStream.Current);
				}

			} while (await requestStream.MoveNext());

			ClientDisconnected?.Invoke(this, currentClientId);

			IsConnected = false;

			currentClientStream = null;
			currentClientId = null;
		}

		/// <summary>
		/// Sends a message to client.
		/// </summary>
		/// <param name="messageName">Message identificator.</param>
		/// <param name="data">Body of the message.</param>
		/// <returns></returns>
		public async Task SendMessageAsync(string operationId, string messageName, string data = "")
		{
			if (IsConnected)
			{
				Logger.LogDebug($"GrpcServer.SendMessageAsync operationId = '{operationId}, messageName = '{messageName}'");
				await currentClientStream.WriteAsync(new GrpcMessage
				{
					OperationId = operationId,
					MessageName = messageName,
					Data = data
				});
			}
			else
			{
				Logger.LogDebug("GrpcServer.SendMessageAsync error = Client disconnected");
				throw new Exception("Client disconnected.");
			}
		}

		public async Task SendMessageAsync(GrpcMessage message)
		{
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
		protected virtual async Task HandleMessageAsync(GrpcMessage message)
		{
			var handler = handlers.ContainsKey(message.MessageName) ? handlers[message.MessageName] : null;

			if (handler != null)
			{
				var result = await handler.HandleServerMessage(message, this);
			}
			else
			{
				throw new ApplicationException($"Grpc reflection error. Message handler '{message.MessageName}' is not registered!");
			}

			MessageReceived?.Invoke(this, message);
		}
		#endregion
	}
}
