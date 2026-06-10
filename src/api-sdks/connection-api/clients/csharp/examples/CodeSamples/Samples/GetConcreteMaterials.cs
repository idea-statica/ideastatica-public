using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Api.Connection.Model.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get all concrete materials available in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetConcreteMaterials(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//A steel-to-steel project may not contain any concrete material.
			//Add one from the MPRL first so the list below is not empty.
			await conClient.Material.AddMaterialConcreteAsync(conClient.ActiveProjectId, new ConMprlElement() { MprlName = "C70/85" });

			//Get all concrete materials in the project.
			List<MatConcrete> concreteMaterials = (await conClient.Material.GetConcreteMaterialsAsync(conClient.ActiveProjectId)).Cast<MatConcrete>().ToList();

			Console.WriteLine("Concrete materials in the project: " + concreteMaterials.Count);
			foreach (MatConcrete concrete in concreteMaterials)
			{
				Console.WriteLine($"Id: {concrete.Id} Name: {concrete.Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
