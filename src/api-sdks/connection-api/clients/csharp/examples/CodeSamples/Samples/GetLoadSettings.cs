using IdeaStatiCa.Api.Connection.Model.Connection;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get the load settings of a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetLoadSettings(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// Get the load settings for the connection.
			ConLoadSettings loadSettings = await conClient.LoadEffect.GetLoadSettingsAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"Load settings of connection {connectionId}:");
			Console.WriteLine($"LoadsInEquilibrium= {loadSettings.LoadsInEquilibrium}");
			Console.WriteLine($"LoadsInPercentage= {loadSettings.LoadsInPercentage}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
