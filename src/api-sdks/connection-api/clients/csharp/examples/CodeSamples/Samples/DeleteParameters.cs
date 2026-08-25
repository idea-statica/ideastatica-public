using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Delete all parameters and parameter model links of a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task DeleteParameters(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/User_testing_end_v23_1.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// Get all parameters (including hidden ones) defined in the connection.
			List<IdeaParameter> parameters = await conClient.Parameter.GetParametersAsync(conClient.ActiveProjectId, connectionId, true);
			Console.WriteLine($"Parameters before delete: {parameters.Count}");

			// Delete all parameters and parameter model links for the connection.
			await conClient.Parameter.DeleteParametersAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine("All parameters were deleted.");

			parameters = await conClient.Parameter.GetParametersAsync(conClient.ActiveProjectId, connectionId, true);
			Console.WriteLine($"Parameters after delete: {parameters.Count}");

			string exampleFolder = GetExampleFolderPathOnDesktop("DeleteParameters");

			// Save updated file.
			string fileName = "deleted-parameters.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
