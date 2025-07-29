using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.RcsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Calculates all avaliable sections in an existing RCS project.
		/// </summary>
		/// <param name="rcsClient">The connected RCS API Client</param>
		public static async Task ImportReinforcedCrossSection(IRcsApiClient rcsClient)
		{
			string filePath = "Inputs/Project1.IdeaRcs";

			RcsProject rcsProject = await rcsClient.Project.OpenProjectAsync(filePath);

			//Get the list of avaliable reinforced cross-sections in the project
			List<RcsReinforcedCrossSection> reinforcedCrossSections = await rcsClient.CrossSection.ReinforcedCrossSectionsAsync(rcsClient.Project.ProjectId);

			//Print avaliable Reinforced Cross Sections in the project
			foreach (var css in reinforcedCrossSections)
			{
				Console.WriteLine($"Id={css.Id} Name={css.Name}");
			}

			//Define the import settings. Set an Id or Provide 0
			RcsReinforcedCrosssSectionImportSetting importSetting = new RcsReinforcedCrosssSectionImportSetting
			{
				//Provide Id of new Refinforced Cross-section
				ReinforcedCrossSectionId = 10,
				PartsToImport = "Complete"
			};

			//Reference to .Nav file with the Cross-section defined.
			string navFilePath = "Inputs/rect-L-4-2.nav";
			string templateXML = "";

			using (var sr = new StreamReader(navFilePath))
			{
				// Read the stream as a string, and write the string to the console.
				templateXML = await sr.ReadToEndAsync();
			}

			RcsReinforcedCrossSectionImportData importData = new RcsReinforcedCrossSectionImportData()
			{
				Setting = importSetting,
				Template = templateXML
			};

			//Reinforced Cross-section is updated and returned
			RcsReinforcedCrossSection newReinforcedCrossSection = await rcsClient.CrossSection.ImportReinforcedCrossSectionAsync(rcsClient.Project.ProjectId, importData);


			//Get the list of avaliable reinforced cross-sections in the project
			List<RcsReinforcedCrossSection> reinforcedCrossSectionsUpdated = await rcsClient.CrossSection.ReinforcedCrossSectionsAsync(rcsClient.Project.ProjectId);

			//Print avaliable Reinforced Cross Sections in the project
			foreach (var css in reinforcedCrossSectionsUpdated)
			{
				Console.WriteLine($"Id={css.Id} Name={css.Name}");
			}

			//We will now want to assign the new reinforced cross-section to a Section.

			//Get the list of avaliable sections in the project
			List<RcsSection> sections = await rcsClient.Section.SectionsAsync(rcsClient.Project.ProjectId);
			//Print avaliable Reinforced Cross Sections in the project
			foreach (var section in sections)
			{
				Console.WriteLine($"Id={section.Id} Name={section.Name}");
			}

			//Find the section inwhich we want to update. In this case there is a Section with the Name 'SectionA'
			RcsSection sectionToUpdate = sections.Where(x => x.Name == "S 1").First();

			//Find the reinforced cross-section which we want to set to the section.
			sectionToUpdate.RCSId = newReinforcedCrossSection.Id;

			//Section is updated and returned
			RcsSection updatedSection = await rcsClient.Section.UpdateSectionAsync(rcsClient.Project.ProjectId, sectionToUpdate);

			//Get a folder on Desktop for the Example
			string exampleFolder = GetExampleFolderPathOnDesktop("Add Reinforced Cross Section");

			// Save updated file.
			string fileName = "Project1_wAddedCss.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);


			await rcsClient.Project.SaveProjectAsync(rcsClient.Project.ProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//close the project
			await rcsClient.Project.CloseProjectAsync(rcsClient.Project.ProjectId);
		}

		/// <summary>
		/// Calculates all avaliable sections in an existing RCS project.
		/// </summary>
		/// <param name="rcsClient">The connected RCS API Client</param>
		public static async Task UpdateReinforcedCrossSectionReoLayout(IRcsApiClient rcsClient)
		{
			string filePath = "Inputs/Project1.IdeaRcs";

			RcsProject rcsProject = await rcsClient.Project.OpenProjectAsync(filePath);

			//Get the list of avaliable reinforced cross-sections in the project
			List<RcsReinforcedCrossSection> reinforcedCrossSections = await rcsClient.CrossSection.ReinforcedCrossSectionsAsync(rcsClient.Project.ProjectId);

			//Print avaliable Reinforced Cross Sections in the project
			foreach (var css in reinforcedCrossSections)
			{
				Console.WriteLine($"Id={css.Id} Name={css.Name}");
			}

			RcsReinforcedCrossSection crossSectionToUpdate = reinforcedCrossSections[0];

			//Define the import settings. Set an Id or Provide 0
			RcsReinforcedCrosssSectionImportSetting importSetting = new RcsReinforcedCrosssSectionImportSetting
			{
				//Provide Id of new Refinforced Cross-section
				ReinforcedCrossSectionId = crossSectionToUpdate.CrossSectionId,
				//We can choose between 'Reinf', 'Css' or 'Tendon' or 'Complete'
				PartsToImport = "Reinf"
			};

			//Reference to .Nav file with the Cross-section defined.
			string navFilePath = "Inputs/rect-L-4-2.nav";
			string templateXML = "";

			using (var sr = new StreamReader(navFilePath))
			{
				// Read the stream as a string, and write the string to the console.
				templateXML = await sr.ReadToEndAsync();
			}

			RcsReinforcedCrossSectionImportData importData = new RcsReinforcedCrossSectionImportData()
			{
				Setting = importSetting,
				Template = templateXML
			};

			//Reinforced Cross-section is updated and returned
			RcsReinforcedCrossSection newReinforcedCrossSection = await rcsClient.CrossSection.ImportReinforcedCrossSectionAsync(rcsClient.Project.ProjectId, importData);

			//Get a folder on Desktop for the Example
			string exampleFolder = GetExampleFolderPathOnDesktop("Update Reinforced Cross Section Layout");

			// Save updated file.
			string fileName = "Project1_UpdateLayout.ideaRcs";
			string saveFilePath = Path.Combine(exampleFolder, fileName);


			await rcsClient.Project.SaveProjectAsync(rcsClient.Project.ProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//close the project
			await rcsClient.Project.CloseProjectAsync(rcsClient.Project.ProjectId);
		}


	}
}
