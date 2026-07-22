namespace NorsokChecker.Services.Norsok64
{
	/// <summary>
	/// K/Y/X joint classification per NORSOK Comm. 6.4.2 — faithful port of extract.py classify_kyx.
	///
	/// Per brace: transverse force q = N_Sd·sinθ·side (+ = points away from the chord on that face).
	/// K = part of |q| cancelled by SAME-side OPPOSITE-sign partners, nearest gap first (greedy) —
	/// each pairing carries its own gap (→ its own Q_g downstream). X = remainder balanced by ANY
	/// opposite-side brace (through-chord, no coaxiality). Y = leftover (beam shear in the chord).
	/// Optional gate: leftover ≤ gate·|q| → treat as 100 % K (NORSOK "within 10 %" wording — "should",
	/// not "shall"; default 0 = honest continuous breakdown).
	/// </summary>
	public static class KyxClassifier
	{
		public const double DefaultGate = 0.0;
		private const double Eps = 1e-9;

		/// <summary>Input geometry per brace for one load effect.</summary>
		public sealed record BraceGeom(string Name, int Side, double ThetaDeg, double NSd,
			double MipSd = 0.0, double MopSd = 0.0);

		public static List<KyxClass> Classify(IReadOnlyList<BraceGeom> bracesGeom,
			IReadOnlyList<BraceGap> gaps, double gate = DefaultGate)
		{
			// transverse (perpendicular-to-chord) force WITH sign, per brace
			var info = bracesGeom.ToDictionary(b => b.Name, b =>
			{
				double th = (b.ThetaDeg) * Math.PI / 180.0;
				double q = b.NSd * Math.Sin(th) * (b.Side >= 0 ? 1.0 : -1.0);
				return (Q: q, b.Side, b.NSd, b.ThetaDeg, b.MipSd, b.MopSd);
			});

			// gap lookup by unordered name pair
			var gapOf = new Dictionary<(string, string), double>();
			foreach (var g in gaps ?? Array.Empty<BraceGap>())
			{
				gapOf[(g.A, g.B)] = g.GapM;
				gapOf[(g.B, g.A)] = g.GapM;
			}

			var result = new List<KyxClass>();
			foreach (var b in bracesGeom)
			{
				var me = info[b.Name];
				double qB = me.Q;
				double absQ = Math.Abs(qB);
				if (absQ < Eps)
				{
					result.Add(new KyxClass
					{
						Name = b.Name, NSd = me.NSd, MipSd = me.MipSd, MopSd = me.MopSd,
						QTrans = qB, FrK = 0, FrX = 0, FrY = 0, Note = "no transverse force",
					});
					continue;
				}

				// --- 1+2: K = same-side, opposite-sign partners, nearest gap first ---
				var sameSideOpp = new List<(double Gap, string Partner, double Avail)>();
				foreach (var other in bracesGeom)
				{
					if (other.Name == b.Name) continue;
					var o = info[other.Name];
					if (o.Side != me.Side) continue;               // K requires SAME side
					if (o.Q * qB >= 0) continue;                   // K requires OPPOSITE sign
					double gm = gapOf.TryGetValue((b.Name, other.Name), out var g) ? g : double.PositiveInfinity;
					sameSideOpp.Add((gm, other.Name, Math.Abs(o.Q)));
				}
				sameSideOpp.Sort((x, y) => x.Gap.CompareTo(y.Gap));    // nearest (smallest gap) first

				double remaining = absQ;
				var kComponents = new List<KComponent>();
				foreach (var (gm, partner, avail) in sameSideOpp)
				{
					if (remaining <= Eps) break;
					double take = Math.Min(remaining, avail);
					if (take <= Eps) continue;
					kComponents.Add(new KComponent
					{
						Partner = partner,
						GapM = double.IsPositiveInfinity(gm) ? null : gm,
						Q = take, Frac = take / absQ,
					});
					remaining -= take;
				}
				double balancedK = absQ - remaining;
				double leftover = remaining;

				// --- 5: gate shortcut → treat as pure K ---
				if (leftover <= gate * absQ + Eps && balancedK > Eps)
				{
					double scale = absQ / balancedK;
					foreach (var kc in kComponents) kc.Frac *= scale;   // rescale so frK sums to 1
					result.Add(new KyxClass
					{
						Name = b.Name, NSd = me.NSd, MipSd = me.MipSd, MopSd = me.MopSd,
						QTrans = qB, FrK = 1.0, FrX = 0, FrY = 0, KComponents = kComponents,
						Note = $"balanced to {100.0 * leftover / absQ:F1}% <= gate {100.0 * gate:F0}% -> 100% K",
					});
					continue;
				}

				// --- 3: X = remainder balanced by OPPOSITE-side braces (through the chord) ---
				double oppCapacity = bracesGeom
					.Where(o => o.Name != b.Name && info[o.Name].Side != me.Side)
					.Sum(o => Math.Abs(info[o.Name].Q));
				double x = Math.Min(leftover, oppCapacity);
				double y = leftover - x;                            // 4: leftover → beam shear

				var notes = new List<string>();
				if (balancedK / absQ > Eps && kComponents.Count == 0)
					notes.Add("K with no gap data");
				result.Add(new KyxClass
				{
					Name = b.Name, NSd = me.NSd, MipSd = me.MipSd, MopSd = me.MopSd,
					QTrans = qB,
					FrK = balancedK / absQ, FrX = x / absQ, FrY = y / absQ,
					KComponents = kComponents, Note = string.Join("; ", notes),
				});
			}
			return result;
		}
	}
}
