using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Modify a given connections parameters.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateParameters(ConnectionApiClient conClient)
		{
			string filePath = "Inputs/User_testing_end_v23_1.ideaCon";
			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			//Get projectId Guid
			Guid projectId = conProject.ProjectId;
			var connections = await conClient.Connection.GetConnectionsAsync(projectId);
			int connectionId = connections[0].Id;

			//Get only visible parameters that we would expect to update.
			List<IdeaParameter> parametersVisible = await conClient.Parameter.GetParametersAsync(projectId, connectionId, false);

			//You can get all the parameters using this call.
			List<IdeaParameter> parametersAll = await conClient.Parameter.GetParametersAsync(projectId, connectionId, true);

			//Update parameters
			List<IdeaParameterUpdate> updates = new List<IdeaParameterUpdate>();

			foreach (var visibleParam in parametersVisible)
			{
				if (visibleParam.Key == "NoCols")
				{
					Console.WriteLine("Current No of Bolt Rows: "+visibleParam.Value);
					Console.WriteLine("Please Select the Number of Bolt Rows");
					string noOfBolts = Console.ReadLine();
					updates.Add(new IdeaParameterUpdate(visibleParam.Key, noOfBolts));
				}
			}

			//FIX: Parameter Data should be looked at. We should output IdeaParameter.
			List<ParameterData> parameters = await conClient.Parameter.UpdateParametersAsync(projectId, connectionId, updates);

			string exampleFolder = GetExampleFolderPathOnDesktop("UpdateParameters");
			string fileName = "User_testing_end_v23_1_updated.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);

			//Save the applied template
			await conClient.Project.SaveProjectAsync(projectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(projectId);
		}
	}
}
