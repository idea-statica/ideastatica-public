using IdeaStatiCa.BimImporter.Persistence;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Tests.Persistence
{
	[TestFixture]
	internal class JsonPersistenceTest
	{
		private JsonPersistence jsonPersistence;

		[SetUp]
		public void SetUp()
		{
			jsonPersistence = new JsonPersistence();
		}

		private class TestToken : AbstractPersistenceToken
		{
			public string TestProperty { get; set; }
		}

		[Test]
		public void SaveAndLoad_SameJsonPersistenceInstance()
		{
			TestToken testToken = new TestToken()
			{
				TestProperty = "123456",
				Type = TokenObjectType.Member
			};

			// Store the token
			jsonPersistence.StoreToken(testToken);
			jsonPersistence.StoreMapping(1, "member-1");

			// Serialize data
			StringWriter writter = new StringWriter();
			jsonPersistence.Save(writter);

			// Deserialize data
			StringReader reader = new StringReader(writter.ToString());
			jsonPersistence.Load(reader);

			// Check the data is valid
			List<BimApi.IIdeaPersistenceToken> tokens = jsonPersistence.GetTokens().ToList();
			Assert.That(tokens.Count, Is.EqualTo(1));
			Assert.That(tokens[0], Is.InstanceOf<TestToken>());
			Assert.That(((TestToken)tokens[0]).TestProperty, Is.EqualTo("123456"));
			Assert.That(((TestToken)tokens[0]).Type, Is.EqualTo(TokenObjectType.Member));

			List<(int, string)> mappings = jsonPersistence.GetMappings().ToList();
			Assert.That(mappings.Count, Is.EqualTo(1));
			Assert.That(mappings[0].Item1, Is.EqualTo(1));
			Assert.That(mappings[0].Item2, Is.EqualTo("member-1"));
		}

		[Test]
		public void SaveAndLoad_DifferentJsonPersistenceInstance()
		{
			TestToken testToken = new TestToken()
			{
				TestProperty = "123456",
				Type = TokenObjectType.Member
			};

			// Store the token
			jsonPersistence.StoreToken(testToken);
			jsonPersistence.StoreMapping(1, "member-1");

			// Serialize data
			StringWriter writter = new StringWriter();
			jsonPersistence.Save(writter);

			// Deserialize data
			JsonPersistence jsonPersistence2 = new JsonPersistence();
			StringReader reader = new StringReader(writter.ToString());
			jsonPersistence2.Load(reader);

			// Check the data is valid
			List<BimApi.IIdeaPersistenceToken> tokens = jsonPersistence2.GetTokens().ToList();
			Assert.That(tokens.Count, Is.EqualTo(1));
			Assert.That(tokens[0], Is.InstanceOf<TestToken>());
			Assert.That(((TestToken)tokens[0]).TestProperty, Is.EqualTo("123456"));
			Assert.That(((TestToken)tokens[0]).Type, Is.EqualTo(TokenObjectType.Member));

			List<(int, string)> mappings = jsonPersistence.GetMappings().ToList();
			Assert.That(mappings.Count, Is.EqualTo(1));
			Assert.That(mappings[0].Item1, Is.EqualTo(1));
			Assert.That(mappings[0].Item2, Is.EqualTo("member-1"));
		}
	}
}