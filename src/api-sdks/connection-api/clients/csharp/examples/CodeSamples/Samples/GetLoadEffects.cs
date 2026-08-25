using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get all load effects defined in a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetLoadEffects(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// Get all load effects defined in the connection.
			List<ConLoadEffect> loadEffects = await conClient.LoadEffect.GetLoadEffectsAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"Connection {connectionId} contains {loadEffects.Count} load effect(s).");

			foreach (ConLoadEffect loadEffect in loadEffects)
			{
				Console.WriteLine($"Id= {loadEffect.Id}, Name= {loadEffect.Name}, Active= {loadEffect.Active}, MemberLoadings= {loadEffect.MemberLoadings.Count}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
