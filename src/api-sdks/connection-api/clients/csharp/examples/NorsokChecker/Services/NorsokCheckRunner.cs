using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Runs IDEA StatiCa CBFEM calculations and evaluates results against Norsok M-001 criteria.
	/// </summary>
	public class NorsokCheckRunner
	{
		private readonly IConnectionApiClient _client;
		private readonly Guid _projectId;
		private readonly Action<string> _log;

		public NorsokCheckRunner(IConnectionApiClient client, Guid projectId, Action<string> log)
		{
			_client = client;
			_projectId = projectId;
			_log = log;
		}

		/// <summary>
		/// Calculate a single connection and evaluate Norsok compliance.
		/// </summary>
		public async Task<NorsokResult> CheckConnectionAsync(int connectionId, CancellationToken ct = default)
		{
			// Run CBFEM calculation
			var connectionIds = new List<int> { connectionId };
			var calcResults = await _client.Calculation.CalculateAsync(_projectId, connectionIds, cancellationToken: ct);

			if (calcResults == null || calcResults.Count == 0)
			{
				_log("  WARNING: No calculation results returned.");
				return new NorsokResult { ConnectionId = connectionId, Passed = false };
			}

			// Retrieve detailed results
			var detailed = await _client.Calculation.GetResultsAsync(_projectId, connectionIds, cancellationToken: ct);

			var norsokResult = new NorsokResult
			{
				ConnectionId = connectionId,
				OverallPassed = calcResults[0].Passed,
			};

			var summary = calcResults[0];

			// Process summary checks
			foreach (var s in summary.ResultSummary ?? new())
			{
				if (s.Skipped) continue;

				norsokResult.CheckItems.Add(new NorsokCheckItem
				{
					CheckName = s.Name ?? "(unnamed)",
					Category = "Summary",
					Utilization = s.CheckValue,
					Passed = s.CheckStatus,
					Details = s.UnityCheckMessage ?? ""
				});

				if (s.CheckValue > norsokResult.MaxUtilization)
					norsokResult.MaxUtilization = s.CheckValue;
			}

			// Process detailed plate/weld/bolt results if available
			if (detailed != null && detailed.Count > 0)
			{
				var det = detailed[0];

				foreach (var p in det.CheckResPlate ?? new())
				{
					norsokResult.CheckItems.Add(new NorsokCheckItem
					{
						CheckName = p.Name ?? "(plate)",
						Category = "Plates",
						Utilization = p.MaxStress,
						Passed = p.CheckStatus,
						Details = $"Stress={p.MaxStress:F1} MPa, Strain={p.MaxStrain:F4}"
					});
				}

				foreach (var w in det.CheckResWeld ?? new())
				{
					norsokResult.CheckItems.Add(new NorsokCheckItem
					{
						CheckName = w.Name ?? "(weld)",
						Category = "Welds",
						Utilization = w.UnityCheck,
						Passed = w.CheckStatus,
						Details = $"Unity={w.UnityCheck:F3}"
					});

					if (w.UnityCheck > norsokResult.MaxUtilization)
						norsokResult.MaxUtilization = w.UnityCheck;
				}

				foreach (var b in det.CheckResBolt ?? new())
				{
					norsokResult.CheckItems.Add(new NorsokCheckItem
					{
						CheckName = b.Name ?? "(bolt)",
						Category = "Bolts",
						Utilization = b.UnityCheck,
						Passed = b.CheckStatus,
						Details = $"Unity={b.UnityCheck:F3}"
					});

					if (b.UnityCheck > norsokResult.MaxUtilization)
						norsokResult.MaxUtilization = b.UnityCheck;
				}
			}

			norsokResult.Passed = norsokResult.OverallPassed && norsokResult.MaxUtilization <= 1.0;

			return norsokResult;
		}
	}
}
