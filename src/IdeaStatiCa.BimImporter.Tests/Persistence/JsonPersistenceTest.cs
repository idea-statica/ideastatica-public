using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using NSubstitute;
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
		private IPluginLogger logger;

		[SetUp]
		public void SetUp()
		{
			logger = Substitute.For<IPluginLogger>();
			jsonPersistence = new JsonPersistence(logger);
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
			jsonPersistence.StoreToken("member-1", testToken);
			jsonPersistence.StoreMapping(1, "member-1");

			// Serialize data
			StringWriter writter = new StringWriter();			
			jsonPersistence.Save(writter);

			// Deserialize data
			StringReader reader = new StringReader(writter.ToString());
			jsonPersistence.Load(reader);

			// Check the data is valid
			List<(string, IIdeaPersistenceToken)> tokens = jsonPersistence.GetTokens().ToList();
			Assert.That(tokens.Count, Is.EqualTo(1));
			Assert.That(tokens[0].Item1, Is.EqualTo("member-1"));
			Assert.That(tokens[0].Item2, Is.InstanceOf<TestToken>());
			Assert.That(((TestToken)tokens[0].Item2).TestProperty, Is.EqualTo("123456"));
			Assert.That(((TestToken)tokens[0].Item2).Type, Is.EqualTo(TokenObjectType.Member));

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
			jsonPersistence.StoreToken("member-1", testToken);
			jsonPersistence.StoreMapping(1, "member-1");

			// Serialize data
			StringWriter writter = new StringWriter();
			jsonPersistence.Save(writter);

			// Deserialize data
			JsonPersistence jsonPersistence2 = new JsonPersistence(logger);
			StringReader reader = new StringReader(writter.ToString());
			jsonPersistence2.Load(reader);

			// Check the data is valid
			List<(string, IIdeaPersistenceToken)> tokens = jsonPersistence2.GetTokens().ToList();
			Assert.That(tokens.Count, Is.EqualTo(1));
			Assert.That(tokens[0].Item1, Is.EqualTo("member-1"));
			Assert.That(tokens[0].Item2, Is.InstanceOf<TestToken>());
			Assert.That(((TestToken)tokens[0].Item2).TestProperty, Is.EqualTo("123456"));
			Assert.That(((TestToken)tokens[0].Item2).Type, Is.EqualTo(TokenObjectType.Member));

			List<(int, string)> mappings = jsonPersistence.GetMappings().ToList();
			Assert.That(mappings.Count, Is.EqualTo(1));
			Assert.That(mappings[0].Item1, Is.EqualTo(1));
			Assert.That(mappings[0].Item2, Is.EqualTo("member-1"));
		}

		[Test]
		[TestCase("../../../TestData/bimapi-data-serialized-with-none.json")]
		[TestCase("../../../TestData/bimapi-data-serialized-with-auto.json")]
		[TestCase("../../../TestData/bimapi-data-serialized-with-all.json")]
		public void LoadOldVersionsOfJson(string jsonPath)
		{			
			StringReader reader = new StringReader(File.ReadAllText(jsonPath));
			jsonPersistence.Load(reader);

			// Check the data is valid
			List<(string, IIdeaPersistenceToken)> tokens = jsonPersistence.GetTokens().ToList();
			Assert.That(tokens.Count, Is.EqualTo(72));
			List<(int, string)> mappings = jsonPersistence.GetMappings().ToList();
			Assert.That(mappings.Count, Is.EqualTo(183));
		}
	}
}