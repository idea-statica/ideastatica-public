using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class MemberImporterTest
	{
		[Test]
		public void MemberImport_WhenElementsIsEmpty_ThrowsConstraintException()
		{
			// Setup
			ImportContext ctx = new ImportContext();

			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.Elements.Returns(new List<IIdeaElement1D>());

			MemberImporter memberImporter = new MemberImporter(null);

			// Tested method
			Assert.That(() => memberImporter.Import(ctx, member), Throws.TypeOf<ConstraintException>());
		}

		[Test]
		public void MemberImport_WhenStartNodeAndEndNodeAreEqual_ThrowsConstraintException()
		{
			// Setup
			ImportContext ctx = new ImportContext();

			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.Elements.Returns(new List<IIdeaElement1D>() {
				Substitute.For<IIdeaElement1D>()
			});

			MemberImporter memberImporter = new MemberImporter(null);

			// Tested method
			Assert.That(() => memberImporter.Import(ctx, member), Throws.TypeOf<ConstraintException>());
		}

		[Test]
		public void MemberImport_asdasd()
		{
			// Setup
			ImportContext ctx = new ImportContext();

			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.Elements.Returns(new List<IIdeaElement1D>() {
				Substitute.For<IIdeaElement1D>()
			});

			MemberImporter memberImporter = new MemberImporter(null);

			// Tested method
			Assert.That(() => memberImporter.Import(ctx, member), Throws.TypeOf<ConstraintException>());
		}
	}
}