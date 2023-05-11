using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Tests.Helpers;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests.Importers
{
	[TestFixture]
	internal class PointLoadOnLineImporterTest
	{
		[Test]
		public void PointLoadOnLineImport()
		{
			// Setup
			var ctxBuilder = new ImportContextBuilder();
			(var geometry, var refGeometry) = ctxBuilder.Add<IIdeaSegment3D>();

			var pointLoadOnLine = Substitute.For<IIdeaPointLoadOnLine>();
			pointLoadOnLine.Id.Returns("pointLoadOnLine");
			pointLoadOnLine.Direction.Returns(LoadDirection.InGcs);
			pointLoadOnLine.Fx.Returns(1.234);
			pointLoadOnLine.Fy.Returns(2.345);
			pointLoadOnLine.Fz.Returns(3.456);
			pointLoadOnLine.Mx.Returns(4.567);
			pointLoadOnLine.My.Returns(5.678);
			pointLoadOnLine.Mz.Returns(6.789);
			pointLoadOnLine.Ey.Returns(7.890);
			pointLoadOnLine.Ez.Returns(8.901);
			pointLoadOnLine.Geometry.Returns(geometry);
			pointLoadOnLine.RelativePosition.Returns(12.987);

			var pointLoadOnLineImporter = new PointLoadOnLineImporter(new NullLogger());

			// Tested method
			var iomObject = pointLoadOnLineImporter.Import(ctxBuilder.Context, pointLoadOnLine);

			// Asserts
			Assert.That(iomObject, Is.InstanceOf<PointLoadOnLine>());
			var iomPointLoadOnLine = (PointLoadOnLine)iomObject;

			Assert.That(iomPointLoadOnLine.Direction, Is.EqualTo(LoadDirection.InGcs));
			Assert.That(iomPointLoadOnLine.Fx, Is.EqualTo(1.234));
			Assert.That(iomPointLoadOnLine.Fy, Is.EqualTo(2.345));
			Assert.That(iomPointLoadOnLine.Fz, Is.EqualTo(3.456));
			Assert.That(iomPointLoadOnLine.Mx, Is.EqualTo(4.567));
			Assert.That(iomPointLoadOnLine.My, Is.EqualTo(5.678));
			Assert.That(iomPointLoadOnLine.Mz, Is.EqualTo(6.789));
			Assert.That(iomPointLoadOnLine.Ey, Is.EqualTo(7.890));
			Assert.That(iomPointLoadOnLine.Ez, Is.EqualTo(8.901));
			Assert.That(iomPointLoadOnLine.Geometry, Is.EqualTo(refGeometry));
			Assert.That(iomPointLoadOnLine.RelativePosition, Is.EqualTo(12.987));
		}
	}
}
