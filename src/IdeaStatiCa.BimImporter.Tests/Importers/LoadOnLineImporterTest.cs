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
	public class LoadOnLineImporterTest
	{
		[Test]
		public void LoadOnLineImport()
		{
			// Setup
			var ctxBuilder = new ImportContextBuilder();
			(var geometry, var refGeometry) = ctxBuilder.Add<IIdeaSegment3D>();

			var loadOnLine = Substitute.For<IIdeaLoadOnLine>();
			loadOnLine.Id.Returns("loadOnLine");
			loadOnLine.RelativeBeginPosition.Returns(0.01);
			loadOnLine.RelativeEndPosition.Returns(10.99);
			loadOnLine.ExY.Returns(1.23);
			loadOnLine.ExZ.Returns(2.34);
			loadOnLine.ExYEnd.Returns(3.45);
			loadOnLine.ExZEnd.Returns(4.56);
			loadOnLine.Type.Returns(LoadType.LoadForce);
			loadOnLine.Direction.Returns(LoadDirection.InLcs);
			loadOnLine.Bimp.Returns(new LoadImpulse() { X = 1.2, Y = 2.3, Z = 3.4 });
			loadOnLine.Eimp.Returns(new LoadImpulse() { X = 4.4, Y = 5.5, Z = 6.6 });
			loadOnLine.Geometry.Returns(geometry);
			loadOnLine.LoadProjection.Returns(LoadProjection.Projection);

			var loadOnLineImporter = new LoadOnLineImporter(new NullLogger());

			// Tested method
			var iomObject = loadOnLineImporter.Import(ctxBuilder.Context, loadOnLine);

			// Asserts
			Assert.That(iomObject, Is.InstanceOf<LoadOnLine>());
			var iomLoadOnLine = (LoadOnLine)iomObject;

			Assert.That(iomLoadOnLine.RelativeBeginPosition, Is.EqualTo(0.01));
			Assert.That(iomLoadOnLine.RelativeEndPosition, Is.EqualTo(10.99));
			Assert.That(iomLoadOnLine.ExY, Is.EqualTo(1.23));
			Assert.That(iomLoadOnLine.ExZ, Is.EqualTo(2.34));
			Assert.That(iomLoadOnLine.ExYEnd, Is.EqualTo(3.45));
			Assert.That(iomLoadOnLine.ExZEnd, Is.EqualTo(4.56));
			Assert.That(iomLoadOnLine.Type, Is.EqualTo(LoadType.LoadForce));
			Assert.That(iomLoadOnLine.Direction, Is.EqualTo(LoadDirection.InLcs));
			Assert.That(iomLoadOnLine.Bimp.X, Is.EqualTo(1.2));
			Assert.That(iomLoadOnLine.Bimp.Y, Is.EqualTo(2.3));
			Assert.That(iomLoadOnLine.Bimp.Z, Is.EqualTo(3.4));
			Assert.That(iomLoadOnLine.Eimp.X, Is.EqualTo(4.4));
			Assert.That(iomLoadOnLine.Eimp.Y, Is.EqualTo(5.5));
			Assert.That(iomLoadOnLine.Eimp.Z, Is.EqualTo(6.6));
			Assert.That(iomLoadOnLine.Geometry, Is.EqualTo(refGeometry));
			Assert.That(iomLoadOnLine.LoadProjection, Is.EqualTo(LoadProjection.Projection));
		}
	}
}
