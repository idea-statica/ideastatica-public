using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Api.Connection.Model.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Add a new bolt grade material from the MPRL (Material and Product Range Library) to the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task AddMaterialBoltGrade(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Define the new bolt grade by its name in the MPRL.
			ConMprlElement newBoltGrade = new ConMprlElement() { MprlName = "10.9" };

			//Add the bolt grade material to the project.
			await conClient.Material.AddMaterialBoltGradeAsync(conClient.ActiveProjectId, newBoltGrade);
			Console.WriteLine("Bolt grade added to the project: " + newBoltGrade.MprlName);

			//Verify by listing the bolt grade materials which are now available in the project.
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
