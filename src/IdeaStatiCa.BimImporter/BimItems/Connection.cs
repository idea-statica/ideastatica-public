using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.ImportedObjects;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.BimItems
{
	public class Connection : IBimItem
	{
		public BIMItemType Type => BIMItemType.Node;

		public IIdeaObject ReferencedObject { get; }

		public static Connection FromNodeAndMembers(IIdeaNode node, IEnumerable<IIdeaMember1D> members)
		{
			return new Connection(new ConnectionPoint(node, members));
		}

		internal Connection(ConnectionPoint connection)
		{
			ReferencedObject = connection;
		}
	}
}