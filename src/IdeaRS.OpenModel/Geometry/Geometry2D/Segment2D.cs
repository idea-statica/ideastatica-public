using IdeaRS.OpenModel.Message;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Represents a segment in two-dimensional space.
	/// </summary>
	[XmlInclude(typeof(LineSegment2D))]
	[XmlInclude(typeof(ArcSegment2D))]
	[KnownType(typeof(LineSegment2D))]
	[KnownType(typeof(ArcSegment2D))]
	[DataContract]
	public abstract class Segment2D : OpenObject
	{
		/// <summary>
		/// Gets or sets the endpoint of the segment.
		/// </summary>
		[DataMember]
		public Point2D EndPoint { get; set; }
	}
}