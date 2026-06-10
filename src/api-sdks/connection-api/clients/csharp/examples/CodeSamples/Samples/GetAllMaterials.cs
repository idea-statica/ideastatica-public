using IdeaRS.OpenModel.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get all materials (steel, concrete, bolt grade, welding, headed stud grade) available in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetAllMaterials(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all materials in the project. Items are polymorphic IOM materials (e.g. MatSteelEc2, MaterialBoltGrade).
			List<Material> allMaterials = (await conClient.Material.GetAllMaterialsAsync(conClient.ActiveProjectId)).Cast<Material>().ToList();

			Console.WriteLine("Materials in the project: " + allMaterials.Count);
			foreach (Material material in allMaterials)
			{
				Console.WriteLine($"Id: {material.Id} Name: {material.Name} Type: {material.GetType().Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
