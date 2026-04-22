using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using SweepChecker.Models;
using System.Globalization;
using System.IO;

namespace SweepChecker.Services
{
	/// <summary>
	/// Drives the parameter sweep over every combination of the "Sweep=true" parameters
	/// for each connection in the active project. Mirrors the Python Simplified_03 script.
	/// </summary>
	public class SweepRunner
	{
		private readonly IConnectionApiClient _client;
		private readonly Action<string> _log;

		public SweepRunner(IConnectionApiClient client, Action<string> log)
		{
			_client = client;
			_log = log;
		}

		public async Task<SweepOutput> RunAsync(
			string projectFilePath,
			IReadOnlyList<SweepParameter> parameters,
			CancellationToken ct = default)
		{
			if (!System.IO.File.Exists(projectFilePath))
				throw new FileNotFoundException($"Project file not found: {projectFilePath}");

			_log($"Opening project {projectFilePath}");
			var project = await _client.Project.OpenProjectAsync(projectFilePath, ct);
			var projectId = project.ProjectId;

			var output = new SweepOutput
			{
				project_file = projectFilePath,
				project_id = projectId.ToString(),
				connection_count = project.Connections?.Count ?? 0,
			};

			foreach (var p in parameters.Where(p => p.Sweep))
			{
				output.sweep_ranges_start[p.Key] = p.Start;
				output.sweep_ranges_end[p.Key] = p.End;
				output.sweep_ranges_step[p.Key] = p.Step;
				output.parameter_descriptions[p.Key] = p.Description;
			}

			try
			{
				var connections = project.Connections ?? new();
				if (connections.Count == 0) throw new Exception("No connections found in the project.");
				_log($"Found {connections.Count} connection(s).");

				int globalIter = 0;
				foreach (var conn in connections)
				{
					ct.ThrowIfCancellationRequested();
					_log($"── Connection {conn.Name} (id={conn.Id}) ──");

					var connOut = new ConnectionSweepOutput
					{
						connection_id = conn.Id,
						connection_name = conn.Name ?? "",
					};

					// First push every non-swept parameter as a single-value update
					var fixedUpdates = parameters
						.Where(p => !p.Sweep && !string.IsNullOrWhiteSpace(p.SingleValue))
						.Select(p => new IdeaParameterUpdate { Key = p.Key, Expression = p.SingleValue })
						.ToList();

					if (fixedUpdates.Count > 0)
					{
						var resp = await _client.Parameter.UpdateAsync(projectId, conn.Id, fixedUpdates, cancellationToken: ct);
						LogFailed(resp, "fixed");
					}

					// Iterate the Cartesian product of swept parameter values
					var sweepParams = parameters.Where(p => p.Sweep).ToList();
					var combos = CartesianProduct(sweepParams.Select(p => p.EnumerateValues().ToList()).ToList());

					foreach (var combo in combos)
					{
						ct.ThrowIfCancellationRequested();
						globalIter++;

						var updates = new List<IdeaParameterUpdate>(sweepParams.Count);
						var paramMap = new Dictionary<string, double>(sweepParams.Count);
						for (int i = 0; i < sweepParams.Count; i++)
						{
							var key = sweepParams[i].Key;
							var val = combo[i];
							paramMap[key] = val;
							updates.Add(new IdeaParameterUpdate
							{
								Key = key,
								Expression = val.ToString("R", CultureInfo.InvariantCulture)
							});
						}

						var desc = string.Join(", ", paramMap.Select(kv => $"{kv.Key}={kv.Value:G4}"));
						_log($"  #{globalIter}  {desc}");

						var upResp = await _client.Parameter.UpdateAsync(projectId, conn.Id, updates, cancellationToken: ct);
						LogFailed(upResp, "sweep");

						var calcResults = await _client.Calculation.CalculateAsync(projectId, new List<int> { conn.Id }, cancellationToken: ct);
						if (calcResults == null || calcResults.Count == 0)
						{
							_log("    WARNING: No summary results returned.");
							continue;
						}

						var detailed = await _client.Calculation.GetResultsAsync(projectId, new List<int> { conn.Id }, cancellationToken: ct);
						if (detailed == null || detailed.Count == 0)
						{
							_log("    WARNING: No detailed results returned.");
							continue;
						}

						var record = BuildIterationRecord(globalIter, paramMap, calcResults[0], detailed[0]);
						connOut.iterations.Add(record);
						_log($"    passed={record.overall_passed}  maxUtil={record.analysis_max_utilization:F3}  LCs={record.load_cases.Count}");
					}

					connOut.iteration_count = connOut.iterations.Count;
					output.connections.Add(connOut);
				}

				return output;
			}
			finally
			{
				try { await _client.Project.CloseProjectAsync(projectId, cancellationToken: ct); }
				catch (Exception ex) { _log($"(close-project failed: {ex.Message})"); }
			}
		}

		private void LogFailed(ParameterUpdateResponse? resp, string tag)
		{
			if (resp?.FailedValidations == null) return;
			foreach (var f in resp.FailedValidations)
				_log($"    validation {tag}: {f.Key} — {f.Message}");
		}

