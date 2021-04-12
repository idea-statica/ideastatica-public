using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class ImportContextTest
	{
		[Test]
		public void asd()
		{
			IProject project = Substitute.For<IProject>();

			Member1D iomObject = new Member1D();
			IIdeaObject bimObject = Substitute.For<IIdeaObject>();
			bimObject.Id.Returns("testobject");

			IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
			importer.Import(Arg.Any<ImportContext>(), bimObject).Returns(iomObject);

			ImportContext ctx = new ImportContext(importer, project);

			ReferenceElement refElm = ctx.Import(bimObject);
			
			Assert.That(ctx.OpenModel.Member1D.Contains(iomObject), Is.True);
			project.Received().GetIomId(bimObject);
		}

		[Test]
		public void asd1()
		{
			IProject project = Substitute.For<IProject>();

			Member1D iomObject = new Member1D();
			IIdeaObject bimObject = Substitute.For<IIdeaObject>();
			bimObject.Id.Returns("testobject");

			IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
			importer.Import(Arg.Any<ImportContext>(), bimObject).Returns(iomObject);

			ImportContext ctx = new ImportContext(importer, project);

			ReferenceElement refElm1 = ctx.Import(bimObject);
			ReferenceElement refElm2 = ctx.Import(bimObject);

			Assert.That(refElm1, Is.EqualTo(refElm2));
			project.Received(1).GetIomId(bimObject);
		}
	}
}