using System.Text.Json;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Parses the raw JSON string from GetRawJsonResultsAsync (CheckResultsData)
	/// into structured data we can feed into Norsok formula evaluations.
	///
	/// The raw JSON is a serialized CheckResultsData with dictionaries:
	///   Plates: { id -> PlateCheckResData }
	///   Welds:  { id -> WeldCheckResData }
	///   Bolts:  { id -> BoltCheckResData }
	///   PlatesInfo, WeldsInfo, BoltsInfo for material properties
	/// </summary>
	public class RawResultsParser
	{
		/// <summary>
		/// Parses the raw JSON and extracts plate, weld, and bolt check data.
		/// </summary>
		public static ParsedRawResults Parse(string rawJson)
		{
			var result = new ParsedRawResults();

			using var doc = JsonDocument.Parse(rawJson);
			var root = doc.RootElement;

			// Parse Plates
			if (root.TryGetProperty("Plates", out var plates))
			{
				foreach (var kvp in plates.EnumerateObject())
				{
					var p = kvp.Value;
					result.Plates.Add(new PlateData
					{
						Id = GetInt(p, "Id"),
						Name = GetString(p, "Name"),
						MaxStress = GetDouble(p, "MaxStress"),
						MaxStrain = GetDouble(p, "MaxStrain"),
						MaxUnityCheck = GetDouble(p, "MaxUnityCheck"),
						Thickness = GetDouble(p, "Thickness"),
						MaterialFy = GetDouble(p, "MaterialFy"),
						MaterialDesignFy = GetDouble(p, "MaterialDesignFy"),
						MaterialModulusOfElasticity = GetDouble(p, "MaterialModulusOfElasticity"),
						MaterialSafetyFactor = GetDouble(p, "MaterialSafetyFactor"),
						MaterialName = GetString(p, "MaterialName"),
						CheckStatus = GetBool(p, "CheckStatus"),
						LoadCaseId = GetInt(p, "LoadCaseId"),
					});
				}
			}

			// Parse Welds
			if (root.TryGetProperty("Welds", out var welds))
			{
				foreach (var kvp in welds.EnumerateObject())
				{
					var w = kvp.Value;
					result.Welds.Add(new WeldData
					{
						Id = GetInt(w, "Id"),
						Name = GetString(w, "Name"),
						MaxEquivalentStress = GetDouble(w, "MaxEquivalentStress"),
						UnityCheckWeld = GetDouble(w, "UnityCheckWeld"),
						UnityCheckBaseMetal = GetDouble(w, "UnityCheckBaseMetal"),
						MaxUnityCheck = GetDouble(w, "MaxUnityCheck"),
						Thickness = GetDouble(w, "Thickness"),
						DesignedThickness = GetDouble(w, "DesignedThickness"),
						Length = GetDouble(w, "Length"),
						MaterialFu = GetDouble(w, "MaterialFu"),
						BetaW = GetDouble(w, "BetaW"),
						GammaM2 = GetDouble(w, "GammaM2"),
						SigmaPerpendicular = GetDouble(w, "SigmaPerpendicular"),
						Tauy = GetDouble(w, "Tauy"),
						Taux = GetDouble(w, "Taux"),
						CheckStatus = GetBool(w, "CheckStatus"),
						LoadCaseId = GetInt(w, "LoadCaseId"),
					});
				}
			}

			// Parse Bolts
			if (root.TryGetProperty("Bolts", out var bolts))
			{
				foreach (var kvp in bolts.EnumerateObject())
				{
					var b = kvp.Value;
					result.Bolts.Add(new BoltData
					{
						Id = GetInt(b, "Id"),
						Name = GetString(b, "Name"),
						BoltTensionForce = GetDouble(b, "BoltTensionForce"),
						BoltShearForce = GetDouble(b, "BoltShearForce"),
						BoltTensionResistance = GetDouble(b, "BoltTensionResistance"),
						BoltShearResistance = GetDouble(b, "BoltShearResistance"),
						UnityCheckTension = GetDouble(b, "UnityCheckTension"),
						UnityCheckShear = GetDouble(b, "UnityCheckShear"),
						InteractionTensionShear = GetDouble(b, "InteractionTensionShear"),
						MaxUnityCheck = GetDouble(b, "MaxUnityCheck"),
						BoltAssemblyName = GetString(b, "BoltAssemblyName"),
						CheckStatus = GetBool(b, "CheckStatus"),
						LoadCaseId = GetInt(b, "LoadCaseId"),
					});
				}
			}

			return result;
		}

		private static double GetDouble(JsonElement el, string prop)
		{
			if (el.TryGetProperty(prop, out var val) && val.ValueKind == JsonValueKind.Number)
				return val.GetDouble();
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

	// ── Parsed data models ──

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
		public double MaxStress { get; set; }
		public double MaxStrain { get; set; }
		public double MaxUnityCheck { get; set; }
		public double Thickness { get; set; }
		public double MaterialFy { get; set; }
		public double MaterialDesignFy { get; set; }
		public double MaterialModulusOfElasticity { get; set; }
		public double MaterialSafetyFactor { get; set; }
		public string MaterialName { get; set; } = string.Empty;
		public bool CheckStatus { get; set; }
		public int LoadCaseId { get; set; }
	}

	public class WeldData
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public double MaxEquivalentStress { get; set; }
		public double UnityCheckWeld { get; set; }
		public double UnityCheckBaseMetal { get; set; }
		public double MaxUnityCheck { get; set; }
		public double Thickness { get; set; }
		public double DesignedThickness { get; set; }
		public double Length { get; set; }
		public double MaterialFu { get; set; }
		public double BetaW { get; set; }
		public double GammaM2 { get; set; }
		public double SigmaPerpendicular { get; set; }
		public double Tauy { get; set; }
		public double Taux { get; set; }
		public bool CheckStatus { get; set; }
		public int LoadCaseId { get; set; }
	}

	public class BoltData
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public double BoltTensionForce { get; set; }
		public double BoltShearForce { get; set; }
		public double BoltTensionResistance { get; set; }
		public double BoltShearResistance { get; set; }
		public double UnityCheckTension { get; set; }
		public double UnityCheckShear { get; set; }
		public double InteractionTensionShear { get; set; }
		public double MaxUnityCheck { get; set; }
		public string BoltAssemblyName { get; set; } = string.Empty;
		public bool CheckStatus { get; set; }
		public int LoadCaseId { get; set; }
	}
}
