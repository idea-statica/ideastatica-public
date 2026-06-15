using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the list of all projects which are currently open in the service for the connected client.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetActiveProjects(IConnectionApiClient conClient)
		{
			//Open two projects so there is something to list.
			await conClient.Project.OpenProjectAsync("Inputs/simple cleat connection.ideaCon");
			await conClient.Project.OpenProjectAsync("Inputs/simple knee connection.ideaCon");

			//Get all projects opened by this client.
			List<ConProject> activeProjects = await conClient.Project.GetActiveProjectsAsync();

			Console.WriteLine("Number of active projects: " + activeProjects.Count);

			foreach (var project in activeProjects)
			{
				Console.WriteLine($"Project {project.ProjectId}: '{project.ProjectInfo.Name}' ({project.Connections.Count} connections)");
			}

			//Close all the opened projects.
			foreach (var project in activeProjects)
			{
				await conClient.Project.CloseProjectAsync(project.ProjectId);
			}
		}
	}
}
