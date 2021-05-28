﻿using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// Basic implementation of <see cref="IPersistence"/>. Stores mappings and tokens in a hash table.
	/// </summary>
	public abstract class AbstractPersistence : IPersistence
	{
		protected HashSet<(int, string)> Mappings { get; set; } = new HashSet<(int, string)>();
		protected Dictionary<string, IIdeaPersistenceToken> Tokens { get; set; } = new Dictionary<string, IIdeaPersistenceToken>();

		public IEnumerable<(int, string)> GetMappings() => Mappings;

		public IEnumerable<(string, IIdeaPersistenceToken)> GetTokens() => Tokens.Select(x => (x.Key, x.Value));

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
		public void StoreToken(string bimApiId, IIdeaPersistenceToken token)
		{
			if (token is null)
			{
				throw new ArgumentNullException(nameof(token));
			}

			if (Tokens.ContainsKey(bimApiId))
			{
				throw new ArgumentException("The token is already stored.", nameof(token));
			}

			Tokens.Add(bimApiId, token);
		}
	}
}