using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter.Persistence
{
	public abstract class AbstractPersistenceToken : IIdeaPersistenceToken
	{
		public TokenObjectType Type { get; set; }
	}
}