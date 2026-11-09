using BimApiLinkCadExample.Conversion;
using CadExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// A node id holds the node's coordinates (see <see cref="NodeId"/>), so this importer needs nothing from the CAD
	/// application.
	/// </summary>
	internal class NodeImporter : StringIdentifierImporter<IIdeaNode>
	{
		/// <summary>
		/// Creates the node at the coordinates its id holds.
		/// </summary>
		public override IIdeaNode Create(string id)
		{
			CadPoint3D point = NodeId.ToPoint(id);

			return new IdeaNode(id)
			{
				Name = id,
				Vector = new IdeaVector3D(point.X, point.Y, point.Z),
			};
		}
	}
}
