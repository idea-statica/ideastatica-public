namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a geometrical shape for Rebar in 3D space.
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.RebarShape,CI.StructuralElements", "CI.StructModel.Structure.IRebarShape,CI.BasicTypes")]
	public class RebarShape: OpenElementId
	{
		/// <summary>
		/// Gets or sets the segments of the rebar shape.
		/// </summary>
		public RebarPolyLine3D SourceGeometry { get; set; }

		/// <summary>
		/// Gets or sets the start rebar hook.
		/// </summary>
		public RebarHookBase StartHook { get; set; }

		/// <summary>
		/// Gets or sets the end rebar hook.
		/// </summary>
		public RebarHookBase EndHook { get; set; }
	}
}