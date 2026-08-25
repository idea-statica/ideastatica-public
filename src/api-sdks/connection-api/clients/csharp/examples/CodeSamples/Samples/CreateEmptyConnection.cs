using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Adds a new empty connection to an opened project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task CreateEmptyConnection(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple knee connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			Console.WriteLine($"The project contains {connections.Count} connection(s)");

			//Create a new empty connection. If null or an empty string is passed instead of a name, a default name 'CON{newId}' is assigned.
			ConConnection emptyConnection = await conClient.Connection.CreateEmptyConnectionAsync(conClient.ActiveProjectId, "New empty connection");

			Console.WriteLine($"Connection '{emptyConnection.Name}' was created with Id {emptyConnection.Id}");

			//The empty connection has no members or operations yet. It can be populated using the Member, Operation and Template APIs.
			List<ConMember> members = await conClient.Member.GetMembersAsync(conClient.ActiveProjectId, emptyConnection.Id);
			Console.WriteLine($"The new connection contains {members.Count} members");

			string exampleFolder = GetExampleFolderPathOnDesktop("CreateEmptyConnection");
			string saveFilePath = Path.Combine(exampleFolder, "knee connection - empty connection added.ideaCon");

			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
