using IdeaRS.OpenModel.CrossSection;

namespace NorsokChecker.Services.Norsok64
{
	/// <summary>
	/// Builds {crossSectionId → JointSectionInfo} from the Material API cross-sections —
	/// port of extract.py xs_map. D/T parsed from the section name (mm); fy/fu read from the
	/// inline material element (already Pa); fy40/fu40 band applied for wall thickness &gt; 40 mm.
	/// </summary>
	public static class JointSectionMap
	{
		/// <summary>
		/// The one definition of "tubular" in this app. CrossSectionDetector used to keep a second,
		/// slightly different set (it also listed CFRegPolygon), which meant a section could be
		/// tubular for one code path and not the other.
		/// </summary>
		public static readonly HashSet<CrossSectionType> ChsTypes = new()
		{
			CrossSectionType.RolledCHS, CrossSectionType.CHSPar, CrossSectionType.CHSg,
			CrossSectionType.O, CrossSectionType.Oval,
		};

		/// <summary>
		/// Same test against the type NAME, for the IOM payload — BeamData.CrossSectionType is a
		/// string there, not the enum.
		/// </summary>
		public static bool IsTubularTypeName(string? typeName)
		{
			if (string.IsNullOrWhiteSpace(typeName)) return false;
			return ChsTypes.Any(t =>
				string.Equals(t.ToString(), typeName!.Trim(), StringComparison.OrdinalIgnoreCase));
		}

		public static Dictionary<int, JointSectionInfo> FromCrossSections(IEnumerable<object> crossSections)
		{
			var map = new Dictionary<int, JointSectionInfo>();
			foreach (var obj in crossSections)
			{
				if (obj is not CrossSectionParameter cs) continue;
				string? name = cs.Name;

				// PRIMARY: real numbers from the cross-section parameters (naming-independent).
				// D/T come as ParameterDouble ("D"/"Diameter"/"R", "T"/"Thickness"/"T1"), in metres
				// for parametric sections — convert to mm like CrossSectionDetector does.
				double? d = null, t = null;
				bool isChs = ChsTypes.Contains(cs.CrossSectionType);
				if (isChs && cs.Parameters != null)
				{
					foreach (var p in cs.Parameters)
					{
						if (p is not ParameterDouble pd) continue;
						switch (p.Name?.ToUpperInvariant())
						{
							case "D" or "DIAMETER": d = pd.Value; break;
							case "R" or "RADIUS": d = pd.Value * 2; break;
							case "T" or "THICKNESS" or "T1": t = pd.Value; break;
						}
					}
					if (d is > 0 and < 10) d *= 1000;   // m → mm
					if (t is > 0 and < 1) t *= 1000;
				}

				// FALLBACK: parse the section name (also covers CHS-named sections whose type
				// enum isn't in ChsTypes). Requires the name to actually say "CHS" — see below.
				if (d is not > 0 || t is not > 0)
				{
					var (nd, nt) = JointSectionInfo.ParseChs(name);
					if (nd != null) { d = nd; t = nt; isChs = true; }
				}
				// There used to be a SECOND fallback here: any "<number><separator><number>" in the
				// name of a tubular section, with no requirement that the name say "CHS" at all. It
				// is gone, and the python reference never had it (xs_map calls only parse_chs).
				//
				// It did more harm than good. On 'PIPE(Imp)3-1/2XS' it matched the FRACTION in the
				// name and returned D = 1 mm, T = 2 mm for a Ø102/8.3 tube — dimensions that are not
				// merely imprecise but physically impossible, and that passed every downstream test
				// for "dimensions are known". Two consequences, measured on CON1 of test_cs:
				//   - the IOM refinement overwrote them with the real 102.0/8.3 and then, comparing
				//     the name against the model, reported a 2 % disagreement as an assumption the
				//     user had to read — a warning the python app does not raise, because it never
				//     invented the 1 mm in the first place;
				//   - had the IOM read failed for any reason, the 1 mm tube would have been checked
				//     as valid data (beta = 1/141 = 0.007) instead of being reported as unreadable.
				// A name that does not spell out its dimensions now yields nothing, and the model
				// geometry is the only source - which is the honest answer.
				// IsCHS says only that the TYPE is tubular. It used to also require D and T, so a
				// tube whose dimensions could not be read was reported as "not CHS" — a statement
				// contradicted by its own name. The two are separate answers; see
				// JointSectionInfo.RejectReason.

				// material inline on the cross-section: fy/fu already in Pa; >40 mm → fy40/fu40 band
				double? fy = null, fu = null;
				string? matName = null;
				var el = cs.Material?.Element;
				if (el != null)
				{
					matName = (el as IdeaRS.OpenModel.Material.Material)?.Name;
					double? fyThin = GetDouble(el, "fy"), fuThin = GetDouble(el, "fu");
					double? fyThick = GetDouble(el, "fy40"), fuThick = GetDouble(el, "fu40");
					bool useThick = t is > 40.0 && fyThick != null;
					fy = useThick ? fyThick : fyThin;
					fu = useThick ? fuThick : fuThin;
				}

				map[cs.Id] = new JointSectionInfo
				{
					Name = name, D = d, T = t, IsCHS = isChs,
					TypeName = cs.CrossSectionType.ToString(),
					Fy = fy, Fu = fu, MaterialName = matName,
				};
			}
			return map;
		}

		/// <summary>Read a double property by name (the steel material subclasses differ per code).</summary>
		private static double? GetDouble(object obj, string prop)
		{
			var p = obj.GetType().GetProperty(prop);
			if (p == null || p.PropertyType != typeof(double)) return null;
			double v = (double)p.GetValue(obj)!;
			return v > 0 ? v : null;
		}
	}
}
