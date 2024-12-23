using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Save the pdf report of a given connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task SaveReport_Pdf(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			string exampleFolder = GetExampleFolderPathOnDesktop("GenerateReport");

			// Save updated file.
			string fileName = "simple cleat connection.pdf";
			string pdfFilePath = Path.Combine(exampleFolder, fileName);

			//Save Report to PDF
			await conClient.Report.SaveReportPdfAsync(conClient.ActiveProjectId, connectionId, pdfFilePath);

			Console.WriteLine($"Report saved to: {pdfFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
