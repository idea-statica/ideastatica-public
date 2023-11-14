using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.Utilities;
using System.Diagnostics;

namespace IdeaStatiCa.Plugin
{
	public class GrpcBimHostingFactory : IBimHostingFactory
	{
		private IdeaStatiCa.Plugin.Grpc.Services.GrpcService grpcService;
		private GrpcServer grpcServer;
		private GrpcServiceClient<IIdeaStaticaApp> checkBotClient;

		public IBIMPluginHosting Create(IBIMPluginFactory pluginFactory, IPluginLogger logger)
		{
			logger.LogDebug("GrpcBimHostingFactory.Create");
			if (checkBotClient != null)
			{
				Debug.Assert(grpcServer != null);
			}

			this.InitGrpcClient(logger);
			// It will be used for gRPC communication
			BIMPluginHostingGrpc pluginHostingGrpc = new BIMPluginHostingGrpc(pluginFactory, grpcServer, logger);
			if (pluginHostingGrpc.Service is ApplicationBIM appBim)
			{
				appBim.IdeaStaticaApp = checkBotClient.Service;
				// @Todo: better way to pass it, maybe a common IRemoteApp interface that implements both?
				if (checkBotClient.Service is IProgressMessaging)
				{
					appBim.Progress = checkBotClient.Service;
				}
			}

			return pluginHostingGrpc;
		}

		public IProgressMessaging InitGrpcClient(IPluginLogger logger)
		{
			if (checkBotClient == null)
			{
				logger.LogDebug("GrpcBimHostingFactory.InitGrpcClient - creating GrpcServiceClient<IIdeaStaticaApp>");

				int clientId = Process.GetCurrentProcess().Id;
				int grpcPort = PortFinder.FindPort(Constants.MinGrpcPort, Constants.MaxGrpcPort);

				grpcService = new IdeaStatiCa.Plugin.Grpc.Services.GrpcService(logger);

				grpcServer = new GrpcServer(logger, grpcService, null);
				grpcServer.StartAsync(clientId.ToString(), grpcPort);

				checkBotClient = new GrpcServiceClient<IIdeaStaticaApp>(Constants.GRPC_CHECKBOT_HANDLER_MESSAGE, grpcServer.GrpcService, grpcServer.Logger);
			}
			else
			{
				logger.LogDebug("GrpcBimHostingFactory.InitGrpcClient - returning the existing  GrpcServiceClient<IIdeaStaticaApp>");
			}

			return checkBotClient.Service is IProgressMessaging messaging ? messaging : null;
		}
	}
}