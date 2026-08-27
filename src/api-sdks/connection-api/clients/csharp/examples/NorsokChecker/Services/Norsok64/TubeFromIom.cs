using IdeaRS.OpenModel.Connection;
// NorsokChecker.Services also declares a PlateData (the raw-results one), so the IOM types are
// aliased here — an unqualified PlateData in this file would silently be the wrong type.
using IomBeam = IdeaRS.OpenModel.Connection.BeamData;
using IomPlate = IdeaRS.OpenModel.Connection.PlateData;

namespace NorsokChecker.Services.Norsok64
{
	/// <summary>
	/// Reads a tube's D and T from the connection's own IOM model instead of parsing them out of
	/// the section name. Port of extract.py tube_from_iom_beam.
	///
	/// Why: the name is not a reliable source. Measured against the Eurocode cross-section library,
	/// a "CHS&lt;D&gt;/&lt;T&gt;" parse rejects 2641 of 2760 circular profiles (96 %) — RO323.9X12.5,
	/// MSRR101.6x10.0, bare 76.0x3.5, GB-SSP42X2.5, every ASME PIPE...SCH40 — and it can be
	/// confidently WRONG: PIPE127STD is really D = 141.3 mm, because 127 is the nominal size.
	/// The parametric route is no better for catalogue sections: a rolledCHS carries exactly one
	/// parameter, UniqueName, so there is no D/T to read.
	///
	/// IDEA models a tube as n flat facets on the wall MID-surface. Facet origins therefore sit on
	/// the mid-surface circle (diameter D-T), and the largest distance between two of them is that
	/// circle's inscribed-polygon chord, short of the circle by cos(pi/n):
	///     D = maxdist / cos(pi/n) + T
	/// Measured worst error 0.4 %, identical at 24 / 64 / 96 facet divisions (the project's
	/// DivisionOfSurfaceOfBiggestCircularHollowMember setting), and unaffected by cuts.
	/// </summary>
	public static class TubeFromIom
	{
		/// <summary>
		/// Fewer facets than this is not a modelled tube wall. Measured: IDEA facets a tube 16..96
		/// times depending on the project setting, while non-round shapes give 2 (angle) or 3
		/// (I, U). 8 separates them with room to spare.
		/// </summary>
		public const int MinTubeFacets = 8;

		/// <summary>
		/// (D, T) in mm from one IOM beam, or (null, null) with the reason when it cannot be read.
		///
		/// Only call this for a beam whose cross-section type is tubular — an I-section yields 3
		/// facets and two distinct thicknesses, and the arithmetic below would happily turn them
		/// into a plausible-looking number.
		/// </summary>
		public static (double? D, double? T, string? Why) FromBeam(IomBeam? beam)
		{
			var facets = (beam?.Plates is { } plates ? plates : new List<IomPlate>())
				.Where(p => !p.IsNegativeObject)
				.ToList();

			int n = facets.Count;
			if (n < MinTubeFacets)
				return (null, null, $"only {n} facet(s) in the model — not a modelled tube wall");

			// one wall thickness, or it is not a tube (an I-section gives web + flange)
			var thicknesses = facets
				.Select(p => Math.Round(p.Thickness, 6))
				.Where(t => t > 0)
				.Distinct()
				.ToList();
			if (thicknesses.Count != 1)
				return (null, null, thicknesses.Count == 0
					? "facets carry no thickness"
					: $"{thicknesses.Count} distinct facet thicknesses — not a uniform tube wall");

			var origins = facets.Where(p => p.Origin != null).Select(p => p.Origin).ToList();
			if (origins.Count < MinTubeFacets)
				return (null, null, "facets carry no origin");

			// IOM lengths are metres; the app works in mm
			double t = thicknesses[0] * 1000.0;
			double maxDistSq = 0;
			for (int i = 0; i < origins.Count; i++)
				for (int j = i + 1; j < origins.Count; j++)
				{
					double dx = origins[i].X - origins[j].X;
					double dy = origins[i].Y - origins[j].Y;
					double dz = origins[i].Z - origins[j].Z;
					double d2 = dx * dx + dy * dy + dz * dz;
					if (d2 > maxDistSq) maxDistSq = d2;
				}

			double maxDist = Math.Sqrt(maxDistSq) * 1000.0;
			if (maxDist <= 0)
				return (null, null, "all facet origins coincide");

			double d = maxDist / Math.Cos(Math.PI / n) + t;

			// Plausibility, as extract.py:167-168 has it. No section in the library is anywhere near
			// these bounds, so this never fires on real input — it is here because the alternative to
			// rejecting an impossible tube is REPORTING one, and a number the caller cannot tell from
			// a good one is worse than an admitted failure. A wall at or over half the diameter is not
			// a tube at all, and it would make beta and gamma meaningless rather than merely wrong.
			if (d < 10.0 || d > 5000.0 || t >= d / 2.0)
				return (null, null, $"implausible geometry (D={d:F1} mm, T={t:F1} mm)");

			return (d, t, null);
		}

		/// <summary>
		/// {beam name → its BeamData} for the tubular beams of one connection, so callers can look
		/// a member up by name. Non-tubular beams are left out deliberately: the facet formula must
		/// never run on them.
		/// </summary>
		public static Dictionary<string, IomBeam> TubularBeamsByName(ConnectionData? iom)
		{
			var map = new Dictionary<string, IomBeam>();
			foreach (var b in iom?.Beams ?? new List<IomBeam>())
			{
				if (string.IsNullOrEmpty(b.Name) || b.IsNegativeObject) continue;
				if (!JointSectionMap.IsTubularTypeName(b.CrossSectionType)) continue;
				map[b.Name!] = b;
			}
			return map;
		}
	}
}
