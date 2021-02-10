using Grpc.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
    /// <summary>
    /// Client implementation for the Grpc connection.
    /// </summary>
    public class GrpcClient
    {
        #region Private Fields
        private Channel channel;
        private Dictionary<string, IGrpcMessageHandler> handlers = new Dictionary<string, IGrpcMessageHandler>();
        private AsyncDuplexStreamingCall<GrpcMessage, GrpcMessage> client;
        private string errorMessage;
        private List<ChannelOption> channelOptions = new List<ChannelOption>()
        {
            new ChannelOption(ChannelOptions.MaxReceiveMessageLength, "2048 MB"),
            new ChannelOption(ChannelOptions.MaxSendMessageLength, "2048 MB")
        };
        #endregion

        #region Properties & Events
        /// <summary>
        /// Triggered when a message is received from the server.
        /// </summary>
        public event EventHandler<GrpcMessage> MessageReceived;

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
        /// Servert port.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Determines whether client is connected.
        /// </summary>
        public bool IsConnected { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// Initializes the IdeaStatiCa Grpc client.
        /// </summary>
        /// <param name="clientId">Current client ID (PID)</param>
        /// <param name="port">Port on which the server is running.</param>
        public GrpcClient(string clientId, int port)
        {
            ClientID = clientId;
            Port = port;
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

            handlers.Add(handlerId, handler);
        }

        /// <summary>
        /// Connects to the server.
        /// </summary>
        public async Task ConnectAsync()
        {
            try
            {
                channel = new Channel($"localhost:{Port}", ChannelCredentials.Insecure, channelOptions);

                var serviceClient = new GrpcService.GrpcServiceClient(channel);

                client = serviceClient.ConnectAsync();
                IsConnected = true;

                ClientConnected?.Invoke(this, EventArgs.Empty);

                // Handle incoming messages
                _ = Task.Run(async () =>
                {
                    while (await client.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                    {
                        var response = client.ResponseStream.Current;

                        await HandleMessageAsync(response);
                    }
                });
            }
            catch(Exception e)
            {
                errorMessage = e.Message;
                
                await DisconnectAsync();
            }
        }

        /// <summary>
        /// Disconnets client from the server.
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            await client?.RequestStream?.CompleteAsync();
            await channel.ShutdownAsync();

            IsConnected = false;
            ClientDisconnected?.Invoke(this, EventArgs.Empty);

            IsConnected = false;
        }

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        /// <param name="messageName">Message identificator.</param>
        /// <param name="data">Body of the message.</param>
        /// <returns></returns>
        public async Task SendMessageAsync(string messageName, string data = "", string operationId = null)
        {
            if (IsConnected)
            {
                var message = new GrpcMessage()
                {
                    ClientId = this.ClientID,
                    Data = data,
                    OperationId = operationId,
                    MessageName = messageName
                };

                await SendMessageAsync(message);
            }
            else
            {
                throw new Exception("Client disconnected.");
            }
        }

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        /// <param name="message">Grpc message to send.</param>
        /// <param name="data">Body of the message.</param>
        /// <returns></returns>
        public async Task SendMessageAsync(GrpcMessage message)
        {
            if (IsConnected)
            {
                await client.RequestStream.WriteAsync(message);
            }
            else
            {
                throw new Exception("Client disconnected.");
            }
        }

        /// <summary>
        /// Handle incoming messages.
        /// </summary>
        /// <param name="message">Message incoming from server.</param>
        /// <returns></returns>
        protected virtual async Task HandleMessageAsync(GrpcMessage message)
        {
            var handler = handlers.ContainsKey(message.MessageName) ? handlers[message.MessageName] : null;

            if (handler != null)
            {
                var result = await handler.HandleClientMessage(message, this);                
            }

            MessageReceived?.Invoke(this, message);
        }
        #endregion
    }
}
