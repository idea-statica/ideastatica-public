using FeaExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkFeaExample.Importers
{
	/// <summary>
	/// Imports a node by its FEA id.
	/// </summary>
	internal class NodeImporter : IntIdentifierImporter<IIdeaNode>
	{
		private readonly IFeaGeometryApi _geometry;

		public NodeImporter(IFeaGeometryApi geometry)
		{
			_geometry = geometry;
		}

		/// <summary>
		/// Creates the node at the coordinates of the FEA node.
		/// </summary>
		public override IIdeaNode Create(int id)
		{
			FeaNode node = _geometry.GetNode(id);

			return new IdeaNode(id)
			{
				Vector = new IdeaVector3D(node.X, node.Y, node.Z),
			};
		}
	}
}
