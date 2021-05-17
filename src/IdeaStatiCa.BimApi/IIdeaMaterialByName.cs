namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A material defined by <see cref="IIdeaObject.Name"/>.
	/// </summary>
	public interface IIdeaMaterialByName : IIdeaMaterial
	{
		/// <summary>
		/// Material type
		/// </summary>
		MaterialType MaterialType { get; }
	}
}