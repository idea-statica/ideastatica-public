using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	/// <summary>
	/// Client implementation for the Grpc connection.
	/// </summary>
	public class GrpcClient : IGrpcClient
	{
		#region Private Fields
		private Channel channel;
		private Dictionary<string, IGrpcMessageHandler> handlers = new Dictionary<string, IGrpcMessageHandler>();
		private AsyncDuplexStreamingCall<GrpcMessage, GrpcMessage> client;
		private string errorMessage;
		protected readonly IPluginLogger Logger;
		private readonly int MaxDataLength;
		#endregion

		#region Properties & Events

		/// <summary>
		/// Triggered every time client connects. Sends a client ID in args.
		/// </summary>
		public event EventHandler ClientConnected;

		/// <summary>
		/// Triggered when client has disconnected. Sends a client ID in args.
		/// </summary>
		public event EventHandler ClientDisconnected;

		/// <summary>
		/// ID of the client. Typically the PID of the plugins process.
		/// </summary>
		public string ClientID { get; private set; }

		/// <summary>
		/// Port on which the server will communicate.
		/// </summary>
		public int Port { get; private set; }

		public string Host { get; private set; }

		/// <summary>
		/// Determines whether client is connected.
		/// </summary>
		public bool IsConnected { get; private set; }
		#endregion

		#region ctor
		/// <summary>
		/// Initializes the IdeaStatiCa Grpc client.
		/// </summary>
		/// <param name="logger">The instance of the pluginLogger</param>
		/// <param name="maxDataLength">The maximal size of GrpcMessage.data in grpc message</param>
		public GrpcClient(IPluginLogger logger, int maxDataLength = Constants.GRPC_MAX_MSG_SIZE)
		{
			Debug.Assert(logger != null);
			logger.LogDebug($"GrpcClient constructor");
			MaxDataLength = maxDataLength;
			this.Logger = logger;
			Host = "localhost";
		}
		#endregion

		#region Methods
		/// <summary>
		/// Registers a message handler.
		/// </summary>
		/// <param name="handlerId">UniqueID of the handler.</param>
		/// <param name="handler">Handler implementation.</param>
		public void RegisterHandler(string handlerId, IGrpcMessageHandler handler)
		{
			if (handlers.ContainsKey(handlerId))
			{
				throw new ArgumentException($"Handler with ID {handlerId} is already registered.");
			}

			Logger.LogDebug($"GrpcClient.RegisterHandler handlerId = {handlerId}, handler = {handler.GetType().Name}");

			handlers.Add(handlerId, handler);
		}

		/// <summary>
		/// Connects to the server.
		/// </summary>
		/// <param name="clientId">Current client ID (PID)</param>
		/// <param name="port">Port on which the server is running.</param>
		/// <returns></returns>
		private void Connect(string clientId, int port)
		{
			ClientID = clientId;
			Port = port;

			string address = $"localhost:{Port}";
			Logger.LogDebug($"GrpcClient.Connect address = '{address}', clientId = '{clientId}', port = '{port}'");

			channel = new Channel(address, ChannelCredentials.Insecure, CommunicationTools.GetChannelOptions(MaxDataLength));

			var serviceClient = new GrpcService.GrpcServiceClient(channel);

			client = serviceClient.ConnectAsync();
			IsConnected = true;

			ClientConnected?.Invoke(this, EventArgs.Empty);

			Logger.LogDebug("GrpcClient.Connect : client is connected");
		}

		/// <summary>
		/// Connects to the server and starts listen to responses.
		/// </summary>
		/// <param name="clientId">Current client ID (PID)</param>
		/// <param name="port">Port on which the server is running.</param>
		/// <returns></returns>
		public Task StartAsync(string clientId, int port)
		{
			Connect(clientId, port);

			return Task.Run(async () =>
			{
				string address = $"{Host}:{Port}";
				Logger.LogDebug($"GrpcClient.StartAsync : address = '{address}'");
				try
				{
					while (await ResponseStream())
					{
						var grpcMessage = client.ResponseStream.Current;
						RunMessageHandler(grpcMessage);
					}
				}
				catch (Exception e)
				{
					Logger.LogTrace("GrpcClient.StartAsync reading response stream failed", e);
					errorMessage = e.Message;
					await StopAsync();
				}
			});
		}

		private void RunMessageHandler(GrpcMessage message)
		{
			Task.Factory.StartNew(() =>
			{
				try
				{
					HandleMessageAsync(message);
				}
				catch (Exception e)
				{
					Logger.LogWarning($"GrpcClient.RunMessageHandler failed : MessageName = '{message.MessageName}', OperationId = '{message.OperationId}'", e);
				}
			});
		}

		private async Task<bool> ResponseStream()
		{
			return await client.ResponseStream.MoveNext(cancellationToken: CancellationToken.None);
		}

		/// <summary>
		/// Disconnets client from the server.
		/// </summary>
		/// <returns></returns>
		public async Task StopAsync()
		{
			Logger.LogDebug("GrpcClient.DisconnectAsync");
			try
			{
				await client?.RequestStream?.CompleteAsync();
				await channel.ShutdownAsync();
			}
			catch (Exception e)
			{
				// client can already been closed
				Logger.LogDebug("GprcClient.DisconnectAsync - can not close gRPC client", e);
				return;
			}
			finally
			{
				IsConnected = false;
				ClientDisconnected?.Invoke(this, EventArgs.Empty);
			}

			Logger.LogDebug("GprcClient.DisconnectAsync : disconnected");
		}

		/// <summary>
		/// Sends a message to the server.
		/// </summary>
		/// <param name="message">Grpc message to send.</param>
		/// <param name="data">Body of the message.</param>
		/// <returns></returns>
		public async Task SendMessageAsync(GrpcMessage message)
		{
			int dataStringLength = string.IsNullOrEmpty(message.Data) ? 0 : message.Data.Length;

			Logger.LogDebug($"GrpcClient.SendMessageAsync MessageName = '{message?.MessageName};, ClientId = '{message?.ClientId}', OperationId = '{message?.OperationId}', dataLength = {dataStringLength}");

			// check if the size of the sending data is not bigger than size of the buffer
			if ((2 * dataStringLength) > MaxDataLength)
			{
				// data are too large - compress them
				message.Compression = true;
				message.Data = CommunicationTools.Zip(message.Data);
				Logger.LogTrace($"Compressing data origSize = {dataStringLength}, compressedSize = {message.Data.Length}");
			}

			if (IsConnected)
			{
				try
				{
					await client.RequestStream.WriteAsync(message);
				}
				catch (Exception e)
				{
					Logger.LogDebug("GrpcClient.SendMessageAsync failed in 'await client.RequestStream.WriteAsync(message)'", e);
				}
			}
			else
			{
				Logger.LogDebug($"GrpcClient.SendMessageAsync failed : client is not connected. MessageName = '{message?.MessageName}' OperationId = '{message?.OperationId}'");
				throw new Exception("GrpcClient.SendMessageAsync client is not connected.");
			}
		}

		/// <summary>
		/// Handle incoming messages.
		/// </summary>
		/// <param name="message">Message incoming from server.</param>
		/// <returns></returns>
		internal virtual void HandleMessageAsync(GrpcMessage message)
		{
			Logger.LogDebug($"GrpcClient.HandleMessageAsync MessageName = {message?.MessageName}, ClientId = {message?.ClientId}, OperationId = {message?.OperationId}, Compression = {message.Compression}");

			if (message.Compression)
			{
				// data are compressed
				message.Data = CommunicationTools.Unzip(message.Data);
				message.Compression = false;
			}

			var handler = handlers.ContainsKey(message.MessageName) ? handlers[message.MessageName] : null;

			if (handler != null)
			{
				Logger.LogDebug($"GrpcClient.HandleMessageAsync calling handler Handler = {handler.GetType().Name}, MessageName = {message?.MessageName},MessageType = '{message?.MessageType}'");

				if (message?.MessageType == GrpcMessage.Types.MessageType.Request)
				{
					handler.HandleServerMessage(message, this);
				}
				else
				{
					handler.HandleClientMessage(message, this);
				}
			}
			else
			{
				throw new ApplicationException($"Grpc reflection error. Message handler '{message.MessageName}' is not registered!");
			}
		}

		public Task ConnectAsync(IAsyncStreamReader<GrpcMessage> requestStream, IServerStreamWriter<GrpcMessage> responseStream, ServerCallContext context)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
