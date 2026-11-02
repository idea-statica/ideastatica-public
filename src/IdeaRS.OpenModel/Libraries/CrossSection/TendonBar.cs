using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Posttensioned reinforcement type
	/// </summary>
	[DataContract]
	public enum FatigueTypeOfPrestressingSteel
	{
		/// <summary>
		/// Table 6.4N: Parameters for S-N curves of prestressing steel - row 2
		/// </summary>
		PostTensioningSingleStrandsInPlasticDucts,

		/// <summary>
		/// Table 6.4N: Parameters for S-N curves of prestressing steel - row 3
		/// </summary>
		PostTensioningStraightTendonsOrCurvedTendonsInPlasticDucts,

		/// <summary>
		/// Table 6.4N: Parameters for S-N curves of prestressing steel - row 4
		/// </summary>
		PostTensioningCurvedTendonsInSteelDucts,

		/// <summary>
		/// Table 6.4N: Parameters for S-N curves of prestressing steel - row 5
		/// </summary>
		PostTensioningSplicingDevices,

		/// <summary>
		/// Table 6.4N: Parameters for S-N curves of prestressing steel - row 1
		/// </summary>
		PreTensioning,

		/// <summary>
		/// Table 6.4N: Parameters for S-N curves of prestressing steel - special type
		/// </summary>
		ExternalTendon,
	}

	/// <summary>
	/// Tendon bar type
	/// </summary>
	[DataContract]
	public enum TendonBarType
	{
		/// <summary>
		/// Post-tensioned
		/// </summary>
		Posttensioned,

		/// <summary>
		/// Pre-tensioned
		/// </summary>
		Pretensioned
	}

	/// <summary>
	/// Tendon bar
	/// </summary>
	[OpenModelClass("CI.Services.Concrete.ReinforcedSection.TendonBar,CI.ReinforcedSection")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class TendonBar : OpenObject
	{
		/// <summary>
		/// Tendon Id
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Tendon type
		/// </summary>
		public TendonBarType TendonType { get; set; }

		/// <summary>
		/// Position of bar
		/// </summary>
		[OpenModelProperty("CentrePoint")]
		public Point2D Point { get; set; }

		/// <summary>
		/// Material
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// order of tendon prestessing
		/// </summary>
		public int PrestressingOrder { get; set; }

		/// <summary>
		/// number of ropes in tendon
		/// </summary>
		public int NumStrandsInTendon { get; set; }

		/// <summary>
		/// set of prestress reinforcement type
		/// </summary>
		public FatigueTypeOfPrestressingSteel PrestressReinforcementType { get; set; }

		/// <summary>
		/// Phase
		/// </summary>
		public int Phase { get; set; }

		/// <summary>
		/// Tendon duct
		/// </summary>
		public TendonDuct TendonDuct { get; set; }
	}
}