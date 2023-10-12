using IdeaStatiCa.Public;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// Instance of GrpcServer with Reflection Service added.
	/// </summary>
	public class GrpcReflectionServer : GrpcServer
	{
		/// <summary>
		/// Creates a grpc reflection service for specified instance.
		/// </summary>
		/// <param name="instance">Instance targeted by reflection calls.</param>
		/// <param name="logger">pluginLogger</param>
		/// <param name="blobStorageProvider">Provider of blob storages</param>
		/// <param name="maxDataLength">The maximal size of GrpcMessage.data in bytes in grpc message</param>
		/// <param name="chunkSize">Size of one chunk in bytes for blob storage data transferring</param>
		public GrpcReflectionServer(object instance, IPluginLogger logger, IBlobStorageProvider blobStorageProvider, int maxDataLength = Constants.GRPC_MAX_MSG_SIZE, int chunkSize = Constants.GRPC_CHUNK_SIZE) :
			base(logger, new Services.GrpcService(logger, maxDataLength), blobStorageProvider, maxDataLength, chunkSize)
		{
			logger.LogDebug("GrpcReflectionServer");

			GrpcService.RegisterHandler(Constants.GRPC_REFLECTION_HANDLER_MESSAGE, new GrpcReflectionMessageHandler(instance, logger));
		}
	}
}
