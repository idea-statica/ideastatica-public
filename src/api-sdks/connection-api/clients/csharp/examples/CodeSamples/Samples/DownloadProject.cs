using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Downloads the current project from the service and saves it as an .ideaCon file.
		/// The downloaded file includes all changes made by previous API calls.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task DownloadProject(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			string exampleFolder = GetExampleFolderPathOnDesktop("DownloadProject");
			string saveFilePath = Path.Combine(exampleFolder, "downloaded-project.ideaCon");

			//SaveProjectAsync calls the download-project endpoint and writes the received data to the file.
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);

			Console.WriteLine("Project downloaded to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
