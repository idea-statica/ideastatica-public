using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.Api.Connection.Model.Connection;
using IdeaStatiCa.ConnectionApi;


namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Update existing load effects in a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateLoadEffect(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			ConLoadSettings loadSettings = await conClient.LoadEffect.GetLoadSettingsAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine(loadSettings.ToString());

			// Get Load Effects
			List<ConLoadEffect> loadEffects = await conClient.LoadEffect.GetLoadEffectsAsync(conClient.ActiveProjectId, connectionId);

			double effectMultiplier = 1.25;

			ConLoadEffect loadEffectBasis = null;

			for (int i = 0; i < loadEffects.Count; i++)
			{
				if (i == 0)
				{
					loadEffectBasis = loadEffects[i];
					continue;
				}

				ConLoadEffect loadEffect = loadEffects[i];
				var memberLoadings = loadEffect.MemberLoadings.ToList();

				// NOTE: LoadEffect provides all the member loadings - even if LoadsInEquilibrium is set to False.
				for (int j = 0; j < memberLoadings.Count; j++)
				{
					ConLoadEffectMemberLoad loading = memberLoadings[j];
					ConLoadEffectMemberLoad loadingBasis = memberLoadings[j];

					loading.SectionLoad.N = loadingBasis.SectionLoad.N * effectMultiplier;
					loading.SectionLoad.Vy = loadingBasis.SectionLoad.Vy * effectMultiplier;
					loading.SectionLoad.Vz = loadingBasis.SectionLoad.Vz * effectMultiplier;
					loading.SectionLoad.Mz = loadingBasis.SectionLoad.Mz * effectMultiplier;
					loading.SectionLoad.My = loadingBasis.SectionLoad.My * effectMultiplier;
					loading.SectionLoad.Mx = loadingBasis.SectionLoad.Mx * effectMultiplier;
				}

				await conClient.LoadEffect.UpdateLoadEffectAsync(conClient.ActiveProjectId, connectionId, loadEffect);

				// Increase each increment by 25% of the original value.
				effectMultiplier += 0.25;
			}

			string exampleFolder = GetExampleFolderPathOnDesktop("ApplyTemplate");
			
			// Save updated file.
			string fileName = "updated-load-effects.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);

		}
	}
}
