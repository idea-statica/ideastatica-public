using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.ImportedObjects
{
	public class ConnectionPoint : IIdeaConnectionPoint
	{
		public string Id => "$connection-" + Node.Id;

		public string Name => Node.Name;

		public IIdeaNode Node { get; }

		public IEnumerable<IIdeaConnectedMember> ConnectedMembers { get; }

		public IEnumerable<IIdeaPlate> Plates { get; }

		public IEnumerable<IIdeaBoltGrid> BoltGrids { get; }

		public IEnumerable<IIdeaWeld> Welds { get; }

		public ConnectionPoint(IIdeaNode node, IEnumerable<IIdeaMember1D> members)
		{
			Node = node;
			ConnectedMembers = members.Select(x => new ConnectedMember(x)).ToList();
			Plates = new List<IIdeaPlate>();
		}
	}
}