using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Tests
{

	[TestFixture]
	public class XmlParsingServiceTests
	{
		private IXmlParsingIRService _xmlParsingIRService;

		[SetUp]
		public void SetUp()
		{
			_xmlParsingIRService = new XmlParsingService();
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
	}

}