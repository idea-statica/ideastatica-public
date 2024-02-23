using IdeaStatiCa.IntermediateModel.Downgrade;
using IdeaStatiCa.IntermediateModel.IRModel;
using System.Xml.Linq;

namespace IdeaStatiCa.IntermediateModel.Tests
{

	[TestFixture]
	public class DowngradeTests
	{
		private IXmlParsingIRService _xmlParsingIRService;
		private IIRExportToXMLService _iRExportToXMLService;

		private IIRDowngradeService _iRDowngradeService;

		private readonly string TestData = "TestData";

		[SetUp]
		public void SetDown()
		{
			_xmlParsingIRService = new XmlParsingService();

			_iRExportToXMLService = new IRExportToXMLService();

			_iRDowngradeService = new IRDowngradeService();
		}


		[Test]
		public void Downgrade_WithoutVersion_ShouldFail()
		{
			string xmlContent = "<root><person><name>John Doe</name><age>30</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);

			_iRDowngradeService.LoadModel(model);

			Assert.That(() => _iRDowngradeService.Downgrade("2"), Throws.Exception);

		}

		[Test]
		public void FromOriginalXml_DowngradeNotAffect_ReturnsSameXML()
		{
			string xmlContent = "<root><person><name>John Doe</name><age>30</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);

			model.Version = "2.0.2";
			_iRDowngradeService.LoadModel(model);

			//expect throwing exception open model/model bim missing in data
			Assert.That(() => _iRDowngradeService.Downgrade("2"), Throws.Exception);

			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(anObject: exportedXML);
			AssertEqualXml(xmlContent, exportedXML);

		}

		[TestCase("2", 0)]
		[TestCase("2.0.1", 1)]
		[TestCase("2.0.2", 2)]
		public void FromParsedXml_CheckPossible_Downgrade_Steps(string version, int numOfSteps)
		{
			string xmlContent = $"<root><Version>{version}</Version><person><name>John Doe</name><age>30</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			_iRDowngradeService.LoadModel(model);


			Assert.IsTrue(_iRDowngradeService.GetVersionsToDowngrade().Count() == numOfSteps);

		}


		[TestCase("OpenModel-Simple202.xml", "OpenModel-Simple.xml")]
		[TestCase("OpenModel-Vantaa202.xml", "OpenModel-Vantaa.xml")]
		public void FromParsedXml_Downgrade_LargeFile(string fileName, string expectedFile)
		{
			string xmlContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, fileName));

			string xmlExpectedContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, expectedFile));

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			_iRDowngradeService.LoadModel(model);

			_iRDowngradeService.Downgrade("2");


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