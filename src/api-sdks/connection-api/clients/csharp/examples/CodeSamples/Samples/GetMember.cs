using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets information about a specific member in a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetMember(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			List<ConMember> members = await conClient.Member.GetMembersAsync(conClient.ActiveProjectId, connectionId);
			int memberId = members[0].Id;

			//Get the data of one specific member by its Id.
			ConMember member = await conClient.Member.GetMemberAsync(conClient.ActiveProjectId, connectionId, memberId);

			Console.WriteLine($"Id: {member.Id}");
			Console.WriteLine($"Name: {member.Name}");
			Console.WriteLine($"Active: {member.Active}");
			Console.WriteLine($"Is continuous: {member.IsContinuous}");
			Console.WriteLine($"Is bearing: {member.IsBearing}");
			Console.WriteLine($"Cross-section Id: {member.CrossSectionId}");
			Console.WriteLine($"Position defined by: {member.Position.DefinedBy}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
