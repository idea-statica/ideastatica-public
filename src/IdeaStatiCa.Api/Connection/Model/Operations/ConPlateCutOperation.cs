#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Plate-cut operation. Trims a plate at its intersection with another entity. Maps to the
	/// new-model <c>PlateCutOperation</c> record.
	/// <para>
	/// Both <see cref="ModifiedObject"/> (the plate being cut) and <see cref="CutBy"/> (the cutting
	/// object) reuse <see cref="ConCutByTarget"/> / <see cref="ConCutByKind"/> — the desktop UI lets
	/// either side be any of the plate-like or cutter-like entities (Plate, MemberPlate,
	/// PlateOfOperation, StiffeningMemberPlate, NegativeVolumeMemberPlate, StubMemberPlate, Member,
	/// StiffeningMember, NegativeVolumeMember, StubMember, WorkPlane).
	/// </para>
	/// </summary>
	public class ConPlateCutOperation : ConOperation
	{
		public ConPlateCutOperation() : base()
		{
			Active = true;
		}

		[JsonConstructor]
		public ConPlateCutOperation(int id) : base(id)
		{
			Active = true;
		}

		/// <summary>
		/// The plate (or plate-like entity) being cut. Required.
		/// </summary>
		public ConCutByTarget ModifiedObject { get; set; } = new ConCutByTarget();

		/// <summary>
		/// The entity that cuts the plate (plate, member, work plane, etc.). Required.
		/// </summary>
		public ConCutByTarget CutBy { get; set; } = new ConCutByTarget();

		/// <summary>
		/// How the cutting object trims the plate.
		/// </summary>
		public ConCuttingMethod CuttingMethod { get; set; }

		/// <summary>
		/// Which side of the plate is kept after the cut.
		/// </summary>
		public ConPlateSide RemainingPart { get; set; }

		/// <summary>
		/// Offset of the cut along the plate, in metres.
		/// </summary>
		public double Offset { get; set; }

		/// <summary>
		/// Optional weld at the cut edge.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConWeldData? Weld { get; set; }
	}
}
