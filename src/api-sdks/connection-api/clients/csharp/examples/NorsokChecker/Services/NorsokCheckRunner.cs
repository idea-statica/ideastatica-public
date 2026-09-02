using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;
using NorsokChecker.Services.Norsok64;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Runs the §6.4 tubular-joint check over a connection's topology and turns the engine's rows
	/// into report results.
	///
	/// It needs no calculation: the topology, the joint plane, θ, the gaps and the per-brace K/Y/X
	/// balance all come from the model and the load effects. The CBFEM plate/weld/bolt group that
	/// used to sit beside it — the one thing here that DID need a calculation — is mothballed in
	/// Services/Cbfem_Mothballed/, and §6.3 in Services/Formulas63_Mothballed/.
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
		/// §6.4 via AUTO-TOPOLOGY (port of the python reference pipeline): chord/brace identification,
		/// joint-plane fit, per-brace force resolution, chord-stress averaging (Begin/End), K/Y/X
		/// force-balance classification, weighted §6.4 check per brace. One report card per brace,
		/// enveloped over all load effects. Returns false when the topology gate rejects the joint
		/// (verdict ERROR) — the caller may then fall back to the manual joint parameters.
		/// </summary>
		/// <param name="topology">
		/// The finished topology, handed back whatever the verdict. <paramref name="results"/> carries
		/// only the ENVELOPE — one row per brace, its governing load effect — because that is what a
		/// summary needs. The §6.4 tab also has to show any single load effect, the K/Y/X split per
		/// state and the derivation behind each number, and all of that lives here and was previously
		/// discarded when this method returned.
		/// </param>
		public bool EvaluateJointChecksFromTopology(
			IReadOnlyList<JointMemberData> members,
			List<ConLoadEffect>? loadEffects,
			List<NorsokFormulaResult> results,
			double kyxGate = 0.0,
			Action<JointTopology>? topology = null)
		{
			var topo = new JointTopologyBuilder(kyxGate: kyxGate, log: _log).Build(members, loadEffects);
			topology?.Invoke(topo);

			_log($"    §6.4 topology: chord={topo.Chord?.Name}, braces={topo.GapBraces.Count}, " +
				 $"plane fit: {topo.PlaneFitBasis}, verdict={topo.Verdict.Status}");
			foreach (var e in topo.Verdict.Errors) _log($"      [E] {e}");
			foreach (var w in topo.Verdict.Warnings) _log($"      [W] {w}");

			// Warnings are published whatever the verdict: they say the check ran with clamped
			// parameters (§6.4.3.1) or on an assumption, and that belongs in the results, not only
			// in the log where it used to stay.
			PublishTopologyNotes(topo, results);

			if (topo.Verdict.Status == "ERROR" || topo.JointChecks.Count == 0)
			{
				// A joint outside the scope of §6.4 has NOT passed — it has not been assessed, and
				// an empty result set used to read as "everything passed" in the caller. One row per
				// failed condition, as the python reference's UI lists them: joining them into a
				// single string made a joint that failed six gates look like it failed one.
				var reasons = topo.Verdict.Errors.Count > 0
					? topo.Verdict.Errors
					: new List<string> { "the joint produced no §6.4 check" };

				for (int i = 0; i < reasons.Count; i++)
				{
					results.Add(new NorsokFormulaResult
					{
						Section = "6.4",
						// EMPTY: no equation was evaluated on a joint outside the chapter's scope, and
						// "6.4.3" is a CLAUSE — printed as "(Eq. 6.4.3)" it names an equation the norm
						// does not have.
						Equation = "",
						Title = reasons.Count > 1
							? $"Outside the scope of §6.4 ({i + 1} of {reasons.Count})"
							: "Outside the scope of §6.4",
						CheckExpression = reasons[i],
						Formula = "-",
						FormulaSubstituted = "no §6.4 check was performed for this joint",
						Demand = 0,
						Capacity = 0,
						Utilization = 0,
						// not a failure: nothing was checked. Reporting this as FAIL alongside the
						// words "NOT ASSESSED" said both at once, which cannot be true.
						NotAssessed = true,
						// The topology rejected the joint on a property of its geometry, so §6.4 does
						// not cover it — a different statement from "the inputs would not read".
						Reason = NotAssessedReason.OutsideScope,
					});
				}
				return false;
			}

			// per-LE classification summary
			foreach (var le in topo.Classification)
				foreach (var c in le.Rows)
					_log($"      LE{le.Id} {c.Name}: K={c.FrK:P0} X={c.FrX:P0} Y={c.FrY:P0} " +
						 $"(q={c.QTrans / 1e3:F1} kN){(string.IsNullOrEmpty(c.Note) ? "" : " — " + c.Note)}");

			// envelope: the governing load effect per brace — see JointEnvelope for the rule
			foreach (var brace in topo.GapBraces)
			{
				var gov = JointEnvelope.Pick(topo.JointChecks, brace.Name);
				if (gov == null || gov.Row.Skipped)
				{
					// A brace nothing could be checked on is NOT absent from the joint — publishing
					// nothing let the connection read PASS while a brace went unassessed, and the
					// §6.4 tab showed two rows for a three-brace joint. One row per brace, always.
					var reason = gov?.Row.Reason
						?? JointEnvelope.SkipReason(topo.JointChecks, brace.Name)
						?? "no data";
					_log($"    §6.4 {brace.Name}: not assessed ({reason})");
					results.Add(new NorsokFormulaResult
					{
						Section = "6.4.3.6",
						Equation = "6.57",
						Title = $"Tubular Joint — {brace.Name}",
						CheckExpression = reason,
						Formula = "-",
						FormulaSubstituted = $"{brace.Name} could not be checked: {reason}",
						NotAssessed = true,
						// This brace alone lacked the data for its check while the joint itself was
						// assessed — the model is what to look at, not the chapter's scope.
						Reason = NotAssessedReason.NotEvaluated,
					});
					continue;
				}

				// carry the governing state onto the row so the results table and the report can
				// point at it — the id is what a detail view would resolve by, the name is display
				var worst = gov.Row;
				worst.GovLeId = gov.LeId;
				worst.GovLeName = gov.LeName;

				var card = Joint64ReportAdapter.BuildResultFromRow(worst, gov.LeName);
				results.Add(card);
				_log($"    §6.4.3.6 {brace.Name}: util={(double.IsInfinity(worst.Util) ? 999 : worst.Util) * 100:F1}% " +
					 $"[{gov.LeName}] {(worst.Passed ? "PASS" : "FAIL")}");
			}
			return true;
		}

		/// <summary>
		/// Publish the topology's warnings as results, one per warning.
		///
		/// They used to go to the log only, so the engineer never saw that a joint had been checked
		/// with β, γ or θ clamped to the §6.4.3.1 range, or that an assumption had been made about
		/// its geometry. The python reference lists them next to the errors, in a different colour,
		/// which is what the third state (NotAssessed) now allows here too — a warning is neither a
		/// pass nor a failure, and the joint may well be checked despite it.
		/// </summary>
		private static void PublishTopologyNotes(JointTopology topo, List<NorsokFormulaResult> results)
		{
			var warns = topo.Verdict.Warnings;
			for (int i = 0; i < warns.Count; i++)
			{
				results.Add(new NorsokFormulaResult
				{
					Section = "6.4.3.1",
					// EMPTY, not "-": a note evaluates no equation. The literal dash rendered as
					// "(Eq. -)" on every note row.
					Equation = "",
					Title = warns.Count > 1
						? $"Assumption ({i + 1} of {warns.Count})"
						: "Assumption",
					CheckExpression = warns[i],
					Formula = "-",
					FormulaSubstituted = "the check proceeds; the note above qualifies its result",
					Demand = 0,
					Capacity = 0,
					Utilization = 0,
					// a note, not an unassessed check: the joint is still checked, so this must not
					// make the connection read as "partly assessed"
					IsNote = true,
					NotAssessed = true,
				});
			}
		}

	}
}
