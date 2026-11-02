#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// A single item referenced as the source / target of a Weld / Contact operation or as a
	/// connected plate of a fastener grid. Supports four reference modes — pick the one that
	/// fits the entity you want to point at:
	/// <list type="bullet">
	/// <item><b>Top-level plate</b>: set <see cref="PlateId"/> only. Matches LibData's flat plate list.</item>
	/// <item><b>Plate produced by an operation</b> (e.g. STIFF1's plate <c>a</c>, BP1's base plate):
	/// set <see cref="OperationId"/> and optionally <see cref="PlateSubIndex"/>.</item>
	/// <item><b>Whole member</b> (e.g. column <c>C</c>): set <see cref="MemberId"/> alone, leave
	/// <see cref="PlateSubIndex"/> unset. Maps to a <c>MemberTarget</c>.</item>
	/// <item><b>A specific plate of a member</b> (e.g. <c>C | Web 1</c>, <c>C | Top flange 1</c>):
	/// set <see cref="MemberId"/> together with <see cref="PlateSubIndex"/>. Maps to a
	/// <c>MemberPlateTarget</c> — the libdata composite identifier is built by the converter.</item>
	/// </list>
	/// Member-based modes are used by Weld / Contact operations; fastener grids use only the
	/// plate-based modes.
	/// </summary>
	public class ConConnectedItem
	{
		/// <summary>Plate index in the connection (equivalent to LibData plate ID) for top-level plates.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? PlateId { get; set; }

		/// <summary>
		/// Operation that produces the plate being referenced (e.g. a Stiffener or BasePlate operation).
		/// Use together with <see cref="PlateSubIndex"/> to pick a specific plate of that operation.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? OperationId { get; set; }

		/// <summary>
		/// Member to reference. Set alone to point at the whole member (e.g. column <c>C</c>),
		/// or together with <see cref="PlateSubIndex"/> to point at one of the member's plates
		/// (Web / Top flange / Bottom flange).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MemberId { get; set; }

		/// <summary>
		/// Sub-index of the plate inside the parent entity (defaults to 1):
		/// <list type="bullet">
		/// <item>with <see cref="OperationId"/>: the n-th plate produced by that operation.</item>
		/// <item>with <see cref="MemberId"/>: 1 = Web, 2 = Top flange, 3 = Bottom flange (for I sections).</item>
		/// </list>
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? PlateSubIndex { get; set; }
	}
}
