using NorsokChecker.Models;

namespace NorsokChecker.Services.Cbfem_Mothballed
{
	/// <summary>
	/// The CBFEM plate / weld / bolt checks — MOTHBALLED. Nothing calls this; see README.md in this
	/// folder for why the chapter was shelved and what it would take to bring it back.
	///
	/// Lifted verbatim out of NorsokCheckRunner when the chapter was removed, so the norm reading it
	/// encodes survives: NORSOK Table 6-1 factors rather than the EC3 defaults (γ_M0 = 1.15 on
	/// plates, γ_M2 = 1.30 on welds), and the distinction between a check that failed and one that
	/// could not be made — a plate the engine reported no stress for is NOT a plate at 0 %.
	/// </summary>
	internal static class CbfemChecks
	{
		/// <summary>
		/// All three groups. The caller supplies the factors, which
		/// <see cref="ProjectSettingsService"/> also writes into the project before the calculation —
		/// the engine's own resistances then already reflect them.
		/// </summary>
		internal static void EvaluateAll(ParsedRawResults parsed, double gammaM0, double gammaM2,
			List<NorsokFormulaResult> results)
		{
			EvaluatePlateChecks(parsed, gammaM0, results);
			EvaluateWeldChecks(parsed, gammaM2, results);
			EvaluateBoltChecks(parsed, results);
		}

		private static void EvaluatePlateChecks(ParsedRawResults parsed, double gammaM,
			List<NorsokFormulaResult> results)
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

		private static void EvaluateWeldChecks(ParsedRawResults parsed, double gammaM2,
			List<NorsokFormulaResult> results)
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

		private static void EvaluateBoltChecks(ParsedRawResults parsed, List<NorsokFormulaResult> results)
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
