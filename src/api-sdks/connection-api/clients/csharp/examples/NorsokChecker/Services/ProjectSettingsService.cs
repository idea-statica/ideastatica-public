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

		public ProjectSettingsService(IConnectionApiClient client, Action<string> log)
		{
			_client = client;
			_log = log;
		}

		/// <summary>
		/// Reads current project settings, discovers gamma factor keys,
		/// and updates code factors to Norsok values.
		/// </summary>
		public async Task<Dictionary<string, object>?> ApplyNorsokFactorsAsync(Guid projectId, CancellationToken ct = default)
		{
			_log("Reading current project settings...");
			var settings = await _client.Settings.GetSettingsAsync(projectId, cancellationToken: ct);

			_log($"  Total settings: {settings.Count} keys");

			// Find all gamma-related keys by searching for "amma" (catches Gamma, gamma, GammaM)
			var gammaKeys = settings.Keys
				.Where(k => k.IndexOf("amma", StringComparison.OrdinalIgnoreCase) >= 0)
				.ToList();

			_log($"  Gamma-related keys found: {gammaKeys.Count}");
			foreach (var k in gammaKeys)
			{
				_log($"    [{k}] = {settings[k]}");
			}

			// Find keys for GammaM0, GammaM1, GammaM2 by matching the end of the key path
			string? keyM0 = gammaKeys.FirstOrDefault(k => k.EndsWith("GammaM0", StringComparison.OrdinalIgnoreCase));
			string? keyM1 = gammaKeys.FirstOrDefault(k => k.EndsWith("GammaM1", StringComparison.OrdinalIgnoreCase));
			string? keyM2 = gammaKeys.FirstOrDefault(k => k.EndsWith("GammaM2", StringComparison.OrdinalIgnoreCase));

			if (keyM0 == null && keyM1 == null && keyM2 == null)
			{
				// Dump ALL keys if no gamma keys found at all
				_log("  WARNING: No gamma keys found. Dumping all settings keys for diagnosis:");
				foreach (var k in settings.Keys.OrderBy(k => k))
				{
					_log($"    [{k}] = {settings[k]}");
				}
				_log("  Skipping code factor update. Set them manually in Project Settings.");
				return null;
			}

			var updates = new Dictionary<string, object>();

			if (keyM0 != null)
			{
				_log($"  γM0: [{keyM0}] = {settings[keyM0]} → {GammaM0_Norsok}");
				updates[keyM0] = GammaM0_Norsok;
			}
			if (keyM1 != null)
			{
				_log($"  γM1: [{keyM1}] = {settings[keyM1]} → {GammaM1_Norsok}");
				updates[keyM1] = GammaM1_Norsok;
			}
			if (keyM2 != null)
			{
				_log($"  γM2: [{keyM2}] = {settings[keyM2]} → {GammaM2_Norsok}");
				updates[keyM2] = GammaM2_Norsok;
			}

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
				_log("Continuing with existing code factors. Set them manually in Project Settings.");
				return null;
			}
		}
	}
}
