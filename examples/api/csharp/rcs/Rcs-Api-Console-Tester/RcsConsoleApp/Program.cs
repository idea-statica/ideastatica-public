using IdeaRS.OpenModel;
using IdeaStatiCa.Api.RCS;
using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.RcsClient.Factory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RcsApiConsoleApp
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			///WARNING!!!
			///DO NOT MODIFY REGIONS IN THIS FILE AS THEY ARE USED IN BUILDING DOCUMENTATION.
			///WARNING!!!

			#region create_client


			//Directory to IDEA StatiCa installation on your computer.
			string directoryPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 24.1";
			try
			{
				//Pass path to a new RCS Client Factory
				using (var rcsClientFactory = new RcsClientFactory(directoryPath, new IdeaStatiCa.Plugin.NullLogger()))
				{

					//Create the client from the Factory
					using (IRcsApiController client = await rcsClientFactory.CreateRcsApiClient())
					{
						#endregion

						bool existingproject = true;

						if (existingproject)
						{
							#region open_existing

							//Getting the directory path to the sample file in example project.
							string samplePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

							//filepath to existing .ideaRcs project
							string rcsFilePath = Path.Combine(samplePath, "SampleFiles\\Reinforced concrete T-section.IdeaRcs");

							//Opens project on the server side to start performing operations
							bool okay = await client.OpenProjectAsync(rcsFilePath, CancellationToken.None);

							#endregion
						}
						else
						{
							bool fromIomModel = true;

							if (fromIomModel)
							{
								#region open_from_model

								//OpenModel defined in Memory
								OpenModel model = new OpenModel();

								await client.CreateProjectFromIOMAsync(model, CancellationToken.None);

								#endregion
							}

							else
							{
								#region open_from_iom_file

								//Filepath to existing Iom XML file to be convert to an RCS Project
								string iomFilePath = "pathToIoM.xml";

								await client.CreateProjectFromIOMFileAsync(iomFilePath, CancellationToken.None);

								#endregion
							}
						}

						#region calculate_project

						List<RcsSectionResultOverview> briefResults = await client.CalculateAsync(new RcsCalculationParameters(), CancellationToken.None);

						JToken parsedJson = JToken.Parse(JsonConvert.SerializeObject(briefResults));
						string output = parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);

						//Print brief results to the Console
						Console.Write(output);

						#endregion

						#region section_results

						//Get List of Sections
						List<RcsSection> sections = await client.GetProjectSectionsAsync(CancellationToken.None);

						//Set Detailed Result Parameters
						//Selecting only the first section in the Project
						RcsResultParameters resultParams = new RcsResultParameters()
						{
							Sections = new List<int>() { sections[0].Id }
						};

						List<RcsSectionResultDetailed> detailedResult = client.GetResultsAsync(resultParams, CancellationToken.None).Result;

						JToken parsedJsonResult = JToken.Parse(JsonConvert.SerializeObject(briefResults));
						string outputresults = parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);

						//Print brief results to the Console
						Console.Write(outputresults);

						#endregion

						#region save_project

						string saveFilePath = "newSavePath.ideaRcs";

						//Save project - either provide opened path or provide a new path.
						await client.SaveProjectAsync(saveFilePath, CancellationToken.None);

						#endregion

					}
				}
			}
			catch (Exception ex)
			{
				// report an error and return error
				Console.WriteLine($"RcsApiConsoleApp failed : {ex.Message}");
				Environment.Exit(1);
			}

			// success
			Environment.Exit(0);
		}
	}
}