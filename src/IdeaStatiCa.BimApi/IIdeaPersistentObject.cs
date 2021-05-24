namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// An object that can be recreated at any point by its persistence <see cref="Token"/>.
	/// </summary>
	public interface IIdeaPersistentObject : IIdeaObject
	{
		/// <summary>
		/// Persistence token. Holds data neccessery to recreated the object in future.
		/// </summary>
		IIdeaPersistenceToken Token { get; }
	}
}