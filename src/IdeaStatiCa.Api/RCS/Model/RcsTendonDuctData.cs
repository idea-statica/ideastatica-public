using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry2D;

namespace IdeaStatiCa.Api.RCS.Model
{
	/// <summary>
	/// Tendon duct input
	/// </summary>
	public class RcsTendonDuctData
	{
		/// <summary>
		/// Position of the duct center
		/// </summary>
		public Point2D Point { get; set; }

		/// <summary>
		/// Duct diameter in meters
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// Is debonding tube
		/// </summary>
		public bool IsDebondingTube { get; set; }

		/// <summary>
		/// Duct material type
		/// </summary>
		public MaterialDuct MaterialDuct { get; set; }
	}
}