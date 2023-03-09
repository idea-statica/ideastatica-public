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

		public void Delete(string contentId) =>
			RunSynchroniously(grpcBlobStorageClient.DeleteAsync(blobStorageId, contentId)).Wait();

		public bool Exist(string contentId)
		{
			var resultTask = RunSynchroniously(grpcBlobStorageClient.ExistAsync(blobStorageId, contentId));
			resultTask.Wait();
			return resultTask.Result;
		}

		public IReadOnlyCollection<string> GetEntries()
		{
			var resultTask = RunSynchroniously(grpcBlobStorageClient.GetEntriesAsync(blobStorageId));
			resultTask.Wait();
			return resultTask.Result;
		}

		public void Init(string basePath) =>
			throw new NotImplementedException("Init is not implemented");

		public Stream Read(string contentId)
		{
			var readTask= RunSynchroniously(grpcBlobStorageClient.ReadAsync(blobStorageId, contentId));
			readTask.Wait();
			return readTask.Result;
		}

		public void Write(Stream content, string contentId) =>
			RunSynchroniously(grpcBlobStorageClient.WriteAsync(blobStorageId, contentId, content)).Wait();

		private Task<T> RunSynchroniously<T>(Task<T> taskToRun)
		{
			return Task.Run(async () =>
			{
				return await taskToRun.ConfigureAwait(false);
			});
		}

		private Task RunSynchroniously(Task taskToRun)
		{
			return Task.Run(async () =>
			{
				await taskToRun.ConfigureAwait(false);
			});
		}
	}
}
