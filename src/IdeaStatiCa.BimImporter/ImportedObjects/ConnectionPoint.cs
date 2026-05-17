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

		// Auto-created connection points (from Connection.FromNodeAndMembers) inherit their name from the
		// underlying node. Plugin link implementations often set Node.Name to a coordinate-encoded id
		// (e.g. AdvanceSteel sets Name = id.ToString() which is the "X;Y;Z" GetPointId string), and that
		// would propagate into IOM ConnectionPoint.Name, bypassing BimImporter.ImportContext fallback
		// `cp.Name = $"C {cp.Id}"`. Return null so the fallback applies and synthetic connection points
		// get clean user-facing labels (`C 452`, etc.) in Checkbot. Bug from #34xxx.
		public string Name => null;

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