using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Save the word report of a given connection.
		/// </summary>
		/// <param name="conClient"></param>
		/// <returns></returns>
		public static async Task SaveReport_Word(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			//Get the first connection in the project.
			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			string exampleFolder = GetExampleFolderPathOnDesktop("GenerateReport");

			// Save updated file.
			string fileName = "simple cleat connection.docx";
			string wordFilePath = Path.Combine(exampleFolder, fileName);

			//Save Report to PDF.
			await conClient.Report.SaveReportWordAsync(conClient.ActiveProjectId, connectionId, wordFilePath);

			Console.WriteLine($"Report saved to: {wordFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
