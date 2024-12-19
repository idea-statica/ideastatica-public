using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example exports the connection to Idea Open Model (IOM).
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task ExportIomModel(IConnectionApiClient conClient) 
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//FIX Needs to output the Iom Model xml.
			await conClient.Export.ExportIomAsync(conClient.ActiveProjectId, connectionId);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
