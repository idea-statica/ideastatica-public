using SweepChecker.Models;

namespace SweepChecker.Services
{
	/// <summary>In-memory flattening of sweep results into rows suited for the charts / data grid.</summary>
	public static class SweepIO
	{
		/// <summary>One row per (connection, iteration, load case) — drives the per-LC charts.</summary>
		public static List<LoadCaseRow> Flatten(SweepOutput data)
		{
			var rows = new List<LoadCaseRow>();
			foreach (var conn in data.connections)
			{
				foreach (var it in conn.iterations)
				{
					foreach (var lc in it.load_cases)
					{
						rows.Add(new LoadCaseRow
						{
							ConnectionId = conn.connection_id,
							ConnectionName = conn.connection_name,
							Iteration = it.iteration,
							Parameters = it.parameters,
							OverallPassed = it.overall_passed,
							LoadCaseId = lc.load_case_id,
							MaxSummaryUtilization = lc.max_summary_utilization,
							CriticalSummaryCheckName = lc.critical_summary_check_name,
							MaxPlateUtilization = lc.max_plate_utilization,
							CriticalPlateUtilizationName = lc.critical_plate_utilization_name,
							MaxPlateStress = lc.max_plate_stress,
							MaxStressPlateName = lc.max_stress_plate_name,
							MaxPlateStrain = lc.max_plate_strain,
							MaxStrainPlateName = lc.max_strain_plate_name,
							MaxWeldUtilization = lc.max_weld_utilization,
							CriticalWeldName = lc.critical_weld_name,
							CriticalWeldId = lc.critical_weld_id,
							MaxBoltUtilizationIterationScope = lc.max_bolt_utilization_iteration_scope,
							CriticalBoltNameIterationScope = lc.critical_bolt_name_iteration_scope,
						});
					}
				}
			}
			return rows;
		}
	}
}
