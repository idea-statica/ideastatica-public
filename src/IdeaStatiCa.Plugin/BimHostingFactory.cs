using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.Utilities;
using System.Diagnostics;

namespace IdeaStatiCa.Plugin
{
	public class GrpcBimHostingFactory : IBimHostingFactory
	{
		private IPluginLogger Logger { get; set; }
		private IBIMPluginFactory PluginFactory { get; set; }

		public GrpcBimHostingFactory(IBIMPluginFactory pluginFactory, IPluginLogger logger)
		{
			this.Logger = logger;
			this.PluginFactory = pluginFactory;
		}
		public IBIMPluginHosting Create(GrpcServiceClient<IIdeaStaticaApp> checkBotClient = null, GrpcServer grpcServer = null)
		{
			int clientId = Process.GetCurrentProcess().Id;
			int grpcPort = PortFinder.FindPort(Constants.MinGrpcPort, Constants.MaxGrpcPort);

			grpcServer = grpcServer ?? new GrpcServer(Logger);
			grpcServer.Connect(clientId.ToString(), grpcPort);
			
			grpcServer.StartAsync();

			checkBotClient = checkBotClient ?? new GrpcServiceClient<IIdeaStaticaApp>(Constants.GRPC_CHECKBOT_HANDLER_MESSAGE, grpcServer, grpcServer.Logger);

			// It will be used for gRPC communication
			var pluginHostingGrpc = new BIMPluginHostingGrpc(PluginFactory, grpcServer, grpcServer.Logger);
			if (pluginHostingGrpc.Service is ApplicationBIM appBim)
			{
				appBim.IdeaStaticaApp = checkBotClient.Service;
			}

			return pluginHostingGrpc;
		}
	}
}
