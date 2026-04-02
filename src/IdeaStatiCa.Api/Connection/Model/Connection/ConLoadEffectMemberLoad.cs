namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// Represents the load applied to a specific member within a load effect.
	/// <para>
	/// For <strong>continuous members</strong> (e.g., a chord passing through a truss node),
	/// a single load effect will contain two <see cref="ConLoadEffectMemberLoad"/> entries for the same
	/// <see cref="MemberId"/>: one with <see cref="Position"/> = <c>Begin</c> and one with <c>End</c>.
	/// These represent the internal forces on either side of the connection point.
	/// For <strong>ended members</strong> (e.g., braces), only a single entry is returned.
	/// </para>
	/// </summary>
	public class ConLoadEffectMemberLoad
	{
		/// <summary>
		/// Identifier of the member this load applies to.
		/// </summary>
		public int MemberId { get; set; }

		/// <summary>
		/// Position along the member at which the internal forces are reported.
		/// Continuous members have entries at both <c>Begin</c> and <c>End</c>;
		/// ended members have a single entry.
		/// See <see cref="ConLoadEffectSectionLoad"/> for the sign convention at each position.
		/// </summary>
		public ConLoadEffectPositionEnum Position { get; set; }

		/// <summary>
		/// Internal forces at this member section. See <see cref="ConLoadEffectSectionLoad"/> for sign convention details.
		/// </summary>
		public ConLoadEffectSectionLoad SectionLoad { get; set; }
	}
}