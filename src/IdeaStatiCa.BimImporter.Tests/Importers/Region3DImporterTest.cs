using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Tests.Helpers;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Tests.Importers
{
	[TestFixture]
	public class Region3DImporterTest
	{
		[Test]
		public void Region3DImport()
		{
			// Setup
			ImportContextBuilder ctxBuilder = new ImportContextBuilder();
			(var outline, var refOutline) = ctxBuilder.Add<IIdeaPolyLine3D>();
			(var opening1, var refOpening1) = ctxBuilder.Add<IIdeaPolyLine3D>();
			(var opening2, var refOpening2) = ctxBuilder.Add<IIdeaPolyLine3D>();
			var lcs = Substitute.For<CoordSystem>();

			IIdeaRegion3D region3D = Substitute.For<IIdeaRegion3D>();
			region3D.Id.Returns("region3D");
			region3D.Outline.Returns(outline);
			region3D.Openings.Returns(new List<IIdeaPolyLine3D>() { opening1, opening2 });
			region3D.LocalCoordinateSystem.Returns(lcs);

			Region3DImporter region3DImporter = new Region3DImporter(new NullLogger());

			// Tested method
			OpenElementId iomObject = region3DImporter.Import(ctxBuilder.Context, region3D);

			// Asserts
			Assert.That(iomObject, Is.InstanceOf<Region3D>());
			Region3D iomElement = (Region3D)iomObject;

			Assert.That(iomElement.Outline, Is.EqualTo(refOutline));
			Assert.That(iomElement.Openings.Count, Is.EqualTo(2));
			CollectionAssert.AreEqual(new List<ReferenceElement>() { refOpening1, refOpening2 }, iomElement.Openings);
			Assert.That(iomElement.LocalCoordinateSystem, Is.EqualTo(lcs));
		}
	}
}
