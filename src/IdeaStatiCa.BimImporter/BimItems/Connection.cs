using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.ImportedObjects;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.BimItems
{
	internal class Connection : IBimItem
	{
		public BIMItemType Type => BIMItemType.Node;

		public IIdeaObject ReferencedObject { get; }

		public static Connection FromNodeAndMembers(IIdeaNode node, ISet<IIdeaMember1D> members)
		{
			return new Connection(new ConnectionPoint(node, members));
		}

		public Connection(ConnectionPoint connection)
		{
			ReferencedObject = connection;
		}
	}
}