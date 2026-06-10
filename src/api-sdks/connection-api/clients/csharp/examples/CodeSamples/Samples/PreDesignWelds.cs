using IdeaStatiCa.Api.Connection.Model.Connection;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Pre-designs the sizes of all welds in a connection using a selected weld sizing method.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task PreDesignWelds(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Pre-design all welds in the connection.
			//Available sizing methods: FullStrength, MinimumDuctility, OverStrengthFactor, CapacityEstimation.
			string result = await conClient.Operation.PreDesignWeldsAsync(conClient.ActiveProjectId, connectionId, ConWeldSizingMethodEnum.FullStrength);

			Console.WriteLine($"Weld pre-design of connection '{connections[0].Name}' finished: {result}");

			string exampleFolder = GetExampleFolderPathOnDesktop("PreDesignWelds");
			string saveFilePath = Path.Combine(exampleFolder, "knee-connection-predesigned-welds.ideaCon");

			//Save the project with the re-sized welds.
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
