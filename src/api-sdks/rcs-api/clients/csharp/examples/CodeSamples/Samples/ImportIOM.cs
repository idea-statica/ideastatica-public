using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdeaStatiCa.RcsApi;
using IdeaStatiCa.RcsApi.Client;

namespace CodeSamples.Samples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Creates a new connection project from a selected IOM file. The first connection point in the IOM file will be added to the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task CreateProjectFromIom(RcsApiClient rcsClient)
		{
			//string filePath = "Inputs/multiple_connections.xml";

			//await rcsClient.Project.ImportIOMFileAsync(rcsClient.Project, filePath);

			//RcsProject rcsProject = await rcsClient.Project.CreateProjectFromIomFileAsync(filePath);

			//Get projectId Guid
			//Guid projectId = rcsProject.ProjectId;
			//var connections = await conClient.Connection.GetConnectionsAsync(projectId);
			//int connectionId = connections[0].Id;

			//string saveFilePath = "connection-file-from-IOM.ideaCon";

			//await conClient.Project.SaveProjectAsync(projectId, saveFilePath);
		}
	}
}
