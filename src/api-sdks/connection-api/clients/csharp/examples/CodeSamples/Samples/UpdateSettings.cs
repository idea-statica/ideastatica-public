using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Updates a setting value in an open project. The example changes the language of the report.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateSettings(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Find the key of the setting to be updated by searching the project settings.
			Dictionary<string, object> settings = await conClient.Settings.GetSettingsAsync(conClient.ActiveProjectId, "LanguageInReport");

			string settingKey = settings.Keys.First(k => k.Contains("LanguageInReport"));
			Console.WriteLine($"Current value: {settingKey} = {settings[settingKey]}");

			//Update one or more settings by sending a dictionary of key-value pairs.
			var settingsToUpdate = new Dictionary<string, object>
			{
				{ settingKey, "de" }
			};

			Dictionary<string, object> updatedSettings = await conClient.Settings.UpdateSettingsAsync(conClient.ActiveProjectId, settingsToUpdate);

			Console.WriteLine($"Updated value: {settingKey} = {updatedSettings[settingKey]}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
