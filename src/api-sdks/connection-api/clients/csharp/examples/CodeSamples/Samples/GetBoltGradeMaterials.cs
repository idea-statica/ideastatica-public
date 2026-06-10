using IdeaRS.OpenModel.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get all bolt grade materials available in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetBoltGradeMaterials(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all bolt grade materials in the project.
			List<MaterialBoltGrade> boltGrades = (await conClient.Material.GetBoltGradeMaterialsAsync(conClient.ActiveProjectId)).Cast<MaterialBoltGrade>().ToList();

			Console.WriteLine("Bolt grades in the project: " + boltGrades.Count);
			foreach (MaterialBoltGrade boltGrade in boltGrades)
			{
				Console.WriteLine($"Id: {boltGrade.Id} Name: {boltGrade.Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
