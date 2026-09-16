#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Contact operation. Plate-to-plate contact (no weld material) — geometry only.
	/// Maps to the new-model <c>ContactOperation</c>.
	///
	/// <para>Not to be confused with <see cref="ConContactGridOperation"/>, which is a fastener-grid
	/// flavour for base plates pressed against a foundation block. This DTO is for plate-to-plate
	/// contact within a steel connection (the "contact" branch of the legacy WeldOrContact command).</para>
	/// </summary>
	public class ConContactOperation : ConOperation
	{
		public ConContactOperation() : base()
		{
			Active = true;
		}

		[JsonConstructor]
		public ConContactOperation(int id) : base(id)
		{
			Active = true;
		}

		/// <summary>How the two operands are joined.</summary>
		public ConWeldPlacement Placement { get; set; }

		/// <summary>First operand — an edge of this member/plate is the contact source.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConConnectedItem? FirstMemberOrPlate { get; set; }

		/// <summary>Edge indices of <see cref="FirstMemberOrPlate"/> that participate in the contact.</summary>
		public List<int> FirstPlateEdgeIndex { get; set; } = new List<int>();

		/// <summary>Second operand — paired member/plate. Despite the UI's "Plate" label, this slot
		/// also accepts members (the whole member or a member's plate) — use
		/// <see cref="ConConnectedItem.MemberId"/> for that.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConConnectedItem? SecondMemberOrPlate { get; set; }

		/// <summary>Edge index of <see cref="SecondMemberOrPlate"/> when <see cref="Placement"/> is
		/// <see cref="ConWeldPlacement.EdgeToEdge"/>. Ignored for surface placement.</summary>
		public int SecondPlateEdgeIndex { get; set; }
	}
}
