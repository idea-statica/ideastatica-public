namespace NorsokChecker.Models
{
	/// <summary>
	/// Display model for the Members DataGrid — auto-populated from API.
	/// L and k are editable per member (buckling parameters).
	/// </summary>
	public class MemberDisplayInfo
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		/// <summary>"Chord" or "Brace"</summary>
		public string Role { get; set; } = string.Empty;
		/// <summary>"CHS", "RHS", "I-section", "Channel", "Angle", "Other"</summary>
		public string Shape { get; set; } = string.Empty;
		/// <summary>Profile description e.g. "CHS 500/20", "HEB 300"</summary>
		public string Profile { get; set; } = string.Empty;
		/// <summary>CHS: outside diameter [mm]. I: height h [mm].</summary>
		public double Diameter { get; set; }
		/// <summary>CHS: wall thickness [mm]. I: web thickness tw [mm].</summary>
		public double WallThickness { get; set; }
		public double Fy { get; set; } = 355;
		public string MaterialName { get; set; } = string.Empty;
		/// <summary>Unbraced length [mm] — editable</summary>
		public double L { get; set; } = 5000;
		/// <summary>Effective length factor — editable (Table 6-2)</summary>
		public double K { get; set; } = 0.7;
		public bool IsCHS => Shape == "CHS";

		/// <summary>Display string for the geometry column</summary>
		public string GeometryDisplay => Shape switch
		{
			"CHS" => Diameter > 0 ? $"D={Diameter:F0} t={WallThickness:F1}" : "",
			"I-section" => Diameter > 0 ? $"h={Diameter:F0} tw={WallThickness:F1}" : "",
			"RHS" => Diameter > 0 ? $"h={Diameter:F0} t={WallThickness:F1}" : "",
			_ => ""
		};
	}
}
