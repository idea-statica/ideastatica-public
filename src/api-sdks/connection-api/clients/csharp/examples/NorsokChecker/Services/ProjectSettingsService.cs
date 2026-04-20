using IdeaStatiCa.ConnectionApi;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Reads and updates project settings (code factors) via the Connection API.
	/// Sets γM values to match NORSOK N-004 Table 6-1.
	///
	/// NORSOK N-004 Table 6-1 Material factors:
	///   γM0 = 1.15  (Class 1,2,3 cross-sections)
	///   γM1 = 1.15  (Class 4 cross-sections, buckling)
	///   γM2 = 1.30  (Net section at bolt holes, welds, bolts)
	///
	/// Additionally, §6.1 states:
	///   For checks using EN 1993-1-1/1-5/1-8 material factors,
	///   multiply by γBC = 1.05 (additional building code factor).
	/// </summary>
	public class ProjectSettingsService
	{
		private readonly IConnectionApiClient _client;
		private readonly Action<string> _log;

		// NORSOK N-004 Table 6-1 material factors
		public const double GammaM0_Norsok = 1.15;
		public const double GammaM1_Norsok = 1.15;
		public const double GammaM2_Norsok = 1.30;  // Was 1.25 (EC3) — Norsok requires 1.30

		// §6.1: additional building code factor for EN 1993-based checks
		public const double GammaBC = 1.05;

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

			// Find all gamma-related keys
			var gammaKeys = settings.Keys
				.Where(k => k.IndexOf("amma", StringComparison.OrdinalIgnoreCase) >= 0)
				.ToList();

			_log($"  Gamma-related keys found: {gammaKeys.Count}");
			foreach (var k in gammaKeys)
			{
				_log($"    [{k}] = {settings[k]}");
			}

			string? keyM0 = gammaKeys.FirstOrDefault(k => k.EndsWith("GammaM0", StringComparison.OrdinalIgnoreCase));
			string? keyM1 = gammaKeys.FirstOrDefault(k => k.EndsWith("GammaM1", StringComparison.OrdinalIgnoreCase));
			string? keyM2 = gammaKeys.FirstOrDefault(k => k.EndsWith("GammaM2", StringComparison.OrdinalIgnoreCase));
			string? keyM3 = gammaKeys.FirstOrDefault(k => k.EndsWith("GammaM3", StringComparison.OrdinalIgnoreCase));

			if (keyM0 == null && keyM1 == null && keyM2 == null)
			{
				_log("  WARNING: No gamma keys found. Dumping all settings keys for diagnosis:");
				foreach (var k in settings.Keys.OrderBy(k => k))
				{
					_log($"    [{k}] = {settings[k]}");
				}
				_log("  Skipping code factor update. Set them manually in Project Settings.");
				return null;
			}

			var updates = new Dictionary<string, object>();

			_log("Applying NORSOK N-004 Table 6-1 material factors:");

			if (keyM0 != null)
			{
				_log($"  γM0 = {GammaM0_Norsok} (Class 1,2,3 cross-sections)  [{keyM0}]: {settings[keyM0]} → {GammaM0_Norsok}");
				updates[keyM0] = GammaM0_Norsok;
			}
			if (keyM1 != null)
			{
				_log($"  γM1 = {GammaM1_Norsok} (Class 4 + buckling)  [{keyM1}]: {settings[keyM1]} → {GammaM1_Norsok}");
				updates[keyM1] = GammaM1_Norsok;
			}
			if (keyM2 != null)
			{
				_log($"  γM2 = {GammaM2_Norsok} (Welds, bolts, net section)  [{keyM2}]: {settings[keyM2]} → {GammaM2_Norsok}");
				updates[keyM2] = GammaM2_Norsok;
			}
			if (keyM3 != null)
			{
				// γM3 also gets 1.30 in Norsok (same as γM2 for slip-resistant connections)
				_log($"  γM3 = {GammaM2_Norsok} (Slip-resistant connections)  [{keyM3}]: {settings[keyM3]} → {GammaM2_Norsok}");
				updates[keyM3] = GammaM2_Norsok;
			}

			_log($"  γBC = {GammaBC} (additional building code factor, applied in formula evaluation)");

			_log($"Updating {updates.Count} code factor(s) via API...");
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
