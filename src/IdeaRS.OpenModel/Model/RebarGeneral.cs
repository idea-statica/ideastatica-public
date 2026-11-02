namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Determine type of rebar General
	/// </summary>
	public enum RebarGeneralType : int
	{
		/// <summary>
		/// Not defined type
		/// </summary>
		NotDefined = 0,

		/// <summary>
		/// Rebar Single
		/// </summary>
		Single = 1,

		/// <summary>
		/// Rebar Stirrup
		/// </summary>
		Stirrup = 2
	}

	/// <summary>
	/// Represents a single main rebar in 3D space.
	/// Holds data from generated rebar or rebar imported from Tekla.
	/// the rebarShape is in global coordinates and it is not possible to prject it along Member1D, Polyline, ... (referenceLine)
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.RebarGeneral,CI.StructuralElements", "CI.StructModel.Structure.IRebarGeneral,CI.BasicTypes")]
	public class RebarGeneral : RebarBase
	{
		/// <summary>
		/// create a new instance.
		/// </summary>
		public RebarGeneral()
		{
		}

		/// <summary>
		/// Normal vector of the plane where the rebar lie.
		/// Used to deterine if this rebar is stirrup.
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D PlaneNormal { get; set; }

		/// <summary>
		/// Rebar type
		/// </summary>
		public RebarGeneralType RebarType { get; set; }
	}
}