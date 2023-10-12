using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	public interface IGrpcBlobStorageClient
	{
		Task DeleteAsync(string blobStorageId, string contentId);
		void Dispose();
		Task<bool> ExistAsync(string blobStorageId, string contentId);
		Task<List<string>> GetEntriesAsync(string blobStorageId);
		Task<Stream> ReadAsync(string blobStorageId, string contentId);
		Task WriteAsync(string blobStorageId, string contentId, Stream content);
	}

	/// <summary>
	/// Client for gRPC blob storage communication
	/// </summary>
	public class GrpcBlobStorageClient : IDisposable, IGrpcBlobStorageClient
	{
		private readonly GrpcBlobStorageService.GrpcBlobStorageServiceClient client;
		private readonly Channel channel;
		private readonly IPluginLogger logger;
		private readonly int chunkSize;

		/// <summary>
		/// Creates gRPC blob storage client
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="port">Port</param>
		/// <param name="host">Host</param>
		/// <param name="chunkSize">Size of chunk in bytes</param>
		/// <param name="maxDataLength">Maximum message length in bytes for channel</param>
		public GrpcBlobStorageClient(IPluginLogger logger, int port, string host = "localhost", int chunkSize = Constants.GRPC_CHUNK_SIZE, int maxDataLength = Constants.GRPC_MAX_MSG_SIZE)
		{
			this.logger = logger;
			this.chunkSize = chunkSize;
			channel = new Channel(host, port, ChannelCredentials.Insecure, CommunicationTools.GetChannelOptions(maxDataLength));
			client = new GrpcBlobStorageService.GrpcBlobStorageServiceClient(channel);

			logger.LogDebug($"Created GrpcBlobStorageClient, host: '{host}', port: '{port}', chunkSize: '{chunkSize}', maxDataLength: '{maxDataLength}'");
		}

		/// <summary>
		/// Asynchronously writes data with content id to the specified blob storage
		/// </summary>
		/// <param name="blobStorageId">Blob storage id</param>
		/// <param name="contentId">Id of the data in blob storage</param>
		/// <param name="content">Source data</param>
		/// <returns></returns>
		public async Task WriteAsync(string blobStorageId, string contentId, Stream content)
		{
			logger.LogDebug($"GrpcBlobStorageClient starts Write, blobStorageId: '{blobStorageId}', contentId: '{contentId}', content length in bytes: {content.Length}");
			content.Seek(0, SeekOrigin.Begin);

			try
			{
				using (var call = client.Write())
				{
					var requestStream = call.RequestStream;
					var buffer = new byte[chunkSize];
					while (true)
					{
						int numRead = await content.ReadAsync(buffer, 0, buffer.Length);
						if (numRead == 0)
						{
							break;
						}
						if (numRead < buffer.Length)
						{
							Array.Resize(ref buffer, numRead);
						}

						await requestStream.WriteAsync(new ContentData()
						{
							Data = ByteString.CopyFrom(buffer),
							BlobStorageId = blobStorageId,
							ContentId = contentId
						});

						logger.LogTrace($"GrpcBlobStorageClient Write, blobStorageId: '{blobStorageId}', contentId: '{contentId}' sent {buffer.Length} bytes");
					}
					await requestStream.CompleteAsync();

					var res = await call.ResponseAsync;
					content.Seek(0, SeekOrigin.Begin);
				}

				logger.LogDebug($"GrpcBlobStorageClient ends Write, blobStorageId: '{blobStorageId}', contentId: '{contentId}'");
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageClient Write failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Asynchronously reads data with content id from the specified blob storage
		/// </summary>
		/// <param name="blobStorageId">Blob storage id</param>
		/// <param name="contentId">Id of the data in blob storage</param>
		/// <returns>Readed data</returns>
		public async Task<Stream> ReadAsync(string blobStorageId, string contentId)
		{
			try
			{
				logger.LogDebug($"GrpcBlobStorageClient begins Read, blobStorageId: '{blobStorageId}', contentId: '{contentId}'");

				var contentRequest = new ContentRequest()
				{
					BlobStorageId = blobStorageId,
					ContentId = contentId
				};

				var content = new MemoryStream();
				ByteString currentData;

				using (var call = client.Read(contentRequest))
				{
					var responseStream = call.ResponseStream;
					while (await responseStream.MoveNext())
					{
						currentData = responseStream.Current.Data;
						currentData.WriteTo(content);
						logger.LogTrace($"GrpcBlobStorageClient Read, blobStorageId: '{blobStorageId}', contentId: '{contentId}' received {currentData.Length} bytes");
					}
					content.Seek(0, SeekOrigin.Begin);
				}

				logger.LogDebug($"GrpcBlobStorageClient ends Read, blobStorageId: '{blobStorageId}', contentId: '{contentId}', content length in bytes: {content.Length}");
				return content;
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageClient Read failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Asynchronously calls Exists on specified blob storage for the content id
		/// </summary>
		/// <param name="blobStorageId">Blob storage id</param>
		/// <param name="contentId">Id of the data in blob storage</param>
		/// <returns>Returns true if content id exists, otherwise false</returns>
		public async Task<bool> ExistAsync(string blobStorageId, string contentId)
		{
			try
			{
				logger.LogDebug($"GrpcBlobStorageClient Exist, blobStorageId: '{blobStorageId}', contentId: '{contentId}'");

				var contentRequest = new ContentRequest()
				{
					BlobStorageId = blobStorageId,
					ContentId = contentId
				};

				var response = await client.ExistAsync(contentRequest);
				return response.Exist;
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageClient Exist failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Asynchronously calls Delete on specified blob storage for the content id
		/// </summary>
		/// <param name="blobStorageId">Blob storage id</param>
		/// <param name="contentId">Id of the data in blob storage</param>
		/// <returns></returns>
		public async Task DeleteAsync(string blobStorageId, string contentId)
		{
			try
			{
				logger.LogDebug($"GrpcBlobStorageClient Delete, blobStorageId: '{blobStorageId}', contentId: '{contentId}'");

				var contentRequest = new ContentRequest()
				{
					BlobStorageId = blobStorageId,
					ContentId = contentId
				};

				await client.DeleteAsync(contentRequest);
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageClient Delete failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Asynchronously calls GetEntries on specified blob storage
		/// </summary>
		/// <param name="blobStorageId">Blob storage id</param>
		/// <returns>Returns list of all content ids in specified blob storage</returns>
		public async Task<List<string>> GetEntriesAsync(string blobStorageId)
		{
			try
			{
				logger.LogDebug($"GrpcBlobStorageClient GetEntries, blobStorageId: '{blobStorageId}'");

				var getEntriesRequest = new GetEntriesRequest()
				{
					BlobStorageId = blobStorageId
				};

				var response = await client.GetEntriesAsync(getEntriesRequest);
				return response.ContentId.ToList();
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageClient GetEntries failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Shuts down communication channel with gRPC server
		/// </summary>
		public void Dispose()
		{
			try
			{
				if (!channel.ShutdownAsync().Wait(TimeSpan.FromMinutes(1)))
				{
					logger.LogDebug("Time out - gRPC client does not shut down channel within one minute.");
				}

				logger.LogDebug("Disposed GrpcBlobStorageClient");
			}
			catch (Exception ex)
			{
				logger.LogDebug("Dispose of GrpcBlobStorageClient failed.", ex);
			}
		}
	}
}
