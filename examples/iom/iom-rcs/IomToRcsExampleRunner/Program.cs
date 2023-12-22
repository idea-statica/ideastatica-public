using IdeaRS.OpenModel;
using IdeaStatiCa.RcsClient.Factory;
using IomToRcsExamples;

namespace IomToRcsExampleRunner
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			//Lets Create the Open Model 

			var exampleToSave = RcsExampleBuilder.Example.ReinforcedBeam;

			OpenModel openModel = RcsExampleBuilder.BuildExampleModel(exampleToSave);

			string path = Path.Combine(exampleToSave.ToString() + ".xml");

			openModel.SaveToXmlFile(path);

			#region Create Rcs Project

			string directoryPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 23.1\\net6.0-windows";

			var rcsClientFactory = new RcsClientFactory(directoryPath);

			var client = await rcsClientFactory.CreateRcsApiClient();

			var created = client.CreateProjectFromIOMAsync(openModel, CancellationToken.None);

			//var created2 = await client.CreateProjectFromIOMFileAsync("IomToRcsExampleRunner.xml", CancellationToken.None);
			//"IomToRcsExampleRunner.xml"

			string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		
			//await client.SaveProjectAsync(Path.Combine(savePath, exampleToSave.ToString() + ".ideaRcs"), CancellationToken.None);

			//client.OpenProjectAsync();
			#endregion
		}

	}
}