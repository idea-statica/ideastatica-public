using IdeaRS.OpenModel.Model;
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
	public class Member2DImporterTest
	{
		[Test]
		public void Member2DImport()
		{
			// Setup
			var ctxBuilder = new ImportContextBuilder();

			(var element1, var refElement1) = ctxBuilder.Add<IIdeaElement2D>();
			(var element2, var refElement2) = ctxBuilder.Add<IIdeaElement2D>();
			(var element3, var refElement3) = ctxBuilder.Add<IIdeaElement2D>();

			var member2D = Substitute.For<IIdeaMember2D>();
			member2D.Id.Returns("member2D");
			member2D.Name.Returns("member2D");
			member2D.Elements2D.Returns(new List<IIdeaElement2D>()
			{
				element1,
				element2,
				element3
			});

			var member2DImporter = new Member2DImporter(new NullLogger());

			// Tested method
			var iomObject = member2DImporter.Import(ctxBuilder.Context, member2D);

			// Assert
			Assert.That(iomObject, Is.InstanceOf<Member2D>());
			Member2D iomMember2D = (Member2D)iomObject;

			Assert.That(iomMember2D.Name, Is.EqualTo("member2D"));
			Assert.That(iomMember2D.Elements2D, Is.EquivalentTo(new[] { refElement1, refElement2, refElement3 }));
		}

		[Test]
		public void Member2DImport_NoElements_ThrowsConstraintException()
		{
			// Setup
			var ctxBuilder = new ImportContextBuilder();

			var member2D = Substitute.For<IIdeaMember2D>();
			member2D.Id.Returns("member2D");
			member2D.Elements2D.Returns(new List<IIdeaElement2D>());

			var member2DImporter = new Member2DImporter(new NullLogger());

			// Tested method
			Assert.That(() => member2DImporter.Import(ctxBuilder.Context, member2D), Throws.TypeOf<ConstraintException>());
		}

		[Test]
		public void Member2DImport_WhenAnElementIsDuplicated_ThrowsConstraintException()
		{
			// Setup
			var ctxBuilder = new ImportContextBuilder();

			(var element1, _) = ctxBuilder.Add<IIdeaElement2D>();
			(var element2, _) = ctxBuilder.Add<IIdeaElement2D>();

			var member2D = Substitute.For<IIdeaMember2D>();
			member2D.Id.Returns("member2D");
			member2D.Elements2D.Returns(new List<IIdeaElement2D>()
			{
				element1,
				element2,
				element2
			});

			var member2DImporter = new Member2DImporter(new NullLogger());

			// Tested method
			Assert.That(() => member2DImporter.Import(ctxBuilder.Context, member2D), Throws.TypeOf<ConstraintException>());
		}
	}
}
