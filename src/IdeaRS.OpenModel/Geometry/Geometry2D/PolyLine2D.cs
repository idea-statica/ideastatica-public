using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Represents a polyline in two-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry2D.PolyLine2D,CI.Geometry2D")]
	[DataContract]
	public class PolyLine2D : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PolyLine2D()
		{
			Segments = new List<Segment2D>();
		}

		/// <summary>
		/// Gets or sets the point where the <c>PolyLine2D</c> begins.
		/// </summary>
		[DataMember]
		public Point2D StartPoint { get; set; }

		/// <summary>
		/// Gets segments of <c>PolyLine2D</c>.
		/// </summary>
		[DataMember]
		public List<Segment2D> Segments { get; set; }
	}
}