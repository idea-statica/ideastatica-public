namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// The edition of the steel design code a connection is designed and checked to.
	///
	/// For the American code this is what decides between LRFD and ASD - the two give different
	/// resistances - so it is not a label but an input of the check.
	/// </summary>
	public enum ConSteelCodeEditionEnum
	{
		/// <summary>
		/// No edition. On input it means "keep the edition the connection already has"; on output it
		/// is reported for a project saved before the editions existed.
		/// </summary>
		NotSpecified = 0,

		AISC_360_10_LRFD = 1,
		AISC_360_10_ASD = 2,
		AISC_360_16_LRFD = 3,
		AISC_360_16_ASD = 4,
		AISC_360_22_LRFD = 5,
		AISC_360_22_ASD = 6,

		CSA_S16_14 = 7,
		CSA_S16_19 = 8,

		EN_1993_1_8_2005 = 9,
		EN_1993_1_8_2024 = 10
	}
}
