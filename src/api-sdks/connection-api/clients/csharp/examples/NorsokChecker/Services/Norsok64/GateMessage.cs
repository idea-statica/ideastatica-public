using System.Globalization;
using NorsokChecker.Models;

namespace NorsokChecker.Services.Norsok64
{
	/// <summary>What a gate message is about. The wording lives in <see cref="GateMessage.Render"/>.</summary>
	public enum GateKind
	{
		/// <summary>A finished, unit-free sentence: a section rejection, "No brace", "No load effect".</summary>
		Text,
		/// <summary>Member–Partner feet overlap; Value = the gap in metres (negative).</summary>
		Overlap,
		/// <summary>Member sits Value metres out of the joint plane through the chord; Limit = the tolerance.</summary>
		OutOfPlane,
		/// <summary>Member's eccentricity along the chord, Value metres, over Limit = D/4 metres.</summary>
		EccAlongChord,
		/// <summary>Member's θ = Value degrees is below the parallel floor.</summary>
		ThetaParallel,
		/// <summary>Member is Value degrees off the plane, past Limit degrees — a hard error.</summary>
		OffPlaneError,
		/// <summary>Member is Value degrees off the plane — borderline.</summary>
		OffPlaneWarn,
		/// <summary>Member's β = Value is outside 0.2–1.0.</summary>
		BetaOutside,
		/// <summary>Member's θ = Value degrees is outside 30–90°.</summary>
		ThetaOutside,
		/// <summary>The chord's γ = Value is outside 10–50.</summary>
		GammaOutside,
		/// <summary>Member's section name says Ø Value metres, the model measures Ø Limit metres.</summary>
		DiameterFromModel,
		/// <summary>No coplanar pair within Limit degrees; the closest-pair deviation Value is shared by Count pairs.</summary>
		PlaneTie,
		/// <summary>No coplanar pair within Limit degrees; the plane came from Member–Partner at Value degrees.</summary>
		PlanePair,
		/// <summary>No brace pair could be formed; the plane was fitted across all braces.</summary>
		PlaneNoPair,
		/// <summary>The K/Y/X classifier's shortcut: leftover Value (ratio) ≤ gate Limit (ratio) → pure K.</summary>
		KGateShortcut,
	}

