using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.Utilities;
using System.Diagnostics;

namespace IdeaStatiCa.Plugin
{
	public class GrpcBimHostingFactory : IBimHostingFactory
	{
		private GrpcServer _grpcServer;
		private GrpcServiceClient<IIdeaStaticaApp> _checkBotClient;

		public IBIMPluginHosting Create(IBIMPluginFactory pluginFactory, IPluginLogger logger)
		{
			if (_checkBotClient != null)
			{
				Debug.Assert(_grpcServer != null);
			}

			this.InitGrpcClient(logger);
			// It will be used for gRPC communication
			BIMPluginHostingGrpc pluginHostingGrpc = new BIMPluginHostingGrpc(pluginFactory, _grpcServer, logger);
			if (pluginHostingGrpc.Service is ApplicationBIM appBim)
			{
				appBim.IdeaStaticaApp = _checkBotClient.Service;
				// @Todo: better way to pass it, maybe a common IRemoteApp interface that implements both?
				if (_checkBotClient.Service is IProgressMessaging)
				{
					appBim.Progress = _checkBotClient.Service;
				}
			}

			return pluginHostingGrpc;
		}

		public IProgressMessaging InitGrpcClient(IPluginLogger logger)
		{
			if (_checkBotClient == null)
			{
				int clientId = Process.GetCurrentProcess().Id;
				int grpcPort = PortFinder.FindPort(Constants.MinGrpcPort, Constants.MaxGrpcPort);

				_grpcServer = new GrpcServer(logger);
				_grpcServer.StartAsync(clientId.ToString(), grpcPort);

				_checkBotClient = new GrpcServiceClient<IIdeaStaticaApp>(Constants.GRPC_CHECKBOT_HANDLER_MESSAGE, _grpcServer.GrpcService, _grpcServer.Logger);
			}

			return _checkBotClient.Service is IProgressMessaging messaging ? messaging : null;
		}
	}
}