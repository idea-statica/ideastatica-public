using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the common operation properties (default weld material, plate material and bolt assembly) of a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetCommonOperationProperties(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Get the common properties which are shared by all operations of the connection.
			//A null value means the project default is used.
			ConOperationCommonProperties commonProperties = await conClient.Operation.GetCommonOperationPropertiesAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"Common operation properties of connection '{connections[0].Name}':");
			Console.WriteLine($"Weld material Id: {commonProperties.WeldMaterialId?.ToString() ?? "project default"}");
			Console.WriteLine($"Plate material Id: {commonProperties.PlateMaterialId?.ToString() ?? "project default"}");
			Console.WriteLine($"Bolt assembly Id: {commonProperties.BoltAssemblyId?.ToString() ?? "project default"}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
