using IdeaStatiCa.IntermediateModel;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.IOM.VersioningService.Upgrade;
using IdeaStatiCa.Plugin;
using NUnit.Framework;
using System.Xml.Linq;

namespace IdeaStatiCa.OpenModel.VersioningServiceTests
{
	[TestFixture]
	public class UpgradeTests
	{
		private IXmlParsingIRService _xmlParsingIRService;
		private IIRExportToXMLService _iRExportToXMLService;

		private IUpgradeService _upgradeService;
		private IPluginLogger _logger;
		IConfigurationStepService _configurationStepService;

		private readonly string TestData = "TestData";

		[SetUp]
		public void Setup()
		{
			_logger = new NullLogger();
			_xmlParsingIRService = new XmlParsingService(_logger);

			_iRExportToXMLService = new IRExportToXMLService(_logger);

			_configurationStepService = new ConfigurationStepService(_logger);

			_upgradeService = new UpgradeService(_logger, _configurationStepService);
		}

		[TestCase("OpenModel-Simple.xml", "OpenModel-SimpleUpgraded.xml")]
		[TestCase("OpenModel-Large.xml", "OpenModel-LargeUpgraded.xml")]
		[TestCase("ModelBIM-AS_Black_Point.xml", "ModelBIM-AS_Black_PointUpgraded.xml")]
		[TestCase("CHK.xml", "CHK.xml")]
		public void FromParsedXml_Upgrade_LargeFile(string fileName, string expectedFile)
		{
			string xmlContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, fileName));

			string xmlExpectedContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, expectedFile));

			var model = _xmlParsingIRService.ParseXml(xmlContent);

			// Assert
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.RootItem);
			Assert.IsInstanceOf(typeof(SObject), model.RootItem);

			_upgradeService.LoadModel(model);

			_upgradeService.Upgrade();


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
