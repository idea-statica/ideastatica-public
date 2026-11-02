namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// SLS Check Crack width Ec2
	/// </summary>
	public class ConcreteSLSCheckResultCrackWidthEc2 : ConcreteSLSCheckResultCrackWidth
	{
		/// <summary>
		/// Normal force for quasi-permanent combination
		/// </summary>
		public double N { get; set; }

		/// <summary>
		/// Bending moment around y axis for quasi-permanent combination
		/// </summary>
		public double My { get; set; }

		/// <summary>
		/// Bending moment around z axis for quasi-permanent combination
		/// </summary>
		public double Mz { get; set; }

		/// <summary>
		/// calculated crack width value
		/// </summary>
		public double W { get; set; }

		/// <summary>
		/// limiting crack width value
		/// </summary>
		public double Wlim { get; set; }
	}
}