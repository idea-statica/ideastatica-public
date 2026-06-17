using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Updates the project information (name, author, description, ...) of an open project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateProjectData(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get the current project data and modify the required fields.
			ConProject project = await conClient.Project.GetProjectDataAsync(conClient.ActiveProjectId);

			ConProjectData projectData = project.ProjectInfo;
			projectData.Name = "Updated project name";
			projectData.Author = "API user";
			projectData.Description = "Project data updated by the Connection API";
			projectData.Date = DateTime.Now;

			//Send the updated project data to the service.
			ConProject updatedProject = await conClient.Project.UpdateProjectDataAsync(conClient.ActiveProjectId, projectData);

			Console.WriteLine("Name: " + updatedProject.ProjectInfo.Name);
			Console.WriteLine("Author: " + updatedProject.ProjectInfo.Author);
			Console.WriteLine("Description: " + updatedProject.ProjectInfo.Description);

			string exampleFolder = GetExampleFolderPathOnDesktop("UpdateProjectData");
			string saveFilePath = Path.Combine(exampleFolder, "updated-project-data.ideaCon");

			//Save the project with the updated data.
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
