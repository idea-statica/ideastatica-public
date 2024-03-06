using IdeaStatiCa.IntermediateModel;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.IOM.VersioningService.Downgrade;
using IdeaStatiCa.Plugin;
using NUnit.Framework;
using System.Xml.Linq;

namespace IdeaStatiCa.IOM.VersioningServiceTests
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
		[TestCase("2.0.2", 1)]
		public void FromParsedXml_CheckPossible_Downgrade_Steps(string version, int numOfSteps)
		{
			string xmlContent = $"<OpenModel xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Version>{version}</Version></OpenModel>";

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			_downgradeService.LoadModel(model);


			Assert.IsTrue(_downgradeService.GetVersionsToDowngrade().Count() == numOfSteps);

		}

		[TestCase("OpenModel-SimpleUpgraded.xml", "OpenModel-Simple.xml")]
		[TestCase("OpenModel-LargeUpgraded.xml", "OpenModel-Large.xml")]
		[TestCase("ModelBIM-AS_Black_PointUpgraded.xml", "ModelBIM-AS_Black_Point.xml")]
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

			AssertEqualXml(xmlExpectedContent, exportedXML);

		}

		private void AssertEqualXml(string expectedXml, string actualXml)
		{
			Assert.IsTrue(XNode.DeepEquals(XElement.Parse(expectedXml), XElement.Parse(actualXml)),
				String.Format("{0} \n does not equal \n{1}", actualXml, expectedXml));
		}
	}
}
