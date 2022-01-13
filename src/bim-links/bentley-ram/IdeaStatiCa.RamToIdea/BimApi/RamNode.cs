using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Utilities;
using MathNet.Spatial.Euclidean;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	/// <summary>
	/// Implementation of <see cref="IIdeaNode"/> for RSTAB.
	/// </summary>
	internal class RamNode : IIdeaNode
	{
		public IdeaVector3D Vector { get; }

		public Vector3D Position { get; }

		public string Id { get; }

		public string Name { get; }

		public IIdeaPersistenceToken Token => null;

		public INode FrameNode { get; }

		private RamNode(double x, double y, double z)
		{
			Vector = new IdeaVector3D(x, y, z);
			Position = new Vector3D(x, y, z);
		}

		public RamNode(INode node)
			: this(node.sLocation.dXLoc.InchesToMeters(), node.sLocation.dYLoc.InchesToMeters(), node.sLocation.dZLoc.InchesToMeters())
		{
			Id = $"node-{node.lUniqueID}";
			Name = node.lLabel.ToString();
			FrameNode = node;
		}

		public RamNode(Vector3D position)
			: this(position.X, position.Y, position.Z)
		{
			Id = $"node-generated-{position.X},{position.Y},{position.Z}";
			Name = "";
		}
	}
}