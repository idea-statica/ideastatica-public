using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Evaluate expressions in the context of a connection and its parameters.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task EvaluateExpression(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/User_testing_end_v23_1.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// The request body must be a JSON string literal, so wrap the expression in quotes.
			string expression = "15*2";
			string result = await conClient.Parameter.EvaluateExpressionAsync(conClient.ActiveProjectId, connectionId, $"\"{expression}\"");
			Console.WriteLine($"Expression '{expression}' evaluated to: {result}");

			// An expression can also reference parameters defined in the connection.
			// 'NoCols' is an existing parameter (number of bolt rows) in this project.
			string parameterExpression = "NoCols*2";
			string parameterResult = await conClient.Parameter.EvaluateExpressionAsync(conClient.ActiveProjectId, connectionId, $"\"{parameterExpression}\"");
			Console.WriteLine($"Expression '{parameterExpression}' evaluated to: {parameterResult}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