		private static IterationRecord BuildIterationRecord(
			int iterIdx,
			Dictionary<string, double> parameters,
			ConResultSummary summary,
			ConnectionCheckRes detailed)
		{
			var record = new IterationRecord
			{
				iteration = iterIdx,
				parameters = parameters,
				overall_passed = summary.Passed,
			};

			// Summary checks
			foreach (var s in summary.ResultSummary ?? new())
			{
				if (s.Skipped) continue;
				record.summary_checks_all.Add(new SummaryCheckItem
				{
					name = s.Name ?? "(unnamed)",
					check_value = s.CheckValue,
					check_status = s.CheckStatus,
					unity_check_message = s.UnityCheckMessage ?? "",
					load_case_id = s.LoadCaseId == 0 ? (int?)null : s.LoadCaseId,
					skipped = s.Skipped,
				});
			}
			record.analysis_max_utilization = record.summary_checks_all.Count == 0
				? 0.0
				: record.summary_checks_all.Max(s => s.check_value);

			// Plates, welds, bolts from detailed result
			foreach (var p in detailed.CheckResPlate ?? new())
			{
				record.plates_all.Add(new PlateItem
				{
					name = p.Name ?? "(unnamed)",
					load_case_id = p.LoadCaseId == 0 ? (int?)null : p.LoadCaseId,
					unity_check = p.MaxStress, // plates don't expose a unity check separately; stress drives it
					check_status = p.CheckStatus,
					max_stress = p.MaxStress,
					max_strain = p.MaxStrain,
					items = p.Items,
				});
			}
			foreach (var w in detailed.CheckResWeld ?? new())
			{
				record.welds_all.Add(new WeldItem
				{
					id = w.Id == 0 ? (int?)null : w.Id,
					name = w.Name ?? "(unnamed)",
					load_case_id = w.LoadCaseId == 0 ? (int?)null : w.LoadCaseId,
					unity_check = w.UnityCheck,
					check_status = w.CheckStatus,
					items = w.Items,
				});
			}
			foreach (var b in detailed.CheckResBolt ?? new())
			{
				record.bolts_all_iteration_scope.Add(new BoltItem
				{
					name = b.Name ?? "(unnamed)",
					unity_check = b.UnityCheck,
					check_status = b.CheckStatus,
				});
			}

			var boltMax = record.bolts_all_iteration_scope.OrderByDescending(b => b.unity_check).FirstOrDefault();
			record.max_bolt_utilization_iteration_scope = boltMax?.unity_check ?? 0.0;
			record.critical_bolt_name_iteration_scope = boltMax?.name ?? "";

			// Group everything by load_case_id so the postprocessor can slice per-LC
			record.load_cases = BuildLoadCaseBlocks(record);

			return record;
		}

		private static List<LoadCaseRecord> BuildLoadCaseBlocks(IterationRecord rec)
		{
			var keys = new HashSet<int?>();
			foreach (var s in rec.summary_checks_all) keys.Add(s.load_case_id);
			foreach (var p in rec.plates_all) keys.Add(p.load_case_id);
			foreach (var w in rec.welds_all) keys.Add(w.load_case_id);

			var blocks = new List<LoadCaseRecord>();
			foreach (var k in keys.OrderBy(k => k ?? int.MaxValue))
			{
				var lc = new LoadCaseRecord { load_case_id = k };

				foreach (var s in rec.summary_checks_all.Where(s => s.load_case_id == k))
				{
					lc.summary_checks.Add(s);
					if (s.check_value > lc.max_summary_utilization)
					{
						lc.max_summary_utilization = s.check_value;
						lc.critical_summary_check_name = s.name;
					}
				}

				foreach (var p in rec.plates_all.Where(p => p.load_case_id == k))
				{
					lc.plates.Add(p);
					if (p.unity_check > lc.max_plate_utilization)
					{
						lc.max_plate_utilization = p.unity_check;
						lc.critical_plate_utilization_name = p.name;
					}
					if (p.max_stress > lc.max_plate_stress)
					{
						lc.max_plate_stress = p.max_stress;
						lc.max_stress_plate_name = p.name;
					}
					if (p.max_strain > lc.max_plate_strain)
					{
						lc.max_plate_strain = p.max_strain;
						lc.max_strain_plate_name = p.name;
					}
				}

				foreach (var w in rec.welds_all.Where(w => w.load_case_id == k))
				{
					lc.welds.Add(w);
					if (w.unity_check > lc.max_weld_utilization)
					{
						lc.max_weld_utilization = w.unity_check;
						lc.critical_weld_name = w.name;
						lc.critical_weld_id = w.id;
					}
				}

				// bolts don't carry load_case_id in the API response, so attach iteration-scope copy
				lc.bolt_checks_iteration_scope = rec.bolts_all_iteration_scope;
				lc.max_bolt_utilization_iteration_scope = rec.max_bolt_utilization_iteration_scope;
				lc.critical_bolt_name_iteration_scope = rec.critical_bolt_name_iteration_scope;

				blocks.Add(lc);
			}
			return blocks;
		}

		private static IEnumerable<IReadOnlyList<double>> CartesianProduct(List<List<double>> lists)
		{
			if (lists.Count == 0) { yield return Array.Empty<double>(); yield break; }
			var idx = new int[lists.Count];
			while (true)
			{
				var combo = new double[lists.Count];
				for (int i = 0; i < lists.Count; i++) combo[i] = lists[i][idx[i]];
				yield return combo;

				int k = lists.Count - 1;
				while (k >= 0)
				{
					idx[k]++;
					if (idx[k] < lists[k].Count) break;
					idx[k] = 0;
					k--;
				}
				if (k < 0) yield break;
			}
		}
	}
}
