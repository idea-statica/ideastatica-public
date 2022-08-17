using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Scoping
{
	internal class BimApiImporterCacheAdapter : IBimApiImporter
	{
		private readonly Dictionary<IIdentifier, IIdeaObject> _importedObjects = new();

		private readonly IBimApiImporter _bimApiImporter;

		public BimApiImporterCacheAdapter(IBimApiImporter bimApiImporter)
		{
			_bimApiImporter = bimApiImporter;
		}

		public T Get<T>(Identifier<T> identifier)
			where T : IIdeaObject
		{
			if (_importedObjects.TryGetValue(identifier, out var obj)
				&& obj is T res)
			{
				return res;
			}

			res = _bimApiImporter.Get(identifier);
			_importedObjects[identifier] = res;

			return res;
		}

		public IIdeaObject Get(IIdentifier identifier)
		{
			if (_importedObjects.TryGetValue(identifier, out var obj))
			{
				return obj;
			}

			obj = _bimApiImporter.Get(identifier);
			_importedObjects[identifier] = obj;

			return obj;
		}
	}
}