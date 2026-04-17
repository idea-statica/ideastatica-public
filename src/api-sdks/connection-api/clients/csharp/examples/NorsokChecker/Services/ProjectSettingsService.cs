using IdeaStatiCa.ConnectionApi;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Reads and updates project settings (code factors) via the Connection API.
	/// Sets γM values to match NORSOK N-004 Table 6-1.
	///
	/// Actual settings keys (discovered from API):
	///   calculation/Analysis/CodeFactors/EN/GammaM0
	///   calculation/Analysis/CodeFactors/EN/GammaM1
	///   calculation/Analysis/CodeFactors/EN/GammaM2
	///   calculation/Analysis/CodeFactors/EN/GammaM3
	///   calculation/Analysis/CodeFactors/EN/GammaMu
	/// </summary>
	public class ProjectSettingsService
	{
		private readonly IConnectionApiClient _client;
		private readonly Action<string> _log;

		// NORSOK N-004 material factors (Table 6-1, §6.3.7)
		public const double GammaM0_Norsok = 1.15;
		public const double GammaM1_Norsok = 1.15;
		public const double GammaM2_Norsok = 1.25;

		// Actual settings keys from IDEA StatiCa Connection API
		private const string Key_GammaM0 = "calculation/Analysis/CodeFactors/EN/GammaM0";
		private const string Key_GammaM1 = "calculation/Analysis/CodeFactors/EN/GammaM1";
		private const string Key_GammaM2 = "calculation/Analysis/CodeFactors/EN/GammaM2";

		public ProjectSettingsService(IConnectionApiClient client, Action<string> log)
		{
			_client = client;
			_log = log;
		}

		/// <summary>
		/// Reads current project settings and updates code factors to Norsok values.
		/// </summary>
		public async Task<Dictionary<string, object>?> ApplyNorsokFactorsAsync(Guid projectId, CancellationToken ct = default)
		{
			_log("Reading current project settings...");
			var settings = await _client.Settings.GetSettingsAsync(projectId, cancellationToken: ct);

			// Log current gamma values
			LogSettingValue(settings, Key_GammaM0);
			LogSettingValue(settings, Key_GammaM1);
			LogSettingValue(settings, Key_GammaM2);

			// Apply Norsok factors
			var updates = new Dictionary<string, object>
			{
				[Key_GammaM0] = GammaM0_Norsok,
				[Key_GammaM1] = GammaM1_Norsok,
				[Key_GammaM2] = GammaM2_Norsok,
			};

			_log($"Updating code factors to NORSOK N-004:");
			_log($"  γM0: {GetValue(settings, Key_GammaM0)} → {GammaM0_Norsok}");
			_log($"  γM1: {GetValue(settings, Key_GammaM1)} → {GammaM1_Norsok}");
			_log($"  γM2: {GetValue(settings, Key_GammaM2)} → {GammaM2_Norsok}");

			try
			{
				var updated = await _client.Settings.UpdateSettingsAsync(projectId, updates, cancellationToken: ct);
				_log("Project settings updated successfully.");
				return updated;
			}
			catch (Exception ex)
			{
				_log($"WARNING: Settings update failed: {ex.Message}");
				_log("Continuing with existing code factors.");
				return null;
			}
		}

		private void LogSettingValue(Dictionary<string, object> settings, string key)
		{
			if (settings.TryGetValue(key, out var val))
				_log($"  {key} = {val}");
			else
				_log($"  {key} = (not found)");
		}

		private static string GetValue(Dictionary<string, object> settings, string key)
		{
			return settings.TryGetValue(key, out var val) ? val?.ToString() ?? "null" : "?";
		}
	}
}
