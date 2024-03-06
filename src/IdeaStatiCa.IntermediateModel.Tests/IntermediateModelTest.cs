using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.Plugin;
using System.Xml.Linq;

namespace IdeaStatiCa.IntermediateModel.Tests
{

	[TestFixture]
	public class XmlParsingServiceTests
	{
		private IXmlParsingIRService _xmlParsingIRService;
		private IIRExportToXMLService _iRExportToXMLService;
		private IPluginLogger _logger;
		private readonly string TestData = "ModelTestData";

		[SetUp]
		public void SetUp()
		{
			_logger = new NullLogger();
			_xmlParsingIRService = new XmlParsingService(_logger);

			_iRExportToXMLService = new IRExportToXMLService(_logger);
		}

		[Test]
		public void ParseXml_WhenXmlIsValid_ReturnsValidModel()
		{
			string xmlContent = "<root><person><name>John Doe</name><age>30</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
		}

		[Test]
		public void ParseXml_CheckAttributes()
		{
			string xmlContent = "<root xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><person xsi:type=\"LoadGroupEC\" ><name>John Doe</name><age>30</age></person><person><name>franta</name><age>50</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			//contains attributes
			Assert.IsTrue((model.RootItem as SObject)?.Properties.Values?.Count((p => p is SAttribute)) == 2);

			//contains Slist of people
			Assert.IsTrue((model.RootItem as SObject)?.Properties.Values?.Count((p => p is SList)) == 1);
		}


		[Test]
		public void FromParsedXml_ThanReconstructOriginal_ReturnsSameXML()
		{
			string xmlContent = "<root><person><name>John Doe</name><age>30</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);


			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(anObject: exportedXML);
			AssertEqualXml(xmlContent, exportedXML);

		}

		[Test]
		public void FromParsedXml_ThanReconstructOriginal_CheckAttributes()
		{
			string xmlContent = "<root xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><person xsi:type=\"LoadGroupEC\" ><name>John Doe</name><age>30</age></person><person><name>franta</name><age>50</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			//contains attributes
			Assert.IsTrue((model.RootItem as SObject)?.Properties.Values?.Count((p => p is SAttribute)) == 2);

			//contains Slist of people
			Assert.IsTrue((model.RootItem as SObject)?.Properties.Values?.Count((p => p is SList)) == 1);

			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(anObject: exportedXML);
			AssertEqualXml(xmlContent, exportedXML);

		}



		[TestCase("OpenModel-Simple.xml")]
		[TestCase("OpenModel-Larger.xml")]
		[TestCase("ModelBIM-AS_Black_Point.xml")]
		public void FromParsedXml_ThanReconstructOriginal_LargeFile(string fileName)
		{
			string xmlContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, fileName));

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			//contains attributes
			Assert.IsTrue((model.RootItem as SObject)?.Properties.Values?.Count((p => p is SAttribute)) == 2);

			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(exportedXML);

			AssertEqualXml(xmlContent, exportedXML);

		}

		private void AssertEqualXml(string expectedXml, string actualXml)
		{
			Assert.IsTrue(XNode.DeepEquals(XElement.Parse(expectedXml), XElement.Parse(actualXml)),
				String.Format("{0} \n does not equal \n{1}", actualXml, expectedXml));
		}
	}

}