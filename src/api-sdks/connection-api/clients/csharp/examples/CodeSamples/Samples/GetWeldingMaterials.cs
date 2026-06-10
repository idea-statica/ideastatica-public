using IdeaRS.OpenModel.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get all welding materials available in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetWeldingMaterials(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all welding materials in the project. Items are polymorphic IOM materials (e.g. MatWeldingEc2).
			List<MatWelding> weldingMaterials = (await conClient.Material.GetWeldingMaterialsAsync(conClient.ActiveProjectId)).Cast<MatWelding>().ToList();

			Console.WriteLine("Welding materials in the project: " + weldingMaterials.Count);
			foreach (MatWelding weldingMaterial in weldingMaterials)
			{
				Console.WriteLine($"Id: {weldingMaterial.Id} Name: {weldingMaterial.Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
