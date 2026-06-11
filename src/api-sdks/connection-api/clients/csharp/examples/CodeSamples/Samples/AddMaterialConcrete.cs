using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Api.Connection.Model.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Add a new concrete material from the MPRL (Material and Product Range Library) to the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task AddMaterialConcrete(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Define the new concrete material by its name in the MPRL.
			ConMprlElement newConcrete = new ConMprlElement() { MprlName = "C70/85" };

			//Add the concrete material to the project (used by concrete blocks of anchoring operations).
			await conClient.Material.AddMaterialConcreteAsync(conClient.ActiveProjectId, newConcrete);
			Console.WriteLine("Concrete material added to the project: " + newConcrete.MprlName);

			//Verify by listing the concrete materials which are now available in the project.
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
