using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter.Persistence
{
	public interface IObjectRestorer
	{
		IIdeaPersistentObject Restore(IIdeaPersistenceToken token);
	}
}