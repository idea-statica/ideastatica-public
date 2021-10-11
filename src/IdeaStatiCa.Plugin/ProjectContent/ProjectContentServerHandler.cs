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
		private IProjectContent projectContent;

		public ProjectContentServerHandler()
		{

		}

		public IProjectContent ProjectContent { get => projectContent; private set => projectContent = value; }

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

				//grpcInvokeData.MethodName

				return Task.FromResult<object>(new object());
			}
			catch (Exception e)
			{
				await server.SendMessageAsync(message.OperationId, "Error", e.Message);

				return null;
			}
		}
	}
}
