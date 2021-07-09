using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.ImportedObjects;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.BimItems
{
	internal class Connection : IBimItem
	{
		public BIMItemType Type => BIMItemType.Node;

		public IIdeaObject ReferencedObject { get; }

		public Connection(ConnectionPoint connection)
		{
			ReferencedObject = connection;
		}
	}
}