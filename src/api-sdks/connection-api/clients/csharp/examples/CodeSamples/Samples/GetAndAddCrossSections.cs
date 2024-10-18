using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get the list of avaliable Cross-sections in a project and Add some to the project. 
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetAndAddCrossSections(ConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			//Get projectId Guid
			Guid projectId = conProject.ProjectId;

			Dictionary<string, int> CrossSectionMap = new Dictionary<string, int>();
			Dictionary<string, int> SteelMaterialMap = new Dictionary<string, int>();

			List<IdeaRS.OpenModel.Material.MatSteel> steelMaterials = (await conClient.Material.GetSteelMaterialsAsync(projectId)).Cast<IdeaRS.OpenModel.Material.MatSteel>().ToList();
			steelMaterials.ForEach(x => SteelMaterialMap.Add(x.Name, x.Id));

			List<IdeaRS.OpenModel.CrossSection.CrossSection> crossSections = (await conClient.Material.GetCrossSectionsAsync(projectId)).Cast<IdeaRS.OpenModel.CrossSection.CrossSection>().ToList();
			crossSections.ForEach(x => CrossSectionMap.Add(x.Name, x.Id));

			//List of new Cross-Sections to Add.
			List<ConMprlCrossSection> crossSectionToAdd = new List<ConMprlCrossSection>() 
			{ 
				new ConMprlCrossSection("S 355", "IPE240"),
				new ConMprlCrossSection("S 275", "IPE300"),
				new ConMprlCrossSection("S 355", "IPE450") 
			};

			foreach (var section in crossSectionToAdd)
			{
				if (!SteelMaterialMap.ContainsKey(section.MaterialName))
				{
					//FIX: Add Materal should return the Material not the ConMprlElement
					ConMprlElement addedMaterial = await conClient.Material.AddMaterialSteelAsync(projectId, new ConMprlElement(section.MaterialName));
					Console.WriteLine("Successfully Added new Material: " + addedMaterial.MprlName);
				}
				else
					Console.WriteLine("Material already in project:" + section.MaterialName);

				//Only Add Assemblies which are not in the model currently.
				if (!CrossSectionMap.ContainsKey(section.MprlName))
				{
					//FIX: This should Output the created Bolt Assembly Object. We need the ID.
					ConMprlCrossSection added = await conClient.Material.AddCrossSectionAsync(projectId, new ConMprlCrossSection("S 355", section.MprlName));
					Console.WriteLine("Successfully Added new Cross-section: " + added.MprlName);

					//Need to check what happens if name is not found...
				}
				else
					Console.WriteLine("cross-section already in project:" + section);
			}

			string exampleFolder = GetExampleFolderPathOnDesktop("GetAndAddCrossSections");
			string fileName = "simple cleat connection-addedSection.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);

			//Save the applied template
			await conClient.Project.SaveProjectAsync(projectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			await conClient.Project.CloseProjectAsync(projectId);
		}
	}
}
