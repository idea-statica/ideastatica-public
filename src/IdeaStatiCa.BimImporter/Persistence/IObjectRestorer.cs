using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// Object restorer recreates objects from their persistence tokens.
	/// </summary>
	public interface IObjectRestorer
	{
		/// <summary>
		/// Creates an instance based on a given persistence <paramref name="token"/>.
		/// </summary>
		/// <param name="token">Persistence token</param>
		/// <returns>Instance of <see cref="IIdeaPersistentObject"/></returns>
		IIdeaPersistentObject Restore(IIdeaPersistenceToken token);
	}
}