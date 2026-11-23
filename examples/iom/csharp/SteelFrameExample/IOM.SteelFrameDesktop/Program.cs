using IdeaRS.OpenModel;
using IdeaStatiCa.ConnectionApi;
using IOM.SteelFrameModel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IOM.SteelFrameDesktop
{
	/// <summary>
	/// Builds a frame in IOM, saves it, and creates an IDEA StatiCa Connection project from it with the Connection API
	/// of the IDEA StatiCa installed on this computer.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// Where IDEA StatiCa 26.1 is installed unless the user chooses another folder.
		/// </summary>
		private const string DefaultSetupDir = @"C:\Program Files\IDEA StatiCa\StatiCa 26.1";

		/// <summary>
		/// Asks which frame to build, then builds it, saves it and creates the Connection project.
		/// </summary>
		/// <param name="args">
		/// Optional: the folder IDEA StatiCa is installed in. Without it, the app uses <see cref="DefaultSetupDir"/>.
		/// </param>
		/// <returns>0 when the project is saved, 1 when IDEA StatiCa is not found or the Connection API fails.</returns>
		private static async Task<int> Main(string[] args)
		{
			string setupDir = args.Length > 0 ? args[0] : DefaultSetupDir;
			if (!File.Exists(Path.Combine(setupDir, "IdeaStatiCa.ConnectionRestApi.exe")))
			{
				Console.Error.WriteLine($"IDEA StatiCa is not in '{setupDir}'. Give the folder it is installed in as the first argument.");
				return 1;
			}

			Console.WriteLine("Which frame?");
			Console.WriteLine("  1 - Steel frame, EN, with internal forces");
			Console.WriteLine("  2 - Simple frame, AS (Australia), without internal forces");
			bool australian = Console.ReadLine()?.Trim() == "2";

			string name = australian ? "SimpleFrameAus" : "SteelFrame";
			var container = australian
				? new OpenModelContainer { OpenModel = SimpleFrameAus.CreateIOM() }
				: new OpenModelContainer { OpenModel = SteelFrame.CreateIOM(), OpenModelResult = SteelFrame.LoadInternalForces() };

			string iomPath = Path.GetFullPath(name + ".xml");
			Tools.OpenModelContainerToFile(container, iomPath);
			Console.WriteLine($"The IOM is saved in {iomPath}");

			// Without the ids of the connection points, the Connection API creates only the first connection.
			var connectionIds = container.OpenModel.ConnectionPoint.Select(cp => cp.Id).ToList();
			string projectPath = Path.GetFullPath(name + ".ideaCon");
			try
			{
				Console.WriteLine($"Starting the Connection API of {setupDir} ...");
				using var runner = new ConnectionApiServiceRunner(setupDir);
				using var client = await runner.CreateApiClient();

				var project = await client.Project.CreateProjectFromIomFileAsync(iomPath, connectionIds);
				Console.WriteLine($"Connections in the project: {string.Join(", ", project.Connections.Select(c => c.Name))}");

				await client.Project.SaveProjectAsync(project.ProjectId, projectPath);
				await client.Project.CloseProjectAsync(project.ProjectId);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"The Connection API failed: {ex.Message}");
				return 1;
			}

			Console.WriteLine($"The project is saved in {projectPath}");
			return 0;
		}
	}
}
