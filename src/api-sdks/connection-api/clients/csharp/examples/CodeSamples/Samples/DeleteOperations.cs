using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Deletes all manufacturing operations of a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task DeleteOperations(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			var operations = await conClient.Operation.GetOperationsAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"Operations in connection '{connections[0].Name}' before delete: {operations.Count}");

			//Delete all operations of the connection.
			await conClient.Operation.DeleteOperationsAsync(conClient.ActiveProjectId, connectionId);

			operations = await conClient.Operation.GetOperationsAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"Operations in connection '{connections[0].Name}' after delete: {operations.Count}");

			string exampleFolder = GetExampleFolderPathOnDesktop("DeleteOperations");
			string saveFilePath = Path.Combine(exampleFolder, "connection-without-operations.ideaCon");

			//Save the modified project.
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
