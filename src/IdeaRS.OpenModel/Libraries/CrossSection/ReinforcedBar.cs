using IdeaRS.OpenModel.Geometry2D;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Reinforced bar
	/// </summary>
	[OpenModelClass("CI.Services.Concrete.ReinforcedSection.ReinfBar,CI.ReinforcedSection")]
	public class ReinforcedBar : OpenObject
	{
		/// <summary>
		/// Position of bar
		/// </summary>
		[OpenModelProperty("CentrePoint")]
		public Point2D Point { get; set; }

		/// <summary>
		/// Diameter
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// Material
		/// </summary>
		public ReferenceElement Material { get; set; }
	}
}