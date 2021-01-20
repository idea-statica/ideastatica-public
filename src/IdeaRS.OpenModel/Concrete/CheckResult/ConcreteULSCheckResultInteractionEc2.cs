namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// ULS Check Interaction Ec2
	/// </summary>
	public class ConcreteULSCheckResultInteractionEc2 : ConcreteULSCheckResultInteraction
	{
		///// <summary>
		///// Gets code type
		///// </summary>
		//public EC2CodeType CodeType { get; internal set; }

		/// <summary>
		/// Design value of the applied axial force
		/// </summary>
		public double Ned { get; set; }

		/// <summary>
		/// Design value of the applied bending moment around y axis
		/// </summary>
		public double Medy { get; set; }

		/// <summary>
		/// Design value of the applied bending moment around z axis
		/// </summary>
		public double Medz { get; set; }

		/// <summary>
		/// Design value of the applied shear force
		/// </summary>
		public double Ved { get; set; }

		/// <summary>
		/// Design value of the applied torsional moment
		/// </summary>
		public double Ted { get; set; }

		/// <summary>
		/// Calculated value of the exploitation of the cross-section (for interaction of shear and torsion) related to the limit value
		/// </summary>
		public double CheckValueShearAndTorsion { get; set; }

		/// <summary>
		/// Calculated value of the exploitation of the cross-section (for interaction of shear, torsion and bending) related to the limit value
		/// </summary>
		public double CheckValueShearTorsionAndBending { get; set; }

		///// <summary>
		///// shear results
		///// </summary>
		//public ConcreteULSCheckResultShearEc2 Shear { get; set; }

		///// <summary>
		///// torsion results
		///// </summary>
		//public ConcreteULSCheckResultTorsionEc2 Torsion { get; set; }

		///// <summary>
		///// response results
		///// </summary>
		//public ConcreteULSCheckResultResponseEc2 Response { get; set; }

		///// <summary>
		///// condition1
		///// </summary>
		//public double condition1 { get; set; }

		///// <summary>
		///// condition2
		///// </summary>
		//public double condition2 { get; set; }

		///// <summary>
		///// check value of concrete
		///// </summary>
		//public double CheckValueConcrete { get; set; }

		///// <summary>
		///// result of concrete
		///// </summary>
		//public CheckResult ResultConcrete { get; set; }

		///// <summary>
		///// shear reinforcement
		///// </summary>
		//public double asw { get; set; }

		///// <summary>
		///// force in shear reinforcement
		///// </summary>
		//public double fstw { get; set; }

		///// <summary>
		///// maximal force in shear reinforcement
		///// </summary>
		//public double fstwmax { get; set; }

		///// <summary>
		///// check value of force in shear reinforcement
		///// </summary>
		//public double CheckValueFstw { get; set; }

		///// <summary>
		///// result of force in shear reinforcement
		///// </summary>
		//public CheckResult ResultFstw { get; set; }

		///// <summary>
		///// longitudinal reinforcement
		///// </summary>
		//public double asl { get; set; }

		///// <summary>
		///// force in longitudinal reinforcement
		///// </summary>
		//public double fstl { get; set; }

		///// <summary>
		///// maximal force in longitudinal reinforcement
		///// </summary>
		//public double fstlmax { get; set; }

		///// <summary>
		///// check value of force in longitudinal reinforcement
		///// </summary>
		//public double CheckValueFstl { get; set; }

		///// <summary>
		///// result of force in shear reinforcement
		///// </summary>
		//public CheckResult ResultFstl { get; set; }

		///// <summary>
		///// force in longitudinal reinforcement due to bending and normal force
		///// </summary>
		//public double fb { get; set; }

		///// <summary>
		///// force in longitudinal reinforcement due to shear and torsion
		///// </summary>
		//public double fst { get; set; }

		///// <summary>
		///// strain in longitudinal reinforcement due to shear
		///// </summary>
		//public double epss { get; set; }

		///// <summary>
		///// strain in longitudinal reinforcement due to torsion
		///// </summary>
		//public double epst { get; set; }

		///// <summary>
		///// infinity check value, maximum display check value
		///// </summary>
		//public double InfinityCheckValue { get; set; }
	}
}