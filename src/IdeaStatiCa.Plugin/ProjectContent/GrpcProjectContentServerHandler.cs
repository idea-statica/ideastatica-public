using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Public;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.ProjectContent
{
	/// <summary>
	/// Responsible for accessing to data of a project
	/// </summary>
	public class GrpcProjectContentServerHandler : IGrpcMessageHandler
	{
		private IProjectContent projectContent;

		public GrpcProjectContentServerHandler()
		{

		}

		public IProjectContent ProjectContent { get => projectContent; private set => projectContent = value; }

		public Task<object> HandleClientMessage(GrpcMessage message, GrpcClient client)
		{
			throw new NotImplementedException();
		}

		public Task<object> HandleServerMessage(GrpcMessage message, GrpcServer server)
		{
			return Task.FromResult<object>(new object());
		}
	}
}
