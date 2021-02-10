using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
    /// <summary>
    /// GrpcClient implementation that waits for message callbacks.
    /// </summary>
    public class GrpcSynchronousClient : GrpcClient
    {
        TaskCompletionSource<GrpcMessage> gprcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();

        /// <summary>
        /// Initializes a new <see cref="GrpcSynchronousClient"/>
        /// </summary>
        /// <param name="clientId">ID of the client.</param>
        /// <param name="port"></param>
        public GrpcSynchronousClient(string clientId, int port) : base(clientId, port)
        {
        }

        /// <summary>
        /// Sends message and waits for the callback from the server.
        /// </summary>
        /// <param name="messageName">Message identificator.</param>
        /// <param name="data">Body of the message.</param>
        /// <returns></returns>
        public async Task<GrpcMessage> SendMessageSync(string messageName, string data)
        {
            var operationId = Guid.NewGuid().ToString();

            await SendMessageAsync(messageName, data, operationId);

            // wait for the callback handler
            var messageReceived = false;
            GrpcMessage incomingMessage = null;

            
            while (!messageReceived)
            {                
                incomingMessage = await gprcMessageCompletionSource.Task;

                if(incomingMessage.OperationId == operationId)
                {
                    if(incomingMessage.MessageName == "Error")
                    {
                        throw new ApplicationException(incomingMessage.Data);
                    }

                    messageReceived = true;
                }
                else
                {
                    gprcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();
                }
            }

            return incomingMessage;
        }

        /// <summary>
        /// Overrides the base HandleMessage.
        /// </summary>
        /// <returns></returns>
        protected override Task HandleMessageAsync(GrpcMessage message)
        {
            gprcMessageCompletionSource?.TrySetResult(message);

            return base.HandleMessageAsync(message);
        }
    }
}
