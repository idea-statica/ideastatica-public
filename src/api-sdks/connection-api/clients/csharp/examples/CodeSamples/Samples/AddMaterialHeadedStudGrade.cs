using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Api.Connection.Model.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Add a new headed stud grade material from the MPRL (Material and Product Range Library) to the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task AddMaterialHeadedStudGrade(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Define the new headed stud grade by its name in the MPRL.
			ConMprlElement newHeadedStudGrade = new ConMprlElement() { MprlName = "SD1" };

			//Add the headed stud grade material to the project.
			await conClient.Material.AddMaterialHeadedStudGradeAsync(conClient.ActiveProjectId, newHeadedStudGrade);
			Console.WriteLine("Headed stud grade added to the project: " + newHeadedStudGrade.MprlName);

			//Verify by listing the headed stud grade materials which are now available in the project.
			List<MaterialHeadedStudGrade> headedStudGrades = (await conClient.Material.GetHeadedStudGradeMaterialsAsync(conClient.ActiveProjectId)).Cast<MaterialHeadedStudGrade>().ToList();

			Console.WriteLine("Headed stud grades in the project: " + headedStudGrades.Count);
			foreach (MaterialHeadedStudGrade headedStudGrade in headedStudGrades)
			{
				Console.WriteLine($"Id: {headedStudGrade.Id} Name: {headedStudGrade.Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
