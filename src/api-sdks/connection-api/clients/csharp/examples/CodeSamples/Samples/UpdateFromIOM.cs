using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Updates a connection project (created from IOM) by a new version of the IOM model and results.
		/// In a real workflow the updated IOM file would come from a modified FEA model;
		/// here the same file is reused to demonstrate the call.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateFromIOM(IConnectionApiClient conClient)
		{
			string iomFilePath = "Inputs/multiple_connections.xml";

			//Create the project from the IOM file.
			await conClient.Project.CreateProjectFromIomFileAsync(iomFilePath);
			Console.WriteLine("Project created from IOM with Id: " + conClient.ActiveProjectId);

			//Update the open project from the (modified) IOM file - model and results.
			ConProject updatedProject = await conClient.Project.UpdateProjectFromIomFileAsync(conClient.ActiveProjectId, iomFilePath);

			Console.WriteLine("Project updated from IOM. Number of connections: " + updatedProject.Connections.Count);

			string exampleFolder = GetExampleFolderPathOnDesktop("UpdateFromIOM");
			string saveFilePath = Path.Combine(exampleFolder, "updated-from-iom.ideaCon");

			//Save the updated project.
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
