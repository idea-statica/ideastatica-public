using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// Stores id mappings and tokens.
	/// </summary>
	public interface IPersistence
	{
		/// <summary>
		/// Get all stored id mappings.
		/// </summary>
		/// <returns>Id mappings</returns>
		IEnumerable<(int, string)> GetMappings();

		/// <summary>
		/// Store an id mapping.
		/// </summary>
		/// <param name="iomId">IOM id</param>
		/// <param name="bimApiId">BimApi id</param>
		void StoreMapping(int iomId, string bimApiId);

		/// <summary>
		/// Get all stored tokens.
		/// </summary>
		/// <returns>Tokens</returns>
		IEnumerable<(string, IIdeaPersistenceToken)> GetTokens();

		/// <summary>
		/// Store a token.
		/// </summary>
		/// <param name="bimApiId">BimApi id</param>
		/// <param name="token">Persistence token</param>
		void StoreToken(string bimApiId, IIdeaPersistenceToken token);

		/// <summary>
		/// Occurs when data are loaded or changed by an external process.
		/// </summary>
		event Action DataLoaded;
	}
}