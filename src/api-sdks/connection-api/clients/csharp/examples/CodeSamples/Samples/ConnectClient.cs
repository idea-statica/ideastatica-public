using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Shows how a client is connected to the ConnectionRestApi service.
		/// The SDK calls the connect-client endpoint automatically when the API client is created
		/// and passes the obtained ClientId in the header of all subsequent requests.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task ConnectClient(IConnectionApiClient conClient)
		{
			//The ClientId was obtained from the connect-client endpoint when this client was created.
			Console.WriteLine("ClientId of this connected client: " + conClient.ClientId);

			//The raw endpoint can also be called directly.
			//Each call registers a new client in the service and returns its unique identifier.
			//Note: this API client keeps using its original ClientId from above.
			string newClientId = await conClient.ClientApi.ConnectClientAsync();

			Console.WriteLine("ClientId returned by a direct connect-client call: " + newClientId);
		}
	}
}
