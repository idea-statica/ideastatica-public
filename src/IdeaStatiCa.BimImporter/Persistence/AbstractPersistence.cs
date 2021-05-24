using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// Basic implementaion of <see cref="IPersistence"/>. Stores mappings and tokes in hash tables.
	/// </summary>
	public abstract class AbstractPersistence : IPersistence
	{
		protected HashSet<(int, string)> Mappings { get; set; } = new HashSet<(int, string)>();
		protected HashSet<IIdeaPersistenceToken> Tokens { get; set; } = new HashSet<IIdeaPersistenceToken>();

		public IEnumerable<(int, string)> GetMappings() => Mappings;

		public IEnumerable<IIdeaPersistenceToken> GetTokens() => Tokens;

		///<inheritdoc cref="IPersistence.StoreMapping(int, string)"/>
		///<exception cref="ArgumentException">Throws if the mapping has already been stored before.</exception>
		public void StoreMapping(int iomId, string bimApiId)
		{
			if (!Mappings.Add((iomId, bimApiId)))
			{
				throw new ArgumentException("The mapping already stored.");
			}
		}

		///<inheritdoc cref="IPersistence.StoreToken(IIdeaPersistenceToken)"/>
		///<exception cref="ArgumentNullException">Throws if <paramref name="token"/> is null.</exception>
		///<exception cref="ArgumentException">Throws if the token has already been stored before.</exception>
		public void StoreToken(IIdeaPersistenceToken token)
		{
			if (token is null)
			{
				throw new ArgumentNullException(nameof(token));
			}

			if (!Tokens.Add(token))
			{
				throw new ArgumentException("The token is already stored.", nameof(token));
			}
		}
	}
}