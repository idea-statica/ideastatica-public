#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// The member being cut by a Cut operation. Discriminated by <see cref="Kind"/>:
	/// <list type="bullet">
	/// <item><see cref="ConCutMemberKind.Member"/> — set <see cref="MemberId"/>.</item>
	/// <item><see cref="ConCutMemberKind.StiffeningMember"/> — set <see cref="OperationId"/>;
	/// <see cref="RemainingPart"/> is optional (default <see cref="ConRemainingPart.Begin"/>).</item>
	/// <item><see cref="ConCutMemberKind.NegativeVolumeMember"/> — set <see cref="OperationId"/>.</item>
	/// <item><see cref="ConCutMemberKind.StubMember"/> — set <see cref="OperationId"/>.</item>
	/// </list>
	/// </summary>
	public class ConCutMemberTarget
	{
		public ConCutMemberKind Kind { get; set; }

		/// <summary>Required when <see cref="Kind"/> is <see cref="ConCutMemberKind.Member"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MemberId { get; set; }

		/// <summary>
		/// Required when <see cref="Kind"/> is one of
		/// <see cref="ConCutMemberKind.StiffeningMember"/> /
		/// <see cref="ConCutMemberKind.NegativeVolumeMember"/> /
		/// <see cref="ConCutMemberKind.StubMember"/>. Refers to the id of the operation that
		/// produces the member.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? OperationId { get; set; }

		/// <summary>
		/// Optional. Only meaningful when <see cref="Kind"/> is <see cref="ConCutMemberKind.StiffeningMember"/>.
		/// Selects which end of the stiffening member is kept after the cut.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConRemainingPart? RemainingPart { get; set; }
	}
}
