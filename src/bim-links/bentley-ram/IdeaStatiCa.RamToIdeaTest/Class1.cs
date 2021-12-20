using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdea;
using NUnit.Framework;
using System.IO;

namespace IdeaStatiCa.RamToIdeaTest
{
	[TestFixture]
	public class Class1
	{
		[Test]
		public void Test()
		{
			using (RamDatabase ramDatabase = RamDatabase.Create(@"C:\Users\dalibor.bacovsky\Downloads\PoCStructure.rss"))
			{
				JsonPersistence persistence = new JsonPersistence();
				Project project = new Project(new NullLogger(), persistence);

				var importer = BimImporter.BimImporter.Create(ramDatabase.GetModel(), project, new NullLogger());

				var xml = Tools.ModelToXml(importer.ImportConnections());
				File.WriteAllText("iom.xml", xml);
			}
		}
	}
}