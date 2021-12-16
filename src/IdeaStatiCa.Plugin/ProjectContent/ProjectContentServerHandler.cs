using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Public;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Google.Protobuf;

namespace IdeaStatiCa.Plugin.ProjectContent
{
	/// <summary>
	/// Responsible for accessing to data of a project
	/// </summary>
	public class ProjectContentServerHandler : IGrpcMessageHandler
	{
		private IProjectContent contentSource;

		public ProjectContentServerHandler(IProjectContent projectContent)
		{
			this.contentSource = projectContent;
		}

		/// <summary>
		/// Get or set the source of data
		/// </summary>
		public IProjectContent ContentSource { get => contentSource; private set => contentSource = value; }

		public Task<object> HandleClientMessage(GrpcMessage message, IGrpcSender grpcSender)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invoke methods implemented in the instance <see cref="contentSource"/> and send result back to a caller by <paramref name="server"/>
		/// </summary>
		/// <param name="message">Request</param>
		/// <param name="server">Server to be used to send response to a caller</param>
		/// <returns></returns>
		public async Task<object> HandleServerMessage(GrpcMessage message, IGrpcSender grpcSender)
		{
			GrpcServer server = (GrpcServer)grpcSender;
			try
			{
				var grpcInvokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(message.Data);
				var arguments = grpcInvokeData.Parameters;

				switch (grpcInvokeData.MethodName)
				{
					case "GetContent":
						{
							var result = ContentSource.GetContent();
							var jsonResult = result != null ? JsonConvert.SerializeObject(result) : string.Empty;

							await server.SendMessageAsync(
									message.OperationId,
									message.MessageName,
									jsonResult
									);
							return Task.FromResult(true);
						}
					case "Delete":
						{
							var arg1 = arguments.First();
							var contentId = arg1.Value.ToString();
							ContentSource.Delete(contentId);
							var jsonResult = "OK";

							await server.SendMessageAsync(
									message.OperationId,
									message.MessageName,
									jsonResult
									);
							return Task.FromResult(true);
						}
					case "Exist":
						{
							var arg1 = arguments.First();
							var contentId = arg1.Value.ToString();
							var contentExist = ContentSource.Exist(contentId);
							var jsonResult = JsonConvert.SerializeObject(contentExist);

							await server.SendMessageAsync(
									message.OperationId,
									message.MessageName,
									jsonResult
									);
							return Task.FromResult(true);
						}
					case "Write":
						{
							var arg1 = arguments.First();
							var contentId = arg1.Value.ToString();
							Stream destStream = null;
							try
							{
								if (!ContentSource.Exist(contentId))
								{
									destStream = ContentSource.Create(contentId);
								}
								else
								{
									destStream = ContentSource.Get(contentId);
								}

								message.Buffer.WriteTo(destStream);
							}
							finally
							{
								if(destStream != null)
								{
									destStream.Dispose();
								}
							}
							message.Data = "OK";

							await server.SendMessageAsync(message);
							return Task.FromResult(true);
						}
					case "Read":
						{
							var arg1 = arguments.First();
							var contentId = arg1.Value.ToString();
							if(ContentSource.Exist(contentId))
							{
								using (var srcStream = ContentSource.Get(contentId))
								{
									message.Buffer = ByteString.FromStream(srcStream);
								}
							}
							else
							{
								message.Buffer = ByteString.Empty;
							}
	
							message.Data = "OK";

							await server.SendMessageAsync(message);
							return Task.FromResult(true);
						}
					default:
						throw new Exception($"Error HandleServerMessage: not supported method '{grpcInvokeData?.MethodName}' ");
				}

			}
			catch (Exception e)
			{
				await server.SendMessageAsync(message.OperationId, "Error", $"Error '{e.Message}'");

				return null;
			}
		}
	}
}
