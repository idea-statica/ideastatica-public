using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Represents a line segment in two-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry2D.LineSegment2D,CI.Geometry2D")]
	[DataContract]
	public class LineSegment2D : Segment2D
	{
	}
}