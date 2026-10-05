namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Kind of entity referenced as the cutting object of a Cut operation.
	/// Maps to the <c>TargetBase</c> hierarchy. Covers every variant the desktop UI offers
	/// (Member, Plate, Work plane) plus the operation-produced member / plate variants
	/// supported by the new-model <c>CutOperation</c> record.
	/// </summary>
	public enum ConCutByKind
	{
		/// <summary>A regular connection member (beam) referenced by its integer id.</summary>
		Member,

		/// <summary>A plate referenced directly by its integer id.</summary>
		Plate,

		/// <summary>A plate produced by another operation, identified by operation id + plate sub-index.</summary>
		PlateOfOperation,

		/// <summary>A specific plate of a regular member, identified by member id + plate sub-index.</summary>
		MemberPlate,

		/// <summary>A stiffening member produced by a stiffening-member operation.</summary>
		StiffeningMember,

		/// <summary>A specific plate of a stiffening member, identified by operation id + plate sub-index.</summary>
		StiffeningMemberPlate,

		/// <summary>A member produced by a negative-volume-member operation.</summary>
		NegativeVolumeMember,

		/// <summary>A specific plate of a negative-volume member, identified by operation id + plate sub-index.</summary>
		NegativeVolumeMemberPlate,

		/// <summary>A stub produced by a stub operation.</summary>
		StubMember,

		/// <summary>A specific plate of a stub, identified by operation id + plate sub-index.</summary>
		StubMemberPlate,

		/// <summary>A work plane produced by a Work-Plane operation.</summary>
		WorkPlane,
	}
}
