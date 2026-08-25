using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Opens an existing .ideaCon project file in the service and lists the connections it contains.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task OpenProject(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";

			//Open the project. It stays open in the service until it is closed.
			ConProject project = await conClient.Project.OpenProjectAsync(filePath);

			Console.WriteLine("Project opened with Id: " + project.ProjectId);
			Console.WriteLine("Project name: " + project.ProjectInfo.Name);
			Console.WriteLine("Design code: " + project.ProjectInfo.CountryCode);

			foreach (var connection in project.Connections)
			{
				Console.WriteLine($"Connection {connection.Id}: {connection.Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
