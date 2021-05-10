using IdeaStatiCa.BimApi;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class ProjectTest
	{
		[Test]
		public void GetIomIdStringId_IfMappingDoesNotExist_ThrowKeyNotFoundException()
		{
			Project project = new Project();
			Assert.That(() => project.GetIomId("non existent it"), Throws.InstanceOf<KeyNotFoundException>());
		}

		[Test]
		public void GetBimObject_IfMappingDoesNotExist_ThrowKeyNotFoundException()
		{
			Project project = new Project();
			Assert.That(() => project.GetBimObject(1), Throws.InstanceOf<KeyNotFoundException>());
		}

		[Test]
		public void GetIomIdIIdeaObject_IfMappingDoesNotExist_ThrowsNothing()
		{
			Project project = new Project();

			int id = project.GetIomId(Substitute.For<IIdeaObject>());
			Assert.That(() => project.GetIomId(Substitute.For<IIdeaObject>()), Throws.Nothing);
		}

		[Test]
		public void GetIomIdIIdeaObject_SameObject_ReturnsSameIomId()
		{
			// Setup
			Project project = new Project();

			IIdeaObject obj = Substitute.For<IIdeaObject>();

			// Tested method
			int id1 = project.GetIomId(obj);
			int id2 = project.GetIomId(obj);

			Assert.That(id1, Is.EqualTo(id2));
		}

		[Test]
		public void GetIomIdIIdeaObject_DifferentObjectWithSameBimApiId_ReturnsSameIomId()
		{
			// Setup: two different IIdeaObject with the same Id
			Project project = new Project();

			IIdeaObject obj1 = Substitute.For<IIdeaObject>();
			obj1.Id.Returns("id");
			IIdeaObject obj2 = Substitute.For<IIdeaObject>();
			obj2.Id.Returns("id");

			// Tested method
			int id1 = project.GetIomId(obj1);
			int id2 = project.GetIomId(obj2);

			Assert.That(id1, Is.EqualTo(id2));
		}

		[Test]
		public void GetIomIdIIdeaObject_DifferentObjectWithDifferentBimApiId_ReturnsDifferentIomId()
		{
			// Setup: two different IIdeaObject with different Id
			Project project = new Project();

			IIdeaObject obj1 = Substitute.For<IIdeaObject>();
			obj1.Id.Returns("id1");
			IIdeaObject obj2 = Substitute.For<IIdeaObject>();
			obj2.Id.Returns("id2");

			// Tested method
			int id1 = project.GetIomId(obj1);
			int id2 = project.GetIomId(obj2);

			Assert.That(id1, Is.Not.EqualTo(id2));
		}

		[Test]
		public void GetIomIdStringId_ReturnsIdCreatedByGetIomIdIIdeaObject()
		{
			// Setup
			Project project = new Project();

			IIdeaObject obj = Substitute.For<IIdeaObject>();
			obj.Id.Returns("id1");
			int idNew = project.GetIomId(obj);

			// Tested method
			int id = project.GetIomId(obj);

			Assert.That(id, Is.EqualTo(idNew));
		}
	}
}