using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IntermediateModel.Upgrade;
using System.Xml.Linq;

namespace IdeaStatiCa.IntermediateModel.Tests
{

	[TestFixture]
	public class UpgradeTests
	{
		private IXmlParsingIRService _xmlParsingIRService;
		private IIRExportToXMLService _iRExportToXMLService;

		private IIRUpgradeService _iRUpgradeService;

		private readonly string TestData = "TestData";

		[SetUp]
		public void SetUp()
		{
			_xmlParsingIRService = new XmlParsingService();

			_iRExportToXMLService = new IRExportToXMLService();

			_iRUpgradeService = new IRUpgradeService();
		}


		[Test]
		public void Upgrate_WithoutVersion_ShouldFail()
		{
			string xmlContent = "<root><person><name>John Doe</name><age>30</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);

			_iRUpgradeService.LoadModel(model);

			Assert.That(() => _iRUpgradeService.Upgrade(), Throws.Exception);

		}

		[Test]
		public void FromOriginalXml_UpgradeNotAffect_ReturnsSameXML()
		{
			string xmlContent = "<root><person><name>John Doe</name><age>30</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);

			model.Version = "2";
			_iRUpgradeService.LoadModel(model);

			//expect throwing exception open model/model bim missing in data
			Assert.That(() => _iRUpgradeService.Upgrade(), Throws.Exception);

			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(anObject: exportedXML);
			AssertEqualXml(xmlContent, exportedXML);

		}





		[TestCase("OpenModel-Simple.xml", "OpenModel-SimpleUpgraded.xml")]
		[TestCase("OpenModel-Vantaa.xml", "OpenModel-VantaaUpgraded.xml")]
		[TestCase("ModelBIM-AS_Black_Point.xml", "ModelBIM-AS_Black_PointUpgraded.xml")]
		public void FromParsedXml_Upgrade_LargeFile(string fileName, string expectedFile)
		{
			string xmlContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, fileName));

			string xmlExpectedContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, expectedFile));

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			_iRUpgradeService.LoadModel(model);

			_iRUpgradeService.Upgrade();


			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(anObject: exportedXML);
			File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, "res.xml"), exportedXML);

			AssertEqualXml(xmlExpectedContent, exportedXML);

		}

		private void AssertEqualXml(string expectedXml, string actualXml)
		{
			Assert.IsTrue(XNode.DeepEquals(XElement.Parse(expectedXml), XElement.Parse(actualXml)),
				String.Format("{0} \n does not equal \n{1}", actualXml, expectedXml));
		}
	}

}