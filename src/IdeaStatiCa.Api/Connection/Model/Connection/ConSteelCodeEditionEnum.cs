using System.Runtime.Serialization;

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
		[EnumMember(Value = "NotSpecified")]
		NotSpecified = 0,

		[EnumMember(Value = "AISC_360_10_LRFD")]
		AISC_360_10_LRFD = 1,
		[EnumMember(Value = "AISC_360_10_ASD")]
		AISC_360_10_ASD = 2,
		[EnumMember(Value = "AISC_360_16_LRFD")]
		AISC_360_16_LRFD = 3,
		[EnumMember(Value = "AISC_360_16_ASD")]
		AISC_360_16_ASD = 4,
		[EnumMember(Value = "AISC_360_22_LRFD")]
		AISC_360_22_LRFD = 5,
		[EnumMember(Value = "AISC_360_22_ASD")]
		AISC_360_22_ASD = 6,

		[EnumMember(Value = "CSA_S16_14")]
		CSA_S16_14 = 7,
		[EnumMember(Value = "CSA_S16_19")]
		CSA_S16_19 = 8,

		[EnumMember(Value = "EN_1993_1_8_2005")]
		EN_1993_1_8_2005 = 9,
		[EnumMember(Value = "EN_1993_1_8_2024")]
		EN_1993_1_8_2024 = 10
	}
}
