namespace IdeaStatiCa.BimApi.Results
{
	public class InternalForcesData : IIdeaResultData
	{
		/// <summary>
		/// Normal force
		/// </summary>
		public double N { get; set; }

		/// <summary>
		/// Shear force in y direction
		/// </summary>
		public double Qy { get; set; }

		/// <summary>
		/// Shear force in z direction
		/// </summary>
		public double Qz { get; set; }

		/// <summary>
		/// Bending moment around x-axis
		/// </summary>
		public double Mx { get; set; }

		/// <summary>
		/// Bending moment around y-axis
		/// </summary>
		public double My { get; set; }

		/// <summary>
		/// Bending moment around z-axis
		/// </summary>
		public double Mz { get; set; }
	}
}