using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Represents a segment in two-dimensional space.
	/// </summary>
	[XmlInclude(typeof(LineSegment2D))]
	[XmlInclude(typeof(ArcSegment2D))]
	public abstract class Segment2D : OpenObject
	{
		/// <summary>
		/// Gets or sets the endpoint of the segment.
		/// </summary>
		public Point2D EndPoint { get; set; }
	}
}