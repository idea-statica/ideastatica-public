using IdeaStatiCa.ConnectionApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples.Samples
{
	internal class UpdateProjectFromIOM
	{

		/// <summary>
		/// Creates a new connection project from a selected IOM file and then Updates it using another IOM. The first connection point in the IOM file will be added to the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateProjectFromIom(IConnectionApiClient conClient)
		{
			//Create the project using IOM.
			string filePath = "Inputs/multiple_connections.xml";
			await conClient.Project.CreateProjectFromIomFileAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Update the connection project using IOM.
			string UpdateFilePath = "Inputs/multiple_connections_updated.xml";
			await conClient.Project.UpdateProjectFromIomFileAsync(conClient.ActiveProjectId, UpdateFilePath);

			//Save the updated project to disc.
			string saveFilePath = "Updated_connection-file-from-IOM.ideaCon";
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
