using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class ProjectTest
	{
		private IObjectRestorer objectRestorer;
		private IPersistence persistence;
		private Project project;

		[SetUp]
		public void SetUp()
		{
			objectRestorer = Substitute.For<IObjectRestorer>();
			persistence = Substitute.For<IPersistence>();
			project = new Project(new NullLogger(), persistence, objectRestorer);
		}

		[Test]
		public void GetIomIdStringId_IfMappingDoesNotExist_ThrowKeyNotFoundException()
		{
			Assert.That(() => project.GetIomId("non existent it"), Throws.InstanceOf<KeyNotFoundException>());
		}

		[Test]
		public void GetBimObject_IfMappingDoesNotExist_ThrowKeyNotFoundException()
		{
			Assert.That(() => project.GetBimObject(1), Throws.InstanceOf<KeyNotFoundException>());
		}

		[Test]
		public void GetIomIdIIdeaObject_IfMappingDoesNotExist_ThrowsNothing()
		{
			int id = project.GetIomId(Substitute.For<IIdeaObject>());
			Assert.That(() => project.GetIomId(Substitute.For<IIdeaObject>()), Throws.Nothing);
		}

		[Test]
		public void GetIomIdIIdeaObject_SameObject_ReturnsSameIomId()
		{
			// Setup
			IIdeaObject obj = Substitute.For<IIdeaObject>();

			// Tested method
			int id1 = project.GetIomId(obj);
			int id2 = project.GetIomId(obj);

			Assert.That(id1, Is.EqualTo(id2));
		}

		[Test]
		public void GetIomIdIIdeaObject_ReturnsIdGreaterThan0()
		{
			// Setup
			IIdeaObject obj = Substitute.For<IIdeaObject>();

			// Tested method
			int id = project.GetIomId(obj);

			Assert.That(id, Is.GreaterThan(0));
		}

		[Test]
		public void GetIomIdIIdeaObject_DifferentObjectWithSameBimApiId_ReturnsSameIomId()
		{
			// Setup: two different IIdeaObject with the same Id
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
			// Setup: mapping bimapi id 'css' -> iom id 1
			persistence.GetMappings().Returns(new List<(int, string)>() {
				(1, "css")
			});

			IIdeaCrossSection css = Substitute.For<IIdeaCrossSection>();
			css.Id.Returns("css");

			// Reload data
			persistence.DataLoaded += Raise.Event<Action>();

			// Tested method: get id for object with id 'css'
			int id = project.GetIomId(css);

			// Assert
			Assert.That(id, Is.EqualTo(1));
		}

		[Test]
		public void GetIomIdIIdeaObject_AfterLoad_DoesNotReturnAlreadyExistingId()
		{
			// Setup: mapping 'css1' -> 0, 'css2' -> 1
			persistence.GetMappings().Returns(new List<(int, string)>() {
				(1, "css1"),
				(2, "css2"),
			});

			IIdeaObject obj = Substitute.For<IIdeaObject>();
			obj.Id.Returns("id1");

			// Reload data
			persistence.DataLoaded += Raise.Event<Action>();

			// Tested method: create new mapping for an object
			int idNew = project.GetIomId(obj);

			// Assert
			Assert.That(idNew, Is.Not.EqualTo(0).Or.Not.EqualTo(1));
		}

		[Test]
		public void GetBimObject_AfterLoad_ReturnsTheCorrectObject()
		{
			// Setup: mapping bimapi id 'member' -> iom id 1
			persistence.GetMappings().Returns(new List<(int, string)>() {
				(1, "member"),
			});

			IIdeaPersistenceToken token = Substitute.For<IIdeaPersistenceToken>();
			persistence.GetTokens().Returns(new List<(string, IIdeaPersistenceToken)>()
			{
				("member", token)
			});

			// Prepare a member with id 'member'
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.Id.Returns("member");

			// Prepare an object restorer for the member
			objectRestorer.Restore(token).Returns(member);

			// Reload data
			persistence.DataLoaded += Raise.Event<Action>();

			// Tested method: get object for iom id 1
			IIdeaObject obj = project.GetBimObject(1);

			// Assert
			Assert.That(obj, Is.EqualTo(member));
		}

		[Test]
		public void GetIomIdIIdeaObject_AfterLoad_ReturnsTheCorrectId()
		{
			// Setup: mapping bimapi id 'member' -> iom id 1
			persistence.GetMappings().Returns(new List<(int, string)>() {
				(1, "member"),
			});

			IIdeaPersistenceToken token = Substitute.For<IIdeaPersistenceToken>();
			persistence.GetTokens().Returns(new List<(string, IIdeaPersistenceToken)>()
			{
				("member", token)
			});

			// Prepare a member with id 'member'
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.Id.Returns("member");

			// Prepare an object restorer for the member
			objectRestorer.Restore(token).Returns(member);

			// Reload data
			persistence.DataLoaded += Raise.Event<Action>();

			// Tested method: get id for the member
			int id = project.GetIomId(member);

			// Assert
			Assert.That(id, Is.EqualTo(1));
		}

		[Test]
		public void GetGetIomId_AfterLoad_ReturnsNonCollidingId()
		{
			// Setup: stored id mapping 'obj1' -> 1
			persistence.GetMappings().Returns(new List<(int, string)>() {
				(1, "obj1"),
			});

			// Prepare a new fresh object
			IIdeaObject obj = Substitute.For<IIdeaObject>();
			obj.Id.Returns("obj2");

			// Reload data
			persistence.DataLoaded += Raise.Event<Action>();

			// Tested method: get a new id for the new object
			int id = project.GetIomId(obj);

			// Assert
			Assert.That(id, Is.Not.EqualTo(1));
		}

		[Test]
		public void GetPersistenceToken_ReturnsTokenOfFreshlyCreatedObject()
		{
			// Setup: create a member and its persistence token
			IIdeaPersistenceToken token = Substitute.For<IIdeaPersistenceToken>();
			persistence.GetTokens().Returns(new List<(string, IIdeaPersistenceToken)>()
			{
				("member", token)
			});

			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.Id.Returns("member");
			member.Token.Returns(token);

			// Get a new IOM id for the member
			int id = project.GetIomId(member);

			// Tested method: retrieve the persistence token by the IOM id
			IIdeaPersistenceToken returnedToken = project.GetPersistenceToken(id);

			// Assert: check that tokens equal
			Assert.That(returnedToken, Is.EqualTo(token));
		}

		[Test]
		public void GetPersistenceToken_ReturnsTokenOfLoadedObject()
		{
			// Setup: create a stored member and its persistence token
			persistence.GetMappings().Returns(new List<(int, string)>() {
				(1, "member"),
			});

			IIdeaPersistenceToken token = Substitute.For<IIdeaPersistenceToken>();
			persistence.GetTokens().Returns(new List<(string, IIdeaPersistenceToken)>()
			{
				("member", token)
			});

			// Prepare a member with id 'member'
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.Id.Returns("member");
			member.Token.Returns(token);

			// Prepare an object restorer for the member
			objectRestorer.Restore(token).Returns(member);

			// Reload data
			persistence.DataLoaded += Raise.Event<Action>();

			// Tested method: retrieve the persistence token by the IOM id
			IIdeaPersistenceToken returnedToken = project.GetPersistenceToken(1);

			// Assert: check that tokens equal
			Assert.That(returnedToken, Is.EqualTo(token));
		}
	}
}