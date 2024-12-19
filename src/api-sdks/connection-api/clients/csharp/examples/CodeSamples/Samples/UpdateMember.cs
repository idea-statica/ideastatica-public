using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;


namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Update a members cross-section with an avaliable one in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateMemberCrossSection(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection - sections.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get First Connection
			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Create map of CrossSections and Materials.
			Dictionary<string, int> CrossSectionMap = new Dictionary<string, int>();

			//Get the cross-sections in the project.
			List<IdeaRS.OpenModel.CrossSection.CrossSection> crossSections = (await conClient.Material.GetCrossSectionsAsync(conClient.ActiveProjectId)).Cast<IdeaRS.OpenModel.CrossSection.CrossSection>().ToList();
			crossSections.ForEach(x => CrossSectionMap.Add(x.Name, x.Id));

			Console.WriteLine("List of avaliable cross-sections in the project:");
			foreach (var css in crossSections)
			{
				Console.WriteLine($"({css.Id}) {css.Name}");
			}

			//Get Member Information.
			List<ConMember> members = await conClient.Member.GetMembersAsync(conClient.ActiveProjectId, connectionId);

			foreach (var member in members)
			{
				Console.WriteLine($"{member.Name} ({member.Id}) has section size of {CrossSectionMap.FirstOrDefault(x => x.Value == member.Id).Key} {(member.IsBearing ? "and is the Bearing Member" : "")}");
			}

			bool found = false;
			int memberId = 0;
			while (!found)
			{
				Console.WriteLine("Select which member (by Id) to change cross-section:");
				string intput = Console.ReadLine();

				if (int.TryParse(intput, out memberId))
				{
					if (members.Select(x => x.Id).Contains(memberId))
					{
						found = true;
						break;
					}
				}
				Console.WriteLine("Member not found provide valid Id");
			}

			bool foundCss = false;
			int cssId = 0;
			while (!foundCss)
			{
				Console.WriteLine($"Select which cross-section to apply to Member {memberId}");
				string intput = Console.ReadLine();
				if (int.TryParse(intput, out cssId))
				{
					if (crossSections.Select(x => x.Id).Contains(cssId))
					{
						found = true;
						break;
					}
				}
				Console.WriteLine("Cross-section not found. Provide valid cross-section Id");
			}

			//Get the member with the provided MemberId.
			ConMember conMember = members.FirstOrDefault(x=> x.Id == memberId);
			conMember.CrossSectionId = cssId;

			//Update the member.
			var updatedMember = await conClient.Member.UpdateMemberAsync(conClient.ActiveProjectId, connectionId, conMember);

			//Get Member Information again.
			members = await conClient.Member.GetMembersAsync(conClient.ActiveProjectId, connectionId);

			foreach (var member in members)
			{
				Console.WriteLine($"{member.Name} ({member.Id}) has section size of {CrossSectionMap.FirstOrDefault(x => x.Value == member.CrossSectionId).Key} {(member.IsBearing ? "and is the Bearing Member" : "")}");
			}

			string exampleFolder = GetExampleFolderPathOnDesktop("UpdateMemberCrossSection");
			string fileName = "simple cleat - member update.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);

			//Save the applied template
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
