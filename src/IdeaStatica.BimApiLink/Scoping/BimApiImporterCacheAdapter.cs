using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.Scoping
{
	internal class BimApiImporterCacheAdapter : IBimApiImporter
	{
		private readonly Dictionary<IIdentifier, IIdeaObject> _importedObjects = new Dictionary<IIdentifier, IIdeaObject>();

		private readonly IBimApiImporter _bimApiImporter;

		public BimApiImporterCacheAdapter(IBimApiImporter bimApiImporter)
		{
			_bimApiImporter = bimApiImporter;
		}

		public T Get<T>(Identifier<T> identifier)
			where T : IIdeaObject
		{
			if (_importedObjects.TryGetValue(identifier, out IIdeaObject obj)
				&& obj is T storedObj)
			{
				return storedObj;
			}

			T newObj = _bimApiImporter.Get(identifier);
			_importedObjects[identifier] = newObj;

			return newObj;
		}

		public IIdeaObject Get(IIdentifier identifier)
		{
			if (_importedObjects.TryGetValue(identifier, out IIdeaObject obj))
			{
				return obj;
			}

			IIdeaObject newObj = _bimApiImporter.Get(identifier);
			_importedObjects[identifier] = newObj;

			return newObj;
		}
	}
}