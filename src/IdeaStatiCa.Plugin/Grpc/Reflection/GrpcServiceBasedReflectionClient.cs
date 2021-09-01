namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// WCF-like implementation of the GrpcReflectionClient that uses interceptors to proxy calls to server.
	/// </summary>
	/// <typeparam name="IReflectionService"></typeparam>
	public class GrpcServiceBasedReflectionClient<IReflectionService> : GrpcReflectionClient where IReflectionService : class
	{
		/// <summary>
		/// Service used to call server methods.
		/// </summary>
		public IReflectionService Service { get; private set; }

		public GrpcServiceBasedReflectionClient(string clientId, int port) : base(clientId, port)
		{
			Service = GrpcReflectionServiceFactory.CreateInstance<IReflectionService>(this);

			RegisterHandler(GrpcReflectionMessageHandler.GRPC_REFLECTION_HANDLER_MESSAGE, new GrpcReflectionMessageHandler(Service));
		}
	}
}
