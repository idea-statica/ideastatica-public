using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Updates the data of a specific connection in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateConnection(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			ConConnection connection = connections[0];

			Console.WriteLine($"Current name: '{connection.Name}', description: '{connection.Description}'");

			//Modify the connection data.
			connection.Name = "Renamed connection";
			connection.Description = "Description updated through the Connection API";

			//Apply the new data to the connection.
			ConConnection updatedConnection = await conClient.Connection.UpdateConnectionAsync(conClient.ActiveProjectId, connection.Id, connection);

			Console.WriteLine($"Updated name: '{updatedConnection.Name}', description: '{updatedConnection.Description}'");

			string exampleFolder = GetExampleFolderPathOnDesktop("UpdateConnection");
			string saveFilePath = Path.Combine(exampleFolder, "simple cleat - connection update.ideaCon");

			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
