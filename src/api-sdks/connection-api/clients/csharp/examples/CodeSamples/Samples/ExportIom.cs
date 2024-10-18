using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{

		/// <summary>
		/// This example exports the connection to Idea Open Model (IOM).
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task ExportIom_NOTWORKING(ConnectionApiClient conClient) 
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			//Get projectId Guid
			Guid projectId = conProject.ProjectId;
			var connections = await conClient.Connection.GetConnectionsAsync(projectId);
			int connectionId = connections[0].Id;

			//ConnectionData conData = await conClient.Export.ExportIomXmlAsync()

			//string saveFilePath = "connection-file-from-IOM.ideaCon";

			//await conClient.Project.SaveProjectAsync(projectId, saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(projectId);
		}
	}
}
