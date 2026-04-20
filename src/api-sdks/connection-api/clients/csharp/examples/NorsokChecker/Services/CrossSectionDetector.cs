using System.Text.Json;
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

		private DetectedCrossSection? ParseCrossSection(object cssObj)
		{
			try
			{
				var result = new DetectedCrossSection();

				// The API returns CrossSectionParameter objects directly
				if (cssObj is CrossSectionParameter cssPar)
				{
					result.Name = cssPar.Name ?? cssPar.CrossSectionType.ToString();
					_log($"      CSS: '{result.Name}' type={cssPar.CrossSectionType}");

					// Detect shape from CrossSectionType enum
					switch (cssPar.CrossSectionType)
					{
						case CrossSectionType.RolledCHS:
						case CrossSectionType.CHSPar:
						case CrossSectionType.CHSg:
						case CrossSectionType.O:
						case CrossSectionType.Oval:
						case CrossSectionType.CFRegPolygon:
							result.IsCHS = true;
							result.ShapeType = "CHS";
							// Extract D and t from parameters
							foreach (var p in cssPar.Parameters)
							{
								if (p is not ParameterDouble pd) continue;
								var pName = p.Name?.ToUpperInvariant() ?? "";
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
							result.ShapeType = "I-section";
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
		public string Name { get; set; } = string.Empty;
		public string ShapeType { get; set; } = "Other";
		public bool IsCHS { get; set; }
		public double Diameter { get; set; }
		public double Thickness { get; set; }
	}
}
