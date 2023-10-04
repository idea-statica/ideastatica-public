using IdeaStatiCa.Plugin;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace ConnCalculatorConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Start");

			if (args.Length < 1)
			{
				Console.WriteLine("The path to Idea StatiCa installation dir is required as the argument");
				return;
			}

			if (!Directory.Exists(args[0]))
			{
				Console.WriteLine(string.Format("Missing Idea StatiCa installation in '{0}'", args[0]));
				return;
			}

			string ideaStaticaInstallDir = args[0];
			Console.WriteLine(string.Format("Using Idea StatiCa from '{0}'", ideaStaticaInstallDir));

			// the default idea connection project for calculation
			string ideaConnProjectFileName = "testProj.ideaCon";

			if (args.Length > 1)
			{
				// custom idea connection project can be specified in the second argumet
				ideaConnProjectFileName = args[1];

				if(!File.Exists(ideaConnProjectFileName))
				{
					Console.WriteLine(string.Format("The project file '{0}' doesn't exist", ideaConnProjectFileName));
					return;
				}
			}

			// create the instance of factory - it looks for IDEA StatiCa in the directory 'ideaStaticaInstallDir'
			ConnHiddenClientFactory calcFactory = new ConnHiddenClientFactory(ideaStaticaInstallDir);

			//create the instance of the IDEA StatiCa Client
			ConnectionHiddenCheckClient client = calcFactory.Create();
			try
			{
				// open project on the service side
				client.OpenProject(ideaConnProjectFileName);

				try
				{
					// get detail about idea connection project
					var projInfo = client.GetProjectInfo();

					if (projInfo != null && projInfo.Connections != null)
					{
						// iterate all connections in the project
						foreach (var con in projInfo.Connections)
						{
							Console.WriteLine(string.Format("Starting calculation of connection {0}", con.Identifier));

							// calculate a get results for each connection in the project
							var cbfemResults = client.Calculate(con.Identifier);
							Console.WriteLine("Calculation is done");

							var jsonSetting = new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Culture = CultureInfo.InvariantCulture };

							var jsonFormating = Newtonsoft.Json.Formatting.Indented;
							string resultsJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(cbfemResults, jsonFormating, jsonSetting);

							Console.WriteLine(resultsJsonString);

							// get the geometry of the connection in XML format
							var geometryJsonString = client.GetConnectionModelXML(con.Identifier);

							// convert it ir the instance
							var conData = Tools.ConnectionDataFromXml(geometryJsonString);
							Debug.Assert(conData != null, "Worng format");

							Console.WriteLine("");
							Console.WriteLine("The geometry of the calculated project");
							Console.WriteLine(geometryJsonString);
						}
					}
				}
				finally
				{
					// Delete temps in case of a crash
					client.CloseProject();
				}
			}
			finally
			{
				if (client != null)
				{
					client.Close();
				}
			}

			Console.WriteLine("End");
		}
	}
}
