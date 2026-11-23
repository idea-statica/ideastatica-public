using System.Xml.Linq;
using FluentAssertions;
using IdeaStatiCa.IntermediateModel;
using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.IOM.VersioningService.Exceptions;
using IdeaStatiCa.IOM.VersioningService.Upgrade;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.OpenModel.VersioningServiceTests
{
	/// <summary>
	/// Before 3.3.1 a plate names its steel grade, and before 3.3.2 a weld names its material; the up steps turn the name
	/// into a reference. A name the model does not list cannot become one, so the upgrade fails rather than drop the
	/// material - except for a negative volume, which only cuts.
	/// </summary>
	[TestFixture]
	public class UnresolvedMaterialNameTests
	{
		private const string ListedSteel = "S 355";
		private const string UnlistedSteel = "S 460";
		private const string OtherUnlistedSteel = "S 690";
		private const string ListedWelding = "E 42";
		private const string UnlistedWelding = "E 99";

		private static readonly XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";

		private IXmlParsingIRService _xmlParsingIRService = null!;
		private IIRExportToXMLService _iRExportToXMLService = null!;
		private IUpgradeService _upgradeService = null!;
		private IPluginLogger _logger = null!;

		[SetUp]
		public void Setup()
		{
			_logger = Substitute.For<IPluginLogger>();
			_xmlParsingIRService = new XmlParsingService(_logger);
			_iRExportToXMLService = new IRExportToXMLService(_logger);
			_upgradeService = new UpgradeService(_logger, new ConfigurationStepService(_logger));
		}

		[Test]
		public void Upgrade_PlateNamingAGradeTheModelDoesNotList_FailsAt331NamingThePlateAndTheGrade()
		{
			string xml = LegacyModelWithPlates(Plate(1, "Base Plate", UnlistedSteel));

			UpgradeStepException failure = Assert.Throws<UpgradeStepException>(() => Upgrade(xml))!;

			failure.Message.Should().Contain("3.3.1");
			UnresolvedMaterialNameException unresolved = failure.InnerException.Should().BeOfType<UnresolvedMaterialNameException>().Subject;
			unresolved.Items.Should().ContainSingle().Which.Should().BeEquivalentTo(new UnresolvedMaterialName(ConnectionElementKind.Plate, "1", "Base Plate", "Part-1", UnlistedSteel));
			unresolved.Message.Should().Contain("Base Plate").And.Contain("Part-1").And.Contain(UnlistedSteel);
		}

		[Test]
		public void Upgrade_SeveralPlatesNamingGradesTheModelDoesNotList_FailsOnceListingEach()
		{
			string xml = LegacyModelWithPlates(
				Plate(1, "Base Plate", UnlistedSteel),
				Plate(2, "Stiffener", ListedSteel),
				Plate(3, "End Plate", OtherUnlistedSteel));

			UpgradeStepException failure = Assert.Throws<UpgradeStepException>(() => Upgrade(xml))!;

			failure.InnerException.Should().BeOfType<UnresolvedMaterialNameException>()
				.Which.Items.Should().BeEquivalentTo(new[]
				{
					new UnresolvedMaterialName(ConnectionElementKind.Plate, "1", "Base Plate", "Part-1", UnlistedSteel),
					new UnresolvedMaterialName(ConnectionElementKind.Plate, "3", "End Plate", "Part-3", OtherUnlistedSteel),
				}, options => options.WithStrictOrdering());
		}

		[Test]
		public void Upgrade_NegativeVolumeNamingAGradeTheModelDoesNotList_UpgradesWithoutTheMaterialAndWarns()
		{
			string xml = LegacyModelWithPlates(Plate(1, "Opening", UnlistedSteel, isNegativeObject: true));

			XDocument upgraded = Upgrade(xml);

			upgraded.Descendants("PlateData").Single().Element("Material").Should().BeNull();
			_logger.Received().LogWarning(Arg.Is<string>(message => message.Contains("Opening") && message.Contains(UnlistedSteel)), Arg.Any<Exception>());
		}

		[Test]
		public void Upgrade_PlateNamingNoGrade_LeavesItWithoutMaterial()
		{
			string xml = LegacyModelWithPlates(Plate(1, "Base Plate", string.Empty));

			XDocument upgraded = Upgrade(xml);

			upgraded.Descendants("PlateData").Single().Element("Material").Should().BeNull();
		}

		[Test]
		public void Upgrade_PlateNamingAListedGrade_RefersToThatSteel()
		{
			string xml = LegacyModelWithPlates(Plate(1, "Base Plate", ListedSteel));

			XDocument upgraded = Upgrade(xml);

			XElement material = upgraded.Descendants("PlateData").Single().Element("Material")!;
			material.Element("TypeName")!.Value.Should().Be("MatSteel");
			material.Element("Id")!.Value.Should().Be("1");
		}

		[Test]
		public void Upgrade_WeldNamingAMaterialTheModelDoesNotList_FailsAt332NamingTheWeldAndTheMaterial()
		{
			string xml = LegacyModelWithWelds(Weld(7, "Fillet", UnlistedWelding));

			UpgradeStepException failure = Assert.Throws<UpgradeStepException>(() => Upgrade(xml))!;

			failure.Message.Should().Contain("3.3.2");
			failure.InnerException.Should().BeOfType<UnresolvedMaterialNameException>()
				.Which.Items.Should().ContainSingle().Which.Should().BeEquivalentTo(new UnresolvedMaterialName(ConnectionElementKind.Weld, "7", "Fillet", null, UnlistedWelding));
		}

		// The WeldMaterial reference is the weld's material; the name beside it is then only a label.
		[Test]
		public void Upgrade_WeldWithAWeldMaterialReference_KeepsThatMaterialWhateverItsName()
		{
			string xml = LegacyModelWithWelds(Weld(7, "Fillet", UnlistedWelding, weldMaterialId: "1"));

			XDocument upgraded = Upgrade(xml);

			XElement material = upgraded.Descendants("WeldData").Single().Element("Material")!;
			material.Element("TypeName")!.Value.Should().Be("MatWelding");
			material.Element("Id")!.Value.Should().Be("1");
		}

		[Test]
		public void Upgrade_WeldNamingNoMaterial_KeepsTheAutomaticMaterial()
		{
			string xml = LegacyModelWithWelds(Weld(7, "Fillet", string.Empty));

			XDocument upgraded = Upgrade(xml);

			upgraded.Descendants("WeldData").Single().Element("Material").Should().BeNull();
		}

		private XDocument Upgrade(string xml)
		{
			var model = _xmlParsingIRService.ParseXml(xml);
			_upgradeService.LoadModel(model);
			_upgradeService.Upgrade();
			return XDocument.Parse(_iRExportToXMLService.ExportToXml(model));
		}

		private static string LegacyModelWithPlates(params XElement[] plates) => LegacyModel(plates, Array.Empty<XElement>());

		private static string LegacyModelWithWelds(params XElement[] welds) => LegacyModel(Array.Empty<XElement>(), welds);

		// IOM 3.3.0: plates and welds still name their materials.
		private static string LegacyModel(XElement[] plates, XElement[] welds)
		{
			return new XElement("OpenModel",
				new XAttribute(XNamespace.Xmlns + "xsi", Xsi),
				new XElement("Version", "3.3.0"),
				new XElement("MatSteel",
					new XElement("MatSteel", new XAttribute(Xsi + "type", "MatSteelEc2"), new XElement("Id", 1), new XElement("Name", ListedSteel))),
				new XElement("MatWelding",
					new XElement("MatWelding", new XAttribute(Xsi + "type", "MatWeldingEc2"), new XElement("Id", 1), new XElement("Name", ListedWelding))),
				new XElement("Connections",
					new XElement("ConnectionData",
						new XElement("Plates", plates),
						new XElement("Welds", welds)))).ToString();
		}

		private static XElement Plate(int id, string name, string material, bool isNegativeObject = false)
		{
			return new XElement("PlateData",
				new XElement("Id", id),
				new XElement("Name", name),
				new XElement("OriginalModelId", $"Part-{id}"),
				new XElement("Material", material),
				new XElement("IsNegativeObject", isNegativeObject ? "true" : "false"));
		}

		private static XElement Weld(int id, string name, string material, string? weldMaterialId = null)
		{
			return new XElement("WeldData",
				new XElement("Id", id),
				new XElement("Name", name),
				new XElement("Material", material),
				weldMaterialId == null
					? null
					: new XElement("WeldMaterial", new XElement("TypeName", "MatWelding"), new XElement("Id", weldMaterialId)));
		}
	}
}
