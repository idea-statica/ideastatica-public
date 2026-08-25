using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the estimated production cost of a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetProductionCost(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Get the production cost estimate of the connection.
			//The cost is based on the cost estimation settings of the project (steel, bolts, welds, hole drilling).
			ConProductionCost productionCost = await conClient.Connection.GetProductionCostAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"Total estimated production cost of connection '{connections[0].Name}': {productionCost.TotalEstimatedCost:F2}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
