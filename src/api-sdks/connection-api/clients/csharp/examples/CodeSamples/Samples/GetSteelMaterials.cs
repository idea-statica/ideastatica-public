using IdeaRS.OpenModel.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get all steel materials available in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetSteelMaterials(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all steel materials in the project. Items are polymorphic IOM materials (e.g. MatSteelEc2).
			List<MatSteel> steelMaterials = (await conClient.Material.GetSteelMaterialsAsync(conClient.ActiveProjectId)).Cast<MatSteel>().ToList();

			Console.WriteLine("Steel materials in the project: " + steelMaterials.Count);
			foreach (MatSteel steel in steelMaterials)
			{
				Console.WriteLine($"Id: {steel.Id} Name: {steel.Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
