using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the topology of a connection in JSON format.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetConnectionTopology(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Get the topology of the connection as a JSON string.
			//It describes how the members of the connection are arranged (e.g. for searching similar designs).
			string topology = await conClient.Connection.GetConnectionTopologyAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"Topology of connection '{connections[0].Name}' ({connectionId}):");
			Console.WriteLine(topology);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
