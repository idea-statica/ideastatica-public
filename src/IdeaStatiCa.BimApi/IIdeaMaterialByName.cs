namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A material to import by name, any name is allowed.
	/// The correct material is resolved by the user in CCM/Checkbot.
	/// <see cref="IIdeaObject.Name"/> must not be null.
	/// </summary>
	public interface IIdeaMaterialByName : IIdeaMaterial
	{
		/// <summary>
		/// Material type
		/// </summary>
		MaterialType MaterialType { get; }
	}
}