using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Public;
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
			throw new NotImplementedException();
		}

		public bool Exist(string contentId)
		{
			throw new NotImplementedException();
		}

		public Stream Get(string contentId)
		{
			throw new NotImplementedException();
		}

		public List<ProjectDataItem> GetContent()
		{
			throw new NotImplementedException();
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
	}
}
