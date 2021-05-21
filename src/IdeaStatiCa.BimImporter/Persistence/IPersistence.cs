using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Persistence
{
	public interface IPersistence
	{
		IEnumerable<(int, string)> GetMappings();

		void StoreMapping(int iomId, string bimApiId);

		IEnumerable<IIdeaPersistenceToken> GetTokens();

		void StoreToken(IIdeaPersistenceToken serializable);
	}
}