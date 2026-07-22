using IdeaStatiCa.Api.Connection.Model;

namespace NorsokChecker.Services.Norsok64
{
	/// <summary>
	/// Joint-topology analysis — faithful port of extract.py build_connection's analytical core
	/// (chord identification, joint-plane RANSAC fit, per-brace θ/side/eccentricity, toe-to-toe gaps,
	/// assumption gate) plus the per-LE pipeline: STEP 2 brace forces → STEP 3 K/Y/X classification →
	/// STEP 4-prep chord stresses → STEP 4 §6.4 checks. The visual members2d/3d blocks are not ported.
	/// WORK POINT (node) = the GLOBAL ORIGIN (0,0,0), always (verified law, extract.py:773).
	/// </summary>
	public sealed class JointTopologyBuilder
	{
		// assumption-check tolerances (NORSOK + layer-1 policy) — mirror extract.py:18-38
		public const double CoplanarWarnDeg = 5.0;
		public const double CoplanarMaxDeg = 15.0;
		public const double ParallelMinThetaDeg = 5.0;
		public const double OutOfPlaneOffsetMm = 5.0;
		public const double EccAlongChordFrac = 0.25;
		public const double PlaneFitTolDeg = 2.0;
		public const double CoplanarEvalDeg = 15.0;

		private readonly double _oopTolMm;
		private readonly double _planeTolDeg;
		private readonly double _coplanarTolDeg;
		private readonly double _kyxGate;
		private readonly Action<string>? _log;

		public JointTopologyBuilder(double oopTolMm = OutOfPlaneOffsetMm, double planeTolDeg = PlaneFitTolDeg,
			double coplanarTolDeg = CoplanarEvalDeg, double kyxGate = KyxClassifier.DefaultGate,
			Action<string>? log = null)
		{
			_oopTolMm = oopTolMm;
			_planeTolDeg = planeTolDeg;
			_coplanarTolDeg = coplanarTolDeg;
			_kyxGate = kyxGate;
			_log = log;
		}

		/// <summary>Chord = the bearing member; fallback continuous / largest Ø. Port of identify_chord.</summary>
		public static (JointMemberData? Chord, List<string> Warnings) IdentifyChord(IReadOnlyList<JointMemberData> members)
		{
			var warns = new List<string>();
			if (members.Count == 0)
				return (null, new List<string> { "No members in this connection — nothing to identify as a chord." });

			var bearings = members.Where(m => m.IsBearing).ToList();
			var continuous = members.Where(m => m.IsContinuous).ToList();

			JointMemberData chord;
			if (bearings.Count == 1)
				chord = bearings[0];
			else if (bearings.Count > 1)
			{
				warns.Add($"{bearings.Count} bearing members — chord ambiguous.");
				chord = bearings[0];
			}
			else
			{
				warns.Add("No bearing member — chord picked by heuristic (continuous / largest Ø).");
				var cand = continuous.Count > 0 ? continuous : members.ToList();
				chord = cand.OrderByDescending(m => m.Section.D ?? 0).First();
			}

			if (continuous.Count == 0)
				warns.Add("No continuous member (chord must be continuous).");
			else if (continuous.Count > 1)
				warns.Add($"{continuous.Count} continuous members — chord ambiguous.");
			return (chord, warns);
		}

