using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Adds a new member to a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task AddMember(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			List<ConMember> members = await conClient.Member.GetMembersAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"The connection contains {members.Count} members");

			//Define the new member. The cross-section of an existing member is reused.
			//If CrossSectionId is not set, the first cross-section available in the project is used.
			ConMember newMember = new ConMember
			{
				Name = "D",
				CrossSectionId = members[0].CrossSectionId,
				Position = new ConMemberPosition
				{
					DefinedBy = ConMemberPlacementDefinitionTypeEnum.DirectionVector,
					AxisX = new IdeaRS.OpenModel.Geometry3D.Vector3D(0.7071067811865476, 0, 0.7071067811865476)
				}
			};

			//Add the new member to the connection.
			ConMember addedMember = await conClient.Member.AddMemberAsync(conClient.ActiveProjectId, connectionId, newMember);
			Console.WriteLine($"Member '{addedMember.Name}' was added with Id {addedMember.Id}");

			members = await conClient.Member.GetMembersAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"The connection now contains {members.Count} members");

			string exampleFolder = GetExampleFolderPathOnDesktop("AddMember");
			string saveFilePath = Path.Combine(exampleFolder, "simple cleat - member added.ideaCon");

			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
