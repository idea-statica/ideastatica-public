using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests.Importers
{
	[TestFixture]
	internal class CrossSectionImporterTest
	{
		private CrossSectionImporter importer;
		private IImportContext ctx;

		[SetUp]
		public void SetUp()
		{
			importer = new CrossSectionImporter(new NullLogger());
			ctx = Substitute.For<IImportContext>();
		}

		[Test]
		public void Import_IfImportingIIdeaCrossSectionByParametersAndCssTypeIsOneComponentCss_ThrowsConstraintException()
		{
			// Setup: create a IIdeaCrossSectionByParameters instance with type OneComponentCss
			IIdeaCrossSectionByParameters css = Substitute.For<IIdeaCrossSectionByParameters>();
			css.Id.Returns("css");
			css.Type.Returns(CrossSectionType.OneComponentCss);

			// Tested method
			Assert.That(() => importer.Import(ctx, css), Throws.InstanceOf<ConstraintException>());
		}

		[Test]
		public void Import_IfCssIsNotByParametersOrByCenterLineOrByComponents_ThrowsConstraintException()
		{
			// Setup
			IIdeaCrossSection css = Substitute.For<IIdeaCrossSection>();
			css.Id.Returns("css");

			// Tested method
			Assert.That(() => importer.Import(ctx, css), Throws.InstanceOf<ConstraintException>());
		}
	}
}