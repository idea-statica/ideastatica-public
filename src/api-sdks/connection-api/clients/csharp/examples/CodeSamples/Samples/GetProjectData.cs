using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the data of an open project (project info and the list of connections).
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetProjectData(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get the data of the open project.
			ConProject projectData = await conClient.Project.GetProjectDataAsync(conClient.ActiveProjectId);

			Console.WriteLine("Project Id: " + projectData.ProjectId);
			Console.WriteLine("Name: " + projectData.ProjectInfo.Name);
			Console.WriteLine("Design code: " + projectData.ProjectInfo.CountryCode);
			Console.WriteLine("Date: " + projectData.ProjectInfo.Date);

			foreach (var connection in projectData.Connections)
			{
				Console.WriteLine($"Connection {connection.Id}: {connection.Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
