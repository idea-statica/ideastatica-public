using Nito.AsyncEx.Synchronous;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	public interface IGrpcSynchronousClient : IGrpcClient
	{
		GrpcMessage SendMessageDataSync(string messageName, string data);
		GrpcMessage SendMessageDataSync(GrpcMessage grpcMessage);
	}

	/// <summary>
	/// GrpcClient implementation that waits for message callbacks.
	/// </summary>
	public class GrpcSynchronousClient : GrpcClient, IGrpcSynchronousClient
	{
		TaskCompletionSource<GrpcMessage> grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();

		/// <summary>
		/// Initializes a new <see cref="GrpcSynchronousClient"/>
		/// </summary>
		/// <param name="clientId">ID of the client.</param>
		/// <param name="port"></param>
		public GrpcSynchronousClient(string clientId, int port) : base(clientId, port)
		{
		}

		/// <summary>
		/// Needed for by UT
		/// </summary>
		public GrpcSynchronousClient() : base("", 80)
		{
		}

		/// <summary>
		/// Sends message and waits for the callback from the server.
		/// </summary>
		/// <param name="messageName">Message identificator.</param>
		/// <param name="data">Body of the message.</param>
		/// <returns></returns>
		public GrpcMessage SendMessageDataSync(string messageName, string data)
		{
			return Task.Run(async () =>
			{
				var operationId = Guid.NewGuid().ToString(); 

				grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();

				await SendMessageAsync(messageName, data, operationId);

				// wait for the callback handler
				var messageReceived = false;
				GrpcMessage incomingMessage = null;

				while (!messageReceived)
				{
					incomingMessage = await grpcMessageCompletionSource.Task;

					if (incomingMessage.OperationId == operationId)
					{
						if (incomingMessage.MessageName == "Error")
						{
							throw new ApplicationException(incomingMessage.Data);
						}

						messageReceived = true;
					}
					else
					{
						grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();
					}
				}

				return incomingMessage;
			}).WaitAndUnwrapException();
		}

		public GrpcMessage SendMessageDataSync(GrpcMessage grpcMessage)
		{
			return Task.Run(async () =>
			{
				var operationId = Guid.NewGuid().ToString();
				grpcMessage.OperationId = operationId;

				grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();

				await SendMessageAsync(grpcMessage);

				// wait for the callback handler
				var messageReceived = false;
				GrpcMessage incomingMessage = null;

				while (!messageReceived)
				{
					incomingMessage = await grpcMessageCompletionSource.Task;

					if (incomingMessage.OperationId == operationId)
					{
						if (incomingMessage.MessageName == "Error")
						{
							throw new ApplicationException(incomingMessage.Data);
						}

						messageReceived = true;
					}
					else
					{
						grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();
					}
				}

				return incomingMessage;
			}).WaitAndUnwrapException();
		}

		/// <summary>
		/// Overrides the base HandleMessage.
		/// </summary>
		/// <returns></returns>
		internal override Task HandleMessageAsync(GrpcMessage message)
		{
			grpcMessageCompletionSource?.TrySetResult(message);

			return base.HandleMessageAsync(message);
		}
	}
}
