using IdeaRS.OpenModel.Material;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Updates the common operation properties (default bolt assembly) for all operations of a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateCommonOperationProperties(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Pick a bolt assembly from the project to be set as the default for all operations.
			List<BoltAssembly> boltAssemblies = (await conClient.Material.GetBoltAssembliesAsync(conClient.ActiveProjectId)).Cast<BoltAssembly>().ToList();
			BoltAssembly newDefaultBoltAssembly = boltAssemblies.Last();
			Console.WriteLine($"New default bolt assembly: Id: {newDefaultBoltAssembly.Id} Name: {newDefaultBoltAssembly.Name}");

			//Update the common properties of all operations of the connection.
			//Properties which are left null (here weld and plate material) remain unchanged.
			ConOperationCommonProperties commonProperties = new ConOperationCommonProperties();
			commonProperties.BoltAssemblyId = newDefaultBoltAssembly.Id;

			await conClient.Operation.UpdateCommonOperationPropertiesAsync(conClient.ActiveProjectId, connectionId, commonProperties);

			//Read the common properties back to verify the update.
			ConOperationCommonProperties updatedProperties = await conClient.Operation.GetCommonOperationPropertiesAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"Bolt assembly Id after update: {updatedProperties.BoltAssemblyId}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
