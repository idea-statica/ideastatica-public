using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example exports the connection to an Ifc to an Ifc file (.ifc).
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task ExportIfc(IConnectionApiClient conClient) 
		{
			string filePath = "Inputs/simple knee connection.ideaCon";

			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			string exampleFolderPath = GetExampleFolderPathOnDesktop("ExportIFC");
			string connectionName = string.IsNullOrEmpty(connections[0].Name) ? "Conn1" : connections[0].Name;
			string ifcPath = Path.Combine(exampleFolderPath, connectionName + ".ifc");

			//Export the connection to Ifc file.
			await conClient.Export.ExportIfcFileAsync(conClient.ActiveProjectId, connectionId, ifcPath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
