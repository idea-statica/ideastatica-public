using Autofac;
using Grpc.Core;
using IdeaStatiCa.PluginRunner.Services;
using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner
{
	public class PluginRunner
	{
		private readonly PluginServicesProvider _pluginServicesProvider;
		private readonly PluginLauncher _pluginLauncher;

		public static PluginRunner Create(int port)
		{
			Channel channel = new($"127.0.0.1:{port}", ChannelCredentials.Insecure);
			return new PluginRunner(channel);
		}

		public PluginRunner(Channel channel)
		{
			IContainer container = GetContainer(channel);

			_pluginServicesProvider = container.Resolve<PluginServicesProvider>();
			_pluginLauncher = container.Resolve<PluginLauncher>();
		}

		public Task<PluginLaunchResponse> Run(PluginLaunchRequest request) => _pluginLauncher.Launch(_pluginServicesProvider, request);

		public T GetService<T>()
		{
			object? service = _pluginServicesProvider.GetService(typeof(T));

			if (service is null)
			{
				throw new Exception();
			}

			return (T)service;
		}

		private static IContainer GetContainer(Channel channel)
		{
			ContainerBuilder builder = new();

			builder.RegisterInstance(channel).As<ChannelBase>();

			builder.RegisterType<Protos.ApplicationService.ApplicationServiceClient>().AsImplementedInterfaces();
			builder.RegisterType<Protos.EventService.EventServiceClient>().AsImplementedInterfaces();
			builder.RegisterType<Protos.StorageService.StorageServiceClient>().AsImplementedInterfaces();
			builder.RegisterType<Protos.PluginService.PluginServiceClient>().AsImplementedInterfaces();
			builder.RegisterType<Protos.ProjectService.ProjectServiceClient>().AsImplementedInterfaces();

			builder.RegisterType<ApplicationService>().AsImplementedInterfaces();
			builder.RegisterType<EventService>().AsImplementedInterfaces();
			builder.RegisterType<KeyValueStorage>().AsImplementedInterfaces();
			builder.RegisterType<PluginService>().AsImplementedInterfaces();
			builder.RegisterType<ProjectService>().AsImplementedInterfaces();

			builder.RegisterType<PluginServicesProvider>().SingleInstance();
			builder.RegisterType<PluginLaunchRequest>().SingleInstance();

			IContainer container = builder.Build();
			return container;
		}
	}
}