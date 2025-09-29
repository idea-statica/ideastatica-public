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
			(var segment, var refSegment) = ctxBuilder.Add<IIdeaSegment3D>();

			IIdeaElement1D element = MockHelper.CreateElement(
				"elm",
				segment,
				3);

			ElementImporter elementImport = new ElementImporter(new NullLogger());

			// Tested method
			OpenElementId iomObject = elementImport.Import(ctxBuilder.Context, element);

			// Asserts
			Assert.That(iomObject, Is.InstanceOf<Element1D>());
			Element1D iomElement = (Element1D)iomObject;

			Assert.That(iomElement.Name, Is.EqualTo("elm"));
			Assert.That(iomElement.Segment, Is.EqualTo(refSegment));
			Assert.That(iomElement.RotationRx, Is.EqualTo(3));
		}
	}
}