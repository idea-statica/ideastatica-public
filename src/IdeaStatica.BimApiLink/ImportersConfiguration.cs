using IdeaStatica.BimApiLink.Importers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink
{
	public sealed class ImportersConfiguration
	{
		internal ImporterManager Manager { get; } = new ImporterManager();

		public ImportersConfiguration RegisterProvider(IImporterProvider provider)
		{
			Manager.RegisterProvider(provider);
			return this;
		}

		public ImportersConfiguration RegisterImporter<T>(IImporter<T> importer)
			where T : IIdeaObject
		{
			Manager.RegisterImporter(importer);
			return this;
		}

		public ImportersConfiguration RegisterImporter<T>(Func<IImporter<T>> factory)
			where T : IIdeaObject
		{
			RegisterProvider(new FuncProviderAdapter<T>(factory));
			return this;
		}

		public ImportersConfiguration RegisterContainer(IServiceProvider serviceProvider)
		{
			Manager.RegisterProvider(new ServiceProviderAdapter(serviceProvider));
			return this;
		}

		private sealed class FuncProviderAdapter<T> : IImporterProvider
			where T : IIdeaObject
		{
			private readonly Func<IImporter<T>> _factory;

			public FuncProviderAdapter(Func<IImporter<T>> factory)
			{
				_factory = factory;
			}

			public IImporter? GetProvider(Type type)
			{
				if (type != typeof(T))
				{
					return null;
				}

				return _factory();
			}
		}

		private sealed class ServiceProviderAdapter : IImporterProvider
		{
			private readonly IServiceProvider _serviceProvider;

			public ServiceProviderAdapter(IServiceProvider serviceProvider)
			{
				_serviceProvider = serviceProvider;
			}

			public IImporter? GetProvider(Type type)
			{
				Type providerType = typeof(IImporter<>).MakeGenericType(type);
				return (IImporter?)_serviceProvider.GetService(providerType);
			}
		}
	}
}