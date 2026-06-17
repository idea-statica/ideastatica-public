using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the raw CBFEM results (a serialized instance of CheckResultsData) as a JSON string and saves it to a file.
		/// The calculation is run by the service automatically when the raw results are requested.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetRawJsonResults(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			List<int> connectionIds = new List<int> { connections[0].Id };

			//Get the raw CBFEM results. One JSON string is returned for each requested connection.
			List<string> rawJsonResults = await conClient.Calculation.GetRawJsonResultsAsync(conClient.ActiveProjectId, connectionIds);

			string exampleFolder = GetExampleFolderPathOnDesktop("GetRawJsonResults");

			for (int i = 0; i < rawJsonResults.Count; i++)
			{
				string jsonFilePath = Path.Combine(exampleFolder, $"raw-results-connection-{connectionIds[i]}.json");
				await File.WriteAllTextAsync(jsonFilePath, rawJsonResults[i]);

				Console.WriteLine($"Raw results of connection {connectionIds[i]} ({rawJsonResults[i].Length} characters) saved to: {jsonFilePath}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
