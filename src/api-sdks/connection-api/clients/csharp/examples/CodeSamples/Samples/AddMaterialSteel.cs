using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Api.Connection.Model.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Add a new steel material from the MPRL (Material and Product Range Library) to the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task AddMaterialSteel(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Define the new steel material by its name in the MPRL.
			ConMprlElement newSteel = new ConMprlElement() { MprlName = "S 450" };

			//Add the steel material to the project. It can then be assigned to plates, members or welds.
			await conClient.Material.AddMaterialSteelAsync(conClient.ActiveProjectId, newSteel);
			Console.WriteLine("Steel material added to the project: " + newSteel.MprlName);

			//Verify by listing the steel materials which are now available in the project.
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
