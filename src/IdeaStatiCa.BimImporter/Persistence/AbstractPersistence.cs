using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Persistence
{
	public abstract class AbstractPersistence : IPersistence
	{
		protected HashSet<(int, string)> Mappings { get; set; } = new HashSet<(int, string)>();
		protected HashSet<IIdeaPersistenceToken> Tokens { get; set; } = new HashSet<IIdeaPersistenceToken>();

		public IEnumerable<(int, string)> GetMappings() => Mappings;

		public IEnumerable<IIdeaPersistenceToken> GetTokens() => Tokens;

		public void StoreMapping(int iomId, string bimApiId)
		{
			if (!Mappings.Add((iomId, bimApiId)))
			{
				throw new ArgumentException("The mapping already stored.");
			}
		}

		public void StoreToken(IIdeaPersistenceToken token)
		{
			if (!Tokens.Add(token))
			{
				throw new ArgumentException("The token is already stored.", nameof(token));
			}
		}
	}
}