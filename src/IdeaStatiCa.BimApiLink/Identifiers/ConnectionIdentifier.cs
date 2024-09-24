using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System.Globalization;

namespace IdeaStatiCa.BimApiLink.Identifiers
{
	public class ConnectionIdentifier<T> : ImmutableIdentifier<T>
		where T : IIdeaObject
	{
		public IList<ImmutableIdentifier<IIdeaPlate>> Plates { get; set; }

		public IList<ImmutableIdentifier<IIdeaConnectedMember>> ConnectedMembers { get; set; }

		public IList<ImmutableIdentifier<IIdeaFoldedPlate>> FoldedPlates { get; set; }

		public IList<ImmutableIdentifier<IIdeaAnchorGrid>> AnchorGrids { get; set; }

		public IList<ImmutableIdentifier<IIdeaBoltGrid>> BoltGrids { get; set; }

		public IList<ImmutableIdentifier<IIdeaPinGrid>> PinGrids { get; set; }

		public IList<ImmutableIdentifier<IIdeaWeld>> Welds { get; set; }

		public IList<ImmutableIdentifier<IIdeaCut>> Cuts { get; set; }

		public Identifier<IIdeaNode> Node { get; set; }

		private static string GetPointId(double x, double y, double z)
		{
			return $"{x.ToString("G", CultureInfo.InvariantCulture)};{y.ToString("G", CultureInfo.InvariantCulture)};{z.ToString("G", CultureInfo.InvariantCulture)}";
		}

		public ConnectionIdentifier(double X, double Y, double Z)
			: base(typeof(T).FullName + "-" + GetPointId(X, Y, Z))
		{
			Node = new StringIdentifier<IIdeaNode>(GetPointId(X, Y, Z));
		}
	}
}