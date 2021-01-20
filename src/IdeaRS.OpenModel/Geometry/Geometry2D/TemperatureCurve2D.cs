namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Reperesents a thermal curve.
	/// </summary>
	[OpenModelClass("CI.Geometry2D.TemperatureCurve2D,CI.Geometry2D")]
	public class TemperatureCurve2D : Polygon2D
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public TemperatureCurve2D()
			: base()
		{
		}

		/// <summary>
		/// The temperature [K]
		/// </summary>
		public double Temperature { get; set; }
	}
}