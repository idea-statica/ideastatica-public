namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Describes an <see cref="IIdeaPersistentObject"/> object and hold all information neccessery for restoration of the object.
	/// All implementations must be serializable.
	/// </summary>
	public interface IIdeaPersistenceToken
	{
		string Id { get; }
	}
}