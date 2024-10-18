using NUnit.Framework;
using System.Xml.Linq;

namespace IdeaStatiCa.IOM.VersioningServiceTests
{
	public static class UtHelper
	{
		public static void Attach(string content, string fileName)
		{
			string path = Path.Combine(TestContext.CurrentContext.WorkDirectory, fileName);
			File.WriteAllText(path, content);
			TestContext.AddTestAttachment(path);
		}

		public static void AssertEqualXml(string expectedXml, string actualXml, string testCaseName)
		{
			IdeaStatiCa.IOM.VersioningServiceTests.UtHelper.Attach(expectedXml, $"{testCaseName}-expected.xml");
			IdeaStatiCa.IOM.VersioningServiceTests.UtHelper.Attach(actualXml, $"{testCaseName}-actual.xml");

			Assert.IsTrue(XNode.DeepEquals(XElement.Parse(expectedXml), XElement.Parse(actualXml)),
				String.Format("{0} \n does not equal \n{1}", actualXml, expectedXml));
		}
	}
}
