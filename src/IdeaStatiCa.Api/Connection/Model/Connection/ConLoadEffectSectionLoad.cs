namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// Internal forces at a member cross-section within a connection.
	/// <para>
	/// <strong>Sign convention:</strong> The sign convention depends on the <see cref="ConLoadEffectMemberLoad.Position"/> value.
	/// At the <c>End</c> position, the standard structural convention applies (positive <see cref="N"/> = tension).
	/// At the <c>Begin</c> position, the values of <see cref="N"/> and <see cref="My"/> use an inverted sign convention
	/// — they must be negated to obtain the standard structural convention (positive N = tension).
	/// The shear forces (<see cref="Vy"/>, <see cref="Vz"/>) use a consistent sign convention at both positions.
	/// </para>
	/// </summary>
	public class ConLoadEffectSectionLoad
	{
		/// <summary>
		/// Axial force [kN]. Positive value = tension at the <c>End</c> position.
		/// At the <c>Begin</c> position the sign is inverted — negate to get the standard structural convention.
		/// </summary>
		public double N { get; set; }

		/// <summary>
		/// Shear force in the local Y direction [kN]. Sign convention is consistent at both Begin and End positions.
		/// </summary>
		public double Vy { get; set; }

		/// <summary>
		/// Shear force in the local Z direction [kN]. Sign convention is consistent at both Begin and End positions.
		/// </summary>
		public double Vz { get; set; }

		/// <summary>
		/// Torsional moment [kNm]. Not used when <see cref="ConLoadEffect.IsPercentage"/> is true.
		/// </summary>
		public double Mx { get; set; }

		/// <summary>
		/// Bending moment about the local Y axis [kNm].
		/// At the <c>Begin</c> position the sign is inverted — negate to get the standard structural convention.
		/// </summary>
		public double My { get; set; }

		/// <summary>
		/// Bending moment about the local Z axis [kNm].
		/// </summary>
		public double Mz { get; set; }
	}
}