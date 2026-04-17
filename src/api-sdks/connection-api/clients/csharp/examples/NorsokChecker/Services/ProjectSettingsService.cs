using IdeaStatiCa.ConnectionApi;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Reads and updates project settings (code factors) via the Connection API.
	/// Sets γM values to match NORSOK N-004 Table 6-1.
	/// </summary>
	public class ProjectSettingsService
	{
		private readonly IConnectionApiClient _client;
		private readonly Action<string> _log;

		// NORSOK N-004 material factors (Table 6-1, §6.3.7)
		public const double GammaM0_Norsok = 1.15;
		public const double GammaM1_Norsok = 1.15;
		public const double GammaM2_Norsok = 1.25;

		// Known possible key patterns for gamma factors in IDEA StatiCa settings.
		// The actual key depends on the API version / design code.
		private static readonly (string[] keys, double norsokValue, string label)[] GammaFactors =
		{
			(new[] { "GammaM0", "gammaM0", "γ M0", "yM0", "gamma_m0", "Gamma_M0" }, GammaM0_Norsok, "γM0"),
			(new[] { "GammaM1", "gammaM1", "γ M1", "yM1", "gamma_m1", "Gamma_M1" }, GammaM1_Norsok, "γM1"),
			(new[] { "GammaM2", "gammaM2", "γ M2", "yM2", "gamma_m2", "Gamma_M2" }, GammaM2_Norsok, "γM2"),
		};

		public ProjectSettingsService(IConnectionApiClient client, Action<string> log)
		{
			_client = client;
			_log = log;
		}

		/// <summary>
		/// Reads current project settings, logs ALL keys, and updates code factors
		/// to Norsok values. Only updates keys that actually exist in the settings.
		/// </summary>
		public async Task<Dictionary<string, object>?> ApplyNorsokFactorsAsync(Guid projectId, CancellationToken ct = default)
		{
			_log("Reading current project settings...");
			var settings = await _client.Settings.GetSettingsAsync(projectId, cancellationToken: ct);

			// Log ALL settings keys so we can discover the correct names
			_log($"  Total settings: {settings.Count} keys");
			foreach (var kvp in settings)
			{
				_log($"  [{kvp.Key}] = {kvp.Value}");
			}

			// Find the actual keys for gamma factors
			var updates = new Dictionary<string, object>();

			foreach (var (candidates, norsokValue, label) in GammaFactors)
			{
				string? foundKey = null;
				foreach (var candidate in candidates)
				{
					if (settings.ContainsKey(candidate))
					{
						foundKey = candidate;
						break;
					}
				}

				// Also try case-insensitive search if no exact match
				if (foundKey == null)
				{
					foreach (var settingKey in settings.Keys)
					{
						// Match keys containing "M0", "M1", "M2" with gamma-like prefix
						var lower = settingKey.ToLowerInvariant();
						var labelLower = label.ToLowerInvariant().Replace("γ", "gamma");
						if (lower.Contains("gamma") && lower.Contains(label[^1].ToString()))
						{
							foundKey = settingKey;
							break;
						}
					}
				}

				if (foundKey != null)
				{
					_log($"  Found {label} as key '{foundKey}' = {settings[foundKey]} → updating to {norsokValue}");
					updates[foundKey] = norsokValue;
				}
				else
				{
					_log($"  WARNING: Could not find settings key for {label}. Check log above for available keys.");
				}
			}

			if (updates.Count > 0)
			{
				_log($"Updating {updates.Count} code factor(s) to NORSOK N-004 values...");
				try
				{
					var updated = await _client.Settings.UpdateSettingsAsync(projectId, updates, cancellationToken: ct);
					_log("Project settings updated successfully.");
					return updated;
				}
				catch (Exception ex)
				{
					_log($"WARNING: Settings update failed: {ex.Message}");
					_log("Continuing with default code factors. You may need to set them manually in IDEA StatiCa.");
				}
			}
			else
			{
				_log("WARNING: No matching gamma factor keys found in settings. Skipping update.");
				_log("You may need to set code factors manually in IDEA StatiCa Project Settings.");
			}

			return null;
		}
	}
}
