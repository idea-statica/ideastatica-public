using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Api.Connection.Model.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Add a new welding material from the MPRL (Material and Product Range Library) to the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task AddMaterialWeld(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Define the new welding material by its name in the MPRL.
			ConMprlElement newWeldingMaterial = new ConMprlElement() { MprlName = "S 275" };

			//Add the welding material to the project. It can then be assigned to weld operations.
			await conClient.Material.AddMaterialWeldAsync(conClient.ActiveProjectId, newWeldingMaterial);
			Console.WriteLine("Welding material added to the project: " + newWeldingMaterial.MprlName);

			//Verify by listing the welding materials which are now available in the project.
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
