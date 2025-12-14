using IdeaRS.OpenModel.Geometry2D;

namespace IdeaStatiCa.Api.RCS.Model
{
	/// <summary>
	/// Component of a cross-section (concrete region)
	/// </summary>
	public class RcsCssComponentData
	{
		/// <summary>
		/// Reference to the concrete material (must exist in project)
		/// </summary>
		public RcsMaterial Material { get; set; }

		/// <summary>
		/// Construction phase
		/// </summary>
		public int Phase { get; set; }

		/// <summary>
		/// Geometry of the component
		/// </summary>
		public Region2D Geometry { get; set; }
	}
}