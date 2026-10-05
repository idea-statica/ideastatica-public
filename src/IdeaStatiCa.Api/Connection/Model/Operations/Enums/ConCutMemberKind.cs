namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Kind of member referenced as the target of a Cut operation (the entity that gets cut).
	/// Maps to the <c>MemberTargetBase</c> hierarchy.
	/// </summary>
	public enum ConCutMemberKind
	{
		/// <summary>A regular connection member (beam) referenced by its integer id.</summary>
		Member,

		/// <summary>A stiffening member produced by a stiffening-member operation.</summary>
		StiffeningMember,

		/// <summary>A member produced by a negative-volume-member operation.</summary>
		NegativeVolumeMember,

		/// <summary>A stub produced by a stub operation.</summary>
		StubMember,
	}
}
