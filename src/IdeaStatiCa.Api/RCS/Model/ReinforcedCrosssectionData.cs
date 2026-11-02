using System.Collections.Generic;

namespace IdeaStatiCa.Api.RCS.Model
{
	/// <summary>
	/// Input DTO for creating a reinforced cross-section.
	/// Contains embedded cross-section geometry instead of references.
	/// </summary>
	public class ReinforcedCrossSectionData
	{
		/// <summary>
		/// Name of the reinforced cross-section
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Cross-section geometry definition (embedded, not a reference)
		/// </summary>
		public RcsCrossSectionData CrossSection { get; set; }

		/// <summary>
		/// Reinforcement bars
		/// </summary>
		public List<RcsReinforcedBarData> Bars { get; set; } = new List<RcsReinforcedBarData>();

		/// <summary>
		/// Stirrups
		/// </summary>
		public List<RcsStirrupsData> Stirrups { get; set; } = new List<RcsStirrupsData>();

		/// <summary>
		/// Tendon bars (for prestressed sections)
		/// </summary>
		public List<RcsTendonBarData> TendonBars { get; set; } = new List<RcsTendonBarData>();

		/// <summary>
		/// Tendon ducts (for prestressed sections)
		/// </summary>
		public List<RcsTendonDuctData> TendonDucts { get; set; } = new List<RcsTendonDuctData>();
	}
}