using System.Text.RegularExpressions;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.ConnectionApi;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Detects cross-section shapes from the Material API and parses
	/// CHS (Circular Hollow Section) dimensions from profile names.
	///
	/// Profile naming patterns for CHS:
	///   "CHS 500/20", "CHS500x20", "O 508/12.7", "RO 323.9/10"
	///   "PIPE 508x12.7", "TUB 500x20"
	/// </summary>
	public class CrossSectionDetector
	{
		private readonly IConnectionApiClient _client;
		private readonly Action<string> _log;

		// Patterns that indicate a circular hollow section
		private static readonly string[] ChsIndicators = { "CHS", "RO ", "PIPE", "TUB", "O ", "Circular" };

		// Regex to extract diameter and thickness: "CHS 500/20" or "CHS500x20"
		private static readonly Regex ChsDimensionRegex = new(
			@"(\d+\.?\d*)\s*[/x×]\s*(\d+\.?\d*)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public CrossSectionDetector(IConnectionApiClient client, Action<string> log)
		{
			_client = client;
			_log = log;
		}

		/// <summary>
		/// Read all cross-sections from the project and detect which are CHS.
		/// Returns list of detected profiles with parsed D and t.
		/// </summary>
		public async Task<List<DetectedCrossSection>> DetectAsync(Guid projectId, CancellationToken ct = default)
		{
			var results = new List<DetectedCrossSection>();

			try
			{
				var crossSections = await _client.Material.GetCrossSectionsAsync(projectId, cancellationToken: ct);
				_log($"    Cross-sections in project: {crossSections.Count}");

				foreach (var cssObj in crossSections)
				{
					var css = ParseCrossSection(cssObj);
					if (css != null)
					{
						results.Add(css);
						string shapeStr = css.IsCHS ? $"CHS D={css.Diameter:F1}mm t={css.Thickness:F1}mm" : css.ShapeType;
						_log($"      [{css.Name}] → {shapeStr}");
					}
				}
			}
			catch (Exception ex)
			{
				_log($"    WARNING: Could not read cross-sections: {ex.Message}");
			}

			return results;
		}

		/// <summary>Read a double property by name — the steel material subclasses differ per code.</summary>
		private static double? ReadDouble(object obj, string prop)
		{
			var p = obj.GetType().GetProperty(prop);
			if (p == null || p.PropertyType != typeof(double)) return null;
			double v = (double)p.GetValue(obj)!;
			return v > 0 ? v : null;
		}

		private DetectedCrossSection? ParseCrossSection(object cssObj)
		{
			try
			{
				var result = new DetectedCrossSection();

				// The API returns CrossSectionParameter objects directly
				if (cssObj is CrossSectionParameter cssPar)
				{
					result.Id = cssPar.Id;
					result.Name = cssPar.Name ?? cssPar.CrossSectionType.ToString();

					_log($"      CSS id={result.Id}: '{result.Name}' type={cssPar.CrossSectionType}");

					// Detect shape from CrossSectionType enum. The tubular cases below must stay in
					// step with JointSectionMap.ChsTypes, which is what the §6.4 path gates on —
					// this list used to include CFRegPolygon and that one did not, so a section
					// could be tubular for one path and not the other. A regular polygon is not a
					// circular tube and §6.4 does not address it, so it is out of both.
					switch (cssPar.CrossSectionType)
					{
						case CrossSectionType.RolledCHS:
						case CrossSectionType.CHSPar:
						case CrossSectionType.CHSg:
						case CrossSectionType.O:
						case CrossSectionType.Oval:
							result.IsCHS = true;
							result.ShapeType = "CHS";
							// Extract D and t from parameters
							foreach (var p in cssPar.Parameters)
							{
								if (p is not ParameterDouble pd) continue;
								var pName = p.Name?.ToUpperInvariant() ?? "";
								result.AllParams[p.Name ?? ""] = pd.Value;
								_log($"        param: {p.Name} = {pd.Value}");
								if (pName == "D" || pName == "DIAMETER")
									result.Diameter = pd.Value;
								else if (pName == "R" || pName == "RADIUS")
									result.Diameter = pd.Value * 2;
								else if (pName == "T" || pName == "THICKNESS" || pName == "T1")
									result.Thickness = pd.Value;
							}
							// If dimensions in meters, convert to mm
							if (result.Diameter > 0 && result.Diameter < 10)
								result.Diameter *= 1000;
							if (result.Thickness > 0 && result.Thickness < 1)
								result.Thickness *= 1000;
							_log($"      → CHS D={result.Diameter:F1}mm t={result.Thickness:F1}mm");
							break;

						case CrossSectionType.RolledRHS:
						case CrossSectionType.RHSg:
						case CrossSectionType.CFRhs:
							result.ShapeType = "RHS";
							break;

						case CrossSectionType.RolledI:
						case CrossSectionType.RolledIPar:
						case CrossSectionType.Iw:
						case CrossSectionType.Iwn:
						case CrossSectionType.Ign:
						case CrossSectionType.Igh:
						case CrossSectionType.BeamShapeIHaunchChamfer:
						case CrossSectionType.BeamShapeIHaunchChamferAssym:
						case CrossSectionType.BeamShapeIrevDegen:
						case CrossSectionType.BeamShapeIrevDegenAdd:
							result.ShapeType = "I-section";
							foreach (var p in cssPar.Parameters)
							{
								if (p is not ParameterDouble pd) continue;
								result.AllParams[p.Name ?? ""] = pd.Value;
								_log($"        param: {p.Name} = {pd.Value}");
							}
							// Height: typically first large parameter, or named H/h
							// For rolled sections (HEA, HEB, IPE) parameters may just be positional
							if (cssPar.Parameters.Count > 0)
							{
								var doubles = cssPar.Parameters.OfType<ParameterDouble>().ToList();
								// Try named parameters first
								var hParam = doubles.FirstOrDefault(p => p.Name?.ToUpperInvariant() is "H" or "HEIGHT");
								var twParam = doubles.FirstOrDefault(p => p.Name?.ToUpperInvariant() is "TW" or "WEBTHICKNESS" or "S");
								// Fallback: for rolled sections, first param is typically height
								if (hParam == null && doubles.Count > 0)
									hParam = doubles[0]; // first param = height
								if (twParam == null && doubles.Count > 2)
									twParam = doubles[2]; // third param often = web thickness
								if (hParam != null)
									result.Diameter = hParam.Value < 10 ? hParam.Value * 1000 : hParam.Value;
								if (twParam != null)
									result.Thickness = twParam.Value < 1 ? twParam.Value * 1000 : twParam.Value;
							}
							break;

						case CrossSectionType.RolledU:
						case CrossSectionType.RolledUPar:
							result.ShapeType = "Channel";
							break;

						case CrossSectionType.RolledAngle:
						case CrossSectionType.RolledLPar:
							result.ShapeType = "Angle";
							break;

						default:
							result.ShapeType = "Other";
							break;
					}

					// Also try to parse dimensions from the name string
					if (result.IsCHS && result.Diameter == 0 && !string.IsNullOrEmpty(result.Name))
					{
						var match = ChsDimensionRegex.Match(result.Name);
						if (match.Success)
						{
							result.Diameter = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
							result.Thickness = double.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
						}
					}

					// Material off the cross-section itself, so the grid can show it without a
					// calculation — it used to come only from the raw results' plates, i.e. never
					// before the first run. Same source and the same >40 mm fy40 band as
					// JointSectionMap uses, so the two cannot report different steel. Read after the
					// dimensions above, because the band depends on the wall thickness.
					var matEl = cssPar.Material?.Element;
					if (matEl != null)
					{
						result.MaterialName = (matEl as IdeaRS.OpenModel.Material.Material)?.Name ?? "";
						double? fy = ReadDouble(matEl, "fy");
						double? fyThick = ReadDouble(matEl, "fy40");
						if (result.Thickness > 40.0 && fyThick is > 0) fy = fyThick;
						if (fy is > 0) result.Fy = fy.Value / 1e6;      // Pa -> MPa
					}

					return result;
				}

				// Fallback: try name-based detection for other object types
				string name = cssObj?.ToString() ?? "";
				_log($"      CSS raw: '{name}' (type: {cssObj?.GetType().Name})");
				result.Name = name;
				result.ShapeType = "Other";

				bool isChs = ChsIndicators.Any(ind => name.IndexOf(ind, StringComparison.OrdinalIgnoreCase) >= 0);
				if (isChs)
				{
					result.IsCHS = true;
					result.ShapeType = "CHS";
					var match = ChsDimensionRegex.Match(name);
					if (match.Success)
					{
						result.Diameter = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
						result.Thickness = double.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
					}
				}

				return result;
			}
			catch
			{
				return null;
			}
		}
	}

	public class DetectedCrossSection
	{
		/// <summary>
		/// The cross-section's own id, so a member can be matched to ITS section via
		/// ConMember.CrossSectionId. Without this the caller had nothing to match on and fell back
		/// to sorting the project's sections by diameter, which assigned the wrong profile to every
		/// member but one.
		/// </summary>
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string ShapeType { get; set; } = "Other";
		public bool IsCHS { get; set; }
		/// <summary>CHS: outside diameter [mm]. I: height h [mm].</summary>
		public double Diameter { get; set; }
		/// <summary>CHS: wall thickness [mm]. I: web thickness [mm].</summary>
		public double Thickness { get; set; }
		/// <summary>All parameters from the cross-section</summary>
		public Dictionary<string, double> AllParams { get; set; } = new();
		/// <summary>Steel grade name, e.g. "S 355" — available without a calculation.</summary>
		public string MaterialName { get; set; } = string.Empty;
		/// <summary>Yield strength [MPa], fy40 band applied for a wall over 40 mm. 0 when unknown.</summary>
		public double Fy { get; set; }
	}
}
