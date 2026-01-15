using IdeaRS.OpenModel.Geometry2D;

namespace IdeaStatiCa.Api.RCS.Model
{
	/// <summary>
	/// Tendon bar input
	/// </summary>
	public class RcsTendonBarData
	{
		/// <summary>
		/// Position of the tendon
		/// </summary>
		public Point2D Point { get; set; }

		/// <summary>
		/// Reference to the prestress steel material (must exist in project)
		/// </summary>
		public string MaterialName { get; set; }

		/// <summary>
		/// Number of strands in tendon
		/// </summary>
		public int NumStrandsInTendon { get; set; }

		/// <summary>
		/// Prestressing order
		/// </summary>
		public int PrestressingOrder { get; set; }

		/// <summary>
		/// Associated duct (optional, for post-tensioned)
		/// </summary>
		public RcsTendonDuctData TendonDuct { get; set; }
	}
}