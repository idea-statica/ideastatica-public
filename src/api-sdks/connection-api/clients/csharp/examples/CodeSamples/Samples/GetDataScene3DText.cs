using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the Scene3D visualization data of a given connection serialized as a JSON string and saves it to a file.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetDataScene3DText(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection - sections.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Get the serialized Scene3D data of the connection in JSON format.
			string sceneJson = await conClient.Presentation.GetDataScene3DTextAsync(conClient.ActiveProjectId, connectionId);

			string exampleFolder = GetExampleFolderPathOnDesktop("GetDataScene3DText");

			string fileName = "scene3d.json";
			string jsonFilePath = Path.Combine(exampleFolder, fileName);

			await File.WriteAllTextAsync(jsonFilePath, sceneJson);

			Console.WriteLine($"Scene3D data of connection {connectionId} ({sceneJson.Length} characters) saved to: {jsonFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
