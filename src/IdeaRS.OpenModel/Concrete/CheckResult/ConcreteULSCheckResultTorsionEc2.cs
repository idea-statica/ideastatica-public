namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// ULS Check Torsion Ec2
	/// </summary>
	public class ConcreteULSCheckResultTorsionEc2 : ConcreteULSCheckResultTorsion
	{
		///// <summary>
		///// Gets code type
		///// </summary>
		//public EC2CodeType CodeType { get; internal set; }

		///// <summary>
		///// area enclosed by the center-lines of the connecting walls
		///// </summary>
		//public double ak { get; set; }

		///// <summary>
		///// perimeter of the area Ak
		///// </summary>
		//public double uk { get; set; }

		///// <summary>
		///// effective wall thickness
		///// </summary>
		//public double te { get; set; }

		///// <summary>
		///// angle of compression struts (see Figure 6.5)
		///// </summary>
		//public double theta { get; set; }

		///// <summary>
		///// longitudinal reinforcement
		///// </summary>
		//public double asl { get; set; }

		///// <summary>
		///// prestress reinforcement
		///// </summary>
		//public double asp { get; set; }

		///// <summary>
		///// shear reinforcement
		///// </summary>
		//public double asw { get; set; }

		///// <summary>
		///// strength of shear reinforcement
		///// </summary>
		//public double fywd { get; set; }

		///// <summary>
		///// longitudinal force due to torsion
		///// </summary>
		//public double ft { get; set; }

		///// <summary>
		///// coefficient ni
		///// </summary>
		//public double ni { get; set; }

		///// <summary>
		///// coefficient alpha cw
		///// </summary>
		//public double alphacw { get; set; }

		/// <summary>
		/// Design value of the applied torsional moment 
		/// </summary>
		public double Ted { get; set; }

		/// <summary>
		/// torsional cracking moment
		/// </summary>
		public double Trdc { get; set; }

		/// <summary>
		/// design torsional resistance moment
		/// </summary>
		public double Trdmax { get; set; }

		/// <summary>
		/// design torsional resistance moment of shear reinforcement
		/// </summary>
		public double Trds { get; set; }

		/// <summary>
		/// decisive design torsional resistance
		/// </summary>
		public double Trd { get; set; }

		///// <summary>
		///// infinity check value, maximum display check value
		///// </summary>
		//public double InfinityCheckValue { get; set; }
	}
}