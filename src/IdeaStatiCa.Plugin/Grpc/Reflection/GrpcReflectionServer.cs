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
		/// <param name="port">Port on which server will run.</param>
		/// <param name="logger">logger</param>
		public GrpcReflectionServer(object instance, int port, IPluginLogger logger) : base(port, logger)
		{
			RegisterHandler(Constants.GRPC_REFLECTION_HANDLER_MESSAGE, new GrpcReflectionMessageHandler(instance));
		}
	}
}
