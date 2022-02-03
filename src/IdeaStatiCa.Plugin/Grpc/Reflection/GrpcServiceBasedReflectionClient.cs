namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// WCF-like implementation of the GrpcReflectionClient that uses interceptors to proxy calls to server.
	/// </summary>
	/// <typeparam name="IReflectionService"></typeparam>
	public class GrpcServiceBasedReflectionClient<IReflectionService> : GrpcClient where IReflectionService : class
	{
		/// <summary>
		/// Service used to call server methods.
		/// </summary>
		public IReflectionService Service { get; private set; }

		public GrpcServiceBasedReflectionClient(IPluginLogger logger) : base(logger)
		{
			var grpcReflectionHandler = new GrpcMethodInvokerHandler(IdeaStatiCa.Plugin.Constants.GRPC_REFLECTION_HANDLER_MESSAGE, this, logger);
			Service = GrpcReflectionServiceFactory.CreateInstance<IReflectionService>(grpcReflectionHandler);
			RegisterHandler(IdeaStatiCa.Plugin.Constants.GRPC_REFLECTION_HANDLER_MESSAGE, grpcReflectionHandler);
		}
	}
}
