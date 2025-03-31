using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Scoping
{
	internal class BimApiImporterCacheAdapter : IBimApiImporter
	{
		private readonly Dictionary<IIdentifier, IIdeaObject> _importedObjects = new Dictionary<IIdentifier, IIdeaObject>();

		private readonly IBimApiImporter _bimApiImporter;

		public BimApiImporterCacheAdapter(IBimApiImporter bimApiImporter)
		{
			_bimApiImporter = bimApiImporter;
		}

		public T Check<T>(Identifier<T> identifier) where T : IIdeaObject
		{
			T newObj = _bimApiImporter.Check(identifier);

			return newObj;
		}

		public IIdeaObject Check(IIdentifier identifier)
		{
			IIdeaObject newObj = _bimApiImporter.Check(identifier);
			return newObj;
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

			//due to bulk selection don't cache welds bolts anchors - in this cases item assigned to more connections cause wrong definition of object 
			if (!(newObj is IIdeaWeld)
				&& !(newObj is IIdeaAnchorGrid)
				&& !(newObj is IIdeaBoltGrid)
				)
			{
				_importedObjects[identifier] = newObj;
			}

			return newObj;
		}
	}
}