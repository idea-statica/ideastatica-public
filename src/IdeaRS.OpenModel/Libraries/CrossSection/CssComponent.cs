using IdeaRS.OpenModel.Geometry2D;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// One component of cross-section
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.CrossSection.CssComponent,CI.CrossSection")]
	public class CssComponent : OpenObject
	{
		/// <summary>
		/// Component Id
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Material
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// Geometry
		/// </summary>
		public Region2D Geometry { get; set; }

		/// <summary>
		/// Phase
		/// </summary>
		public int Phase { get; set; }
	}
}