		/// <summary>Build the full topology + per-LE analysis for one connection.</summary>
		public JointTopology Build(IReadOnlyList<JointMemberData> members, IEnumerable<ConLoadEffect>? loadEffects)
		{
			var topo = new JointTopology();
			var (chord, warns) = IdentifyChord(members);
			if (chord == null)
			{
				topo.Verdict = new TopologyVerdict { Status = "ERROR", Errors = warns };
				return topo;
			}
			topo.Chord = chord;
			topo.Braces = members.Where(m => m.Id != chord.Id).ToList();

			Vec3 node = Vec3.Zero;                          // work point = global origin, always
			Vec3 ex = chord.AxisX.Unit();
			var secC = chord.Section;

			// ---- joint plane: X = chord axis (fixed); in-plane Y = robust RANSAC fit through brace dirs ----
			Vec3 fallbackEy = Math.Abs(Vec3.Dot(ex, new Vec3(0, 0, 1))) < 0.99
				? Vec3.Cross(new Vec3(0, 0, 1), ex).Unit()
				: new Vec3(0, 1, 0);

			Vec3? PerpOf(JointMemberData b)
			{
				Vec3 bd = JointForceResolver.EffDir(b).Unit();
				Vec3 perp = bd - ex * Vec3.Dot(bd, ex);     // component across the chord
				return perp.Norm > 1e-6 ? perp.Unit() : null;
			}

			var perps = topo.Braces
				.Select(b => (Brace: b, Perp: PerpOf(b)))
				.Where(t => t.Perp != null)
				.Select(t => (t.Brace, Perp: t.Perp!.Value))
				.ToList();

			Vec3 ey;
			double planeSpread = 0.0;
			if (perps.Count > 0)
			{
				double tol = Math.Max(0.0, _planeTolDeg);
				double DevDeg(Vec3 nCand, JointMemberData b)
				{
					Vec3 bd = JointForceResolver.EffDir(b).Unit();
					return Math.Asin(Math.Clamp(Math.Abs(Vec3.Dot(bd, nCand)), 0.0, 1.0)) * 180.0 / Math.PI;
				}

				// try each brace's perp as the seed plane; count inliers
				((int Count, double NegTot) Score, List<Vec3> Inliers, Vec3 NCand)? best = null;
				foreach (var (bseed, pseed) in perps)
				{
					Vec3 nCand = Vec3.Cross(ex, pseed).Unit();
					if (nCand.Norm < 1e-9) continue;
					var inl = perps.Where(t => DevDeg(nCand, t.Brace) <= tol).ToList();
					double tot = inl.Sum(t => DevDeg(nCand, t.Brace));
					var score = (inl.Count, -tot);
					if (best == null || score.CompareTo(best.Value.Score) > 0)
						best = (score, inl.Select(t => t.Perp).ToList(), nCand);
				}
				int nInliers = best?.Score.Count ?? 0;

				List<Vec3> inlierPerps;
				if (perps.Count == 1)
				{
					inlierPerps = new List<Vec3> { perps[0].Perp };
					topo.PlaneFitBasis = $"single brace {perps[0].Brace.Name}";
				}
				else if (nInliers >= 2)
				{
					inlierPerps = best!.Value.Inliers;
					topo.PlaneFitBasis = $"{nInliers} coplanar braces within {tol:G}deg";
				}
				else
				{
					// no pair within FIT tolerance → closest pair (or all braces on a tie / no valid pair)
					var pairs = new List<(double Dev, JointMemberData Bi, JointMemberData Bj, Vec3 Pi, Vec3 Pj)>();
					for (int i = 0; i < perps.Count; i++)
						for (int j = i + 1; j < perps.Count; j++)
						{
							Vec3 nIj = Vec3.Cross(ex, perps[i].Perp);
							if (nIj.Norm < 1e-9) continue;
							nIj = nIj.Unit();
							pairs.Add((DevDeg(nIj, perps[j].Brace), perps[i].Brace, perps[j].Brace,
								perps[i].Perp, perps[j].Perp));
						}
					if (pairs.Count > 0)
					{
						pairs.Sort((a, b) => a.Dev.CompareTo(b.Dev));
						double d0 = pairs[0].Dev;
						var tied = pairs.Where(p => Math.Abs(p.Dev - d0) <= 1e-6).ToList();
						if (tied.Count > 1)
						{
							inlierPerps = perps.Select(t => t.Perp).ToList();
							topo.PlaneFitBasis = "all braces (tie on closest pair)";
							topo.PlaneWarn = $"No two braces are coplanar within the {tol:G}deg fit tolerance and the " +
								$"closest-pair deviation ({d0:F1}deg) is shared by {tied.Count} pairs, so the plane is " +
								"averaged across all braces. The 2D plane is only indicative — check the 3D view.";
						}
						else
						{
							var p0 = pairs[0];
							inlierPerps = new List<Vec3> { p0.Pi, p0.Pj };
							topo.PlaneFitBasis = $"closest pair {p0.Bi.Name}-{p0.Bj.Name}";
							topo.PlaneWarn = $"No two braces are coplanar within the {tol:G}deg fit tolerance; " +
								$"the joint plane was built from the closest pair {p0.Bi.Name}-{p0.Bj.Name} " +
								$"(mutual deviation {p0.Dev:F1}deg > {tol:G}deg). The 2D plane is only indicative — " +
								"check the 3D view.";
						}
					}
					else
					{
						inlierPerps = perps.Select(t => t.Perp).ToList();
						topo.PlaneFitBasis = "all braces (no valid pair)";
						topo.PlaneWarn = "Could not form a brace pair for the joint plane; " +
							"fitted across all braces — the 2D plane is only indicative.";
					}
				}

				// final in-plane Y = dominant direction (SVD Vt[0] equivalent) of sign-aligned inlier perps
				(ey, planeSpread) = DominantDirection(inlierPerps);
				ey = (ey - ex * Vec3.Dot(ey, ex)).Unit();
				Vec3 nTmp = Vec3.Cross(ex, ey).Unit();
				topo.PlaneOutliers = topo.Braces
					.Where(b => Math.Asin(Math.Clamp(Math.Abs(Vec3.Dot(JointForceResolver.EffDir(b).Unit(), nTmp)), 0, 1))
						* 180.0 / Math.PI > tol)
					.Select(b => b.Name).ToList();
			}
			else
			{
				ey = fallbackEy;
				topo.PlaneFitBasis = "no brace (fallback orientation)";
			}
			Vec3 nPlane = Vec3.Cross(ex, ey).Unit();
			topo.Ex = ex; topo.Ey = ey; topo.NPlane = nPlane; topo.PlaneSpread = planeSpread;

			// ---- per-brace geometry helpers (ports of the nested closures) ----
			double Theta(JointMemberData m)
			{
				Vec3 bd = JointForceResolver.EffDir(m).Unit();
				double ang = Math.Acos(Math.Clamp(Vec3.Dot(ex, bd), -1.0, 1.0)) * 180.0 / Math.PI;
				return ang <= 90 ? ang : 180 - ang;
			}
			double CoplanarDev(JointMemberData m)
			{
				Vec3 bd = JointForceResolver.EffDir(m).Unit();
				return Math.Asin(Math.Clamp(Math.Abs(Vec3.Dot(bd, nPlane)), 0.0, 1.0)) * 180.0 / Math.PI;
			}
			int BraceSide(JointMemberData m)
			{
				Vec3 d3 = JointForceResolver.EffDir(m).Unit();
				Vec3 dp = d3 - nPlane * Vec3.Dot(d3, nPlane);
				if (dp.Norm < 1e-9) return 1;
				return Vec3.Dot(dp.Unit(), ey) >= 0 ? 1 : -1;
			}

			double rChord = (secC.D ?? 0) / 2.0 / 1000.0;   // chord radius (m): feet land on the SURFACE

			(double Landing, double Foot) LandingAndFoot(JointMemberData m)
			{
				int side = BraceSide(m);
				Vec3 d3 = JointForceResolver.EffDir(m).Unit();
				Vec3 dp = d3 - nPlane * Vec3.Dot(d3, nPlane);
				if (dp.Norm < 1e-9) return (0.0, 0.0);
				dp = dp.Unit();
				Vec3 o = m.Origin;
				double ox = Vec3.Dot(o - node, ex), oy = Vec3.Dot(o - node, ey);
				double dx = Vec3.Dot(dp, ex), dy = Vec3.Dot(dp, ey);
				double nn = Math.Sqrt(dx * dx + dy * dy); dx /= nn; dy /= nn;
				double zSurf = side * rChord;
				double landing = Math.Abs(dy) < 1e-9 ? ox : ox + (zSurf - oy) / dy * dx;
				double sinTh = Math.Abs(dy);
				double dDiam = (m.Section.D ?? 0) / 1000.0;
				double foot = sinTh > 1e-3 ? dDiam / 2 / sinTh : dDiam / 2;
				return (landing, foot);
			}

			// braces meta (β from chord D; ecc decomposed against the joint frame)
			foreach (var b in topo.Braces)
			{
				var sb = b.Section;
				topo.BracesMeta.Add(new BraceMeta
				{
					Name = b.Name, Section = sb,
					ThetaDeg = Theta(b),
					Beta = (sb.D is > 0 && secC.D is > 0) ? sb.D / secC.D : null,
					CoplanarDevDeg = CoplanarDev(b),
					OopOffsetM = Math.Abs(Vec3.Dot(JointForceResolver.EccVec(b), nPlane)),
					EccAlongM = Vec3.Dot(JointForceResolver.EccVec(b), ex),
					IsCHS = sb.IsCHS,
					Side = BraceSide(b),
				});
			}

			// gaps between braces on the SAME chord face — every pair, adjacency flagged (Fig 6-6 / Fig 6-2)
			topo.GapBraces = topo.Braces.Where(b => !b.IsContinuous).ToList();
			if (topo.GapBraces.Count >= 2)
			{
				var groups = new Dictionary<int, List<(string Name, double Landing, double Foot)>> { [1] = new(), [-1] = new() };
				foreach (var b in topo.GapBraces)
				{
					var (lnd, ft) = LandingAndFoot(b);
					groups[BraceSide(b)].Add((b.Name, lnd, ft));
				}
				foreach (var (sd, items) in groups)
				{
					if (items.Count < 2) continue;
					items.Sort((a, b) => a.Landing.CompareTo(b.Landing));   // order along the chord
					for (int i = 0; i < items.Count; i++)
						for (int j = i + 1; j < items.Count; j++)
						{
							var a = items[i]; var c = items[j];
							topo.Gaps.Add(new BraceGap
							{
								A = a.Name, B = c.Name,
								GapM = (c.Landing - c.Foot) - (a.Landing + a.Foot),  // toe-to-toe
								Side = sd, Adjacent = j == i + 1,
							});
						}
				}
			}

			// assumption gate (non-blocking); plane-fit warning + adjacent-overlap errors folded in after
			topo.Verdict = ClassifyAssumptions(chord, secC, topo.Braces, topo.BracesMeta, rChord * 2.0, warns);
			FinalizeVerdict(topo);

			// coplanarity evaluation against the finished plane (decoupled 15° NORSOK tolerance)
			topo.Coplanar = topo.BracesMeta.All(bm => bm.CoplanarDevDeg <= _coplanarTolDeg);
			topo.EvalOutliers = topo.BracesMeta.Where(bm => bm.CoplanarDevDeg > _coplanarTolDeg)
				.Select(bm => bm.Name).ToList();

			var les = (loadEffects ?? Enumerable.Empty<ConLoadEffect>()).ToList();
			var sideByName = topo.GapBraces.ToDictionary(b => b.Name, BraceSide);
			var thetaByName = topo.GapBraces.ToDictionary(b => b.Name, Theta);

			// STEP 2: per-LE brace forces resolved into the joint plane
			foreach (var le in les)
			{
				var loadsByMember = (le.MemberLoadings ?? new List<ConLoadEffectMemberLoad>())
					.Where(ml => ml.SectionLoad != null)
					.GroupBy(ml => ml.MemberId)
					.ToDictionary(g => g.Key, g => g.First().SectionLoad!);
				var rows = topo.GapBraces.Select(b =>
				{
					var row = JointForceResolver.BraceForceInPlane(b,
						loadsByMember.TryGetValue(b.Id, out var sl) ? sl : null, ex, nPlane);
					row.Side = sideByName[b.Name];
					return row;
				}).ToList();
				topo.BraceForces.Add(new PerLoadEffect<BraceForceRow> { Id = le.Id, Name = le.Name ?? "", Rows = rows });
			}

			// STEP 4-prep: per-LE chord stresses at each brace footprint (avg of Begin/End, NORSOK p.31)
			foreach (var le in les)
			{
				var (fAvg, mAvg, _) = JointForceResolver.ChordAvgLoad(chord, le);
				var rows = topo.GapBraces.Select(b =>
				{
					var (_, nb) = JointForceResolver.BraceSubplaneNormal(b, ex, nPlane);
					var st = JointForceResolver.ChordStressAtBrace(fAvg, mAvg, ex, secC, nb, sideByName[b.Name]);
					st.Name = b.Name;
					return st;
				}).ToList();
				topo.ChordStresses.Add(new PerLoadEffect<ChordStressRow> { Id = le.Id, Name = le.Name ?? "", Rows = rows });
			}

			// STEP 3: per-LE K/Y/X classification from the transverse-force balance
			foreach (var leForces in topo.BraceForces)
			{
				var geom = leForces.Rows.Select(r => new KyxClassifier.BraceGeom(
					r.Name, sideByName.GetValueOrDefault(r.Name, 1),
					thetaByName.GetValueOrDefault(r.Name, 90.0), r.NSd, r.Mip, r.Mop)).ToList();
				topo.Classification.Add(new PerLoadEffect<KyxClass>
				{
					Id = leForces.Id, Name = leForces.Name,
					Rows = KyxClassifier.Classify(geom, topo.Gaps, _kyxGate),
				});
			}

			// STEP 4: NORSOK §6.4 check per brace per LE
			topo.JointChecks = JointCheckOrchestrator.RunAll(topo);
			return topo;
		}

