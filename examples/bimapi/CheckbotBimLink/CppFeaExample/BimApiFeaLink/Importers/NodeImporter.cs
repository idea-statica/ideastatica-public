using FeaApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiFeaLink.Importers
{
	public class NodeImporter : IntIdentifierImporter<IIdeaNode>
	{
		private readonly IFeaGeometryApi geometry;

		public NodeImporter(IFeaGeometryApi geometry)
		{
			this.geometry = geometry;
		}

		public override IIdeaNode Create(int id)
		{
			var v = GetLocation(id);
			return new IdeaNode(id)
			{
				Vector = v,
			};
		}

		private IdeaVector3D GetLocation(int id)
		{
			IFeaNode feaNode = geometry.GetNode(id);
			return new IdeaVector3D(feaNode.X, feaNode.Y, feaNode.Z);
		}
	}
}