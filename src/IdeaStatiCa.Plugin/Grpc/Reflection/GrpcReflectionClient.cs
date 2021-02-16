﻿using IdeaStatiCa.Plugin.Utilities;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// Client for the <see cref="GrpcReflectionMessageHandler"/> enabled <see cref="GrpcClient"/>
	/// </summary>
	public class GrpcReflectionClient
	{
		private GrpcSynchronousClient client;

		/// <summary>
		/// Determines whether the client is connected.
		/// </summary>
		public bool IsConnected => client.IsConnected;

		/// <summary>
		/// Initializes the <see cref="GrpcReflectionClient"/>
		/// </summary>
		/// <param name="clientId">ID of the client.</param>
		/// <param name="port">Port on which <see cref="GrpcServer"/> is running.</param>
		public GrpcReflectionClient(string clientId, int port)
		{
			client = new GrpcSynchronousClient(clientId, port);
		}

		/// <summary>
		/// Connects the client to the server.
		/// </summary>
		/// <returns></returns>
		public async Task ConnectAsync()
		{
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
		public async Task<T> InvokeMethodAsync<T>(string methodName, params object[] arguments)
		{
			var parsedArgs = ReflectionHelper.GetMethodInvokeArguments(arguments);
			var request = new GrpcReflectionInvokeData()
			{
				MethodName = methodName,
				Parameters = parsedArgs
			};
			var data = JsonConvert.SerializeObject(request);
			var response = await client.SendMessageSync(GrpcReflectionMessageHandler.GRPC_REFLECTION_HANDLER_MESSAGE, data);

			// hadnle response
			var responseData = JsonConvert.DeserializeObject<T>(response.Data);

			return responseData;
		}

		public async Task<object> InvokeMethodAsync(Type returnType, string methodName, params object[] arguments)
		{
			var parsedArgs = ReflectionHelper.GetMethodInvokeArguments(arguments);
			var request = new GrpcReflectionInvokeData()
			{
				MethodName = methodName,
				Parameters = parsedArgs
			};
			var data = JsonConvert.SerializeObject(request);
			var response = await client.SendMessageSync(GrpcReflectionMessageHandler.GRPC_REFLECTION_HANDLER_MESSAGE, data);

			// hadnle response
			var responseData = JsonConvert.DeserializeObject(response.Data, returnType);

			return responseData;
		}
	}
}
