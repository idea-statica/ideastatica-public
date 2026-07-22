using IdeaStatiCa.Api.Connection.Model;

namespace NorsokChecker.Services.Norsok64
{
	/// <summary>
	/// Ports of extract.py's force-reading recipe (verified vs IDEA unbalanced forces):
	/// member_loading_global / node_equilibrium, brace_subplane_normal / brace_force_in_plane,
	/// _chord_avg_load / chord_stress_at_brace. PURE SI throughout (N, N·m, m, Pa).
	/// </summary>
	public static class JointForceResolver
	{
		public const double EquilibriumTolForceN = 1000.0;
		public const double EquilibriumTolMomNm = 1000.0;

		/// <summary>
		/// Sign to apply to a member's axisX to get its EFFECTIVE direction (the way the body extends
		/// FROM the joint node): continuous or connectedBy=End → -1, else +1. Port of eff_dir_sign.
		/// </summary>
		public static double EffDirSign(JointMemberData m) =>
			(m.IsContinuous || m.ConnectedBy == ConMemberConnectedByEnum.End) ? -1.0 : 1.0;

		/// <summary>Effective direction the member body extends from the node (unit).</summary>
		public static Vec3 EffDir(JointMemberData m) => (m.AxisX.Unit() * EffDirSign(m));

		/// <summary>
		/// The member's eccentricity as a GLOBAL vector = offsetEx·axisX + offsetEy·axisY + offsetEz·axisZ
		/// (offsets applied in the member's LOCAL CSYS; origin − ecc lies on the canonical axis through
		/// the node). Port of ecc_vec.
		/// </summary>
		public static Vec3 EccVec(JointMemberData m) =>
			m.AxisX * m.OffsetEx + m.AxisY * m.OffsetEy + m.AxisZ * m.OffsetEz;

		/// <summary>
		/// One member loading → (F_glob [N], M_glob about node [N·m]) in GLOBAL coords.
		/// Port of member_loading_global — see extract.py:139 for the full recipe:
		///   F = n·axisX + vy·axisY + vz·axisZ;  M_sec = mx·axisX + my·axisY + mz·axisZ
		///   application point per forcesIn (node-mode / position-mode), M = M_sec + r×F.
		/// </summary>
		public static (Vec3 F, Vec3 M) MemberLoadingGlobal(JointMemberData m, ConLoadEffectSectionLoad sl, Vec3 node)
		{
			Vec3 ax = m.AxisX, ay = m.AxisY, az = m.AxisZ;
			Vec3 F = ax * sl.N + ay * sl.Vy + az * sl.Vz;
			Vec3 Msec = ax * sl.Mx + ay * sl.My + az * sl.Mz;

			Vec3 ap;
			if (m.ForcesIn == ConMemberForcesInEnum.Position)
			{
				Vec3 axn = ax.Unit();
				Vec3 ecc = EccVec(m);
				double x = m.ModelX;
				if (m.IsContinuous)
					ap = node + axn * (-x) + ecc;                          // always -axisX
				else if (m.ConnectedBy == ConMemberConnectedByEnum.End)
					ap = node + axn * (-x) + ecc;
				else
					ap = node + axn * x + ecc;
			}
			else if (m.IsContinuous)
			{
				// node projected onto the member's axis line
				double t = Vec3.Dot(node - m.Origin, ax);
				ap = m.Origin + ax * t;
			}
			else
			{
				ap = m.Origin;                                             // origin already has ecc
			}
			Vec3 r = ap - node;
			return (F, Msec + Vec3.Cross(r, F));
		}

		/// <summary>
		/// Per-LE residual (unbalanced) force/moment at the node — self-check that forces/axes/levers are
		/// read right. PLAIN SUM (API forces are already 'member action on node'). Port of node_equilibrium.
		/// </summary>
		public static List<(int Id, string Name, double ResF, double ResM, bool Ok)> NodeEquilibrium(
			IReadOnlyList<JointMemberData> members, IEnumerable<ConLoadEffect>? loadEffects, Vec3 node)
		{
			var byId = members.ToDictionary(m => m.Id);
			var outp = new List<(int, string, double, double, bool)>();
			foreach (var le in loadEffects ?? Enumerable.Empty<ConLoadEffect>())
			{
				Vec3 SF = Vec3.Zero, SM = Vec3.Zero;
				foreach (var ml in le.MemberLoadings ?? new List<ConLoadEffectMemberLoad>())
				{
					if (!byId.TryGetValue(ml.MemberId, out var m) || ml.SectionLoad == null) continue;
					var (F, M) = MemberLoadingGlobal(m, ml.SectionLoad, node);
					SF += F; SM += M;
				}
				double resF = SF.Norm, resM = SM.Norm;
				outp.Add((le.Id, le.Name ?? "", resF, resM,
					resF <= EquilibriumTolForceN && resM <= EquilibriumTolMomNm));
			}
			return outp;
		}

