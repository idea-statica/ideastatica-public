using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;
using NorsokChecker.Services.Norsok64;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Orchestrates NORSOK N-004 compliance checking.
	///
	/// Data sources:
	/// 1. Raw JSON (CheckResultsData) → plate stresses, weld utilization, bolt forces
	/// 2. LoadEffect API → member internal forces (N, Vy, Vz, Mx, My, Mz)
	/// 3. User-provided tubular geometry (D, t, L, k) for §6.3 member formulas
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
						Equation = "6.4.3",
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
					Equation = "-",
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

		/// <summary>
		/// Evaluate the Norsok formulas: the CBFEM plate/weld/bolt group and §6.4 tubular joints.
		/// §6.3 (tubular members) is mothballed — see CHAPTER_63_FINDINGS.md.
		/// </summary>
		public List<NorsokFormulaResult> EvaluateNorsokFormulas(
			int connectionId,
			string? rawJsonResults,
			List<ConLoadEffect>? loadEffects = null,
			List<MemberDisplayInfo>? members = null,
			bool includeCbfemChecks = true)
		{
			var results = new List<NorsokFormulaResult>();

			// Raw CBFEM results are only needed by the plate/weld/bolt group. §6.4 works from
			// load effects and geometry alone, so without a calculation there is nothing to parse.
			ParsedRawResults? parsed = null;
			if (rawJsonResults != null)
			{
				try
				{
					parsed = RawResultsParser.Parse(rawJsonResults);
				}
				catch (Exception ex)
				{
					_log($"    ERROR parsing raw results: {ex.Message}");
					results.Add(new NorsokFormulaResult
					{
						Section = "CBFEM", Equation = "-", Title = "Parse Error",
						CheckExpression = ex.Message, Passed = false
					});
					return results;
				}

				_log($"    Parsed: {parsed.Plates.Count} plates, {parsed.Welds.Count} welds, {parsed.Bolts.Count} bolts");
			}

			double gammaM0 = ProjectSettingsService.GammaM0_Norsok;  // 1.15
			double gammaM2 = ProjectSettingsService.GammaM2_Norsok;  // 1.30
			// γBC = 1.05 (§6.1) is NOT applied, and not "implicitly" either, as this comment used to
			// say. The norm asks for it only "where OTHER material factors are used than given in
			// Table 6-1"; these two ARE Table 6-1, written into the project settings before the run,
			// so multiplying by γBC as well would double-count — 1.15 × 1.05 = 1.21 against the 1.15
			// the norm asks for. See ProjectSettingsService.GammaBC_NotApplied.

			// ─── CBFEM PLATE / WELD / BOLT CHECKS ───
			if (includeCbfemChecks && parsed != null)
			{
				EvaluatePlateChecks(parsed, gammaM0, results);
				// Norsok Table 6-1: γM2 = 1.30 for welds (not EC3 default 1.25)
				EvaluateWeldChecks(parsed, gammaM2, results);
				EvaluateBoltChecks(parsed, results);
			}
			else
			{
				_log("    CBFEM plate/weld/bolt checks disabled (chapter toggle)");
			}

			// ─── TUBULAR MEMBER FORMULAS (§6.3) — MOTHBALLED ───
			// EvaluateTubularMemberFormulas and everything under Services/Formulas/ still compiles
			// but is no longer called. Two inputs Eq 6.27 needs cannot be derived from a connection
			// model or expressed by this app: k per plane (the restraint is not symmetric even for
			// a round tube) and the far-end moments M1 for C_m case (b). See CHAPTER_63_FINDINGS.md
			// for the measurements and for what re-enabling would require.

			// ─── TUBULAR JOINT CHECKS (§6.4) ───
			// Run by EvaluateJointChecksFromTopology, which the caller invokes separately: the
			// topology derives the chord, the joint plane, θ, the gaps and the per-brace K/Y/X
			// balance from the model. The manual alternative that used to live here — one joint
			// type, one θ and one gap for the whole joint — was both unreachable and at odds with
			// §6.4, where those quantities differ per brace.

			return results;
		}

		private void EvaluatePlateChecks(ParsedRawResults parsed, double gammaM, List<NorsokFormulaResult> results)
		{
			foreach (var plate in parsed.Plates)
			{
				if (plate.MaterialFy <= 0) continue;

				double f_y = plate.MaterialFy;
				double f_d = f_y / gammaM;
				double utilization = plate.MaxStress / f_d;

				results.Add(new NorsokFormulaResult
				{
					// "Plate", not "6.3.2": this is a CBFEM plate von-Mises check, not a §6.3
					// tubular member check. Sharing the key put it under a §6.3 report heading,
					// and would have merged it with the §6.3.2 axial-tension results if §6.3 were
					// ever re-enabled. Same shape as the Weld / Bolt groups.
					Section = "Plate",
					Equation = "6.1",
					Title = $"Plate: {plate.Name}",
					CheckExpression = "σ_Ed ≤ f_yd = f_y / γ_M",
					Formula = "f_yd = f_y / γ_M",
					FormulaSubstituted = $"f_yd = {f_y:F1} / {gammaM:F2} = {f_d:F1} MPa",
					Demand = plate.MaxStress,
					Capacity = f_d,
					Utilization = utilization,
					// A plate the engine reported no stress for is NOT a plate at 0 %: zero is the
					// most favourable stress there is, so this used to report PASS at 0.0 %.
					Passed = plate.HasStress && utilization <= 1.0,
					NotAssessed = !plate.HasStress,
					LoadCaseId = plate.LoadCaseId,
					Variables = new List<FormulaVariable>
					{
						new() { Symbol = "σ_vM", Description = $"Von Mises stress ({plate.Name})", Value = plate.MaxStress, Unit = "MPa" },
						new() { Symbol = "f_y", Description = $"Yield strength ({plate.MaterialName})", Value = f_y, Unit = "MPa" },
						new() { Symbol = "γ_M", Description = "Norsok material factor γM0 (Table 6-1)", Value = gammaM, Unit = "-" },
						new() { Symbol = "f_d", Description = "Design strength = f_y/γ_M", Value = f_d, Unit = "MPa" },
						new() { Symbol = "t", Description = "Plate thickness", Value = plate.Thickness, Unit = "mm" },
						new() { Symbol = "ε_max", Description = "Max strain", Value = plate.MaxStrain, Unit = "-" },
					}
				});
			}
		}

		private void EvaluateWeldChecks(ParsedRawResults parsed, double gammaM2, List<NorsokFormulaResult> results)
		{
			foreach (var weld in parsed.Welds)
			{
				// Prefer the engine-computed resistance — raw results do not serialize
				// f_u/β_w, and the engine value already reflects the Norsok γM2 pushed
				// into the project settings before calculation.
				double fu = weld.MaterialFu;
				double betaW = weld.BetaW > 0 ? weld.BetaW : 0.85;
				double resistance = weld.EquivalentStressResistance;
				string resistanceFormula;
				if (resistance > 0)
				{
					resistanceFormula = $"f_w,Rd = {resistance:F1} MPa (engine value, Norsok γM2 = {gammaM2:F2} applied via project settings)";
				}
				else if (fu > 0)
				{
					resistance = fu / (betaW * gammaM2);
					resistanceFormula = $"f_w,Rd = {fu:F1} / ({betaW:F2} × {gammaM2:F2}) = {resistance:F1} MPa";
				}
				else
				{
					resistanceFormula = "f_w,Rd unavailable — raw results contain neither equivalentStressResistance nor f_u";
				}

				// no resistance to check against, OR no stress reported for it — either way there is
				// nothing to assess, and a missing stress is not a weld at 0 %
				bool noData = resistance <= 0 || !weld.HasStress;
				double utilization = noData ? 0 : weld.MaxEquivalentStress / resistance;

				results.Add(new NorsokFormulaResult
				{
					Section = "Weld",
					Equation = "EN 1993-1-8 §4.5",
					Title = $"Weld: {(string.IsNullOrEmpty(weld.Name) ? $"#{weld.Id}" : weld.Name)}{(noData ? " — NOT ASSESSED" : "")}",
					CheckExpression = noData
						? (resistance <= 0
							? "weld resistance not available in the raw results — nothing to check against"
							: "the engine reported no stress for this weld")
						: "σ_w ≤ f_w,Rd",
					Formula = "f_w,Rd = f_u / (β_w · γ_M2)",
					FormulaSubstituted = resistanceFormula,
					Demand = weld.MaxEquivalentStress,
					Capacity = resistance,
					Utilization = utilization,
					// NOT a failure. It used to say "check cannot be verified, marked FAIL" — which
					// asserts two things that cannot both hold, the same contradiction the §6.4 rows
					// had before the third state existed. Nothing was checked here.
					Passed = !noData && utilization <= 1.0,
					NotAssessed = noData,
					LoadCaseId = weld.LoadCaseId,
					Variables = new List<FormulaVariable>
					{
						new() { Symbol = "σ_w", Description = "Max equivalent weld stress", Value = weld.MaxEquivalentStress, Unit = "MPa" },
						new() { Symbol = "σ_⊥", Description = "Perpendicular stress", Value = weld.SigmaPerpendicular, Unit = "MPa" },
						new() { Symbol = "τ_⊥", Description = "Shear perpendicular", Value = weld.Tauy, Unit = "MPa" },
						new() { Symbol = "τ_∥", Description = "Shear parallel", Value = weld.Taux, Unit = "MPa" },
						new() { Symbol = "f_u", Description = "Ultimate tensile strength", Value = fu, Unit = "MPa" },
						new() { Symbol = "β_w", Description = "Correlation factor", Value = betaW, Unit = "-" },
						new() { Symbol = "γ_M2", Description = "Norsok material factor γM2 = 1.30 (Table 6-1: welds, bolts)", Value = gammaM2, Unit = "-" },
						new() { Symbol = "Resistance", Description = "f_u/(β_w·γ_M2)", Value = resistance, Unit = "MPa" },
						new() { Symbol = "a", Description = "Weld throat thickness", Value = weld.DesignedThickness, Unit = "mm" },
						new() { Symbol = "L", Description = "Weld length", Value = weld.Length, Unit = "mm" },
					}
				});
			}
		}

		private void EvaluateBoltChecks(ParsedRawResults parsed, List<NorsokFormulaResult> results)
		{
			foreach (var bolt in parsed.Bolts)
			{
				double interactionCheck = bolt.InteractionTensionShear;

				double tensionTerm = bolt.BoltTensionResistance > 0 ? bolt.BoltTensionForce / bolt.BoltTensionResistance : 0;
				double shearTerm = bolt.BoltShearResistance > 0 ? bolt.BoltShearForce / (1.4 * bolt.BoltShearResistance) : 0;

				results.Add(new NorsokFormulaResult
				{
					Section = "Bolt",
					Equation = "EN 1993-1-8 §3.6",
					Title = $"Bolt: {bolt.Name}",
					CheckExpression = "F_t,Sd/F_t,Rd + F_v,Sd/(1.4·F_v,Rd) ≤ 1.0",
					Formula = "Interaction = F_t,Sd/F_t,Rd + F_v,Sd/(1.4·F_v,Rd)",
					FormulaSubstituted = $"= {bolt.BoltTensionForce:F1}/{bolt.BoltTensionResistance:F1} + {bolt.BoltShearForce:F1}/(1.4×{bolt.BoltShearResistance:F1}) = {tensionTerm:F4} + {shearTerm:F4} = {interactionCheck:F4}",
					Demand = interactionCheck,
					Capacity = 1.0,
					Utilization = interactionCheck,
					Passed = interactionCheck <= 1.0,
					LoadCaseId = bolt.LoadCaseId,
					Variables = new List<FormulaVariable>
					{
						new() { Symbol = "F_t,Sd", Description = "Bolt tension force", Value = bolt.BoltTensionForce, Unit = "kN" },
						new() { Symbol = "F_v,Sd", Description = "Bolt shear force", Value = bolt.BoltShearForce, Unit = "kN" },
						new() { Symbol = "F_t,Rd", Description = "Tension resistance", Value = bolt.BoltTensionResistance, Unit = "kN" },
						new() { Symbol = "F_v,Rd", Description = "Shear resistance", Value = bolt.BoltShearResistance, Unit = "kN" },
						new() { Symbol = "UC_tension", Description = "Tension utilization", Value = bolt.UnityCheckTension, Unit = "-" },
						new() { Symbol = "UC_shear", Description = "Shear utilization", Value = bolt.UnityCheckShear, Unit = "-" },
						new() { Symbol = "Interaction", Description = "Combined check", Value = interactionCheck, Unit = "-" },
						new() { Symbol = "Assembly", Description = $"Bolt assembly: {bolt.BoltAssemblyName}", Value = 0, Unit = bolt.BoltAssemblyName },
					}
				});
			}
		}

	}
}
