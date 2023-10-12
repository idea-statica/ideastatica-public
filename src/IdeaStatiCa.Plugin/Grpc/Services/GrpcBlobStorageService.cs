using Google.Protobuf;
using Grpc.Core;
using IdeaStatiCa.Public;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Services
{
	/// <summary>
	/// Service in gRPC server for blob storage communication
	/// </summary>
	public class GrpcBlobStorageService : Grpc.GrpcBlobStorageService.GrpcBlobStorageServiceBase
	{
		private readonly IPluginLogger logger;
		private readonly IBlobStorageProvider blobStorageProvider;
		private readonly int chunkSize;

		/// <summary>
		/// Creates gRPC blob storage service
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="blobStorageProvider">Provider of blob storages</param>
		/// <param name="chunkSize">Size of chunk in bytes</param>
		public GrpcBlobStorageService(IPluginLogger logger, IBlobStorageProvider blobStorageProvider, int chunkSize = Constants.GRPC_CHUNK_SIZE)
		{
			this.logger = logger;
			this.blobStorageProvider = blobStorageProvider;
			this.chunkSize = chunkSize;

			logger.LogDebug($"Created GrpcBlobStorageService, chunkSize: '{chunkSize}'");
		}

		/// <summary>
		/// Writes data with content id to the specified blob storage. Blob storage id, content id & data are all present in the requestStream.
		/// </summary>
		/// <param name="requestStream">Streamed data received from client and to be written to the blob storage</param>
		/// <param name="context">Call context of gRPC communication</param>
		/// <returns></returns>
		public override async Task<VoidResponse> Write(IAsyncStreamReader<ContentData> requestStream, ServerCallContext context)
		{
			try
			{
				logger.LogDebug($"GrpcBlobStorageService begins Write");
				string blobStorageId = null;
				string contentId = null;

				using (var content = new MemoryStream())
				{
					ByteString currentData;
					while (await requestStream.MoveNext())
					{
						currentData = requestStream.Current.Data;
						blobStorageId = requestStream.Current.BlobStorageId;
						contentId = requestStream.Current.ContentId;

						currentData.WriteTo(content);
						logger.LogTrace($"GrpcBlobStorageService Write, blobStorageId: '{blobStorageId}', contentId: '{contentId}' received {currentData.Length} bytes");
					}
					content.Seek(0, SeekOrigin.Begin);
					if (blobStorageId == null)
					{
						throw new ArgumentException($"'{Constants.BlobStorageId}' is not defined in {nameof(ContentData)} object.");
					}
					if (contentId == null)
					{
						throw new ArgumentException($"'{Constants.ContentId}' is not defined in {nameof(ContentData)} object.");
					}

					blobStorageProvider.GetBlobStorage(blobStorageId).Write(content, contentId);
					logger.LogDebug($"GrpcBlobStorageService ends Write, blobStorageId: '{blobStorageId}', contentId: '{contentId}', content length in bytes: {content.Length}");
				}

				return new VoidResponse();
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageService Write failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Reads data with content id from the specified blob storage
		/// </summary>
		/// <param name="request">Request contains BlobStorageId and ContentId</param>
		/// <param name="responseStream">Streamed data to be read from the blob storage and sent to the client</param>
		/// <param name="context">Call context of gRPC communication</param>
		/// <returns></returns>
		public override async Task Read(ContentRequest request, IServerStreamWriter<ContentData> responseStream, ServerCallContext context)
		{
			try
			{
				using (var content = blobStorageProvider.GetBlobStorage(request.BlobStorageId).Read(request.ContentId))
				{

					logger.LogDebug($"GrpcBlobStorageService starts Read, blobStorageId: '{request.BlobStorageId}', contentId: '{request.ContentId}', content length in bytes: {content.Length}");

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

						await responseStream.WriteAsync(new ContentData()
						{
							Data = ByteString.CopyFrom(buffer),
							BlobStorageId = request.BlobStorageId,
							ContentId = request.ContentId,
						});

						logger.LogTrace($"GrpcBlobStorageService Read, blobStorageId: '{request.BlobStorageId}', contentId: '{request.ContentId}' sent {buffer.Length} bytes");
					}

					logger.LogDebug($"GrpcBlobStorageService ends Read, blobStorageId: '{request.BlobStorageId}', contentId: '{request.ContentId}'");
				}
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageService Read failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Determines if content with specified content id exists in specified blob storage
		/// </summary>
		/// <param name="request">Request contains BlobStorageId and ContentId</param>
		/// <param name="context">Call context of gRPC communication</param>
		/// <returns></returns>
		public override Task<ExistResponse> Exist(ContentRequest request, ServerCallContext context)
		{
			try
			{
				logger.LogDebug($"GrpcBlobStorageService Exist, blobStorageId: '{request.BlobStorageId}', contentId: '{request.ContentId}'");

				bool contentExists = blobStorageProvider.GetBlobStorage(request.BlobStorageId).Exist(request.ContentId);

				return Task.FromResult(new ExistResponse()
				{
					Exist = contentExists
				});
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageService Exist failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Deletes specified content in specified blob storage
		/// </summary>
		/// <param name="request">Request contains BlobStorageId and ContentId</param>
		/// <param name="context">Call context of gRPC communication</param>
		/// <returns></returns>
		public override Task<VoidResponse> Delete(ContentRequest request, ServerCallContext context)
		{
			try
			{
				logger.LogDebug($"GrpcBlobStorageService Delete, blobStorageId: '{request.BlobStorageId}', contentId: '{request.ContentId}'");

				blobStorageProvider.GetBlobStorage(request.BlobStorageId).Delete(request.ContentId);

				return Task.FromResult(new VoidResponse());
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageService Delete failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Get list of all content ids in specified blob storage
		/// </summary>
		/// <param name="request">Request contains BlobStorageId</param>
		/// <param name="context">Call context of gRPC communication</param>
		/// <returns></returns>
		public override Task<GetEntriesResponse> GetEntries(GetEntriesRequest request, ServerCallContext context)
		{
			try
			{
				logger.LogDebug($"GrpcBlobStorageService GetEntries, blobStorageId: '{request.BlobStorageId}'");

				var entries = blobStorageProvider.GetBlobStorage(request.BlobStorageId).GetEntries();

				var getEntriesResponse = new GetEntriesResponse();
				getEntriesResponse.ContentId.AddRange(entries);

				return Task.FromResult(getEntriesResponse);
			}
			catch (Exception ex)
			{
				logger.LogDebug("GrpcBlobStorageService Delete failed.", ex);
				throw;
			}
		}
	}
}
