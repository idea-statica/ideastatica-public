using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Tests.Helpers;
using IdeaStatiCa.Plugin;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests.Importers
{
	[TestFixture]
	public class ElementImporterTest
	{
		[Test]
		public void ElementImport()
		{
			// Setup
			ImportContextBuilder ctxBuilder = new ImportContextBuilder();
			(var cssStart, var refCssStart) = ctxBuilder.Add<IIdeaCrossSection>();
			(var cssEnd, var refCssEnd) = ctxBuilder.Add<IIdeaCrossSection>();
			(var segment, var refSegment) = ctxBuilder.Add<IIdeaSegment3D>();

			IIdeaElement1D element = MockHelper.CreateElement(
				"elm",
				segment,
				cssStart,
				cssEnd,
				new IdeaVector3D(1, 2, 3),
				new IdeaVector3D(4, 5, 6),
				3);

			ElementImporter elementImport = new ElementImporter(new NullLogger());

			// Tested method
			OpenElementId iomObject = elementImport.Import(ctxBuilder.Context, element);

			// Asserts
			Assert.That(iomObject, Is.InstanceOf<Element1D>());
			Element1D iomElement = (Element1D)iomObject;

			Assert.That(iomElement.Name, Is.EqualTo("elm"));
			Assert.That(iomElement.CrossSectionBegin, Is.EqualTo(refCssStart));
			Assert.That(iomElement.CrossSectionEnd, Is.EqualTo(refCssEnd));
			Assert.That(iomElement.Segment, Is.EqualTo(refSegment));
			Assert.That(iomElement.RotationRx, Is.EqualTo(3));

			Assert.That(iomElement.EccentricityBeginX, Is.EqualTo(1));
			Assert.That(iomElement.EccentricityBeginY, Is.EqualTo(2));
			Assert.That(iomElement.EccentricityBeginZ, Is.EqualTo(3));

			Assert.That(iomElement.EccentricityEndX, Is.EqualTo(4));
			Assert.That(iomElement.EccentricityEndY, Is.EqualTo(5));
			Assert.That(iomElement.EccentricityEndZ, Is.EqualTo(6));
		}
	}
}