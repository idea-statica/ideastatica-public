using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Public;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.ProjectContent
{
	/// <summary>
	/// Storage of project  items
	/// </summary>
	public interface IProjectContentStorage
	{
		/// <summary>
		/// Get data from storage and write them to <paramref name="outputStream"/>
		/// </summary>
		/// <param name="contentId">valid Id of the requested project content.</param>
		/// <param name="outputStream">Open Stream for writing to store result.</param>
		/// <exception cref="System.Exception">Exception is thrown if the operation fails</exception>
		/// <returns>1</returns>
		int ReadData(string contentId, Stream outputStream);

		/// <summary>
		/// Write data to storage
		/// </summary>
		/// <param name="contentId">Name of the project item</param>
		/// <param name="inputData">Data of the project item</param>
		/// <exception cref="System.Exception">Exception is thrown if the operation fails</exception>
		/// <returns>1</returns>
		int WriteData(string contentId, Stream inputStream);
	}

	/// <summary>
	/// Implementation of <see cref="IProjectContent"/> for accessing project data of using grpc 
	/// </summary>
	public class ProjectContentClientHandler : GrpcMethodInvokerHandler, IProjectContent, IProjectContentStorage
	{
		readonly static string Error = "Error";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="client">Grpc client</param>
		public ProjectContentClientHandler(IGrpcSender client, IPluginLogger logger) : base(Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE, client, logger)
		{
		}

		#region IProjectContent
		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="sourceContent"></param>
		public void CopyContent(IProjectContent sourceContent)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="contentId"></param>
		/// <returns></returns>
		public Stream Create(string contentId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invoke method 'Delete' remotely
		/// </summary>
		/// <param name="contentId">Id of the item to delete</param>
		public void Delete(string contentId)
		{
			string data = string.Empty;
			var grpcMessage = new GrpcMessage();
			grpcMessage.MessageName = HandlerName;
			grpcMessage.MessageType = GrpcMessage.Types.MessageType.Request;

			var invokeData = new GrpcReflectionInvokeData();
			invokeData.MethodName = "Delete";
			var parameters = new List<GrpcReflectionArgument>();
			parameters.Add(new GrpcReflectionArgument(typeof(string).ToString(), contentId));
			invokeData.Parameters = parameters;
			grpcMessage.Data = JsonConvert.SerializeObject(invokeData);

			var grpcResult = SendMessageDataSync(grpcMessage);
			CheckError(grpcResult.Data);

			var projectContent = !string.IsNullOrEmpty(grpcResult?.Data) ? JsonConvert.DeserializeObject<bool>(grpcResult?.Data) : false;

			return;
		}

		/// <summary>
		/// Invoke method 'Exist' remotely
		/// </summary>
		/// <param name="contentId">Id of the project item</param>
		/// <returns>OK</returns>
		public bool Exist(string contentId)
		{
			string data = string.Empty;
			var grpcMessage = new GrpcMessage();
			grpcMessage.MessageName = HandlerName;
			grpcMessage.MessageType = GrpcMessage.Types.MessageType.Request;

			var invokeData = new GrpcReflectionInvokeData();
			invokeData.MethodName = "Exist";
			var parameters = new List<GrpcReflectionArgument>();
			parameters.Add(new GrpcReflectionArgument(typeof(string).ToString(), contentId));
			invokeData.Parameters = parameters;
			grpcMessage.Data = JsonConvert.SerializeObject(invokeData);

			var grpcResult = SendMessageDataSync(grpcMessage);
			CheckError(grpcResult.Data);

			var projectContent = JsonConvert.DeserializeObject<bool>(grpcResult?.Data);

			return projectContent;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contentId"></param>
		/// <returns></returns>
		public Stream Get(string contentId)
		{
			return new RemoteDataStream(contentId, this);
		}

		/// <summary>
		/// Get project content by invoging remote method 'GetContent' using GrpcClient
		/// </summary>
		/// <returns>List of all project items</returns>
		public List<ProjectDataItem> GetContent()
		{
			string data = string.Empty;
			var grpcMessage = new GrpcMessage();
			grpcMessage.MessageName = HandlerName;
			grpcMessage.MessageType = GrpcMessage.Types.MessageType.Request;

			var invokeData = new GrpcReflectionInvokeData();
			invokeData.MethodName = "GetContent";
			var parameters = new List<GrpcReflectionArgument>();
			invokeData.Parameters = parameters;
			grpcMessage.Data = JsonConvert.SerializeObject(invokeData);

			var grpcResult = SendMessageDataSync(grpcMessage);
			CheckError(grpcResult.Data);

			var projectContent = !string.IsNullOrEmpty(grpcResult?.Data) ? JsonConvert.DeserializeObject<List<ProjectDataItem>>(grpcResult?.Data) : null;

			return projectContent;
		} 
		#endregion

		#region IProjectContentStorage
		/// <summary>
		/// Get requested <paramref name="contentId"/> by invoking remote method 'Read' by GrpcClient and write them to <paramref name="outputStream"/>
		/// </summary>
		/// <param name="contentId">valid Id of the requested project content.</param>
		/// <param name="outputStream">Open Stream for witing to store result.</param>
		/// <exception cref="System.Exception">Exception is thrown if the operation fails</exception>
		/// <returns>Always 1</returns>
		public int ReadData(string contentId, Stream outputStream)
		{
			Debug.Assert(outputStream != null);
			Debug.Assert(!string.IsNullOrEmpty(contentId));

			string data = string.Empty;
			var grpcMessage = new GrpcMessage();
			grpcMessage.MessageName = HandlerName;

			var invokeData = new GrpcReflectionInvokeData();
			invokeData.MethodName = "Read";
			var parameters = new List<GrpcReflectionArgument>();
			parameters.Add(new GrpcReflectionArgument(typeof(string).ToString(), contentId));
			invokeData.Parameters = parameters;
			grpcMessage.Data = JsonConvert.SerializeObject(invokeData);

			var grpcResult = SendMessageDataSync(grpcMessage);
			CheckError(grpcResult.Data);

			grpcResult.Buffer.WriteTo(outputStream);
			return 1;
		}

		/// <summary>
		/// Invoke remote method 'Write' which writes data remotely.
		/// </summary>
		/// <param name="contentId">Name of the project item</param>
		/// <param name="inputData">Data of the project item</param>
		/// <exception cref="System.Exception">Exception is thrown if the operation fails</exception>
		/// <returns>Always 1</returns>
		public int WriteData(string contentId, Stream inputStream)
		{
			Debug.Assert(inputStream != null);
			Debug.Assert(!string.IsNullOrEmpty(contentId));

			string data = string.Empty;
			var grpcMessage = new GrpcMessage();
			grpcMessage.MessageName = HandlerName;

			var invokeData = new GrpcReflectionInvokeData();
			invokeData.MethodName = "Write";
			var parameters = new List<GrpcReflectionArgument>();
			parameters.Add(new GrpcReflectionArgument(typeof(string).ToString(), contentId));
			invokeData.Parameters = parameters;
			grpcMessage.Data = JsonConvert.SerializeObject(invokeData);
			grpcMessage.Buffer = Google.Protobuf.ByteString.FromStream(inputStream);

			var grpcResult = SendMessageDataSync(grpcMessage);
			CheckError(grpcResult.Data);

			return 1;
		}
		#endregion


		/// <summary>
		/// Validate <paramref name="data"/> and thow Exception if <paramref name="data"/> represents an error
		/// </summary>
		/// <param name="data">String to validate</param>
		void CheckError(string data)
		{
			if(!string.IsNullOrEmpty(data) && data.StartsWith(Error))
			{
				throw new Exception(data);
			}
		}
	}
}
