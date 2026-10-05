#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Weld operation. Joins an edge of <see cref="FirstMemberOrPlate"/> to either an edge or the
	/// surface of <see cref="SecondMemberOrPlate"/> (per <see cref="Placement"/>) using the supplied
	/// <see cref="Weld"/> definition. Maps to the new-model <c>WeldOperation</c>.
	///
	/// <para>Old-model libdata stores Weld and Contact as a single command discriminated by a
	/// <c>WeldOrContact</c> enum. The new model splits them into <c>WeldOperation</c> /
	/// <c>ContactOperation</c>; this DTO drives the Weld branch.</para>
	/// </summary>
	public class ConWeldOperation : ConOperation
	{
		public ConWeldOperation() : base()
		{
			Active = true;
		}

		[JsonConstructor]
		public ConWeldOperation(int id) : base(id)
		{
			Active = true;
		}

		/// <summary>How the two operands are joined.</summary>
		public ConWeldPlacement Placement { get; set; }

		/// <summary>First operand — an edge of this member/plate is welded.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConConnectedItem? FirstMemberOrPlate { get; set; }

		/// <summary>Edge indices of <see cref="FirstMemberOrPlate"/> that participate in the weld.</summary>
		public List<int> FirstPlateEdgeIndex { get; set; } = new List<int>();

		/// <summary>Second operand — the welded edge of <see cref="FirstMemberOrPlate"/> connects to
		/// either an edge or the surface of this member/plate (per <see cref="Placement"/>). Despite
		/// the UI's "Plate" label, this slot also accepts members (the whole member or a member's
		/// plate like <c>C | Web 1</c>) — use <see cref="ConConnectedItem.MemberId"/> for that.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConConnectedItem? SecondMemberOrPlate { get; set; }

		/// <summary>Edge index of <see cref="SecondMemberOrPlate"/> when <see cref="Placement"/> is
		/// <see cref="ConWeldPlacement.EdgeToEdge"/>. Ignored for surface placement.</summary>
		public int SecondPlateEdgeIndex { get; set; }

		/// <summary>Weld definition (size, type, material, optional intermittent pattern).</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConWeldData? Weld { get; set; }
	}
}
