#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Cut operation. Trims a member at its intersection with another entity. Maps to the
	/// new-model <c>CutOperation</c> record.
	/// <para>
	/// The member being cut (<see cref="Member"/>) can be any member-like entity — a regular
	/// connection member, a stiffening member, a negative-volume member, or a stub. The cutting
	/// object (<see cref="CutBy"/>) supports the same set of member-like entities plus plates and
	/// work planes (matching the desktop UI's three icons), with an additional plate-of-operation
	/// variant for plates produced by another operation.
	/// </para>
	/// </summary>
	public class ConCutOperation : ConOperation
	{
		public ConCutOperation() : base()
		{
			Active = true;
		}

		[JsonConstructor]
		public ConCutOperation(int id) : base(id)
		{
			Active = true;
		}

		/// <summary>
		/// The member being cut. Required.
		/// </summary>
		public ConCutMemberTarget Member { get; set; } = new ConCutMemberTarget();

		/// <summary>
		/// The entity that cuts the member (member, plate, work plane, etc.). Required.
		/// </summary>
		public ConCutByTarget CutBy { get; set; } = new ConCutByTarget();

		/// <summary>
		/// How the cutting object trims the cut member.
		/// </summary>
		public ConCuttingMethod CuttingMethod { get; set; }

		/// <summary>
		/// If true the member is extended to the cutting object before being cut.
		/// </summary>
		public bool ExtendMember { get; set; }

		/// <summary>
		/// Which side of the cutting object is kept when more than one cut plane is possible.
		/// </summary>
		public ConCuttingPlane CuttingPlane { get; set; }

		/// <summary>
		/// Orientation of the cut relative to the cut member's axis.
		/// </summary>
		public ConCuttingDirection Direction { get; set; }

		/// <summary>
		/// Offset of the cut along the member axis, in metres.
		/// </summary>
		public double Offset { get; set; }

		/// <summary>
		/// Optional weld for the cut on the member's webs.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConWeldData? WebsWeld { get; set; }

		/// <summary>
		/// Optional weld for the cut on the member's flanges.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConWeldData? FlangesWeld { get; set; }
	}
}
