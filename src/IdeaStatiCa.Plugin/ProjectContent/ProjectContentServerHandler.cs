using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Public;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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

		public IProjectContent ContentSource { get => contentSource; private set => contentSource = value; }

		public Task<object> HandleClientMessage(GrpcMessage message, GrpcClient client)
		{
			throw new NotImplementedException();
		}

		public async Task<object> HandleServerMessage(GrpcMessage message, GrpcServer server)
		{
			try
			{
				var grpcInvokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(message.Data);
				var arguments = grpcInvokeData.Parameters;

				switch(grpcInvokeData.MethodName)
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
					default:
						throw new Exception($"HandleServerMessage: not supported method '{grpcInvokeData?.MethodName}' ");
				}

			}
			catch (Exception e)
			{
				await server.SendMessageAsync(message.OperationId, "Error", e.Message);

				return null;
			}
		}
	}
}
