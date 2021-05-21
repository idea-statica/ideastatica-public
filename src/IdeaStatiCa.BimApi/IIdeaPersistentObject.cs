namespace IdeaStatiCa.BimApi
{
	public interface IIdeaPersistentObject : IIdeaObject
	{
		IIdeaPersistenceToken Token { get; }
	}
}