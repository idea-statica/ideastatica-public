using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Save the word report of a given connection.
		/// </summary>
		/// <param name="conClient"></param>
		/// <returns></returns>
		public static async Task SaveReport_Word_NOTWORKING(ConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			//Get projectId Guid
			Guid projectId = conProject.ProjectId;
			var connections = await conClient.Connection.GetConnectionsAsync(projectId);
			int connectionId = connections[0].Id;

			string exampleFolder = GetExampleFolderPathOnDesktop("GenerateReport");

			// Save updated file.
			string fileName = "simple cleat connection.docx";
			string pdfFilePath = Path.Combine(exampleFolder, fileName);

			//Save Report to PDF
			await conClient.Report.SaveReportWordAsync(projectId, connectionId, pdfFilePath);

			Console.WriteLine($"Report saved to: {pdfFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(projectId);
		}
	}
}
