using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Closes an open project in the service and releases its resources.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task CloseProject(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			Guid projectId = conClient.ActiveProjectId;
			Console.WriteLine("Project opened with Id: " + projectId);

			//Close the project to release resources in the service.
			//Any unsaved changes are lost, so save the project first if required.
			await conClient.Project.CloseProjectAsync(projectId);

			Console.WriteLine("Project " + projectId + " was closed.");

			//No project is active on the client anymore.
			Console.WriteLine("Active project Id after closing: " + conClient.ActiveProjectId);
		}
	}
}
