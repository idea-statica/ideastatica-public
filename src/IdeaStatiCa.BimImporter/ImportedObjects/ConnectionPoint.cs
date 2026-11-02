using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.ImportedObjects
{
	/// <summary>
	/// Connection Point
	/// </summary>
	public class ConnectionPoint : IIdeaConnectionPoint
	{
		public string Id => "$connection-" + Node.Id;

		public string Name => Node.Name;

		public IIdeaNode Node { get; }

		public IEnumerable<IIdeaConnectedMember> ConnectedMembers { get; }

		public IEnumerable<IIdeaPlate> Plates { get; }

		public IEnumerable<IIdeaFoldedPlate> FoldedPlates { get; }

		public IEnumerable<IIdeaBoltGrid> BoltGrids { get; }

		public IEnumerable<IIdeaAnchorGrid> AnchorGrids { get; }

		public IEnumerable<IIdeaPinGrid> PinGrids { get; }

		public IEnumerable<IIdeaWeld> Welds { get; }

		public IEnumerable<IIdeaCut> Cuts { get; }


		public ConnectionPoint(IIdeaNode node, IEnumerable<IIdeaMember1D> members)
		{
			Node = node;
			ConnectedMembers = members.Select(x => new ConnectedMember(x)).ToList();
			Plates = new List<IIdeaPlate>();
			FoldedPlates = new List<IIdeaFoldedPlate>();
			BoltGrids = new List<IIdeaBoltGrid>();
			AnchorGrids = new List<IIdeaAnchorGrid>();
			Welds = new List<IIdeaWeld>();
			Cuts = new List<IIdeaCut>();
			PinGrids = new List<IIdeaPinGrid>();
		}
	}
}