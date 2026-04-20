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
		/// <summary>"CHS", "RHS", "I-section", "Other"</summary>
		public string Shape { get; set; } = string.Empty;
		public double Diameter { get; set; }
		public double WallThickness { get; set; }
		public double Fy { get; set; } = 355;
		public string MaterialName { get; set; } = string.Empty;
		/// <summary>Unbraced length [mm] — editable</summary>
		public double L { get; set; } = 5000;
		/// <summary>Effective length factor — editable (Table 6-2)</summary>
		public double K { get; set; } = 0.7;
		public bool IsCHS => Shape == "CHS";
	}
}
