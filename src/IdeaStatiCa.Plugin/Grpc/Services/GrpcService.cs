using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Services
{
	public class GrpcService : Grpc.GrpcService.GrpcServiceBase, IGrpcService
	{
		#region Private fields
		private readonly IPluginLogger logger;
		private string currentClientId;
		private IServerStreamWriter<GrpcMessage> currentClientStream;
		private Dictionary<string, IGrpcMessageHandler> handlers = new Dictionary<string, IGrpcMessageHandler>();
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
		#endregion

		#region ctor
		/// <summary>
		/// Initializes the grpc service
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="maxDataLength">The maximal size of GrpcMessage.data in grpc message</param>
		public GrpcService(IPluginLogger logger)
		{
			this.logger = logger;
			MaxDataLength = Constants.GRPC_MAX_MSG_SIZE;
			logger.LogInformation("GrpcService constructor");
		}
		#endregion

		#region Methods
		/// <summary>
		/// Establishes a two way connection between server and client.
		/// </summary>
		/// <param name="requestStream">Data incoming from client</param>
		/// <param name="responseStream">Data sent to client</param>
		/// <returns></returns>
		public async override Task ConnectAsync(IAsyncStreamReader<GrpcMessage> requestStream, IServerStreamWriter<GrpcMessage> responseStream, ServerCallContext context)
		{
			logger.LogDebug("GrpcService.ConnectAsync");

			try
			{
				if (!await requestStream.MoveNext())
				{
					logger.LogDebug("GrpcService.ConnectAsync MoveNext returned false");
					return;
				}

				do
				{
					// do not allow connection from multiple clients.
					if (IsConnected && currentClientId != requestStream.Current.ClientId)
					{

						logger.LogDebug($"GrpcService.ConnectAsync error = Client already connected currentClientId = {currentClientId}");
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
							logger.LogDebug($"GrpcService.ConnectAsync - first connection currentClientId = {currentClientId}, ");
							currentClientStream = responseStream;

							IsConnected = true;

							ClientConnected?.Invoke(this, currentClientId);
						}

						logger.LogTrace($"GrpcService.ConnectAsync - reading message ");

						var message = requestStream.Current;


						logger.LogTrace($"GrpcService.ConnectAsync - calling RunHandler");
						RunHandler(message);
					}

				} while (await requestStream.MoveNext());
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcService.ConnectAsync Communication failed", ex);
			}
			finally
			{

				ClientDisconnected?.Invoke(this, currentClientId);

				IsConnected = false;

				currentClientStream = null;
				currentClientId = null;
			}
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
			logger.LogDebug($"GrpcService.SendMessageAsync operationId = '{message.OperationId}, messageName = '{message.MessageName}', dataLength = {dataStringLength}");

			// check if the size of the sending data is not bigger than size of the buffer
			if ((2 * dataStringLength) > MaxDataLength)
			{
				// data are too large - compress them
				message.Compression = true;
				message.Data = CommunicationTools.Zip(message.Data);
				logger.LogTrace($"Compressing data origSize = {dataStringLength}, compressedSize = {message.Data.Length}");
			}

			await currentClientStream.WriteAsync(message);
		}

		/// <summary>
		/// Registers a message handler.
		/// </summary>
		/// <param name="handlerId">UniqueID of the handler.</param>
		/// <param name="handler">Handler implementation.</param>
		public void RegisterHandler(string handlerId, IGrpcMessageHandler handler)
		{
			logger.LogDebug($"GrpcServer.RegisterHandler handlerId = '{handlerId}' handler = '{handler.GetType().Name}'");
			if (handlers.ContainsKey(handlerId))
			{
				throw new ArgumentException($"Handler with ID {handlerId} is already registered.");
			}

			handlers.Add(handlerId, handler);
		}

		private void RunHandler(GrpcMessage message)
		{
			logger.LogDebug($"GrpcServer.RunHandler : clientID = '{message?.ClientId}', operationId = {message?.OperationId}");
			Task.Run(() =>
			{
				// handle incoming message
				HandleMessageAsync(message);
			});
		}

		/// <summary>
		/// Handle incoming messages.
		/// </summary>
		/// <param name="message">Message incoming from client.</param>
		/// <returns></returns>
		protected virtual Task HandleMessageAsync(GrpcMessage message)
		{
			logger.LogDebug($"GrpcServer.HandleMessageAsync : clientID = '{message?.ClientId}', operationId = '{message?.OperationId}', MessageName = '{message.MessageName}', Compression = {message.Compression}");

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
		}
		#endregion
	}
}
