using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		public static async Task GetAndSetBearingMember(ConnectionApiClient conClient)
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			//Get projectId Guid
			Guid projectId = conProject.ProjectId;
			var connections = await conClient.Connection.GetAllConnectionsDataAsync(projectId);
			int connectionId = connections[0].Id;

			List<ConMember> members = await conClient.Member.GetAllMemberDataAsync(projectId, connectionId);

			foreach (var member in members)
			{
				Console.WriteLine($"{member.Name} ({member.Id}) is {(member.IsContinuous ? "Continuous" : "Ended")} {(member.IsBearing ? "and is the Bearing Member" :"")}");
			}

			Console.WriteLine("Select which member to set as bearing:");
			string intput = Console.ReadLine();

			int newBearingMemberId = int.Parse(intput);

			//Set the bearing member
			await conClient.Member.SetBearingMemberAsync(projectId, connectionId, newBearingMemberId);

			//Retrieve again the member data.
			members = await conClient.Member.GetAllMemberDataAsync(projectId, connectionId);
			foreach (var member in members)
			{
				Console.WriteLine($"{member.Name} ({member.Id}) is {(member.IsContinuous ? "Continuous" : "Ended")} {(member.IsBearing ? "and is the Bearing Member" : "")}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(projectId.ToString());
		}
	}
}
