namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represents named and identifiable node that several members can connect to.
	/// </summary>
	public interface IIdeaNode : IIdeaPersistentObject
	{
		IdeaVector3D Vector { get; }
	}
}