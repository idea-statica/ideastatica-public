using IdeaStatiCa.IntermediateModel;
using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.IOM.VersioningService.Downgrade;
using IdeaStatiCa.IOM.VersioningService.Upgrade;
using IdeaStatiCa.IOM.VersioningServiceTests;
using IdeaStatiCa.Plugin;
using NUnit.Framework;

namespace IdeaStatiCa.OpenModel.VersioningServiceTests
{
	/// <summary>
	/// Step 3.3.5 adds OriginalModelId to WeldData, CutData, CutBeamByBeamData, BoltGrid,
	/// AnchorGrid and PinGrid. An undefined OriginalModelId is a legitimate value, so the up step
	/// invents nothing; the down step strips the property from those six kinds and leaves
	/// ConcreteBlockData - which carried OriginalModelId before 3.3.5 - untouched.
	/// </summary>
	[TestFixture]
	public class Step335Tests
	{
		private readonly string TestData = "TestData";

		private IXmlParsingIRService _xmlParsingIRService;
		private IIRExportToXMLService _iRExportToXMLService;
		private IUpgradeService _upgradeService;
		private IDowngradeService _downgradeService;
		private IPluginLogger _logger;
		private IConfigurationStepService _configurationStepService;

		[SetUp]
		public void Setup()
		{
			_logger = new NullLogger();
			_xmlParsingIRService = new XmlParsingService(_logger);
			_iRExportToXMLService = new IRExportToXMLService(_logger);
			_configurationStepService = new ConfigurationStepService(_logger);
			_upgradeService = new UpgradeService(_logger, _configurationStepService);
			_downgradeService = new DowngradeService(_logger, _configurationStepService);
		}

		[TestCase("OriginalModelId-Kinds_3_3_4.xml", "OriginalModelId-Kinds_3_3_4-Upgraded.xml")]
		public void Upgrade_StampsVersion_AndInventsNoIdentity(string fileName, string expectedFile)
		{
			var model = _xmlParsingIRService.ParseXml(ReadTestData(fileName));

			_upgradeService.LoadModel(model);
			_upgradeService.Upgrade();

			UtHelper.AssertEqualXml(ReadTestData(expectedFile), _iRExportToXMLService.ExportToXml(model), expectedFile);
		}

		[TestCase("OriginalModelId-Kinds_3_3_5.xml", "OriginalModelId-Kinds_3_3_4.xml")]
		public void Downgrade_RemovesOriginalModelId_FromTheKindsAddedIn335(string fileName, string expectedFile)
		{
			var model = _xmlParsingIRService.ParseXml(ReadTestData(fileName));

			_downgradeService.LoadModel(model);
			_downgradeService.Downgrade(new Version(3, 3, 4));

			UtHelper.AssertEqualXml(ReadTestData(expectedFile), _iRExportToXMLService.ExportToXml(model), expectedFile);
		}

		private string ReadTestData(string fileName)
			=> File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, fileName));
	}
}
