using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaConnectionPoint : AbstractIdeaObject<IIdeaConnectionPoint>, IIdeaConnectionPoint
	{
		protected IdeaConnectionPoint(Identifier<IIdeaConnectionPoint> identifer)
			: base(identifer)
		{
		}

		public IdeaConnectionPoint(int id)
			: this(new IntIdentifier<IIdeaConnectionPoint>(id))
		{ }

		public IdeaConnectionPoint(string id)
			: this(new StringIdentifier<IIdeaConnectionPoint>(id))
		{ }

		public IdeaConnectionPoint(double X, double Y, double Z)
			: this(new ConnectionIdentifier<IIdeaConnectionPoint>(X, Y, Z))
		{ }
		public virtual IIdeaNode Node { get; set; }

		public IEnumerable<IIdeaConnectedMember> ConnectedMembers { get; set; }

		public IEnumerable<IIdeaPlate> Plates { get; set; }

		public IEnumerable<IIdeaFoldedPlate> FoldedPlates { get; set; }

		public IEnumerable<IIdeaAnchorGrid> AnchorGrids { get; set; }

		public IEnumerable<IIdeaBoltGrid> BoltGrids { get; set; }

		public IEnumerable<IIdeaWeld> Welds { get; set; }

		public IEnumerable<IIdeaCut> Cuts { get; set; }
	}
}
