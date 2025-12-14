using IdeaRS.OpenModel.Geometry2D;

namespace IdeaStatiCa.Api.RCS.Model
{
	/// <summary>
	/// Reinforcement bar input
	/// </summary>
	public class RcsReinforcedBarData
	{
		/// <summary>
		/// Bar diameter in meters
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// Position of the bar center
		/// </summary>
		public Point2D Point { get; set; }

		/// <summary>
		/// Reference to the reinforcement material (must exist in project)
		/// </summary>
		public RcsMaterial Material { get; set; }
	}
}