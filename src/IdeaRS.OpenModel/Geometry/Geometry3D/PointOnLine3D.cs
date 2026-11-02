namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Represents a Point dependent on the line in three-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry3D.PointOnLine3D,CI.Geometry3D", "CI.Geometry3D.IPointOnLine3D,CI.BasicTypes")]
	public class PointOnLine3D : OpenElementId
	{
		/// <summary>
		/// Gets or sets the reference to <see cref="IdeaRS.OpenModel.Geometry3D.PolyLine3D "/> line or <see cref="IdeaRS.OpenModel.Geometry3D.Segment3D "/> segment.
		/// </summary>
		public ReferenceElement Geometry { get; set; }

		/// <summary>
		/// Gets or sets the position value on the line.
		/// </summary>
		public double Position { get; set; }

		/// <summary>
		/// Gets or sets absolute or relative representation of the position value.
		/// </summary>
		public bool IsRelative { get; set; }

		/// <summary>
		/// Gets or sets the position value is measured from the start or the ent of the line.
		/// </summary>
		public bool IsFromStart { get; set; }
	}
}