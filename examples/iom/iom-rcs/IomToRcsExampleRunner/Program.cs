using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin.Api.RCS;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.RcsClient.Factory;
using IomToRcsExamples;

namespace IomToRcsExampleRunner
{
	internal class Program
	{
		private static IdeaStatiCa.Plugin.IPluginLogger Logger;

		static Program()
		{
			// initialize logger
			// idea log file can be found in 'C:\Users\USER_NAME\AppData\Local\Temp\IdeaStatiCa\Logs\'
			// This example creates a log 'IomToRcsExampleRunner.log'
			SerilogFacade.Initialize();
			Logger = LoggerProvider.GetLogger("iomtorcsexamplerunner");
		}

		static async Task Main(string[] args)
		{
			IRcsApiController? client = null;
			try
			{
				//Lets Create the Open Model 

				var exampleToSave = RcsExampleBuilder.Example.ReinforcedBeam;

				Logger.LogDebug($"Building '{exampleToSave}'");

				OpenModel openModel = RcsExampleBuilder.BuildExampleModel(exampleToSave);

				string path = Path.Combine(exampleToSave.ToString() + ".xml");

				Logger.LogDebug($"Saving '{path}'");

				openModel.SaveToXmlFile(path);

				#region Create Rcs Project

				string directoryPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 24.0\\net6.0-windows";
				//string directoryPath = "C:\\Dev\\IdeaStatiCa\\bin\\Debug\\net6.0-windows";

				Logger.LogDebug($"Opening RCS Client from '{directoryPath}'");

				var rcsClientFactory = new RcsClientFactory(directoryPath, Logger);

				client = await rcsClientFactory.CreateRcsApiClient();

				Logger.LogDebug($"Creating an RCS Project form IOM");

				var created = await client.CreateProjectFromIOMAsync(openModel, CancellationToken.None);

				//var created2 = await client.CreateProjectFromIOMFileAsync("IomToRcsExampleRunner.xml", CancellationToken.None);
				//"IomToRcsExampleRunner.xml"

				string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

				var rcsFileName = Path.Combine(savePath, exampleToSave.ToString() + ".ideaRcs");
				await client.SaveProjectAsync(rcsFileName, CancellationToken.None); ;

				Logger.LogDebug($"Rcs Project was saved '{rcsFileName}'");

				//client.OpenProjectAsync();
				#endregion
			}
			catch(Exception ex)
			{
				Logger.LogWarning("Creatin RCS project from IOM failed", ex);
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				client?.Dispose();
			}
		}

	}
}