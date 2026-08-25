using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the detailed results of the CBFEM analysis (checks of plates, bolts, welds etc.) for a given connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetResults(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			List<int> connectionIds = new List<int> { connections[0].Id };

			//Run the CBFEM analysis first.
			await conClient.Calculation.CalculateAsync(conClient.ActiveProjectId, connectionIds);

			//Get the detailed results of the CBFEM analysis.
			List<ConnectionCheckRes> results = await conClient.Calculation.GetResultsAsync(conClient.ActiveProjectId, connectionIds);

			foreach (ConnectionCheckRes connectionResult in results)
			{
				Console.WriteLine($"Connection: {connectionResult.Name} (Id: {connectionResult.Id})");

				foreach (CheckResSummary summary in connectionResult.CheckResSummary)
				{
					Console.WriteLine($"  Name: {summary.Name}, CheckValue: {summary.CheckValue}, CheckStatus: {summary.CheckStatus}");
				}

				Console.WriteLine($"  Plate checks: {connectionResult.CheckResPlate.Count}, Bolt checks: {connectionResult.CheckResBolt.Count}, Weld checks: {connectionResult.CheckResWeld.Count}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
