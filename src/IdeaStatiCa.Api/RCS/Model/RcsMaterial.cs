namespace IdeaStatiCa.Api.RCS.Model
{
	/// <summary>
	/// Reference to a material in the project.
	/// Specify either Id (preferred) or Name. Id takes precedence if both provided.
	/// </summary>
	public class RcsMaterial
	{
		/// <summary>
		/// Material ID (preferred - obtained from GET /materials endpoints)
		/// </summary>
		public int? Id { get; set; }

		/// <summary>
		/// Material name (alternative to ID, case-insensitive lookup)
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Type of material
		/// </summary>
		public RcsMaterialType MaterialType { get; set; }
	}
}