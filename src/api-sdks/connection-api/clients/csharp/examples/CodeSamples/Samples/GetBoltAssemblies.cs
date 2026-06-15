using IdeaRS.OpenModel.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get all bolt assemblies available in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetBoltAssemblies(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all bolt assemblies in the project.
			List<BoltAssembly> boltAssemblies = (await conClient.Material.GetBoltAssembliesAsync(conClient.ActiveProjectId)).Cast<BoltAssembly>().ToList();

			Console.WriteLine("Bolt assemblies in the project: " + boltAssemblies.Count);
			foreach (BoltAssembly boltAssembly in boltAssemblies)
			{
				//Dimensions of IOM objects are in SI units (m).
				Console.WriteLine($"Id: {boltAssembly.Id} Name: {boltAssembly.Name} Diameter: {boltAssembly.Diameter} m");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
