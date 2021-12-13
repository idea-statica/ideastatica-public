using IdeaRS.OpenModel;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Tests.Helpers;
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

		[Test]
		public void Import_CrossSectionByName()
		{
			// Setup
			IIdeaCrossSection css = Substitute.For<IIdeaCrossSectionByName>();
			css.Id.Returns("css");
			css.Name.Returns("IPE220");

			// Tested method
			OpenElementId iomElem = importer.Import(ctx, css);

			// Assert
			Assert.That(iomElem, Is.TypeOf<CrossSectionParameter>());

			CrossSectionParameter iomCss = (CrossSectionParameter)iomElem;
			Assert.That(iomCss.CrossSectionType, Is.EqualTo(CrossSectionType.UniqueName));

			ParameterString expectedParam = new ParameterString()
			{
				Name = "UniqueName",
				Value = "IPE220"
			};
			Assert.That(iomCss.Parameters, Contains.Item(expectedParam).Using(new ParameterEqualityComparer()));
		}

		[Test]
		public void Import_IfCssByNameAndNameIsNull_ThrowsConstraintException()
		{
			// Setup
			IIdeaCrossSection css = Substitute.For<IIdeaCrossSectionByName>();
			css.Id.Returns("css");
			css.Name.Returns((string)null);

			// Tested method
			Assert.That(() => importer.Import(ctx, css), Throws.InstanceOf<ConstraintException>());
		}
	}
}