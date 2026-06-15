using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets all design sets of the Connection Library which are available for the current user.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetDesignSets(IConnectionApiClient conClient)
		{
			//Get all design sets available for the current user.
			//This includes the predefined set delivered with IDEA StatiCa and the user's personal and company sets.
			//No project needs to be opened for this call.
			List<ConDesignSet> designSets = await conClient.ConnectionLibrary.GetDesignSetsAsync();

			Console.WriteLine($"Available design sets: {designSets.Count}");
			foreach (ConDesignSet designSet in designSets)
			{
				Console.WriteLine($"Id: {designSet.Id} Name: {designSet.Name} Type: {designSet.Type}");
			}
		}
	}
}
