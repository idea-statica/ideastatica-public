using System.Xml.Linq;
using FluentAssertions;
using IdeaStatiCa.IntermediateModel;
using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.IOM.VersioningService.Upgrade;
using IdeaStatiCa.Plugin;
using NUnit.Framework;

namespace IdeaStatiCa.OpenModel.VersioningServiceTests
{
	/// <summary>
	/// Step 3.3.4 materializes a MatConcrete for a concrete-block material name that is absent from
	/// the model's MatConcrete list, so the reference resolves to the real grade instead of being
	/// dropped. Old base-plate models (like this fixture) carry the block material only on the
	/// inline AnchorGrid.ConcreteBlock, with an empty top-level MatConcrete list.
	/// </summary>
	[TestFixture]
	public class Step334MaterializeTests
	{
		private const string XsiNs = "http://www.w3.org/2001/XMLSchema-instance";

		private IXmlParsingIRService _xmlParsingIRService;
		private IIRExportToXMLService _iRExportToXMLService;
		private IUpgradeService _upgradeService;
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
		}

		// EN model (MatSteelEc2) -> the materialized concrete is Eurocode (MatConcreteEc2).
		[TestCase("ConcreteBlock-Materialize.xml", "C20/25", "MatConcreteEc2")]
		public void Upgrade_MaterializesMissingConcreteMaterial(string fileName, string expectedGrade, string expectedXsiType)
		{
			var xml = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", fileName));

			var model = _xmlParsingIRService.ParseXml(xml);
			_upgradeService.LoadModel(model);
			_upgradeService.Upgrade();
			var upgradedXml = _iRExportToXMLService.ExportToXml(model);

			var doc = XDocument.Parse(upgradedXml);

			// The missing grade is now a real, library-loaded MatConcrete of the model's design code.
			var materialized = doc.Descendants("MatConcrete")
				.SingleOrDefault(e => (string?)e.Element("Name") == expectedGrade);
			materialized.Should().NotBeNull($"the block material '{expectedGrade}' should be materialized in MatConcrete");
			materialized!.Attribute(XName.Get("type", XsiNs))?.Value.Should().Be(expectedXsiType);
			((string?)materialized.Element("LoadFromLibrary")).Should().Be("true");

			var materializedId = (string?)materialized.Element("Id");
			materializedId.Should().NotBeNullOrEmpty();

			// Every concrete block now references that material by Id (nothing dropped).
			var blocks = doc.Descendants("ConcreteBlockData").ToList();
			blocks.Should().NotBeEmpty();
			foreach (var block in blocks)
			{
				var matRef = block.Element("Material");
				matRef.Should().NotBeNull();
				((string?)matRef!.Element("TypeName")).Should().Be("MatConcrete");
				((string?)matRef.Element("Id")).Should().Be(materializedId);
			}
		}
	}
}
