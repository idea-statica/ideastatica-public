using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using System.Text;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example creates a reusable connection template with structured metadata from an existing designed connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task CreateTemplateFromConnection(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Create the template and get metadata inherited from the source connection.
			ConTemplateCreateResult result = await conClient.Template.CreateTemplateFromConnectionAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine("Template name: " + result.Name);
			Console.WriteLine($"Design code: {result.DesignCode}, manufacturing type: {result.ManufacturingType}");
			Console.WriteLine($"Operations: {result.OperationCount}, parameters: {result.ParameterCount}, parametric links: {result.ParamModelLinkCount}");

			string exampleFolder = GetExampleFolderPathOnDesktop("CreateTemplateFromConnection");
			string saveFilePath = Path.Combine(exampleFolder, "knee-connection.contemp");

			//Save the contemp payload so it can be re-imported later by Template.ImportTemplateFromFile.
			File.WriteAllText(saveFilePath, result.Template, Encoding.Unicode);
			Console.WriteLine("Template saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
