using IdeaStatiCa.Plugin.Utilities;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// Client for the <see cref="GrpcReflectionMessageHandler"/> enabled <see cref="GrpcClient"/>
	/// </summary>
	public class GrpcReflectionClient : IMethodInvoker
	{
		private IGrpcSynchronousClient client;

		protected readonly IPluginLogger Logger;

		/// <summary>
		/// Determines whether the client is connected.
		/// </summary>
		public bool IsConnected => client.IsConnected;

		public IGrpcSynchronousClient GrpcSyncClient => client;

		/// <summary>
		/// Initializes the <see cref="GrpcReflectionClient"/>
		/// </summary>
		/// <param name="clientId">ID of the client.</param>
		/// <param name="port">Port on which <see cref="GrpcServer"/> is running.</param>
		/// <param name="logger"></param>
		public GrpcReflectionClient(string clientId, int port, IPluginLogger logger)
		{
			Debug.Assert(logger != null);
			this.Logger = logger;
			client = new GrpcSynchronousClient(clientId, port, logger);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="client">Client for grpc communication</param>
		internal GrpcReflectionClient(IGrpcSynchronousClient client)
		{
			this.client = client;
		}

		/// <summary>
		/// Registers a message handler.
		/// </summary>
		/// <param name="handlerId">UniqueID of the handler.</param>
		/// <param name="handler">Handler implementation.</param>
		public void RegisterHandler(string handlerId, IGrpcMessageHandler handler)
		{
			Logger.LogDebug($"GrpcReflectionClient.GrpcReflectionClient handlerId = {handlerId}, handler = ${handler.GetType().Name}");
			client.RegisterHandler(handlerId, handler);
		}

		/// <summary>
		/// Connects the client to the server.
		/// </summary>
		/// <returns></returns>
		public async Task ConnectAsync()
		{
			Logger.LogDebug($"GrpcReflectionClient.ConnectAsync()");
			await client.ConnectAsync();
		}

		/// <summary>
		/// Disconnects client from the server.
		/// </summary>
		/// <returns></returns>
		public async Task DisconnectAsync()
		{
			await client.DisconnectAsync();
		}

		/// <summary>
		/// Invokes remote method over Grpc via reflection.
		/// </summary>
		/// <typeparam name="T">Type to which the result will be converted.</typeparam>
		/// <param name="methodName">Name of the method to invoke.</param>
		/// <param name="arguments">Arguments with which the method will be called.</param>
		/// <returns></returns>
		public T InvokeMethod<T>(string methodName, params object[] arguments)
		{
			var parsedArgs = ReflectionHelper.GetMethodInvokeArguments(arguments);
			var request = new GrpcReflectionInvokeData()
			{
				MethodName = methodName,
				Parameters = parsedArgs
			};
			var data = JsonConvert.SerializeObject(request);
			var response = client.SendMessageDataSync(Constants.GRPC_REFLECTION_HANDLER_MESSAGE, data);

			// hadnle response
			var responseData = JsonConvert.DeserializeObject<T>(response.Data);

			return responseData;
		}
	}
}
