using IdeaStatiCa.Plugin.Utilities;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	public class GrpcMethodInvokerHandler : IMethodInvoker, IGrpcMessageHandler
	{
		public readonly string HandlerName;
		public IGrpcSender GrpcSender { get; private set; }

		TaskCompletionSource<GrpcMessage> grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();

		public GrpcMethodInvokerHandler(string handlerName, IGrpcSender grpcSender)
		{
			HandlerName = handlerName;
			GrpcSender = grpcSender;
		}

		public T InvokeMethod<T>(string methodName, params object[] arguments)
		{
			var parsedArgs = ReflectionHelper.GetMethodInvokeArguments(arguments);
			var request = new GrpcReflectionInvokeData()
			{
				MethodName = methodName,
				Parameters = parsedArgs
			};
			string data = JsonConvert.SerializeObject(request);


			var response = SendMessageDataSync(HandlerName, data);

			// hadnle response
			var responseData = JsonConvert.DeserializeObject<T>(response.Data);

			return responseData;
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

				await GrpcSender.SendMessageAsync(operationId, messageName, data);

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

				await GrpcSender.SendMessageAsync(grpcMessage);

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



		public Task<object> HandleServerMessage(GrpcMessage message, GrpcServer server)
		{
			grpcMessageCompletionSource?.TrySetResult(message);
			return Task.FromResult<object>(message);
		}

		public Task<object> HandleClientMessage(GrpcMessage message, GrpcClient client)
		{
			grpcMessageCompletionSource?.TrySetResult(message);
			return Task.FromResult<object>(message);
		}
	}
}
