using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets information about all members in a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetMembers(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Get the list of all members in the connection.
			List<ConMember> members = await conClient.Member.GetMembersAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"The connection contains {members.Count} members:");
			foreach (var member in members)
			{
				Console.WriteLine($"({member.Id}) {member.Name} is {(member.IsContinuous ? "Continuous" : "Ended")} {(member.IsBearing ? "and is the Bearing Member" : "")}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
