using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Runs the CBFEM calculation of all connections in the project and prints the summary of the results.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task Calculate(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all connections in the project.
			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			List<int> connectionIds = connections.Select(c => c.Id).ToList();

			//Run the CBFEM calculation and get the summary of the results.
			List<ConResultSummary> results = await conClient.Calculation.CalculateAsync(conClient.ActiveProjectId, connectionIds);

			foreach (ConResultSummary result in results)
			{
				Console.WriteLine($"Connection {result.Id} passed: {result.Passed}");

				foreach (var summary in result.ResultSummary)
				{
					Console.WriteLine($"  Name: {summary.Name}, CheckValue: {summary.CheckValue}, CheckStatus: {summary.CheckStatus}");
				}
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
