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

		/// <summary>
		/// Initializes new <see cref="GrpcReflectionMessageHandler"/>
		/// </summary>
		/// <param name="instance">Instance for which the messages will be handled.</param>
		public GrpcReflectionMessageHandler(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentException("Instance cannot be null.");
			}

			this.instance = instance;
		}

		public async Task<object> HandleServerMessage(GrpcMessage message, GrpcServer server)
		{
			try
			{
				var grpcInvokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(message.Data);
				var arguments = grpcInvokeData.Parameters;
				var result = await ReflectionHelper.InvokeMethodFromGrpc(instance, grpcInvokeData.MethodName, arguments);
				var jsonResult = result != null ? JsonConvert.SerializeObject(result) : string.Empty;

				await server.SendMessageAsync(
						message.OperationId,
						message.MessageName,
						jsonResult
						);

				return result;
			}
			catch (Exception e)
			{
				await server.SendMessageAsync(message.OperationId, "Error", e.Message);

				return null;
			}
		}

		public Task<object> HandleClientMessage(GrpcMessage message, GrpcClient client)
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
