namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Combination according to SIA in the model
	/// </summary>
	[OpenModelClass("CI.StructModel.Loading.CombiInputEC,CI.Loading", "CI.StructModel.Loading.ICombiInput,CI.BasicTypes", typeof(CombiInput))]
	public class CombiInputSIA : CombiInput
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CombiInputSIA()
			: base()
		{
		}

		/// <summary>
		/// Type of EC combination
		/// </summary>
		[OpenModelProperty("TypeCombiEC")]
		public TypeOfCombiSIA TypeCombiSIA { get; set; }

		/// <summary>
		/// Type of combination
		/// </summary>
		[OpenModelProperty("TypeLinear_Envelope")]
		public TypeCalculationCombiSIA TypeCalculationCombi { get; set; }
	}

	/// <summary>
	/// Type of combination
	/// </summary>
	public enum TypeCalculationCombiSIA
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
		/// Combination according to code
		/// </summary>
		Code = 2,
	}

	/// <summary>
	/// Type of combination
	/// </summary>
	public enum TypeOfCombiSIA
	{
		/// <summary>
		/// combination
		/// </summary>
		ULS = 0,

		/// <summary>
		/// accidental combination 1
		/// </summary>
		Accidental = 2,

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