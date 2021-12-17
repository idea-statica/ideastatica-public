using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdea;
using NUnit.Framework;
using System;
using System.IO;

namespace IdeaStatiCa.RamToIdeaTest
{
	[TestFixture]
    public class Class1
    {
		[Test]
		public void Test()
		{
			using (RamDatabase ramDatabase = RamDatabase.Create("PoCStructure.rss"))
			{
				Plugin.ModelBIM modelBim = ramDatabase.GetModelBIM();
				var xml = Tools.ModelToXml(modelBim);
				File.WriteAllText("iom.xml", xml);
			}
		}
    }
}
