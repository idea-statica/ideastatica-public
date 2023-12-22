#region rcsusings
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaStatiCa.RcsClient.Client;
using IdeaStatiCa.RcsClient.Factory;
#endregion
using IdeaRS.OpenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using IdeaStatiCa.Plugin.Api.RCS;

namespace RcsApiConsoleApp
{
	public static class RcsCodeSamples
	{
		public static async Task GettingStarted()
		{
			#region createclient
			//Directory to IDEA StatiCa installation on your computer.
			string directoryPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 23.1";

			//Pass path to a new RCS Client Factory
			var rcsClientFactory = new RcsClientFactory(directoryPath);

			//Create the client from the Factory
			IRcsApiController client = await rcsClientFactory.CreateRcsApiClient();
			#endregion

			bool existingproject = true;

			if (existingproject)
			{
				#region openexisting
				//filepath to existing .ideaRcs project
				string rcsFilePath = "";

				//Opens project on the server side to start performing operations
				bool okay = await client.OpenProjectAsync(rcsFilePath, CancellationToken.None);
				#endregion
			}
			else
			{
				bool fromIomModel = true;

				if (fromIomModel)
				{
					#region openfrommodel

					//OpenModel defined in Memory
					OpenModel model = new OpenModel();

					client.CreateProjectFromIOMAsync(model, CancellationToken.None);

					#endregion
				}

				else
				{
					#region openfromiomfile

					//Filepath to existing Iom XML file to be convert to an RCS Project
					string iomFilePath = "pathToIoM.xml";

					client.CreateProjectFromIOMFileAsync(iomFilePath, CancellationToken.None);

					#endregion
				}
			}

			#region calculateproject

			List<RcsSectionResultOverview> briefResults = await client.CalculateAsync(new RcsCalculationParameters(), CancellationToken.None);

			JToken parsedJson = JToken.Parse(JsonConvert.SerializeObject(briefResults));
			string output = parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);

			//Print brief results to the Console
			Console.Write(output);

			#endregion

			#region sectionresults
			
			//Get List of Sections
			List<RcsSectionModel> sections = await client.GetProjectSectionsAsync(CancellationToken.None);

			//Set Detailed Result Parameters
			//Selecting only the first section in the Project
			RcsResultParameters resultParams = new RcsResultParameters()
			{
				Sections = new List<int>() { sections[0].Id }
			};

			List<RcsDetailedResultForSection> detailedResult = client.GetResultsAsync(resultParams, CancellationToken.None).Result;

			JToken parsedJsonResult = JToken.Parse(JsonConvert.SerializeObject(briefResults));
			string outputresults = parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);

			//Print brief results to the Console
			Console.Write(outputresults);

			#endregion

			#region saveproject

			string saveFilePath = "newSavePath.ideaRcs";

			//Save project - either provide opened path or provide a new path.
			await client.SaveProjectAsync(saveFilePath, CancellationToken.None);

			#endregion

			#region dispose

			client.Dispose();
			rcsClientFactory.Dispose();

			#endregion
		}

		public static async Task ChangeSectionReiforcedSection(RcsApiClient client)
		{
			#region changereinforcedcrosssection
			//Get the list of avaliable reinforced cross-sections in the project
			List<ReinforcedCrossSectionModel> reinforcedCrossSections = await client.GetProjectReinforcedCrossSectionsAsync(CancellationToken.None);

			//Get the list of avaliable sections in the project
			List<RcsSectionModel> sections = await client.GetProjectSectionsAsync(CancellationToken.None);

			//Find the section inwhich we want to update. In this case there is a Section with the Name 'SectionA'
			RcsSectionModel sectionToUpdate = sections.Where(x => x.Name == "SectionA").First();

			//Find the reinforced cross-section which we want to set to the section.
			sectionToUpdate.RCSId = reinforcedCrossSections.Where(x => x.Name == "RCS2").First().Id;

			//Section is updated and returned
			RcsSectionModel updatedSection = await client.UpdateSectionAsync(sectionToUpdate, CancellationToken.None);
			#endregion
		}

		public static async Task ChangeReinforcedSectionLayout(RcsApiClient client)
		{
			#region changereinforcedcsslayout
			//Get the list of avaliable reinforced cross-sections in the project
			List<ReinforcedCrossSectionModel> reinforcedCrossSections = await client.GetProjectReinforcedCrossSectionsAsync(CancellationToken.None);

			//Find the reinforced cross-section which we want to update in the project.
			ReinforcedCrossSectionModel reinforcedCrossSection = reinforcedCrossSections.Where(x => x.Name == "RCS2").First();

			//Define the import settings
			ReinfCssImportSetting importSetting = new ReinfCssImportSetting();

			importSetting.ReinfCssId = reinforcedCrossSection.Id;
			
			//We can choose between 'Reinf', 'Css' or 'Tendon' or 'Complete'
			importSetting.PartsToImport = "Reinf";

			//Filepath to nav file
			string navFilePath = "templatePath.nav";
			string templateXML = "";

			using (var sr = new StreamReader(navFilePath))
			{
				// Read the stream as a string, and write the string to the console.
				templateXML = await sr.ReadToEndAsync();
			}

			//Reinforced Cross-section is updated and returned
			ReinforcedCrossSectionModel updatedSection = await client.ImportReinfCssAsync(importSetting, templateXML, CancellationToken.None); 
			#endregion
		}

		public static async Task AddNewReinforcedCrossSection(RcsApiClient client)
		{
			#region addreinforcedcss
			//Get the list of avaliable reinforced cross-sections in the project
			List<ReinforcedCrossSectionModel> reinforcedCrossSections = await client.GetProjectReinforcedCrossSectionsAsync(CancellationToken.None);

			//Find the reinforced cross-section which we want to update in the project.
			ReinforcedCrossSectionModel reinforcedCrossSection = reinforcedCrossSections.Where(x => x.Name == "RCS2").First();

			//Define the import settings
			ReinfCssImportSetting importSetting = new ReinfCssImportSetting();

			//Provide Id of new Refinforced Cross-section
			importSetting.ReinfCssId = 30;
			
			//Set the Type to Complete
			importSetting.PartsToImport = "Complete";

			string navFilePath = "templatePath.nav";
			string templateXML = "";

			using (var sr = new StreamReader(navFilePath))
			{
				// Read the stream as a string, and write the string to the console.
				templateXML = await sr.ReadToEndAsync();
			}

			//Reinforced Cross-section is updated and returned
			ReinforcedCrossSectionModel newReinforcedCrossSection = await client.ImportReinfCssAsync(importSetting, templateXML, CancellationToken.None);

			//We will now want to assign the new reinforced cross-section to a Section.

			//Get the list of avaliable sections in the project
			List<RcsSectionModel> sections = await client.GetProjectSectionsAsync(CancellationToken.None);

			//Find the section inwhich we want to update. In this case there is a Section with the Name 'SectionA'
			RcsSectionModel sectionToUpdate = sections.Where(x => x.Name == "SectionA").First();

			//Find the reinforced cross-section which we want to set to the section.
			sectionToUpdate.RCSId = newReinforcedCrossSection.Id;

			//Section is updated and returned
			RcsSectionModel updatedSection = await client.UpdateSectionAsync(sectionToUpdate, CancellationToken.None);

			#endregion
		}

	}
}
