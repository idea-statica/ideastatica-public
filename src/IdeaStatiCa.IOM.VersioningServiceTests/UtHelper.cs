using NUnit.Framework;
using System.Linq;
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

			var expected = XElement.Parse(expectedXml);
			Normalize(expected);

			var actual = XElement.Parse(actualXml);
			Normalize(actual);

			Assert.IsTrue(XNode.DeepEquals(expected, actual),
				string.Format("{0} \n does not equal \n{1}", actualXml, expectedXml));
		}

		private static void Normalize(XElement element)
		{
			// sort attributes
			List<XAttribute> orderedAttributes = element.Attributes()
				.OrderBy(x => x.Name.ToString())
				.ToList();
			
			element.ReplaceAttributes(orderedAttributes);

			// recursively sort children
			List<XNode> orderedChildren = element.Nodes()
				.OrderBy(x => x, Comparer<XNode>.Create(CompareNodes))
				.ToList();
			
			foreach (var child in orderedChildren.OfType<XElement>())
			{
				Normalize(child);
			}

			element.ReplaceNodes(orderedChildren);
		}

		private static int CompareNodes(XNode x, XNode y)
		{
			if(x is XElement xe && y is XElement ye)
			{
				return string.Compare(xe.Name.ToString(), ye.Name.ToString(), StringComparison.Ordinal);
			}
			else if (x is XText xt && y is XText yt)
			{
				return string.Compare(xt.Value, yt.Value, StringComparison.Ordinal);
			}
			else
			{
				return 0;
			}
		}
	}
}
