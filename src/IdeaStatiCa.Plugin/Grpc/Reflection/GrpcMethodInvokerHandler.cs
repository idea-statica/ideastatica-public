using IdeaStatiCa.Plugin.Utilities;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
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
		internal IPluginLogger Logger { get; private set; }
		public IGrpcSender GrpcSender { get; private set; }

		TaskCompletionSource<GrpcMessage> grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();

		public MethodTask(IPluginLogger logger, IGrpcSender grpcSender, string operationId)
		{
			Debug.Assert(logger != null);
			Debug.Assert(grpcSender != null);
			
			this.Logger = logger;
			this.OperationId = operationId;
			this.GrpcSender = grpcSender;
		}

		public GrpcMessage SendMessageDataSync(GrpcMessage grpcMessage)
		{
			Logger.LogDebug($"MethodTask.SendMessageDataSync operationId = '{grpcMessage.OperationId}', MessageName = '{grpcMessage?.MessageName}', ClientId = '{grpcMessage.ClientId}'");

			return Task.Run(async () =>
			{
				Logger.LogDebug($"MethodTask.SendMessageDataSync : Task is starting  operationId = '{grpcMessage.OperationId}', MessageName = '{grpcMessage?.MessageName}', ClientId = '{grpcMessage.ClientId}'");

				grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();
				string originalOperationId = grpcMessage.OperationId;

				await GrpcSender.SendMessageAsync(grpcMessage);

				Logger.LogTrace($"MethodTask.SendMessageDataSync : message was sent  operationId = '{grpcMessage.OperationId}', MessageName = '{grpcMessage?.MessageName}', ClientId = '{grpcMessage.ClientId}'");

				// wait for the callback handler
				var messageReceived = false;
				GrpcMessage incomingMessage = null;

				Logger.LogTrace($"MethodTask.SendMessageDataSync : starting to wait for a response  operationId = '{grpcMessage.OperationId}', MessageName = '{grpcMessage?.MessageName}', ClientId = '{grpcMessage.ClientId}'");


				while (!messageReceived)
				{
					incomingMessage = await grpcMessageCompletionSource.Task;

					Logger.LogDebug($"MethodTask.SendMessageDataSync message  OperationId = '{incomingMessage?.OperationId}', MessageName = '{incomingMessage?.MessageName}'");

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

				Logger.LogTrace($"MethodTask.SendMessageDataSync : returning = '{incomingMessage.OperationId}', MessageName = '{incomingMessage?.MessageName}', ClientId = '{incomingMessage.ClientId}'");

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

		internal IPluginLogger Logger { get; private set; }

		//TaskCompletionSource<GrpcMessage> grpcMessageCompletionSource = new TaskCompletionSource<GrpcMessage>();
		ConcurrentDictionary<string, IMethodTask> executedMethods = new ConcurrentDictionary<string, IMethodTask>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="handlerName">Name of the message handler</param>
		/// <param name="grpcSender">Message sender</param>
		/// <param name="logger">Logger</param>
		public GrpcMethodInvokerHandler(string handlerName, IGrpcSender grpcSender, IPluginLogger logger)
		{
			Debug.Assert(!string.IsNullOrEmpty(handlerName));
			Debug.Assert(grpcSender != null);
			Debug.Assert(logger != null);
			HandlerName = handlerName;
			GrpcSender = grpcSender;
			Logger = logger;
		}

		/// <summary>
		/// Invoke method by reflex
		/// </summary>
		/// <typeparam name="T">Type of the result</typeparam>
		/// <param name="methodName">Name of the method to invoke</param>
		/// <param name="returnType">Type of the return value</param>
		/// <param name="arguments">arguments which will be passed to the invoked method</param>
		/// <returns>Result of the method</returns>
		/// <exception cref="ArgumentException">Exception is thrown in case of an error</exception>

		public T InvokeMethod<T>(string methodName, Type returnType, params object[] arguments)
		{
			Logger.LogDebug($"GrpcMethodInvokerHandler.InvokeMethod methodName = '{methodName}', returnType = '{returnType.Name}'");

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

			if (!string.IsNullOrEmpty(response.Data) || returnType != typeof(void))
			{
				// handle response
				if(!string.IsNullOrEmpty(response.DataType) && response.DataType.Equals(typeof(Exception).Name))
				{
					// en exception was thrown - write arguments to log 
					try
					{
						StringBuilder sb = new StringBuilder();
						foreach (var arg in arguments)
						{
							var argVal = JsonConvert.SerializeObject(arg);
							if (argVal?.Length > 255)
							{
								// do not write too long strings to logfile
								argVal = argVal.Substring(0, 255);
							}

							sb.Append($"type = '{arg.GetType().Name}' : val = '{argVal}' ;");
						}

						Logger.LogDebug($"GrpcMethodInvokerHandler.InvokeMethod FAILED methodName = '{methodName}' arguments = '{sb.ToString()}'");
					}
					catch (Exception ex)
					{
						Logger.LogWarning("GrpcMethodInvokerHandler.InvokeMethod - can not write arguments to log", ex);
					}

					var deserializeException = JsonConvert.DeserializeObject<Exception>(response.Data);

					throw new ArgumentException(deserializeException.Message, deserializeException);
				}

				var responseData = JsonConvert.DeserializeObject(response.Data, returnType);

				Logger.LogDebug("GrpcMethodInvokerHandler.InvokeMethod finished");
				return (T)responseData;
			}

			return default(T);
		}

		public GrpcMessage SendMessageDataSync(GrpcMessage grpcMessage)
		{
			var operationId = Guid.NewGuid().ToString();
			grpcMessage.OperationId = operationId;

			var methodTask = new MethodTask(this.Logger, this.GrpcSender, operationId);
			if (executedMethods.TryAdd(operationId, methodTask) == false)
			{
				throw new Exception($"Operation {operationId} is executing");
			}

			Logger.LogDebug($"GrpcMethodInvokerHandler.SendMessageDataSync operationId = '{grpcMessage.OperationId}'");
			GrpcMessage incomingMessage = methodTask.SendMessageDataSync(grpcMessage);

			return incomingMessage;
		}

		public Task<object> HandleServerMessage(GrpcMessage message, IGrpcSender grpcSender)
		{
			Logger.LogDebug($"GrpcMethodInvokerHandler.HandleServerMessage operationId = '{message.OperationId}', messageName = '{message.MessageName}'");
			SetCompleted(message);
			return Task.FromResult<object>(message);
		}

		public Task<object> HandleClientMessage(GrpcMessage message, IGrpcSender grpcSender)
		{
			Logger.LogDebug($"GrpcMethodInvokerHandler.HandleClientMessage operationId = '{message.OperationId}', messageName = '{message.MessageName}'");
			SetCompleted(message);
			return Task.FromResult<object>(message);
		}

		private void SetCompleted(GrpcMessage message)
		{
			Logger.LogTrace($"MethodTask.SetCompleted : operationId = '{message.OperationId}'");

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
