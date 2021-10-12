using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Public;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.ProjectContent
{
	/// <summary>
	/// Implementation of <see cref="IProjectContent"/> for accessing project data of using grpc 
	/// </summary>
	public class ProjectContentClientHandler : IGrpcMessageHandler, IProjectContent
	{
		readonly IGrpcSynchronousClient syncClient;
		readonly string HandlerName;
		readonly static string Error = "Error";

		public ProjectContentClientHandler(IGrpcSynchronousClient client)
		{
			this.HandlerName = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;
			this.syncClient = client;
		}

		#region IProjectContent
		public void CopyContent(IProjectContent sourceContent)
		{
			throw new NotImplementedException();
		}

		public Stream Create(string contentId)
		{
			throw new NotImplementedException();
		}

		public void Delete(string contentId)
		{
			string data = string.Empty;
			var grpcMessage = new GrpcMessage();
			grpcMessage.MessageName = HandlerName;

			var invokeData = new GrpcReflectionInvokeData();
			invokeData.MethodName = "Delete";
			var parameters = new List<GrpcReflectionArgument>();
			parameters.Add(new GrpcReflectionArgument(typeof(string).ToString(), contentId));
			invokeData.Parameters = parameters;
			grpcMessage.Data = JsonConvert.SerializeObject(invokeData);

			var grpcResult = syncClient.SendMessageDataSync(grpcMessage);
			CheckError(grpcResult.Data);

			var projectContent = !string.IsNullOrEmpty(grpcResult?.Data) ? JsonConvert.DeserializeObject<bool>(grpcResult?.Data) : false;

			return;
		}

		public bool Exist(string contentId)
		{
			string data = string.Empty;
			var grpcMessage = new GrpcMessage();
			grpcMessage.MessageName = HandlerName;

			var invokeData = new GrpcReflectionInvokeData();
			invokeData.MethodName = "Exist";
			var parameters = new List<GrpcReflectionArgument>();
			parameters.Add(new GrpcReflectionArgument(typeof(string).ToString(), contentId));
			invokeData.Parameters = parameters;
			grpcMessage.Data = JsonConvert.SerializeObject(invokeData);

			var grpcResult = syncClient.SendMessageDataSync(grpcMessage);
			CheckError(grpcResult.Data);

			var projectContent = JsonConvert.DeserializeObject<bool>(grpcResult?.Data);

			return projectContent;
		}

		public Stream Get(string contentId)
		{
			throw new NotImplementedException();
		}

		public List<ProjectDataItem> GetContent()
		{
			string data = string.Empty;
			var grpcMessage = new GrpcMessage();
			grpcMessage.MessageName = HandlerName;

			var invokeData = new GrpcReflectionInvokeData();
			invokeData.MethodName = "GetContent";
			var parameters = new List<GrpcReflectionArgument>();
			invokeData.Parameters = parameters;
			grpcMessage.Data = JsonConvert.SerializeObject(invokeData);

			var grpcResult = syncClient.SendMessageDataSync(grpcMessage);
			CheckError(grpcResult.Data);

			var projectContent = !string.IsNullOrEmpty(grpcResult?.Data) ? JsonConvert.DeserializeObject<List<ProjectDataItem>>(grpcResult?.Data) : null;

			return projectContent;
		} 
		#endregion

		#region IGrpcMessageHandler
		public Task<object> HandleClientMessage(GrpcMessage message, GrpcClient client)
		{
			throw new NotImplementedException();
		}

		public Task<object> HandleServerMessage(GrpcMessage message, GrpcServer server)
		{
			throw new NotImplementedException();
		} 
		#endregion

		void CheckError(string data)
		{
			if(!string.IsNullOrEmpty(data) && data.StartsWith(Error))
			{
				throw new Exception(data);
			}
		}
	}
}
