using System.Text.Json;
using System.Text.RegularExpressions;
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
				// The API returns List<Object> which are JSON elements
				string json = cssObj is JsonElement je ? je.GetRawText() : cssObj.ToString() ?? "";

				// Try to extract name from JSON
				string name = "";
				if (cssObj is JsonElement elem && elem.ValueKind == JsonValueKind.Object)
				{
					if (elem.TryGetProperty("mprlName", out var mprlName))
						name = mprlName.GetString() ?? "";
					else if (elem.TryGetProperty("MprlName", out var mprlName2))
						name = mprlName2.GetString() ?? "";
					else if (elem.TryGetProperty("name", out var nameEl))
						name = nameEl.GetString() ?? "";
				}

				if (string.IsNullOrEmpty(name))
					name = json.Length > 100 ? json[..100] : json;

				var result = new DetectedCrossSection { Name = name };

				// Check if CHS
				bool isChs = ChsIndicators.Any(indicator =>
					name.IndexOf(indicator, StringComparison.OrdinalIgnoreCase) >= 0);

				if (isChs)
				{
					result.IsCHS = true;
					result.ShapeType = "CHS";

					// Parse dimensions
					var match = ChsDimensionRegex.Match(name);
					if (match.Success)
					{
						result.Diameter = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
						result.Thickness = double.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
					}
				}
				else if (name.IndexOf("RHS", StringComparison.OrdinalIgnoreCase) >= 0 ||
						 name.IndexOf("SHS", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					result.ShapeType = "RHS";
				}
				else if (name.IndexOf("HE", StringComparison.OrdinalIgnoreCase) >= 0 ||
						 name.IndexOf("IPE", StringComparison.OrdinalIgnoreCase) >= 0 ||
						 name.IndexOf("W ", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					result.ShapeType = "I-section";
				}
				else
				{
					result.ShapeType = "Other";
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
