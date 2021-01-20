using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents an arc segment in 3D space defined by 3 points
	/// Start, End and a Point between start and end point.
	/// </summary>	
	public class RebarSegment3DArc3Pts : RebarSegment3DBase
	{
		/// <summary>
		/// Gets or sets the point of circular arc somewhere between start and end point.
		/// </summary>
		public Point3D ThirdPoint { get; set; }
	}
}