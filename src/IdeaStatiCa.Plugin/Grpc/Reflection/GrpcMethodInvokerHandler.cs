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

		public T InvokeMethod<T>(string methodName, Type returnType, params object[] arguments)
		{
			var parsedArgs = ReflectionHelper.GetMethodInvokeArguments(arguments);
			var request = new GrpcReflectionInvokeData()
			{
				MethodName = methodName,
				Parameters = parsedArgs
			};
			string data = JsonConvert.SerializeObject(request);

			GrpcMessage msg = new GrpcMessage()
			{
				Data = data,
				DataType = request.GetType().Name,
				MessageName = HandlerName,
				MessageType = GrpcMessage.Types.MessageType.Request,
			};


			var response = SendMessageDataSync(msg);

			// hadnle response
			var responseData = JsonConvert.DeserializeObject(response.Data, returnType);

			return (T)responseData;
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

		public Task<object> HandleServerMessage(GrpcMessage message, IGrpcSender grpcSender)
		{
			grpcMessageCompletionSource?.TrySetResult(message);
			return Task.FromResult<object>(message);
		}

		public Task<object> HandleClientMessage(GrpcMessage message, IGrpcSender grpcSender)
		{
			grpcMessageCompletionSource?.TrySetResult(message);
			return Task.FromResult<object>(message);
		}
	}
}