	/// <summary>
	/// A message from the topology gates or the classifier, as DATA: what happened, to which member,
	/// with the measured value and the limit in SI. The sentence is composed by <see cref="Render"/>
	/// with whatever display settings the caller holds — at the moment it is shown, not when the
	/// topology was built.
	///
	/// These used to be finished strings, written by the builder through a static display setting.
	/// A reader who switched to inches then had every table in inches and the banner still saying
	/// "12.0 mm out of the joint plane" until the next run. The verdict never depended on the
	/// setting — every comparison is in millimetres and degrees — only the words did, so the words
	/// are what moved.
	/// </summary>
	/// <param name="Value">The measured quantity: metres, degrees or a ratio, by <paramref name="Kind"/>.</param>
	/// <param name="Limit">The bound it was compared with, in the same unit as <paramref name="Value"/>.</param>
	public sealed record GateMessage(
		GateKind Kind,
		string Member = "",
		string Partner = "",
		double Value = 0.0,
		double Limit = 0.0,
		int Count = 0,
		string? Text = null)
	{
		/// <summary>A unit-free sentence carried as is.</summary>
		public static GateMessage Plain(string text) => new(GateKind.Text, Text: text);

		/// <summary>The sentence, in the reader's units and precision. Null settings mean the defaults.</summary>
		public string Render(DisplaySettings? display = null)
		{
			var d = display ?? new DisplaySettings();
			var c = CultureInfo.InvariantCulture;
			string uL = QuantityFormat.LengthLabel(d.Length);
			string Len(double m) =>
				QuantityFormat.ToLength(m, d.Length).ToString("F" + d.SmallLengthDecimals, c) + " " + uL;
			string Dia(double m) =>
				QuantityFormat.ToLength(m, d.Length).ToString("F" + d.LengthDecimals, c);
			string Ang(double deg) => deg.ToString("F" + d.AngleDecimals, c);
			string Rat(double v) => v.ToString("F" + d.RatioDecimals, c);
			string Pct(double v, int? dp = null) => QuantityFormat.Percent(v, c, dp ?? d.PercentDecimals);

			return Kind switch
			{
				GateKind.Text => Text ?? "",

				// AN OVERLAP JOINT IS NOT OUTSIDE §6.4 — this tool does not implement it. §6.4.4 (N-004
				// Rev. 3 p. 33) says overlap joints "may be designed using the simple joint provision of
				// 6.4.3 with the following exemptions and additions" — shear parallel to the chord face
				// becomes a failure mode, §6.4.3.5 stops applying, and the through-brace force gains a
				// portion of the overlapping brace's. None of that is implemented here, and THAT is the
				// reason the joint is not checked. The message used to attribute our limit to the
				// standard ("out of 6.4 gap rules") while the report's own validity table printed
				// g/D ≥ −0.6 as satisfied on the same joint.
				GateKind.Overlap =>
					$"{Member}-{Partner}: feet overlap (gap {Len(Value)} < 0) — an overlap joint. "
					+ "§6.4.4 covers these with additions this tool does not implement (shear along "
					+ "the chord face, the through-brace force share), so it is not checked here.",

				// Measured from the plane through the CHORD AXIS — so the message says so. "out-of-plane
				// ecc. 40 mm" on a joint the engineer displaced as one rigid body was both wrong and
				// unactionable. Both numbers convert together: this tolerance is ours, not a clause
				// bound, so unlike the §6.4.1 gap there is no citation to keep in its original unit.
				GateKind.OutOfPlane =>
					$"{Member}: {Len(Value)} out of the joint plane through the chord (>{Len(Limit)}).",

				// D/4 comes from Figure 6-1, which dimensions the heavy-wall chord section as "D/4 or
				// Min.300mm" either side of the ECCENTRICITY it labels. The figure dimensions a joint
				// CAN, not an admissible eccentricity, so reading it as a limit on the offset is our
				// choice and the message says which part is which.
				GateKind.EccAlongChord =>
					$"{Member}: ecc. along chord e={Len(Value)} (>D/4={Len(Limit)} — tool tolerance, "
					+ "from the joint-can dimension in Figure 6-1, not a §6.4 limit).",

				GateKind.ThetaParallel => $"{Member}: θ={Ang(Value)}° — parallel to chord (degenerate).",
				GateKind.OffPlaneError =>
					$"{Member}: {Ang(Value)}° off plane (>{Limit.ToString("F0", c)}°) — different plane / multiplanar.",
				GateKind.OffPlaneWarn => $"{Member}: {Ang(Value)}° off plane (borderline).",
				GateKind.BetaOutside => $"{Member}: β={Rat(Value)} outside 0.2–1.0.",
				GateKind.ThetaOutside => $"{Member}: θ={Ang(Value)}° outside 30–90°.",
				GateKind.GammaOutside => $"γ={Rat(Value)} outside 10–50.",

				// "PIPE127STD" is really Ø141.3, because 127 is the nominal size: the name and the model
				// disagreeing by more than 2 % is worth saying, WITH a unit — two bare numbers left a
				// reader on inches with nothing to tell them these were millimetres.
				GateKind.DiameterFromModel =>
					$"{Member}: section name says Ø{Dia(Value)} {uL}, the model measures Ø{Dia(Limit)} {uL}.",

				GateKind.PlaneTie =>
					$"No two braces are coplanar within the {Limit.ToString("G", c)}° fit tolerance and the "
					+ $"closest-pair deviation ({Ang(Value)}°) is shared by {Count} pairs, so the plane is "
					+ "averaged across all braces. The 2D plane is only indicative — check the 3D view.",
				GateKind.PlanePair =>
					$"No two braces are coplanar within the {Limit.ToString("G", c)}° fit tolerance; "
					+ $"the joint plane was built from the closest pair {Member}-{Partner} "
					+ $"(mutual deviation {Ang(Value)}° > {Limit.ToString("G", c)}°). The 2D plane is only "
					+ "indicative — check the 3D view.",
				GateKind.PlaneNoPair =>
					"Could not form a brace pair for the joint plane; fitted across all braces — the 2D "
					+ "plane is only indicative.",

				// The gate itself is a round figure the user typed, so it keeps no decimals.
				GateKind.KGateShortcut =>
					$"balanced to {Pct(Value)} <= gate {Pct(Limit, 0)} -> 100 % K",

				_ => Text ?? Kind.ToString(),
			};
		}

		/// <summary>The default-settings sentence — for logs and tests, never for a page the user chose units on.</summary>
		public override string ToString() => Render();
	}

	public static class GateMessageExtensions
	{
		/// <summary>The messages as sentences under <paramref name="display"/>.</summary>
		public static IEnumerable<string> Texts(this IEnumerable<GateMessage> messages,
			DisplaySettings? display = null) => messages.Select(m => m.Render(display));
	}
}
