namespace ConnectionIomGenerator.Model
{
	/// <summary>
	/// Represents a structural member (beam or column) in a connection model.
	/// This record defines the geometric properties and orientation of a member connecting to a joint.
	/// </summary>
	public record Member
	{
		/// <summary>
		/// Gets or sets the unique identifier of the member.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the member.
		/// This is a required property used for identification and display purposes.
		/// </summary>
		public required string Name { get; set; }

		/// <summary>
		/// Gets or sets the cross-section name of the member.
		/// This is a required property that references a cross-section profile (e.g., "HEA200", "IPE300").
		/// The cross-section must be available in the IDEA StatiCa cross-section library.
		/// </summary>
		public required string CrossSection { get; set; }

		/// <summary>
		/// Gets or sets the horizontal direction angle of the member in degrees.
		/// This angle defines the rotation around the vertical (Z) axis in the horizontal plane.
		/// Value of 0° represents the positive X-axis direction, 90° represents the positive Y-axis direction.
		/// </summary>
		public float Direction { get; set; }

		/// <summary>
		/// Gets or sets the pitch (vertical inclination) angle of the member in degrees.
		/// This angle defines the rotation around the Y-axis, controlling the vertical tilt of the member.
		/// Positive values tilt the member upward, negative values tilt it downward.
		/// Value of 0° represents a horizontal member, 90° represents a vertical member pointing up.
		/// </summary>
		public float Pitch { get; set; }

		/// <summary>
		/// Gets or sets the rotation angle of the member's cross-section around its longitudinal axis in degrees.
		/// This angle defines the rotation around the local X-axis (along the member's length).
		/// Used to orient asymmetric cross-sections or to control the orientation of the cross-section's axes.
		/// Value of 0° represents the default orientation of the cross-section.
		/// </summary>
		public float Rotation { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the member is continuous through the connection.
		/// <para>
		/// When <c>true</c>, the member passes through the connection node and continues on the opposite side,
		/// creating a continuous structural element (e.g., a continuous beam or column).
		/// </para>
		/// <para>
		/// When <c>false</c>, the member ends at the connection node (ended member),
		/// typically used for secondary members connecting to a main member.
		/// </para>
		/// </summary>
		public bool IsContinuous { get; set; }
	}
}
