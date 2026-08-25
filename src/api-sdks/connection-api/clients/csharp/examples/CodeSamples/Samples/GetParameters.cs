using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get the parameters defined in a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetParameters(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/User_testing_end_v23_1.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// Get only the visible parameters of the connection.
			List<IdeaParameter> parametersVisible = await conClient.Parameter.GetParametersAsync(conClient.ActiveProjectId, connectionId, false);

			// Pass 'true' to also include the hidden parameters.
			List<IdeaParameter> parametersAll = await conClient.Parameter.GetParametersAsync(conClient.ActiveProjectId, connectionId, true);

			Console.WriteLine($"Connection {connectionId} has {parametersVisible.Count} visible parameter(s) ({parametersAll.Count} including hidden).");

			foreach (IdeaParameter parameter in parametersVisible)
			{
				Console.WriteLine($"Key= {parameter.Key}, Expression= {parameter.Expression}, Value= {parameter.Value}, Unit= {parameter.Unit}, Type= {parameter.ParameterType}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
