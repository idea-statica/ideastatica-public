namespace NorsokChecker.Models
{
	/// <summary>
	/// User inputs for NORSOK N-004 §5 Design Classification.
	/// </summary>
	public class DesignClassificationInput
	{
		/// <summary>Failure would have substantial consequences (loss of life, pollution, major financial)</summary>
		public bool SubstantialConsequences { get; set; } = true;

		/// <summary>Structure has residual strength after member/joint failure</summary>
		public bool ResidualStrength { get; set; }

		/// <summary>Joint has high geometric complexity (multiplanar, full-pen welds, high restraint)</summary>
		public bool HighComplexity { get; set; } = true;

		/// <summary>High fatigue utilisation (DFF &lt; 3)</summary>
		public bool HighFatigue { get; set; }

		/// <summary>Through-thickness stresses are present</summary>
		public bool ThroughThickness { get; set; }
	}
}
