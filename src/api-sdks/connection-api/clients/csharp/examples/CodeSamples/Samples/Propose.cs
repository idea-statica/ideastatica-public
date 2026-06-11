using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using ConSearchOption = IdeaStatiCa.Api.Connection.Model.SearchOption;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Proposes suitable design items from the Connection Library for a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task Propose(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Search parameters can filter the proposals by set membership and connection features.
			//Here we search all available sets and ignore all feature filters (the default).
			ConConnectionLibrarySearchParameters searchParameters = new ConConnectionLibrarySearchParameters();
			searchParameters.HasBolts = ConSearchOption.Ignore;
			searchParameters.HasWelds = ConSearchOption.Ignore;

			//Propose design items matching the topology and design code of the connection.
			List<ConDesignItem> proposedItems = await conClient.ConnectionLibrary.ProposeAsync(conClient.ActiveProjectId, connectionId, searchParameters);

			Console.WriteLine($"Proposed design items for connection '{connections[0].Name}': {proposedItems.Count}");
			foreach (ConDesignItem designItem in proposedItems)
			{
				Console.WriteLine($"Name: {designItem.Name} Design code: {designItem.DesignCode} DesignItemId: {designItem.ConDesignItemId}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
