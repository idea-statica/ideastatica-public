using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a Point in 3D.
	/// Defines an Arc segment by radius.
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.RebarPoint3D,CI.StructuralElements", "CI.StructModel.Structure.IRebarPoint3D,CI.BasicTypes")]
	public class RebarPoint3D : Point3D
	{
		/// <summary>
		/// Gets or sets the radius of the Arc
		/// </summary>
		public double BendRadius { get; set; }
	}
}