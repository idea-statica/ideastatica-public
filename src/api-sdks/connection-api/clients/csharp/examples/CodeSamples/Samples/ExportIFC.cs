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
		/// <summary>
		/// This example exports the connection to an Ifc to an Ifc file (.ifc).
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task ExportIfc(ConnectionApiClient conClient) 
		{
			string filePath = "Inputs/simple knee connection.ideaCon";

			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			Guid projectId = conProject.ProjectId;
			var connections = await conClient.Connection.GetAllConnectionsDataAsync(projectId);
			int connectionId = connections[0].Id;

			string exampleFolderPath = GetExampleFolderPathOnDesktop("ExportIFC");
			string connectionName = string.IsNullOrEmpty(connections[0].Name) ? "Conn1" : connections[0].Name;
			string ifcPath = Path.Combine(exampleFolderPath, connectionName + ".ifc");

			//FIX Naming remove 'Con'
			await conClient.Export.ExportIfcFileAsync(projectId, connectionId, ifcPath);

			await conClient.Project.CloseProjectAsync(projectId.ToString());

		}
	}
}
