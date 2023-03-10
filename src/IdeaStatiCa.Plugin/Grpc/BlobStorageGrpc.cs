using IdeaStatiCa.Public;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
			Task.Run(async () =>
			{
				await grpcBlobStorageClient.DeleteAsync(blobStorageId, contentId);
			}).Wait();
		}

		public bool Exist(string contentId)
		{
			var resultTask = Task.Run(async () =>
			{
				return await grpcBlobStorageClient.ExistAsync(blobStorageId, contentId);
			});
			resultTask.Wait();
			return resultTask.Result;
		}

		public IReadOnlyCollection<string> GetEntries()
		{
			var resultTask = Task.Run(async () => { return await grpcBlobStorageClient.GetEntriesAsync(blobStorageId); });
			resultTask.Wait();
			return resultTask.Result;
		}

		public void Init(string basePath)
		{
			var resultTask = Task.Run(async () =>
			{
				return await grpcBlobStorageClient.ExistAsync(blobStorageId, "xxx");
			});
			resultTask.Wait();
		}

		public Stream Read(string contentId)
		{
			var readTask= Task.Run(async() => 
			{
				return await grpcBlobStorageClient.ReadAsync(blobStorageId, contentId).ConfigureAwait(false);
			});
			readTask.Wait();
			return readTask.Result;
		}

		public void Write(Stream content, string contentId)
		{
			Task.Run(async () => { await grpcBlobStorageClient.WriteAsync(blobStorageId, contentId, content); }).Wait();
		}

	}
}
