using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IOM.GeneratorExample;
using System;
using System.IO;

namespace IOM.SteelFrame
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Start generate example of IOM...");

			//create IOM and results
			OpenModel example = SteelFrameExample.CreateIOM();
			OpenModelResult result = Helpers.GetResults();

			var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

			//OPTIONAL save to the files
			example.SaveToXmlFile(Path.Combine(desktopDir, "IOM-SteelFrame.xml"));
			result.SaveToXmlFile(Path.Combine(desktopDir, "IOM-SteelFrame.xmlR"));

			//Generate IDEA Connection by web service
			Console.WriteLine("Generating IDEA Connection project by web service");
			var fileConnFileNameFromWeb = Path.Combine(desktopDir, "connectionFromIOM-web.ideaCon");
			
			WebServiceHelpers.CreateOnServer(example, result, fileConnFileNameFromWeb);

			// end console application
			Console.WriteLine("Done. Press any key to exit.");
			Console.ReadKey();
		}
	}
}
