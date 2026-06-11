using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the list of manufacturing operations of a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetOperations(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Get the list of manufacturing operations defined in the connection.
			List<ConOperation> operations = await conClient.Operation.GetOperationsAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"Connection '{connections[0].Name}' contains {operations.Count} operation(s):");
			foreach (ConOperation operation in operations)
			{
				Console.WriteLine($"Id: {operation.Id} Name: {operation.Name} Type: {operation.OperationType} Active: {operation.Active}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
