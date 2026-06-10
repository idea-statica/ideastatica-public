using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Downloads the preview picture (PNG) of a design item from the Connection Library.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetDesignItemPicture(IConnectionApiClient conClient)
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

			string exampleFolder = GetExampleFolderPathOnDesktop("GetDesignItemPicture");
			string saveFilePath = Path.Combine(exampleFolder, designItem.ConDesignItemId + ".png");

			//Download the preview picture of the design item and save it as a PNG file.
			//SaveDesignItemPictureAsync is a client extension of the get-picture endpoint (GetDesignItemPictureAsync).
			await conClient.ConnectionLibrary.SaveDesignItemPictureAsync(designItem.ConDesignSetId, designItem.ConDesignItemId, saveFilePath);

			Console.WriteLine($"Picture of design item '{designItem.Name}' saved to: {saveFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
