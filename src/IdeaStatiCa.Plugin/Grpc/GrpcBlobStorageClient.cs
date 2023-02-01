using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	public class GrpcBlobStorageClient : IDisposable
	{
		private readonly GrpcBlobStorageService.GrpcBlobStorageServiceClient client;
		private readonly Channel channel;
		private readonly IPluginLogger logger;
		private readonly int chunkSize;

		// todo comments

		public GrpcBlobStorageClient(IPluginLogger logger, int port, string host = "localhost", int chunkSize = Constants.GRPC_CHUNK_SIZE, int maxDataLength = Constants.GRPC_MAX_MSG_SIZE)
		{
			this.logger = logger;
			this.chunkSize = chunkSize;
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
				using (var call = client.Write(metadata))
				{
					var requestStream = call.RequestStream;
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

						await requestStream.WriteAsync(new ContentData()
						{
							Data = ByteString.CopyFrom(buffer)
						});
					}
					await requestStream.CompleteAsync();

					var res = await call.ResponseAsync;
				}
			}
			catch (Exception ex)
			{
				// todo
				logger.LogError("todo host, port,...", ex);
				throw;
			}
		}

		public async Task<Stream> ReadAsync(string blobStorageId, string contentId)
		{
			try
			{
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
						content.Position += currentData.Length;
					}
					content.Position = 0;
				}

				return content;
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

		public async Task DeleteAsync(string blobStorageId, string contentId)
		{
			// todo log, try catch
			var contentRequest = new ContentRequest()
			{
				BlobStorageId = blobStorageId,
				ContentId = contentId
			};

			_ = await client.DeleteAsync(contentRequest);
		}

		public async Task<List<string>> GetEntriesAsync(string blobStorageId)
		{
			var getEntriesRequest = new GetEntriesRequest()
			{
				BlobStorageId = blobStorageId
			};

			var response = await client.GetEntriesAsync(getEntriesRequest);
			return response.ContentId.ToList();
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
