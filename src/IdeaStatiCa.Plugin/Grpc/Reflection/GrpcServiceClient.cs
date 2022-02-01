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
		/// <param name="grpcCommunicator">gRPC communicator</param>
		public GrpcServiceClient(string handlerName, IGrpcCommunicator grpcCommunicator, IPluginLogger logger)
		{
			this.Logger = logger;
			HandlerName = handlerName;
			var grpcReflectionHandler = new GrpcMethodInvokerHandler(HandlerName, grpcCommunicator, logger);
			Service = GrpcReflectionServiceFactory.CreateInstance<ServiceType>(grpcReflectionHandler);

			grpcCommunicator.RegisterHandler(HandlerName, grpcReflectionHandler);
		}

		/// <summary>
		/// Client of the service
		/// </summary>
		public ServiceType Service { get; private set; }
	}
}
