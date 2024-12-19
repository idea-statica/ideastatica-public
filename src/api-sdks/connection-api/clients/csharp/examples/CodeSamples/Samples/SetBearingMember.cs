using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Set the bearing member of a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetAndSetBearingMember(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			List<ConMember> members = await conClient.Member.GetMembersAsync(conClient.ActiveProjectId, connectionId);

			foreach (var member in members)
			{
				Console.WriteLine($"{member.Name} ({member.Id}) is {(member.IsContinuous ? "Continuous" : "Ended")} {(member.IsBearing ? "and is the Bearing Member" :"")}");
			}

			Console.WriteLine("Select which member to set as bearing:");
			string intput = Console.ReadLine();

			int newBearingMemberId = int.Parse(intput);

			//Set the bearing member
			await conClient.Member.SetBearingMemberAsync(conClient.ActiveProjectId, connectionId, newBearingMemberId);

			//Retrieve again the member data.
			members = await conClient.Member.GetMembersAsync(conClient.ActiveProjectId, connectionId);
			foreach (var member in members)
			{
				Console.WriteLine($"{member.Name} ({member.Id}) is {(member.IsContinuous ? "Continuous" : "Ended")} {(member.IsBearing ? "and is the Bearing Member" : "")}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
