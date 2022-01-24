using IdeaStatiCa.Plugin.Utilities;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	public interface IMethodTask
	{
		GrpcMessage SendMessageDataSync(GrpcMessage grpcMessage);
		void SetResult(GrpcMessage message);
	}

	public class MethodTask : IMethodTask
	{
		public readonly string OperationId;
		public IGrpcSender GrpcSender { get; private set; }

		TaskCompletionSource<GrpcMessage> grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();

		public MethodTask(IGrpcSender grpcSender, string operationId)
		{
			this.OperationId = operationId;
			this.GrpcSender = grpcSender;
		}

		public GrpcMessage SendMessageDataSync(GrpcMessage grpcMessage)
		{
			return Task.Run(async () =>
			{
				grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();
				string originalOperationId = grpcMessage.OperationId;

				await GrpcSender.SendMessageAsync(grpcMessage);

				// wait for the callback handler
				var messageReceived = false;
				GrpcMessage incomingMessage = null;

				while (!messageReceived)
				{
					incomingMessage = await grpcMessageCompletionSource.Task;

					if (incomingMessage.OperationId == originalOperationId)
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

		public void SetResult(GrpcMessage message)
		{
			grpcMessageCompletionSource?.TrySetResult(message);
		}
	}

	public class GrpcMethodInvokerHandler : IMethodInvoker, IGrpcMessageHandler
	{
		public readonly string HandlerName;
		public IGrpcSender GrpcSender { get; private set; }

		//TaskCompletionSource<GrpcMessage> grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();
		ConcurrentDictionary<string, IMethodTask> executedMethods = new ConcurrentDictionary<string, IMethodTask>();

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
			var operationId = Guid.NewGuid().ToString();
			grpcMessage.OperationId = operationId;

			var methodTask = new MethodTask(this.GrpcSender, operationId);
			if (executedMethods.TryAdd(operationId, methodTask) == false)
			{
				throw new Exception($"Operation {operationId} is executing");
			}

			GrpcMessage incomingMessage = methodTask.SendMessageDataSync(grpcMessage);

			return incomingMessage;


			//return Task.Run(async () =>
			//{
			//	var operationId = Guid.NewGuid().ToString();
			//	grpcMessage.OperationId = operationId;

			//	grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();

			//	await GrpcSender.SendMessageAsync(grpcMessage);

			//	// wait for the callback handler
			//	var messageReceived = false;
			//	GrpcMessage incomingMessage = null;

			//	while (!messageReceived)
			//	{
			//		incomingMessage = await grpcMessageCompletionSource.Task;

			//		if (incomingMessage.OperationId == operationId)
			//		{
			//			if (incomingMessage.MessageName == "Error")
			//			{
			//				throw new ApplicationException(incomingMessage.Data);
			//			}

			//			messageReceived = true;
			//		}
			//		else
			//		{
			//			grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();
			//		}
			//	}

			//	return incomingMessage;
			//}).WaitAndUnwrapException();
		}

		public Task<object> HandleServerMessage(GrpcMessage message, IGrpcSender grpcSender)
		{
			SetCompleted(message);
			return Task.FromResult<object>(message);
		}

		public Task<object> HandleClientMessage(GrpcMessage message, IGrpcSender grpcSender)
		{
			SetCompleted(message);
			return Task.FromResult<object>(message);
		}

		private void SetCompleted(GrpcMessage message)
		{
			if (executedMethods.TryRemove(message.OperationId, out var methodTask))
			{
				methodTask.SetResult(message);
			}
			else
			{
				throw new Exception($"Operation {message?.OperationId} is not executing");
			}
		}
	}
}
