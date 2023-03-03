using IdeaStatiCa.Public;
using System;
using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.Plugin.Grpc
{
	public class BlobStorageGrpc : IBlobStorage
	{
		readonly IGrpcBlobStorageClient grpcBlobStorageClient;
		readonly string blobStorageId;

		public BlobStorageGrpc(IGrpcBlobStorageClient grpcBlobStorageClient, string blobStorageId)
		{
			this.grpcBlobStorageClient = grpcBlobStorageClient;
			this.blobStorageId = blobStorageId;
		}

		public void Delete(string contentId)
		{
			grpcBlobStorageClient.DeleteAsync(blobStorageId, contentId).Wait();
		}

		public bool Exist(string contentId)
		{
			var existsTask = grpcBlobStorageClient.ExistAsync(blobStorageId, contentId);
			existsTask.Wait();
			return existsTask.Result;
		}

		public IReadOnlyCollection<string> GetEntries()
		{
			var getEntitiesTask = grpcBlobStorageClient.GetEntriesAsync(blobStorageId);
			getEntitiesTask.Wait();
			return getEntitiesTask.Result;
		}

		public void Init(string basePath)
		{
		}

		public Stream Read(string contentId)
		{
			var readTask = grpcBlobStorageClient.ReadAsync(blobStorageId, contentId);
			readTask.Wait();
			return readTask.Result;
		}

		public void Write(Stream content, string contentId)
		{
			grpcBlobStorageClient.WriteAsync(blobStorageId, contentId, content).Wait();
		}
	}
}
