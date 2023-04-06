using IdeaRS.OpenModel.Model;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Tests.Helpers;
using NUnit.Framework;
using NSubstitute;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests.Importers
{
	[TestFixture]
	public class Element2DImporterTest
	{
		[Test]
		public void Element2DImport()
		{
			// Setup
			ImportContextBuilder ctxBuilder = new ImportContextBuilder();
			(var material, var refMaterial) = ctxBuilder.Add<IIdeaMaterial>();
			(var geometricRegion, var refGeometricRegion) = ctxBuilder.Add<IIdeaRegion3D>();
			(var innerLine, var refInnerLine) = ctxBuilder.Add<IIdeaPolyLine3D>();
			(var innerPoint, var refInnerPoint) = ctxBuilder.Add<IIdeaNode>();

			IIdeaElement2D element2D = Substitute.For<IIdeaElement2D>();
			element2D.Id.Returns("element2D");
			element2D.Name.Returns("element2D");
			element2D.Material.Returns(material);
			element2D.GeometricRegion.Returns(geometricRegion);
			element2D.Thickness.Returns(2.54);
			element2D.EccentricityZ.Returns(1.23);
			element2D.InnerLines.Returns(new List<IIdeaPolyLine3D>() { innerLine });
			element2D.InnerPoints.Returns(new List<IIdeaNode>() { innerPoint });
			element2D.ElementType.Returns(Element2DType.Slab);

			Element2DImporter element2DImporter = new Element2DImporter(new NullLogger());

			// Tested method
			OpenElementId iomObject = element2DImporter.Import(ctxBuilder.Context, element2D);

			// Asserts
			Assert.That(iomObject, Is.InstanceOf<Element2D>());
			Element2D iomElement = (Element2D)iomObject;

			Assert.That(iomElement.Name, Is.EqualTo("element2D"));
			Assert.That(iomElement.Material, Is.EqualTo(refMaterial));
			Assert.That(iomElement.GeometricRegion, Is.EqualTo(refGeometricRegion));
			Assert.That(iomElement.Thickness, Is.EqualTo(2.54));
			Assert.That(iomElement.EccentricityZ, Is.EqualTo(1.23));
			Assert.That(iomElement.InnerLines.Count, Is.EqualTo(1));
			CollectionAssert.Contains(iomElement.InnerLines, refInnerLine);
			Assert.That(iomElement.InnerPoints.Count, Is.EqualTo(1));
			CollectionAssert.Contains(iomElement.InnerPoints, refInnerPoint);
			Assert.That(iomElement.ElementType, Is.EqualTo(Element2DType.Slab));
		}
	}
}
