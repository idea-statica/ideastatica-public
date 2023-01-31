using Grpc.Core;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Services
{
	// todo comment
	public class GrpcBlobStorageService : Grpc.GrpcBlobStorageService.GrpcBlobStorageServiceBase
	{
		public override async Task<VoidResponse> Write(IAsyncStreamReader<ContentData> requestStream, ServerCallContext context)
		{
			// todo implement
			return await base.Write(requestStream, context);
		}

		public override Task Read(ContentRequest request, IServerStreamWriter<ContentData> responseStream, ServerCallContext context)
		{
			// todo implement, async
			return base.Read(request, responseStream, context);
		}

		public override async Task<ExistResponse> Exist(ContentRequest request, ServerCallContext context)
		{
			// todo implement
			var existResponse = new ExistResponse()
			{
				Exist = false
			};
			return await Task.FromResult(existResponse);
		}

		public override async Task<VoidResponse> Delete(ContentRequest request, ServerCallContext context)
		{
			// todo implement
			return await base.Delete(request, context);
		}

		public override async Task<GetEntriesResponse> GetEntries(GetEntriesRequest request, ServerCallContext context)
		{
			// todo implement
			return await base.GetEntries(request, context);
		}
	}
}