		/// <summary>
		/// The brace's OWN sub-plane normal n_b = unit(ex × brace_dir), FLIPPED so dot(n_b, n_plane) ≥ 0
		/// (consistent M_ip sign across braces). Degenerate (brace ∥ chord) falls back to n_plane.
		/// Shared by brace-force and chord-stress resolution so the two frames can never drift.
		/// Port of brace_subplane_normal.
		/// </summary>
		public static (Vec3 Bx, Vec3 Nb) BraceSubplaneNormal(JointMemberData m, Vec3 ex, Vec3 nPlane)
		{
			Vec3 bx = EffDir(m).Unit();
			Vec3 nb = Vec3.Cross(ex, bx);
			if (nb.Norm < 1e-9)
				nb = nPlane;                    // brace parallel to chord — sub-plane undefined
			nb = nb.Unit();
			if (Vec3.Dot(nb, nPlane) < 0)
				nb = -nb;                       // unify orientation across all braces
			return (bx, nb);
		}

		/// <summary>
		/// Resolve ONE brace loading into NORSOK joint-check components in the brace's own sub-plane.
		/// SECTION forces (no r×F transfer to the node, per the reference implementation's choice).
		/// Port of brace_force_in_plane.
		/// </summary>
		public static BraceForceRow BraceForceInPlane(JointMemberData m, ConLoadEffectSectionLoad? sl,
			Vec3 ex, Vec3 nPlane)
		{
			sl ??= new ConLoadEffectSectionLoad();
			Vec3 ax = m.AxisX, ay = m.AxisY, az = m.AxisZ;
			Vec3 F = ax * sl.N + ay * sl.Vy + az * sl.Vz;
			Vec3 M = ax * sl.Mx + ay * sl.My + az * sl.Mz;

			var (bx, nb) = BraceSubplaneNormal(m, ex, nPlane);
			Vec3 ip = Vec3.Cross(nb, bx).Unit();   // in-plane transverse axis

			return new BraceForceRow
			{
				Name = m.Name,
				NSd = Vec3.Dot(F, bx),             // + tension
				Vip = Vec3.Dot(F, ip),
				Vop = Vec3.Dot(F, nb),
				Mip = Vec3.Dot(M, nb),             // in-plane bending
				Mop = Vec3.Dot(M, ip),             // out-of-plane bending
				Mtor = Vec3.Dot(M, bx),            // torsion (excluded from 6.57)
				SubNormalDot = Vec3.Dot(nb, nPlane),
			};
		}

		/// <summary>
		/// Average chord internal force/moment at the joint (GLOBAL) for one LE — NORSOK p.31: average of
		/// the chord loads on either side of the intersection (continuous chord = Begin+End loadings).
		/// SECTION moments, no r×F transfer. Port of _chord_avg_load.
		/// </summary>
		public static (Vec3 Favg, Vec3 Mavg, int SideCount) ChordAvgLoad(JointMemberData chord, ConLoadEffect le)
		{
			Vec3 ax = chord.AxisX, ay = chord.AxisY, az = chord.AxisZ;
			var sls = (le.MemberLoadings ?? new List<ConLoadEffectMemberLoad>())
				.Where(ml => ml.MemberId == chord.Id && ml.SectionLoad != null)
				.Select(ml => ml.SectionLoad!).ToList();
			if (sls.Count == 0)
				return (Vec3.Zero, Vec3.Zero, 0);
			Vec3 F = Vec3.Zero, M = Vec3.Zero;
			foreach (var sl in sls)
			{
				F += ax * sl.N + ay * sl.Vy + az * sl.Vz;
				M += ax * sl.Mx + ay * sl.My + az * sl.Mz;
			}
			return (F * (1.0 / sls.Count), M * (1.0 / sls.Count), sls.Count);
		}

		/// <summary>
		/// Chord nominal stresses at ONE brace footprint, NORSOK Qf sign convention (Pa):
		/// σ_a = N/A (+tension); σ_my = −(M_ip·side·R/I) (+compression in footprint);
		/// σ_mz = M_op·R/I (sign irrelevant, squared in A²). Port of chord_stress_at_brace.
		/// </summary>
		public static ChordStressRow ChordStressAtBrace(Vec3 fAvg, Vec3 mAvg, Vec3 ex,
			JointSectionInfo secC, Vec3 nb, int side)
		{
			double? dMm = secC.D, tMm = secC.T;
			if (dMm is not > 0 || tMm is not > 0)
				return new ChordStressRow { Side = side };

			double D = dMm.Value * 1e-3, T = tMm.Value * 1e-3;
			double di = D - 2 * T, R = D / 2.0;
			double A = Math.PI / 4.0 * (D * D - di * di);
			double I = Math.PI / 64.0 * (Math.Pow(D, 4) - Math.Pow(di, 4));
			double nChord = Vec3.Dot(fAvg, ex);
			double sigmaA = A > 0 ? nChord / A : 0.0;

			double mIp = Vec3.Dot(mAvg, nb);
			double zIp = side * R;
			double sigmaMyFibre = I > 0 ? mIp * zIp / I : 0.0;   // + tension by mechanics
			double sigmaMy = -sigmaMyFibre;                       // NORSOK: + compression in footprint

			Vec3 ipAxis = Vec3.Cross(nb, ex).Norm > 1e-9 ? Vec3.Cross(nb, ex).Unit() : Vec3.Zero;
			double mOp = Vec3.Dot(mAvg, ipAxis);
			double sigmaMz = I > 0 ? mOp * R / I : 0.0;

			return new ChordStressRow
			{
				SigmaA = sigmaA, SigmaMy = sigmaMy, SigmaMz = sigmaMz,
				A = A, I = I, R = R, NChord = nChord, MipChord = mIp, MopChord = mOp, Side = side,
			};
		}
	}
}
