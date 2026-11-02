namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a bend rebar hook in 3D.
	/// </summary>
	public class RebarHookBend: RebarHookBase
	{
		/// <summary>
		/// Length of the Hook. 
		/// </summary>
		public double Length { get; set; }

		/// <summary>
		/// Bending Angle in X-Y Plane. 
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// Bending Radius. 
		/// It will override Vertex Radius in case of <c>RebarArcRadVertex3D</c> and <c>RebarArcRadVertex3D</c>. 
		/// </summary>
		public double Radius { get; set; }
	}
}