using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the Scene3D visualization data (triangulated mesh of the connection model) of a given connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetDataScene3D(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Get the data for the Scene3D visualization of the connection.
			DrawData sceneData = await conClient.Presentation.GetDataScene3DAsync(conClient.ActiveProjectId, connectionId);

			//Vertices and normals are stored as flat lists of coordinates (x, y, z per vertex).
			Console.WriteLine($"Scene3D data of connection {connectionId}:");
			Console.WriteLine($"  Groups: {sceneData.Groups.Count}");
			Console.WriteLine($"  Vertices: {sceneData.Vertices.Count / 3}");
			Console.WriteLine($"  Normals: {sceneData.Normals.Count / 3}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
