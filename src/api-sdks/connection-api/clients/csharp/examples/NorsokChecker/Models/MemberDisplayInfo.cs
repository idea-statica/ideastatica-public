namespace NorsokChecker.Models
{
	/// <summary>
	/// Display model for the Members DataGrid — auto-populated from API.
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
		public bool IsCHS => Shape == "CHS";
	}
}
