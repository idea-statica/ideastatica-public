using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the version of the running IDEA StatiCa Connection API service.
		/// Useful to verify which service version the client is connected to.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetVersion(IConnectionApiClient conClient)
		{
			//Get the IdeaStatiCa API assembly version of the running service.
			string version = await conClient.ClientApi.GetVersionAsync();

			Console.WriteLine("Connected to IDEA StatiCa Connection API service version: " + version);
		}
	}
}
