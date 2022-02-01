namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// Client of the service which is hosted by gRPC server
	/// </summary>
	/// <typeparam name="ServiceType"></typeparam>
	public class GrpcServiceClient<ServiceType> where ServiceType : class
	{
		public readonly string HandlerName;
		internal IPluginLogger Logger { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="handlerName">Handler name</param>
		/// <param name="grpcServer">gRPC server</param>
		public GrpcServiceClient(string handlerName, GrpcServer grpcServer, IPluginLogger logger)
		{
			this.Logger = logger;
			HandlerName = handlerName;
			var grpcReflectionHandler = new GrpcMethodInvokerHandler(HandlerName, grpcServer, logger);
			Service = GrpcReflectionServiceFactory.CreateInstance<ServiceType>(grpcReflectionHandler);

			grpcServer.RegisterHandler(HandlerName, grpcReflectionHandler);
		}

		/// <summary>
		/// Client of the service
		/// </summary>
		public ServiceType Service { get; private set; }
	}
}
