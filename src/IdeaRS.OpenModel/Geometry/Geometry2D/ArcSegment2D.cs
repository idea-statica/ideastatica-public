namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Represents an arc segment in two-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry2D.CircularArcSegment2D,CI.Geometry2D")]
	public class ArcSegment2D : Segment2D
	{
		/// <summary>
		/// Gets or sets the point of circular arc somewhere between start and end point.
		/// </summary>
		public Point2D Point { get; set; }
	}
}