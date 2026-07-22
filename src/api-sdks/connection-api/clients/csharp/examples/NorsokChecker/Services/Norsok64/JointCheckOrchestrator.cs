namespace NorsokChecker.Services.Norsok64
{
	/// <summary>
	/// STEP 4 — run the NORSOK §6.4 simple-joint check (Norsok64Engine.CheckJoint) for EVERY brace in
	/// EVERY load effect. Faithful port of extract.py joint_checks_all_les: §6.4 checks the CHORD WALL
	/// at each brace footprint (one check per brace, materially fy_CHORD); each brace's classification
	/// (frK/frX/frY + per-gap K components from STEP 3) drives the weighted resistance; the chord
	/// stresses from STEP 4-prep drive Qf.
	/// </summary>
	public static class JointCheckOrchestrator
	{
		public static List<PerLoadEffect<JointCheckRow>> RunAll(JointTopology topo, double gammaM = 1.15)
		{
			var outp = new List<PerLoadEffect<JointCheckRow>>();
			var secC = topo.Chord?.Section;
			double? dMm = secC?.D, tMm = secC?.T, fyChord = secC?.Fy;

			var clsByLe = topo.Classification.ToDictionary(le => le.Id,
				le => le.Rows.ToDictionary(c => c.Name));
			var stByLe = topo.ChordStresses.ToDictionary(le => le.Id,
				le => le.Rows.ToDictionary(b => b.Name));
			var secByName = topo.BracesMeta.ToDictionary(bm => bm.Name, bm => bm.Section);
			var thetaByName = topo.BracesMeta.ToDictionary(bm => bm.Name, bm => bm.ThetaDeg);

			foreach (var le in topo.BraceForces)
			{
				var clsMap = clsByLe.GetValueOrDefault(le.Id) ?? new Dictionary<string, KyxClass>();
				var stMap = stByLe.GetValueOrDefault(le.Id) ?? new Dictionary<string, ChordStressRow>();
				var rows = new List<JointCheckRow>();

				foreach (var bf in le.Rows)
				{
					var bsec = secByName.GetValueOrDefault(bf.Name);
					var cl = clsMap.GetValueOrDefault(bf.Name);
					var st = stMap.GetValueOrDefault(bf.Name);
					double? bd = bsec?.D, bt = bsec?.T;
					double thetaDeg = thetaByName.GetValueOrDefault(bf.Name, 0.0);

					// guard: need full chord+brace geometry, chord material, and a classification
					if (dMm is not > 0 || tMm is not > 0 || fyChord is not > 0 ||
						bd is not > 0 || bt is not > 0 || thetaDeg <= 0 || cl == null)
					{
						rows.Add(new JointCheckRow
						{
							Name = bf.Name, Skipped = true,
							Reason = "missing section/material/classification data",
						});
						continue;
					}

					// A brace with no axial classification (frK=frY=frX=0) still has a real bending check
					// (Table 6-3 bending Qu / Table 6-4 moment row are class-independent). Only skip when
					// there is truly nothing to check (no classification AND no bending load).
					double frSum = cl.FrK + cl.FrY + cl.FrX;
					bool hasMoment = Math.Abs(bf.Mip) > 1e-9 || Math.Abs(bf.Mop) > 1e-9;
					if (frSum <= 1e-9 && !hasMoment)
					{
						rows.Add(new JointCheckRow
						{
							Name = bf.Name, Skipped = true,
							Reason = string.IsNullOrEmpty(cl.Note)
								? "no axial force to classify (K/Y/X = 0) and no bending load"
								: cl.Note,
						});
						continue;
					}

					// K balancing components → (frac, gap m); gaps that came back null → 0 (touching)
					var kComponents = cl.KComponents
						.Select(kc => (kc.Frac, kc.GapM ?? 0.0)).ToList();
					double gRep = kComponents.Count > 0 ? kComponents[0].Item2 : 0.0;

					var inp = Joint64Input.FromSI(
						D: dMm.Value * 1e-3, T: tMm.Value * 1e-3, fyChord: fyChord.Value,
						d: bd.Value * 1e-3, t: bt.Value * 1e-3,
						fyBrace: bsec!.Fy ?? fyChord.Value,
						thetaDeg: thetaDeg, g: gRep,
						frK: cl.FrK, frY: cl.FrY, frX: cl.FrX,
						kComponents: kComponents.Count > 0 ? kComponents.Select(k => (k.Frac, k.Item2)).ToList() : null,
						nSd: bf.NSd, mipSd: bf.Mip, mopSd: bf.Mop,
						sigmaASd: st?.SigmaA ?? 0.0, sigmaMySd: st?.SigmaMy ?? 0.0, sigmaMzSd: st?.SigmaMz ?? 0.0,
						gammaM: gammaM);

					var r = Norsok64Engine.CheckJoint(inp);

					// dominant class (largest fraction) for representative display values
					var frac = new Dictionary<Joint64Class, double>
					{
						[Joint64Class.K] = cl.FrK, [Joint64Class.Y] = cl.FrY, [Joint64Class.X] = cl.FrX,
					};
					var dom = frac.Values.Max() > 1e-9
						? frac.OrderByDescending(kv => kv.Value).First().Key
						: Joint64Class.K;

					rows.Add(new JointCheckRow
					{
						Name = bf.Name, Skipped = false,
						Util = r.UtilWeighted, Passed = r.Passed,
						ChordOverstressed = r.ChordOverstressed,
						NoAxialClassification = frSum <= 1e-9,
						WithinRange = r.WithinRange,
						NRdWeighted = r.NRdWeighted, MRdIp = r.MRdIp, MRdOp = r.MRdOp,
						DomClass = dom.ToString(),
						Engine = r, Inputs = inp, ChordStress = st, Classification = cl,
					});
				}
				outp.Add(new PerLoadEffect<JointCheckRow> { Id = le.Id, Name = le.Name, Rows = rows });
			}
			return outp;
		}
	}
}
