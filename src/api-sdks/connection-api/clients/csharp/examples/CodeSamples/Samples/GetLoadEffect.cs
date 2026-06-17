using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get a single load effect of a connection and print its member loadings.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetLoadEffect(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// Get all load effects to find an existing load effect Id.
			List<ConLoadEffect> loadEffects = await conClient.LoadEffect.GetLoadEffectsAsync(conClient.ActiveProjectId, connectionId);
			int loadEffectId = loadEffects[0].Id;

			// Get a single load effect by its Id.
			ConLoadEffect loadEffect = await conClient.LoadEffect.GetLoadEffectAsync(conClient.ActiveProjectId, connectionId, loadEffectId);

			Console.WriteLine($"Load effect: Id= {loadEffect.Id}, Name= {loadEffect.Name}, IsPercentage= {loadEffect.IsPercentage}");

			// Print the internal forces applied to each member.
			// Note: continuous members have two entries (Begin and End), ended members have one.
			foreach (ConLoadEffectMemberLoad loading in loadEffect.MemberLoadings)
			{
				var f = loading.SectionLoad;
				Console.WriteLine($"Member {loading.MemberId} ({loading.Position}): N= {f.N}, Vy= {f.Vy}, Vz= {f.Vz}, Mx= {f.Mx}, My= {f.My}, Mz= {f.Mz}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
