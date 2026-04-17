namespace NorsokChecker.Models
{
	/// <summary>
	/// Aggregated Norsok N-004 compliance result for a single connection.
	/// </summary>
	public class NorsokResult
	{
		public int ConnectionId { get; set; }
		public double MaxUtilization { get; set; }
		public bool Passed { get; set; }
		public bool OverallPassed { get; set; }
		public List<NorsokFormulaResult> FormulaResults { get; set; } = new();
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
