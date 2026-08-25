using IdeaRS.OpenModel;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Creates a new empty connection project with a chosen design code.
		/// The empty project can then be populated using other API calls (e.g. members, operations or templates).
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task CreateEmptyProject(IConnectionApiClient conClient)
		{
			//Create an empty project for the given design code and name.
			ConProject project = await conClient.Project.CreateProjectAsync(new ConProjectData { CountryCode = CountryCode.ECEN, Name = "My empty project" });

			Console.WriteLine("Empty project created with Id: " + project.ProjectId);
			Console.WriteLine("Project name: " + project.ProjectInfo.Name);
			Console.WriteLine("Design code: " + project.ProjectInfo.CountryCode);
			Console.WriteLine("Number of connections: " + project.Connections.Count);

			string exampleFolder = GetExampleFolderPathOnDesktop("CreateEmptyProject");
			string saveFilePath = Path.Combine(exampleFolder, "empty-project.ideaCon");

			//Save the empty project.
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
