using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using IOM.GeneratorExample;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IOM.SteelFrameDesktop
{
	class Program
	{
		private static string IdeaInstallDir;
		public static async Task Main(string[] args)
		{
			IdeaInstallDir = IOM.SteelFrameDesktop.Properties.Settings.Default.IdeaInstallDir;

			if (!Directory.Exists(IdeaInstallDir))
			{
				Console.WriteLine("IDEA StatiCa installation was not found in '{0}'", IdeaInstallDir);
				return;
			}

			Console.WriteLine("IDEA StatiCa installation directory is '{0}'", IdeaInstallDir);

			Console.WriteLine("Select the required example");
			Console.WriteLine("1  - Steel frame ECEN");
			Console.WriteLine("2  - Simple frame AUS");

			var option = Console.ReadLine();

			OpenModel iom = null;
			OpenModelResult iomResult = null;

			if (option.Equals("2", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("Generating Australian steel frame ...");

				// create IOM and results
				iom = SimpleFrameAUS.CreateIOM();
				iomResult = null;
			}
			else
			{
				Console.WriteLine("Generating ECEN steel frame ...");

				// create IOM and results
				iom = SteelFrameExample.CreateIOM();
				iomResult = Helpers.GetResults();
			}

			//Combine into a Container for Project Creation
			OpenModelContainer iomContainer = new OpenModelContainer()
			{
				OpenModel = iom,
				OpenModelResult = iomResult
			};

			string iomFileName = "example.xml";
			IdeaRS.OpenModel.Tools.OpenModelContainerToFile(iomContainer, iomFileName);

			var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			var fileConnFileNameFromLocal = Path.Combine(desktopDir, "connectionFromIOM-local.ideaCon");

			//Automatically start API service and Attach it.
			using (var clientFactory = new ConnectionApiServiceRunner(IdeaInstallDir))
			{
				// create ConnectionApiClient
				using (var conClient = await clientFactory.CreateApiClient())
				{
					try
					{
						//Create the Connection project from the IOM file.
						//This creates the project on the server side. 
						Console.WriteLine("Creating Idea connection project ");
						var projData = await conClient.Project.CreateProjectFromIomFileAsync(iomFileName, null);
						Console.WriteLine($"Generated Connection Project from IOM. Project is active on the Server with Guid: {conClient.ActiveProjectId}.", fileConnFileNameFromLocal);

						//Save connection to local computer.
						await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, fileConnFileNameFromLocal);
						Console.WriteLine($"Save Project to local disk at path: {fileConnFileNameFromLocal}.");

						//Close project on server.
						await conClient.Project.CloseProjectAsync(projData.ProjectId);
						Console.WriteLine($"Project with Id {conClient.ActiveProjectId} closed on Server");
					}
					catch (Exception e)
					{
						Console.WriteLine("Error '{0}'", e.Message);
					}
				}
			}

			// end console application
			Console.WriteLine("Done. Press any key to exit.");
			Console.ReadKey();
		}
	}
}
