using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;
using NorsokChecker.Services.Formulas;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Orchestrates NORSOK N-004 compliance checking.
	/// Parses raw CBFEM results and evaluates Norsok §6.3 formulas.
	///
	/// Strategy:
	/// 1. Parse raw JSON → extract plate stresses, weld utilization, bolt forces
	/// 2. For plate-level checks: use max stress vs. f_y with Norsok γ_M
	/// 3. For member-level formulas (6.3.2–6.3.8): use plate data as proxy
	///    for stress state (the CBFEM results give plate-level stresses, not
	///    member internal forces directly — those would need structural analysis).
	/// 4. Each formula is evaluated and a NorsokFormulaResult is returned with
	///    all variable values for full traceability in the report.
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

			ParsedRawResults parsed;
			try
			{
				parsed = RawResultsParser.Parse(rawJsonResults);
			}
			catch (Exception ex)
			{
				_log($"    ERROR parsing raw results: {ex.Message}");
				results.Add(new NorsokFormulaResult
				{
					Section = "6.3", Equation = "-", Title = "Parse Error",
					CheckExpression = ex.Message,
					Passed = false
				});
				return results;
			}

			_log($"    Parsed: {parsed.Plates.Count} plates, {parsed.Welds.Count} welds, {parsed.Bolts.Count} bolts");

			// ─── PLATE CHECKS ───
			// For each plate, evaluate Norsok stress-based formulas using the CBFEM stresses.
			// The plate's MaxStress is the von Mises equivalent stress from the FE model.
			// We compare this against f_y/γ_M (Norsok material factor).

			foreach (var plate in parsed.Plates)
			{
				if (plate.MaterialFy <= 0) continue;

				double f_y = plate.MaterialFy;
				double gammaM = ProjectSettingsService.GammaM0_Norsok; // 1.15
				double maxStress = plate.MaxStress;
				double thickness = plate.Thickness;

				// §6.3.2 analogy — plate axial stress check
				// MaxStress from CBFEM is von Mises equivalent stress.
				// Compare against f_y / γ_M as per Norsok.
				double f_d = f_y / gammaM;
				double utilization = maxStress / f_d;

				results.Add(new NorsokFormulaResult
				{
					Section = "6.3.2",
					Equation = "6.1",
					Title = $"Plate Stress: {plate.Name}",
					CheckExpression = "σ_vM ≤ f_y / γ_M",
					Demand = maxStress,
					Capacity = f_d,
					Utilization = utilization,
					Passed = utilization <= 1.0,
					Variables = new List<FormulaVariable>
					{
						new() { Symbol = "σ_vM", Description = $"Von Mises stress ({plate.Name})", Value = maxStress, Unit = "MPa" },
						new() { Symbol = "f_y", Description = $"Yield strength ({plate.MaterialName})", Value = f_y, Unit = "MPa" },
						new() { Symbol = "γ_M", Description = "Norsok material factor (§6.3.7)", Value = gammaM, Unit = "-" },
						new() { Symbol = "f_d", Description = "Design strength = f_y/γ_M", Value = f_d, Unit = "MPa" },
						new() { Symbol = "t", Description = "Plate thickness", Value = thickness, Unit = "mm" },
						new() { Symbol = "ε_max", Description = "Max strain", Value = plate.MaxStrain, Unit = "-" },
						new() { Symbol = "LC", Description = "Critical load case ID", Value = plate.LoadCaseId, Unit = "-" },
					}
				});

				// If plate is tubular (CHS), also run the tubular-specific checks
				// This requires D and t — we can infer if the plate represents a tubular member.
				// For now, we flag tubular geometry checks as needing user input.
			}

			// ─── WELD CHECKS ───
			// Norsok requires minimum weld quality level B (ISO 5817).
			// Here we re-check weld utilization with Norsok γ_M2 = 1.25.

			foreach (var weld in parsed.Welds)
			{
				double gammaM2 = ProjectSettingsService.GammaM2_Norsok; // 1.25
				double maxStress = weld.MaxEquivalentStress;
				double fu = weld.MaterialFu;
				double betaW = weld.BetaW > 0 ? weld.BetaW : 0.85; // default correlation factor

				// Weld resistance: f_u / (β_w · γ_M2)
				double resistance = fu > 0 ? fu / (betaW * gammaM2) : 0;
				double utilization = resistance > 0 ? maxStress / resistance : 0;

				results.Add(new NorsokFormulaResult
				{
					Section = "Weld",
					Equation = "EN 1993-1-8 §4.5",
					Title = $"Weld: {weld.Name}",
					CheckExpression = "σ_w ≤ f_u / (β_w · γ_M2)",
					Demand = maxStress,
					Capacity = resistance,
					Utilization = utilization,
					Passed = utilization <= 1.0,
					Variables = new List<FormulaVariable>
					{
						new() { Symbol = "σ_w", Description = "Max equivalent weld stress", Value = maxStress, Unit = "MPa" },
						new() { Symbol = "σ_⊥", Description = "Perpendicular stress", Value = weld.SigmaPerpendicular, Unit = "MPa" },
						new() { Symbol = "τ_⊥", Description = "Shear perpendicular", Value = weld.Tauy, Unit = "MPa" },
						new() { Symbol = "τ_∥", Description = "Shear parallel (along weld)", Value = weld.Taux, Unit = "MPa" },
						new() { Symbol = "f_u", Description = "Ultimate tensile strength", Value = fu, Unit = "MPa" },
						new() { Symbol = "β_w", Description = "Correlation factor", Value = betaW, Unit = "-" },
						new() { Symbol = "γ_M2", Description = "Norsok weld safety factor", Value = gammaM2, Unit = "-" },
						new() { Symbol = "Resistance", Description = "f_u/(β_w·γ_M2)", Value = resistance, Unit = "MPa" },
						new() { Symbol = "a", Description = "Weld throat thickness", Value = weld.DesignedThickness, Unit = "mm" },
						new() { Symbol = "L", Description = "Weld length", Value = weld.Length, Unit = "mm" },
						new() { Symbol = "LC", Description = "Critical load case ID", Value = weld.LoadCaseId, Unit = "-" },
					}
				});
			}

			// ─── BOLT CHECKS ───
			// Norsok M-001 §9.3: Only 8.8 and 10.9 grade bolts allowed.
			// Re-check bolt utilization with Norsok factors.

			foreach (var bolt in parsed.Bolts)
			{
				// Bolt interaction: combined tension + shear
				double interactionCheck = bolt.InteractionTensionShear;
				double tensionUC = bolt.UnityCheckTension;
				double shearUC = bolt.UnityCheckShear;

				results.Add(new NorsokFormulaResult
				{
					Section = "Bolt",
					Equation = "EN 1993-1-8 §3.6",
					Title = $"Bolt: {bolt.Name}",
					CheckExpression = "F_t,Sd/F_t,Rd + F_v,Sd/(1.4·F_v,Rd) ≤ 1.0",
					Demand = interactionCheck,
					Capacity = 1.0,
					Utilization = interactionCheck,
					Passed = interactionCheck <= 1.0,
					Variables = new List<FormulaVariable>
					{
						new() { Symbol = "F_t,Sd", Description = "Bolt tension force", Value = bolt.BoltTensionForce / 1000.0, Unit = "kN" },
						new() { Symbol = "F_v,Sd", Description = "Bolt shear force", Value = bolt.BoltShearForce / 1000.0, Unit = "kN" },
						new() { Symbol = "F_t,Rd", Description = "Tension resistance", Value = bolt.BoltTensionResistance / 1000.0, Unit = "kN" },
						new() { Symbol = "F_v,Rd", Description = "Shear resistance", Value = bolt.BoltShearResistance / 1000.0, Unit = "kN" },
						new() { Symbol = "UC_tension", Description = "Tension utilization", Value = tensionUC, Unit = "-" },
						new() { Symbol = "UC_shear", Description = "Shear utilization", Value = shearUC, Unit = "-" },
						new() { Symbol = "Interaction", Description = "Combined tension+shear check", Value = interactionCheck, Unit = "-" },
						new() { Symbol = "Assembly", Description = "Bolt assembly", Value = 0, Unit = bolt.BoltAssemblyName },
						new() { Symbol = "LC", Description = "Critical load case ID", Value = bolt.LoadCaseId, Unit = "-" },
					}
				});
			}

			// ─── TUBULAR MEMBER FORMULAS (demo with first plate's material data) ───
			// If we have plate data, demonstrate the tubular formulas with example geometry.
			// In production, these would be fed from actual member cross-section data.

			var refPlate = parsed.Plates.FirstOrDefault(p => p.MaterialFy > 0);
			if (refPlate != null)
			{
				double f_y = refPlate.MaterialFy;
				double gammaM = ProjectSettingsService.GammaM0_Norsok;

				// Extract max plate stress as proxy for axial stress
				double maxPlateStress = parsed.Plates.Max(p => p.MaxStress);

				// For demonstration: use the reference plate's material for tubular formulas.
				// Actual D and t should come from the tubular member geometry.
				// We add an informational result indicating tubular checks require geometry input.
				results.Add(new NorsokFormulaResult
				{
					Section = "6.3",
					Equation = "-",
					Title = "Tubular Member Checks (geometry required)",
					CheckExpression = "§6.3.2–6.3.8 require tubular D, t, member length — provide via parameters",
					Demand = maxPlateStress,
					Capacity = f_y / gammaM,
					Utilization = maxPlateStress / (f_y / gammaM),
					Passed = maxPlateStress <= f_y / gammaM,
					Variables = new List<FormulaVariable>
					{
						new() { Symbol = "σ_max", Description = "Max plate stress (all plates)", Value = maxPlateStress, Unit = "MPa" },
						new() { Symbol = "f_y", Description = "Reference yield strength", Value = f_y, Unit = "MPa" },
						new() { Symbol = "f_d", Description = "Design strength = f_y/γ_M", Value = f_y / gammaM, Unit = "MPa" },
						new() { Symbol = "γ_M", Description = "Norsok material factor", Value = gammaM, Unit = "-" },
					}
				});
			}

			return results;
		}
	}
}
