namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// Check Detailing Ec2
	/// </summary>
	public class ConcreteCheckDetailingEc2 : ConcreteCheckDetailing
	{
		/// <summary>
		/// check value of longitudinal reinforcement
		/// </summary>
		public double CheckValueLongReinf { get; set; }

		/// <summary>
		/// check value of shear  reinforcement
		/// </summary>
		public double CheckValueShearReinf { get; set; }
	}
}