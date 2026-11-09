using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using System.Collections.Generic;
using System.Linq;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// Imports a connection: its node and the objects it is made of.
	/// </summary>
	internal class ConnectionImporter : ImporterConnectionIdentifier<IIdeaConnectionPoint>
	{
		/// <summary>
		/// For a new selection the identifier holds only the node, and BimApiLink adds the parts afterwards. When
		/// Checkbot synchronizes a saved connection, the identifier lists its parts as well.
		/// </summary>
		public override IIdeaConnectionPoint Create(ConnectionIdentifier<IIdeaConnectionPoint> id)
		{
			return new IdeaConnectionPoint(id.GetStringId())
			{
				Node = Get(id.Node),
				ConnectedMembers = GetAll(id.ConnectedMembers),
				Plates = GetAll(id.Plates),
				BoltGrids = GetAll(id.BoltGrids),
				Cuts = GetAll(id.Cuts),
			};
		}

		/// <summary>
		/// Imports every object in the list. A missing list gives an empty one.
		/// </summary>
		private List<T> GetAll<T>(IEnumerable<Identifier<T>> identifiers)
			where T : IIdeaObject
		{
			return identifiers?.Select(identifier => Get(identifier)).ToList() ?? new List<T>();
		}
	}
}
