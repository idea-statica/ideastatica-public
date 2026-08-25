using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets data about a specific connection in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetConnection(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Get the data of one specific connection by its Id.
			ConConnection connection = await conClient.Connection.GetConnectionAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"Id: {connection.Id}");
			Console.WriteLine($"Name: {connection.Name}");
			Console.WriteLine($"Identifier: {connection.Identifier}");
			Console.WriteLine($"Description: {connection.Description}");
			Console.WriteLine($"Analysis type: {connection.AnalysisType}");
			Console.WriteLine($"Include buckling: {connection.IncludeBuckling}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
