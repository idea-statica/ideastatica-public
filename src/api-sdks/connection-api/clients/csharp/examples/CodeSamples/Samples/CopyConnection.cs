using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Creates a copy of an existing connection in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task CopyConnection(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Copy the connection. If null or an empty string is passed instead of a name, a unique name is derived from the source connection name.
			ConConnection newConnection = await conClient.Connection.CopyConnectionAsync(conClient.ActiveProjectId, connectionId, "Copy of " + connections[0].Name);

			Console.WriteLine($"Connection '{newConnection.Name}' was created with Id {newConnection.Id}");

			//List all connections in the project to see the new copy.
			connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			Console.WriteLine($"The project now contains {connections.Count} connections:");
			foreach (var connection in connections)
			{
				Console.WriteLine($"({connection.Id}) {connection.Name}");
			}

			string exampleFolder = GetExampleFolderPathOnDesktop("CopyConnection");
			string saveFilePath = Path.Combine(exampleFolder, "knee connection - copied.ideaCon");

			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
