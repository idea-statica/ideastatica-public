using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Tests.Helpers;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests.Importers
{
	[TestFixture]
	public class PolyLine3DImporterTest
	{
		[Test]
		public void PolyLine3DImport()
		{
			// Setup
			ImportContextBuilder ctxBuilder = new ImportContextBuilder();
			(var segment1, var refSegment1) = ctxBuilder.Add<IIdeaSegment3D>();
			(var segment2, var refSegment2) = ctxBuilder.Add<IIdeaSegment3D>();

			IIdeaPolyLine3D polyLine3D = Substitute.For<IIdeaPolyLine3D>();
			polyLine3D.Id.Returns("polyLine3D");
			polyLine3D.Segments.Returns(new List<IIdeaSegment3D>() { segment1, segment2 });

			PolyLine3DImporter polyLine3DImporter = new PolyLine3DImporter(new NullLogger());

			// Tested method
			OpenElementId iomObject = polyLine3DImporter.Import(ctxBuilder.Context, polyLine3D);

			// Asserts
			Assert.That(iomObject, Is.InstanceOf<PolyLine3D>());
			PolyLine3D iomElement = (PolyLine3D)iomObject;

			Assert.That(iomElement.Segments.Count, Is.EqualTo(2));
			CollectionAssert.AreEqual(new List<ReferenceElement>() { refSegment1, refSegment2 }, iomElement.Segments);
		}
	}
}
