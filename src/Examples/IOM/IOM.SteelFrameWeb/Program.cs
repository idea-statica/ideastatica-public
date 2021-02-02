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

			// save to the files
			example.SaveToXmlFile(Path.Combine(desktopDir, "IOM-SteelFrame.xml"));
			result.SaveToXmlFile(Path.Combine(desktopDir, "IOM-SteelFrame.xmlR"));

			#region Generatig IDEA Connection by web service
			Console.WriteLine("Generating IDEA Connection project by web service");
			var fileConnFileNameFromWeb = Path.Combine(desktopDir, "connectionFromIOM-web.ideaCon");
			SteelFrameExample.CreateOnServer(example, result, fileConnFileNameFromWeb);
			#endregion

			// end console application
			Console.WriteLine("Done. Press any key to exit.");
			Console.ReadKey();
		}
	}
}
