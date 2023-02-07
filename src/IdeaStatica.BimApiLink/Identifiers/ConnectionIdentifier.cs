using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System.Globalization;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public class ConnectionIdentifier<T> : ImmutableIdentifier<T>
		where T : IIdeaObject
	{
		public ICollection<ImmutableIdentifier<IIdeaPlate>> Plates { get; set; }

		public ICollection<ImmutableIdentifier<IIdeaConnectedMember>> ConnectedMembers { get; set; }

		public ICollection<ImmutableIdentifier<IIdeaFoldedPlate>> FoldedPlates { get; set; }

		public ICollection<ImmutableIdentifier<IIdeaAnchorGrid>> AnchorGrids { get; set; }

		public ICollection<ImmutableIdentifier<IIdeaBoltGrid>> BoltGrids { get; set; }

		public ICollection<ImmutableIdentifier<IIdeaWeld>> Welds { get; set; }

		public ICollection<ImmutableIdentifier<IIdeaCut>> Cuts { get; set; }

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