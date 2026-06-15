using IdeaStatiCa.Api.Connection.Model.Connection;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Set the load settings of a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task SetLoadSettings(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// Get the current load settings for the connection.
			ConLoadSettings loadSettings = await conClient.LoadEffect.GetLoadSettingsAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"Current settings: LoadsInEquilibrium= {loadSettings.LoadsInEquilibrium}, LoadsInPercentage= {loadSettings.LoadsInPercentage}");

			// Toggle the loads in equilibrium setting and set it back to the connection.
			loadSettings.LoadsInEquilibrium = !loadSettings.LoadsInEquilibrium;

			ConLoadSettings updatedSettings = await conClient.LoadEffect.SetLoadSettingsAsync(conClient.ActiveProjectId, connectionId, loadSettings);
			Console.WriteLine($"Updated settings: LoadsInEquilibrium= {updatedSettings.LoadsInEquilibrium}, LoadsInPercentage= {updatedSettings.LoadsInPercentage}");

			string exampleFolder = GetExampleFolderPathOnDesktop("SetLoadSettings");

			// Save updated file.
			string fileName = "updated-load-settings.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
