using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Creates a new connection project from a selected IOM file. The first connection point in the IOM file will be added to the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task CreateProjectFromIom(ConnectionApiClient conClient) 
		{
			string filePath = "Inputs/multiple_connections.xml";
			ConProject conProject = await conClient.Project.CreateProjectFromIomFileAsync(filePath);

			//Get projectId Guid
			Guid projectId = conProject.ProjectId;
			var connections = await conClient.Connection.GetConnectionsAsync(projectId);
			int connectionId = connections[0].Id;

			string saveFilePath = "connection-file-from-IOM.ideaCon";

			await conClient.Project.SaveProjectAsync(projectId, saveFilePath);
		}
	}
}
