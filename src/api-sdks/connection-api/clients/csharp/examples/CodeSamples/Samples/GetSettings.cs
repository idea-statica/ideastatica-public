using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the setting values of an open project, both the full list and a filtered search.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetSettings(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all settings of the project.
			Dictionary<string, object> allSettings = await conClient.Settings.GetSettingsAsync(conClient.ActiveProjectId);
			Console.WriteLine("Number of settings: " + allSettings.Count);

			foreach (var setting in allSettings.Take(5))
			{
				Console.WriteLine($"{setting.Key} = {setting.Value}");
			}

			//The optional search parameter filters the settings by a keyword.
			Dictionary<string, object> filteredSettings = await conClient.Settings.GetSettingsAsync(conClient.ActiveProjectId, "LanguageInReport");

			foreach (var setting in filteredSettings)
			{
				Console.WriteLine($"Filtered: {setting.Key} = {setting.Value}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
