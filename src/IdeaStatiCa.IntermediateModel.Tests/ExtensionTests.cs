using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.Plugin;
using System.Xml.Linq;

namespace IdeaStatiCa.IntermediateModel.Tests
{
	[TestFixture]
	public class ExtensionTests
	{
		private IXmlParsingIRService _xmlParsingIRService;
		private IIRExportToXMLService _iRExportToXMLService;
		private IPluginLogger _logger;

		private readonly string xmlContent = "<?xml version=\"1.0\" encoding=\"utf-16\"?><root><leaf>A</leaf><persons><person><name>John Doe</name><age>30</age></person><person><name>Lee Harper</name><age>51</age></person></persons></root>";

		[SetUp]
		public void SetUp()
		{
			_logger = new NullLogger();
			_xmlParsingIRService = new XmlParsingService(_logger);

			_iRExportToXMLService = new IRExportToXMLService(_logger);
		}

		[Test]
		public void Extension_GetElements()
		{
			var model = _xmlParsingIRService.ParseXml(xmlContent);

			Assert.IsTrue(model.GetElements("root").Count() == 1);

			Assert.IsTrue(model.GetElements("Root").Count() == 1);

			Assert.IsTrue(model.GetElements("root;person").Count() == 2);
		}

		[Test]
		public void Extension_GetElementsValue()
		{
			var model = _xmlParsingIRService.ParseXml(xmlContent);

			var leaf = model.GetElements("root;leaf");
			Assert.IsTrue(leaf.Count() == 1);
			Assert.IsTrue(leaf.First().GetElementValue(string.Empty) == "A");

			foreach (var person in model.GetElements("root;persons;person"))
			{
				Assert.IsNotEmpty(person.GetElementValue("age"));
			}
		}

		[Test]
		public void Extension_TakeElement()
		{
			string xmlContentResult = "<root><persons><person><name>John Doe</name><age>30</age></person><person><name>Lee Harper</name><age>51</age></person></persons></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			var root = model.RootItem;
			var leaf = root.TakeElementProperty("leaf");

			Assert.IsNotNull(leaf);

			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(exportedXML);
			AssertEqualXml(xmlContentResult, exportedXML);
		}

		[Test]
		public void Extension_ChangeElementValue()
		{
			string xmlContentResult = "<root><leaf>B</leaf><persons><person><name>John Doe</name><age>30</age></person><person><name>Lee Harper</name><age>51</age></person></persons></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			var root = model.RootItem;


			var leaf = root.GetElements("leaf");

			leaf.First().ChangeElementValue("B");

			Assert.IsNotNull(leaf);

			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(exportedXML);
			AssertEqualXml(xmlContentResult, exportedXML);
		}

		[Test]
		public void Extension_ChangeElementName()
		{
			string xmlContentResult = "<?xml version=\"1.0\" encoding=\"utf-16\"?><root><branch>A</branch><persons><person><name>John Doe</name><age>30</age></person><person><name>Lee Harper</name><age>51</age></person></persons></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			var root = model.RootItem;

			root.ChangeElementPropertyName("leaf", "branch");


			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(exportedXML);
			AssertEqualXml(xmlContentResult, exportedXML);
		}

		[Test]
		public void Extension_CreateElementProperty()
		{
			string xmlContentResult = "<?xml version=\"1.0\" encoding=\"utf-16\"?><root><leaf>A</leaf><persons><person><name>John Doe</name><age>30</age></person><person><name>Lee Harper</name><age>51</age></person></persons><branch>CC</branch></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			var root = model.RootItem;


			var branch = root.CreateElementProperty("branch");

			branch.ChangeElementValue("CC");

			Assert.IsNotNull(branch);

			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(exportedXML);
			AssertEqualXml(xmlContentResult, exportedXML);
		}

		[Test]
		public void Extension_CreateElementPropertyDuplicityShowThrowException()
		{


			var model = _xmlParsingIRService.ParseXml(xmlContent);

			var root = model.RootItem;


			var branch = root.CreateElementProperty("branch");

			branch.ChangeElementValue("CC");

			Assert.IsNotNull(branch);

			Assert.That(() => root.CreateElementProperty("branch"), Throws.Exception);
		}

		private void AssertEqualXml(string expectedXml, string actualXml)
		{
			Assert.IsTrue(XNode.DeepEquals(XElement.Parse(expectedXml), XElement.Parse(actualXml)),
				String.Format("{0} \n does not equal \n{1}", actualXml, expectedXml));
		}
	}
}
