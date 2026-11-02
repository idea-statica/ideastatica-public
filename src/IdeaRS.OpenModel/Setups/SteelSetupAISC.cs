
namespace IdeaRS.OpenModel
{

	/// <summary>
	/// Steel setup IBC class
	/// </summary>
	public class SteelSetupAISC : SteelSetup
	{
		/// <summary>
		/// ACI 318
		/// </summary>
		public double ReductionFactorTension {get; set;}

		/// <summary>
		/// ACI 318
		/// </summary>
		public double ReductionFactorShear {get; set;}

		/// <summary>
		/// Safety factor BoltTensileShear_Omega
		/// </summary>
		public double BoltTensileShear_Omega {get; set;}

		/// <summary>
		/// Safety factor BoltTensileShearCombined_Omega
		/// </summary>
		public double BoltTensileShearCombined_Omega {get; set;}

		/// <summary>
		/// Safety factor BoltBearing_Omega
		/// </summary>
		public double BoltBearing_Omega {get; set;}

		/// <summary>
		/// Safety factor FilletWelds_Omega
		/// </summary>
		public double FilletWelds_Omega {get; set;}

		/// <summary>
		/// Safety factor BoltTensileShear_Phi
		/// </summary>
		public double BoltTensileShear_Phi {get; set;}

		/// <summary>
		/// Safety factor BoltTensileShearCombined_Phi
		/// </summary>
		public double BoltTensileShearCombined_Phi {get; set;}

		/// <summary>
		/// Safety factor BoltBearing_Phi
		/// </summary>
		public double BoltBearing_Phi {get; set;}

		/// <summary>
		/// Safety factor FilletWelds_Phi
		/// </summary>
		public double FilletWelds_Phi {get; set;}

		/// <summary>
		/// Safety factor MaterialFy_Omega
		/// </summary>
		public double MaterialFy_Omega {get; set;}

		/// <summary>
		/// Safety factor MaterialFy_Phi
		/// </summary>
		public double MaterialFy_Phi {get; set;}

		/// <summary>
		/// Safety factor BoltSlipRes_Phi
		/// </summary>
		public double BoltSlipRes_Phi {get; set;}

		/// <summary>
		/// Safety factor BoltSlipRes_Phi
		/// </summary>
		public double BoltSlipRes_Omega {get; set;}
	}
}
