using FluentAssertions;
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
		private IPluginLogger logger;
		private IImporter<IIdeaObject> importer;
		private IResultImporter resultImporter;

		private ImportContext ctx;

		private BimImporterConfiguration configuration;

		[SetUp]
		public void SetUp()
		{
			project = Substitute.For<IProject>();
			logger = Substitute.For<IPluginLogger>();
			importer = Substitute.For<IImporter<IIdeaObject>>();
			resultImporter = Substitute.For<IResultImporter>();

			configuration = new BimImporterConfiguration();

			ctx = new ImportContext(importer, resultImporter, project, logger, configuration);
		}

		[Test]
		public void Import_IfArgumentIsNull_ShouldReturnNull()
		{
			// Tested method
			ReferenceElement refElm = ctx.Import(null);

			// Assert: should add the imported object into IOM
			refElm.Should().BeNull();
		}

		[Test]
		public void Import_ShouldAddsObjectToOpenModel()
		{
			// Setup
			Member1D iomObject = new Member1D();
			IIdeaObject bimObject = Substitute.For<IIdeaObject>();
			bimObject.Id.Returns("testobject");

			importer.Import(Arg.Any<ImportContext>(), bimObject).Returns(iomObject);

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

			importer.Import(Arg.Any<ImportContext>(), bimObject).Returns(iomObject);

			// Tested method
			ReferenceElement refElm1 = ctx.Import(bimObject);
			ReferenceElement refElm2 = ctx.Import(bimObject);

			// Assert: IImporter.Import should receive only 1 call, refElm1 should equal to refElm2
			Assert.That(refElm1, Is.EqualTo(refElm2));
			importer.ReceivedWithAnyArgs(1).Import(default, default);
			project.Received(1).GetIomId(bimObject);
		}

		//[Test]
		//public void Import_IfObjectIsInstanceOfIIdeaObjectWithResults_CallsResultImporter()
		//{
		//	// Setup
		//	Member1D iomObject = new Member1D();
		//	IIdeaObjectWithResults objectWithResults = Substitute.For<IIdeaObjectWithResults>();
		//	objectWithResults.Id.Returns("testobject");

		//	importer.Import(Arg.Any<ImportContext>(), objectWithResults).Returns(iomObject);

		//	// Tested method
		//	ReferenceElement refElm = ctx.Import(objectWithResults);

		//	// Assert: should call IResultImporter.Import
		//	resultImporter.Received().Import(ctx, refElm, objectWithResults);
		//}

		[Test]
		public void ImportBimItem_ShouldAddBimItemAndImportReferencedObject()
		{
			// Setup
			Member1D iomObject = new Member1D();
			IIdeaObject bimObject = Substitute.For<IIdeaObject>();
			bimObject.Id.Returns("testobject");

			importer.Import(Arg.Any<ImportContext>(), bimObject).Returns(iomObject);

			IBimItem bimItem = Substitute.For<IBimItem>();
			bimItem.Type.Returns(BIMItemType.Member);
			bimItem.ReferencedObject.Returns(bimObject);

			project.GetIomId(bimObject).Returns(1);

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