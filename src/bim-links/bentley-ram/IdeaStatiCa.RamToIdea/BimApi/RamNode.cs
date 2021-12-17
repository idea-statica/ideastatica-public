using IdeaStatiCa.BimApi;
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

		public IIdeaPersistenceToken Token { get; }

		public RamNode(INode node)
		{
			Id = $"node-{node.lUniqueID}";
			Name = node.lLabel.ToString();

			SCoordinate coord = node.sLocation;
			Vector = new IdeaVector3D(
				Inches2Meters(coord.dXLoc),
				Inches2Meters(coord.dYLoc),
				Inches2Meters(coord.dZLoc));
		}

		private static double Inches2Meters(double val)
		{
			return val * 0.0254;
		}
	}
}