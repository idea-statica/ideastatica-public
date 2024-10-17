using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		public static async Task SaveReport_Pdf_NOTWORKING(ConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			//Get projectId Guid
			Guid projectId = conProject.ProjectId;
			var connections = await conClient.Connection.GetAllConnectionsDataAsync(projectId);
			int connectionId = connections[0].Id;

			string exampleFolder = GetExampleFolderPathOnDesktop("GenerateReport");

			// Save updated file.
			string fileName = "simple cleat connection.pdf";
			string pdfFilePath = Path.Combine(exampleFolder, fileName);

			//Save Report to PDF
			await conClient.Report.SaveReportWordAsync(projectId, connectionId, pdfFilePath);

			Console.WriteLine($"Report saved to: {pdfFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(projectId.ToString());
		}
	}
}
