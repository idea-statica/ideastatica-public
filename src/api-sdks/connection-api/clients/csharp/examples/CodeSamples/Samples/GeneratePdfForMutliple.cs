using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Generates one PDF report for multiple connections in the project. The generated PDF contains one section per each connection report.
		/// Uses the SaveMultipleReportsPdfAsync client extension method which wraps the GeneratePdfForMutliple operation.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GeneratePdfForMutliple(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/User_testing_end_v23_1.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all connections in the project and request the report for all of them.
			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			List<int> connectionIds = connections.Select(c => c.Id).ToList();

			string exampleFolder = GetExampleFolderPathOnDesktop("GenerateReportMultiple");

			string fileName = "multiple connections report.pdf";
			string pdfFilePath = Path.Combine(exampleFolder, fileName);

			//Save the report of all requested connections to one PDF file.
			await conClient.Report.SaveMultipleReportsPdfAsync(conClient.ActiveProjectId, connectionIds, pdfFilePath);

			Console.WriteLine($"Report of {connectionIds.Count} connection(s) saved to: {pdfFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
