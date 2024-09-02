using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Represents a polygon in two-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry2D.Polygon2D,CI.Geometry2D")]
	[DataContract]
	public class Polygon2D : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Polygon2D()
		{
			Points = new List<Point2D>();
		}

		/// <summary>
		/// List of polygon points
		/// </summary>
		[DataMember]
		public List<Point2D> Points { get; set; }
	}
}