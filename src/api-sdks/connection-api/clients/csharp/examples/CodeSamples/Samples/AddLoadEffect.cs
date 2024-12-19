using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.Api.Connection.Model.Connection;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example adds a new load effect to an opened project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task AddLoadEffect(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// Get Load Effects
			List<ConLoadEffect> loadEffects = await conClient.LoadEffect.GetLoadEffectsAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine("Add new Load effect.");
			Console.WriteLine("Specify Name or Hit Enter for Quick Add");
			string input = Console.ReadLine()?? "";

			if (string.IsNullOrEmpty(input))
			{
				//Generic quick add of a load effect
				//BUG: DOES NOT WORK.
				//BUG: DEFAULT SHOULD BE ACTIVE.
				var newLoadEffect = await conClient.LoadEffect.AddLoadEffectAsync(conClient.ActiveProjectId, connectionId);
				
				if(newLoadEffect != null) 
					Console.WriteLine($"Load Effect Added: Name= {newLoadEffect.Name}, Id= {newLoadEffect.Id}"); 
			}
			{
				ConLoadEffect loadEffect = new ConLoadEffect() { Name = input };

				var newLoadEffect = await conClient.LoadEffect.AddLoadEffectAsync(conClient.ActiveProjectId, connectionId, loadEffect);
				if (newLoadEffect != null)
					Console.WriteLine($"Load Effect Added: Name= {newLoadEffect.Name}, Id= {newLoadEffect.Id}");
			}

			// Get Load Effects after add.
			loadEffects = await conClient.LoadEffect.GetLoadEffectsAsync(conClient.ActiveProjectId, connectionId);

			string exampleFolder = GetExampleFolderPathOnDesktop("AddLoadEffect");
			
			// Save updated file.
			string fileName = "add-load-effects.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
