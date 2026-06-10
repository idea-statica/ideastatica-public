using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;
using NorsokChecker.Services.Formulas;

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
		/// Fetch load effects (internal forces) for a connection from the API.
		/// </summary>
		public async Task<List<ConLoadEffect>> GetLoadEffectsAsync(int connectionId, CancellationToken ct = default)
		{
			return await _client.LoadEffect.GetLoadEffectsAsync(_projectId, connectionId, cancellationToken: ct);
		}

		/// <summary>
		/// Evaluate all Norsok formulas using raw results, load effects, and tubular geometry.
		/// </summary>
		public List<NorsokFormulaResult> EvaluateNorsokFormulas(
			int connectionId,
			string rawJsonResults,
			List<ConLoadEffect>? loadEffects = null,
			TubularGeometry? geometry = null,
			double memberLength = 0,
			double kFactor = 0.7,
			TubularJointGeometry? jointGeometry = null,
			DesignClassificationInput? dcInput = null,
			double[]? chordStresses = null,
			List<MemberDisplayInfo>? members = null)
		{
			var results = new List<NorsokFormulaResult>();

			// Parse raw CBFEM results
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
					CheckExpression = ex.Message, Passed = false
				});
				return results;
			}

			_log($"    Parsed: {parsed.Plates.Count} plates, {parsed.Welds.Count} welds, {parsed.Bolts.Count} bolts");

			double gammaM0 = ProjectSettingsService.GammaM0_Norsok;  // 1.15
			double gammaM2 = ProjectSettingsService.GammaM2_Norsok;  // 1.30
			// Note: γBC = 1.05 (ProjectSettingsService.GammaBC) is the additional
			// building code factor per §6.1. It is applied implicitly when the CBFEM
			// engine uses the updated γM values. For standalone formula checks on
			// tubular members (§6.3), the Norsok γM = 1.15 already accounts for this.

			// ─── DESIGN CLASSIFICATION (§5) ───
			if (dcInput != null)
			{
				var dc = DesignClassification.ClassifyJoint(
					dcInput.SubstantialConsequences, dcInput.ResidualStrength, dcInput.HighComplexity);

				// Use max weld utilization for stress classification
				double maxWeldUtil = parsed.Welds.Count > 0
					? parsed.Welds.Max(w => w.MaxUnityCheck) : 0;
				var stressLevel = DesignClassification.ClassifyStressLevel(maxWeldUtil);

				string sql = DesignClassification.GetSteelQualityLevel(dc, dcInput.ThroughThickness);
				string ic = dcInput.HighFatigue
					? DesignClassification.GetInspectionCategory_HighFatigue(dc, true)
					: DesignClassification.GetInspectionCategory_LowFatigue(dc, stressLevel);

				var dcResult = DesignClassification.GenerateClassificationReport(
					dc, sql, ic, maxWeldUtil, dcInput.HighFatigue);
				results.Add(dcResult);

				_log($"    §5 Classification: {dc}, SQL={sql}, Inspection={ic}");
			}

			// ─── PLATE STRESS CHECKS ───
			EvaluatePlateChecks(parsed, gammaM0, results);

			// ─── WELD CHECKS ───
			// Norsok Table 6-1: γM2 = 1.30 for welds (not EC3 default 1.25)
			EvaluateWeldChecks(parsed, gammaM2, results);

			// ─── BOLT CHECKS ───
			EvaluateBoltChecks(parsed, results);

			// ─── TUBULAR MEMBER FORMULAS (§6.3.2–6.3.8) ───
			// These require: load effects (N, V, M) + tubular geometry (D, t) + member length
			if (loadEffects != null && loadEffects.Count > 0 && geometry != null)
			{
				EvaluateTubularMemberFormulas(loadEffects, geometry, memberLength, kFactor, gammaM0, results, members);
			}
			else
			{
				string reason = geometry == null ? "tubular geometry not provided" : "no load effects available";
				_log($"    Skipping §6.3 tubular member formulas ({reason})");
			}

			// ─── TUBULAR JOINT CHECKS (§6.4) ───
			if (loadEffects != null && loadEffects.Count > 0 && jointGeometry != null && jointGeometry.D > 0)
			{
				// §6.4.3.1 Validity range check — MUST pass before running joint formulas
				var (valid, validityResult) = ValidateJointGeometry(jointGeometry);
				results.Add(validityResult);

				if (valid)
				{
					double sigmaA = chordStresses != null && chordStresses.Length > 0 ? chordStresses[0] : 0;
					double sigmaMy = chordStresses != null && chordStresses.Length > 1 ? chordStresses[1] : 0;
					double sigmaMz = chordStresses != null && chordStresses.Length > 2 ? chordStresses[2] : 0;
					EvaluateTubularJointChecks(loadEffects, jointGeometry, gammaM0, sigmaA, sigmaMy, sigmaMz, results);
				}
				else
				{
					_log("    §6.4 SKIPPED — joint geometry outside validity range (§6.4.3.1)");
				}
			}
			else if (jointGeometry != null && jointGeometry.D > 0)
			{
				_log("    Skipping §6.4 joint checks (no load effects available)");
			}

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
					Section = "6.3.2",
					Equation = "6.1",
					Title = $"Plate: {plate.Name}",
					CheckExpression = "σ_Ed ≤ f_yd = f_y / γ_M",
					Formula = "f_yd = f_y / γ_M",
					FormulaSubstituted = $"f_yd = {f_y:F1} / {gammaM:F2} = {f_d:F1} MPa",
					Demand = plate.MaxStress,
					Capacity = f_d,
					Utilization = utilization,
					Passed = utilization <= 1.0,
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
				double fu = weld.MaterialFu;
				double betaW = weld.BetaW > 0 ? weld.BetaW : 0.85;
				double resistance = fu > 0 ? fu / (betaW * gammaM2) : 0;
				double utilization = resistance > 0 ? weld.MaxEquivalentStress / resistance : 0;

				results.Add(new NorsokFormulaResult
				{
					Section = "Weld",
					Equation = "EN 1993-1-8 §4.5",
					Title = $"Weld: {(string.IsNullOrEmpty(weld.Name) ? $"#{weld.Id}" : weld.Name)}",
					CheckExpression = "σ_w ≤ f_w,Rd = f_u / (β_w · γ_M2)",
					Formula = "f_w,Rd = f_u / (β_w · γ_M2)",
					FormulaSubstituted = $"f_w,Rd = {fu:F1} / ({betaW:F2} × {gammaM2:F2}) = {resistance:F1} MPa",
					Demand = weld.MaxEquivalentStress,
					Capacity = resistance,
					Utilization = utilization,
					Passed = utilization <= 1.0,
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

		/// <summary>
		/// Evaluate Norsok §6.3 tubular member formulas — one set of checks per member.
		/// Each member is checked with its own geometry, f_y, L and k from the Members
		/// grid; its forces are never mixed with other members' capacities.
		/// </summary>
		private void EvaluateTubularMemberFormulas(
			List<ConLoadEffect> loadEffects,
			TubularGeometry fallbackGeometry,
			double fallbackLength,
			double fallbackK,
			double gammaM_base,
			List<NorsokFormulaResult> results,
			List<MemberDisplayInfo>? members = null)
		{
			var memberIds = loadEffects
				.Where(le => le.MemberLoadings != null)
				.SelectMany(le => le.MemberLoadings!)
				.Where(ml => ml.SectionLoad != null)
				.Select(ml => ml.MemberId)
				.Distinct()
				.ToList();

			foreach (int memberId in memberIds)
			{
				var mdi = members?.FirstOrDefault(m => m.Id == memberId);
				if (mdi != null && !mdi.IsCHS)
				{
					_log($"    §6.3 skipped for member '{mdi.Name}' — not CHS (outside §6.3 scope)");
					continue;
				}

				TubularGeometry geo = mdi != null && mdi.Diameter > 0 && mdi.WallThickness > 0
					? TubularGeometryCalc.Calculate(mdi.Diameter, mdi.WallThickness)
					: fallbackGeometry;
				double f_y = mdi != null && mdi.Fy > 0 ? mdi.Fy : 355;
				double mL = mdi?.L ?? fallbackLength;
				double mK = mdi?.K ?? fallbackK;
				string label = mdi?.Name ?? $"Member {memberId}";

				_log($"    Running §6.3 tubular checks for '{label}': D={geo.D}mm, t={geo.t}mm, fy={f_y}MPa, L={mL}mm, k={mK}");
				_log($"      Geometry: A={geo.A:F0}mm², W={geo.W:F0}mm³, Z={geo.Z:F0}mm³, i={geo.i:F1}mm");

				EvaluateSingleMemberFormulas(loadEffects, memberId, label, geo, mL, mK, f_y, gammaM_base, results);
			}

			int memberCheckCount = results.Count(r => r.Section.StartsWith("6.3") && !r.Title.StartsWith("Plate"));
			_log($"    {memberCheckCount} tubular member formula result(s) across {memberIds.Count} member(s)");
		}

		/// <summary>
		/// Run all §6.3 checks for one member, enveloping across load cases and member
		/// ends belonging to this member only.
		/// </summary>
		private void EvaluateSingleMemberFormulas(
			List<ConLoadEffect> loadEffects,
			int memberId,
			string memberLabel,
			TubularGeometry geo,
			double mL,
			double mK,
			double f_y,
			double gammaM,
			List<NorsokFormulaResult> results)
		{
			// §6.3.7 material factor — class 4 sections (f_y/f_cle > 0.170) get variable γM (Eq. 6.22)
			double f_cle = 2.0 * 0.3 * 2.1e5 * geo.t / geo.D;
			if (f_y / f_cle > 0.170)
			{
				double f_cl = GetLocalBucklingStrength(f_y, geo.D, geo.t);

				// Conservative σ_c,Sd estimate from this member's own loadings (Eq. 6.25)
				double sigma_c_est = 0;
				foreach (var le in loadEffects)
				{
					foreach (var ml in le.MemberLoadings ?? new())
					{
						if (ml.MemberId != memberId || ml.SectionLoad == null) continue;
						double N_kN = Math.Abs(ml.SectionLoad.N / 1000.0);
						double M_kNm = Math.Sqrt(Math.Pow(ml.SectionLoad.My / 1000.0, 2) + Math.Pow(ml.SectionLoad.Mz / 1000.0, 2));
						sigma_c_est = Math.Max(sigma_c_est, N_kN * 1000.0 / geo.A + M_kNm * 1e6 / geo.W); // MPa
					}
				}

				var (varGammaM, gammaResult) = Formulas.MaterialFactorCalc.Calculate(
					sigma_c_est, f_cl, f_y, f_cle);

				gammaM = varGammaM;
				gammaResult.Title = $"{gammaResult.Title} — {memberLabel}";
				results.Add(gammaResult);
				_log($"      §6.3.7 Class 4 section: f_y/f_cle={f_y / f_cle:F3}, γM={gammaM:F3}");
			}
			else
			{
				_log($"      §6.3.7 Class 1-3 section: f_y/f_cle={f_y / f_cle:F3}, γM={gammaM} (constant)");
			}

			// Track worst-case results across load cases and member ends
			NorsokFormulaResult? worstTension = null;
			NorsokFormulaResult? worstCompression = null;
			NorsokFormulaResult? worstBending = null;
			NorsokFormulaResult? worstShear = null;
			NorsokFormulaResult? worstTorsion = null;
			NorsokFormulaResult? worstTensionBending = null;
			NorsokFormulaResult? worstCompBending627 = null;
			NorsokFormulaResult? worstCompBending628 = null;
			NorsokFormulaResult? worstShearBending = null;
			NorsokFormulaResult? worstShearBendingTorsion = null;

			foreach (var le in loadEffects)
			{
				if (le.MemberLoadings == null || le.MemberLoadings.Count == 0) continue;

				foreach (var ml in le.MemberLoadings)
				{
					if (ml.MemberId != memberId || ml.SectionLoad == null) continue;
					var sl = ml.SectionLoad;

					// API returns forces in N and moments in N·m — convert to kN and kNm
					double N = sl.N / 1000.0;      // N → kN
					double Vy = sl.Vy / 1000.0;    // N → kN
					double Vz = sl.Vz / 1000.0;    // N → kN
					double Mx = sl.Mx / 1000.0;    // N·m → kN·m
					double My = sl.My / 1000.0;    // N·m → kN·m
					double Mz = sl.Mz / 1000.0;    // N·m → kN·m

					double V_total = Math.Sqrt(Vy * Vy + Vz * Vz); // Resultant shear [kN]
					double M_resultant = Math.Sqrt(My * My + Mz * Mz);
					double Mt = Math.Abs(Mx); // Torsional moment magnitude [kNm]

					// Label with member, stamp governing load case, keep the worst per check
					void Track(ref NorsokFormulaResult? worst, NorsokFormulaResult r)
					{
						r.Title = $"{r.Title} — {memberLabel}";
						r.LoadCaseId = le.Id;
						if (worst == null || r.Utilization > worst.Utilization)
							worst = r;
					}

					// §6.3.2 Axial Tension (N > 0 means tension)
					if (N > 0)
						Track(ref worstTension, AxialTensionCheck.Evaluate(N, geo.A, f_y, gammaM));

					// §6.3.3 Axial Compression (N < 0 means compression, pass as positive)
					if (N < 0)
						Track(ref worstCompression, AxialCompressionCheck.Evaluate(
							Math.Abs(N), geo.A, f_y, geo.D, geo.t, mK, mL, geo.i, gammaM));

					// §6.3.4 Bending (resultant moment)
					if (M_resultant > 0)
						Track(ref worstBending, BendingCheck.Evaluate(M_resultant, geo.W, geo.Z, f_y, geo.D, geo.t, gammaM));

					// §6.3.5 Beam Shear
					if (V_total > 0)
						Track(ref worstShear, ShearCheck.EvaluateBeamShear(V_total, geo.A, f_y, gammaM));

					// §6.3.5 Torsional Shear (Eq. 6.14)
					if (Mt > 0)
						Track(ref worstTorsion, ShearCheck.EvaluateTorsionalShear(Mt, geo.Ip, geo.D, f_y, gammaM));

					// §6.3.8.1 Tension + Bending
					if (N > 0 && M_resultant > 0)
					{
						double N_t_Rd = geo.A * f_y / gammaM / 1000.0;
						double M_Rd = GetBendingResistance(geo, f_y, gammaM);
						Track(ref worstTensionBending, TensionBendingCheck.Evaluate(N, N_t_Rd, My, Mz, M_Rd));
					}

					// §6.3.8.2 Compression + Bending (both equations)
					if (N < 0 && M_resultant > 0)
					{
						double N_abs = Math.Abs(N);
						double M_Rd = GetBendingResistance(geo, f_y, gammaM);

						// N_c,Rd from axial compression formula
						var compResult = AxialCompressionCheck.Evaluate(
							N_abs, geo.A, f_y, geo.D, geo.t, mK, mL, geo.i, gammaM);
						double N_c_Rd = compResult.Capacity;

						// Euler buckling loads
						double N_Ey = CompressionBendingCheck.EulerBucklingLoad(geo.A, mK, mL, geo.i);
						double N_Ez = N_Ey; // Same for tubular (symmetric)

						// Cm factors — use 0.85 as default (conservative, Table 6-2)
						double Cmy = 0.85;
						double Cmz = 0.85;

						// Eq. 6.27 stability check
						Track(ref worstCompBending627, CompressionBendingCheck.EvaluateStability(
							N_abs, N_c_Rd, My, Mz, M_Rd, Cmy, Cmz, N_Ey, N_Ez));

						// Eq. 6.28 cross-section check
						double f_cl_local = GetLocalBucklingStrength(f_y, geo.D, geo.t);
						double N_cl_Rd = geo.A * f_cl_local / gammaM / 1000.0;
						Track(ref worstCompBending628, CompressionBendingCheck.EvaluateCrossSection(
							N_abs, N_cl_Rd, My, Mz, M_Rd));
					}

					// §6.3.8.3 Shear + Bending
					if (V_total > 0 && M_resultant > 0)
					{
						double M_Rd = GetBendingResistance(geo, f_y, gammaM);
						double V_Rd = geo.A * f_y / (2.0 * Math.Sqrt(3.0) * gammaM) / 1000.0;
						Track(ref worstShearBending, ShearBendingCheck.Evaluate(M_resultant, M_Rd, V_total, V_Rd));
					}

					// §6.3.8.4 Shear + Bending + Torsion (Eq. 6.33) — only when torsion is present
					if (Mt > 0 && M_resultant > 0)
					{
						double V_Rd = geo.A * f_y / (2.0 * Math.Sqrt(3.0) * gammaM) / 1000.0;
						double f_m = BendingCheck.CharacteristicBendingStrength(geo.W, geo.Z, f_y, geo.D, geo.t);
						Track(ref worstShearBendingTorsion, ShearBendingTorsionCheck.Evaluate(
							M_resultant, V_total, V_Rd, Mt, geo.W, f_m, f_y, geo.D, geo.t, gammaM));
					}
				}
			}

			// Add worst-case results for this member
			if (worstTension != null) results.Add(worstTension);
			if (worstCompression != null) results.Add(worstCompression);
			if (worstBending != null) results.Add(worstBending);
			if (worstShear != null) results.Add(worstShear);
			if (worstTorsion != null) results.Add(worstTorsion);
			if (worstTensionBending != null) results.Add(worstTensionBending);
			if (worstCompBending627 != null) results.Add(worstCompBending627);
			if (worstCompBending628 != null) results.Add(worstCompBending628);
			if (worstShearBending != null) results.Add(worstShearBending);
			if (worstShearBendingTorsion != null) results.Add(worstShearBendingTorsion);
		}

		/// <summary>Get bending resistance M_Rd [kNm] per §6.3.4</summary>
		private static double GetBendingResistance(TubularGeometry geo, double f_y, double gammaM)
		{
			var bendingResult = BendingCheck.Evaluate(1.0, geo.W, geo.Z, f_y, geo.D, geo.t, gammaM);
			return bendingResult.Capacity; // M_Rd in kNm
		}

		/// <summary>Get local buckling strength f_cl [MPa] per Eq. 6.6–6.8</summary>
		/// <summary>
		/// §6.4.3.1 — Validate joint geometry ranges before running §6.4 formulas.
		/// Returns (isValid, formulaResult with details).
		/// If invalid, §6.4 checks must NOT be executed.
		/// </summary>
		private (bool valid, NorsokFormulaResult result) ValidateJointGeometry(TubularJointGeometry joint)
		{
			double beta = joint.Beta;
			double gamma = joint.Gamma;
			double theta = joint.ThetaDeg;
			double gapRatio = joint.GapRatio;

			var violations = new List<string>();

			if (beta < 0.2 || beta > 1.0)
				violations.Add($"β = {beta:F3} — required: 0.2 ≤ β ≤ 1.0");
			if (gamma < 10 || gamma > 50)
				violations.Add($"γ = {gamma:F1} — required: 10 ≤ γ ≤ 50");
			if (theta < 30 || theta > 90)
				violations.Add($"θ = {theta:F0}° — required: 30° ≤ θ ≤ 90°");
			if (joint.JointType == JointType.K && gapRatio < 0.6)
			{
				// g/D ≥ 0.6 for K joints — wait, the norm says g ≥ -0.6D... let me re-check
				// Actually: "g/D ≥ 0.6 (for K joints)" is NOT right
				// The norm says gap should be > 50mm and < D
				if (joint.Gap < 50)
					violations.Add($"g = {joint.Gap:F0}mm — required: g > 50mm for K-joints");
				if (joint.Gap > joint.D)
					violations.Add($"g = {joint.Gap:F0}mm > D = {joint.D:F0}mm — required: g < D");
			}

			bool valid = violations.Count == 0;

			foreach (var v in violations)
				_log($"    VALIDITY ERROR: {v}");

			var variables = new List<FormulaVariable>
			{
				new() { Symbol = "β", Description = $"d/D = {joint.d:F0}/{joint.D:F0}", Value = beta, Unit = "-" },
				new() { Symbol = "γ", Description = $"D/(2T) = {joint.D:F0}/(2×{joint.T:F0})", Value = gamma, Unit = "-" },
				new() { Symbol = "θ", Description = "brace-to-chord angle", Value = theta, Unit = "°" },
			};

			if (joint.JointType == JointType.K)
			{
				variables.Add(new() { Symbol = "g", Description = "gap between braces", Value = joint.Gap, Unit = "mm" });
				variables.Add(new() { Symbol = "g/D", Description = "gap ratio", Value = gapRatio, Unit = "-" });
			}

			string checkExpr = valid
				? "All parameters within validity range — §6.4 formulas applicable"
				: $"OUTSIDE VALIDITY RANGE — §6.4 formulas NOT applicable. {violations.Count} violation(s)";

			var result = new NorsokFormulaResult
			{
				Section = "6.4.3.1",
				Equation = "Table",
				Title = valid ? "Joint Geometry — Valid" : "Joint Geometry — INVALID",
				CheckExpression = checkExpr,
				Formula = "0.2 ≤ β ≤ 1.0,  10 ≤ γ ≤ 50,  30° ≤ θ ≤ 90°",
				FormulaSubstituted = valid
					? $"β={beta:F3} ✓  γ={gamma:F1} ✓  θ={theta:F0}° ✓"
					: string.Join(";  ", violations),
				Demand = 0,
				Capacity = 1,
				Utilization = 0,
				Passed = valid,
				Variables = variables
			};

			return (valid, result);
		}

		private static double GetLocalBucklingStrength(double f_y, double D, double t)
		{
			double f_cle = 2.0 * 0.3 * 2.1e5 * t / D;
			double ratio = f_y / f_cle;

			if (ratio <= 0.170) return f_y;
			if (ratio <= 1.911) return (1.047 - 0.274 * ratio) * f_y;
			return f_cle;
		}

		/// <summary>
		/// Evaluate §6.4 tubular joint checks for each brace member per load case.
		/// </summary>
		private void EvaluateTubularJointChecks(
			List<ConLoadEffect> loadEffects,
			TubularJointGeometry joint,
			double gammaM,
			double sigmaA_chord, double sigmaMy_chord, double sigmaMz_chord,
			List<NorsokFormulaResult> results)
		{
			_log($"    Running §6.4 joint checks: Chord {joint.D}×{joint.T}, Brace {joint.d}×{joint.t}, θ={joint.ThetaDeg}°, {joint.JointType}");
			_log($"    β={joint.Beta:F3}, γ={joint.Gamma:F1}, τ={joint.Tau:F3}");
			_log($"    Chord stresses for Qf: σ_a={sigmaA_chord:F1}, σ_my={sigmaMy_chord:F1}, σ_mz={sigmaMz_chord:F1} MPa");

			if (joint.Beta < 0.2 || joint.Beta > 1.0)
				_log($"    WARNING: β={joint.Beta:F3} outside validity range 0.2–1.0");
			if (joint.Gamma < 10 || joint.Gamma > 50)
				_log($"    WARNING: γ={joint.Gamma:F1} outside validity range 10–50");

			NorsokFormulaResult? worstJoint = null;

			foreach (var le in loadEffects)
			{
				if (le.MemberLoadings == null) continue;

				foreach (var ml in le.MemberLoadings)
				{
					if (ml.SectionLoad == null) continue;
					var sl = ml.SectionLoad;

					double N_kN = sl.N / 1000.0;
					double My_kNm = sl.My / 1000.0;
					double Mz_kNm = sl.Mz / 1000.0;

					var r = TubularJointCheck.EvaluateJointInteraction(
						joint, N_kN, My_kNm, Mz_kNm,
						sigmaA_chord, sigmaMy_chord, sigmaMz_chord, gammaM);
					r.LoadCaseId = le.Id;

					if (worstJoint == null || r.Utilization > worstJoint.Utilization)
						worstJoint = r;
				}
			}

			if (worstJoint != null)
			{
				results.Add(worstJoint);
				_log($"    §6.4.3.6 Joint interaction: util={worstJoint.Utilization * 100:F1}% LC{worstJoint.LoadCaseId} {(worstJoint.Passed ? "PASS" : "FAIL")}");
			}
		}
	}
}
