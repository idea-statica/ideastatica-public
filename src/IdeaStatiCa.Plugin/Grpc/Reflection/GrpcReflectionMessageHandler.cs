using IdeaStatiCa.Plugin.Utilities;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// Handler that invokes methods for specified object over Grpc via reflection.
	/// </summary>
	public class GrpcReflectionMessageHandler : IGrpcMessageHandler<object>
	{
		private object instance;

		public bool IsSynchronous => true;
		internal IPluginLogger Logger { get; private set; }

		/// <summary>
		/// Initializes new <see cref="GrpcReflectionMessageHandler"/>
		/// </summary>
		/// <param name="instance">Instance for which the messages will be handled.</param>
		public GrpcReflectionMessageHandler(object instance, IPluginLogger logger)
		{
			if (instance == null)
			{
				throw new ArgumentException("Instance cannot be null.");
			}

			Logger = logger;
			this.instance = instance;
		}

		public async Task<object> HandleServerMessage(GrpcMessage message, IGrpcSender grpcSender)
		{
			try
			{
				var grpcInvokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(message.Data);
				var arguments = grpcInvokeData.Parameters;
				var result = await ReflectionHelper.InvokeMethodFromGrpc(instance, grpcInvokeData.MethodName, arguments);

				var jsonResult = result != null ? JsonConvert.SerializeObject(result) : string.Empty;

				string dataType = result != null ? result.GetType().Name : string.Empty;

				var responseMsg = new GrpcMessage()
				{
					ClientId = message.ClientId,
					OperationId = message.OperationId,
					MessageName = message.MessageName,
					MessageType = GrpcMessage.Types.MessageType.Response,
					Data = jsonResult,
					DataType = dataType,
				};


				await grpcSender.SendMessageAsync(responseMsg);

				return result;
			}
			catch (Exception e)
			{
				var errMsg = new GrpcMessage()
				{
					OperationId = message.OperationId,
					MessageType = GrpcMessage.Types.MessageType.Response,
					MessageName = "Error",
					Data = e.Message,
					DataType = typeof(string).Name
				};

				await grpcSender.SendMessageAsync(errMsg);

				return null;
			}
		}

		public Task<object> HandleClientMessage(GrpcMessage message, IGrpcSender client)
		{
			var callback = JsonConvert.DeserializeObject<GrpcReflectionCallbackData>(message.Data);
			if(callback == null)
			{
				object res = null;
				return Task<object>.FromResult(res);
			}

			var value = callback.Value != null ? JsonConvert.DeserializeObject(callback.Value.ToString(), Type.GetType(callback.ValueType)) : null;
			return Task<object>.FromResult(value);
		}

	}
}
