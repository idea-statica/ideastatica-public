using System.Text.RegularExpressions;
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
		// tolerant CHS dimension fallback: "CHS 457/16", "CHS457x16", "RO 323.9/10"...
		private static readonly Regex DimRegex = new(@"(\d+\.?\d*)\s*[/x×]\s*(\d+\.?\d*)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly HashSet<CrossSectionType> ChsTypes = new()
		{
			CrossSectionType.RolledCHS, CrossSectionType.CHSPar, CrossSectionType.CHSg,
			CrossSectionType.O, CrossSectionType.Oval,
		};

		public static Dictionary<int, JointSectionInfo> FromCrossSections(IEnumerable<object> crossSections)
		{
			var map = new Dictionary<int, JointSectionInfo>();
			foreach (var obj in crossSections)
			{
				if (obj is not CrossSectionParameter cs) continue;
				string? name = cs.Name;
				var (d, t) = JointSectionInfo.ParseChs(name);
				bool isChs = d != null;
				if (d == null && name != null && ChsTypes.Contains(cs.CrossSectionType))
				{
					// name isn't the strict "CHSd/t" convention but the type says CHS → tolerant parse
					var m = DimRegex.Match(name);
					if (m.Success)
					{
						d = double.Parse(m.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
						t = double.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
						isChs = true;
					}
				}

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
