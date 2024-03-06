using IdeaStatiCa.IntermediateModel;
using IdeaStatiCa.IOM.VersioningService;
using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.Plugin;
using NUnit.Framework;

namespace IdeaStatiCa.OpenModel.VersioningServiceTests
{
	[TestFixture]
	public class VersioningServiceBaseTests
	{
		private IXmlParsingIRService _xmlParsingIRService;

		private IVersioningService _versioningService;
		private IPluginLogger _logger;
		IConfigurationStepService _configurationStepService;

		[SetUp]
		public void Setup()
		{
			_logger = new NullLogger();
			_xmlParsingIRService = new XmlParsingService(_logger);

			_configurationStepService = new ConfigurationStepService(_logger);

			_versioningService = new VersioningServiceBase(_logger, _configurationStepService);
		}

		[Test]
		public void VersioningService_Load_NullModel_ShouldFail()
		{
			Assert.That(() => _versioningService.LoadModel(null), Throws.Exception);
		}

		[Test]
		public void VersioningService_Load_ModelIsNotIOM_ShouldFail()
		{
			string xmlContent = "<root><person><name>John Doe</name><age>30</age></person></root>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			Assert.That(() => _versioningService.LoadModel(model), Throws.Exception);
		}

		[Test]
		public void VersioningService_Load_ModelIsNotContainsVersion_ShouldFail()
		{
			string xmlContent = "<OpenModel xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n</OpenModel>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			Assert.That(() => _versioningService.LoadModel(model), Throws.Exception);
		}

		[Test]
		public void VersioningService_Load_ModelContainsVersion_ShouldSuccess()
		{
			string xmlContent = "<OpenModel xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Version>2.0.0</Version></OpenModel>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			_versioningService.LoadModel(model);

			Assert.IsNotNull(model.Version);
		}
	}
}
