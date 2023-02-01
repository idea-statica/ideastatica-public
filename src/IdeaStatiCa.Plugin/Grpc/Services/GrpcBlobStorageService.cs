using Google.Protobuf;
using Grpc.Core;
using IdeaStatiCa.Public;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Services
{
	// todo comment
	public class GrpcBlobStorageService : Grpc.GrpcBlobStorageService.GrpcBlobStorageServiceBase
	{
		private readonly IPluginLogger logger;
		private readonly IBlobStorageProvider blobStorageProvider;
		private readonly int chunkSize;

		public GrpcBlobStorageService(IPluginLogger logger, IBlobStorageProvider blobStorageProvider, int chunkSize = Constants.GRPC_CHUNK_SIZE)
		{
			this.logger = logger;
			this.blobStorageProvider = blobStorageProvider;
			this.chunkSize = chunkSize;
		}
		public override async Task<VoidResponse> Write(IAsyncStreamReader<ContentData> requestStream, ServerCallContext context)
		{
			var metadata = context.RequestHeaders;
			var blobStorageId = metadata.GetValue(Constants.BlobStorageId);
			var contentId = metadata.GetValue(Constants.ContentId);
			
			if (blobStorageId == null)
			{
				throw new ArgumentException($"'{Constants.BlobStorageId}' is not defined in header metadata.");
			}
			if (contentId == null)
			{
				throw new ArgumentException($"'{Constants.ContentId}' is not defined in header metadata.");
			}

			using (var content = new MemoryStream())
			{
				ByteString currentData;
				while (await requestStream.MoveNext())
				{
					currentData = requestStream.Current.Data;
					currentData.WriteTo(content);
					content.Position += currentData.Length;
				}
				content.Position = 0;

				blobStorageProvider.GetBlobStorage(blobStorageId).Write(content, contentId);
			}

			return new VoidResponse();
		}

		public override async Task Read(ContentRequest request, IServerStreamWriter<ContentData> responseStream, ServerCallContext context)
		{
			var content = blobStorageProvider.GetBlobStorage(request.BlobStorageId).Read(request.ContentId);

			while (true)
			{
				byte[] buffer = new byte[chunkSize];
				int numRead = await content.ReadAsync(buffer, 0, buffer.Length);
				if (numRead == 0)
				{
					break;
				}
				if (numRead < buffer.Length)
				{
					Array.Resize(ref buffer, numRead);
				}

				await responseStream.WriteAsync(new ContentData()
				{
					Data = ByteString.CopyFrom(buffer)
				});
			}
		}

		public override Task<ExistResponse> Exist(ContentRequest request, ServerCallContext context)
		{
			var exist = blobStorageProvider.GetBlobStorage(request.BlobStorageId).Exist(request.ContentId);

			return Task.FromResult(new ExistResponse()
			{
				Exist = exist
			});
		}

		public override Task<VoidResponse> Delete(ContentRequest request, ServerCallContext context)
		{
			blobStorageProvider.GetBlobStorage(request.BlobStorageId).Delete(request.ContentId);

			return Task.FromResult(new VoidResponse());
		}

		public override Task<GetEntriesResponse> GetEntries(GetEntriesRequest request, ServerCallContext context)
		{
			var entries = blobStorageProvider.GetBlobStorage(request.BlobStorageId).GetEntries();

			var getEntriesResponse = new GetEntriesResponse();
			getEntriesResponse.ContentId.AddRange(entries);

			return Task.FromResult(getEntriesResponse);
		}
	}
}
