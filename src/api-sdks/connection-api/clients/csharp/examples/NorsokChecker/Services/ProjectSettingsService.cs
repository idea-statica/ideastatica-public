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
		/// Reads current project settings and updates code factors to Norsok values.
		/// Returns the settings dictionary after update.
		/// </summary>
		public async Task<Dictionary<string, object>> ApplyNorsokFactorsAsync(Guid projectId, CancellationToken ct = default)
		{
			_log("Reading current project settings...");
			var settings = await _client.Settings.GetSettingsAsync(projectId, cancellationToken: ct);

			_log($"  Current settings keys: {string.Join(", ", settings.Keys.Take(20))}...");

			// Log current gamma values if they exist
			LogSettingValue(settings, "yM0");
			LogSettingValue(settings, "yM1");
			LogSettingValue(settings, "yM2");

			// Apply Norsok factors
			var updates = new Dictionary<string, object>
			{
				["yM0"] = GammaM0_Norsok,
				["yM1"] = GammaM1_Norsok,
				["yM2"] = GammaM2_Norsok,
			};

			_log($"Updating code factors: γM0={GammaM0_Norsok}, γM1={GammaM1_Norsok}, γM2={GammaM2_Norsok}");
			var updated = await _client.Settings.UpdateSettingsAsync(projectId, updates, cancellationToken: ct);

			_log("Project settings updated to NORSOK N-004 values.");
			return updated;
		}

		private void LogSettingValue(Dictionary<string, object> settings, string key)
		{
			if (settings.TryGetValue(key, out var val))
				_log($"  {key} = {val}");
		}
	}
}
