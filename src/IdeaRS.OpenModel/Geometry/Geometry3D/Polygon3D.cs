using System.Collections.Generic;

namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Represents a polygon in three-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry3D.Polygon3D,CI.Geometry3D")]
	public class Polygon3D : OpenObject
	{
		/// <summary>
		/// Contructor
		/// </summary>
		public Polygon3D()
		{
			Points = new List<Point3D>();
		}

		/// <summary>
		/// List of polygon points
		/// </summary>
		public List<Point3D> Points { get; set; }
	}
}