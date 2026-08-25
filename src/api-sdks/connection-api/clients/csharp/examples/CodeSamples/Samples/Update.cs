using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Update the parameters of a connection with new values.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task Update(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/User_testing_end_v23_1.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			// Parameter expressions are always strings - e.g. "3", not 3.
			// 'NoCols' is an existing parameter (number of bolt rows) in this project.
			List<IdeaParameterUpdate> updates = new List<IdeaParameterUpdate>
			{
				new IdeaParameterUpdate() { Key = "NoCols", Expression = "3" }
			};

			ParameterUpdateResponse response = await conClient.Parameter.UpdateAsync(conClient.ActiveProjectId, connectionId, updates);

			Console.WriteLine($"Parameters set to model: {response.SetToModel}");

			foreach (IdeaParameterValidationResponse failed in response.FailedValidations)
			{
				Console.WriteLine($"Validation failed for '{failed.Key}': {failed.Message} ({failed.ValidationStatus})");
			}

			IdeaParameter updatedParameter = response.Parameters.FirstOrDefault(p => p.Key == "NoCols");
			if (updatedParameter != null)
			{
				Console.WriteLine($"Parameter '{updatedParameter.Key}' updated: Expression= {updatedParameter.Expression}, Value= {updatedParameter.Value}");
			}

			string exampleFolder = GetExampleFolderPathOnDesktop("Update");

			// Save updated file.
			string fileName = "updated-parameters.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
