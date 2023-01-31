using Grpc.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	public class GrpcBlobStorageClient : IDisposable
	{
		private readonly GrpcBlobStorageService.GrpcBlobStorageServiceClient client;
		private readonly Channel channel;
		private readonly IPluginLogger logger;

		// todo comments

		public GrpcBlobStorageClient(IPluginLogger logger, int port, string host = "localhost", int maxDataLength = Constants.GRPC_MAX_MSG_SIZE)
		{
			this.logger = logger;
			channel = new Channel(host, port, ChannelCredentials.Insecure, CommunicationTools.GetChannelOptions(maxDataLength));
			client = new GrpcBlobStorageService.GrpcBlobStorageServiceClient(channel);

			logger.LogDebug($"Created GrpcBlobStorageClient, host: '{host}', port: '{port}', maxDataLength: '{maxDataLength}'");
		}

		public async Task WriteAsync(string blobStorageId, string contentId, Stream content)
		{
			var metadata = new Metadata();
			metadata.Add(Constants.BlobStorageId, blobStorageId);
			metadata.Add(Constants.ContentId, contentId);

			try
			{
				// todo like AddPhoto
				await client.Write(metadata);
			}
			catch (Exception ex)
			{
				// todo
				logger.LogError("todo host, port,...", ex);
				throw;
			}
		}

		public async Task<bool> ExistAsync(string blobStorageId, string contentId)
		{
			// todo log, try catch
			var contentRequest = new ContentRequest()
			{
				BlobStorageId = blobStorageId,
				ContentId = contentId
			};

			var response = await client.ExistAsync(contentRequest);
			return response.Exist;
		}

		public void Dispose()
		{
			try
			{
				channel.ShutdownAsync().Wait();
			}
			catch (Exception ex)
			{
				logger.LogInformation("Dispose of GrpcBlobStorageClient instance failed.", ex);
			}
		}
	}
}
