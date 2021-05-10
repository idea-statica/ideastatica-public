using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
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
			Project project = new Project(new NullLogger());
			Assert.That(() => project.GetIomId("non existent it"), Throws.InstanceOf<KeyNotFoundException>());
		}

		[Test]
		public void GetBimObject_IfMappingDoesNotExist_ThrowKeyNotFoundException()
		{
			Project project = new Project(new NullLogger());
			Assert.That(() => project.GetBimObject(1), Throws.InstanceOf<KeyNotFoundException>());
		}

		[Test]
		public void GetIomIdIIdeaObject_IfMappingDoesNotExist_ThrowsNothing()
		{
			Project project = new Project(new NullLogger());

			int id = project.GetIomId(Substitute.For<IIdeaObject>());
			Assert.That(() => project.GetIomId(Substitute.For<IIdeaObject>()), Throws.Nothing);
		}

		[Test]
		public void GetIomIdIIdeaObject_SameObject_ReturnsSameIomId()
		{
			// Setup
			Project project = new Project(new NullLogger());

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
			Project project = new Project(new NullLogger());

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
			Project project = new Project(new NullLogger());

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
			Project project = new Project(new NullLogger());

			IIdeaObject obj = Substitute.For<IIdeaObject>();
			obj.Id.Returns("id1");
			int idNew = project.GetIomId(obj);

			// Tested method
			int id = project.GetIomId(obj);

			Assert.That(id, Is.EqualTo(idNew));
		}

		[Test]
		public void GetIomIdIIdeaObject_AfterLoad_ReturnsLoadedIdMapping()
		{
			// Setup: existing conversion table with mapping bimapi id 'css' -> iom id 1
			Project project = new Project(new NullLogger());
			ConversionDictionaryString conversionTable = new ConversionDictionaryString();
			conversionTable.Add("css", 1);

			IIdeaCrossSection css = Substitute.For<IIdeaCrossSection>();
			css.Id.Returns("css");

			project.Load(Substitute.For<IGeometry>(), conversionTable);

			// Tested method: get id for object with id 'css'
			int id = project.GetIomId(css);

			// Assert
			Assert.That(id, Is.EqualTo(1));
		}

		[Test]
		public void GetIomIdIIdeaObject_AfterLoad_DoesNotReturnAlreadyExistingId()
		{
			// Setup: existing conversion table with mapping bimapi id 'css' -> iom id 1
			Project project = new Project(new NullLogger());
			ConversionDictionaryString conversionTable = new ConversionDictionaryString();
			conversionTable.Add("css", 1);

			// Load prepared conversion table
			project.Load(Substitute.For<IGeometry>(), conversionTable);

			IIdeaObject obj = Substitute.For<IIdeaObject>();
			obj.Id.Returns("id1");

			// Tested method: create new mapping for an object
			int idNew = project.GetIomId(obj);

			// Assert
			Assert.That(idNew, Is.Not.EqualTo(1));
		}

		[Test]
		public void GetBimObject_AfterLoad_ReturnsTheCorrectObject()
		{
			// Setup: existing conversion table with mapping bimapi id 'member' -> iom id 1
			Project project = new Project(new NullLogger());
			ConversionDictionaryString conversionTable = new ConversionDictionaryString();
			conversionTable.Add("member", 1);

			// Prepare a member with id 'member'
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.Id.Returns("member");

			// Prepare a geometry with the member
			IGeometry geometry = Substitute.For<IGeometry>();
			geometry.GetMembers().Returns(new List<IIdeaMember1D>() { member });

			// Load prepared conversion table
			project.Load(geometry, conversionTable);

			// Tested method: get object for iom id 1
			IIdeaObject obj = project.GetBimObject(1);

			// Assert
			Assert.That(obj, Is.EqualTo(member));
		}

		[Test]
		public void GetIomIdIIdeaObject_AfterLoad_ReturnsTheCorrectId()
		{
			// Setup: existing conversion table with mapping bimapi id 'member' -> iom id 1
			Project project = new Project(new NullLogger());
			ConversionDictionaryString conversionTable = new ConversionDictionaryString();
			conversionTable.Add("member", 1);

			// Prepare a member with id 'member'
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.Id.Returns("member");

			// Prepare a geometry with the member
			IGeometry geometry = Substitute.For<IGeometry>();
			geometry.GetMembers().Returns(new List<IIdeaMember1D>() { member });

			// Load prepared conversion table
			project.Load(geometry, conversionTable);

			// Tested method: get id for the member
			int id = project.GetIomId(member);

			// Assert
			Assert.That(id, Is.EqualTo(1));
		}
	}
}