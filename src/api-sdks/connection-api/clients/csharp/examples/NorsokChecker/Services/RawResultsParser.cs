using System.Text.Json;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Parses the raw JSON string from GetRawJsonResultsAsync (CheckResultsData).
	///
	/// IMPORTANT: The raw JSON uses:
	///   - camelCase property names (e.g. "maxStress", "materialFy")
	///   - SI base units: Pa for stress, m for lengths, N for forces
	///
	/// This parser converts everything to engineering units:
	///   - Stress: Pa → MPa (÷ 1e6)
	///   - Length: m → mm (× 1000)
	///   - Force: N → kN (÷ 1000)
	///   - Modulus: Pa → MPa (÷ 1e6)
	/// </summary>
	public class RawResultsParser
	{
		public static ParsedRawResults Parse(string rawJson)
		{
			var result = new ParsedRawResults();

			using var doc = JsonDocument.Parse(rawJson);
			var root = doc.RootElement;

			// Parse Plates — JSON key is "plates" (lowercase)
			if (root.TryGetProperty("plates", out var plates))
			{
				foreach (var kvp in plates.EnumerateObject())
				{
					var p = kvp.Value;
					result.Plates.Add(new PlateData
					{
						Id = GetInt(p, "id"),
						Name = GetString(p, "name"),
						MaxStress = GetDouble(p, "maxStress") / 1e6,             // Pa → MPa
						MaxStrain = GetDouble(p, "maxStrain"),
						MaxUnityCheck = GetDouble(p, "maxUnityCheck"),
						Thickness = GetDouble(p, "thickness") * 1000.0,           // m → mm
						MaterialFy = GetDouble(p, "materialFy") / 1e6,            // Pa → MPa
						MaterialDesignFy = GetDouble(p, "materialDesignFy") / 1e6, // Pa → MPa
						MaterialModulusOfElasticity = GetDouble(p, "materialModulusOfElasticity") / 1e6, // Pa → MPa
						MaterialSafetyFactor = GetDouble(p, "materialSafetyFactor"),
						MaterialName = GetString(p, "materialName"),
						CheckStatus = GetBool(p, "checkStatus"),
						LoadCaseId = GetInt(p, "loadCaseId"),
					});
				}
			}

			// Parse Welds — JSON key is "welds" (lowercase)
			if (root.TryGetProperty("welds", out var welds))
			{
				foreach (var kvp in welds.EnumerateObject())
				{
					var w = kvp.Value;
					result.Welds.Add(new WeldData
					{
						Id = GetInt(w, "id"),
						Name = GetString(w, "name"),
						MaxEquivalentStress = GetDouble(w, "maxEquivalentStress") / 1e6, // Pa → MPa
						UnityCheckWeld = GetDouble(w, "unityCheckWeld"),
						UnityCheckBaseMetal = GetDouble(w, "unityCheckBaseMetal"),
						MaxUnityCheck = GetDouble(w, "maxUnityCheck"),
						Thickness = GetDouble(w, "thickness") * 1000.0,                  // m → mm
						DesignedThickness = GetDouble(w, "designedThickness") * 1000.0,  // m → mm
						Length = GetDouble(w, "length") * 1000.0,                         // m → mm
						MaterialFu = GetDouble(w, "materialFu") / 1e6,                   // Pa → MPa
						BetaW = GetDouble(w, "betaW"),
						GammaM2 = GetDouble(w, "gammaM2"),
						SigmaPerpendicular = GetDouble(w, "sigmaPerpendicular") / 1e6,   // Pa → MPa
						Tauy = GetDouble(w, "tauy") / 1e6,                               // Pa → MPa
						Taux = GetDouble(w, "taux") / 1e6,                               // Pa → MPa
						CheckStatus = GetBool(w, "checkStatus"),
						LoadCaseId = GetInt(w, "loadCaseId"),
					});
				}
			}

			// Parse Bolts — JSON key is "bolts" (lowercase)
			if (root.TryGetProperty("bolts", out var bolts))
			{
				foreach (var kvp in bolts.EnumerateObject())
				{
					var b = kvp.Value;
					result.Bolts.Add(new BoltData
					{
						Id = GetInt(b, "id"),
						Name = GetString(b, "name"),
						BoltTensionForce = GetDouble(b, "boltTensionForce") / 1000.0,       // N → kN
						BoltShearForce = GetDouble(b, "boltShearForce") / 1000.0,            // N → kN
						BoltTensionResistance = GetDouble(b, "boltTensionResistance") / 1000.0, // N → kN
						BoltShearResistance = GetDouble(b, "boltShearResistance") / 1000.0,  // N → kN
						UnityCheckTension = GetDouble(b, "unityCheckTension"),
						UnityCheckShear = GetDouble(b, "unityCheckShear"),
						InteractionTensionShear = GetDouble(b, "interactionTensionShear"),
						MaxUnityCheck = GetDouble(b, "maxUnityCheck"),
						BoltAssemblyName = GetString(b, "boltAssemblyName"),
						CheckStatus = GetBool(b, "checkStatus"),
						LoadCaseId = GetInt(b, "loadCaseId"),
					});
				}
			}

			return result;
		}

		private static double GetDouble(JsonElement el, string prop)
		{
			if (el.TryGetProperty(prop, out var val))
			{
				if (val.ValueKind == JsonValueKind.Number)
					return val.GetDouble();
				// Handle "NaN" strings
				if (val.ValueKind == JsonValueKind.String && val.GetString() == "NaN")
					return 0;
			}
			return 0;
		}

		private static int GetInt(JsonElement el, string prop)
		{
			if (el.TryGetProperty(prop, out var val) && val.ValueKind == JsonValueKind.Number)
				return val.GetInt32();
			return 0;
		}

		private static string GetString(JsonElement el, string prop)
		{
			if (el.TryGetProperty(prop, out var val) && val.ValueKind == JsonValueKind.String)
				return val.GetString() ?? string.Empty;
			return string.Empty;
		}

		private static bool GetBool(JsonElement el, string prop)
		{
			if (el.TryGetProperty(prop, out var val))
			{
				if (val.ValueKind == JsonValueKind.True) return true;
				if (val.ValueKind == JsonValueKind.False) return false;
			}
			return false;
		}
	}

	// ── Parsed data models (all values in engineering units: MPa, mm, kN) ──

	public class ParsedRawResults
	{
		public List<PlateData> Plates { get; set; } = new();
		public List<WeldData> Welds { get; set; } = new();
		public List<BoltData> Bolts { get; set; } = new();
	}

	public class PlateData
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public double MaxStress { get; set; }              // MPa
		public double MaxStrain { get; set; }
		public double MaxUnityCheck { get; set; }
		public double Thickness { get; set; }              // mm
		public double MaterialFy { get; set; }             // MPa
		public double MaterialDesignFy { get; set; }       // MPa
		public double MaterialModulusOfElasticity { get; set; } // MPa
		public double MaterialSafetyFactor { get; set; }
		public string MaterialName { get; set; } = string.Empty;
		public bool CheckStatus { get; set; }
		public int LoadCaseId { get; set; }
	}

	public class WeldData
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public double MaxEquivalentStress { get; set; }    // MPa
		public double UnityCheckWeld { get; set; }
		public double UnityCheckBaseMetal { get; set; }
		public double MaxUnityCheck { get; set; }
		public double Thickness { get; set; }              // mm
		public double DesignedThickness { get; set; }      // mm
		public double Length { get; set; }                  // mm
		public double MaterialFu { get; set; }             // MPa
		public double BetaW { get; set; }
		public double GammaM2 { get; set; }
		public double SigmaPerpendicular { get; set; }     // MPa
		public double Tauy { get; set; }                   // MPa
		public double Taux { get; set; }                   // MPa
		public bool CheckStatus { get; set; }
		public int LoadCaseId { get; set; }
	}

	public class BoltData
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public double BoltTensionForce { get; set; }       // kN
		public double BoltShearForce { get; set; }         // kN
		public double BoltTensionResistance { get; set; }  // kN
		public double BoltShearResistance { get; set; }    // kN
		public double UnityCheckTension { get; set; }
		public double UnityCheckShear { get; set; }
		public double InteractionTensionShear { get; set; }
		public double MaxUnityCheck { get; set; }
		public string BoltAssemblyName { get; set; } = string.Empty;
		public bool CheckStatus { get; set; }
		public int LoadCaseId { get; set; }
	}
}
