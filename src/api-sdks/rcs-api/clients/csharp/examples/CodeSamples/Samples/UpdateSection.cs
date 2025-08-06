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
		public static async Task UpdateSection(IRcsApiClient rcsClient)
		{
			string filePath = "Inputs/Project3.IdeaRcs";

			RcsProject rcsProject = await rcsClient.Project.OpenProjectAsync(filePath);

			//Get the list of avaliable reinforced cross-sections in the project
			List<RcsReinforcedCrossSection> reinforcedCrossSections = await rcsClient.CrossSection.ReinforcedCrossSectionsAsync(rcsClient.Project.ProjectId);

			//Print avaliable Reinforced Cross Sections in the project
			Console.WriteLine("Avaliable Reinforced Cross-sections");
			foreach (var css in reinforcedCrossSections)
			{
				Console.WriteLine($"Id={css.Id} Name={css.Name}");
			}

			//Get the list of avaliable sections in the project
			List<RcsSection> sections = await rcsClient.Section.SectionsAsync(rcsClient.Project.ProjectId);

			//Print avaliable Reinforced Cross Sections in the project
			Console.WriteLine("Avaliable Sections");
			foreach (var section in sections)
			{
				Console.WriteLine($"Id={section.Id} Name={section.Description}");
			}

			// Prompt until a valid section is selected
			RcsSection sectionToUpdate = null;
			while (sectionToUpdate == null)
			{
				Console.Write("\nEnter the name of the section to update: ");
				string sectionName = Console.ReadLine();

				sectionToUpdate = sections.FirstOrDefault(x => x.Description.Equals(sectionName, StringComparison.OrdinalIgnoreCase));

				if (sectionToUpdate == null)
				{
					Console.WriteLine("Invalid section name. Please try again.");
				}
			}

			// Prompt until a valid reinforced cross-section is selected
			RcsReinforcedCrossSection newReinforcedCrossSection = null;
			while (newReinforcedCrossSection == null)
			{
				Console.Write("\nEnter the name of the reinforced cross-section to apply: ");
				string rcsName = Console.ReadLine();

				newReinforcedCrossSection = reinforcedCrossSections
					.FirstOrDefault(x => x.Name.Equals(rcsName, StringComparison.OrdinalIgnoreCase));

				if (newReinforcedCrossSection == null)
				{
					Console.WriteLine("Invalid reinforced cross-section name. Please try again.");
				}
			}

			//Find the reinforced cross-section which we want to set to the section.
			sectionToUpdate.RCSId = newReinforcedCrossSection.Id;

			//Section is updated and returned
			RcsSection updatedSection = await rcsClient.Section.UpdateSectionAsync(rcsClient.Project.ProjectId, sectionToUpdate);

			//Get a folder on Desktop for the Example
			string exampleFolder = GetExampleFolderPathOnDesktop("Update Section");

			// Save updated file.
			string fileName = "Project3_SectionUpdated.ideaRcs";
			string saveFilePath = Path.Combine(exampleFolder, fileName);


			await rcsClient.Project.SaveProjectAsync(rcsClient.Project.ProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//close the project
			await rcsClient.Project.CloseProjectAsync(rcsClient.Project.ProjectId);
		}
	}
}
