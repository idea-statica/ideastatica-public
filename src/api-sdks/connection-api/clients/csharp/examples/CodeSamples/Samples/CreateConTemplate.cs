using IdeaStatiCa.ConnectionApi;
using System.Text;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example creates a connection template (.contemp content) from an existing designed connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task CreateConTemplate(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Create the template content from the design of the connection.
			string templateXml = await conClient.Template.CreateConTemplateAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"Template created from connection {connectionId}, content length: {templateXml.Length} characters");

			string exampleFolder = GetExampleFolderPathOnDesktop("CreateConTemplate");
			string saveFilePath = Path.Combine(exampleFolder, "knee-connection.contemp");

			//Save the template so it can be re-imported later by Template.ImportTemplateFromFile.
			File.WriteAllText(saveFilePath, templateXml, Encoding.Unicode);
			Console.WriteLine("Template saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
