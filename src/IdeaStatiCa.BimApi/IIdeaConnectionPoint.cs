using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaConnectionPoint : IIdeaObject
	{
		IIdeaNode Node { get; }

		IEnumerable<IIdeaConnectedMember> ConnectedMembers { get; }

		IEnumerable<IIdeaPlate> Plates { get; }

		IEnumerable<IIdeaFoldedPlate> FoldedPlates { get; }

		IEnumerable<IIdeaAnchorGrid> AnchorGrids { get; }

		IEnumerable<IIdeaBoltGrid> BoltGrids { get; }

		IEnumerable<IIdeaWeld> Welds { get; }

		IEnumerable<IIdeaCut> Cuts { get; }
	}
}
