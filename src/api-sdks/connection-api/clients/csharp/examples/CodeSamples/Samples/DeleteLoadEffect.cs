using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Delete a load effect from a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task DeleteLoadEffect(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// Get the load effects currently defined in the connection.
			List<ConLoadEffect> loadEffects = await conClient.LoadEffect.GetLoadEffectsAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"Load effects before delete: {loadEffects.Count}");

			// Delete the last load effect in the list.
			int loadEffectId = loadEffects.Last().Id;
			int deletedId = await conClient.LoadEffect.DeleteLoadEffectAsync(conClient.ActiveProjectId, connectionId, loadEffectId);
			Console.WriteLine($"Deleted load effect with Id = {deletedId}");

			// Get the load effects after the delete.
			loadEffects = await conClient.LoadEffect.GetLoadEffectsAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"Load effects after delete: {loadEffects.Count}");

			string exampleFolder = GetExampleFolderPathOnDesktop("DeleteLoadEffect");

			// Save updated file.
			string fileName = "deleted-load-effect.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
