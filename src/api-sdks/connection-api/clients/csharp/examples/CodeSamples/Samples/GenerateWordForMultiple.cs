using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Generates one Word report for multiple connections in the project. The generated document contains one section per each connection report.
		/// Uses the SaveMultipleReportsWordAsync client extension method which wraps the GenerateWordForMultiple operation.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GenerateWordForMultiple(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/User_testing_end_v23_1.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all connections in the project and request the report for all of them.
			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			List<int> connectionIds = connections.Select(c => c.Id).ToList();

			string exampleFolder = GetExampleFolderPathOnDesktop("GenerateReportMultiple");

			string fileName = "multiple connections report.docx";
			string wordFilePath = Path.Combine(exampleFolder, fileName);

			//Save the report of all requested connections to one Word file.
			await conClient.Report.SaveMultipleReportsWordAsync(conClient.ActiveProjectId, connectionIds, wordFilePath);

			Console.WriteLine($"Report of {connectionIds.Count} connection(s) saved to: {wordFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
