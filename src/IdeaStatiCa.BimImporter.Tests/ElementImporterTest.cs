using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class ElementImporterTest
	{
		[Test]
		public void ElementImport()
		{
			// Setup
			ImportContext ctx = new ImportContext();

			IIdeaCrossSection cssStart = Substitute.For<IIdeaCrossSection>();
			ReferenceElement iomCssStart = MockHelper.CreateRefElement();
			IIdeaCrossSection cssEnd = Substitute.For<IIdeaCrossSection>();
			ReferenceElement iomCssEnd = MockHelper.CreateRefElement();

			IImporter<IIdeaCrossSection> cssImporter = Substitute.For<IImporter<IIdeaCrossSection>>();
			cssImporter.Import(Arg.Any<ImportContext>(), cssStart).Returns(iomCssStart);
			cssImporter.Import(Arg.Any<ImportContext>(), cssEnd).Returns(iomCssEnd);

			IIdeaSegment3D segment = Substitute.For<IIdeaSegment3D>();
			ReferenceElement iomSegment = MockHelper.CreateRefElement();

			IImporter<IIdeaSegment3D> segmentImporter = Substitute.For<IImporter<IIdeaSegment3D>>();
			segmentImporter.Import(ctx, segment).Returns(iomSegment);

			IIdeaElement1D element = MockHelper.CreateElement(
				"elm",
				MockHelper.CreateNode(0, 0, 0),
				MockHelper.CreateNode(1, 1, 1),
				cssStart,
				cssEnd,
				MockHelper.CreateVector3D(1, 2, 3),
				MockHelper.CreateVector3D(4, 5, 6),
				3,
				segment);

			ElementImporter elementImport = new ElementImporter(cssImporter, segmentImporter);

			// Tested method
			ReferenceElement refElm = elementImport.Import(ctx, element);

			// Asserts
			Assert.That(refElm.Element, Is.InstanceOf<Element1D>());
			Element1D iomElement = (Element1D)refElm.Element;

			Assert.That(iomElement.Name, Is.EqualTo("elm"));
			Assert.That(iomElement.CrossSectionBegin, Is.EqualTo(iomCssStart));
			Assert.That(iomElement.CrossSectionEnd, Is.EqualTo(iomCssEnd));
			Assert.That(iomElement.Segment, Is.EqualTo(iomSegment));
			Assert.That(iomElement.RotationRx, Is.EqualTo(3));

			Assert.That(iomElement.EccentricityBeginX, Is.EqualTo(1));
			Assert.That(iomElement.EccentricityBeginY, Is.EqualTo(2));
			Assert.That(iomElement.EccentricityBeginZ, Is.EqualTo(3));

			Assert.That(iomElement.EccentricityEndX, Is.EqualTo(4));
			Assert.That(iomElement.EccentricityEndY, Is.EqualTo(5));
			Assert.That(iomElement.EccentricityEndZ, Is.EqualTo(6));
		}

		[Test]
		public void ElementImport_ImportTwice()
		{
			// Setup
			ImportContext ctx = new ImportContext();

			IIdeaCrossSection css = Substitute.For<IIdeaCrossSection>();
			ReferenceElement iomCss = MockHelper.CreateRefElement();

			IImporter<IIdeaCrossSection> cssImporter = Substitute.For<IImporter<IIdeaCrossSection>>();
			cssImporter.Import(Arg.Any<ImportContext>(), css).Returns(iomCss);

			IIdeaSegment3D segment = Substitute.For<IIdeaSegment3D>();
			ReferenceElement iomSegment = MockHelper.CreateRefElement();

			IImporter<IIdeaSegment3D> segmentImporter = Substitute.For<IImporter<IIdeaSegment3D>>();
			segmentImporter.Import(ctx, segment).Returns(iomSegment);

			IIdeaElement1D element = MockHelper.CreateElement(
				"elm",
				MockHelper.CreateNode(0, 0, 0),
				MockHelper.CreateNode(1, 1, 1),
				css,
				css,
				MockHelper.CreateVector3D(1, 2, 3),
				MockHelper.CreateVector3D(4, 5, 6),
				3,
				segment);

			ElementImporter elementImport = new ElementImporter(cssImporter, segmentImporter);

			// Tested method
			ReferenceElement refElm1 = elementImport.Import(ctx, element);
			ReferenceElement refElm2 = elementImport.Import(ctx, element);

			// Asserts
			Assert.That(refElm1, Is.EqualTo(refElm2));
			Assert.That(ctx.OpenModel.Element1D.Count, Is.EqualTo(1));
		}

		[Test]
		public void ElementImport_StartEndNodeSame()
		{
			// Setup
			ImportContext ctx = new ImportContext();

			IIdeaCrossSection css = Substitute.For<IIdeaCrossSection>();
			ReferenceElement iomCss = MockHelper.CreateRefElement();

			IImporter<IIdeaCrossSection> cssImporter = Substitute.For<IImporter<IIdeaCrossSection>>();
			cssImporter.Import(Arg.Any<ImportContext>(), css).Returns(iomCss);

			IIdeaSegment3D segment = Substitute.For<IIdeaSegment3D>();
			ReferenceElement iomSegment = MockHelper.CreateRefElement();

			IImporter<IIdeaSegment3D> segmentImporter = Substitute.For<IImporter<IIdeaSegment3D>>();
			segmentImporter.Import(ctx, segment).Returns(iomSegment);

			IIdeaNode node = MockHelper.CreateNode(1, 1, 1);

			IIdeaElement1D element = MockHelper.CreateElement(
				"elm",
				node,
				node,
				css,
				css,
				MockHelper.CreateVector3D(1, 2, 3),
				MockHelper.CreateVector3D(4, 5, 6),
				3,
				segment);

			ElementImporter elementImport = new ElementImporter(cssImporter, segmentImporter);

			// Tested method
			Assert.That(() => elementImport.Import(ctx, element), Throws.TypeOf<ConstraintException>());
		}
	}
}