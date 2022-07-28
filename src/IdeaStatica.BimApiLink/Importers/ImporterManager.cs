using IdeaStatiCa.BimApi;
using System.Diagnostics.CodeAnalysis;

namespace IdeaStatica.BimApiLink.Importers
{
	internal class ImporterManager
	{
		private readonly Dictionary<Type, IImporter> _importers = new();
		private readonly List<IImporterProvider> _providers = new();

		public void RegisterImporter<T>(IImporter<T> importer)
			where T : IIdeaObject => _importers.Add(typeof(T), importer);

		public void RegisterProvider(IImporterProvider provider) => _providers.Add(provider);

		public bool TryResolve(Type type, [NotNullWhen(true)] out IImporter? importer)
		{
			if (_importers.TryGetValue(type, out IImporter? importer1))
			{
				importer = importer1;
				return true;
			}

			foreach (IImporterProvider provider in _providers)
			{
				importer = provider.GetProvider(type);
				if (importer is not null)
				{
					return true;
				}
			}

			importer = null;
			return false;
		}
	}
}