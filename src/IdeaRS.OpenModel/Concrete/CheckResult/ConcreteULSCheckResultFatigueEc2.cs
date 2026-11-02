namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// Verification of concrete under compression or shear
	/// </summary>
	public enum FatigueMethod
	{
		/// <summary>
		/// the method has not been set
		/// </summary>
		notSet = 0,

		/// <summary>
		/// method 6.8.7 (1) according to EN 1992-1-1
		/// </summary>
		Method_6_8_7_1,

		/// <summary>
		/// method 6.8.7 (2) according to EN 1992-1-1
		/// </summary>
		Method_6_8_7_2,

		/// <summary>
		/// method 6.8.7 (101) according to EN 1992-2
		/// </summary>
		Method_6_8_7_101,

		/// <summary>
		/// method 6.8.7 (101) according to EN 1992-2
		/// </summary>
		Method_6_8_7_101_ANNEX_NEN,

		/// <summary>
		/// 6.8.7 Verification of concrete under compression or shear
		/// </summary>
		Method_Shear_6_8_7_4,

		/// <summary>
		/// 6.8.5 Verification using damage equivalent stress range - reinfrocement
		/// </summary>
		Method_Reinf_6_8_5_3,

		/// <summary>
		/// 6.8.5 Verification using damage equivalent stress range - prestress
		/// </summary>
		Method_Prestress_6_8_5_3,

		/// <summary>
		/// EN 1992-2, annex NN eq. NN.112
		/// </summary>
		Method_NN112,

		/// <summary>
		/// reinforcement EN 1992-2, annex NN eq. NN.102
		/// </summary>
		Method_Reinf_NN102,

		/// <summary>
		/// reinforcement EN 1992-2, annex NN eq. NN.107
		/// </summary>
		Method_Reinf_NN107,

		/// <summary>
		/// tendon EN 1992-2, annex NN eq. NN.102
		/// </summary>
		Method_Tendon_NN102,

		/// <summary>
		/// tendon EN 1992-2, annex NN eq. NN.107
		/// </summary>
		Method_Tendon_NN107,

		/// <summary>
		/// method 6.8.7 (2) according to EN 1992-1-1 under shear
		/// </summary>
		Method_6_8_7_2_Shear,

		/// <summary>
		/// 6.8.5 Verification of concrete under compression or shear
		/// </summary>
		Method_Reinf_6_8_5_3_Shear,
	}

	/// <summary>
	/// ULS Check Fatigue Ec2
	/// </summary>
	public class ConcreteULSCheckResultFatigueEc2 : ConcreteULSCheckResultFatigue
	{
		/// <summary>
		/// decision fatigue check
		/// </summary>
		public FatigueMethod DecissionMethod { get; set; }

		/// <summary>
		/// internal forces for fatigue calculation max
		/// </summary>
		public IdeaRS.OpenModel.Result.ResultOfInternalForces InternalForceMax { get; set; }

		/// <summary>
		/// internal forces for fatigue calculation min
		/// </summary>
		public IdeaRS.OpenModel.Result.ResultOfInternalForces InternalForceMin { get; set; }
	}
}