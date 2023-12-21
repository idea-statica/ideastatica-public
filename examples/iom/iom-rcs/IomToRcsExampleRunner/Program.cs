using IdeaRS.OpenModel;
using IomToRcsExamples;
using IdeaStatiCa.RcsClient;
using System.Text;
using System.Xml.Serialization;
using System;
using Microsoft.VisualBasic;
using System.Xml;
using IdeaStatiCa.RcsClient.Factory;
using IdeaStatiCa.RcsClient.Client;

namespace IomToRcsExampleRunner
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//Lets Create the Open Model 

			var exampleToSave = RcsExampleBuilder.Example.ReinforcedBeam;

			OpenModel openModel = RcsExampleBuilder.BuildExampleModel(exampleToSave);

			string path = Path.Combine(exampleToSave.ToString() + ".xml");

			openModel.SaveToXmlFile(path);

			#region Create Rcs Project

			//string directoryPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 23.1\\net6.0-windows";

			//var rcsClientFactory = new RcsClientFactory(directoryPath);

			//var client = await rcsClientFactory.CreateRcsApiClient();

			//var created = client.CreateProjectFromIOMAsync(openModel, CancellationToken.None);

			//var created = await client.CreateProjectFromIOMFileAsync("IomToRcsExampleRunner.xml", CancellationToken.None);
			////"IomToRcsExampleRunner.xml"

			//string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			//string path = Path.Combine(savePath, exampleToSave.ToString() + ".ideaRcs");

			//await client.SaveProjectAsync(Path.Combine(savePath, exampleToSave.ToString() + ".ideaRcs"), CancellationToken.None);

			////client.OpenProjectAsync();
			#endregion
		}

	}
}