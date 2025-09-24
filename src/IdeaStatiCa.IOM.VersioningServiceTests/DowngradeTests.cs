using IdeaStatiCa.IntermediateModel;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.IOM.VersioningService.Downgrade;
using IdeaStatiCa.IOM.VersioningServiceTests;
using IdeaStatiCa.Plugin;
using NUnit.Framework;

namespace IdeaStatiCa.OpenModel.VersioningServiceTests
{
	[TestFixture]
	public class DowngradeTests
	{
		private IXmlParsingIRService _xmlParsingIRService;
		private IIRExportToXMLService _iRExportToXMLService;

		private IDowngradeService _downgradeService;
		private IPluginLogger _logger;
		IConfigurationStepService _configurationStepService;

		private readonly string TestData = "TestData";

		[SetUp]
		public void SetDown()
		{
			_logger = new NullLogger();
			_xmlParsingIRService = new XmlParsingService(_logger);

			_iRExportToXMLService = new IRExportToXMLService(_logger);

			_configurationStepService = new ConfigurationStepService(_logger);

			_downgradeService = new DowngradeService(_logger, _configurationStepService);
		}


		[TestCase("2", 0)]
		[TestCase("2.0.1", 1)]
		[TestCase("2.0.2", 2)]
		public void FromParsedXml_CheckPossible_Downgrade_Steps(string version, int numOfSteps)
		{
			string xmlContent = $"<OpenModel xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Version>{version}</Version></OpenModel>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			_downgradeService.LoadModel(model);


			Assert.That(_downgradeService.GetVersionsToDowngrade().Count(), Is.EqualTo(numOfSteps));

		}

		[TestCase("OpenModel-Simple.xml", "OpenModel-Simple.xml")]
		[TestCase("OpenModel-LargeUpgraded.xml", "OpenModel-LargeDowngraded.xml")]
		[TestCase("ModelBIM-AS_Black_PointUpgraded.xml", "ModelBIM-AS_Black_PointDowngraded.xml")]
		[TestCase("ConnectionPoint-1_2.1.0.xml", "ConnectionPoint-1_2.0.0-Downgraded.xml")]
		[TestCase("LoadOnSurface_3_0_0.xml", "LoadOnSurface_downgraded_2_0_0.xml")]
		public void FromParsedXml_Downgrade_LargeFile(string fileName, string expectedFile)
		{
			string xmlContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, fileName));

			string xmlExpectedContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, expectedFile));

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			_downgradeService.LoadModel(model);

			_downgradeService.Downgrade(new Version(2, 0, 0));


			var exportedXML = _iRExportToXMLService.ExportToXml(model);

			Assert.IsNotNull(anObject: exportedXML);

			UtHelper.AssertEqualXml(xmlExpectedContent, exportedXML, expectedFile);
		}

	}
}
