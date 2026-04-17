using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Orchestrates NORSOK N-004 compliance checking.
	/// Parses raw CBFEM results and evaluates Norsok §6.3 formulas.
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
		/// Evaluate all applicable Norsok §6.3 formulas against raw CBFEM results.
		/// Returns a list of NorsokFormulaResult, one per formula evaluated.
		/// </summary>
		public List<NorsokFormulaResult> EvaluateNorsokFormulas(int connectionId, string rawJsonResults)
		{
			var results = new List<NorsokFormulaResult>();

			// TODO: Parse rawJsonResults to extract:
			// - Member internal forces (N_Sd, M_y,Sd, M_z,Sd, V_Sd, M_T,Sd)
			// - Material properties (f_y, E)
			// - Cross-section geometry (D, t for tubular; or h, b, t_w, t_f for I-sections)
			// - Member length and boundary conditions (k, l)
			//
			// For now, return a placeholder result indicating raw results were received.
			// Each formula implementation will be added in subsequent commits.

			_log($"    Raw results length: {rawJsonResults.Length:N0} chars");

			// Placeholder — will be replaced by actual formula implementations
			results.Add(new NorsokFormulaResult
			{
				Section = "6.3",
				Equation = "-",
				Title = "Raw Results Received",
				CheckExpression = "Raw CBFEM data available for formula evaluation",
				Demand = 0,
				Capacity = 1,
				Utilization = 0,
				Passed = true,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "JSON size", Description = "Raw results payload size", Value = rawJsonResults.Length, Unit = "chars" }
				}
			});

			return results;
		}
	}
}