		/// <summary>
		/// Dominant direction of a set of (sign-aligned) unit vectors + the out-of-plane scatter —
		/// equivalent of extract.py's SVD Vt[0] / S[1] over the inlier perp matrix. Computed as the
		/// principal eigenvector of Σ vᵢvᵢᵀ via power iteration (deterministic: seeded by the aligned mean).
		/// </summary>
		internal static (Vec3 Dir, double Spread) DominantDirection(IReadOnlyList<Vec3> vectors)
		{
			if (vectors.Count == 0) return (Vec3.Zero, 0.0);
			Vec3 refV = vectors[0];
			var aligned = vectors.Select(v => Vec3.Dot(v, refV) >= 0 ? v : -v).ToList();
			if (aligned.Count == 1) return (aligned[0].Unit(), 0.0);

			// S = Σ v vᵀ (3×3 symmetric)
			double sxx = 0, sxy = 0, sxz = 0, syy = 0, syz = 0, szz = 0;
			foreach (var v in aligned)
			{
				sxx += v.X * v.X; sxy += v.X * v.Y; sxz += v.X * v.Z;
				syy += v.Y * v.Y; syz += v.Y * v.Z; szz += v.Z * v.Z;
			}
			Vec3 Mul(Vec3 v) => new(
				sxx * v.X + sxy * v.Y + sxz * v.Z,
				sxy * v.X + syy * v.Y + syz * v.Z,
				sxz * v.X + syz * v.Y + szz * v.Z);

			// power iteration from the aligned mean (positive dot with every row → never orthogonal)
			Vec3 mean = aligned.Aggregate(Vec3.Zero, (acc, v) => acc + v);
			Vec3 v1 = mean.Unit();
			for (int i = 0; i < 100; i++) v1 = Mul(v1).Unit();
			// canonical sign: align with the mean (same rule as the python reference after its SVD)
			if (Vec3.Dot(v1, mean) < 0) v1 = -v1;
			double lambda1 = Vec3.Dot(Mul(v1), v1);

			// deflate and iterate for the 2nd eigenvalue → spread = sqrt(λ2) (2nd singular value)
			Vec3 MulDeflated(Vec3 v) => Mul(v) - v1 * (lambda1 * Vec3.Dot(v1, v));
			Vec3 seed = Math.Abs(v1.X) < 0.9 ? new Vec3(1, 0, 0) : new Vec3(0, 1, 0);
			Vec3 v2 = (seed - v1 * Vec3.Dot(seed, v1)).Unit();
			for (int i = 0; i < 100; i++)
			{
				var next = MulDeflated(v2);
				if (next.Norm < 1e-15) { v2 = Vec3.Zero; break; }
				v2 = next.Unit();
			}
			double lambda2 = v2.Norm > 0 ? Math.Max(0.0, Vec3.Dot(MulDeflated(v2), v2)) : 0.0;
			return (v1, Math.Sqrt(lambda2));
		}

