using Autofac;
using Grpc.Core;
using Grpc.Core.Interceptors;
using IdeaStatiCa.PluginRunner.Services;
using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner
{
	public class PluginRunner
	{
		private readonly PluginServicesProvider _pluginServicesProvider;
		private readonly PluginLauncher _pluginLauncher;

		public static PluginRunner Create(int port, string communicationId)
		{
			Channel channel = new($"127.0.0.1:{port}", ChannelCredentials.Insecure);
			return new PluginRunner(channel, communicationId);
		}

		public PluginRunner(Channel channel, string communicationId)
		{
			IContainer container = GetContainer(channel, communicationId);

			_pluginServicesProvider = container.Resolve<PluginServicesProvider>();
			_pluginLauncher = container.Resolve<PluginLauncher>();
		}

		public Task<PluginLaunchResponse> Run(PluginLaunchRequest request)
			=> _pluginLauncher.Launch(_pluginServicesProvider, request);

		public T GetService<T>()
		{
			object? service = _pluginServicesProvider.GetService(typeof(T));

			if (service is null)
			{
				throw new Exception();
			}

			return (T)service;
		}

		private static IContainer GetContainer(Channel channel, string communicationId)
		{
			ContainerBuilder builder = new();

			CallInvoker callInvoker = channel.Intercept(new SessionIdInterceptor(communicationId));
			builder.RegisterInstance(callInvoker);

			builder.RegisterType<Protos.ApplicationService.ApplicationServiceClient>();
			builder.RegisterType<Protos.EventService.EventServiceClient>();
			builder.RegisterType<Protos.StorageService.StorageServiceClient>();
			builder.RegisterType<Protos.PluginService.PluginServiceClient>();
			builder.RegisterType<Protos.ProjectService.ProjectServiceClient>();

			builder.RegisterType<ApplicationService>().AsImplementedInterfaces();
			builder.RegisterType<EventService>().AsImplementedInterfaces();
			builder.RegisterType<KeyValueStorage>().AsImplementedInterfaces();
			builder.RegisterType<PluginService>().AsImplementedInterfaces();
			builder.RegisterType<ProjectService>().AsImplementedInterfaces();

			builder.RegisterType<PluginServicesProvider>().SingleInstance();
			builder.RegisterType<PluginLauncher>().SingleInstance();

			IContainer container = builder.Build();
			return container;
		}
	}
}