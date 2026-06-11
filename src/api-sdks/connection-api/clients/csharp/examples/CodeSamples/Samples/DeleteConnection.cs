using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Deletes a specific connection from the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task DeleteConnection(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			Console.WriteLine($"The project contains {connections.Count} connection(s)");

			//Create a copy of the first connection so there is a second connection which can be safely deleted.
			ConConnection copiedConnection = await conClient.Connection.CopyConnectionAsync(conClient.ActiveProjectId, connections[0].Id, "To be deleted");
			Console.WriteLine($"Connection '{copiedConnection.Name}' was created with Id {copiedConnection.Id}");

			//Delete the copied connection. The list of the remaining connections is returned.
			List<ConConnection> remainingConnections = await conClient.Connection.DeleteConnectionAsync(conClient.ActiveProjectId, copiedConnection.Id);

			Console.WriteLine($"Connection {copiedConnection.Id} was deleted. Remaining connections:");
			foreach (var connection in remainingConnections)
			{
				Console.WriteLine($"({connection.Id}) {connection.Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
