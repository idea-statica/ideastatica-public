#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// The cutting object of a Cut operation. Discriminated by <see cref="Kind"/>:
	/// <list type="bullet">
	/// <item><see cref="ConCutByKind.Member"/> — set <see cref="MemberId"/>.</item>
	/// <item><see cref="ConCutByKind.Plate"/> — set <see cref="PlateId"/>.</item>
	/// <item><see cref="ConCutByKind.PlateOfOperation"/> — set <see cref="OperationId"/> + <see cref="PlateSubIndex"/>.</item>
	/// <item><see cref="ConCutByKind.MemberPlate"/> — set <see cref="MemberId"/> + <see cref="PlateSubIndex"/>.</item>
	/// <item><see cref="ConCutByKind.StiffeningMember"/> — set <see cref="OperationId"/>; optional <see cref="RemainingPart"/>.</item>
	/// <item><see cref="ConCutByKind.StiffeningMemberPlate"/> — set <see cref="OperationId"/> + <see cref="PlateSubIndex"/>.</item>
	/// <item><see cref="ConCutByKind.NegativeVolumeMember"/> — set <see cref="OperationId"/>.</item>
	/// <item><see cref="ConCutByKind.NegativeVolumeMemberPlate"/> — set <see cref="OperationId"/> + <see cref="PlateSubIndex"/>.</item>
	/// <item><see cref="ConCutByKind.StubMember"/> — set <see cref="OperationId"/>.</item>
	/// <item><see cref="ConCutByKind.StubMemberPlate"/> — set <see cref="OperationId"/> + <see cref="PlateSubIndex"/>.</item>
	/// <item><see cref="ConCutByKind.WorkPlane"/> — set <see cref="OperationId"/>.</item>
	/// </list>
	/// </summary>
	public class ConCutByTarget
	{
		public ConCutByKind Kind { get; set; }

		/// <summary>Required when <see cref="Kind"/> is <see cref="ConCutByKind.Member"/> or <see cref="ConCutByKind.MemberPlate"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MemberId { get; set; }

		/// <summary>Required when <see cref="Kind"/> is <see cref="ConCutByKind.Plate"/>. The plate's integer id.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? PlateId { get; set; }

		/// <summary>
		/// Required when <see cref="Kind"/> references an operation:
		/// <see cref="ConCutByKind.PlateOfOperation"/>,
		/// <see cref="ConCutByKind.StiffeningMember"/>, <see cref="ConCutByKind.StiffeningMemberPlate"/>,
		/// <see cref="ConCutByKind.NegativeVolumeMember"/>, <see cref="ConCutByKind.NegativeVolumeMemberPlate"/>,
		/// <see cref="ConCutByKind.StubMember"/>, <see cref="ConCutByKind.StubMemberPlate"/>,
		/// <see cref="ConCutByKind.WorkPlane"/>.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? OperationId { get; set; }

		/// <summary>
		/// Required when <see cref="Kind"/> is one of the *Plate variants
		/// (<see cref="ConCutByKind.PlateOfOperation"/>, <see cref="ConCutByKind.MemberPlate"/>,
		/// <see cref="ConCutByKind.StiffeningMemberPlate"/>, <see cref="ConCutByKind.NegativeVolumeMemberPlate"/>,
		/// <see cref="ConCutByKind.StubMemberPlate"/>). Identifies the specific plate of the referenced entity.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? PlateSubIndex { get; set; }

		/// <summary>
		/// Optional. Only meaningful when <see cref="Kind"/> is <see cref="ConCutByKind.StiffeningMember"/>.
		/// Selects which end of the stiffening member is kept after the cut.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConRemainingPart? RemainingPart { get; set; }
	}
}
