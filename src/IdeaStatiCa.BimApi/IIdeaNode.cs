namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represents named and identifiable node that several members can connect to.
	/// </summary>
	public interface IIdeaNode : IIdeaObject, IIdeaPersistentObject
	{
		IdeaVector3D Vector { get; }
	}
}