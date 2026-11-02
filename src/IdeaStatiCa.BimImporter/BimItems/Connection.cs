using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.ImportedObjects;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.BimItems
{
	/// <summary>
	/// Connection bim item. 
	/// </summary>
	/// <seealso cref="IdeaStatiCa.BimImporter.BimItems.IBimItem" />
	public class Connection : IBimItem
	{
		/// <summary>
		/// Gets the type of the bim item.
		/// </summary>
		public BIMItemType Type => BIMItemType.Node;

		/// <summary>
		/// Gets the referenced bim object.
		/// </summary>
		public IIdeaObject ReferencedObject { get; }

		/// <summary>
		/// Creates a connection from a node and connecting members.
		/// </summary>
		/// <param name="node">The node of the conenection.</param>
		/// <param name="members">Connecting members.</param>
		/// <returns>Connection instance.</returns>
		public static Connection FromNodeAndMembers(IIdeaNode node, IEnumerable<IIdeaMember1D> members)
		{
			return new Connection(new ConnectionPoint(node, members));
		}

		/// <summary>
		/// Creates a connection from a node and connecting members.
		/// </summary>
		/// <param name="connection">The conenection point.</param>
		/// <returns>Connection instance.</returns>
		public static Connection FromConnectionPoint(IIdeaConnectionPoint connection)
		{
			return new Connection(connection);
		}

		internal Connection(IIdeaConnectionPoint connection)
		{
			ReferencedObject = connection;
		}
	}
}