namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a single main rebar in 3D space.
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.RebarSingle,CI.StructuralElements", "CI.StructModel.Structure.IRebarSingle,CI.BasicTypes")]
	public class RebarSingle : RebarBase
	{
		/// <summary>
		/// create a new instance.
		/// </summary>
		public RebarSingle()
		{
		}

		/// <summary>
		/// Returns the translation of the rebar geometry (RebarShape).
		/// It is an Insert point in reference line coordinate system
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D Translation { get; set; }

		/// <summary>
		/// Returns the rotation (in radians) of the rebar geometry (RebarShape).
		/// It is an angle of rotation around AxisX of the reference line.
		/// </summary>
		public double RotationXrad { get; set; }

		///// <summary>
		///// Insert Point of the rebar geometry (Shape)
		///// </summary>
		//public IdeaRS.OpenModel.Geometry3D.Point3D InsertPoint { get; set; }

		//  /// <summary>
		///// Gets or sets the destination X-Axis (direction) where the rebar geometry will be transformed to. 
		//  /// It represents AxisX if the destination Plane defined by axis X and Y.
		///// </summary>
		//public IdeaRS.OpenModel.Geometry3D.Vector3D AxisX { get; set; }

		//  /// <summary>
		///// Gets or sets the X-Axis (direction) where the rebar geometry will be transformed to. 
		//  /// It represents AxisX if the destination Plane defined by axis X and Y.
		///// </summary>
		//  public IdeaRS.OpenModel.Geometry3D.Vector3D AxisZ  { get; set; }
	}
}