namespace NorsokChecker.Models
{
	/// <summary>
	/// Result of a Norsok M-001 compliance check for a single connection.
	/// </summary>
	public class NorsokResult
	{
		public int ConnectionId { get; set; }
		public double MaxUtilization { get; set; }
		public bool Passed { get; set; }
		public bool OverallPassed { get; set; }
		public List<NorsokCheckItem> CheckItems { get; set; } = new();
	}

	public class NorsokCheckItem
	{
		public string CheckName { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public double Utilization { get; set; }
		public bool Passed { get; set; }
		public string Details { get; set; } = string.Empty;
	}
}
