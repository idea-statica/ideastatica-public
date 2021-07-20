using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class ImportContextTest
	{
		private IProject project;

		[SetUp]
		public void SetUp()
		{
			project = Substitute.For<IProject>();
		}

		[Test]
		public void Import_ShouldAddsObjectToOpenModel()
		{
			// Setup
			Member1D iomObject = new Member1D();
			IIdeaObject bimObject = Substitute.For<IIdeaObject>();
			bimObject.Id.Returns("testobject");

			IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
			importer.Import(Arg.Any<ImportContext>(), bimObject).Returns(iomObject);

			ImportContext ctx = new ImportContext(importer, null, project, new NullLogger());

			// Tested method
			ReferenceElement refElm = ctx.Import(bimObject);

			// Assert: should add the imported object into IOM
			Assert.That(ctx.OpenModel.Member1D.Contains(iomObject), Is.True);
			importer.Received().Import(ctx, bimObject);
			project.Received().GetIomId(bimObject);
		}

		[Test]
		public void Import_IfCalledTwiceWithTheSameObject_ReusesAlreadyImportedObject()
		{
			// Setup
			Member1D iomObject = new Member1D();
			IIdeaObject bimObject = Substitute.For<IIdeaObject>();
			bimObject.Id.Returns("testobject");

			IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
			importer.Import(Arg.Any<ImportContext>(), bimObject).Returns(iomObject);

			ImportContext ctx = new ImportContext(importer, null, project, new NullLogger());

			// Tested method
			ReferenceElement refElm1 = ctx.Import(bimObject);
			ReferenceElement refElm2 = ctx.Import(bimObject);

			// Assert: IImporter.Import should receive only 1 call, refElm1 should equal to refElm2
			Assert.That(refElm1, Is.EqualTo(refElm2));
			importer.ReceivedWithAnyArgs(1).Import(default, default);
			project.Received(1).GetIomId(bimObject);
		}

		[Test]
		public void Import_IfObjectIsInstanceOfIIdeaObjectWithResults_CallsResultImporter()
		{
			// Setup
			Member1D iomObject = new Member1D();
			IIdeaObjectWithResults objectWithResults = Substitute.For<IIdeaObjectWithResults>();
			objectWithResults.Id.Returns("testobject");

			IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
			importer.Import(Arg.Any<ImportContext>(), objectWithResults).Returns(iomObject);

			IResultImporter resultImporter = Substitute.For<IResultImporter>();

			ImportContext ctx = new ImportContext(importer, resultImporter, project, new NullLogger());

			// Tested method
			ReferenceElement refElm = ctx.Import(objectWithResults);

			// Assert: should call IResultImporter.Import
			resultImporter.Received().Import(ctx, refElm, objectWithResults);
		}

		[Test]
		public void ImportBimItem_ShouldAddBimItemAndImportReferencedObject()
		{
			// Setup
			Member1D iomObject = new Member1D();
			IIdeaObject bimObject = Substitute.For<IIdeaObject>();
			bimObject.Id.Returns("testobject");

			IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
			importer.Import(Arg.Any<ImportContext>(), bimObject).Returns(iomObject);

			IBimItem bimItem = Substitute.For<IBimItem>();
			bimItem.Type.Returns(BIMItemType.Member);
			bimItem.ReferencedObject.Returns(bimObject);

			project.GetIomId(bimObject).Returns(1);

			ImportContext ctx = new ImportContext(importer, null, project, new NullLogger());

			// Tested method
			ctx.ImportBimItem(bimItem);

			// Assert: should add bimitem into ctx.BimItems and import referenced object
			Assert.That(ctx.BimItems.Count, Is.EqualTo(1));
			Assert.That(ctx.BimItems[0].Type, Is.EqualTo(BIMItemType.Member));
			Assert.That(ctx.BimItems[0].Id, Is.EqualTo(1));

			Assert.That(ctx.OpenModel.Member1D.Contains(iomObject), Is.True);
			importer.Received().Import(ctx, bimObject);
		}
	}
}