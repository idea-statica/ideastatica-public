using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// Base implementation of <see cref="IIdeaPersistenceToken"/>.
	/// </summary>
	public abstract class AbstractPersistenceToken : IIdeaPersistenceToken
	{
		public string Id { get; set; }

		/// <summary>
		/// Type of the object
		/// </summary>
		public TokenObjectType Type { get; set; }
	}
}