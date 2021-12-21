using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Utilities;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	/// <summary>
	/// Implementation of <see cref="IIdeaNode"/> for RSTAB.
	/// </summary>
	internal class RamNode : IIdeaNode
	{
		public IdeaVector3D Vector { get; }

		public string Id { get; }

		public string Name { get; }

		public IIdeaPersistenceToken Token => null;

		public RamNode(INode node)
		{
			Id = $"node-{node.lUniqueID}";
			Name = node.lLabel.ToString();

			SCoordinate coord = node.sLocation;
			Vector = new IdeaVector3D(
				coord.dXLoc.InchesToMeters(),
				coord.dYLoc.InchesToMeters(),
				coord.dZLoc.InchesToMeters());
		}
	}
}