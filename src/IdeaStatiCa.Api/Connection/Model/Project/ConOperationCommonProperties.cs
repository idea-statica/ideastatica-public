namespace IdeaStatiCa.Api.Connection.Model.Project
{
	/// <summary>
	/// Common properties for operations in connection
	/// </summary>
	public class ConOperationCommonProperties
	{
		public int? WeldMaterialId { get; set; }

		public int? PlateMaterialId { get; set; }

		public int? BoltAssemblyId { get; set; }
	}

	/// <summary>
	/// Type for weld design
	/// </summary>
	public enum ConWeldDesignType
	{
		FullStrength,
		MinimumDuctility,
		OverStrengthFactor,
		CapacityEstimation
	}
}
