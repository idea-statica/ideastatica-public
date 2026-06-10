using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the connection template of a design item from the Connection Library and saves it as a .contemp file.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetTemplate(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Propose design items from the Connection Library to get a design set id and a design item id.
			List<ConDesignItem> proposedItems = await conClient.ConnectionLibrary.ProposeAsync(conClient.ActiveProjectId, connectionId, new ConConnectionLibrarySearchParameters());

			if (proposedItems.Count == 0)
			{
				Console.WriteLine("No design items were proposed for the connection.");
				await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
				return;
			}

			ConDesignItem designItem = proposedItems[0];

			//Get the template of the design item. The content is returned as a base64 encoded string.
			string templateBase64 = await conClient.ConnectionLibrary.GetTemplateAsync(designItem.ConDesignSetId, designItem.ConDesignItemId);

			//Decode the base64 content to get the template XML.
			byte[] templateXml = Convert.FromBase64String(templateBase64);

			string exampleFolder = GetExampleFolderPathOnDesktop("GetTemplate");
			string saveFilePath = Path.Combine(exampleFolder, designItem.ConDesignItemId + ".contemp");

			await File.WriteAllBytesAsync(saveFilePath, templateXml);
			Console.WriteLine($"Template of design item '{designItem.Name}' ({templateXml.Length} bytes) saved to: {saveFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
