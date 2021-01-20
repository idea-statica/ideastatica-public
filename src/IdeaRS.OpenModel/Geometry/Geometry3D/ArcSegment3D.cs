namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Represents an arc segment in three-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry3D.ArcSegment3D,CI.Geometry3D", "CI.Geometry3D.IArcSegment3D,CI.BasicTypes")]
	public class ArcSegment3D : Segment3D
	{
		/// <summary>
		/// Gets or sets the reference to <see cref="IdeaRS.OpenModel.Geometry3D.Point3D "/> of circular arc somewhere between start and end point.
		/// </summary>
		public ReferenceElement Point { get; set; }
	}
}