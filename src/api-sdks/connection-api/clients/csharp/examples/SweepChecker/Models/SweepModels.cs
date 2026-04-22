using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SweepChecker.Models
{
	/// <summary>
	/// A discovered parameter that can be edited / optionally swept.
	/// </summary>
	public class SweepParameter : INotifyPropertyChanged
	{
		private bool _sweep;
		private double _start;
		private double _end;
		private double _step = 0.05;
		private string _singleValue = "";

		public string Key { get; set; } = "";
		public string Description { get; set; } = "";
		public string Unit { get; set; } = "";
		public string ParameterType { get; set; } = "";
		public string CurrentValue { get; set; } = "";

		/// <summary>If false, <see cref="SingleValue"/> is pushed as-is. If true, the triple (Start, End, Step) drives a loop.</summary>
		public bool Sweep { get => _sweep; set { if (_sweep != value) { _sweep = value; OnChanged(); } } }

		public double Start { get => _start; set { if (_start != value) { _start = value; OnChanged(); } } }
		public double End { get => _end; set { if (_end != value) { _end = value; OnChanged(); } } }
		public double Step { get => _step; set { if (_step != value) { _step = value; OnChanged(); } } }

		/// <summary>Used when Sweep == false. Interpreted as an expression sent to the IDEA API.</summary>
		public string SingleValue { get => _singleValue; set { if (_singleValue != value) { _singleValue = value; OnChanged(); } } }

		public IEnumerable<double> EnumerateValues()
		{
			if (!Sweep) yield break;
			if (Step <= 0) yield break;
			for (double v = Start; v <= End + 1e-9; v += Step)
				yield return Math.Round(v, 6);
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnChanged([CallerMemberName] string? n = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
	}

	// ───────── JSON round-trip schema (matches Python Simplified_03/Results_reading_06) ─────────

	public class SweepOutput
	{
		public string project_file { get; set; } = "";
		public string project_id { get; set; } = "";
		public Dictionary<string, double> sweep_ranges_start { get; set; } = new();
		public Dictionary<string, double> sweep_ranges_end { get; set; } = new();
		public Dictionary<string, double> sweep_ranges_step { get; set; } = new();
		public Dictionary<string, string> parameter_descriptions { get; set; } = new();
		public int connection_count { get; set; }
		public List<ConnectionSweepOutput> connections { get; set; } = new();
	}

	public class ConnectionSweepOutput
	{
		public int connection_id { get; set; }
		public string connection_name { get; set; } = "";
		public int iteration_count { get; set; }
		public List<IterationRecord> iterations { get; set; } = new();
	}

	public class IterationRecord
	{
		public int iteration { get; set; }
		/// <summary>Param key → value used in this iteration.</summary>
		public Dictionary<string, double> parameters { get; set; } = new();
		public bool overall_passed { get; set; }
		public double analysis_max_utilization { get; set; }
		public double max_bolt_utilization_iteration_scope { get; set; }
		public string critical_bolt_name_iteration_scope { get; set; } = "";
		public List<SummaryCheckItem> summary_checks_all { get; set; } = new();
		public List<PlateItem> plates_all { get; set; } = new();
		public List<WeldItem> welds_all { get; set; } = new();
		public List<BoltItem> bolts_all_iteration_scope { get; set; } = new();
		public List<LoadCaseRecord> load_cases { get; set; } = new();
	}

	public class LoadCaseRecord
	{
		public int? load_case_id { get; set; }
		public List<SummaryCheckItem> summary_checks { get; set; } = new();
		public double max_summary_utilization { get; set; }
		public string critical_summary_check_name { get; set; } = "";
		public List<PlateItem> plates { get; set; } = new();
		public double max_plate_utilization { get; set; }
		public string critical_plate_utilization_name { get; set; } = "";
		public double max_plate_stress { get; set; }
		public string max_stress_plate_name { get; set; } = "";
		public double max_plate_strain { get; set; }
		public string max_strain_plate_name { get; set; } = "";
		public List<WeldItem> welds { get; set; } = new();
		public double max_weld_utilization { get; set; }
		public string critical_weld_name { get; set; } = "";
		public int? critical_weld_id { get; set; }
		public List<BoltItem> bolt_checks_iteration_scope { get; set; } = new();
		public double max_bolt_utilization_iteration_scope { get; set; }
		public string critical_bolt_name_iteration_scope { get; set; } = "";
	}

	public class SummaryCheckItem
	{
		public string name { get; set; } = "";
		public double check_value { get; set; }
		public bool check_status { get; set; }
		public string unity_check_message { get; set; } = "";
		public int? load_case_id { get; set; }
		public bool skipped { get; set; }
	}

	public class PlateItem
	{
		public string name { get; set; } = "";
		public int? load_case_id { get; set; }
		public double unity_check { get; set; }
		public bool check_status { get; set; }
		public double max_stress { get; set; }
		public double max_strain { get; set; }
		public List<int>? items { get; set; }
	}

	public class WeldItem
	{
		public int? id { get; set; }
		public string name { get; set; } = "";
		public int? load_case_id { get; set; }
		public double unity_check { get; set; }
		public bool check_status { get; set; }
		public List<int>? items { get; set; }
	}

	public class BoltItem
	{
		public string name { get; set; } = "";
		public double unity_check { get; set; }
		public bool check_status { get; set; }
	}

	// ───────── Flattened rows for tables/charts ─────────

	/// <summary>One row per (connection, iteration, load case) — same shape as Python's loadcase_rows.</summary>
	public class LoadCaseRow
	{
		public int ConnectionId { get; set; }
		public string ConnectionName { get; set; } = "";
		public int Iteration { get; set; }
		public Dictionary<string, double> Parameters { get; set; } = new();
		public bool OverallPassed { get; set; }
		public int? LoadCaseId { get; set; }

		public double MaxSummaryUtilization { get; set; }
		public string CriticalSummaryCheckName { get; set; } = "";

		public double MaxPlateUtilization { get; set; }
		public string CriticalPlateUtilizationName { get; set; } = "";
		public double MaxPlateStress { get; set; }
		public string MaxStressPlateName { get; set; } = "";
		public double MaxPlateStrain { get; set; }
		public string MaxStrainPlateName { get; set; } = "";

		public double MaxWeldUtilization { get; set; }
		public string CriticalWeldName { get; set; } = "";
		public int? CriticalWeldId { get; set; }

		public double MaxBoltUtilizationIterationScope { get; set; }
		public string CriticalBoltNameIterationScope { get; set; } = "";

		/// <summary>Short "W=0.100, D=0.150, Rot=15°" style label for chart tooltips/x-axis.</summary>
		public string ParameterLabel => string.Join(", ",
			Parameters.Select(kv => $"{kv.Key}={kv.Value:G4}"));
	}

}
