namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Combination according to EC in the model
	/// </summary>
	[OpenModelClass("CI.StructModel.Loading.CombiInputEC,CI.Loading", "CI.StructModel.Loading.ICombiInput,CI.BasicTypes", typeof(CombiInput))]
	public class CombiInputEC : CombiInput
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CombiInputEC()
			: base()
		{
		}

		/// <summary>
		/// Type of EC combination
		/// </summary>
		public TypeOfCombiEC TypeCombiEC { get; set; }

		/// <summary>
		/// Type of combination
		/// </summary>
		[OpenModelProperty("TypeLinear_Envelope")]
		public TypeCalculationCombiEC TypeCalculationCombi { get; set; }
	}

	/// <summary>
	/// Type of combination
	/// </summary>
	public enum TypeCalculationCombiEC
	{
		/// <summary>
		/// Linear combination
		/// </summary>
		Linear = 0,

		/// <summary>
		/// Envelope combination
		/// </summary>
		Envelope = 1,

		/// <summary>
		/// Combination according to 6.10
		/// </summary>
		Code610 = 2,

		/// <summary>
		/// Combination according to 6.10ab
		/// </summary>
		Code610ab = 3,

		/// <summary>
		/// Nonlinear combination
		/// </summary>
		NonLinear = 4,
	}

	/// <summary>
	/// Type of combination
	/// </summary>
	public enum TypeOfCombiEC
	{
		/// <summary>
		/// combination
		/// </summary>
		ULS = 0,

		/// <summary>
		/// combination
		/// </summary>
		ULS_SET_C = 1,
		
		/// <summary>
		/// accidental combination 1
		/// </summary>
		Accidental = 2,
		
		/// <summary>
		/// accidental combination 1
		/// </summary>
		Accidental_2 = 3,

		/// <summary>
		/// seismic combination 
		/// </summary>
		Seismic = 4,

		/// <summary>
		/// characteristic combination
		/// </summary>
		SLS_Char = 5,

		/// <summary>
		/// frequent combination
		/// </summary>
		SLS_Freq = 6,

		/// <summary>
		/// quasi combination
		/// </summary>
		SLS_Quasi = 7,

		/// <summary>
		/// Fatigue combination
		/// </summary>
		Fatigue = 8,
	}
}