		/// <summary>Hard-ERROR vs soft-WARNING assumption gate. Port of classify_assumptions.</summary>
		private TopologyVerdict ClassifyAssumptions(JointMemberData chord, JointSectionInfo secC,
			IReadOnlyList<JointMemberData> braces, IReadOnlyList<BraceMeta> bracesMeta,
			double dChordM, IReadOnlyList<string> chordWarns)
		{
			var errors = new List<string>();
			var warnings = new List<string>();

			// gaps: a negative ADJACENT gap = overlap joint, out of the 6.4 gap rules → hard ERROR
			// (non-adjacent negative just means an intermediate brace sits between them)
			// note: called after topo.Gaps is populated via the outer instance — passed through field below

			foreach (var w in chordWarns)
			{
				if (w.Contains("continuous member")) errors.Add(w);   // chord must be exactly one continuous
				else warnings.Add(w);
			}

			// forces input mode — only Node / Position supported (we know where the force acts)
			if (chord.ForcesIn != ConMemberForcesInEnum.Node && chord.ForcesIn != ConMemberForcesInEnum.Position)
				errors.Add($"{chord.Name}: unsupported forces input '{chord.ForcesIn}' (only node/position).");
			foreach (var b in braces)
				if (b.ForcesIn != ConMemberForcesInEnum.Node && b.ForcesIn != ConMemberForcesInEnum.Position)
					errors.Add($"{b.Name}: unsupported forces input '{b.ForcesIn}' (only node/position).");

			// all members CHS
			if (!secC.IsCHS)
				errors.Add($"Chord not CHS ({secC.Name}).");
			foreach (var bm in bracesMeta)
				if (!bm.IsCHS)
					errors.Add($"{bm.Name}: not CHS ({bm.Section.Name}).");

			if (braces.Count == 0)
				errors.Add("No brace (chord only).");

			foreach (var bm in bracesMeta)
			{
				if (bm.ThetaDeg < ParallelMinThetaDeg)
					errors.Add($"{bm.Name}: θ={bm.ThetaDeg:F1}° — parallel to chord (degenerate).");
				double dev = bm.CoplanarDevDeg;
				if (dev > CoplanarMaxDeg)
					errors.Add($"{bm.Name}: {dev:F1}° off plane (>{CoplanarMaxDeg:F0}°) — different plane / multiplanar.");
				else if (dev > CoplanarWarnDeg)
					warnings.Add($"{bm.Name}: {dev:F1}° off plane (borderline).");
				double oopMm = bm.OopOffsetM * 1000.0;
				if (oopMm > _oopTolMm)
					errors.Add($"{bm.Name}: out-of-plane ecc. {oopMm:F0} mm (>{_oopTolMm:F0} mm).");
				if (dChordM > 0)
				{
					double e = Math.Abs(bm.EccAlongM);
					if (e > EccAlongChordFrac * dChordM)
						warnings.Add($"{bm.Name}: ecc. along chord e={e * 1000:F0} mm (>D/4={EccAlongChordFrac * dChordM * 1000:F0} mm).");
				}
				if (bm.Beta is double beta && (beta < 0.2 || beta > 1.0))
					warnings.Add($"{bm.Name}: β={beta:F3} outside 0.2–1.0.");
				double th = bm.ThetaDeg;
				if ((th < 30.0 || th > 90.0) && th >= ParallelMinThetaDeg)
					warnings.Add($"{bm.Name}: θ={th:F1}° outside 30–90°.");
			}

			if (secC.D is > 0 && secC.T is > 0)
			{
				double g = secC.D.Value / (2 * secC.T.Value);
				if (g < 10.0 || g > 50.0)
					warnings.Add($"γ={g:F2} outside 10–50.");
			}

			var verdict = new TopologyVerdict { Errors = errors, Warnings = warnings };
			verdict.Status = errors.Count > 0 ? "ERROR" : warnings.Count > 0 ? "WARNING" : "OK";
			return verdict;
		}

		/// <summary>Adds the plane-fit warning + adjacent-overlap errors that need the built gaps/plane.</summary>
		internal static void FinalizeVerdict(JointTopology topo)
		{
			if (!string.IsNullOrEmpty(topo.PlaneWarn))
				topo.Verdict.Warnings.Insert(0, topo.PlaneWarn!);
			foreach (var g in topo.Gaps.Where(g => g.Adjacent && g.GapM < 0))
				topo.Verdict.Errors.Insert(0,
					$"{g.A}-{g.B}: feet overlap (gap {g.GapM * 1000:F0} mm < 0) — overlap joint, out of 6.4 gap rules.");
			topo.Verdict.Status = topo.Verdict.Errors.Count > 0 ? "ERROR"
				: topo.Verdict.Warnings.Count > 0 ? "WARNING" : "OK";
		}
	}
}
