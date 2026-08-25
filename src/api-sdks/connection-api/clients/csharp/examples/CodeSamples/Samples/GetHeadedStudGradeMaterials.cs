using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Api.Connection.Model.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get all headed stud grade materials available in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetHeadedStudGradeMaterials(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//A steel-to-steel project may not contain any headed stud grade.
			//Add one from the MPRL first so the list below is not empty.
			await conClient.Material.AddMaterialHeadedStudGradeAsync(conClient.ActiveProjectId, new ConMprlElement() { MprlName = "SD1" });

			//Get all headed stud grade materials in the project.
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
