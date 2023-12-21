using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaStatiCa.RcsClient.Client;
using IdeaStatiCa.RcsClient.Factory;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using IdeaRS.OpenModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RcsApiConsoleApp
{
	internal class Program
	{
		static async Task Main(string[] args)
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

					await client.CreateProjectFromIOMAsync(model, CancellationToken.None);

					#endregion
				}

				else
				{
					#region openfromiomfile

					//Filepath to existing Iom XML file to be convert to an RCS Project
					string iomFilePath = "pathToIoM.xml";

					await client.CreateProjectFromIOMFileAsync(iomFilePath, CancellationToken.None);

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
	}
}