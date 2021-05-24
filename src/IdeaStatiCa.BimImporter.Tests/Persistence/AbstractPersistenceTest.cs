using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Persistence;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests.Persistence
{
	[TestFixture]
	public class AbstractPersistenceTest
	{
		private AbstractPersistence abstractPersistence;

		private class TestToken : AbstractPersistenceToken
		{
			public string TestProperty { get; set; }
		}

		private bool JsonComparer<T>(T actual, T expected)
		{
			return JToken.DeepEquals(JToken.FromObject(actual), JToken.FromObject(expected));
		}

		[SetUp]
		public void SetUp()
		{
			abstractPersistence = Substitute.For<AbstractPersistence>();
		}

		[Test]
		public void GetMappings_IfNothingWasAdded_ReturnsEmptyEnumerable()
		{
			// Tested method
			IEnumerable<(int, string)> result = abstractPersistence.GetMappings();

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetMappings_ReturnsItemsAddedByStoreMapping()
		{
			// Setup: add some mappings
			abstractPersistence.StoreMapping(2, "a");
			abstractPersistence.StoreMapping(1, "b");
			abstractPersistence.StoreMapping(3, "c");

			// Tested method
			IEnumerable<(int, string)> result = abstractPersistence.GetMappings();

			// Assert
			Assert.That(result, Is.EquivalentTo(new List<(int, string)> {
				(1, "b"),
				(2, "a"),
				(3, "c")
			}));
		}

		[Test]
		public void StoreMapping_IfAddingExistingMapping_ThrowsArgumentException()
		{
			// Setup
			abstractPersistence.StoreMapping(2, "a");

			// Assert: expect exception
			Assert.That(() => abstractPersistence.StoreMapping(2, "a"), Throws.InstanceOf<ArgumentException>());
		}

		[Test]
		public void GetTokens_IfNothingWasAdded_ReturnsEmptyEnumerable()
		{
			// Tested method
			IEnumerable<IIdeaPersistenceToken> result = abstractPersistence.GetTokens();

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetTokens_ReturnsItemsAddedByStoreToken()
		{
			// Setup: add some mappings
			TestToken token1 = new TestToken() { TestProperty = "abc", Type = TokenObjectType.Member };
			TestToken token2 = new TestToken() { TestProperty = "xyz", Type = TokenObjectType.Node };
			TestToken token3 = new TestToken() { TestProperty = "mno", Type = TokenObjectType.Member };

			abstractPersistence.StoreToken(token2);
			abstractPersistence.StoreToken(token1);
			abstractPersistence.StoreToken(token3);

			// Tested method
			IEnumerable<IIdeaPersistenceToken> result = abstractPersistence.GetTokens();

			// Assert: check using json because we don't care about equal references but the content of the classes
			Assert.That(result, Is.EquivalentTo(new List<IIdeaPersistenceToken> {
				token1,
				token2,
				token3
			}).Using<IIdeaPersistenceToken, IIdeaPersistenceToken>(JsonComparer));
		}

		[Test]
		public void StoreToken_IfAddingExistingToken_ThrowsArgumentException()
		{
			// Setup
			TestToken token1 = new TestToken() { TestProperty = "abc", Type = TokenObjectType.Member };
			abstractPersistence.StoreToken(token1);

			// Assert: expect exception
			Assert.That(() => abstractPersistence.StoreToken(token1), Throws.InstanceOf<ArgumentException>());
		}
	}
}