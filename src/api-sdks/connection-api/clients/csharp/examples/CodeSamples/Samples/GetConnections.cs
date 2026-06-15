using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets data about all connections in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetConnections(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get the list of all connections in the opened project.
			List<ConConnection> connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);

			Console.WriteLine($"The project contains {connections.Count} connection(s):");
			foreach (var connection in connections)
			{
				Console.WriteLine($"({connection.Id}) {connection.Name} - analysis type: {connection.AnalysisType}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
