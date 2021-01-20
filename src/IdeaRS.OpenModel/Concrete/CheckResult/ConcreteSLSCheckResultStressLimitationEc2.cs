namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// Type of SLS check in extreme result
	/// </summary>
	public enum TypeSLSLimitStressCheck
	{
		/// <summary>
		/// not set value
		/// </summary>
		notSet = -1,

		/// <summary>
		/// check 7.2 (2) is used
		/// </summary>
		Check_7_2_2_Concrete_fck = 6,

		/// <summary>
		/// check 7.2 (3) is used
		/// </summary>
		Check_7_2_3_Concrete_fck = 7,

		/// <summary>
		/// check 7.2 (5) stress in reinforcement bars is used
		/// </summary>
		Check_7_2_5_Tendons_fpk = 8,

		/// <summary>
		/// check 7.2 (5) stress in tendons is used
		/// </summary>
		Check_7_2_5_ReinforcementBars_fyk = 9,

		/// <summary>
		/// check 5.10.3 (2) stress in tendons is used
		/// </summary>
		Check_5_10_3_2_Tendons = 10,

		/// <summary>
		/// check 5.10.2.1 (1) stress in tendons is used
		/// </summary>
		Check_5_10_2_1_1_Tendons = 11,
	}

	/// <summary>
	/// SLS Check Stress Limitation Ec2
	/// </summary>
	public class ConcreteSLSCheckResultStressLimitationEc2 : ConcreteSLSCheckResultStressLimitation
	{
		///// <summary>
		///// type of anchorage zone
		///// </summary>
		//public TypeOfAnchorageZoneData TypeOfAnchorageZoneData { get; internal set; }

		///// <summary>
		///// Gets code type
		///// </summary>
		//public EC2CodeType CodeType { get; internal set; }

		/// <summary>
		/// type of SLS limit stress check
		/// </summary>
		public TypeSLSLimitStressCheck TypeCheck { get; set; }

		/// <summary>
		/// type of SLS limit stress check
		/// </summary>
		public double Stress { get; set; }

		/// <summary>
		/// type of SLS limit stress check
		/// </summary>
		public double LimitStress { get; set; }

		///// <summary>
		///// off/on concrete extreme
		///// </summary>
		//public bool ConcreteExtremIsOff { get; set; }

		///// <summary>
		///// is cross-section cracked
		///// </summary>
		//public bool CrackedCrossSection { get; set; }
	}
}